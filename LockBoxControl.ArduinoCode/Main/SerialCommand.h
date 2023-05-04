#ifndef SerialCommand_H
#define SerialCommand_H

#include <ArduinoJson.h>
/// @brief this class will handle receiving a command from the server and translates into code that can be used in the arduino.
class SerialCommand
{

public:
  char commandLetter;
  String requestId;

  bool isOk;
  String errorMessage;

  static SerialCommand serialize(String json){
    // Allocate the JSON document
    //
    // Inside the brackets, 500 is the capacity of the memory pool in bytes.
    // Don't forget to change this value to match your JSON document.
    // Use arduinojson.org/v6/assistant to compute the capacity.
    StaticJsonDocument<500> doc;
    DeserializationError error = deserializeJson(doc, json);

    SerialCommand status;
    
    if(error){
      status.isOk = false;
      status.errorMessage = String(error.c_str());
    }
    else{
      status.isOk = true;
      status.commandLetter = *(const char*)doc["commandLetter"];
      status.requestId = String((const char*)doc["requestId"]); // https://arduinojson.org/v6/error/ambiguous-overload-for-operator-equal/
    }

    return status;
  }
  
};

#endif