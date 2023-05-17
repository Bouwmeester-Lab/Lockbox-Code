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
    bool fineResonanceFound = false;
    bool coarseResonanceFound = false;

    long resonanceVoltageFineDac;
    long resonanceVoltageCoarseDac;

    int reflectionValue;
    int lockThreshold = 1600; // default value
    int outOfLockReflectionThreshold = 2000;
    
    DAC& coarseDac; // dac for coarse adjustments (usually dac2 with the amplifier)
    DAC& fineDac; // dac for fine adjustments (no amplifier usually)
    
    SineWaveform<PERIOD>& sineWave;
    
    PID& pid;

    ScanWaveform scan;
    int reflectionPin;
    long fineScanTime;
    long initialScanTime;
    long fineScanWidth = 10000;
    
    void setResonanceVoltage();
    void makeScanFine();
     
public:

    ResonanceLock(DAC& coarseDac, DAC& fineDac, SineWaveform<PERIOD>& sineWaveform, PID& pid, int reflectionPin);
    void setLockThreshold(int threshold);
    void setOutOfLockReflectionThreshold(int threshold);

    void initialize(long initial_scan_time, long fine_scan_time);
    void lock(SerialCommand& command);
    void replyToCommand(SerialCommand& command);
    void resetCoarseScan();
    void resetOnlyScan();
};

template <long PERIOD>
inline void ResonanceLock<PERIOD>::setResonanceVoltage()
{
    if(coarseResonanceFound)
    {
        resonanceVoltageFineDac = fineDac.getCurrentVoltage();
    }
    else
    {
        resonanceVoltageCoarseDac = coarseDac.getCurrentVoltage();
        // coarseDac.setOutputVoltage(resonanceVoltageCoarseDac);
    }
}

template <long PERIOD>
inline void ResonanceLock<PERIOD>::makeScanFine()
{
    if(coarseResonanceFound)
    {
        // make the scan finer
        scan.setLowerScanLimit(resonanceVoltageFineDac - fineScanWidth);
        scan.setUpperScanLimit(resonanceVoltageFineDac + fineScanWidth);
        scan.setSlopeTime(fineScanTime);
    }
    else
    {
        // make the scan finer
        scan.setLowerScanLimit(resonanceVoltageCoarseDac - fineScanWidth);
        scan.setUpperScanLimit(resonanceVoltageCoarseDac + fineScanWidth);
        scan.setSlopeTime(fineScanTime);
    }
    
}

template <long PERIOD>
inline ResonanceLock<PERIOD>::ResonanceLock(DAC &coarseDac, DAC &fineDac, SineWaveform<PERIOD> &sineWaveform, PID &pid, int reflectionPin) : coarseDac(coarseDac), fineDac(fineDac), sineWave(sineWaveform), pid(pid), scan(fineDac.getVoltageLowerLimit(), fineDac.getVoltageUpperLimit()), reflectionPin(reflectionPin)
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

    resetCoarseScan();
    
    fineResonanceFound = false;
    coarseResonanceFound = false;
    this->fineScanTime = fine_scan_time;

}

template <long PERIOD>
inline void ResonanceLock<PERIOD>::lock(SerialCommand& command)
{
    reflectionValue = analogRead(reflectionPin);

    if(!fineResonanceFound)
    {
        if(coarseResonanceFound)
        {
            fineDac.setWaveformVoltage(scan, sineWave);
        }
        else
        {
            coarseDac.setWaveformVoltage(scan);
        }
        
        

        if(reflectionValue < lockThreshold / 2)
        {
            #ifdef DEBUG
            Serial.println(reflectionValue);
            #endif

            // found the resonance
            setResonanceVoltage(); // sets the resonance voltage from the dac depending on which step we are in, either coarse resonance or fine.
            // update the step we are in.
            if(coarseResonanceFound)
            {
                fineResonanceFound = true;
            }
            else
            {
                coarseResonanceFound = true;
                resetOnlyScan(); // we need to only reset the scan not the states here. and the scan used should be the coarse scan but with the fineDac.
            }
        }
        else if(reflectionValue < lockThreshold){
            // found the resonance
            setResonanceVoltage();
            
            // make the scan finer
            makeScanFine();
        }
    }
    else
    {
        if(reflectionValue > outOfLockReflectionThreshold){
            // the lock is lost
            resetCoarseScan();
        }
        // long time1 = micros();
        long correction = pid.calculateCorrection(sineWave.calculateValue()*reflectionValue);\
        // long time2 = micros();
        // Serial.printf("It took %i us to calculate the correction of %i\n", time2 - time1, correction);
        fineDac.setOutputVoltage(resonanceVoltageFineDac, correction, sineWave);
    }

    
}

template <long PERIOD>
inline void ResonanceLock<PERIOD>::replyToCommand(SerialCommand &command)
{
    resetCoarseScan();
    SerialCommandStatus status;
    status.isLongRunning = true;
    status.requestId = command.requestId;
    status.sendThroughSerial();
}

/// @brief The scan will be reset and the resonances set as not found.
/// @tparam PERIOD 
template <long PERIOD>
inline void ResonanceLock<PERIOD>::resetCoarseScan()
{
    // modify the scans used
    scan.setLowerScanLimit(coarseDac.getVoltageLowerLimit());
    scan.setUpperScanLimit(coarseDac.getVoltageUpperLimit());
    scan.setSlopeTime(initialScanTime);

    fineResonanceFound = false;
    coarseResonanceFound = false;
}

template <long PERIOD>
inline void ResonanceLock<PERIOD>::resetOnlyScan()
{
    // modify the scans used
    scan.setLowerScanLimit(fineDac.getVoltageLowerLimit());
    scan.setUpperScanLimit(fineDac.getVoltageUpperLimit());
    scan.setSlopeTime(initialScanTime);
}

#endif