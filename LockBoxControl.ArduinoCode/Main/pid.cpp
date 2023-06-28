#include "pid.h"

PID::PID()
{
}

PID::PID(float P, float I, float D, float setPoint) : P(P), I(I), D(D), setPoint(setPoint)
{
}

PID::PID(float P, float I, float D) : P(P), I(I), D(D)
{
}

PID::~PID()
{
}

long PID::calculateCorrection(float error)
{
    auto correction = P * (error - setPoint) + I * errorSum + D * (error - oldError);

    if(correction > feedbackUpperLimit){
        correction = feedbackUpperLimit;
    }
    else if (correction < feedbackLowerLimit)
    {
        correction = feedbackLowerLimit;
    }
    else{
        errorSum += (error - setPoint)/integralScaling;
    }
    
    oldError = error;

    return correction;
}