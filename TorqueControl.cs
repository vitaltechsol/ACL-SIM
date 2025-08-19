using EasyModbus;
using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

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
        public bool isManuallySet { get; set; }

        public int StatusTextCW { get; private set; }
        public int StatusTextCCW { get; private set; }

        public double torqueOffsetCW = 0;
        private const int Rate = 50; // Rate in milliseconds (higher is slower)
        private CancellationTokenSource CancellationTokenSource;
        public int CurrentTorque  { get; private set; } = 0;

        private int lastValue = 0;

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
            if (lastValue != value )
            {
                lastValue = value;
                SetTorques(value, value);
            }
           
        }

        public async void SetTorqueAsync(int value)
        {
            if (lastValue != value)
            {
                lastValue = value;
                await Task.Run(() => SetTorques(value, value));
            }
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
                    if (!mbc.Connected)
                    {
                        mbc.Connect();
                    }
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
                    errorLog.DisplayError("Failed to update torques: (Servo " + mbc.UnitIdentifier + ") " + cwValue  + " | " + cwValueWithOffset + " - " + ex.Message);
                    mbc.Disconnect();
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

        public async void SetToTorqueTarget(int target)
        {
            CancellationTokenSource?.Cancel(); // Cancel previous task if any

            CancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = CancellationTokenSource.Token;


            if (CurrentTorque == target)
                return; // No need to change torque

            int increment = target > CurrentTorque ? 1 : -1;

            while (CurrentTorque != target)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                CurrentTorque += increment;
                SetTorqueAsync(CurrentTorque);

                await Task.Delay(Rate);
            }
        }

        public bool IsEnabled()
        {
            return this.enabled;
        }


        static int GetActualPos(ModbusClient mbc)
        {
            try { 
            int[] regs = mbc.ReadHoldingRegisters(0x1002, 2);
            return (short)regs[0] << 16 | (ushort)regs[1];
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetActualPos failed: " + ex.Message);
            }
            return 0;
        }

        public void DynamicTorque(int minTorque, int maxTorque)
        {
            var addr = 387;
            var tracker = new EncoderTorqueTracker(10000, minTorque, maxTorque);

            if (!mbc.Connected)
            {
                mbc.Connect();
            }
            mbc.UnitIdentifier = driverID;

            // Optional: set the first read as home
            int firstRead = mbc.ReadHoldingRegisters(addr, 1)[0];
            tracker.SetHome(firstRead);

            int prevValue = 0;
            while (true)
            {
                try
                {
                    int value = mbc.ReadHoldingRegisters(addr, 1)[0];
                    int torque = (int)tracker.Update(value);
                    if (prevValue != value && !isManuallySet)
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {driverID} Addr {addr} = {value} - to torque {torque}");
                        SetTorqueAsync(torque);
                        prevValue = value;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
                Thread.Sleep(10); // Adjust as needed
            }
        }

        public void MoveTo(int targetPosition, bool waitUntilReached = false, int tolerance = 50)
        {
            Debug.WriteLine($"Start MoveTo: {targetPosition}");
            errorLog.DisplayInfo($" Start MoveTo: {targetPosition}");
            // Split 32-bit target into two 16-bit registers
            ushort high = (ushort)((targetPosition >> 16) & 0xFFFF);
            ushort low = (ushort)(targetPosition & 0xFFFF);

            //mbc.WriteMultipleRegisters(REG_TARGET_POS_HIGH, new int[] { high, low });
            //mbc.WriteSingleRegister(REG_TARGET_POS_HIGH, high);
            //mbc.WriteSingleRegister(REG_TARGET_POS_LOW, low);
            //mbc.WriteSingleRegister(0x0010, 0x0000);
            //mbc.WriteSingleRegister(0x0011, 0x0100);
        }

        public void ReadAllValues()
        {
            if (!mbc.Connected)
            {
                mbc.Connect();
            }
            mbc.UnitIdentifier = driverID;

            Console.WriteLine("Connected.");
            mbc.WriteSingleRegister(0, 1); // Pn000 = 1

            int[] addresses = new int[]
        {
            379,
            383, 387,
            390, 391
        };

        //    {
        //        369, 370, 371, 372, 379,
        //    383, 384, 385, 386, 387,
        //    390, 391, 407, 410
        //};
            Console.WriteLine("Monitoring input registers... Press Ctrl+C to stop.\n");

            while (true)
            {
                try
                {
                    foreach (int addr in addresses)
                    {
                        int[] values = mbc.ReadHoldingRegisters(addr, 1);
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Addr {addr} = {values[0]}");
                    }

                    Console.WriteLine("------");
                    Thread.Sleep(250); // Adjust as needed
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        
        


            // Many libraries use 0-based addressing for Modbus tables.
            // We'll scan Input Registers [0..600] and compare 32-bit pairs.
            const int MAX = 600;

            int[] baseHi = new int[MAX + 1];
            int[] baseLo = new int[MAX + 1];
            for (int i = 0; i <= MAX; i++)
            {
                try
                {
                    baseHi[i] = mbc.ReadHoldingRegisters(i, 1)[0];
                    Console.WriteLine($"base {i} = {baseHi[i]}");
                }
                catch { baseHi[i] = int.MinValue; baseLo[i] = int.MinValue; }
            }

            Console.WriteLine("Now move the motor by hand (4 seconds)...");
            Thread.Sleep(14000);

            Console.WriteLine("Scanning for changes...");
            for (int i = 0; i < MAX; i++)
            {
                try
                {
                    int hiAfter = mbc.ReadHoldingRegisters(i, 1)[0];

                    // Combine into signed 32-bit

                    if (baseHi[i] != hiAfter)
                    {
                        Console.WriteLine($"CHANGE @ InputRegs {i}: before: {baseHi[i]} after: {hiAfter}");
                    }
                }
                catch { /* ignore */ }
            }

            mbc.Disconnect();
            Console.WriteLine("Done.");


            return;

        }
    }
}