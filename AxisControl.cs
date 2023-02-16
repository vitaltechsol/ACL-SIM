using ProSimSDK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ACLSim
{
    class AxisControl
    {
        ErrorHandler errorLog = new ErrorHandler();
        public event ErrorHandler.OnError onError;
        bool enabled;
        SerialPort port;
        string movePrefix;
        ProSimConnect connection;
        int axisOfset = 0;
        int direction;

        public AxisControl(string movePrefix, int direction, bool enabled)
        {
            errorLog.onError += (message) => onError(message);

            this.enabled = enabled;
            this.movePrefix = movePrefix;
            this.direction = direction;
        }

        public void SetPort(SerialPort port, ProSimConnect connection)
        {
            this.port = port;
            this.connection = connection;
        }

        public void MoveTo(double value)
        {
            string arduLine =  "<" + movePrefix + ", 0, " + (value + (axisOfset * direction)) + ">";
            try
            {
                port.Write(arduLine);
            }
            catch (Exception ex)
            {
                errorLog.DisplayError("Cannot connect to Arduino COM port. " + ex.Message);
            }
        }

        public void MoveToHome()
        {
            string arduLine = "<" + movePrefix + ", 0, " + 0 + ">";
            try
            {
                port.Write(arduLine);
            }
            catch (Exception ex)
            {
                errorLog.DisplayError("Cannot connect to Arduino COM port. " + ex.Message);
            }
        }


        public async void CenterAxis(string refName, int target, int moveFactor)
        {

            axisOfset = 0;
            int posOffset = 0;
            string directing = "CW";
            int axisPosition;
            bool move = true;
            errorLog.DisplayInfo("Center calibration started " + movePrefix);

            while (move)
            {
                axisPosition = int.Parse(connection.ReadDataRef(refName).ToString());
                
                // errorLog.DisplayInfo("axisPosition " + movePrefix + " " + axisPosition);
                // delay for servo to move
                await Task.Delay(100);
                
                //errorLog.DisplayInfo("posOffset " + posOffset);

                MoveTo(posOffset * direction);

                // Move on direction, if passes target move oposite directions
                if (axisPosition > target)
                {
                    if (directing != "CW")
                    {
                        directing = "CW";
                        // If direction changes, reduce the moving amount for precission
                        if (moveFactor > 1)
                        {
                            moveFactor -= 1;
                        }

                    }
                    posOffset -= moveFactor;
                }
                else
                {
                    if (directing != "CCW")
                    {
                        directing = "CCW";
                        if (moveFactor > 1)
                        {
                            moveFactor -= 1;
                        }

                    }
                    posOffset += moveFactor;
                }

                if (axisPosition == target || axisPosition == target + 1 || axisPosition == target - 1)
                {
                    move = false;
                    errorLog.DisplayInfo("Center calibration completed " + movePrefix + ":" + posOffset);
                    axisOfset = posOffset;
                }

                if (posOffset > 35000 || posOffset < -35000)
                {
                    move = false;
                    errorLog.DisplayError("Maximun reached, could not center " + posOffset);
                }

            }
        }

        // Pitch Speed
        public void ChangeAxisSpeed(double value)
        {
            string arduLine = "<" + movePrefix + "_SPEED, 0, " + value + ">";
            try
            {
                port.Write(arduLine);
            }
            catch (Exception ex)
            {
                errorLog.DisplayError("Cannot connect to Arduino COM port. " + ex.Message);
            }
        }
    }
   
}
