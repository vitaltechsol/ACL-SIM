using EasyModbus;
using System;
using System.Diagnostics;
using System.IO.Ports;

namespace LoadForceSim
{
    internal class TorqueControl
    {
        ModbusClient mbc;
        public event EventHandler OnUpdateStatusCW;
        public event EventHandler OnUpdateStatusCCW;

        public int StatusTextCW { get; private set; }
        public int StatusTextCCW { get; private set; }

        public TorqueControl(string port, byte driverID)
        {
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
            if (StatusTextCCW != value || StatusTextCW != value)
            {
                try
                {
                    mbc.Connect();
                    mbc.WriteSingleRegister(8, value);
                    mbc.WriteSingleRegister(9, value * -1);
                    mbc.Disconnect();
                    UpdateStatusCW(value);
                    UpdateStatusCCW(value);
                    Debug.WriteLine("updated torques " + value);

                }
                catch (Exception ex)
                {
                    mbc.Disconnect();
                    Debug.WriteLine("ERROR: failed update torques " + ex.Message);

                }
            }
        }

        public void SetTorques(int cwValue, int ccwValue)
        {
            if (StatusTextCCW != ccwValue || StatusTextCW != cwValue)
            {
                try
                {
                    mbc.Connect();
                    mbc.WriteSingleRegister(8, cwValue);
                    mbc.WriteSingleRegister(9, ccwValue * -1);
                    mbc.Disconnect();
                    UpdateStatusCW(cwValue);
                    UpdateStatusCCW(ccwValue);
                    Debug.WriteLine("updated torques. cw: " + cwValue + "- ccw: " + ccwValue);

                }
                catch (Exception ex)
                {
                    mbc.Disconnect();
                    Debug.WriteLine("ERROR: failed update torques " + ex.Message);

                }
            }
        }

        public void SetTorqueCW(int value)
        {
            if (StatusTextCW != value)
            {
                try
                {
                    mbc.Connect();
                    mbc.WriteSingleRegister(8, value);
                    mbc.Disconnect();
                    UpdateStatusCW(value);
                    Debug.WriteLine("updated torque CW " + value);
                }
                catch (Exception ex)
                {
                    mbc.Disconnect();
                    Debug.WriteLine("ERROR: failed update torque CW " + ex.Message);
                }
            }
        }

        public void SetTorqueCCW(int value)
        {
            if (StatusTextCCW != value)
            {
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
                    Debug.WriteLine("ERROR: failed update torque CCW " + ex.Message);

                }
            }
        }
    }


}