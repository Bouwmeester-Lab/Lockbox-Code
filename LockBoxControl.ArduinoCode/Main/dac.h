#ifndef dac_H
#define dac_H

#include "ad5791.h"
#include <SPI.h>
#include "utilities.h"
#include "Waveform.h"

// #define NO_LINCOMP 0x0
// #define LINCOMP_10_12_V  0x9
// #define LINCOMP_12_16_V 0xA
// #define LINCOMP_16_19_V 0xB
// #define LINCOMP_19_20_V 0xC

class VoltageLimits{
private:
  long upperLimit = 0;
  long lowerLimit = 0;
public:
  VoltageLimits(long upperLimit, long lowerLimit)
  {
    setLowerLimit(lowerLimit);
    setUpperLimit(upperLimit);
  }

  VoltageLimits(){}

  ~VoltageLimits(){}

  void setUpperLimit(long limit){
    upperLimit = limit;
  }

  void setLowerLimit(long limit){
    lowerLimit = limit;
  }

  long getUpperLimit(){
    return upperLimit;
  }

  long getLowerLimit(){
    return lowerLimit;
  }
};

class DACConfiguration{
public:
  static const unsigned long NO_LINCOMP = 0x0;
  static const unsigned long LINCOMP_10_12_V = 0x9;
  static const unsigned long LINCOMP_12_16_V = 0xA;
  static const unsigned long LINCOMP_16_19_V = 0xB;
  static const unsigned long LINCOMP_19_20_V = 0xC;

  bool rbuf = true; // configure the internal amplifier of the ad5791, A1 -> false is powered up, true is powered down. See AD5791 Features section for more details.
  bool opgnd = true; // false: DAC output is NOT clamped to ground, and DAC in normal mode. True: (default) DAC is clamped down through a 6kOhm resistance

  bool tristate = true; // false: normal operating mode, true (default): dac is in the tristate mode

  bool binary = false; // false (default): uses two complement coding -> negative numbers are encoded using the MSB as the sign bit. True: uses binary representation -> 0 is Vref Negative, 2^20 - 1 is Vref Positive.

  bool sdodis = false; // false (default): SDO pin is enabled. True: SDO pin is disabled (tristate)

  unsigned long lincomp = DACConfiguration::NO_LINCOMP; // linear compensation for different reference voltages. Use either DACConfiguration::NO_LINCOMP, DACConfiguration::LINCOMP_10_12_V, DACConfiguration::LINCOMP_12_16_V, DACConfiguration::LINCOMP_16_19_V, or LINCOMP_19_20_V

  unsigned long getControlRegisterValue(){
    return AD5791_ADDR_REG(AD5791_REG_CTRL) | (lincomp << 9) | (sdodis << 5) | (binary << 4) | (tristate << 3) | (opgnd << 2) | (rbuf << 1); // see table 12 for the structure of the register value
  }

  void allFalse(){
    rbuf = false;
    opgnd = false;
    tristate = false;

    binary = false;
    sdodis = false;
  }

  void defaultConfiguration(){
    rbuf = true;

    opgnd = false;
    tristate = false;

    binary = false;
    sdodis = false;
  }
};

class DAC
{
private:
    // full 20 bit range.
    VoltageLimits maxRangeLimit = VoltageLimits(1048575, 0);
    // allows the user to set a safety range which is smaller than the range allowed by the dac.
    VoltageLimits safetyRangeLimit = VoltageLimits(1048575, 0);
    
    long currentVoltage;
    long offset = 0;

    uint8_t reset;
    uint8_t clear;
    uint8_t sync;
    uint8_t ldac;

    void capSafetyRange(){
      if(safetyRangeLimit.getUpperLimit() > maxRangeLimit.getUpperLimit()){
        safetyRangeLimit.setUpperLimit(maxRangeLimit.getUpperLimit());
      }
      if(safetyRangeLimit.getLowerLimit() < maxRangeLimit.getLowerLimit()){
        safetyRangeLimit.setLowerLimit(maxRangeLimit.getLowerLimit());
      }
    }

    void matchSafetyRange(){
      safetyRangeLimit.setUpperLimit(maxRangeLimit.getUpperLimit());
      safetyRangeLimit.setLowerLimit(maxRangeLimit.getLowerLimit());
    }
public:
    

    

    DACConfiguration configuration = DACConfiguration();

    void setSafetyLimits(long upperLimit, long lowerLimit){
      safetyRangeLimit.setLowerLimit(lowerLimit);
      safetyRangeLimit.setUpperLimit(upperLimit);
    }

    long getVoltageUpperLimit(){
      if(safetyRangeLimit.getUpperLimit() < maxRangeLimit.getUpperLimit()){
        return safetyRangeLimit.getUpperLimit();
      }
      return maxRangeLimit.getUpperLimit();
    }

    long getVoltageLowerLimit(){
      if(safetyRangeLimit.getLowerLimit() > maxRangeLimit.getLowerLimit()){
        return safetyRangeLimit.getLowerLimit();
      }
      return maxRangeLimit.getLowerLimit();
    }

    long getCurrentVoltage(){
      return currentVoltage;
    }

    void setOffset(long offset){
      this->offset = offset;
    }

    VoltageLimits getSafetyRangeVoltageLimits(){
      return safetyRangeLimit;
    }


    /// This sets the dac to accept negative values for the input registry. The MSB is the sign bit, and the rest indicate the number. As such 0 is the actual 0V (if VREFN = - VREFP),
    /// and the upper limit is VREFP and the lower limit is VREFN.
    void set2ComplementMode(bool resetSafetyRange = true){
      maxRangeLimit.setUpperLimit(524287);
      maxRangeLimit.setLowerLimit(-524287);
      
      if(resetSafetyRange){
        matchSafetyRange();
      }
      else{
        capSafetyRange();
      }
      
      

      // change the configuration
      configuration.binary = false;
    }
    /// This sets the dac to accept binary numbers in such a way that a voltage of 0 is VREFN and a voltage of 2^20 - 1 is VREFP.
    void setBinaryMode(bool resetSafetyRange = true){

      maxRangeLimit.setUpperLimit(1048575);
      maxRangeLimit.setLowerLimit(0);

      if(resetSafetyRange){
        matchSafetyRange();
      }
      else{
        capSafetyRange();
      }
      //change the configuration;
      configuration.binary = true;
    }

    DAC(uint8_t reset, uint8_t clear, uint8_t ldac, uint8_t sync){
        this->reset = reset;
        this->clear = clear;
        this->sync = sync;
        this->ldac = ldac;
    }

    // see https://www.analog.com/media/en/technical-documentation/data-sheets/ad5791.pdf page 21
    // sets the output voltage of the dac by setting the input registry. Make sure to provide a value correspoding to the binary or two's complement mode selected during configuration of the dac.
    // if the dac is configured to use binary values, then any value between 0 and 2^20-1 is ok.
    // if the dac is configured to use twos complement mode, then any value between -524287 and 524287 are allowed.
    bool setOutputVoltage(long registryValue){
      
      registryValue += offset; //apply any DC offset if any is set.

      if(registryValue <= getVoltageUpperLimit() && registryValue >= getVoltageLowerLimit()){
        currentVoltage = registryValue;
        AD5791_SetRegisterValue(sync, AD5791_REG_DAC, registryValue);
        return true;
      }
      return false;
    }

    /// Sets up the pins of the dac by configuring the arduino pins as outputs and setting the pins for normal usage of the dac.
    void setUpDacPins(bool initializeSPI = false){
      if(initializeSPI){
        SPI.begin();
      }
        pinMode(reset, OUTPUT);
        pinMode(clear  , OUTPUT);
        pinMode(ldac , OUTPUT);
        pinMode(sync , OUTPUT);

        digitalWrite(ldac,LOW); // when low the input registry cannot be changed.
        digitalWrite(reset,HIGH); // when low the dac is reset.
        digitalWrite(clear,HIGH); // when low the dac input registry is cleared to 0
        digitalWrite(sync,HIGH); // when low the input registry can be modified.
    }
    /// @brief The control registry of the dac controls the mode of operation of the DAC. See Table 11-12 of https://www.analog.com/media/en/technical-documentation/data-sheets/ad5791.pdf
    ///  the evaluation board uses the reference board to supply a stable +10 at the REFP pin and -10V at the REFN pin. Meaning the voltage can go from -10 to +10 V.
    /// @return 
    long getCtrlValue(){
        return AD5791_GetRegisterValue(sync, AD5791_REG_CTRL);
    }

    void clearInputRegister(){
      // clear the dac
        digitalWrite(clear, LOW);
        delay(1);
        digitalWrite(clear, HIGH);
        delay(1);
    }

    void resetDac(){
      // reset the dac
      digitalWrite(reset, LOW);
      delay(1);
      digitalWrite(reset, HIGH);
      delay(1);
    }

    /// initializes the dac using the current configuration associated with the DAC.
    void initializeDac(){
        resetDac();
        clearInputRegister();    

        unsigned long registerValue =  AD5791_WRITE | configuration.getControlRegisterValue();

        #ifdef DEBUG
        Serial.println("initializing dac with the following control registry value");
        Serial.println(registerValue, BIN);
        #endif

        setControlRegister(registerValue);
        zeroDac(10);
    }

    void setControlRegister(long registerValue){
      AD5791_SetRegisterValue(sync, AD5791_REG_CTRL, registerValue);
    }

    //zero
    // slew_time is in microseconds
    // trueZero -> zeros to 0V, if false zeros at the middle of the allowed voltage range (default)
    void zeroDac(unsigned long slew_time = 1000, bool trueZero = false){
        unsigned long startingTime = micros();

        long startingVoltage = currentVoltage;

        long targetVoltage;

        if(trueZero)
        {
          if(configuration.binary)
          {
            targetVoltage = (maxRangeLimit.getUpperLimit() + maxRangeLimit.getLowerLimit())/2;
          }
          else
          {
              targetVoltage = 0;
          }
        }
        else
        {
          targetVoltage = (getVoltageUpperLimit() + getVoltageLowerLimit())/2;
          // Serial.println(targetVoltage);
        }
        
         

        bool reachedZero = false;
        long outputVoltage = startingVoltage;
        
        long previousVoltage = startingVoltage;

        while(!reachedZero){
            previousVoltage = outputVoltage;
            outputVoltage = Utilities::calculateLineVoltage(micros(), targetVoltage, startingVoltage, slew_time, startingTime);

            setOutputVoltage(outputVoltage);

            #ifdef DEBUG
            Serial.printf("ov: %i \n", outputVoltage);
            Serial.printf("sz: %i \n", previousVoltage - outputVoltage);
            #endif

            if(abs(outputVoltage - targetVoltage) < abs(previousVoltage - outputVoltage)){
                setOutputVoltage(targetVoltage);

                #ifdef DEBUG
                Serial.println("Reached target value");
                #endif
                reachedZero = true;
            }
            // just in case we limit how long this loop can last:
            if((unsigned long)(micros() - startingTime) > 2*slew_time){
                setOutputVoltage(targetVoltage);
                reachedZero = true;
            }
        }
    }

    bool down(unsigned long step_size){
      return setOutputVoltage(currentVoltage - step_size);
    }

    bool up(unsigned long step_size){
      return setOutputVoltage(currentVoltage + step_size);
    }

    bool setWaveformVoltage(Waveform& waveform){
      return setOutputVoltage(waveform.calculateValue());
    }

    bool setWaveformVoltage(Waveform& waveform1, Waveform& waveform2){
      return setOutputVoltage(waveform1.calculateValue() + waveform2.calculateValue());
    }

    bool setOutputVoltage(long voltage1, long voltage2, Waveform& waveform){
      return setOutputVoltage(voltage1 + voltage2 + waveform.calculateValue());
    }


};


#endif
