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
                UnitIdentifier = driverID,
                StopBits = StopBits.One,
                Parity = Parity.None
            };
        }

        void UpdateStatusCW(int text)
        {
            StatusTextCW = text;

            EventHandler handler = OnUpdateStatusCW;

            if (handler != null)
            {
                handler(null, EventArgs.Empty);
            }
        }
        void UpdateStatusCCW(int text)
        {
            StatusTextCCW = text;

            EventHandler handler = OnUpdateStatusCCW;

            if (handler != null)
            {
                handler(null, EventArgs.Empty);
            }
        }


        public void SetTorque(int value )
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

        public void SetTorqueCW(int value)
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

        public void SetTorqueCCW(int value)
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