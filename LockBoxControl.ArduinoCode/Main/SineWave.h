#ifndef SINEWAVE_H
#define SINEWAVE_H

#include "dac.h"
#include "utilities.h"

template <long PERIOD> // period in microseconds
class SineWave
{
private:
    /* data */
    DAC& dac;

    long amplitude;

    float phase;
    long startingTime;

    
    long voltages[PERIOD];
public:
    SineWave(DAC& dac, long amplitude, float phase);
    ~SineWave();
    
    long period() const;
    float frequency() const;
    float angular_frequency() const;


    void calculateSineTable();

    void initialize(){
        startingTime = micros();
    }

    void setSineWaveVoltage();
    long getSineWaveVoltage();

};

template <long PERIOD>
inline long SineWave<PERIOD>::period() const
{
    return 0;
}

template <long PERIOD>
inline float SineWave<PERIOD>::frequency() const
{
    return 1.0/PERIOD;
}

template <long PERIOD>
inline float SineWave<PERIOD>::angular_frequency() const
{
    return 2*PI*frequency();
}

template <long PERIOD>
inline void SineWave<PERIOD>::calculateSineTable(){
  
  for(long t = 0; t < PERIOD; t++){
    voltages[t] = Utilities::calculateSinVoltage(t, angular_frequency(), phase, amplitude);
  }
}

template <long PERIOD>
inline void SineWave<PERIOD>::setSineWaveVoltage()
{
        dac.setOutputVoltage(voltages[micros() % PERIOD]); // 
}

template <long PERIOD>
inline long SineWave<PERIOD>::getSineWaveVoltage()
{
        return voltages[micros() % PERIOD];
}

// // frequency in Hz
// template <long PERIOD>
// SineWave<PERIOD>::setFrequency(float frequency){
//   this->frequency = 2*PI*frequency;
//   // get the period
//   period = 1/frequency;
// }

template <long PERIOD>
inline SineWave<PERIOD>::SineWave(DAC &dac, long amplitude, float phase) : dac(dac), amplitude(amplitude), phase(phase)
{
    calculateSineTable();
}

template <long PERIOD>
SineWave<PERIOD>::~SineWave()
{
}


#endif