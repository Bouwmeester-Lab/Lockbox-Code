#ifndef dac_H
#define dac_H

#include "ad5791.h"
#include <SPI.h>

// #define NO_LINCOMP 0x0
// #define LINCOMP_10_12_V  0x9
// #define LINCOMP_12_16_V 0xA
// #define LINCOMP_16_19_V 0xB
// #define LINCOMP_19_20_V 0xC

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
    unsigned long calculateLineVoltage(unsigned long current_millis, unsigned long target_voltage, unsigned long starting_voltage, unsigned long milliseconds_to_reach_target, unsigned long starting_millis){
        return (target_voltage - starting_voltage)/milliseconds_to_reach_target*(current_millis - starting_millis) + starting_voltage;
    }
public:
    uint8_t reset;
    uint8_t clear;
    uint8_t sync;
    uint8_t ldac;

    unsigned long currentVoltage;

    DACConfiguration configuration = DACConfiguration();

    // full 20 bit range.
    long voltageUpperLimit = 1048575;
    long voltageLowerLimit = 0;

    /// This sets the dac to accept negative values for the input registry. The MSB is the sign bit, and the rest indicate the number. As such 0 is the actual 0V (if VREFN = - VREFP),
    /// and the upper limit is VREFP and the lower limit is VREFN.
    void set2ComplementMode(){
      voltageUpperLimit = 524287;
      voltageLowerLimit = -524287;

      // change the configuration
      configuration.binary = false;
    }
    /// This sets the dac to accept binary numbers in such a way that a voltage of 0 is VREFN and a voltage of 2^20 - 1 is VREFP.
    void setBinaryMode(){
      voltageUpperLimit = 1048575;
      voltageLowerLimit = 0;
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
    void setOutputVoltage(long registryValue){
        currentVoltage = registryValue;
        AD5791_SetRegisterValue(sync, AD5791_REG_DAC, registryValue);
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
    }

    void setControlRegister(long registerValue){
      AD5791_SetRegisterValue(sync, AD5791_REG_CTRL, registerValue);
    }

    //zero
    // slew_time is in microseconds
    void zeroDac(unsigned long slew_time = 1000){
        unsigned long startingTime = millis();

        unsigned long startingVoltage = currentVoltage;


        unsigned long targetVoltage = (voltageUpperLimit - voltageLowerLimit)/2;

        bool reachedZero = false;
        unsigned long outputVoltage;


        while(!reachedZero){
            outputVoltage = calculateLineVoltage(millis(), targetVoltage, startingVoltage, slew_time, startingTime);
            setOutputVoltage(outputVoltage);
            if(abs(outputVoltage - targetVoltage) < 10){
                reachedZero = true;
            }
            // just in case we limit how long this loop can last:
            if((unsigned long)(millis() - startingTime) > 2*slew_time){
                reachedZero = true;
            }
        }
    }


};


#endif
