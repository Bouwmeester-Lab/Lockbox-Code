#ifndef ethernetCommunication_H
#define ethernetCommunication_H

#include <NativeEthernet.h>

/// Gets the mac from the teensy
/// Saves it into the array mac provided.
void teensyMAC(uint8_t *mac, bool debug = false) 
{
    for(uint8_t by=0; by<2; by++) mac[by]=(HW_OCOTP_MAC1 >> ((1-by)*8)) & 0xFF;
    for(uint8_t by=0; by<4; by++) mac[by+2]=(HW_OCOTP_MAC0 >> ((3-by)*8)) & 0xFF;
    if(debug){
      Serial.printf("MAC: %02x:%02x:%02x:%02x:%02x:%02x\n", mac[0], mac[1], mac[2], mac[3], mac[4], mac[5]);      
    }    
}

void setupEthernet(uint8_t mac[], bool debug = false){
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

void registerMacWithServer(EthernetClient& client, const IPAddress& server, const uint port, uint8_t mac[], bool tls = false, bool debug = false){
  if(debug){
    Serial.print("registering with ");
    Serial.print(server);
    Serial.println("...");
  }
  

  // if you get a connection, report back via serial:
  if (client.connect(server, port, tls)) {
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

#endif