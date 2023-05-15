#define TEENSY4_0 // set the teensy used.
// #define DEBUG // set this to get debug info in the serial screen.

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
#include "Scan.h"

// preprocessor definitions
#define Tsample 30 //sample time for timer in microseconds
#define pi 3.14159

// constants

// pin definitions
DAC dac1(A0, A1, A2, A3);
DAC dac2(A4, A5, A6, A7);

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


// commands
UpCommand upCommand('u', &dac2, 20000);
DownCommand downCommand('d', &dac2, 20000);
ZeroCommand zeroCommand('z', &dac1, &dac2, 100);

ScanWaveform scan(dac1.getVoltageLowerLimit(), dac1.getVoltageUpperLimit());
ScanCommand scanCommand('s', scan);

long max_bits = 524287;
SineWaveform<333> sineWave(max_bits*0.1, 0);
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
  
  delay(1000);

  //switch to DAC2 and initialize, the lines marked useless will be removed. This is because these variables are not used by AD5791_SetRegisterValue or AD5791_SetRegisterValue

  // dac1.configuration.tristate = false;
  dac1.configuration.defaultConfiguration();
  dac1.set2ComplementMode();
  dac1.initializeDac();

  
  dac2.configuration.defaultConfiguration();
  dac2.set2ComplementMode();
  dac2.initializeDac();

}

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
          scanCommand.scan.setLowerScanLimit(0);
          scanCommand.scan.setUpperScanLimit(10000);
          scanCommand.scan.initializeScan(dac1);
          scanCommand.Execute(command.requestId);
          break;
        case 'w':
          sineCommand.Execute(command.requestId);
          break;
        default:
          SendError("unkown command");
          break;
      }
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
      dac1.setWaveformVoltage(scan);
      break;
    case 'w':
      // time1 = micros();
      dac1.setWaveformVoltage(sineWave);
      // time2 = micros();
      break;
  }
  
  // Serial.println(time2-time1);
}
