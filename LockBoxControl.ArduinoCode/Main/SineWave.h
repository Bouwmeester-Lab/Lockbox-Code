#ifndef SINEWAVE_H
#define SINEWAVE_H

#include "dac.h"
#include "utilities.h"
#include "Waveform.h"

template <long PERIOD> // period in microseconds
class SineWaveform : public Waveform
{
private:
    /* data */
    long amplitude;

    float phase;
    long startingTime;

    
    long voltages[PERIOD];
public:
    SineWaveform(long amplitude, float phase);
    ~SineWaveform();
    
    long period() const;
    float frequency() const;
    float angular_frequency() const;


    void calculateSineTable();

    void initialize(){
        startingTime = micros();
    }
    
    long calculateValue() override;

};

template <long PERIOD>
inline long SineWaveform<PERIOD>::period() const
{
    return 0;
}

template <long PERIOD>
inline float SineWaveform<PERIOD>::frequency() const
{
    return 1.0/PERIOD;
}

template <long PERIOD>
inline float SineWaveform<PERIOD>::angular_frequency() const
{
    return 2*PI*frequency();
}

template <long PERIOD>
inline void SineWaveform<PERIOD>::calculateSineTable(){
  
  for(long t = 0; t < PERIOD; t++){
    voltages[t] = Utilities::calculateSinVoltage(t, angular_frequency(), phase, amplitude);
  }
}

template <long PERIOD>
inline long SineWaveform<PERIOD>::calculateValue()
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
inline SineWaveform<PERIOD>::SineWaveform(long amplitude, float phase) : amplitude(amplitude), phase(phase)
{
    calculateSineTable();
}

template <long PERIOD>
SineWaveform<PERIOD>::~SineWaveform()
{
}


#endif