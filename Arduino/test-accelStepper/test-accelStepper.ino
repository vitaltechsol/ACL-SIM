// Bounce.pde
// -*- mode: C++ -*-
//

//Take 12-24 volt power supply and wire + in to pin 9 on driver
//Ground to pin 6 to turn on motor.
//You can put extra 1 switch or industrial push button on off.For turning on off servo motor if you wish

//ground wire from arduino in to pin 14 and pin 5 of driver
//Pulse pin from arduino to pin 3 of driver
//Direction pin from arduino in to pin 4 of driver.
  
#include <AccelStepper.h>

int driverPUL = 6; // PUL- pin
int driverDIR = 7; // DIR- pin

int driverPUL2 = 8;  // PUL- pin
int driverDIR2 = 9;  // DIR- pin

int driverPUL3 = 10;  // PUL- pin
int driverDIR3 = 11;  // DIR- pin

// Define a stepper and the pins it will use
AccelStepper stepper(1, driverPUL, driverDIR); // Defaults to AccelStepper::FULL4WIRE (4 pins) on 2, 3, 4, 5
AccelStepper stepper2(1, driverPUL2, driverDIR2);
AccelStepper stepper3(1, driverPUL3, driverDIR3);

void setup()
{  
  // Change these to suit your stepper if you want
  stepper.setMaxSpeed(2600);
  stepper.setAcceleration(10000);
  stepper.moveTo(4000);

  stepper2.setMaxSpeed(2600);
  stepper2.setAcceleration(10000);
  stepper2.moveTo(4000);

  stepper3.setMaxSpeed(2600);
  stepper3.setAcceleration(10000);
  stepper3.moveTo(4000);
}

void loop()
{
    // If at the end of travel go to the other end
    if (stepper.distanceToGo() == 0)
      stepper.moveTo(-stepper.currentPosition());

    stepper.run();

    if (stepper2.distanceToGo() == 0)
      stepper2.moveTo(-stepper2.currentPosition());

    stepper2.run();

    if (stepper3.distanceToGo() == 0)
      stepper3.moveTo(-stepper3.currentPosition());

    stepper3.run();
}
