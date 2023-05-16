#ifndef RESONANCELOCK_H
#define RESONANCELOCK_H

#include "dac.h"
#include "Scan.h"
#include "SineWave.h"
#include "pid.h"
#include "SerialCommand.h"

// period in microseconds

template <long PERIOD>
class ResonanceLock
{
private:
    /* data */
    bool resonanceFound = false;
    long resonanceVoltage;
    int reflectionValue;
    int lockThreshold = 1600; // default value
    int outOfLockReflectionThreshold = 2000;
    

    DAC& dac;
    SineWaveform<PERIOD>& sineWave;
    
    PID& pid;

    ScanWaveform scan;
    int reflectionPin;
    long fineScanTime;
    long initialScanTime;
    long fineScanWidth = 10000;
    
     
public:

    ResonanceLock(DAC& dac, SineWaveform<PERIOD>& sineWaveform, PID& pid, int reflectionPin);
    void setLockThreshold(int threshold);
    void setOutOfLockReflectionThreshold(int threshold);

    void initialize(long initial_scan_time, long fine_scan_time);
    void lock(SerialCommand& command);
    void replyToCommand(SerialCommand& command);
    void resetScan();
};

template <long PERIOD>
inline ResonanceLock<PERIOD>::ResonanceLock(DAC& dac, SineWaveform<PERIOD> &sineWaveform, PID& pid, int reflectionPin) : dac(dac), sineWave(sineWaveform), pid(pid), scan(dac.getVoltageLowerLimit(), dac.getVoltageUpperLimit()), reflectionPin(reflectionPin)
{
}

template <long PERIOD>
inline void ResonanceLock<PERIOD>::setLockThreshold(int threshold)
{
    lockThreshold = threshold;
}

template <long PERIOD>
inline void ResonanceLock<PERIOD>::setOutOfLockReflectionThreshold(int threshold)
{
    outOfLockReflectionThreshold = threshold;
}

template <long PERIOD>
inline void ResonanceLock<PERIOD>::initialize(long initial_scan_time, long fine_scan_time)
{
    // set the pin as input
    pinMode(reflectionPin, INPUT);
    // save times
    this->initialScanTime = initial_scan_time;
    this->fineScanTime = fine_scan_time;

    resetScan();
    
    resonanceFound = false;
    this->fineScanTime = fine_scan_time;

}

template <long PERIOD>
inline void ResonanceLock<PERIOD>::lock(SerialCommand& command)
{
    reflectionValue = analogRead(reflectionPin);
    if(!resonanceFound)
    {
        
        dac.setWaveformVoltage(scan, sineWave);
        

        if(reflectionValue < lockThreshold / 2)
        {
            Serial.println(reflectionValue);
            resonanceFound = true;
            // found the resonance
            resonanceVoltage = dac.getCurrentVoltage();
        }
        else if(reflectionValue < lockThreshold){
            // found the resonance
            resonanceVoltage = dac.getCurrentVoltage();
            
            // make the scan finer
            scan.setLowerScanLimit(resonanceVoltage - fineScanWidth);
            scan.setUpperScanLimit(resonanceVoltage + fineScanWidth);
            scan.setSlopeTime(fineScanTime);
        }
    }
    else
    {
        if(reflectionValue > outOfLockReflectionThreshold){
            // the lock is lost
            resonanceFound = false;
            resetScan();
        }
        // long time1 = micros();
        long correction = pid.calculateCorrection(sineWave.calculateValue()*reflectionValue);\
        // long time2 = micros();
        // Serial.printf("It took %i us to calculate the correction of %i\n", time2 - time1, correction);
        dac.setOutputVoltage(resonanceVoltage, correction, sineWave);
    }

    
}

template <long PERIOD>
inline void ResonanceLock<PERIOD>::replyToCommand(SerialCommand &command)
{
    SerialCommandStatus status;
    status.isLongRunning = true;
    status.requestId = command.requestId;
    status.sendThroughSerial();
}

template <long PERIOD>
inline void ResonanceLock<PERIOD>::resetScan()
{
    // modify the scans used
    scan.setLowerScanLimit(dac.getVoltageLowerLimit());
    scan.setUpperScanLimit(dac.getVoltageUpperLimit());
    scan.setSlopeTime(initialScanTime);
}

#endif