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
 
| Axis        | Feature                                           | Status   |
| ----------- | ------------------------------------------------- | -------- |
| Pitch Axis  |                                                   |          |
|             | Self-centering                                    | Complete |
|             | Load increases when hydraulics are not available  | Complete |
|             | Load increases with airspeed                      | Complete |
|             | Load changes based on aft/fwrd position (v1)      | Complete |
|             | Fwd Load increases when approaching a stall       | Complete |
|             | Autopilot moves control column                    | Complete |
|             | Autopilot disengage override by moving the control| Complete |
|             | Clomun pitch stays fixed with hydraulics off      | Complete |
|             | Center calibration when starting the sim          | Complete |
|             | More accurate Load changes based on pitch position| In Prog  |
| Roll Axis   |                                                   |          |
|             | Self-centering                                    | Complete |
|             | Load increased when hydraulics are not available  | Complete |
|             | Autopilot moves control wheel                     | Complete |
|             | Autopilot disengage override by moving the control| Complete |
|             | Trim Adjustment moves Control wheel               | Complete |
|             | Center calibration when starting the sim          | Complete |
| Rudder Axis |                                                   |          |
|             | Self-centering                                    | Complete |
|             | Load increased when hydraulics are not available  | Complete |
|             | Trim Adjustment moves rudders                     | Complete |
|             | Center calibration when starting the sim          | Complete |
| Tiller      |                                                   |          |
|             | Self-centering                                    | Complete |
|             | Load increases when hydraulics are not available  | Complete |
|             | Load decreases with higher ground speed           | Complete |
|             | Center calibration when starting the sim          | Complete |
