using EasyModbus;
using System;
using System.Diagnostics;
using System.IO.Ports;

namespace ACLSim
{
    internal class TorqueControl
    {
        ModbusClient mbc;
        public event EventHandler OnUpdateStatusCW;
        public event EventHandler OnUpdateStatusCCW;
        ErrorHandler errorLog = new ErrorHandler();
        public event ErrorHandler.OnError onError;
        public bool enabled = true;

        public int StatusTextCW { get; private set; }
        public int StatusTextCCW { get; private set; }

        public double torqueOffsetCW = 0;

        public TorqueControl(string port, byte driverID, bool enabled, int torqueOffsetCW) : this(port, driverID)
        {
            this.enabled = enabled;
            this.torqueOffsetCW = torqueOffsetCW * 0.01;
        }
        public TorqueControl(string port, byte driverID)
        {
            errorLog.onError += (message) => onError(message);

            mbc = new ModbusClient(port)
            {
                Baudrate = 115200,
                UnitIdentifier = driverID,
                StopBits = StopBits.One,
                Parity = Parity.None
            };
        }

        void UpdateStatusCW(int value)
        {
            StatusTextCW = value;

            EventHandler handler = OnUpdateStatusCW;

            if (handler != null)
            {
                handler(null, EventArgs.Empty);
            }
        }
        void UpdateStatusCCW(int value)
        {
            StatusTextCCW = value;

            EventHandler handler = OnUpdateStatusCCW;

            if (handler != null)
            {
                handler(null, EventArgs.Empty);
            }
        }


        public void SetTorque(int value)
        {
            SetTorques(value, value);
        }

        public void SetTorques(int cwValue, int ccwValue)
        {
            int cwValueWithOffset = cwValue + Convert.ToInt32(torqueOffsetCW * cwValue);
            if (cwValueWithOffset < 0)
            {
                cwValueWithOffset = 1;
            }

            if (ccwValue < 0)
            {
                ccwValue = 1;
            }

            if (this.enabled && (StatusTextCCW != cwValueWithOffset || StatusTextCW != cwValue))
            {

                try
                {
                    mbc.Connect();
                    mbc.WriteSingleRegister(8, cwValueWithOffset);
                    mbc.WriteSingleRegister(9, ccwValue * -1);
                    UpdateStatusCW(cwValueWithOffset);
                    UpdateStatusCCW(ccwValue);
                    // errorLog.DisplayInfo("Update torque cwValueWithOffset: (Servo " + mbc.UnitIdentifier + ") " + cwValueWithOffset);
                    // errorLog.DisplayInfo("Update torque ccwValue: (Servo " + mbc.UnitIdentifier + ") " + ccwValue);
                    // Debug.WriteLine("updated torques. cw: " + cwValue + "- ccw: " + ccwValue);

                }
                catch (Exception ex)
                {
                    Debug.WriteLine("ERROR: failed update torques " + ex.Message);
                    errorLog.DisplayError("Failed to update torques: (Servo " + mbc.UnitIdentifier + ") " + ex.Message);
                }
                finally
                {
                    mbc.Disconnect();
                }
            }
        }

        public void SetTorqueCW(int cwValue)
        {
            int cwValueWithOffset = cwValue + Convert.ToInt32(torqueOffsetCW * cwValue);
    

            if (this.enabled && StatusTextCW != cwValueWithOffset)
            {
                if (cwValueWithOffset < 0)
                {
                    cwValueWithOffset = 0;
                }
                try
                {
                    mbc.Connect();
                    mbc.WriteSingleRegister(8, cwValueWithOffset);
                    UpdateStatusCW(cwValueWithOffset);
                    //  Debug.WriteLine("updated torque CW " + value);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("ERROR: failed update torque CW " + ex.Message);
                    errorLog.DisplayError("Failed to update torque CW: (Servo " + mbc.UnitIdentifier + ") " + ex.Message);
                }
                finally
                {
                    mbc.Disconnect();
                }
            }
        }

        public void SetTorqueCCW(int value)
        {
            if (this.enabled && StatusTextCCW != value)
            {
                if (value < 0)
                {
                    value = 0;
                }
                try
                {
                    mbc.Connect();
                    mbc.WriteSingleRegister(9, value * -1);
                    UpdateStatusCCW(value);
                    Debug.WriteLine("updated torque CCW " + value);

                }
                catch (Exception ex)
                {
                    Debug.WriteLine("ERROR: failed update torque CCW: " + ex.Message);
                    errorLog.DisplayError("Failed to update torque CCW: (Servo " + mbc.UnitIdentifier + ") " + ex.Message);
                }
                finally
                {
                    mbc.Disconnect();
                }
            }
        }

        public bool IsEnabled()
        {
            return this.enabled;
        }
    }


}