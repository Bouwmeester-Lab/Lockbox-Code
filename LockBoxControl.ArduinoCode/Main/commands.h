#ifndef commands_H
#define commands_H

#include <ArduinoJson.h>

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
public:
  void AddResult(DynamicJsonDocument& doc, uint8_t mac[])
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

void zeroDac(double dvolt_DAC1,  unsigned long volt_limit_up, unsigned long volt_limit_down){
  if (volt_out_DAC1 > (volt_limit_up + volt_limit_down) / 2.0 + abs(dvolt_DAC1))
        {         
            dvolt_DAC1 = -abs(dvolt_DAC1);
            volt_out_DAC1 = volt_out_DAC1 + dvolt_DAC1;
        }
        
        if (volt_out_DAC1 < (volt_limit_up + volt_limit_down) / 2.0 - abs(dvolt_DAC1))
        {
            dvolt_DAC1 = abs(dvolt_DAC1);
            volt_out_DAC1 = volt_out_DAC1 + dvolt_DAC1;
        }   
        errorsum = 0;    
        DAC1_finished = false;
}

void SendError(String error){
  SerialCommandStatus status;
  status.isOk = false;
  status.isLongRunning = false;
  status.errorMessage = error;

  status.sendThroughSerial();
}

#endif