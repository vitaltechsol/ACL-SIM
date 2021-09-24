using EasyModbus;
using System;
using System.IO.Ports;

namespace LoadForceSim
{
    internal class TorqueControl
    {
        ModbusClient mbc;

        public TorqueControl(string port)
        {
            mbc = new ModbusClient(port)
            {
                UnitIdentifier = 2,
                StopBits = StopBits.Two,
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
            }
            catch (Exception ex)
            {
                mbc.Disconnect();
            }
        }
    }


}