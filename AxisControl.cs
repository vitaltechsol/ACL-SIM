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
        string axisName;
        ProSimConnect connection;
        int axisOfset = 0;
        int hydOffPosition = 0;
        bool axisCentered = false;
        bool hydraulicPower = false;

        int direction;

        public AxisControl(string movePrefix, string axisName, int direction, int hydOffPosition, bool enabled)
        {
            errorLog.onError += (message) => onError(message);

            this.enabled = enabled;
            this.axisName = axisName;
            this.movePrefix = movePrefix;
            this.direction = direction;
            this.hydOffPosition = hydOffPosition;
            // If not enabled always make it centered
            if (!enabled)
            {
                axisCentered = true;
            }
        }

        public void SetPort(SerialPort port, ProSimConnect connection)
        {
            if (enabled) { 
                this.port = port;
                this.connection = connection;
            }
        }

        public bool HydraulicPower
        {
            get { return hydraulicPower; }
            set { hydraulicPower = value; }
        }

        public bool AxisCentered
        {
            get { return axisCentered; }
            set { axisCentered = value; }
        }

        public void MoveTo(double value)
        {
            if (enabled)
            {
                string arduLine = "<" + movePrefix + ", 0, " + (value + ((axisOfset) * direction)) + ">";
                try
                {
                    if (port != null)
                    {
                        port.Write(arduLine);
                    }
                    else
                    {
                        errorLog.DisplayError("Arduino COM port is null " + movePrefix);
                    }
                }
                catch (Exception ex)
                {
                    errorLog.DisplayError("Cannot connect to Arduino COM port. " + movePrefix + " " + ex.Message);
                }
            }
        }

        public void MoveToHome()
        {
            if (!enabled)
            {
                return;
            }

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

        public void MoveToHydPos(bool axisDroppedByWind)
        {
            if (!enabled)
            {
                return;
            }

            if (hydraulicPower)
            {
                errorLog.DisplayInfo("Hydraulics On, moving " + axisName + "To 0");
                MoveTo(0);
            }
            else
            {
                if (hydOffPosition != 0 && axisDroppedByWind)
                {
                    errorLog.DisplayInfo("Hydraulics Off, moving " + axisName);
                    MoveTo(hydOffPosition);
                }

            }
        }



        public async void CenterAxis(string refName, int moveFactor, bool axisDroppedByWind)
        {
            if (!enabled)
            {
                axisCentered = true;
                return;
            }

            axisOfset = 0;
            int posOffset = 0;
            string directing = "CW";
            int axisPosition;
            bool move = true;
            int target = 512;
            errorLog.DisplayInfo("Center calibration started " + axisName);

            try
            {

                while (move)
                {
                    axisPosition = int.Parse(connection.ReadDataRef(refName).ToString());

                    // errorLog.DisplayInfo("axisPosition " + movePrefix + " " + axisPosition);
                    // delay for servo to move
                    await Task.Delay(100);

                    //errorLog.DisplayInfo("posOffset " + posOffset);

                    MoveTo(posOffset * direction);

                    // Move on direction, if passes target move opposite directions
                    if (axisPosition > target)
                    {
                        if (directing != "CW")
                        {
                            directing = "CW";
                            // If direction changes, reduce the moving amount for precision
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
                        errorLog.DisplayInfo("Center calibration completed " + axisName + ":" + posOffset);
                        axisOfset = posOffset;
                        axisCentered = true;
                        if (axisDroppedByWind)
                        {
                            MoveToHydPos(axisDroppedByWind);
                        }
                    }

                    if (posOffset > 100000 || posOffset < -100000)
                    {
                        move = false;
                        axisCentered = true;
                        errorLog.DisplayError("Maximum reached, could not center (try reversing direction) " + axisName + " : " + posOffset);
                    }

                }
            }
            catch (Exception ex)
            {
                errorLog.DisplayError("Cannot center calibrate controls." + ex.Message);
            }
        }

        // Pitch Speed
        public void ChangeAxisSpeed(double value)
        {
            if (!enabled)
            {
                return;
            }

            string arduLine = "<" + movePrefix + "_SPEED, 0, " + value + ">";
            try
            {
                port.Write(arduLine);
            }
            catch (Exception ex)
            {
                errorLog.DisplayError("Cannot connect to Arduino COM port. -ChangeAxisSpeed " + ex.Message);
            }
        }

        public bool IsEnabled()
        {
            return enabled;
        }

    }
   
}
