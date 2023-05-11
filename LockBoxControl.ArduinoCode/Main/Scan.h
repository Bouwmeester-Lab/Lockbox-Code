#ifndef SCAN_H
#define SCAN_H

#include "commands.h"
#include "dac.h"

class Scan
{
private:
  DAC& dac;
  long slope_time = 1000; // in microseconds

  long lowerScanLimit;
  long upperScanLimit;

  long startingVoltage = 0;
  unsigned long startingTime = 0;

  long previousVoltage = 0;
  long outputVoltage = 0;

  bool negativeSlope = true;


public:
  Scan(DAC& dac, long slope_time = 1000) : dac(dac)
  {
    this->slope_time = slope_time;

    this->lowerScanLimit = dac.getVoltageLowerLimit();
    

    this->upperScanLimit = dac.getVoltageUpperLimit();
  }

  void setSlopeTime(long slope_time){
    this->slope_time = slope_time;
  }

  void setLowerScanLimit(long limit){
    lowerScanLimit = limit;
  }

  void setLowerScanLimit(){
    lowerScanLimit = dac.getVoltageLowerLimit();
  }

  void setUpperScanLimit(long limit){
    upperScanLimit = limit;
  }

  void setUpperScanLimit(){
    upperScanLimit = dac.getVoltageUpperLimit();
  }

  void initializeScan(){
    startingVoltage = dac.getCurrentVoltage();

    #ifdef DEBUG
    Serial.println(lowerScanLimit);
    Serial.println(dac.getVoltageLowerLimit());
    #endif
    startingTime = micros();
  }

  
  void setScanVoltage(){
    dac.setOutputVoltage(getScanVoltage());
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

    if(abs(outputVoltage - targetVoltage) < abs(previousVoltage - outputVoltage)){
      // reached the bottom of the scan
      negativeSlope = !negativeSlope; // invert the slope
      outputVoltage = targetVoltage;
      //reset the scan
      initializeScan();
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

    scan.initializeScan();
    return status;    
  }
public:
  Scan& scan;
  ScanCommand(char commandLetter, Scan& scan) : Command(commandLetter), scan(scan)
  {
  }
};


#endif
