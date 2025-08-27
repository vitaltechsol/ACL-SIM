using EasyModbus;
using System;
using System.IO.Ports;
using System.Threading.Tasks;

namespace ACLSim
{
    internal class CustomControl
    {
        ModbusClient mbc;
        ErrorHandler errorLog = new ErrorHandler();
        public event ErrorHandler.OnError onError;
        public bool enabled = true;
        byte driverID = 0;
        int prevSpeed = 0;

        public CustomControl(ModbusClient mbc, byte driverID, bool enabled)
        {
            this.enabled = enabled;
            this.mbc = mbc;
            this.driverID = driverID;
            errorLog.onError += (message) => onError(message);
        }

        // 51 - Motor running top speed
        public void SelfCentering(int value)
        {
            if (prevSpeed != value && enabled)
            {
                errorLog.DisplayInfo($"Set self-centering speed for (Servo {driverID}) to {value}");
                SetValue(51, value, 0);
                prevSpeed = value;
            }
        }

        public async Task SetSpeedAsync(int value)
        {
            await Task.Run(() => SelfCentering(value));
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
            if (enabled && mbc != null)
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
            if (enabled && mbc != null)
            { 
                if (value < min)
                {
                    errorLog.DisplayError("failed set custom value (Servo " + mbc.UnitIdentifier + ") " + pn + " to: " + value + " | Minimum is " + min);
                    return;
                }
                try
                {
                    mbc.UnitIdentifier = driverID;
                    mbc.WriteSingleRegister(pn, value);
                    errorLog.DisplayInfo("Set custom value (Servo " + mbc.UnitIdentifier + ") " + pn + " to: " + value);

                }
                catch (Exception ex)
                {
                    errorLog.DisplayError("failed set custom value (Servo " + mbc.UnitIdentifier + ") " + pn + " to: " + value  + " | " + ex.Message);
                }

            }
        }

    }


}