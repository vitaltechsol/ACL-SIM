using ProSimSDK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
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
        int offset = 0;

        public AxisControl(string movePrefix, bool enabled)
        {
            errorLog.onError += (message) => onError(message);

            this.enabled = enabled;
            this.movePrefix = movePrefix;
        }

        public void SetPort(SerialPort port, ProSimConnect connection)
        {
            this.port = port;
            this.connection = connection;
        }

        public void MoveTo(double value)
        {
            string arduLine =  "<" + movePrefix + ", 0, " + (value + offset) + ">";
            port.Write(arduLine);
        }


        public void CenterAxis(string refName, int target, int dir)
        {

            offset = 0;
            int posOffset = 0;
            int moveFactor = 10;
            string directing = "CW";
            int axisPosition;
            bool move = true;
            errorLog.DisplayInfo("Center calibration started " + movePrefix);

            while (move)
            {
                axisPosition = int.Parse(connection.ReadDataRef(refName).ToString());

                //  errorLog.DisplayInfo("axisPosition " + axisPosition);
                //  errorLog.DisplayInfo("posOffset " + posOffset);

                MoveTo(posOffset * dir);

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

                if (axisPosition == target)
                {
                    move = false;
                    errorLog.DisplayInfo("Center calibration complete " + movePrefix + ":" + posOffset);
                    offset = posOffset;
                }

                if (offset > 35000 || offset < -35000)
                {
                    errorLog.DisplayError("Maximun reached, could not center " + posOffset);
                }

            }
        }
    }
   
}
