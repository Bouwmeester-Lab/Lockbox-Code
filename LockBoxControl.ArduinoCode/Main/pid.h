#ifndef PID_H
#define PID_H

template <typename TCorrection>
class PID
{
private:
    /* data */
    float P;
    float I;
    float D;
    float setPoint;
public:
    PID(/* args */);
    ~PID();

    TCorrection calculateCorrection(float error);
};




#endif