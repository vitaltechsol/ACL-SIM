using EasyModbus;
using System;
using System.IO.Ports;

namespace LoadForceSim
{
    internal class CustomControl
    {
        ModbusClient mbc; 

       public CustomControl(string port, byte driverID)
        {
            mbc = new ModbusClient(port)
            {
                Baudrate = 115200,
                UnitIdentifier = driverID,
                StopBits = StopBits.One,
                Parity = Parity.None
            };
        }

        // 51 - Motor running top speed
        public void SetSpeed(int value)
        {
            SetValue(51, value);
        }

        public int GetSpeed()
        {
            return GetValue(51);
        }

        // 192 - Proportinal Gain
        public void SetBounceGain(int value)
        {
            SetValue(192, value);
        }

        public int GetBounceGain()
        {
            return GetValue(192);
        }

        public int GetValue(int pn)
        {
            try
            {
                mbc.Connect();
                // 51 - Motor running top speed
                int[] values =  mbc.ReadHoldingRegisters(pn, 1);
                mbc.Disconnect();
                return values[0];
            }
            catch (Exception ex)
            {
                mbc.Disconnect();
            }

            return 0;
        }

        public void SetValue(int pn, int value)
        {
            try
            {
                mbc.Connect();
                mbc.WriteSingleRegister(pn, value);
                mbc.Disconnect();
            }
            catch (Exception ex)
            {
                mbc.Disconnect();
            }
        }

    }


}