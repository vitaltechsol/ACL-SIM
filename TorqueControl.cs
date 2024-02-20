using EasyModbus;
using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading.Tasks;

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
        byte driverID = 0;

        public int StatusTextCW { get; private set; }
        public int StatusTextCCW { get; private set; }

        public double torqueOffsetCW = 0;

        public TorqueControl(ModbusClient mbc, byte driverID, bool enabled, int torqueOffsetCW) 
        {
            this.enabled = enabled;
            this.torqueOffsetCW = torqueOffsetCW * 0.01;
            this.driverID = driverID;
            this.mbc = mbc;
            errorLog.onError += (message) => onError(message);

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

        public async void SetTorqueAsync(int value)
        {
            await Task.Run(() => SetTorques(value, value));
        }
        public async void SetTorqueCWAsync(int value)
        {
            await Task.Run(() => SetTorqueCW(value));
        }

        public async void SetTorqueCCWAsync(int value)
        {
            await Task.Run(() => SetTorqueCCW(value));
        }

        public async void SetTorques(int cwValue, int ccwValue)
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

                try { 

                    mbc.UnitIdentifier = driverID;
                    mbc.WriteSingleRegister(8, cwValueWithOffset);
                    mbc.WriteSingleRegister(9, ccwValue * -1);
                    UpdateStatusCW(cwValueWithOffset);
                    UpdateStatusCCW(ccwValue);
                    // errorLog.DisplayInfo("Update torque cwValueWithOffset: (Servo " + mbc.UnitIdentifier + ") " + cwValueWithOffset);
                    // errorLog.DisplayInfo("Update torque ccwValue: (Servo " + mbc.UnitIdentifier + ") " + ccwValue);
                    // Debug.WriteLine("updated torques. " + driverID + " cw: " + cwValue + "- ccw: " + ccwValue);

                }
                catch (Exception ex)
                {
                    Debug.WriteLine("ERROR: failed update torques " + ex.Message);
                    errorLog.DisplayError("Failed to update torques: (Servo " + mbc.UnitIdentifier + ") " + cwValue  + " | " + ccwValue + " - " + ex.Message);
                }
            }
        }

        public async void SetTorqueCW(int cwValue)
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
                    mbc.UnitIdentifier = driverID;

                    mbc.WriteSingleRegister(8, cwValueWithOffset);
                    UpdateStatusCW(cwValueWithOffset);
                    //  Debug.WriteLine("updated torque CW " + value);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("ERROR: failed update torque CW " + ex.Message);
                    errorLog.DisplayError("Failed to update torque CW: (Servo " + mbc.UnitIdentifier + ") " + ex.Message);
                }
               
            }
        }

        public async void SetTorqueCCW(int value)
        {
            if (this.enabled && StatusTextCCW != value)
            {
                if (value < 0)
                {
                    value = 0;
                }
                try
                {
                    mbc.UnitIdentifier = driverID;
                    mbc.WriteSingleRegister(9, value * -1);
                    UpdateStatusCCW(value);
                    Debug.WriteLine("updated torque CCW " + value);

                }
                catch (Exception ex)
                {
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