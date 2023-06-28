#ifndef PID_H
#define PID_H


class PID
{
private:
    /* data */
    float P;
    float I;
    float D;
    float setPoint = 0;
    float oldError = 0;

    float errorSum = 0;
    float integralScaling = 100000;

    float feedbackUpperLimit = 400000;
    float feedbackLowerLimit = -400000;
public:
    PID(/* args */);
    PID(float P, float I, float D, float setPoint);
    PID(float P, float I, float D);
    ~PID();

    long calculateCorrection(float error);
};




#endif