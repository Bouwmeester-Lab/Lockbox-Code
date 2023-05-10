#ifndef UTILITIES_H
#define UTILITIES_H

class Utilities{
public:
    static long calculateLineVoltage(unsigned long current_millis, long target_voltage, long starting_voltage, unsigned long milliseconds_to_reach_target, unsigned long starting_millis){
        return (long)((double)(target_voltage - starting_voltage)/milliseconds_to_reach_target*(current_millis - starting_millis) + starting_voltage);
    }
};

#endif