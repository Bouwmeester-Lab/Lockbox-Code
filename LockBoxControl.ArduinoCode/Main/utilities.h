#ifndef UTILITIES_H
#define UTILITIES_H

class Utilities{
public:
    static long calculateLineVoltage(unsigned long current_millis, long target_voltage, long starting_voltage, unsigned long milliseconds_to_reach_target, unsigned long starting_millis){
        return (long)((double)(target_voltage - starting_voltage)/milliseconds_to_reach_target*(current_millis - starting_millis) + starting_voltage);
    }
    /// @brief 
    /// @param current_micros 
    /// @param frequency in MHz, radial frequency, i.e. 2*pi*real_frequency
    /// @param phase in rads
    /// @param amplitude 
    /// @return 
    static long calculateSinVoltage(unsigned long current_micros, float frequency, float phase, long amplitude){
        return (long)(sin(frequency*current_micros+phase)*amplitude);
    }
};

#endif