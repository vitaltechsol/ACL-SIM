# ACL-SIM
 Active Control Loading for Flight Simulator.

[Download Software manual](https://docs.google.com/document/d/1KazFxLndUraUICVV142zfowjpmU_UAafAoSC2X-yo0w/edit?usp=sharing)

## Software Required:
- Prosim-AR 737
- Flight Simulator 2020 or P3D v4+
- [ACL software](https://github.com/vitaltechsol/ACL-SIM/releases)

## Hardware Required:
- AC Servo Motors
- Arduino Mega
- RS485 Controller
[Hardware installation](https://fabianb.medium.com/a-d862f927d0bf)


## Features 
 
| Axis        | Feature                                         | Status   |
| ----------- | ----------------------------------------------- | -------- |
| Pitch Axis  |                                                 |          |
|             | Self-centering                                  | Complete |
|             | Autopilot Follow                                | Complete |
|             | Reposition when Hydraulics are available        | Complete |
|             | Load change when Hydraulics are available       | Complete |
|             | Load change when Hydraulics are Not available   | Complete |
|             | Autopilot disengage override                    | Complete |
|             | Forces increase and decreases (Airspeed)        | Complete |
|             | Elevator Trim                                   | Complete |
|             | Pitch Limit (Avoid Stalling)                    | Complete |
|             | Center Calibration when starting the sim        | Complete |
| Roll Axis   |                                                 |          |
|             | Self-centering                                  | Complete |
|             | Autopilot Follow                                | Complete |
|             | Trim Adjustment                                 | Complete |
|             | Autopilot disengage override                    | Complete |
|             | Center Calibration when starting the sim        | Complete |
| Rudder Axis |                                                 |          |
|             | Rudders move left when hydraulics are off       | Complete |
|             | Load change when hydraulics are not available   | Complete |
|             | Rudder pedals move with rudder trim             | Complete |
|             | Center Calibration when starting the sim        | Complete |
| Tiller      |                                                 |          |
|             | Self-centering                                  | Complete |
|             | Load change when hydraulics are not available   | Complete |
|             | Load change depending on ground speed           | Complete |
|             | Center Calibration when starting the sim        | Complete |
