using EasyModbus;
using System;
using System.IO.Ports;

namespace ACLSim
{
    internal class CustomControl
    {
        ModbusClient mbc;
        ErrorHandler errorLog = new ErrorHandler();
        public event ErrorHandler.OnError onError;
        public bool enabled = true;
        byte driverID = 0;

        public CustomControl(ModbusClient mbc, byte driverID, bool enabled)
        {
            this.enabled = enabled;
            this.mbc = mbc;
            this.driverID = driverID;
            errorLog.onError += (message) => onError(message);
        }

        // 51 - Motor running top speed
        public void SetSpeed(int value)
        {
            SetValue(51, value, 1);
        }

        public int GetSpeed()
        {
            return GetValue(51);
        }

        // 192 - Proportinal Gain
        public void SetBounceGain(int value)
        {
            SetValue(192, value, 5);
        }

        public int GetBounceGain()
        {
            return GetValue(192);
        }

        public int GetValue(int pn)
        {
            if (enabled)
            {
                try
                {
                    mbc.UnitIdentifier = driverID;
                    int[] values = mbc.ReadHoldingRegisters(pn, 1);
                    return values[0];
                }
                catch (Exception ex)
                {
                    errorLog.DisplayError("failed get value (Servo " + mbc.UnitIdentifier + ") : " + ex.Message);
                }
            }

            return 0;
        }

        public void SetValue(int pn, int value, int min)
        {
            if (enabled)
            { 
                if (value < min)
                {
                    errorLog.DisplayError("failed set value (Servo " + mbc.UnitIdentifier + ") " + pn + " to: " + value + " | Minimum is " + min);
                    return;
                }
                try
                {
                    mbc.UnitIdentifier = driverID;
                    mbc.WriteSingleRegister(pn, value);
                }
                catch (Exception ex)
                {
                    errorLog.DisplayError("failed set value (Servo " + mbc.UnitIdentifier + ") " + pn + " to: " + value  + " | " + ex.Message);
                }

            }
        }

    }


}