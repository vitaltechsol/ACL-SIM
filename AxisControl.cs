using ProSimSDK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ACLSim
{
    class AxisControl
    {

        ErrorHandler errorLog = new ErrorHandler();
        public event ErrorHandler.OnError onError;
        bool enabled;
        SerialPort port;
        string movePrefix;
        string axisName;
        ProSimConnect connection;
        int axisOfset = 0;
        int hydOffPosition = 0;
        bool axisCentered = false;
        public bool isCentering { get; set; } = false;
        public bool isTrimming { get; set; } = false;

        private int defaultSpeed {get; set;} = 50;
        public int currentSpeed { get; set; } = 0;
        bool hydraulicPower = false;

        int direction;

        private CancellationTokenSource trimCancelToken;
        private Task trimTask;
        private double currentPosition = 0;
        private const int TrimDelayMs = 5; // 20ms = ~50 updates/sec
                                           //private const double StepSize = 5.0;
        private const double StepSize = 50.0;
        private DateTime lastTrimTime;
        private Timer torqueUpdateTimer;
        private Action onTrimSettled; // callback set by main form
        private const int TrimSettleTimeoutMs = 500;

        public AxisControl(string movePrefix, string axisName, int direction, int defaultSpeed, bool enabled)
        {
            errorLog.onError += (message) => onError(message);

            this.enabled = enabled;
            this.axisName = axisName;
            this.movePrefix = movePrefix;
            this.direction = direction;
            this.defaultSpeed = defaultSpeed;
            // If not enabled always make it centered
            if (!enabled)
            {
                axisCentered = true;
            } 
        }

        public void SetPort(SerialPort port, ProSimConnect connection)
        {
            if (enabled) { 
                this.port = port;
                this.connection = connection;
                ChangeAxisSpeed(defaultSpeed);
            }
        }

        public bool HydraulicPower
        {
            get { return hydraulicPower; }
            set { hydraulicPower = value; }
        }

        public bool AxisCentered
        {
            get { return axisCentered; }
            set { axisCentered = value; }
        }

        public void MoveTo(double value)
        {
            if (enabled)
            {
                string arduLine = "<" + movePrefix + ", 0, " + (value + ((axisOfset) * direction)) + ">";
                try
                {
                    if (port != null)
                    {
                        port.Write(arduLine);
                        // errorLog.DisplayInfo($"Move {movePrefix} to {value} with offset {axisOfset}");
                    }
                    else
                    {
                        errorLog.DisplayError("Arduino COM port is null " + movePrefix);
                    }
                }
                catch (Exception ex)
                {
                    errorLog.DisplayError("MoveTo: Cannot connect to Arduino COM port. " + this.port.PortName + " sending: " + movePrefix + " " + ex.Message);
                }
            }
        }

        public async Task MoveToHome()
        {
            if (!enabled)
            {
                return;
            }

            string arduLine = "<" + movePrefix + ", 0, " + 0 + ">";
            try
            {
                port.Write(arduLine);
                errorLog.DisplayInfo("Moving to Home. " + axisName);

            }
            catch (Exception ex)
            {
                errorLog.DisplayError("MoveToHome: Cannot connect to Arduino COM port. " + axisName + " " + ex.Message);
            }

            await Task.Delay(4000);

        }

        public void MoveToHydPos(bool axisDroppedByWind)
        {
            if (!enabled)
            {
                return;
            }

            if (hydraulicPower)
            {
                errorLog.DisplayInfo("Hydraulics On, moving " + axisName + "To 0");
                MoveTo(0);
            }
            else
            {
                if (hydOffPosition != 0 && axisDroppedByWind)
                {
                    errorLog.DisplayInfo("Hydraulics Off, moving " + axisName);
                    MoveTo(hydOffPosition);
                }

            }
        }

        public async Task CenterAxis(string refName, int moveFactor, bool axisDroppedByWind)
        {
            if (!enabled)
            {
                axisCentered = true;
                return;
            }

            var stopwatch = Stopwatch.StartNew(); // Start timing

            axisOfset = 0;
            int posOffset = 0;
            string directing = "CW";
            int axisPosition;
            bool move = true;
            int target = 512;
            errorLog.DisplayInfo("Center calibration started " + axisName);

            using (CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(20)))
            {
                CancellationToken token = cts.Token;

                try
                {

                    while (move)
                    {
                        token.ThrowIfCancellationRequested();

                        axisPosition = int.Parse(connection.ReadDataRef(refName).ToString());

                        // errorLog.DisplayInfo("axisPosition " + movePrefix + " " + axisPosition);
                        // delay for servo to move
                        await Task.Delay(100);

                        //errorLog.DisplayInfo("posOffset " + posOffset);

                        MoveTo(posOffset * direction);

                        // Move on direction, if passes target move opposite directions
                        if (axisPosition > target)
                        {
                            if (directing != "CW")
                            {
                                directing = "CW";
                                // If direction changes, reduce the moving amount for precision
                                if (moveFactor > 1)
                                {
                                    moveFactor -= 1;
                                }

                            }
                            posOffset -= moveFactor;
                        }
                        else
                        {
                            if (directing != "CCW")
                            {
                                directing = "CCW";
                                if (moveFactor > 1)
                                {
                                    moveFactor -= 1;
                                }

                            }
                            posOffset += moveFactor;
                        }

                        if (axisPosition == target || axisPosition == target + 1 || axisPosition == target - 1)
                        {
                            move = false;
                            errorLog.DisplayInfo($"Center calibration completed {axisName}: {posOffset} for {stopwatch.Elapsed.TotalSeconds.ToString("F2")} seconds. Target {target}, position: {axisPosition}");
                            axisOfset = posOffset;
                            axisCentered = true;
                            if (axisDroppedByWind)
                            {
                                MoveToHydPos(axisDroppedByWind);
                            }
                        }

                        if (posOffset > 100000 || posOffset < -100000)
                        {
                            move = false;
                            axisCentered = true;
                            errorLog.DisplayError("Maximum reached, could not center (try reversing direction) " + axisName + " : " + posOffset);
                        }

                    }
                }
                catch (OperationCanceledException)
                {
                    axisCentered = true;
                    errorLog.DisplayError($"Center calibration timed out for {axisName}: " + stopwatch.Elapsed.TotalSeconds.ToString("F2") + " seconds");
                    errorLog.DisplayError($"Could not center (try reversing direction or increasing Center_Calibration_Speed_{axisName}");
                }
                catch (Exception ex)
                {
                    axisCentered = true;
                    errorLog.DisplayError("Cannot center calibrate controls." + ex.Message + ": " + stopwatch.Elapsed.TotalSeconds.ToString("F2") + " seconds");
                }
            }
        }

        //public async Task TrimToPositionAsync(double targetPosition)
        //{
        //    // Cancel previous trim
        //    trimCancelToken?.Cancel();

        //    trimCancelToken = new CancellationTokenSource();
        //    var token = trimCancelToken.Token;

        //    isTrimming = true;

        //    trimTask = Task.Run(async () =>
        //    {
        //        try
        //        {
        //            while (Math.Abs(currentPosition - targetPosition) > StepSize)
        //            {
        //                token.ThrowIfCancellationRequested();

        //                double direction = Math.Sign(targetPosition - currentPosition);
        //                currentPosition += direction * StepSize;
        //                MoveTo(currentPosition);

        //                await Task.Delay(TrimDelayMs, token);
        //            }

        //            // Final adjustment to target
        //            MoveTo(targetPosition);
        //            currentPosition = targetPosition;
        //        }
        //        catch (OperationCanceledException)
        //        {
        //            // Gracefully exit
        //        }
        //        finally
        //        {
        //            isTrimming = false;
        //        }
        //    });
        //}


        public async Task TrimToPositionAsync(double targetPosition)
        {

            lastTrimTime = DateTime.UtcNow;
            ResetTorqueTimer(); // restart debounce timer

            // Cancel any existing trimming task
            trimCancelToken?.Cancel();

            trimCancelToken = new CancellationTokenSource();
            var token = trimCancelToken.Token;

            isTrimming = true;

            trimTask = Task.Run(async () =>
            {
                try
                {
                    while (Math.Abs(currentPosition - targetPosition) > StepSize)
                    {
                        token.ThrowIfCancellationRequested();

                        double direction = Math.Sign(targetPosition - currentPosition);
                        currentPosition += direction * StepSize;
                        MoveTo(currentPosition);

                        await Task.Delay(TrimDelayMs, token);
                    }

                    // Final move to target
                    currentPosition = targetPosition;
                    MoveTo(currentPosition);
                }
                catch (OperationCanceledException)
                {
                    // Trimming interrupted gracefully
                }
                finally
                {
                    isTrimming = false;
                }
            });
        }

        private void ResetTorqueTimer()
        {
            torqueUpdateTimer?.Dispose();
            torqueUpdateTimer = new Timer(state =>
            {
                if (DateTime.UtcNow - lastTrimTime >= TimeSpan.FromMilliseconds(TrimSettleTimeoutMs))
                {
                    onTrimSettled?.Invoke();
                }
            }, null, TrimSettleTimeoutMs, Timeout.Infinite);
        }

        public void SetOnTrimSettled(Action callback)
        {
            onTrimSettled = callback;
        }


        // Pitch Speed
        public void ChangeAxisSpeed(int value)
        {
            if (!enabled)
            {
                return;
            }

            if (value < 0) value = 0;
            else if (value > 100) value = 100;
            currentSpeed = value;
            errorLog.DisplayInfo("Changing Axis Speed " + axisName + ": " + currentSpeed);

            // 5000 scale
            value = value * 50; 

            string arduLine = "<" + movePrefix + "_SPEED, 0, " + value + ">";
            try
            {
                port.Write(arduLine);
            }
            catch (Exception ex)
            {
                errorLog.DisplayError("Cannot connect to Arduino COM port. -ChangeAxisSpeed " + ex.Message);
            }
        }

        public void ResetAxisSpeed()
        {
            ChangeAxisSpeed(defaultSpeed);
        }

        public void SetDefaultSpeed(int speed)
        {
            this.defaultSpeed = speed;
        }
            

        public bool IsEnabled()
        {
            return enabled;
        }

    }
   
}
