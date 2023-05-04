//Low pass filter, order=2 alpha1=0.0003 ==> 15Hz
class  LPF2
{
  public:
    LPF2(float corner_freq)
    {
      v[0]=0.0;
      v[1]=0.0;      
      LPF2_al = 0.03 / 213 * corner_freq;
    }
  private:
    float LPF2_al;
  private:
    float v[2];
  public:
    float main(float x) //class II 
    {
      if (v[0] == 0.0 && v[1] == 0.0)   //initialize
      {
        v[0] = x;
        v[1] = x;        
      }
      else
      {
        v[0] = (1 - LPF2_al) * v[0] + LPF2_al * x;
        v[1] = (1 - LPF2_al) * v[1] + LPF2_al * v[0];
      }
      
      return v[1];
    }
};