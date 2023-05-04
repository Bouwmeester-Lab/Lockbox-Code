//Low pass butterworth filter order=2 alpha1=0.0003 ==> 15Hz
class  FilterBuLp2
{
  public:
    FilterBuLp2()
    {
      v[0]=0.0;
      v[1]=0.0;
      v[2]=0.0;
    }
  private:
    float v[3];
  public:
    float main(float x) //class II 
    {
      v[0] = v[1];
      v[1] = v[2];
      v[2] = (8.870817736483793681e-7 * x)
         + (-0.99733782013962946067 * v[0])
         + (1.99733427181253486715 * v[1]);
      return 
         (v[0] + v[2]) + 2 * v[1];
    }
};