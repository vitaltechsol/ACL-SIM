using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        static int additionalThrustTorqueFwd = 0;
        static int additionalThrustTorqueBack = 0;
        static int additionalAirSpeedTorqueFwd = 0;
        static int additionalAirSpeedTorqueBack = 0;


        int fwdThrustTorqueFactor = 800;
        int airSpeedTorqueFactor = 20;

        static int torquePitchLow = 30;
        static int torquePitchHigh = 55;

        static SerialPort port;
        int baud = 115200;
        bool isRollCMD = false;
        bool isPitchCMD = false;
        bool isHydAvail = false;

        int offsetX = 7000;
        int offsetY = 500;
        int hydOffPitchPosition = -9500;
        int maxX = 4000;
        int maxY = 8000;
        int minX = -4000;
        int minY = -12000;
        string mbusPort = "COM4";
        Timer timerX;
        static bool sendDataX = false;
        Timer timerY;
        static bool sendDataY = false;
        TorqueControl torquePitch = new TorqueControl("COM4", 1);
        TorqueControl torqueRoll = new TorqueControl("COM4", 2);

        SpeedControl speedPitch = new SpeedControl("COM4", 1);
        SpeedControl speedRoll = new SpeedControl("COM4", 2);


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

            //if (SerialPort.GetPortNames().Count() >= 0)
            //{
            //    foreach (string p in SerialPort.GetPortNames())
            //    {
            //        Debug.WriteLine(p);
            //    }
            //}

            BeginSerial(baud, portName);
            port.Open();
        }

        private void Form1_Shown(Object sender, EventArgs e)
        {
            SetAppSettings();
        }

        private void SetAppSettings()
        {
            hostnameInput.Text = Properties.Settings.Default.ProSimIP;
            chkAutoConnect.Checked = Properties.Settings.Default.AutoConnect;

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

            this.add_data_ref(DayaRefNames.PITCH);
            this.add_data_ref(DayaRefNames.TRIM_ELEVATOR);


            this.add_data_ref(DayaRefNames.ROLL_CMD);
            this.add_data_ref(DayaRefNames.PITCH_CMD);

            this.add_data_ref(DayaRefNames.THRUST_1);
            // this.add_data_ref(DayaRefNames.THRUST_2);
            this.add_data_ref(DayaRefNames.SPEED_IAS);

            this.add_data_ref(DayaRefNames.HYDRAULICS_AVAILABLE);
            this.add_data_ref(DayaRefNames.HYD_PRESS);

            this.add_data_ref(DayaRefNames.AILERON_IN_CPTN);
            this.add_data_ref(DayaRefNames.ELEVATOR_IN_CPTN);

            this.add_data_ref(DayaRefNames.MCP_AP_DISENGAGE);

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
                    item.Value = dataRef.value.ToString();
                    switch (name)
                    {

                        case DayaRefNames.AILERON_LEFT:
                            {
                                if (isRollCMD == true && sendDataX == true)
                                {
                                    double xValue = Math.Round(Convert.ToDouble(dataRef.value) * offsetX);
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
                              //  if ( sendDataY== true)
                             //   {
                                    double yValue = Math.Round(Convert.ToDouble(dataRef.value) * 150000);
                                    // Skip sudden jumps to 0
                                    if (yValue != 0)
                                    {
                                        item.ValueConverted = yValue;
                                        moveToY(yValue);
                                    //   sendDataY = false;
                                    }

                                }
                                break;

                          //  }             

                        case DayaRefNames.THRUST_1:
                            {
                                item.ValueConverted = Math.Round(Convert.ToDouble(dataRef.value) / fwdThrustTorqueFactor);
                                if (additionalThrustTorqueFwd != item.ValueConverted)
                                {
                                    if (IsPitchUp())
                                    {
                                        additionalThrustTorqueFwd = Convert.ToInt32(item.ValueConverted);
                                        additionalThrustTorqueBack = Convert.ToInt32(item.ValueConverted) * 1;
                                    }
                                    else
                                    {
                                        additionalThrustTorqueFwd = Convert.ToInt32(item.ValueConverted) * 1;
                                        additionalThrustTorqueBack = Convert.ToInt32(item.ValueConverted);
                                    }
                                    UpdatePitchTorques();
                                }

                                break;

                            }
                        case DayaRefNames.SPEED_IAS:
                            {
                                //if (sendDataY == true)

                                item.ValueConverted = Math.Round(Convert.ToDouble(dataRef.value) / airSpeedTorqueFactor);
                                if (additionalAirSpeedTorqueFwd != item.ValueConverted)
                                {
                                    if (IsPitchUp())
                                    {
                                        additionalAirSpeedTorqueFwd = Convert.ToInt32(item.ValueConverted);
                                        additionalAirSpeedTorqueBack = Convert.ToInt32(item.ValueConverted) * -1;
                                    }
                                    else
                                    {
                                        additionalAirSpeedTorqueFwd = Convert.ToInt32(item.ValueConverted) * -1;
                                        additionalAirSpeedTorqueBack = Convert.ToInt32(item.ValueConverted);
                                    }

                                    UpdatePitchTorques();

                                }
                                // sendDataY = false;
                                // }
                                break;

                            }
                        case DayaRefNames.PITCH:
                            {
                                if (isPitchCMD == true && sendDataY == true)
                                {
                                    item.ValueConverted = Math.Round(Convert.ToDouble(dataRef.value) * offsetY);
                                    moveToY(item.ValueConverted);
                                    sendDataY = false;
                                }
                                break;

                            }

                        case DayaRefNames.ROLL_CMD:
                            {
                                if (isRollCMD == true)
                                {
                                    // Reset Position
                                    moveToX(0);
                                    if (isHydAvail)
                                    {
                                        torqueRoll.SetTorque(torqueRollLow);
                                    }
                                }
                                else
                                {
                                    torqueRoll.SetTorque(torqueRollHigh);
                                }
                                isRollCMD = Convert.ToBoolean(dataRef.value);
                                Debug.WriteLine("updated isPitchCMD " + isRollCMD);
                                break;
                            }

                        case DayaRefNames.PITCH_CMD:
                            {
                                if (isPitchCMD == true)
                                {
                                    // Reset Position
                                    moveToY(0);
                                    if (isHydAvail)
                                    {
                                        torquePitch.SetTorque(torquePitchLow);
                                    }
                                }
                                else
                                {
                                    torquePitch.SetTorque(torquePitchHigh);
                                }

                                isPitchCMD = Convert.ToBoolean(dataRef.value);
                                Debug.WriteLine("updated isPitchCMD " + isPitchCMD);
                                break;
                            }

                        case DayaRefNames.HYDRAULICS_AVAILABLE:
                            {
                                isHydAvail = Convert.ToBoolean(dataRef.value);
                                DataRefTableItem airSpeed = dataRefs[DayaRefNames.SPEED_IAS];

                                Debug.WriteLine("updated isHydAvail " + isHydAvail);
                                if (!isHydAvail)
                                {
                                    // When hydraulics are off move to max pitch
                                    torqueRoll.SetTorque(torqueRollHigh);
                                    torquePitch.SetTorque(torquePitchHigh);

                                    // move if on ground
                                    double airSpeedValue = Convert.ToDouble(airSpeed.Value);
                                    if (airSpeedValue < 30)
                                    {
                                        moveToY(hydOffPitchPosition);
                                    }
                                }
                                else
                                {
                                    // reset position
                                    changeSpeedPitch(80000);
                                    // move if on ground
                                    if (Convert.ToDouble(airSpeed.Value) < 30)
                                    {
                                        moveToY(0);
                                    }
                                    torqueRoll.SetTorque(torqueRollLow);
                                    torquePitch.SetTorque(torquePitchLow);

                                }

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
                                        DataRef apdisg = new DataRef(DayaRefNames.MCP_AP_DISENGAGE, connection);
                                        apdisg.value = 1;
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
                                        DataRef apdisg = new DataRef(DayaRefNames.MCP_AP_DISENGAGE, connection);
                                        apdisg.value = 1;
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

        private bool IsPitchUp()
        {
            DataRefTableItem itemPitch = dataRefs[DayaRefNames.PITCH];
            double pitchValue = Convert.ToDouble(itemPitch.Value);
            if (pitchValue > 0)
            {
                return true;
            }

            return false;
        }

        private void UpdatePitchTorques()
        {
            int tqccw = torquePitchLow + additionalThrustTorqueFwd + additionalAirSpeedTorqueFwd;
            int tqcw = torquePitchLow + additionalThrustTorqueBack + additionalAirSpeedTorqueBack;
            torquePitch.SetTorques(tqcw, tqccw);

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
    }

    // The data object that is used for the DataRef table
    class DataRefTableItem
    {
        public int index;
        public DataRef dataRef { get; set; }
        public String Name { get { return dataRef.name; } }
        public String Description { get; set; }
        public String DataType { get; set; }
        public String Value { get; set; }
        public double ValueConverted { get; set; }


    }


    public static class DayaRefNames
    {
        public const string AILERON_LEFT = "aircraft.flightControls.leftAileron";
        public const string AILERON_RIGHT = "aircraft.flightControls.rightAileron";
        public const string TRIM_ELEVATOR = "aircraft.flightControls.trim.elevator";

        public const string AILERON_IN_CPTN = "system.analog.A_FC_AILERON_CAPT";
        public const string ELEVATOR_IN_CPTN = "system.analog.A_FC_ELEVATOR_CAPT";

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
