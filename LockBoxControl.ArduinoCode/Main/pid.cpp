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
    errorSum += (error - setPoint)/integralScaling;

    if(I * errorSum > integralFeedbackUpperLimit || I*errorSum < integralFeedbackLowerLimit)
    {
        errorSum = 0;
    }

    auto correction = P * (error - setPoint) + I * errorSum + D * (error - oldError);
    oldError = error;

    return correction;
}