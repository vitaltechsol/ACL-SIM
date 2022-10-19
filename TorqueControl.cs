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

        public TorqueControl(string port, byte driverID, bool enabled) : this (port, driverID)
        {
            this.enabled = enabled;
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


        public void SetTorque(int value )
        {
            SetTorques(value, value);
        }

        public void SetTorques(int cwValue, int ccwValue)
        {
            if (cwValue < 0)
            {
                cwValue = 1;
            }

            if (ccwValue < 0)
            {
                ccwValue = 1;
            }

            if (this.enabled && (StatusTextCCW != ccwValue || StatusTextCW != cwValue))
            {

                try
                {
                    if (!mbc.Connected) { 
                        mbc.Connect();
                    }
                    mbc.WriteSingleRegister(8, cwValue);
                    mbc.WriteSingleRegister(9, ccwValue * -1);
                    mbc.Disconnect();
                    UpdateStatusCW(cwValue);
                    UpdateStatusCCW(ccwValue);
                   // Debug.WriteLine("updated torques. cw: " + cwValue + "- ccw: " + ccwValue);

                }
                catch (Exception ex)
                {
                    mbc.Disconnect();
                    Debug.WriteLine("ERROR: failed update torques " + ex.Message);
                    errorLog.DisplayError("Failed to update torques: (Servo " + mbc.UnitIdentifier + ") " + ex.Message);
                }
            }
        }

        public void SetTorqueCW(int value)
        {
            if (this.enabled && StatusTextCW != value)
            {
                if (value < 0)
                {
                    value = 0;
                }
                try
                {
                    mbc.Connect();
                    mbc.WriteSingleRegister(8, value);
                    mbc.Disconnect();
                    UpdateStatusCW(value);
                  //  Debug.WriteLine("updated torque CW " + value);
                }
                catch (Exception ex)
                {
                    mbc.Disconnect();
                    Debug.WriteLine("ERROR: failed update torque CW " + ex.Message);
                    errorLog.DisplayError("Failed to update torque CW: (Servo " + mbc.UnitIdentifier + ") " + ex.Message);
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
                    mbc.Disconnect();
                    UpdateStatusCCW(value);
                    Debug.WriteLine("updated torque CCW " + value);

                }
                catch (Exception ex)
                {
                    mbc.Disconnect();
                    Debug.WriteLine("ERROR: failed update torque CCW: " + ex.Message);
                    errorLog.DisplayError("Failed to update torque CCW: (Servo " + mbc.UnitIdentifier + ") " + ex.Message);
                }
            }
        }

        public bool IsEnabled()
        {
            return this.enabled;
        }
    }


}