       // -*- mode: C++ -*-
//

//Take 12-24 volt power supply and wire + in to pin 9 on driver
//Ground to pin 6 to turn on motor.
//You can put extra 1 switch or industrial push button on off.For turning on off servo motor if you wish

//ground wire from arduino in to pin 14 and pin 5 of driver
//Pulse pin from arduino to pin 3 of driver
//Direction pin from arduino in to pin 4 of driver.

//https://www.airspayce.com/mikem/arduino/AccelStepper/index.html
#include <AccelStepper.h>

// Roll
int driverPUL = 8;    // PUL- pin
int driverDIR = 9;    // DIR- pin

// Pitch
int driverPUL2 = 10;    // PUL- pin
int driverDIR2 = 11;    // DIR- pin

// Yaw
int driverPUL3 = 6;    // PUL- pin
int driverDIR3 = 7;    // DIR- pin

// Tiller
int driverPUL4 = 4;    // PUL- pin
int driverDIR4 = 5;    // DIR- pin

const int LED_PIN = 13;
const int TQ_PIN = 12;
const int vlt = 2;

int Xdest = 0;
int Ydest = 0;
int Zdest = 0;

// Define a stepper and the pins it will use
AccelStepper stepper(1, driverPUL, driverDIR); 
AccelStepper stepper2(1, driverPUL2, driverDIR2); 
AccelStepper stepper3(1, driverPUL3, driverDIR3); 
AccelStepper stepper4(1, driverPUL4, driverDIR4); 

int speedval = 800;


const byte numChars = 32;
char receivedChars[numChars];
char tempChars[numChars];        // temporary array for use when parsing

// variables to hold the parsed data
char messageFromPC[numChars] = {0};
int integerFromPC = 0;
float floatFromPC = 0.0;

boolean newData = false;


// the setup function runs once when you press reset or power the board
void setup() {

  pinMode( LED_PIN, OUTPUT );
  pinMode( TQ_PIN, OUTPUT );

  // Change these to suit your stepper if you want
  stepper.setMaxSpeed(8000);
  stepper.setAcceleration(8000);

  stepper2.setMaxSpeed(8000);
  stepper2.setAcceleration(8000);

  stepper3.setMaxSpeed(8000);
  stepper3.setAcceleration(8000);

  stepper4.setMaxSpeed(8000);
  stepper4.setAcceleration(8000);

  stepper.moveTo(0);
  stepper2.moveTo(0);
  stepper3.moveTo(0);
  stepper4.moveTo(0);

  Serial.begin(115200);
}

// the loop function runs over and over again forever
void loop() {

  analogWrite( LED_PIN, vlt );
  analogWrite( TQ_PIN, vlt );

  recvWithStartEndMarkers();
  if (newData == true) {
    strcpy(tempChars, receivedChars);
    // this temporary copy is necessary to protect the original data
    //   because strtok() used in parseData() replaces the commas with \0
    parseData();
    showParsedData();
    newData = false;
  }

  stepper.run();
  stepper2.run();
  stepper3.run();
  stepper4.run();
}


void recvWithStartEndMarkers() {
  static boolean recvInProgress = false;
  static byte ndx = 0;
  char startMarker = '<';
  char endMarker = '>';
  char rc;

  while (Serial.available() > 0 && newData == false) {
    rc = Serial.read();

    if (recvInProgress == true) {
      if (rc != endMarker) {
        receivedChars[ndx] = rc;
        ndx++;
        if (ndx >= numChars) {
          ndx = numChars - 1;
        }
      }
      else {
        receivedChars[ndx] = '\0'; // terminate the string
        recvInProgress = false;
        ndx = 0;
        newData = true;
      }
    }

    else if (rc == startMarker) {
      recvInProgress = true;
    }
  }
}

//============

void parseData() {      // split the data into its parts

  char * strtokIndx; // this is used by strtok() as an index

  strtokIndx = strtok(tempChars, ",");     // get the first part - the string
  strcpy(messageFromPC, strtokIndx); // copy it to messageFromPC

  strtokIndx = strtok(NULL, ","); // this continues where the previous call left off
  integerFromPC = atoi(strtokIndx);     // convert this part to an integer

  strtokIndx = strtok(NULL, ",");
  floatFromPC = atof(strtokIndx);     // convert this part to a float

}

//============

void showParsedData() {
//    Serial.print("Message ");
//    Serial.println(messageFromPC);
//    Serial.print("Integer ");
//    Serial.println(integerFromPC);
//    Serial.print("Float ");
//    Serial.println(floatFromPC);

  // Roll
  if (strcmp(messageFromPC, "X_POS") == 0) {
    Xdest = floatFromPC;
    stepper.moveTo(floatFromPC);
  }

  if (strcmp(messageFromPC, "X_POS_SPEED") == 0) {
    stepper.setMaxSpeed(floatFromPC);
    stepper.setAcceleration(floatFromPC);
  }

  // Pitch
  if (strcmp(messageFromPC, "Y_POS") == 0) {
    Ydest = floatFromPC;
    stepper2.moveTo(floatFromPC);
  }

  if (strcmp(messageFromPC, "Y_POS_SPEED") == 0) {
    stepper2.setMaxSpeed(floatFromPC);
    stepper2.setAcceleration(floatFromPC);
  }

  // Yaw
  if (strcmp(messageFromPC, "Z_POS") == 0) {
    Zdest = floatFromPC;
    stepper3.moveTo(floatFromPC);
  }

  if (strcmp(messageFromPC, "Z_POS_SPEED") == 0) {
    stepper3.setMaxSpeed(floatFromPC);
    stepper3.setAcceleration(floatFromPC);
  }

  // Tiller
  if (strcmp(messageFromPC, "Tiller_POS") == 0) {
    Zdest = floatFromPC;
    stepper4.moveTo(floatFromPC);
  }

  if (strcmp(messageFromPC, "Tiller_POS_SPEED") == 0) {
    stepper4.setMaxSpeed(floatFromPC);
    stepper4.setAcceleration(floatFromPC);
  }
}
