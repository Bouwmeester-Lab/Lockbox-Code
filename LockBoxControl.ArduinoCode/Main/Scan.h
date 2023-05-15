#ifndef SCAN_H
#define SCAN_H

#include "commands.h"
#include "dac.h"
#include "Waveform.h"

class ScanWaveform : public Waveform
{
private:

  long slope_time = 1000; // in microseconds

  long lowerScanLimit;
  long upperScanLimit;

  long startingVoltage = 0;
  unsigned long startingTime = 0;

  long previousVoltage = 0;
  long outputVoltage = 0;

  bool negativeSlope = true;


public:
  ScanWaveform(long lowerScanLimit, long upperScanLimit, long slope_time = 1000)
  {
    this->slope_time = slope_time;

    this->lowerScanLimit = lowerScanLimit;
    

    this->upperScanLimit = upperScanLimit;
  }

  void setSlopeTime(long slope_time){
    this->slope_time = slope_time;
  }

  void setLowerScanLimit(long limit){
    lowerScanLimit = limit;
  }

  void setLowerScanLimit(DAC& dac){
    lowerScanLimit = dac.getVoltageLowerLimit();
  }

  void setUpperScanLimit(long limit){
    upperScanLimit = limit;
  }

  void setUpperScanLimit(DAC& dac){
    upperScanLimit = dac.getVoltageUpperLimit();
  }

  void initializeScan(DAC& dac){
    initializeScan(dac.getCurrentVoltage());
  }

  void initializeScan(long initialVoltage){
    startingVoltage = initialVoltage;

    #ifdef DEBUG
    Serial.println(lowerScanLimit);
    //Serial.println(dac.getVoltageLowerLimit());
    #endif
    startingTime = micros();
  }

  
  void setScanVoltage(DAC& dac){
    dac.setOutputVoltage(getScanVoltage());
  }

  long calculateValue() override
  {
    return getScanVoltage();
  }

  long getScanVoltage(){
    // get the current time
    unsigned long currentTime = micros();
    long targetVoltage;

    if(negativeSlope){
      targetVoltage = lowerScanLimit;
    }
    else{
      targetVoltage = upperScanLimit;
    }

    previousVoltage = outputVoltage;
    outputVoltage = Utilities::calculateLineVoltage(currentTime, targetVoltage, startingVoltage, slope_time, startingTime);

    if(abs(outputVoltage - targetVoltage) < abs(previousVoltage - outputVoltage) || outputVoltage == targetVoltage){
      // reached the bottom of the scan
      negativeSlope = !negativeSlope; // invert the slope
      outputVoltage = targetVoltage;
      //reset the scan
      initializeScan(outputVoltage);
    }
    #ifdef DEBUG
    Serial.printf("Target voltage: %i, Output Voltage: %i, Starting Time: %i, Current Time: %i\n", targetVoltage, outputVoltage, startingTime, currentTime);
    #endif
    return outputVoltage;
  }

};

class ScanCommand : public Command
{
private:
  
protected:
  SerialCommandStatus ExecuteCommand(String requestId) override
  {
    SerialCommandStatus status;
    status.isLongRunning = false;
    status.requestId = requestId;
    status.isOk = true;

    
    return status;    
  }
public:
  ScanWaveform& scan;
  ScanCommand(char commandLetter, ScanWaveform& scan) : Command(commandLetter), scan(scan)
  {
  }
};


#endif
