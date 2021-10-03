using EasyModbus;
using System;
using System.IO.Ports;

namespace LoadForceSim
{
    internal class SpeedControl
    {
        ModbusClient mbc; 

       public SpeedControl(string port, byte driverID)
        {
            mbc = new ModbusClient(port)
            {
                UnitIdentifier = driverID,
                StopBits = StopBits.One,
                Parity = Parity.None
            };
        }


        public void SetSpeed(int value)
        {
            try
            {
                mbc.Connect();
                // 51 - Motor running top speed
                mbc.WriteSingleRegister(51, value);
                mbc.Disconnect();
            }
            catch (Exception ex)
            {
                mbc.Disconnect();
            }
        }
    }


}