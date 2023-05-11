#ifndef commands_H
#define commands_H

#include <ArduinoJson.h>
#include "dac.h"

class SerialCommandStatus
{
public:
  bool isOk;
  bool isLongRunning = false;
  String errorMessage;
  String requestId;

  void sendThroughSerial(){
    DynamicJsonDocument doc(510);
    doc["isOk"] = isOk;
    doc["requestId"] = requestId;
    doc["isLongRunning"] = isLongRunning;
    if(isOk){
      doc["errorMessage"] = serialized("null");
      AddResult(doc);
    }
    else{
      doc["errorMessage"] = errorMessage;
    }
    serializeJson(doc, Serial);
    Serial.println();
  }

  virtual void AddResult(DynamicJsonDocument& doc)
  {      

  }
};

class MacResult : public SerialCommandStatus
{
private:
  uint8_t mac[6] = { 0xDE, 0xAD, 0xBE, 0xEF, 0xFE, 0xED };
public:
  void AddResult(DynamicJsonDocument& doc)
  {
    String macAddress = "";
    for(int i = 0; i < 6; i++)
    {
      if(mac[i] < 0x10) {
        macAddress += '0';
      }
      macAddress += String(mac[i], HEX);
      if(i < 5){
        macAddress += String("-");
      }
    }
    doc["result"] =  macAddress;
  }
};
/// sends the mac address through serial
void getMacAddress(String requestId){
  MacResult status;
  status.isOk = true;
  status.requestId = requestId;
  status.sendThroughSerial();
}

// void scan(){
//     if(volt_out_DAC1 > volt_limit_up || volt_out_DAC1 < volt_limit_down)        
//            dvolt_DAC1 = -dvolt_DAC1;             
          
//         volt_out_DAC1 = volt_out_DAC1 + dvolt_DAC1 + sinetable[phi][r];
//         errorsum = 0;
//         DAC1_finished = false;
// }

// void zeroDac(double dvolt_DAC1,  unsigned long volt_limit_up, unsigned long volt_limit_down){
//   if (volt_out_DAC1 > (volt_limit_up + volt_limit_down) / 2.0 + abs(dvolt_DAC1))
//         {         
//             dvolt_DAC1 = -abs(dvolt_DAC1);
//             volt_out_DAC1 = volt_out_DAC1 + dvolt_DAC1;
//         }
        
//         if (volt_out_DAC1 < (volt_limit_up + volt_limit_down) / 2.0 - abs(dvolt_DAC1))
//         {
//             dvolt_DAC1 = abs(dvolt_DAC1);
//             volt_out_DAC1 = volt_out_DAC1 + dvolt_DAC1;
//         }   
//         errorsum = 0;    
//         DAC1_finished = false;
// }

void SendError(String error){
  SerialCommandStatus status;
  status.isOk = false;
  status.isLongRunning = false;
  status.errorMessage = error;

  status.sendThroughSerial();
}

class Command{
private:
  
protected:
  virtual SerialCommandStatus ExecuteCommand(String requestId);
public:
  char commandLetter;
  Command(char commandLetter){
    this->commandLetter = commandLetter;
  }
  

  virtual void Execute(String requestId){
    auto status = ExecuteCommand(requestId);
    status.sendThroughSerial();
  }
};



class UpCommand : public Command
{
private:
  DAC* dac;
  unsigned long step_size;
protected:
  SerialCommandStatus ExecuteCommand(String requestId) override
  {
    SerialCommandStatus status;
    status.isLongRunning = false;
    status.requestId = requestId;
    if(dac->up(step_size)){
      status.isOk = true;
      
      return status;
    }
    else{
      dac->setOutputVoltage(dac->getVoltageUpperLimit());
    }
    status.isOk = false;
    status.errorMessage = "Failed to increment the voltage. The voltage might already be higher that allowed.";
    return status;    
  }
public:
  UpCommand(char commandLetter, DAC* dac, unsigned long step_size) : Command(commandLetter)
  {
    this->dac = dac;
    this->step_size = step_size;
  }
};

class DownCommand : public Command
{
private:
  DAC* dac;
  unsigned long step_size;
protected:
  SerialCommandStatus ExecuteCommand(String requestId) override
  {
    SerialCommandStatus status;
    status.isLongRunning = false;
    status.requestId = requestId;
    if(dac->down(step_size)){
      status.isOk = true;
      
      return status;
    }
    else{
      dac->setOutputVoltage(dac->getVoltageLowerLimit());
    }
    status.isOk = false;
    status.errorMessage = "Failed to decrease the voltage. The voltage might already be at the lowest that it's allowed.";
    return status;    
  }
public:
  DownCommand(char commandLetter, DAC* dac, unsigned long step_size) : Command(commandLetter)
  {
    this->dac = dac;
    this->step_size = step_size;
  }
};

class ZeroCommand : public Command
{
private:
  long slew_time = 100;
  DAC* dac1;
  DAC* dac2;
protected:
  SerialCommandStatus ExecuteCommand(String requestId) override
  {
    dac1->zeroDac(slew_time);
    dac2->zeroDac(slew_time);

    SerialCommandStatus status;
    status.isLongRunning = false;
    status.requestId = requestId;
    status.isOk = true;
    return status;    
  }
public:
  ZeroCommand(char commandLetter, DAC* dac1, DAC* dac2, long slew_time = 100) : Command(commandLetter)
  {
    this->dac1 = dac1;
    this->dac2 = dac2;
    this->slew_time = slew_time;
  }
};

#endif