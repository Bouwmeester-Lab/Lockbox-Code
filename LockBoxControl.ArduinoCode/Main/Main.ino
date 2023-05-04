#include <string.h>
#include <math.h>
#include <IntervalTimer.h>

// custom libraries
#include "LPF2.h"
#include "ad5791.h"
#include "FilterBuLp2.h"
#include "ethernetCommunication.h"
#include "SerialCommand.h"
#include "dac.h"

// preprocessor definitions
#define Tsample 30 //sample time for timer in microseconds
#define pi 3.14159

// constants

// pin definitions
DAC dac1(A0, A1, A2, A3);
DAC dac2(A4, A5, A6, A7);
// const uint8_t reset1 = A0;
// const uint8_t clr1   = A1;
// const uint8_t ldac1  = A2;
// const uint8_t sync1  = A3;

// const uint8_t reset2 = A4;
// const uint8_t clr2   = A5;
// const uint8_t ldac2  = A6;
// const uint8_t sync2  = A7;

// uint8_t reset = reset1;
// uint8_t clr   = clr1;
// uint8_t ldac  = ldac1;
// uint8_t sync  = sync1;

// constants frequencies

const unsigned long oldCtrl_c = 2097152;
const float lpf2_freq1 = 100;
const float lpf2_freq2 = 15;



int r = 0; //index used for sine
int Refl =0;
float error = 0;
float errorsum = 0;
float olderror = 0;
float C = 0; //DC correction in 12-bit units


const int samplefreq = 1000000/Tsample; //sampling freq for timer in Hertz




//-----------------------------------------------------------------------

IntervalTimer myTimer;



FilterBuLp2 LPF;
LPF2 lpf2_1(lpf2_freq1);
LPF2 lpf2_2(lpf2_freq2);


volatile boolean flag = false;
void flagpost(){
  flag = true;
}

//For sinewaves
const int freq = 3000; //freq of sinewave in Hertz 
float ampl =0.001;//Pk-Pk amplitude of sinewave in Volt 
//Scaling is a bit off. Accurate for 0.5-2.0V, 0.025 will give 40mV
const int sinetablesize = (samplefreq -(samplefreq%freq))/freq;
static int sinetable[36][sinetablesize]={};

float AC_ampl_bits = 0;
float dV = 0;
float Volt=0;
unsigned long volt_start; // why was this a float?????????
bool flag_dV_reduced = false;
int monitor_shift = 0;

const int max_bits=524288-1;


double volt_limit_down = 1000;
double volt_limit_up = 500000;



unsigned long DAC2_offset = 250000;//find the proper voltage by delta source, why was this a float?????
const double abs_volt_limit_up = 500000; // limits to protect piezo
const double abs_volt_limit_down = 100;
//PDH LPF 1.28kHz

//works best with 1 kHz ZI LPF
//float P = 1.0;   //0.5
//float I = 0.1;  //0.4
//float D = 0.0; //0.15

//works best with 9 Hz ZI LPF, up to 3 Hz
//float P = 0.5;   //0.5
//float I = 0.1;  //0.4

//140 V on membrane piezo params: minimal I=60n

//float P = 0.1;   //0.5
//float I = 2.0;  //320 -> 250 Hz in ZI LPF min; 32 -> 15 Hz in ZI LPF min
//float D = 0.05; //0.15

/////////////////////////////////////////////////////////////////////////////////////////////////////

float setpoint = 0.;
int phi = 8;
float P = 0.0;   //0.5
float I = 25.0;  //320 -> 250 Hz in ZI LPF min; 32 -> 15 Hz in ZI LPF min
float D = 0.0; //0.15

float Refl_max=2000;
float Lock_trigger=1600;

//////////////////////////////////////////////////////////////////////////////////////////////////////


void setup() 
{
  pinMode(A8, INPUT);

  Serial.begin(38400);
  SPI.begin();

  dac1.setUpDacPins();
  dac2.setUpDacPins();
  
  SPI.beginTransaction(SPISettings(3800000, MSBFIRST, SPI_MODE1));
     
  dac1.getCtrlValue();  // we get the ctrl register value from the first dac.
  unsigned long status = dac1.getCtrlValue(); // we get it again. Is it different? no idea.

  status = status & ~(AD5791_CTRL_LINCOMP(-1) | AD5791_CTRL_SDODIS | AD5791_CTRL_BIN2SC | AD5791_CTRL_RBUF | AD5791_CTRL_OPGND);
  
  dac1.initializeDac(status); // aren't we undoing what we did here in the next line??
  
  dac1.initializeDac(oldCtrl_c); // operation on dac1

  dac2.initializeDac(oldCtrl_c); // on dac2

  //---------------------------------------
  analogReadResolution(12); // 1 point corresponds to 3.3V/2^(12) 
  analogReadAveraging(3);
  
  myTimer.begin(flagpost, Tsample); //reset flag to true every Tsample
  delay(1000);
  
  AC_ampl_bits = max_bits*ampl/5;
  volt_start = AC_ampl_bits;
  Volt = 0;
  dV = 1.0/500.0;       

  for (int h=0;h<36;h++) //sinetable is generated
    {
      for (int j =0; j<sinetablesize; j++)
      {
        sinetable[h][j] = max_bits*(ampl*sin(j*2*pi/sinetablesize+2*h*pi/36))/10;
      }
    }

  //switch to DAC2 and initialize, the lines marked useless will be removed. This is because these variables are not used by AD5791_SetRegisterValue or AD5791_SetRegisterValue
  dac2.initializeDac(oldCtrl_c); // we initialize again, why? no idea, it's just what the code has...
  dac2.setOutputVoltage(DAC2_offset); // DC offset for DAC2     

  // initialize DAC1  the lines marked useless will be removed. This is because these variables are not used by AD5791_SetRegisterValue or AD5791_SetRegisterValue
  volt_start=250000;//scan from 5V
  dac1.initializeDac(oldCtrl_c); // again???
  
  Serial.println("initialized");
  delay(1000);
}

int engage = 0; //switch involved in tranfer between scan and feedback

int print_index = 0;
bool maximum_reached = false;
float flag_monitor = 0;
bool scanning_done = true;   //if false, pid on after scanning
bool freeze = true;
int time_index = 0;
int state_flag = 0;

//freeze parameters
int pid_on_loops = 100000;    //2 sec 
int freeze_loops = 5000;    //1 sec
int freeze_multiplier = 20;  //30;
int cycle_index_freeze_extension = 30;
float I_drift = 0.15;    //pid I component for the drift
float C_end_sum = 0;
int index_loop = 0;   //index_loop * Tsample = time since the program started
int index_loop_cycle_start = 0;
float drift = 0;
int cycle_index = 0;
bool freeze_duration_increase_happened = false;

bool DAC2_finished = false;
bool DAC1_finished = false;
float volt_out_DAC2 = 1;
float dvolt_DAC2 = 0.5;
long status;
double last_resonance_volt = 0;
float dvolt_DAC1 = 0.5;
double volt_out_DAC1 = -100;
int number_scans_DAC1 = 0;
float temp_volt = 0;
float temp_volt_d = 0.0005;
int temp_flag = 0;
int n=0;
float voltoutacc=0;
float t1=0;
int volt_out = 0;
byte incomingByte = 122;

float refladd = 0;
int reflindex = 0;
float reflmean = 0;

SerialCommand command;

void loop() 
{ 
  if (Serial.available())
  {
    auto c = Serial.readStringUntil('\n');

    command = SerialCommand::serialize(c);

    if(command.isOk){
      switch(command.commandLetter){
        case 's':
          TurnOnLed(command.requestId);
          break;
        case 'l':
          TurnOffLed(command.requestId);
          break;
        case 'm':
          getMacAddress(command.requestId);
          break;
        default:
          SendError("unkown command");
          break;
      }
    }
    else{
      SendError(command.errorMessage);
    }
    incomingByte = Serial.read();  // will not be -1
    //Serial.println(incomingByte);   //  l=108; s=115;  z=122
  }

  if (flag)
  {
    flag = false;
    n++; 
    if (volt_out_DAC1 == -100)
      volt_out_DAC1 = volt_limit_down + 1;      
    Refl = analogRead(A8)*3;
    error = -lpf2_2.main(sinetable[phi][r]*Refl)/25.0 * 5;   
    //error = 2 * Refl;//14000-27000  
    //error = 2 * lpf2_1.main(Refl);//14000-27000  

    r = (r+1) % sinetablesize;
    
    AD5791_SetRegisterValue(sync, AD5791_REG_DAC, volt_out_DAC1);
  
      
///////////////////////////////////////////////////////////////
    if (incomingByte == 122)  //"z" for zero
    {
      //Serial.println("z");
        if (volt_out_DAC1 > (volt_limit_up + volt_limit_down) / 2.0 + abs(dvolt_DAC1))
        {         
            dvolt_DAC1 = -abs(dvolt_DAC1);
            volt_out_DAC1 = volt_out_DAC1 + dvolt_DAC1;
        }
        
        if (volt_out_DAC1 < (volt_limit_up + volt_limit_down) / 2.0 - abs(dvolt_DAC1))
        {
            dvolt_DAC1 = abs(dvolt_DAC1);
            volt_out_DAC1 = volt_out_DAC1 + dvolt_DAC1;
        }   
        errorsum = 0;    
        DAC1_finished = false;
    }
///////////////////////////////////////////////////////////////  
    if (incomingByte == 115)  //"s" for scan
    {   
              
        if(volt_out_DAC1 > volt_limit_up || volt_out_DAC1 < volt_limit_down)        
           dvolt_DAC1 = -dvolt_DAC1;             
          
        volt_out_DAC1 = volt_out_DAC1 + dvolt_DAC1 + sinetable[phi][r];
        errorsum = 0;
        DAC1_finished = false;
    }
    ///////////////////////////////////////////////////////////////  
    if (incomingByte == 100)  //"d" for down, decreases DAC2 voltage
    {
        float DAC2_step=20000;
        if(DAC2_offset > DAC2_step)
        {
            DAC2_offset = DAC2_offset - DAC2_step;
            ldac=ldac2;
            reset=reset2;
            clr=clr2;
            sync=sync2; 
            status = AD5791_SetRegisterValue(sync, AD5791_REG_CTRL, oldCtrl_c);      
            AD5791_SetRegisterValue(sync, AD5791_REG_DAC, DAC2_offset);

              // initialize DAC1            
            ldac=ldac1;
            reset=reset1;
            clr=clr1;
            sync=sync1; 
            status = AD5791_SetRegisterValue(sync, AD5791_REG_CTRL, oldCtrl_c);     
        }                 
    }
    ///////////////////////////////////////////////////////////////  
    if (incomingByte == 117)  //"u" for up, increases DAC2 voltage
    {
        float DAC2_step=20000;
        if(DAC2_offset < 500000 - DAC2_step)
        {
            DAC2_offset = DAC2_offset + DAC2_step;
            ldac=ldac2;
            reset=reset2;
            clr=clr2;
            sync=sync2; 
            status = AD5791_SetRegisterValue(sync, AD5791_REG_CTRL, oldCtrl_c);      
            AD5791_SetRegisterValue(sync, AD5791_REG_DAC, DAC2_offset);

              // initialize DAC1            
            ldac=ldac1;
            reset=reset1;
            clr=clr1;
            sync=sync1; 
            status = AD5791_SetRegisterValue(sync, AD5791_REG_CTRL, oldCtrl_c);     
        }                 
    }
///////////////////////////////////////////////////////////////   
    if (incomingByte == 108 && DAC1_finished == false ) //lock
    {
        if(volt_out_DAC1 > volt_limit_up || volt_out_DAC1 < volt_limit_down)        
           dvolt_DAC1 = -dvolt_DAC1; 
        volt_out_DAC1 = volt_out_DAC1 + dvolt_DAC1 + sinetable[phi][r];
  
     //////////////////once the resonace found, scan a small range/////////////////
          if (Refl < Lock_trigger)  //last_resonance_volt initialized 0
          {  
              Serial.println("resonance found");
              //DAC1_finished = true;
              //dvolt_DAC1 = dvolt_DAC1 / 8.0;
              dvolt_DAC1 = 0.1;
              last_resonance_volt = volt_out_DAC1;
              volt_limit_up = last_resonance_volt + 10000;
              volt_limit_down = last_resonance_volt - 10000;                                                       
          }

          if (Refl < Lock_trigger/2.0 && abs(error) < 200)  //last_resonance_volt initialized 0
          {  
              Serial.println("locking");
              DAC1_finished = true;
              //dvolt_DAC1 = dvolt_DAC1 / 8.0;
              last_resonance_volt = volt_out_DAC1;                                                       
          }
    }
///////////////////////////////////////////////////////////////
    if (incomingByte == 108 && DAC1_finished == true ) //lock    
    {      
      engage = 1;
      flag = false;
      scanning_done = true;
      
      if (Refl > Refl_max)
      {
        DAC1_finished = false; // ?????? this is a bool... DAC1_finished == false is wrong. I guess you meant to make dac1_finished = false
        volt_limit_up=abs_volt_limit_up;
        volt_limit_down=abs_volt_limit_down;
        dvolt_DAC1 = 0.5;
      }
      
      //restrict locking range
      if (volt_out_DAC1 > abs_volt_limit_up)
        volt_out_DAC1 = abs_volt_limit_up;
      if (volt_out_DAC1 < abs_volt_limit_down)
        volt_out_DAC1 = abs_volt_limit_down;
      AD5791_SetRegisterValue(sync, AD5791_REG_DAC, volt_out_DAC1);
  
             
  
      if(engage == 1)   //feedback on
      {
        time_index = time_index + 1;          
          //              if (state_flag == 0)    //switch pid off  state_flag==0
          //              {
          //                  if (freeze && time_index > pid_on_loops)
          //                  {
          //                    flag_monitor = 1000;
          //                    state_flag = 1; 
          //                    time_index = 0;          
          //                  }
          //              }
        if(state_flag == 0)//PID
        {       
          errorsum = errorsum + (error - setpoint)/100000;//Scaling the integral
          if (I*errorsum > 400000) //Checking if integral feedback is not too large
            errorsum = 400000/I;
          if (I*errorsum < -400000)
            errorsum = -400000;
          C =P*(error - setpoint) + I*errorsum + D*(error - olderror); //all you have to do is replace this with NN output    
          olderror = error;
        }        
      }
      volt_out_DAC1 = last_resonance_volt + C + sinetable[0][r];
    }

    AD5791_SetRegisterValue(sync, AD5791_REG_DAC, volt_out_DAC1);
  }
}
