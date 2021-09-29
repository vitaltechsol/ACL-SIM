using EasyModbus;
using System;
using System.Diagnostics;
using System.IO.Ports;

namespace LoadForceSim
{
    internal class TorqueControl
    {
        ModbusClient mbc;

        public TorqueControl(string port, byte driverID)
        {
            mbc = new ModbusClient(port)
            {
                UnitIdentifier = driverID,
                StopBits = StopBits.One,
                Parity = Parity.None
            };
        }

        
        public void SetTorque(int value )
        {
            try
            {
                mbc.Connect();
                mbc.WriteSingleRegister(8, value);
                mbc.WriteSingleRegister(9, value * -1);
                mbc.Disconnect();
                Debug.WriteLine("updated torques " + value);

            }
            catch (Exception ex)
            {
                mbc.Disconnect();
                Debug.WriteLine("failed update torques " + ex.Message);

            }
        }

        public void SetTorqueCW(int value)
        {
            try
            {
                mbc.Connect();
                mbc.WriteSingleRegister(8, value);
                mbc.Disconnect();
                Debug.WriteLine("updated torque CW " + value);
            }
            catch (Exception ex)
            {
                mbc.Disconnect();
                Debug.WriteLine("failed update torque CW " + ex.Message);
            }
        }

        public void SetTorqueCCW(int value)
        {
            try
            {
                mbc.Connect();
                mbc.WriteSingleRegister(9, value * -1);
                mbc.Disconnect();
                Debug.WriteLine("updated torque CCW " + value);

            }
            catch (Exception ex)
            {
                mbc.Disconnect();
                Debug.WriteLine("failed update torque CCW " + ex.Message);

            }
        }
    }


}