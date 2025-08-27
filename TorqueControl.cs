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
        public bool isManuallySet { get; }

        public int MinTorque { get; set; }
        public int MaxTorque { get; set; }
        public int HydOffTorque { get; set; }

        public int AdditionalTorque { get; set; } = 0;

        public bool HasHydraulicPower { get; set; } = true;

        public int StatusTextCW { get; private set; }
        public int StatusTextCCW { get; private set; }

        public double torqueOffsetCW = 0;
        private const int Rate = 50; // Rate in milliseconds (higher is slower)
        private CancellationTokenSource CancellationTokenSource;
        public int CurrentTorque { get; private set; } = 0;

        private int lastValue = 0;


        private readonly byte driverID;
        private readonly int encoderPn;
        private CancellationTokenSource _cts;
        private CancellationTokenSource _ctsTarget;
        private Task _loopTask;
        private volatile bool _resetHomeRequested;
        private volatile int _resetHomeRequestedAddVal;
        private volatile bool _isManuallySet;

        // For change detection (optional)
        private int _prevEncoderValue = int.MinValue;

        // Active tracker for current run
        private EncoderTorqueTracker _tracker;


        public TorqueControl(ModbusClient mbc, byte driverID, bool enabled, int torqueOffsetCW)
        {
            this.enabled = enabled;
            this.torqueOffsetCW = torqueOffsetCW * 0.01;
            this.driverID = driverID;
            this.mbc = mbc;
            errorLog.onError += (message) => onError(message);
            this.encoderPn = 387;
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
            if (lastValue != value)
            {
                lastValue = value;
                SetTorques(value, value);
            }

        }

        public void SetMinMax(int min, int max)
        {
            MinTorque = min;
            MaxTorque = max;
            if (_tracker != null) {
                _tracker.SetMinMax(MinTorque, MaxTorque);
            }
        }

        public async Task SetTorqueAsync(int value)
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

            CurrentTorque = cwValue;

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
                    errorLog.DisplayError("Failed to update torques: (Servo " + mbc.UnitIdentifier + ") " + cwValue + " | " + cwValueWithOffset + " - " + ex.Message);
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


        /// <summary>
        /// Starts the background loop. If already running, throws unless you stop first.
        /// </summary>
        public void StartDynamicTorque()
        {
            if (this.enabled)
            {
                if (_loopTask != null && !_loopTask.IsCompleted)
                    throw new InvalidOperationException("Loop already running. Call StopDynamicTorqueAsync() first.");

                _cts = new CancellationTokenSource();
                _loopTask = RunLoopAsync(_cts.Token);
            }
        }

        public void StartTargetTorque(int targetTorque)
        {
            if (this.enabled)
            {
                if (_ctsTarget != null)
                {
                    _ctsTarget.Cancel();
                    _ctsTarget.Dispose();
                }
                _ctsTarget = new CancellationTokenSource();
                _ = IncreaseTorqueAsync(targetTorque, _ctsTarget.Token);
            }
        }

        /// <summary>
        /// Stops the background loop gracefully.
        /// </summary>
        public async Task StopDynamicTorqueAsync()
        {
            if (_loopTask == null) return;
            try
            {
                _cts.Cancel();
                await _loopTask.ConfigureAwait(false);
            }
            catch (OperationCanceledException) { /* expected */ }
            finally
            {
                _cts.Dispose();
                _cts = null;
                _loopTask = null;
                _tracker = null;
            }
        }

        public async Task StopTargetTorqueAsync()
        {
            try
            {
                _cts.Cancel();
            }
            catch (OperationCanceledException) { /* expected */ }
            finally
            {
                _cts.Dispose();
                _cts = null;
            }
        }

        /// <summary>
        /// Request the loop to re-set Home to the current encoder value.
        /// Takes effect on the next iteration.
        /// </summary>
        public void ResetHome()
        {
            _resetHomeRequested = true;
        }


        public void OffsetHome(int offset)
        {
            _resetHomeRequestedAddVal = offset;
        }

        /// <summary>
        /// If you have a manual override UI, set this to true to suspend auto torque updates.
        /// </summary>
        public void SetManualOverride(bool isManual)
        {
            // errorLog.DisplayInfo($"Set Torque ManualOverride: (Servo {mbc.UnitIdentifier}: {isManual}");
            _isManuallySet = isManual;
        }

        private async Task RunLoopAsync(CancellationToken token)
        {
            // Ensure connection
            if (!mbc.Connected)
                mbc.Connect();

            mbc.UnitIdentifier = driverID;

            // Create tracker and set home from first read
            _tracker = new EncoderTorqueTracker(10000, MinTorque, MaxTorque);

            int firstRead = SafeRead(encoderPn);
            _tracker.SetHome(firstRead);
            _prevEncoderValue = firstRead;

            // Initial set
            int initialTorque = _tracker.Update(firstRead);
            await SafeSetTorqueAsync(initialTorque).ConfigureAwait(false);

            // Main loop
            while (!token.IsCancellationRequested)
            {
                try
                {
                    // Brief pacing
                    await Task.Delay(10, token).ConfigureAwait(false);

                    int raw = SafeRead(encoderPn);

                    if (_resetHomeRequested)
                    {
                        _tracker.SetHome(raw);
                        _resetHomeRequested = false;
                        _prevEncoderValue = raw;

                        // int tHome = _tracker.Update(raw); // equals minTorque
                        int tHome = _tracker.Update(raw); // equals minTorque
                        if (!_isManuallySet)
                        {
                            await SafeSetTorqueAsync(tHome).ConfigureAwait(false);

                        }
                        continue; // next iteration
                    }

                    if (_resetHomeRequestedAddVal > 0)
                    {
                        var newHome = _tracker.GetHome() + _resetHomeRequestedAddVal;
                        _tracker.SetHome(newHome);
                        _resetHomeRequestedAddVal = 0;
                    }

                    int torque = _tracker.Update(raw);

                    if (!_isManuallySet && raw != _prevEncoderValue)
                    {
                        // Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {driverID} Addr {encoderPn} = {raw} -> torque {torque} - add {AdditionalTorque}");
                        await SafeSetTorqueAsync(torque + AdditionalTorque).ConfigureAwait(false);
                        _prevEncoderValue = raw;
                    }
                }
                catch (OperationCanceledException)
                {
                    // Graceful stop
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("DynamicTorque error: " + ex.Message);
                    // brief backoff to avoid tight error loop
                    await Task.Delay(100, token).ConfigureAwait(false);
                }
            }

            Console.WriteLine("DynamicTorque loop stopped.");
        }

        private int SafeRead(int address)
        {
            // Robust single-register read with reconnect
            try
            {
                var vals = mbc.ReadHoldingRegisters(address, 1);
                return vals[0];
            }
            catch
            {
                if (!mbc.Connected)
                {
                    try { 
                        mbc.Connect();
                        mbc.UnitIdentifier = driverID;
                    } catch { /* swallow; next loop will retry */ }
                }
                // Re-throw so loop handles and backs off
                throw;
            }
        }

        private Task SafeSetTorqueAsync(int torque)
        {
            // Your existing async setter; wrap if needed
            return SetTorqueAsync(torque);
        }


        private async Task IncreaseTorqueAsync(int maxTorque, CancellationToken token)
        {
            Console.WriteLine("IncreaseTorqueAsync loop started");

            // Ensure connection
            if (!mbc.Connected)
                mbc.Connect();

            mbc.UnitIdentifier = driverID;

            
            // Main loop
            while (CurrentTorque < maxTorque && !token.IsCancellationRequested)
            {
                try
                {
                    CurrentTorque++;
                    await SafeSetTorqueAsync(CurrentTorque).ConfigureAwait(false);
                    // Brief pacing
                    //await Task.Delay(2).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("DynamicTorque error: " + ex.Message);
                }
            }

            Console.WriteLine("IncreaseTorqueAsync loop stopped.");
        }
        //public void MoveToPosition(int targetPosition)
        //{
        //    try
        //    {
        //        // Split 32-bit int into two 16-bit values
        //        ushort high = (ushort)((targetPosition >> 16) & 0xFFFF);
        //        ushort low = (ushort)(targetPosition & 0xFFFF);

        //        // 1. Enable parameter write (write 1 to address 0x0000)
        //        mbc.WriteSingleRegister(0x0000, 1);
        //        Thread.Sleep(10);

        //        // 2. Write target position to Pn120/Pn121 (0x0078, 0x0079)
        //        mbc.WriteSingleRegister(0x0078, high);
        //        mbc.WriteSingleRegister(0x0079, low);
        //        Thread.Sleep(10);

        //        // 3. Trigger internal servo start (write 1 to address 0x0003)
        //        mbc.WriteSingleRegister(0x0003, 1);
        //        Thread.Sleep(100); // Adjust based on speed/distance

        //        Console.WriteLine($"Moved to target: {targetPosition}");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"MoveToPosition failed: {ex.Message}");
        //    }
        //}

        /// <summary>
        /// axisPos: 0..1024 (0 = full right, 512 = center, 1024 = full left)
        /// trim: -1..+1 (-1 = full right trim, +1 = full left trim)
        /// Returns torque in [minTorque, maxTorque].
        /// </summary>
        public async Task ComputeTorqueFromTrimAsync(int axisPos, double trim)
        {
            Console.WriteLine($"axisPos {axisPos} | trim: {trim}");

            var trimMin = -10.0;
            var trimMax = 10.0;
            var minTorque = MinTorque + 6;

            // Normalize axis to [-10, +10]
            double axisNorm = ((axisPos - 512.0) / 512.0) * trimMax;

            // Clamp trim to [-10, +10]
            double trimClamped = Clamp(trim, trimMin, trimMax);

            // Residual deflection after trim compensation
            double residual = Math.Abs(axisNorm - trimClamped);
            residual = Clamp(residual, 0.0, trimMax);

            // Linear interpolation between min and max torque
            int torque = (int)(minTorque + (MaxTorque - minTorque) * (residual / trimMax));

            Console.WriteLine($"torque {torque}");
            await SetTorqueAsync(torque);
        }


        private static double Clamp(double v, double lo, double hi)
            => v < lo ? lo : (v > hi ? hi : v);

        public void HydraulicOff()
        {
            SetManualOverride(true);
            HasHydraulicPower = false;
            //   errorLog.DisplayInfo($"Hydraulic off mode for: (Servo {mbc.UnitIdentifier}: {HydOffTorque}");
            StartTargetTorque(HydOffTorque);
        }

        public void HydraulicOn()
        {
            HasHydraulicPower = true;
            SetManualOverride(false);
        }

        public void APIsOn()
        {
            SetManualOverride(true);
            StartTargetTorque((int)(MaxTorque / 2));
        }

        public void APIsOff()
        {
            if (HasHydraulicPower)
            {
                SetManualOverride(false);
            }
        }

    }
}