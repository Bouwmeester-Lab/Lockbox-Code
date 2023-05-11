#define TEENSY4_0 // set the teensy used.

#include <string.h>
#include <math.h>
#include <IntervalTimer.h>
#include <SPI.h>

// custom libraries
#include "LPF2.h"
#include "ad5791.h"
#include "FilterBuLp2.h"

#ifdef TEENSY4_1
  #include "ethernetCommunication.h"
#endif

#include "SerialCommand.h"
#include "dac.h"
#include "commands.h"
#include "SineWave.h"
#include "SineCommand.h"

// preprocessor definitions
#define Tsample 30 //sample time for timer in microseconds
#define pi 3.14159

// constants

// pin definitions
DAC dac1(A0, A1, A2, A3);
DAC dac2(A4, A5, A6, A7);

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

// commands
UpCommand upCommand('u', &dac2, 20000);
DownCommand downCommand('d', &dac2, 20000);
ZeroCommand zeroCommand('z', &dac1, &dac2, 100);

Scan scan_dac_1(dac1);
ScanCommand scanCommand('s', scan_dac_1);

SineWave<333> sineWave(dac1, max_bits*0.1, 0);
SineCommand<333> sineCommand('w', sineWave);

void setup() 
{
  pinMode(A8, INPUT);

  Serial.begin(38400); // the value inside begin doesn't matter in the teensies.
  SPI.begin(); // initialize the SPI communicaton

  dac1.setUpDacPins();
  dac2.setUpDacPins();
  
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

  // dac1.configuration.tristate = false;
  dac1.configuration.defaultConfiguration();
  dac1.set2ComplementMode();
  dac1.initializeDac();

  
  dac2.configuration.defaultConfiguration();
  dac2.set2ComplementMode();
  dac2.initializeDac();

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

char incomingByte = 122;

float refladd = 0;
int reflindex = 0;
float reflmean = 0;

SerialCommand command;

long time1 = 0;
long time2 = 0;

void loop() 
{ 
  
  if (Serial.available())
  {
    auto c = Serial.readStringUntil('\n');

    command = SerialCommand::serialize(c);

    if(command.isOk){
      switch(command.commandLetter){
        #ifdef TEENSY4_1
        case 'm':
          getMacAddress(command.requestId);
          break;
        #endif
        case 'z':
          zeroCommand.Execute(command.requestId);
          break;
        case 'u':
          upCommand.Execute(command.requestId);
          break;
        case 'd':
          downCommand.Execute(command.requestId);
          break;
        case 's':
          scanCommand.scan.setSlopeTime(2000);
          scanCommand.scan.setLowerScanLimit(-10000);
          scanCommand.scan.setUpperScanLimit(10000);
          scanCommand.Execute(command.requestId);
          break;
        case 'w':
          sineCommand.Execute(command.requestId);
          break;
        default:
          SendError("unkown command");
          break;
      }
      incomingByte = command.commandLetter;
    }
    else{
      SendError(command.errorMessage);
    }
    // incomingByte = Serial.read();  // will not be -1
    //Serial.println(incomingByte);   //  l=108; s=115;  z=122
  }

  // these are the commands that require to be ran in the loop, i.e. scan
  switch(command.commandLetter){
    case 's':
      scan_dac_1.setScanVoltage();
      break;
    case 'w':
      // time1 = micros();
      sineWave.setSineWaveVoltage();
      // time2 = micros();
      break;
  }
  

  // Serial.println(time2-time1);
}
