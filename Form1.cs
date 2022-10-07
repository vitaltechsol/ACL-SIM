using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ProSimSDK;
using static ACLSim.ErrorHandler;
using Timer = System.Timers.Timer;

namespace ACLSim
{

    public partial class Form1 : Form
    {
        // Our main ProSim connection
        // Y is pitch
        // X is roll
        // Z is yaw

        ProSimConnect connection = new ProSimConnect();
        Dictionary<String, DataRefTableItem> dataRefs = new Dictionary<string, DataRefTableItem>();
        static string portName = "COM3";
        static int torqueRollLow = 18;
        static int torqueRollHigh = 65;
        static int additionalAirSpeedTorque = 0;
        static int additionalElevatorTorque = 0;
        static int additionalPitchTorque = 0;
        static string mbusPort = "COM4";

        int torqueFactorAirSpeed = 10;
        int trimFactorElevator = 1200;
        int trimFactorAileron = 1000;
        int trimFactorRudder = 1000;
        int torquePitchLow = 25;
        int torquePitchHigh = 55;
        int torquePitchMax = 70;
        int torqueYawHigh = 40;
        int torqueYawLow = 20;

        int centeringSpeedPitch = 0;
        int centeringSpeedRoll = 0;
        int centeringSpeedYaw = 0;
        int dampeningPitch = 0;
        int dampeningRoll = 0;
        int dampeningYaw = 0;

        int directionAxisPitch;
        int directionAxisRoll;
        int directionAxisYaw;

        static SerialPort port;
        int baud = 115200;
        bool isRollCMD = false;
        bool isPitchCMD = false;
        bool isHydAvail = false;

        int apPositionRollFactor = 700;
        bool sendDataAPDisconnect = true;
        Timer timerX;
        Timer timerY;
        Timer timerZ;
        Timer timerAPdiconnect;
        static bool sendDataX = false;
        static bool sendDataY = false;
        static bool sendDataZ = false;

        TorqueControl torquePitch = new TorqueControl(mbusPort, 
            Convert.ToByte(Properties.Settings.Default.Driver_Pitch_ID));
        TorqueControl torqueRoll = new TorqueControl(mbusPort,
            Convert.ToByte(Properties.Settings.Default.Driver_Roll_ID));
        TorqueControl torqueYaw = new TorqueControl(mbusPort,
            Convert.ToByte(Properties.Settings.Default.Driver_Yaw_ID), 
            Properties.Settings.Default.Enable_Yaw_ACL);

        CustomControl speedPitch = new CustomControl(mbusPort,
            Convert.ToByte(Properties.Settings.Default.Driver_Pitch_ID));
        CustomControl speedRoll = new CustomControl(mbusPort,
            Convert.ToByte(Properties.Settings.Default.Driver_Roll_ID));
        CustomControl speedYaw = new CustomControl(mbusPort,
            Convert.ToByte(Properties.Settings.Default.Driver_Yaw_ID),
            Properties.Settings.Default.Enable_Yaw_ACL);

        AxisControl axisPitch = new AxisControl("Y_POS", Properties.Settings.Default.DirectionAxisPitch, Properties.Settings.Default.Enable_Pitch_ACL);
        AxisControl axisRoll = new AxisControl("X_POS", Properties.Settings.Default.DirectionAxisRoll, Properties.Settings.Default.Enable_Roll_ACL);
        AxisControl axisYaw = new AxisControl("Z_POS", Properties.Settings.Default.DirectionAxisYaw, Properties.Settings.Default.Enable_Yaw_ACL);

        ErrorHandler errorh = new ErrorHandler();

        int lastRollMoved = -1;
        int lastPitchMoved = -1;
        int apDisconnetRollThreshold;
        int apDisconnetPitchThreshold;

        public Form1()
        {
            InitializeComponent();
            errorh.onError += (msg) => ShowFormError(msg);
            torquePitch.onError += (msg) => ShowFormError(msg);
            torqueRoll.onError += (msg) => ShowFormError(msg);
            torqueYaw.onError += (msg) => ShowFormError(msg);
            axisRoll.onError += (msg) => ShowFormError(msg);
            axisYaw.onError += (msg) => ShowFormError(msg);
            axisPitch.onError += (msg) => ShowFormError(msg);



            speedPitch.onError += (msg) => ShowFormError(msg);
            speedRoll.onError += (msg) => ShowFormError(msg);
            speedYaw.onError += (msg) => ShowFormError(msg);


            // Add event to update the torque status
            torquePitch.OnUpdateStatusCCW += (sender1, e1) => UpdateTorquePitchLabelBack(torquePitch.StatusTextCCW);
            torquePitch.OnUpdateStatusCW += (sender1, e1) => UpdateTorquePitchLabelFwd(torquePitch.StatusTextCW);

            // Register to receive connect and disconnect events
            connection.onConnect += connection_onConnect;
            connection.onDisconnect += connection_onDisconnect;
            timerX = new Timer();
            timerX.Interval = 100;
            timerX.Start();
            timerX.Elapsed += sendDataOK_X;
            timerX.AutoReset = true;

            timerY = new Timer();
            timerY.Interval = 100;
            timerY.Start();
            timerY.Elapsed += sendDataOK_Y;
            timerY.AutoReset = true;
            
            timerZ = new Timer();
            timerZ.Interval = 100;
            timerY.Elapsed += sendDataOK_Z;

            timerAPdiconnect = new Timer();
            timerAPdiconnect.Interval = 2000;
            timerAPdiconnect.Elapsed += sendDataAPDisconnectOK;

            //if (SerialPort.GetPortNames().Count() >= 0)
            //{
            //    foreach (string p in SerialPort.GetPortNames())
            //    {
            //        Debug.WriteLine(p);
            //    }
            //}

            BeginSerial(baud, portName);
            try
            {
                port.Open();
            }
            catch (Exception ex)
            {
              errorh.DisplayError("Cannot connect to com port. " + ex.Message);
            }
            axisRoll.SetPort(port, connection);
            axisYaw.SetPort(port, connection);
            axisPitch.SetPort(port, connection);


            dataRefView.Hide();
        }

        private void Form1_Shown(Object sender, EventArgs e)
        {
            propertyGridSettings.SelectedObject = Properties.Settings.Default;
            propertyGridSettings.BrowsableAttributes = new AttributeCollection(new UserScopedSettingAttribute());

            SetAppSettings();
            // Reset position
            moveToX(0);
            moveToY(0);
            moveToZ(0);
        }

        private void SetAppSettings()
        {
            hostnameInput.Text = Properties.Settings.Default.ProSimIP;
            chkAutoConnect.Checked = Properties.Settings.Default.AutoConnect;

            torqueFactorAirSpeed = Properties.Settings.Default.TorqueFactor_AirSpeed;

            torquePitchLow = Properties.Settings.Default.Torque_Pitch_Low;
            torquePitchHigh = Properties.Settings.Default.Torque_Pitch_High;
            torquePitchMax = Properties.Settings.Default.Torque_Pitch_Max;

            torqueRollLow = Properties.Settings.Default.Torque_Roll_Low;
            torqueRollHigh = Properties.Settings.Default.Torque_Roll_High;

            torqueYawLow = Properties.Settings.Default.Torque_Yaw_Low;
            torqueYawHigh = Properties.Settings.Default.Torque_Yaw_High;

            trimFactorElevator = Properties.Settings.Default.TrimFactor_Elevator;
            trimFactorAileron = Properties.Settings.Default.TrimFactor_Aileron;
            trimFactorRudder = Properties.Settings.Default.TrimFactor_Rudder;

            apDisconnetRollThreshold = Properties.Settings.Default.APDisconnetRollThreshold;
            apDisconnetPitchThreshold = Properties.Settings.Default.APDisconnetPitchThreshold;

            apPositionRollFactor = Properties.Settings.Default.APPosition_Roll_Factor;

            torqueYaw.enabled = Properties.Settings.Default.Enable_Yaw_ACL;
            speedYaw.enabled = Properties.Settings.Default.Enable_Yaw_ACL;

            torquePitch.enabled = Properties.Settings.Default.Enable_Pitch_ACL;
            speedPitch.enabled = Properties.Settings.Default.Enable_Pitch_ACL;

            torqueRoll.enabled = Properties.Settings.Default.Enable_Roll_ACL;
            speedRoll.enabled = Properties.Settings.Default.Enable_Roll_ACL;

            centeringSpeedPitch = Properties.Settings.Default.CenteringSpeedPitch;
            centeringSpeedRoll = Properties.Settings.Default.CenteringSpeedRoll;
            centeringSpeedYaw = Properties.Settings.Default.CenteringSpeedYaw;

            dampeningPitch = Properties.Settings.Default.DampeningPitch;
            dampeningRoll = Properties.Settings.Default.DampeningRoll;
            dampeningYaw = Properties.Settings.Default.DampeningYaw;

            directionAxisPitch = Properties.Settings.Default.DirectionAxisRoll;
            directionAxisRoll = Properties.Settings.Default.DirectionAxisRoll;
            directionAxisYaw = Properties.Settings.Default.DirectionAxisYaw;


            // Values from settings
            speedPitch.SetSpeed(centeringSpeedPitch);
            speedPitch.SetBounceGain(dampeningPitch);
            speedRoll.SetSpeed(centeringSpeedRoll);
            speedRoll.SetBounceGain(dampeningRoll);
            speedYaw.SetSpeed(centeringSpeedYaw);
            speedYaw.SetBounceGain(dampeningYaw);


            if (Properties.Settings.Default.AutoConnect)
            {
                connectToProSim();
            }

        }

  
        private void sendDataOK_X(object sender, System.Timers.ElapsedEventArgs e)
        {
            sendDataX = true;
        }

        private void sendDataOK_Y(object sender, System.Timers.ElapsedEventArgs e)
        {
            sendDataY = true;
        }

        private void sendDataOK_Z(object sender, System.Timers.ElapsedEventArgs e)
        {
            timerZ.Stop();
            sendDataZ = true;
        }

        private void sendDataAPDisconnectOK(object sender, System.Timers.ElapsedEventArgs e)
        {
            timerAPdiconnect.Stop();
            sendDataAPDisconnect = true;
        }


        static void BeginSerial(int baud, string name) => port = new SerialPort(name, baud);


        private void connectButton_Click(object sender, EventArgs e)
        {
            // Save
            Properties.Settings.Default.ProSimIP = hostnameInput.Text;
            Properties.Settings.Default.Save();
            connectToProSim();
        }


        void connectToProSim()
        {
            try
            {
                connection.Connect(hostnameInput.Text);
                updateStatusLabel();
            }
            catch (Exception ex)
            {
                updateStatusLabel();
                errorh.DisplayError("Cannot connect to Prosim. " + ex.Message);
            }
        }

        void connection_onDisconnect()
        {
            Invoke(new MethodInvoker(updateStatusLabel));
            port.Close();
        }

        // When we connect to ProSim737 system, update the status label and start filling the table
        void connection_onConnect()
        {
            Invoke(new MethodInvoker(updateStatusLabel));
            Invoke(new MethodInvoker(fillDataRefTable));

        }

        void updateStatusLabel()
        {
            connectionStatusLabel.Text = "Connection status: " + (connection.isConnected ? "Connected" : "Disconnected");
            connectButton.Enabled = !connection.isConnected;
        }


        void fillDataRefTable()
        {
            fillTableWorker.RunWorkerAsync();
        }


        // Fill the table with DataRefs
        private void fillTableWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get all of the DataRefs from ProSim737 System
            DataRefDescription[] descriptions = connection.getDataRefDescriptions().ToArray();

            DataRef dataRef = new DataRef("", 100, connection);

            this.add_data_ref(DayaRefNames.AILERON_LEFT);
            this.add_data_ref(DayaRefNames.AILERON_RIGHT);

            this.add_data_ref(DayaRefNames.ELEVATOR);
            this.add_data_ref(DayaRefNames.TRIM_ELEVATOR);
            this.add_data_ref(DayaRefNames.TRIM_AILERON);
            this.add_data_ref(DayaRefNames.TRIM_RUDDER);

            this.add_data_ref(DayaRefNames.ROLL_CMD);
            this.add_data_ref(DayaRefNames.PITCH_CMD);
            this.add_data_ref(DayaRefNames.MCP_AP_DISENGAGE);

            this.add_data_ref(DayaRefNames.THRUST_1);
            this.add_data_ref(DayaRefNames.THRUST_2);
            this.add_data_ref(DayaRefNames.SPEED_IAS);

            this.add_data_ref(DayaRefNames.HYDRAULICS_AVAILABLE);
            this.add_data_ref(DayaRefNames.HYD_PRESS);

            this.add_data_ref(DayaRefNames.AILERON_CPTN);
            this.add_data_ref(DayaRefNames.ELEVATOR_CPTN);
            this.add_data_ref(DayaRefNames.RUDDER_CAPT);

        }

        private void add_data_ref(string dataRefName)
        {
            // If we don't yet have this DataRef, add it to the table
            if (!dataRefs.ContainsKey(dataRefName))
            {
                // Request a new DataRef from ProSim737 System with 100 msec update interval
                DataRef dataRef = new DataRef(dataRefName, 100, connection);

                // Register to receive updates of the value of the DataRef
                dataRef.onDataChange += dataRef_onDataChange;

                // Create a DataRefTableItem, so it can be displayed in the table
                DataRefTableItem item = new DataRefTableItem() { dataRef = dataRef, Description = "", DataType = "" };
                lock (dataRefs)
                    dataRefs[dataRefName] = item;

                // Use Invoke() to add the item to the table and store the index of the new table row
                Invoke(new MethodInvoker(delegate () { item.index = dataRefTableItemBindingSource.Add(item); }));
            }
        }

      

        void dataRef_onDataChange(DataRef dataRef)
        {
            if (IsDisposed)
                return;

            String name = dataRef.name;
            // Check to make sure the DataRef is somewhere in the table
            if (dataRefs.ContainsKey(name))
            {
                // Get associated table item
                DataRefTableItem item = dataRefs[name];

                // Set the value of the table item to the new value
                try
                {
                    item.Value = Convert.ToDouble(dataRef.value);
                    switch (name)
                    {

                        case DayaRefNames.AILERON_LEFT:
                            {
                                if (isRollCMD == true && sendDataX == true)
                                {
                                    double xValue = Math.Round(item.Value * (apPositionRollFactor * 10));
                                    // Skip sudden jumps to 0
                                    if (xValue != 0)
                                    {
                                        item.valueAdjusted = xValue;
                                        moveToX(xValue);
                                        sendDataX = false;
                                    }

                                }
                                break;

                            }

                        case DayaRefNames.TRIM_ELEVATOR:
                            {

                                if ( sendDataY== true)
                                {
                                    double speed = dataRefs[DayaRefNames.SPEED_IAS].Value;

                                    if (speed > 80)
                                    {
                                        double pitchValue = Math.Round((item.Value * trimFactorElevator) * 100);
                                        // Skip sudden jumps to 0
                                        if (pitchValue != 0)
                                        {
                                            item.valueAdjusted = pitchValue;
                                            moveToY(pitchValue * directionAxisPitch);
                                            sendDataY = false;
                                        }
                                    }
                                }
                                      
                                break;
                           }

                        case DayaRefNames.TRIM_AILERON:
                            {

                                if (sendDataX == true)
                                {
                                    double rollValue = Math.Round(item.Value * trimFactorAileron);
                                    // Skip sudden jumps to 0
                                    if (rollValue != 0)
                                    {
                                        item.valueAdjusted = rollValue * -1 * directionAxisRoll;
                                        moveToX(rollValue * -1 * directionAxisRoll);
                                        sendDataX = false;
                                    }
                                    
                                }

                                break;
                            }

                        case DayaRefNames.TRIM_RUDDER:
                            {

                                if (sendDataZ == true)
                                {
                                    double yawValue = Math.Round(item.Value * trimFactorRudder);
                                    // Skip sudden jumps to 0
                                    if (yawValue != 0)
                                    {
                                        item.valueAdjusted = yawValue * directionAxisYaw;
                                        moveToZ(yawValue * directionAxisYaw);
                                        sendDataZ = false;
                                        timerZ.Start();
                                    }

                                }
                                2
                                break;
                            }

                        case DayaRefNames.ELEVATOR:
                            {
                                double newVal = (18 * item.Value);
                                if (newVal < 0)
                                {
                                    newVal = newVal * -1;
                                }
                                double diff = (newVal - additionalElevatorTorque);

                                if (diff >= 2 || diff <= -2 ||  additionalElevatorTorque == 0)
                                    {
                                       // errorh.DisplayInfo("Additional " + additionalElevatorTorque);
                                       additionalElevatorTorque = (int)newVal;
                                       UpdatePitchTorques();
                                    }
                                break;
                            }

                        case DayaRefNames.SPEED_IAS:
                            {
                                item.valueAdjusted = Math.Round((item.Value - 140) / torqueFactorAirSpeed);

                                if (additionalAirSpeedTorque != item.valueAdjusted)
                                {
                                    additionalAirSpeedTorque = Convert.ToInt32(item.valueAdjusted);
                                    if (additionalAirSpeedTorque > 0)
                                    {
                                        UpdatePitchTorques();
                                    }
                                }

                                break;
                            }
                      

                        case DayaRefNames.ROLL_CMD:
                            {
                                isRollCMD = Convert.ToBoolean(dataRef.value);
                                if (isRollCMD == false)
                                {
                                    // Reset Position
                                    moveToX(0);
                                } else
                                {
                                    // This timer is used to avoid disconnecting
                                    // inmediatly after first connecting
                                    Debug.WriteLine("Don't allow mnual AP disc " + isRollCMD);
                                    sendDataAPDisconnect = false;
                                    timerAPdiconnect.Start();
                                }
                                
                                UpdateRollTorques();

                                Debug.WriteLine("updated isPitchCMD " + isRollCMD);
                                break;
                            }

                        case DayaRefNames.PITCH_CMD:
                            {
                                isPitchCMD = Convert.ToBoolean(dataRef.value);

                                if (isPitchCMD == false)
                                {
                                    // Reset Position
                                    moveToY(0);
                                }
                                {
                                    // This timer is used to avoid disconnecting
                                    // inmediatly after first connecting
                                    Debug.WriteLine("Don't allow mnual AP disc " + isRollCMD);
                                    sendDataAPDisconnect = false;
                                    timerAPdiconnect.Start();
                                }

                                UpdatePitchTorques();
                                Debug.WriteLine("updated isPitchCMD " + isPitchCMD);
                                break;
                            }

                        case DayaRefNames.MCP_AP_DISENGAGE:
                        {
                                bool isDisengaged = Convert.ToBoolean(dataRef.value);
                                errorh.DisplayInfo("MCP A/P switch disengaged " + isDisengaged);
                                if (isDisengaged)
                                {
                                    moveToX(0);
                                    moveToY(0);
                                    isPitchCMD = false;
                                    isRollCMD = false;
                                }
                                break;
                        }

                        case DayaRefNames.HYDRAULICS_AVAILABLE:
                            {
                                isHydAvail = Convert.ToBoolean(dataRef.value);
                                DataRefTableItem airSpeed = dataRefs[DayaRefNames.SPEED_IAS];
                                Debug.WriteLine("updated isHydAvail " + isHydAvail);
                                double airSpeedValue = Convert.ToDouble(airSpeed.Value);

                                // move if on ground
                                if (airSpeedValue < 30)
                                {
                                    torquePitch.SetTorque(torquePitchHigh);
                                    torqueYaw.SetTorque(torqueYawHigh);

                                    if (!isHydAvail)
                                    {
                                        moveToY(Properties.Settings.Default.Position_Pitch_HYD_OFF_Max);
                                        moveToZ(Properties.Settings.Default.Position_Yaw_HYD_OFF_Max);
                                    }
                                    else
                                    {
                                        // reset position
                                        torquePitch.SetTorque(torquePitchHigh);
                                        changeSpeedPitch(80000);
                                        moveToY(0);
                                        moveToZ(0);
                                    }
                                }
                               
                                UpdateRollTorques();
                                UpdatePitchTorques();
                                UpdateYawTorques();

                                break;
                            }


                        case DayaRefNames.AILERON_CPTN:
                            {
                                item.valueAdjusted = lastRollMoved;

                                if (isRollCMD == true)
                                {
                                    int value = Convert.ToInt32(dataRef.value);
                                    double diff1 = value - lastRollMoved;
                                    double diff2 = lastRollMoved - value;

                                    item.valueAdjusted = diff1;

                                    // Disconnect auto pilot
                                    if ((diff1 > apDisconnetRollThreshold || diff2 > apDisconnetRollThreshold) && lastRollMoved != -1)
                                    {
                                        item.valueAdjusted = diff1 * 1000;
                                        // Disconnect
                                        errorh.DisplayInfo("A/P overriden by roll. Disconnecting: | " + diff1 + " | " + diff2 + " | value: " + value + " | Previous value: " + lastRollMoved);
                                        DisconnectAPWithTimer();                                  
                                    }
                                    lastRollMoved = value;
                                }

                                break;
                            }

                        case DayaRefNames.ELEVATOR_CPTN:
                            {
                                if (isPitchCMD == true)
                                {
                                    int value = Convert.ToInt32(dataRef.value);
                                    double diff1 = value - lastPitchMoved;
                                    double diff2 = lastPitchMoved - value;
                                    item.valueAdjusted = diff1;

                                    // Disconnect auto pilot
                                    if ((diff1 > apDisconnetPitchThreshold || diff2 > apDisconnetPitchThreshold) && lastPitchMoved != -1)
                                    {
                                        item.valueAdjusted = diff1 * 1000;
                                        // Disconnect
                                        errorh.DisplayInfo("A/P overriden by pitch. Disconnecting: | " + diff1 + " | " + diff2 + " | Value: " + value + " | Previous value: " + lastPitchMoved);
                                        DisconnectAPWithTimer();
                                    }


                                    lastPitchMoved = value;
                                    sendDataY = false;

                                }
                               
                                break;
                            }

                    }


                }
                catch (Exception ex)
                {
                    Debug.WriteLine("failed to update sim var " + ex.Message);
                    errorh.DisplayError("failed to update sim var " + ex.Message);
                }

                // Signal the DataRefTable to update the row, so the new value is displayed
                 Invoke(new MethodInvoker(delegate ()
                {
                    if (!IsDisposed)
                        dataRefTableItemBindingSource.ResetItem(item.index);
                }));
            }
        }

        private void UpdatePitchTorques()
        {
            int torqueBase = isHydAvail ? torquePitchLow : torquePitchHigh;
            int additionalTorque = torqueBase + additionalAirSpeedTorque;
           

            int newAdditionalPitchTorque = additionalTorque + additionalElevatorTorque;
                int tqcw = newAdditionalPitchTorque;
                int tqccw = newAdditionalPitchTorque;
            
            torquePitch.SetTorques(GetMaxMinPitchTorque(tqcw), GetMaxMinPitchTorque(tqccw));
        }

        // Don't use more than the max or min torques
        private int GetMaxMinPitchTorque(int torque)
        {

            if (torque > torquePitchMax)
            {
                return torquePitchMax;
            }

            if (torque < torquePitchLow)
            {
                return torquePitchLow;
            }

            return torque;
        } 
        private void UpdateRollTorques()
        {
            int torqueBase = !isHydAvail || isRollCMD ? torqueRollHigh : torqueRollLow;
            torqueRoll.SetTorque(torqueBase);
        }

        private void UpdateYawTorques()
        {
            int torqueBase = isHydAvail ? torqueYawLow : torqueYawHigh;
            torqueYaw.SetTorque(torqueBase);
        }

        // Roll
        private void moveToX(double value)
        {
           axisRoll.MoveTo(value);
        }

        // Pitch
        private void moveToY(double value)
        {
            axisPitch.MoveTo(value);
        }


        // Yaw
        private void moveToZ(double value)
        {
           axisYaw.MoveTo(value);
        }

        // Pitch Speed
        private void changeSpeedPitch(double value)
        {
            string arduLine = "<PITCH_SPEED, 0, " + value + ">";
            port.Write(arduLine);
            Debug.WriteLine("updated pitch speed " + value);
        }

        private void UpdateTorquePitchLabelBack(int value)
        {
            Invoke(new MethodInvoker(delegate () { lblTorquePitchBack.Text = value.ToString(); }));
        }

        private void UpdateTorquePitchLabelFwd(int value)
        {
            Invoke(new MethodInvoker(delegate () { lblTorquePitchFwd.Text = value.ToString(); }));

        }

        // Disconnect the AP when the wheel/Column is moved.
        // Wait two seconds before this check can happen again to 
        // avoid inmediate second disconnection
        private void DisconnectAPWithTimer()
        {
            if (sendDataAPDisconnect)
            {
                errorh.DisplayInfo("A/P manually disconnected");
                // Wait 2 seconds before this can be checked again
                sendDataAPDisconnect = false;
                DataRef apdisg = new DataRef(DayaRefNames.MCP_AP_DISENGAGE, connection);
                apdisg.value = 1;
            }
        }

        private void btnCenterOut_Click(object sender, EventArgs e)
        {
            moveToX(0);
            errorh.DisplayInfo("Moved Roll to 0");
            moveToY(0);
            errorh.DisplayInfo("Moved Pitch to 0");
            moveToZ(0);
            errorh.DisplayInfo("Moved Yaw to 0");
            txtbxRollPosition.Text = "0";
            txtbxPitchPosition.Text = "0";
            txtbxYawPosition.Text = "0";

        }

        private void btnGoTo_Click(object sender, EventArgs e)
        {
            moveToX(Convert.ToDouble(txtbxRollPosition.Text));
            moveToY(Convert.ToDouble(txtbxPitchPosition.Text));
            moveToZ(Convert.ToDouble(txtbxYawPosition.Text));

        }

        private void chkAutoConnect_CheckedChanged(object sender, EventArgs e)
        {
            //Save Setting
            Properties.Settings.Default.AutoConnect = chkAutoConnect.Checked;
            Properties.Settings.Default.Save();

        }

        private void btnUpdateTorque_Click(object sender, EventArgs e)
        {
            if (txbRollTorque.Text != "")
            {
                torqueRoll.SetTorque(Int32.Parse(txbRollTorque.Text));
            }

            if (txbPitchTorque.Text != "")
            {
                torquePitch.SetTorque(Int32.Parse(txbPitchTorque.Text));
            }

            if (txbYawTorque.Text != "")
            {
                torqueYaw.SetTorque(Int32.Parse(txbYawTorque.Text));
            }
        }

        private void btnTorqueDefault_Click(object sender, EventArgs e)
        {
            torqueRoll.SetTorque(torqueRollLow);
            torquePitch.SetTorque(torquePitchLow);
            torqueYaw.SetTorque(torqueYawLow);
        }

        private void btnSpeedTest_Click(object sender, EventArgs e)
        {
            if (txbPitchSpeedTest.Text != "")
            {
                speedPitch.SetSpeed(Int32.Parse(txbPitchSpeedTest.Text));
                Properties.Settings.Default.CenteringSpeedPitch = Int32.Parse(txbPitchSpeedTest.Text);
            }

            if (txbRollSpeedTest.Text != "")
            {
                speedRoll.SetSpeed(Int32.Parse(txbRollSpeedTest.Text));
                Properties.Settings.Default.CenteringSpeedRoll = Int32.Parse(txbRollSpeedTest.Text);
            }

            if (txbYawSpeedTest.Text != "")
            {
                speedYaw.SetSpeed(Int32.Parse(txbYawSpeedTest.Text));
                Properties.Settings.Default.CenteringSpeedYaw = Int32.Parse(txbYawSpeedTest.Text);
            }
            Properties.Settings.Default.Save();
        }

        private void btnRecenterSpeedRead_Click(object sender, EventArgs e)
        {
            txbPitchSpeedTest.Text = speedPitch.GetSpeed().ToString();
            txbRollSpeedTest.Text = speedRoll.GetSpeed().ToString();
            txbYawSpeedTest.Text = speedYaw.GetSpeed().ToString();
        }


        private void btnBounceSet_Click(object sender, EventArgs e)
        {
            if (txbBouncePitch.Text != "")
            {
                speedPitch.SetBounceGain(Int32.Parse(txbBouncePitch.Text));
                Properties.Settings.Default.DampeningPitch = Int32.Parse(txbBouncePitch.Text);
            }

            if (txbBounceRoll.Text != "")
            {
                speedRoll.SetBounceGain(Int32.Parse(txbBounceRoll.Text));
                Properties.Settings.Default.DampeningRoll = Int32.Parse(txbBounceRoll.Text);
            }

            if (txbBounceYaw.Text != "")
            {
                speedYaw.SetBounceGain(Int32.Parse(txbBounceYaw.Text));
                Properties.Settings.Default.DampeningYaw = Int32.Parse(txbBounceYaw.Text);
            }

            Properties.Settings.Default.Save();
        }

        private void btnBounceGet_Click(object sender, EventArgs e)
        {

            txbBouncePitch.Text = speedPitch.GetBounceGain().ToString();
            txbBounceRoll.Text = speedRoll.GetBounceGain().ToString();
            txbBounceYaw.Text = speedYaw.GetBounceGain().ToString();

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBoxStatus.Checked)
            {
                dataRefView.Show();

            } else
            {
                dataRefView.Hide();
            }
            
        }

    
        private void propertyGridSettings_PropertyValueChanged_1(object s, PropertyValueChangedEventArgs e)
        {
            Properties.Settings.Default.Save();
            // Reload settigs
            SetAppSettings();

        }

        private void ShowFormError(string message)
        {
            Invoke(new MethodInvoker(delegate () { rtxtLog.Text = message + rtxtLog.Text; }));
        }

    
        // Sef center flgiht controls bases on prosim control position
        private void btnCenterControls_Click(object sender, EventArgs e)
        {
            torquePitch.SetTorque(torquePitchHigh);
            torqueRoll.SetTorque(torqueRollHigh);
            torqueYaw.SetTorque(torqueYawHigh);

            axisRoll.CenterAxis(DayaRefNames.AILERON_CPTN, Properties.Settings.Default.CenteredPositionRoll);
            axisPitch.CenterAxis(DayaRefNames.ELEVATOR_CPTN, Properties.Settings.Default.CenteredPositionPitch);
            axisYaw.CenterAxis(DayaRefNames.RUDDER_CAPT, Properties.Settings.Default.CenteredPositionYaw);

            UpdateRollTorques();
            UpdateYawTorques();
            UpdatePitchTorques();

        }
    }

    // The data object that is used for the DataRef table
    class DataRefTableItem
    {
        public int index;
        public DataRef dataRef { get; set; }
        public String Name { get { return dataRef.name; } }
        public String Description { get; set; }
        public String DataType { get; set; }
        public double Value { get; set; }
        public double valueAdjusted { get; set; }
    }


    public static class DayaRefNames
    {
        public const string AILERON_LEFT = "aircraft.flightControls.leftAileron";
        public const string AILERON_RIGHT = "aircraft.flightControls.rightAileron";
        public const string ELEVATOR = "aircraft.flightControls.elevator";
        public const string TRIM_ELEVATOR = "aircraft.flightControls.trim.elevator";
        public const string TRIM_AILERON = "aircraft.flightControls.trim.aileron.units";
        public const string TRIM_RUDDER = "aircraft.flightControls.trim.rudder.units";

        public const string AILERON_CPTN = "system.analog.A_FC_AILERON_CAPT";
        public const string ELEVATOR_CPTN = "system.analog.A_FC_ELEVATOR_CAPT";
        public const string RUDDER_CAPT = "system.analog.A_FC_RUDDER_CAPT";

        public const string PITCH_CMD = "system.gates.B_PITCH_CMD";
        public const string ROLL_CMD = "system.gates.B_ROLL_CMD";

        public const string HYD_PRESS = "aircraft.hidraulics.sysA.pressure";
        public const string HYDRAULICS_AVAILABLE = "system.gates.B_HYDRAULICS_AVAILABLE";

        public const string THRUST_1 = "aircraft.engines.1.thrust";
        public const string THRUST_2 = "aircraft.engines.2.thrust";

        public const string SPEED_IAS = "aircraft.speed.ias";

        public const string PITCH = "aircraft.pitch";

        public const string MCP_AP_DISENGAGE = "system.switches.S_MCP_AP_DISENGAGE";



    }

}
