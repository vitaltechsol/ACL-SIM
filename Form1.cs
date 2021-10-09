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
using Timer = System.Timers.Timer;

namespace LoadForceSim
{

    public partial class Form1 : Form
    {
        // Our main ProSim connection
        // Y is pitch
        // X is roll
        ProSimConnect connection = new ProSimConnect();
        Dictionary<String, DataRefTableItem> dataRefs = new Dictionary<string, DataRefTableItem>();
        static string portName = "COM3";
        static int torqueRollLow = 18;
        static int torqueRollHigh = 65;
        static int additionalThrustTorque1 = 0;
        static int additionalThrustTorque2 = 0;
        static int additionalAirSpeedTorque = 0;
        static int additionalVerticalSpeedTorque = 1;
      
        int torqueFactorThrust = 2000;
        int torqueFactorAirSpeed = 10;
        int torqueFactorVerticalSpeed = 200;
        int trimFactorElevator = 1200;
        int trimFactorAileron = 1000;
        int torquePitchLow = 25;
        int torquePitchHigh = 55;
        int torquePitchMax = 70;
        int torquePitchMin = 20;

        static SerialPort port;
        int baud = 115200;
        bool isRollCMD = false;
        bool isPitchCMD = false;
        bool isHydAvail = false;

        int offsetX = 7000;
        int offsetY = 300;
        int hydOffPitchPosition = -9500;
        int maxX = 4000;
        int maxY = 8000;
        int minX = -4000;
        int minY = -12000;
        bool sendDataAPDisconnect = true;
        string mbusPort = "COM4";
        Timer timerX;
        static bool sendDataX = false;
        Timer timerY;
        Timer timerAPdiconnect;
        static bool sendDataY = false;
        TorqueControl torquePitch = new TorqueControl("COM4", 1);
        TorqueControl torqueRoll = new TorqueControl("COM4", 2);

        CustomControl speedPitch = new CustomControl("COM4", 1);
        CustomControl speedRoll = new CustomControl("COM4", 2);

        int lastRollMoved = -1;
        int lastPitchMoved = -1;
        int apDisconnetRollThreshold;
        int apDisconnetPitchThreshold;


        public Form1()
        {
            InitializeComponent();

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
            port.Open();

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
        }

        private void SetAppSettings()
        {
            hostnameInput.Text = Properties.Settings.Default.ProSimIP;
            chkAutoConnect.Checked = Properties.Settings.Default.AutoConnect;

            torqueFactorThrust = Properties.Settings.Default.TorqueFactor_Thrust;
            torqueFactorAirSpeed = Properties.Settings.Default.TorqueFactor_AirSpeed;
            torqueFactorVerticalSpeed = Properties.Settings.Default.TorqueFactor_VerticalSpeed;

            torquePitchLow = Properties.Settings.Default.Torque_Pitch_Low;
            torquePitchHigh = Properties.Settings.Default.Torque_Pitch_High;
            torquePitchMax = Properties.Settings.Default.Torque_Pitch_Max;
            torquePitchMin = Properties.Settings.Default.Torque_Pitch_Min;

            torqueRollLow = Properties.Settings.Default.Torque_Roll_Low;
            torqueRollHigh = Properties.Settings.Default.Torque_Roll_High;

            trimFactorElevator = Properties.Settings.Default.TrimFactor_Elevator;
            trimFactorAileron = Properties.Settings.Default.TrimFactor_Aileron;


            if (Properties.Settings.Default.AutoConnect)
            {
                connectToProSim();
            }

            apDisconnetRollThreshold = Properties.Settings.Default.APDisconnetRollThreshold;
            apDisconnetPitchThreshold = Properties.Settings.Default.APDisconnetPitchThreshold;

        }

  
        private void sendDataOK_X(object sender, System.Timers.ElapsedEventArgs e)
        {
            sendDataX = true;
        }

        private void sendDataOK_Y(object sender, System.Timers.ElapsedEventArgs e)
        {
            sendDataY = true;
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
                // MessageBox.Show("Error connecting to ProSim737 System: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            this.add_data_ref(DayaRefNames.TRIM_ELEVATOR);
            this.add_data_ref(DayaRefNames.TRIM_AILERON);
           // this.add_data_ref(DayaRefNames.PITCH);

            this.add_data_ref(DayaRefNames.ROLL_CMD);
            this.add_data_ref(DayaRefNames.PITCH_CMD);
            this.add_data_ref(DayaRefNames.MCP_AP_DISENGAGE);

            this.add_data_ref(DayaRefNames.THRUST_1);
            this.add_data_ref(DayaRefNames.THRUST_2);
            this.add_data_ref(DayaRefNames.SPEED_IAS);
            this.add_data_ref(DayaRefNames.VERTICAL_SPEED);

            this.add_data_ref(DayaRefNames.HYDRAULICS_AVAILABLE);
            this.add_data_ref(DayaRefNames.HYD_PRESS);

            this.add_data_ref(DayaRefNames.AILERON_IN_CPTN);
            this.add_data_ref(DayaRefNames.ELEVATOR_IN_CPTN);
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
                                    double xValue = Math.Round(item.Value * offsetX);
                                    // Skip sudden jumps to 0
                                    if (xValue != 0)
                                    {
                                        item.ValueConverted = xValue;
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
                                        double yValue = Math.Round((item.Value * trimFactorElevator) * 100);
                                        // Skip sudden jumps to 0
                                        if (yValue != 0)
                                        {
                                            item.ValueConverted = yValue;
                                            moveToY(yValue);
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
                                    double xValue = Math.Round(item.Value * trimFactorAileron);
                                    // Skip sudden jumps to 0
                                    if (xValue != 0)
                                    {
                                        item.ValueConverted = xValue * -1;
                                        moveToX(xValue * -1);
                                        sendDataY = false;
                                    }
                                    
                                }

                                break;
                            }

                        case DayaRefNames.THRUST_1:
                            {
                                item.ValueConverted = Math.Round(item.Value / torqueFactorThrust);
                                if (additionalThrustTorque1 != item.ValueConverted)
                                {
                                    additionalThrustTorque1 = Convert.ToInt32(item.ValueConverted);
                                    UpdatePitchTorques();
                                }

                                break;

                            }

                        case DayaRefNames.THRUST_2:
                            {
                                item.ValueConverted = Math.Round(item.Value / torqueFactorThrust);
                                if (additionalThrustTorque2 != item.ValueConverted)
                                {
                                    additionalThrustTorque2 = Convert.ToInt32(item.ValueConverted);
                                    UpdatePitchTorques();
                                }

                                break;

                            }

                        case DayaRefNames.SPEED_IAS:
                            {
                                item.ValueConverted = Math.Round(item.Value / torqueFactorAirSpeed);
                                if (additionalAirSpeedTorque != item.ValueConverted)
                                {
                                    additionalAirSpeedTorque = Convert.ToInt32(item.ValueConverted);
                                    UpdatePitchTorques();

                                }
                                break;

                            }
                        case DayaRefNames.VERTICAL_SPEED:
                            {
                                double value = Math.Round(item.Value / torqueFactorVerticalSpeed);
                                // Vertical speed tells us if more torque should be added when pushing or pulling
                                if (additionalVerticalSpeedTorque != value)
                                {
                                    int valueConverted = Convert.ToInt32(value);
                                    // Can't be less than the minimun or we'll get negative values
                                    if (valueConverted > torquePitchMin)
                                    {
                                        valueConverted = torquePitchMin;
                                    }

                                    if (valueConverted < torquePitchMin * -1)
                                    {
                                        valueConverted = torquePitchMin * -1;
                                    }

                                    additionalVerticalSpeedTorque = valueConverted;
                                    item.ValueConverted = valueConverted;

                                    UpdatePitchTorques();
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
                                    if (!isHydAvail)
                                    {
                                        torquePitch.SetTorque(torquePitchHigh);
                                        moveToY(hydOffPitchPosition);
                                    }
                                    else
                                    {
                                        // reset position
                                        torquePitch.SetTorque(torquePitchHigh);
                                        changeSpeedPitch(80000);
                                        moveToY(0);
                                    }
                                }
                               
                                UpdateRollTorques();
                                UpdatePitchTorques();

                                break;
                            }


                        case DayaRefNames.AILERON_IN_CPTN:
                            {
                                item.ValueConverted = lastRollMoved;

                                if (isRollCMD == true)
                                {
                                    int value = Convert.ToInt32(dataRef.value);
                                    double diff1 = value - lastRollMoved;
                                    double diff2 = lastRollMoved - value;

                                    item.ValueConverted = diff1;

                                    // Disconnect auto pilot
                                    if ((diff1 > apDisconnetRollThreshold || diff2 > apDisconnetRollThreshold) && lastRollMoved != -1)
                                    {
                                        item.ValueConverted = diff1 * 1000;
                                        // Disconnect
                                        DisconnectAPWithTimer();                                  
                                    }
                                    lastRollMoved = value;
                                }

                                break;
                            }

                        case DayaRefNames.ELEVATOR_IN_CPTN:
                            {
                                if (isPitchCMD == true)
                                {
                                    int value = Convert.ToInt32(dataRef.value);
                                    double diff1 = value - lastPitchMoved;
                                    double diff2 = lastPitchMoved - value;
                                    item.ValueConverted = diff1;

                                    // Disconnect auto pilot
                                    if ((diff1 > apDisconnetPitchThreshold || diff2 > apDisconnetPitchThreshold) && lastPitchMoved != -1)
                                    {
                                        item.ValueConverted = diff1 * 1000;
                                        // Disconnect
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
            int vsFactor = Convert.ToInt32(torquePitchMax / 1.3);
            int torqueBase = isHydAvail ? torquePitchLow : torquePitchHigh;
            int additionalTorque = additionalThrustTorque1 + additionalThrustTorque2 + additionalAirSpeedTorque;
            if (additionalVerticalSpeedTorque == 0)
            {
                additionalVerticalSpeedTorque = 1;
            }

            int vsTorqueWithFactor = (vsFactor / additionalVerticalSpeedTorque) + 1;
            // Some additional torque from the air speed
            int speedTorque = Convert.ToInt32(additionalAirSpeedTorque / 1.5);
            if (vsTorqueWithFactor == 0)
            {
                vsTorqueWithFactor = 1;
            }

            if (additionalVerticalSpeedTorque > 0)
            {
                int calcAdditionalTorque = additionalTorque / vsTorqueWithFactor;
                int tqccw = torqueBase + speedTorque + calcAdditionalTorque;
                int tqcw = torqueBase + speedTorque - calcAdditionalTorque;


                torquePitch.SetTorques(GetMaxMinPitchTorque(tqcw), GetMaxMinPitchTorque(tqccw));
            }
            else
            {
                int calcAdditionalTorque = (additionalTorque * -1) / vsTorqueWithFactor;

                int tqccw = torqueBase + speedTorque - calcAdditionalTorque;
                int tqcw = torqueBase + speedTorque + calcAdditionalTorque;

                torquePitch.SetTorques(GetMaxMinPitchTorque(tqcw), GetMaxMinPitchTorque(tqccw));
            }

        }

        // Don't use more than the max or min torques
        private int GetMaxMinPitchTorque(int torque)
        {

            if (torque > torquePitchMax)
            {
                return torquePitchMax;
            }

            if (torque < torquePitchMin)
            {
                return torquePitchMin;
            }

            return torque;
        } 
        private void UpdateRollTorques()
        {
            int torqueBase = isHydAvail ? torqueRollLow : torqueRollHigh;
            torqueRoll.SetTorque(torqueBase);
        }

        // Roll
        private void moveToX(double value)
        {
            if (value > minX && value < maxX)
            {
                string arduLine = "<X_POS, 0, " + value + ">";
                port.Write(arduLine);
            }
        }

        // Pitch
        private void moveToY(double value)
        {
            if (value > minY && value < maxY)
            {
                string arduLine = "<Y_POS, 0, " + value + ">";
                port.Write(arduLine);
            }

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
                Debug.WriteLine("Manual AP override disconnect " + isRollCMD);
                // Wait 2 seconds before this can be checked again
                sendDataAPDisconnect = false;
                DataRef apdisg = new DataRef(DayaRefNames.MCP_AP_DISENGAGE, connection);
                apdisg.value = 1;
            }
        }

        private void btnCenterOut_Click(object sender, EventArgs e)
        {
            moveToX(0);
            moveToY(0);
            txtbxRoll.Text = "0";
            txtbxPitch.Text = "0";
        }

        private void btnGoTo_Click(object sender, EventArgs e)
        {
            moveToX(Convert.ToDouble(txtbxRoll.Text));
            moveToY(Convert.ToDouble(txtbxPitch.Text));
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
        }

        private void btnTorqueDefault_Click(object sender, EventArgs e)
        {
            torqueRoll.SetTorque(torqueRollLow);
            torquePitch.SetTorque(torquePitchLow);
        }

        private void btnSpeedTest_Click(object sender, EventArgs e)
        {
            if (txbPitchSpeedTest.Text != "")
            {
                speedPitch.SetSpeed(Int32.Parse(txbPitchSpeedTest.Text));
            }

            if (txbRollSpeedTest.Text != "")
            {
                speedRoll.SetSpeed(Int32.Parse(txbRollSpeedTest.Text));
            }
        }

        private void btnRecenterSpeedRead_Click(object sender, EventArgs e)
        {
            txbPitchSpeedTest.Text = speedPitch.GetSpeed().ToString();
            txbRollSpeedTest.Text = speedRoll.GetSpeed().ToString();

        }


        private void btnBounceSet_Click(object sender, EventArgs e)
        {
            if (txbBouncePitch.Text != "")
            {
                speedPitch.SetBounceGain(Int32.Parse(txbBouncePitch.Text));
            }

            if (txbBounceRoll.Text != "")
            {
                speedRoll.SetBounceGain(Int32.Parse(txbBounceRoll.Text));
            }
        }

        private void btnBounceGet_Click(object sender, EventArgs e)
        {

            txbBouncePitch.Text = speedPitch.GetBounceGain().ToString();
            txbBounceRoll.Text = speedRoll.GetBounceGain().ToString();
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
        public double ValueConverted { get; set; }
    }


    public static class DayaRefNames
    {
        public const string AILERON_LEFT = "aircraft.flightControls.leftAileron";
        public const string AILERON_RIGHT = "aircraft.flightControls.rightAileron";
        public const string TRIM_ELEVATOR = "aircraft.flightControls.trim.elevator";
        public const string TRIM_AILERON = "aircraft.flightControls.trim.aileron.units";

        public const string AILERON_IN_CPTN = "system.analog.A_FC_AILERON_CAPT";
        public const string ELEVATOR_IN_CPTN = "system.analog.A_FC_ELEVATOR_CAPT";

        public const string PITCH_CMD = "system.gates.B_PITCH_CMD";
        public const string ROLL_CMD = "system.gates.B_ROLL_CMD";

        public const string HYD_PRESS = "aircraft.hidraulics.sysA.pressure";
        public const string HYDRAULICS_AVAILABLE = "system.gates.B_HYDRAULICS_AVAILABLE";

        public const string THRUST_1 = "aircraft.engines.1.thrust";
        public const string THRUST_2 = "aircraft.engines.2.thrust";

        public const string SPEED_IAS = "aircraft.speed.ias";
        public const string VERTICAL_SPEED = "aircraft.verticalspeed";

        public const string PITCH = "aircraft.pitch";

        public const string MCP_AP_DISENGAGE = "system.switches.S_MCP_AP_DISENGAGE";



    }

}
