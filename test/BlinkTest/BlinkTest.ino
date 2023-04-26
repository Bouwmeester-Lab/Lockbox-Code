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
#include <SPI.h>
#include <NativeEthernet.h>

// this is a place holder mac. It is replaced by the function below when the setup is ran.
byte mac[] = { 0xDE, 0xAD, 0xBE, 0xEF, 0xFE, 0xED };

bool debug = false;

void teensyMAC(uint8_t *mac) 
{
    for(uint8_t by=0; by<2; by++) mac[by]=(HW_OCOTP_MAC1 >> ((1-by)*8)) & 0xFF;
    for(uint8_t by=0; by<4; by++) mac[by+2]=(HW_OCOTP_MAC0 >> ((3-by)*8)) & 0xFF;
    if(debug){
      Serial.printf("MAC: %02x:%02x:%02x:%02x:%02x:%02x\n", mac[0], mac[1], mac[2], mac[3], mac[4], mac[5]);      
    }    
}

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

// if you don't want to use DNS (and reduce your sketch size)
// use the numeric IP instead of the name for the server: 192.168.0.192
IPAddress server(192,168,0,192);  // numeric IP for test computer (no DNS)
// char server[] = "c285049a-dcc9-4fce-8ae3-2cd62560db06.mock.pstmn.io";    // name address for Google (using DNS)

// Initialize the Ethernet client library
// with the IP address and port of the server
// that you want to connect to (port 80 is default for HTTP):
EthernetClient client;

void setupEthernet(){
  // get the mac address from the teensy
  teensyMAC(mac);
  if(debug){
    Serial.println("Initialize Ethernet with DHCP:");
  }
  
  if (Ethernet.begin(mac) == 0) {
    if(debug){
      Serial.println("Failed to configure Ethernet using DHCP");
    }
    // Check for Ethernet hardware present
    if (Ethernet.hardwareStatus() == EthernetNoHardware) {
      if(debug){
        Serial.println("Ethernet shield was not found.  Sorry, can't run without hardware. :(");
      }
      
      while (true) {
        delay(1); // do nothing, no point running without Ethernet hardware
      }
    }
    if (Ethernet.linkStatus() == LinkOFF) {
      if(debug){
        Serial.println("Ethernet cable is not connected.");
      }
    }
    // try to congifure using IP address instead of DHCP:
    // Ethernet.begin(mac, ip, myDns);
  } else {
    if(debug){
      Serial.print("  DHCP assigned IP ");
      Serial.println(Ethernet.localIP());    
    }    
    // Serial.println(ByteArrayToHex(mac, 6));
  }
  // give the Ethernet shield a second to initialize:
  delay(1000);
}

// bool printWebData = false; // set to false if you'd rather not print the server's response
void registerMacWithServer(){
  if(debug){
    Serial.print("registering with ");
    Serial.print(server);
    Serial.println("...");
  }
  

  // if you get a connection, report back via serial:
  if (client.connect(server, 5106, false)) {
    if(debug){
      Serial.print("connected to ");
      Serial.println(client.remoteIP());
      Serial.println(client.status());
    }
    
    // Make a HTTP request:
    // Serial.printf("POST /api/Ping/%02x-%02x-%02x-%02x-%02x-%02x HTTP/1.1\n", mac[0], mac[1], mac[2], mac[3], mac[4], mac[5]);
    client.printf("POST /api/Ping/%02x-%02x-%02x-%02x-%02x-%02x HTTP/1.1", mac[0], mac[1], mac[2], mac[3], mac[4], mac[5]);
    client.println();
    // client.println("POST /api/Ping/04-e9-e5-14-b7-69 HTTP/1.1"); //Ping/04-e9-e5-14-b7-69 HTTP/1.1");
    client.printf("Host: %u.%u.%u.%u\n", server[0], server[1], server[2], server[3]);
    client.println("accept: */*");
    client.println("Connection: close");
    client.println("Content-Length: 0");
    
    client.println();
  } else {
    // if you didn't get a connection to the server:
    if(debug){
      Serial.println("connection failed");
    }
    
  }

}

void sendStatus(const char* status){
  if(client.connect(server, 5106, false)){
    if(debug){
      Serial.print("connected to ");
      Serial.println(client.remoteIP());
      Serial.println(client.status());
    }
    client.printf("POST http://%u.%u.%u.%u/api/Ping HTTP/1.1", server[0], server[1], server[2], server[3]); //Ping/04-e9-e5-14-b7-69 HTTP/1.1");
    client.println();

    char buffer[100];
    sprintf(buffer, "{\"status\":\"%s\",\"macAddress\":\"%02x-%02x-%02x-%02x-%02x-%02x\"}", status, mac[0], mac[1], mac[2], mac[3], mac[4], mac[5]);
    int length = strlen(buffer);

    client.printf("Content-Length: %i\n", length);
    client.println("Content-Type: application/json");

    client.printf("Host: %u.%u.%u.%u\n", server[0], server[1], server[2], server[3]);
    client.println("Accept: text/plain");
    
    // client.println("Connection: close");

    
    //Serial.printf("content-length: %i\n", length);
    
    client.println();
    // 
    // body
    client.println(buffer);
    // client.println();
    
  }
  else {
    // if you didn't get a connection to the server:
    if(debug){
      Serial.println("connection failed");
    }
    
  }

}

// the setup function runs once when you press reset or power the board
void setup() {
  
  setupEthernet();
  registerMacWithServer();
  // sendStatus("Locked");

  // initialize digital pin LED_BUILTIN as an output.
  pinMode(LED_BUILTIN, OUTPUT);
}

int lastStatus = 0;

// the loop function runs over and over again forever
void loop() {
  if(millis() - lastStatus > 10000)
  {
    lastStatus = millis();
    sendStatus("Locked");
  }

  // if there are incoming bytes available
  // from the server, read them and print them:
  int len = client.available();
  if (len > 0) {
    byte buffer[250];
    if (len > 250) len = 250;
    client.read(buffer, len);
    if (debug) {
      Serial.write(buffer, len); // show in the serial monitor (slows some boards)
    }
    // client.stop();
    // byteCount = byteCount + len;
  }

  
  

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
        case 'm':
          getMacAddress(command.requestId);
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

void getMacAddress(String requestId){
  MacResult status;
  status.isOk = true;
  status.requestId = requestId;
  status.sendThroughSerial();
}