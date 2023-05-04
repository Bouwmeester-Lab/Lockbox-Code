#ifndef dac_H
#define dac_H

#include "ad5791.h"

class DAC
{
private:
    unsigned long calculateLineVoltage(unsigned long current_millis, unsigned long target_voltage, unsigned long starting_voltage, unsigned long milliseconds_to_reach_target, unsigned long starting_millis){
        return (target_voltage - starting_voltage)/milliseconds_to_reach_target*(current_millis - starting_millis) + starting_voltage;
    }
public:
    const uint8_t reset;
    const uint8_t clear;
    const uint8_t sync;
    const uint8_t ldac;

    unsigned long currentVoltage;

    // full 20 bit range.
    unsigned long voltageUpperLimit = 0;
    unsigned long voltageLowerLimit = 1048575;


    DAC(uint8_t reset, uint8_t clear, uint8_t ldac, uint8_t sync){
        this->reset = reset;
        this->clear = clear;
        this->sync = sync;
        this->ldac = ldac;
    }

    // see https://www.analog.com/media/en/technical-documentation/data-sheets/ad5791.pdf page 21
    void setOutputVoltage(unsigned long registryValue){
        currentVoltage = registryValue;
        AD5791_SetRegisterValue(sync, AD5791_REG_DAC, registryValue);
    }

    void setUpDacPins(){
        pinMode(reset, OUTPUT);
        pinMode(clear  , OUTPUT);
        pinMode(ldac , OUTPUT);
        pinMode(sync , OUTPUT);

        digitalWrite(ldac,LOW);
        digitalWrite(reset,HIGH);
        digitalWrite(clear,HIGH);
        digitalWrite(sync,HIGH);
    }
    /// @brief The control registry of the dac controls the mode of operation of the DAC. See Table 11-12 of https://www.analog.com/media/en/technical-documentation/data-sheets/ad5791.pdf
    /// 
    /// @return 
    long getCtrlValue(){
        return AD5791_GetRegisterValue(sync, AD5791_REG_CTRL);
    }

    void initializeDac(unsigned long registerValue){
        AD5791_SetRegisterValue(sync, AD5791_REG_CTRL, registerValue);
        AD5791_SetRegisterValue(sync, AD5791_REG_DAC, 1);
    }

    //zero
    // slew_time is in microseconds
    void zeroDac(long slew_time = 1000){
        unsigned long currentTime = millis();

        unsigned long startingVoltage = currentVoltage;


        unsigned long targetVoltage = 

        bool reachedZero = false;
        unsigned long outputVoltage;
        while(!reachedZero){
            outputVoltage = calculateLineVoltage(millis(), )
        }
    }


};


#endif
