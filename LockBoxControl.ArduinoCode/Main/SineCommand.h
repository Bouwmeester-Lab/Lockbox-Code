#ifndef SINECOMMAND_H
#define SINECOMMAND_H

#include "commands.h"
#include "SineWave.h"

template <long PERIOD>
class SineCommand : public Command
{
private:
  
protected:
  SerialCommandStatus ExecuteCommand(String requestId) override
  {
    SerialCommandStatus status;
    status.isLongRunning = false;
    status.requestId = requestId;
    status.isOk = true;

    sineWave.initialize();
    return status;    
  }
public:

  SineWave<PERIOD>& sineWave;
  
  SineCommand(char commandLetter, SineWave<PERIOD>& sineWave);
};



template <long PERIOD>
inline SineCommand<PERIOD>::SineCommand(char commandLetter, SineWave<PERIOD> &sineWave)  : Command(commandLetter), sineWave(sineWave)
{
}

#endif