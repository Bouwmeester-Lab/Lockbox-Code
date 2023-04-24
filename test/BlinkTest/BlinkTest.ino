/*
  Blink

  Turns an LED on for one second, then off for one second, repeatedly.

  Most Arduinos have an on-board LED you can control. On the UNO, MEGA and ZERO
  it is attached to digital pin 13, on MKR1000 on pin 6. LED_BUILTIN is set to
  the correct LED pin independent of which board is used.
  If you want to know what pin the on-board LED is connected to on your Arduino
  model, check the Technical Specs of your board at:
  https://www.arduino.cc/en/Main/Products

  modified 8 May 2014
  by Scott Fitzgerald
  modified 2 Sep 2016
  by Arturo Guadalupi
  modified 8 Sep 2016
  by Colby Newman

  This example code is in the public domain.

  https://www.arduino.cc/en/Tutorial/BuiltInExamples/Blink
*/

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
    }
    else{
      doc["errorMessage"] = errorMessage;
    }
     
    serializeJson(doc, Serial);
    Serial.println();
  }
};

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
    // Inside the brackets, 200 is the capacity of the memory pool in bytes.
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

// the setup function runs once when you press reset or power the board
void setup() {
  // initialize digital pin LED_BUILTIN as an output.
  pinMode(LED_BUILTIN, OUTPUT);
}

// the loop function runs over and over again forever
void loop() {
  if(Serial.available()){
    auto c = Serial.readStringUntil('\n');
    SerialCommand command = SerialCommand::serialize(c);
    if(command.isOk){
      switch(command.commandLetter){
        case 's':
          TurnOnLed(command.requestId);
          break;
        case 'l':
          TurnOffLed(command.requestId);
          break;
        default:
          SendError("unkown command");
          break;
      }
    }
    else{
      SendError(command.errorMessage);
    }
  }
}

void SendError(String error){
  SerialCommandStatus status;
  status.isOk = false;
  status.isLongRunning = false;
  status.errorMessage = error;

  status.sendThroughSerial();
}

void TurnOffLed(String requestId){
  digitalWrite(LED_BUILTIN, LOW);
  SerialCommandStatus status;
  status.isOk = true;
  status.requestId = requestId;
  status.sendThroughSerial();
}

void TurnOnLed(String requestId){
  digitalWrite(LED_BUILTIN, HIGH);
  SerialCommandStatus status;
  status.isOk = true;
  status.requestId = requestId;
  status.sendThroughSerial();
}