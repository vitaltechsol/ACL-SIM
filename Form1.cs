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
using EasyModbus;
using ProSimSDK;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
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
        ErrorHandler errorh = new ErrorHandler();
        ProSimConnect connection = new ProSimConnect();
        Dictionary<String, DataRefTableItem> dataRefs = new Dictionary<string, DataRefTableItem>();
        static int torqueRollMin = 18;
        static int torqueRollMax = 30;
        static int torqueRollHydOff = 65;
        static int additionalAirSpeedTorque = 0;
        static int tillerTorqueReduction = 0;

        int pitchTorqueFromPos;
        int yawTorqueFromPos;
        int torqueFactorAirSpeed = 10;
        int trimFactorAileron = 1000;
        int trimFactorRudder = 1000;
        int torquePitchMin = 1;
        int torquePitchMax = 70;
        int torquePitchHydOff = 0;
        int torqueYawHydOff = 40;
        int torqueYawMin = 20;
        int torqueYawMax = 0;
        int torqueTillerMin = 0;
        int torqueTillerMax = 20;
        int torqueTillerHydOff = 5;
        int torqueStallingAdditional = 0;
        bool isStalling = false;
        bool autoCenterOnStatart = false;

        int Centering_Speed_Pitch = 0;
        int Centering_Speed_Roll = 0;
        int Centering_Speed_Yaw = 0;
        int Centering_Speed_Tiller = 0;

        int Dampening_Pitch = 0;
        int Dampening_Roll = 0;
        int Dampening_Yaw = 0;
        int Dampening_Tiller = 0;

        int Direction_Axis_Pitch;
        int Direction_Axis_Roll;
        int Direction_Axis_Yaw;
        int Direction_Axis_Tiller;

        static SerialPort port;
        int baud = 115200;
        bool isRollCMD = false;
        bool isPitchCMD = false;
        bool isHydAvail = false;
        bool axisDroppedByWind = false;
        Boolean manualTorqueSet = false;

        int apPositionRollFactor = 700;
        int apPositionPitchFactor = 700;
        bool sendDataAPDisconnect = true;
        Timer timerX;
        Timer timerY;
        Timer timerZ;
        Timer timerAPdiconnect;
        static bool sendDataX = false;
        static bool sendDataY = false;
        static bool sendDataZ = false;

        static ModbusClient mbc;
        static ModbusClient mbcPitch;
        static ModbusClient mbcRoll;
        static ModbusClient mbcYaw;
        static ModbusClient mbcTiller;

        TorqueControl torquePitch;

        TorqueControl torqueRoll;

        TorqueControl torqueYaw;

        TorqueControl torqueTiller;

        CustomControl speedPitch ;

        CustomControl speedRoll;

        CustomControl speedYaw;

        CustomControl speedTiller;

        AxisControl axisPitch;
       
        AxisControl axisRoll;

        AxisControl axisYaw;

        AxisControl axisTiller;

        int lastRollMoved = -1;
        int lastPitchMoved = -1;
        int AP_Disconnet_Roll_Threshold;
        int AP_Disconnet_Pitch_Threshold;


        static Form1()
        {
            // Initialize static mbc with settings
            string port = getRS485Port(); // Make sure this is a static method or accessible
            string mbcPitchSetting = Properties.Settings.Default.RS485_Pitch;
            string mbcRollSetting = Properties.Settings.Default.RS485_Roll;
            string mbcYawSetting = Properties.Settings.Default.RS485_Yaw;
            string mbcTillerSetting = Properties.Settings.Default.RS485_Tiller;


            mbc = new ModbusClient(port)
            {
                Baudrate = 115200,
                StopBits = StopBits.One,
                Parity = Parity.None,
                ConnectionTimeout = 4000
            };
            
            // "192.168.1.1750-
            if (mbcPitchSetting != "") {
               // mbcPitch = new ModbusClient(mbcPitchSetting, 502)
                 mbcPitch = new ModbusClient()
                 {
                    Baudrate = 115200,
                    StopBits = StopBits.One,
                    Parity = Parity.None,
                    ConnectionTimeout = 4000
                };
                if (isIp(mbcPitchSetting))
                {
                    mbcPitch.IPAddress = mbcPitchSetting;
                    mbcPitch.Port = 502;
                    Debug.WriteLine("mbcPitchSetting " + mbcPitchSetting);
                } else
                {
                    mbcPitch.SerialPort = mbcPitchSetting;
                }
            } else
            {
                mbcPitch = mbc;
            }

            if (mbcRollSetting != "")
            {
                mbcRoll = new ModbusClient()
                {
                    Baudrate = 115200,
                    StopBits = StopBits.One,
                    Parity = Parity.None,
                    ConnectionTimeout = 4000
                };
                if (isIp(mbcRollSetting))
                {
                    mbcRoll.IPAddress = mbcRollSetting;
                    mbcRoll.Port = 502;
                    Debug.WriteLine("mbcRollSetting " + mbcRollSetting);
                } else
                {
                    mbcRoll.SerialPort = mbcRollSetting;
                }
            }
            else
            {
                mbcRoll = mbc;
            }

            if (mbcYawSetting != "")
            {
                mbcYaw = new ModbusClient()
                {
                    Baudrate = 115200,
                    StopBits = StopBits.One,
                    Parity = Parity.None,
                    ConnectionTimeout = 4000
                };
                if (isIp(mbcYawSetting))
                {
                    mbcYaw.IPAddress = mbcYawSetting;
                    mbcYaw.Port = 502;
                    Debug.WriteLine("mbcYawSetting " + mbcYawSetting);
                }
                else
                {
                    mbcYaw.SerialPort = mbcYawSetting;
                }
            }
            else
            {
                mbcYaw = mbc;
            }

            //mbcRoll= new ModbusClient(mbcPitchSetting, 502)
            //{
            //    Baudrate = 115200,
            //    StopBits = StopBits.One,
            //    Parity = Parity.None,
            //    ConnectionTimeout = 4000
            //};

        }

        public Form1()
        {
            //this.Shown += Form1_Shown;
            InitializeComponent();

             torquePitch = new TorqueControl(mbcPitch,
                Convert.ToByte(Properties.Settings.Default.Driver_Pitch_ID),
                Properties.Settings.Default.Enable_Pitch_ACL, 0);

             torqueRoll = new TorqueControl(mbcRoll,
                Convert.ToByte(Properties.Settings.Default.Driver_Roll_ID),
                Properties.Settings.Default.Enable_Roll_ACL, 0);

             torqueYaw = new TorqueControl(mbcYaw,
                Convert.ToByte(Properties.Settings.Default.Driver_Yaw_ID),
                Properties.Settings.Default.Enable_Yaw_ACL, 0);

             torqueTiller = new TorqueControl(mbc,
                Convert.ToByte(Properties.Settings.Default.Driver_Tiller_ID),
                Properties.Settings.Default.Enable_Tiller_ACL, Properties.Settings.Default.Torque_Tiller_Diff_Offset);

             speedPitch = new CustomControl(mbcPitch,
                Convert.ToByte(Properties.Settings.Default.Driver_Pitch_ID),
                Properties.Settings.Default.Enable_Pitch_ACL);

             speedRoll = new CustomControl(mbcRoll,
                Convert.ToByte(Properties.Settings.Default.Driver_Roll_ID),
               Properties.Settings.Default.Enable_Roll_ACL);

             speedYaw = new CustomControl(mbcYaw,
                Convert.ToByte(Properties.Settings.Default.Driver_Yaw_ID),
                Properties.Settings.Default.Enable_Yaw_ACL);

             speedTiller = new CustomControl(mbc,
                Convert.ToByte(Properties.Settings.Default.Driver_Tiller_ID),
                Properties.Settings.Default.Enable_Tiller_ACL);


             axisPitch = new AxisControl("Y_POS",
                "Pitch",
                Properties.Settings.Default.Direction_Axis_Pitch,
                Properties.Settings.Default.Position_Pitch_HYD_OFF_Max,
                Properties.Settings.Default.Enable_Pitch_ACL);

             axisRoll = new AxisControl("X_POS",
                "Roll",
                Properties.Settings.Default.Direction_Axis_Roll,
                0,  // no hydraulic positionchange
                Properties.Settings.Default.Enable_Roll_ACL);

             axisYaw = new AxisControl("Z_POS",
                "Yaw",
                Properties.Settings.Default.Direction_Axis_Yaw,
                Properties.Settings.Default.Position_Yaw_HYD_OFF_Max,
                Properties.Settings.Default.Enable_Yaw_ACL);

             axisTiller = new AxisControl("Tiller_POS",
                "Tiller",
                Properties.Settings.Default.Direction_Axis_Tiller,
                0,  // no hydraulic positionchange
                Properties.Settings.Default.Enable_Tiller_ACL);
        }

        private void Form1_Shown(Object sender, EventArgs e)
        {
            errorh.onError += (msg) => ShowFormError(msg);

            try
            {
                if (mbc != null) {
                    if (mbc.Connected)
                    {
                        mbc.Disconnect();
                    }
                    mbc.Connect();
                }

                if (mbcPitch != null) {
                    if (mbcPitch.Connected)
                    {
                        mbcPitch.Disconnect();
                    }
                    mbcPitch.Connect();
                }

                if (mbcRoll != null)
                {
                    if (mbcRoll.Connected)
                    {
                        mbcRoll.Disconnect();
                    }
                    mbcRoll.Connect();
                }

            }
            catch (Exception ex)
            {
                errorh.DisplayError("COM port error " + ex.Message);
            }

            torquePitch.onError += (msg) => ShowFormError(msg);
            torqueRoll.onError += (msg) => ShowFormError(msg);
            torqueYaw.onError += (msg) => ShowFormError(msg);
            torqueTiller.onError += (msg) => ShowFormError(msg);

            axisPitch.onError += (msg) => ShowFormError(msg);
            axisRoll.onError += (msg) => ShowFormError(msg);
            axisYaw.onError += (msg) => ShowFormError(msg);
            axisTiller.onError += (msg) => ShowFormError(msg);

            speedPitch.onError += (msg) => ShowFormError(msg);
            speedRoll.onError += (msg) => ShowFormError(msg);
            speedYaw.onError += (msg) => ShowFormError(msg);
            speedTiller.onError += (msg) => ShowFormError(msg);

            // Add event to update the torque status
            torquePitch.OnUpdateStatusCCW += (sender1, e1) => UpdateTorquePitchLabelBack(torquePitch.StatusTextCCW);
            torquePitch.OnUpdateStatusCW += (sender1, e1) => UpdateTorquePitchLabelFwd(torquePitch.StatusTextCW);

            torqueRoll.OnUpdateStatusCCW += (sender1, e1) => UpdateTorqueRollLabelAft(torqueRoll.StatusTextCCW);
            torqueRoll.OnUpdateStatusCW += (sender1, e1) => UpdateTorqueRollLabelFwd(torqueRoll.StatusTextCW);

            torqueYaw.OnUpdateStatusCW += (sender1, e1) => UpdateTorqueYawLabel(torqueYaw.StatusTextCW);

            // Register to receive connect and disconnect events
            connection.onConnect += connection_onConnect;
            connection.onDisconnect += connection_onDisconnect;
            timerX = new Timer();
            timerX.Interval = 25;
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

            BeginSerial(baud, getArduinoPort());
            try
            {
                port.Open();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Cannot open connection to Arduino COM port. " + getArduinoPort() + ": " + ex.Message);
                port.Close();
            }

            axisRoll.SetPort(port, connection);
            axisYaw.SetPort(port, connection);
            axisPitch.SetPort(port, connection);
            axisTiller.SetPort(port, connection);

            dataRefView.Hide();

            propertyGridSettings.SelectedObject = Properties.Settings.Default;
            propertyGridSettings.BrowsableAttributes = new AttributeCollection(new UserScopedSettingAttribute());

            SetAppSettings(true);
            StartDynamicTorques();
        }

        static bool isIp(string ip)
        {
            if (ip == null)
            {
                return false;
            }
            return ip.Length > 6;
        }

        static string getRS485Port()
        {
            return "COM" + Properties.Settings.Default.RS485COM_Port;
        }

        static string getArduinoPort()
        {
            return "COM" + Properties.Settings.Default.Arduino_COM_Port;
        }

        private void SetAppSettings(bool firstTime)
        {
            hostnameInput.Text = Properties.Settings.Default.ProSimIP;
            chkAutoConnect.Checked = Properties.Settings.Default.AutoConnect;
            autoCenterOnStatart = Properties.Settings.Default.Auto_Center_On_Start;
            chkAutoCenter.Checked = autoCenterOnStatart;

            torqueFactorAirSpeed = Properties.Settings.Default.Torque_Factor_Air_Speed;

            torquePitchMin = Properties.Settings.Default.Torque_Pitch_Min;
            torquePitchHydOff = Properties.Settings.Default.Torque_Pitch_Hyd_Off;
            torquePitchMax = Properties.Settings.Default.Torque_Pitch_Max;

            torqueRollMin = Properties.Settings.Default.Torque_Roll_Min;
            torqueRollMax = Properties.Settings.Default.Torque_Roll_Max;
            torqueRollHydOff = Properties.Settings.Default.Torque_Roll_Hyd_Off;

            torqueYawMin = Properties.Settings.Default.Torque_Yaw_Min;
            torqueYawMax = Properties.Settings.Default.Torque_Yaw_Max;
            torqueYawHydOff = Properties.Settings.Default.Torque_Yaw_Hyd_Off;

            torqueTillerMax = Properties.Settings.Default.Torque_Tiller_Max;
            torqueTillerMin = Properties.Settings.Default.Torque_Tiller_Min;
            torqueTillerHydOff = Properties.Settings.Default.Torque_Tiller_Hyd_Off;

            torqueStallingAdditional = Properties.Settings.Default.Torque_Stalling_Additional;

            trimFactorAileron = Properties.Settings.Default.TrimFactor_Aileron;
            trimFactorRudder = Properties.Settings.Default.TrimFactor_Rudder;

            AP_Disconnet_Roll_Threshold = Properties.Settings.Default.AP_Disconnet_Roll_Threshold;
            AP_Disconnet_Pitch_Threshold = Properties.Settings.Default.AP_Disconnet_Pitch_Threshold;

            apPositionRollFactor = Properties.Settings.Default.AP_Position_Roll_Factor;

            torqueYaw.enabled = Properties.Settings.Default.Enable_Yaw_ACL;
            speedYaw.enabled = Properties.Settings.Default.Enable_Yaw_ACL;

            torquePitch.enabled = Properties.Settings.Default.Enable_Pitch_ACL;
            speedPitch.enabled = Properties.Settings.Default.Enable_Pitch_ACL;

            torqueRoll.enabled = Properties.Settings.Default.Enable_Roll_ACL;
            speedRoll.enabled = Properties.Settings.Default.Enable_Roll_ACL;

            torqueTiller.enabled = Properties.Settings.Default.Enable_Tiller_ACL;
            speedTiller.enabled = Properties.Settings.Default.Enable_Tiller_ACL;

            Centering_Speed_Pitch = Properties.Settings.Default.Centering_Speed_Pitch;
            Centering_Speed_Roll = Properties.Settings.Default.Centering_Speed_Roll;
            Centering_Speed_Yaw = Properties.Settings.Default.Centering_Speed_Yaw;
            Centering_Speed_Tiller = Properties.Settings.Default.Centering_Speed_Tiller;

            Dampening_Pitch = Properties.Settings.Default.Dampening_Pitch;
            Dampening_Roll = Properties.Settings.Default.Dampening_Roll;
            Dampening_Yaw = Properties.Settings.Default.Dampening_Yaw;
            Dampening_Tiller = Properties.Settings.Default.Dampening_Tiller;

            Direction_Axis_Pitch = Properties.Settings.Default.Direction_Axis_Pitch;
            Direction_Axis_Roll = Properties.Settings.Default.Direction_Axis_Roll;
            Direction_Axis_Yaw = Properties.Settings.Default.Direction_Axis_Yaw;
            Direction_Axis_Tiller = Properties.Settings.Default.Direction_Axis_Tiller;


            if (Properties.Settings.Default.AutoConnect && firstTime)
            {
                // Values from settings
                speedPitch.SetSpeed(Centering_Speed_Pitch);
                speedPitch.SetBounceGain(Dampening_Pitch);
                speedRoll.SetSpeed(Centering_Speed_Roll);
                speedRoll.SetBounceGain(Dampening_Roll);
                speedYaw.SetSpeed(Centering_Speed_Yaw);
                speedYaw.SetBounceGain(Dampening_Yaw);
                speedTiller.SetSpeed(Centering_Speed_Tiller);
                speedTiller.SetBounceGain(Dampening_Tiller);

                axisRoll.ChangeAxisSpeed(8000000);
                axisPitch.ChangeAxisSpeed(8000000);

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
        }

        // When we connect to ProSim737 system, update the status label and start filling the table
        void connection_onConnect()
        {
            Invoke(new MethodInvoker(updateStatusLabel));
            Invoke(new MethodInvoker(fillDataRefTable));
            Invoke(new MethodInvoker(shouldRecenterAxis));

        }

        void shouldRecenterAxis()
        {
            if (Properties.Settings.Default.Auto_Center_On_Start && (!axisPitch.AxisCentered || !axisRoll.AxisCentered || !axisYaw.AxisCentered || !axisTiller.AxisCentered))
            {
                errorh.DisplayInfo("Center axis after connection");
                centerAllAxis();
            }
        }

        void updateStatusLabel()
        {
            if (connection.isConnected)
            {
                connectionStatusLabel.Text = "Connected";
                connectionStatusLabel.ForeColor = Color.LimeGreen;
                errorh.DisplayInfo("Connected to Prosim");
            } else
            {
                connectionStatusLabel.Text = "Disconnected";
                connectionStatusLabel.ForeColor = Color.Red;
                errorh.DisplayError("Disconnected from Prosim");
            }
        }

        void fillDataRefTable()
        {
            try
            {
                fillTableWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                errorh.DisplayError(ex.Message);
            }
            
        }


        // Fill the table with DataRefs
        private void fillTableWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get all of the DataRefs from ProSim737 System
            DataRefDescription[] descriptions = connection.getDataRefDescriptions().ToArray();

            DataRef dataRef = new DataRef("", 10, connection);
           
            //this.add_data_ref("aircraft.autoflight.PitchD");
            //this.add_data_ref("aircraft.autoflight.PitchI");
            //this.add_data_ref("aircraft.autoflight.PitchP");
            //this.add_data_ref("aircraft.flightControls.input.pitch");
            //this.add_data_ref("aircraft.flightControls.input.roll");

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
            this.add_data_ref(DayaRefNames.SPEED_GROUND);

            this.add_data_ref(DayaRefNames.HYDRAULICS_AVAILABLE);
            this.add_data_ref(DayaRefNames.HYD_PRESS);

            this.add_data_ref(DayaRefNames.AILERON_CPTN);
            this.add_data_ref(DayaRefNames.ELEVATOR_CPTN);
            this.add_data_ref(DayaRefNames.RUDDER_CAPT);
            this.add_data_ref(DayaRefNames.TILLER_CAPT);

            //this.add_data_ref(DayaRefNames.WIND_SPEED);
            this.add_data_ref(DayaRefNames.IS_STALLING);
        }

        private void add_data_ref(string dataRefName)
        {
            // If we don't yet have this DataRef, add it to the table
            if (!dataRefs.ContainsKey(dataRefName))
            {
                // Request a new DataRef from ProSim737 System with 100 msec update interval
                DataRef dataRef = new DataRef(dataRefName, 20, connection);

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

      

        async void dataRef_onDataChange(DataRef dataRef)
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
                                    double xValue = Math.Round(item.Value * (apPositionRollFactor * 10) * Direction_Axis_Roll * -1);
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

                        case DayaRefNames.TRIM_AILERON:
                            {
                                double rollValue = Math.Round(item.Value * trimFactorAileron) * Direction_Axis_Roll;
                             //   errorh.DisplayInfo("trim aileron " + rollValue);

                                if (rollValue != 0)
                                {
                                    axisRoll.ChangeAxisSpeed(500);
                                  //  UpdateRollTorques(true);
                                    item.valueAdjusted = rollValue;
                                    _ = axisRoll.TrimToPositionAsync(rollValue); // fire-and-forget or await if async
                                 
                                }

                                break;
                               
                            }

                        case DayaRefNames.TRIM_RUDDER:
                            {

                                //if (sendDataZ == true)
                                //{
                                    double yawValue = Math.Round(item.Value * trimFactorRudder);
                                    // Skip sudden jumps to 0
                                    if (yawValue != 0)
                                    {

                                    //axisYaw.ChangeAxisSpeed(100);
                                    //    item.valueAdjusted = yawValue;
                                    //UpdateYawTorques(true);
                                    // _ = axisYaw.TrimToPositionAsync(yawValue); // fire-and-forget or await if async

                                    axisYaw.ChangeAxisSpeed(500);
                                   // axisYaw.isTrimming = true;
                               //     UpdateYawTorques(true);
                                    item.valueAdjusted = yawValue * Direction_Axis_Yaw;
                                    moveToZ(yawValue * Direction_Axis_Yaw);

                                    sendDataZ = false;
                                }

                                //} else
                                //{
                                //    axisYaw.isTrimming = false;
                                //    UpdateYawTorques(false);
                                //}
                                break;
                            }

                        case DayaRefNames.ELEVATOR:
                            {
                                if (isPitchCMD == true && sendDataY == true)
                                {
                                    double yValue = Math.Round(item.Value * (apPositionPitchFactor * 10) * Direction_Axis_Pitch);
                                    // Skip sudden jumps to 0
                                    if (yValue != 0)
                                    {
                                        item.valueAdjusted = yValue;
                                        moveToY(yValue);
                                        sendDataY = false;
                                    }
                                }
                                break;
                            }

                        case DayaRefNames.SPEED_IAS:
                            {
                                int speedVal = Convert.ToInt32((Math.Round((item.Value - 160) / torqueFactorAirSpeed)));

                                if (isPitchCMD == false)
                                {
                                    if (speedVal > 0)
                                    {
                                        additionalAirSpeedTorque = speedVal;
                                    }
                                    else
                                    {
                                        speedVal = 0;
                                    }

                                    if (additionalAirSpeedTorque != speedVal)
                                    {
                                        // Debug.WriteLine("additionalAirSpeedTorque " + additionalAirSpeedTorque);
                                        item.valueAdjusted = speedVal;
                                        additionalAirSpeedTorque = speedVal;
                                        UpdatePitchTorques(true);
                                    }
                                }

                                break;
                            }


                        case DayaRefNames.SPEED_GROUND:
                            {
                                if (axisTiller.IsEnabled() && item.Value > 0 && item.Value < 20) {
                                   
                                    item.valueAdjusted =  Convert.ToInt32(((((double)torqueTillerMax - (double)torqueTillerMin) / 20) * item.Value));

                                    if (tillerTorqueReduction != item.valueAdjusted)
                                    {
                                        if (item.valueAdjusted > 0)
                                        {
                                          tillerTorqueReduction = Convert.ToInt32(item.valueAdjusted);
                                          updateTillerTorque();
                                        }
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
                                    if (canMoveAfterCenter())
                                    {
                                        moveToX(0);
                                    }
                                } else
                                {
                                    // This timer is used to avoid disconnecting
                                    // immediately after first connecting
                                    Debug.WriteLine("Don't allow manual AP disc " + isRollCMD);
                                    sendDataAPDisconnect = false;
                                    timerAPdiconnect.Start();
                                }
                                
                                UpdateRollTorques(false);
                                Debug.WriteLine("updated isRollCMD " + isRollCMD);
                                break;
                            }

                        case DayaRefNames.PITCH_CMD:
                            {
                                bool oldisPitchCMD = isPitchCMD;

                                isPitchCMD = Convert.ToBoolean(dataRef.value);

                                if (isPitchCMD == false)
                                {
                                    // Reset Position
                                    if (canMoveAfterCenter())
                                    {
                                        moveToY(0);
                                    }
                               
                                    // This timer is used to avoid disconnecting
                                    // immediately after first connecting
                                    Debug.WriteLine("Don't allow manual AP disc " + isRollCMD);
                                    sendDataAPDisconnect = false;
                                    timerAPdiconnect.Start();
                                }
                                UpdatePitchTorques(false);
                                Debug.WriteLine("Updated isPitchCMD " + isPitchCMD);
                                break;
                            }

                        case DayaRefNames.IS_STALLING:
                            {
                                isStalling = Convert.ToBoolean(dataRef.value);
                                errorh.DisplayInfo("Stalling warning: " + isStalling);
                                UpdatePitchTorques(true);
                                break;
                            }

                        case DayaRefNames.MCP_AP_DISENGAGE:
                        {
                                bool isDisengaged = Convert.ToBoolean(dataRef.value);
                                errorh.DisplayInfo("MCP A/P switch disengaged " + isDisengaged);
                                if (isDisengaged)
                                {
                                    if (canMoveAfterCenter())
                                    {
                                        moveToX(0);
                                        moveToY(0);
                                    }
                                    isPitchCMD = false;
                                    isRollCMD = false;
                                }
                                UpdateTorques(true);
                                break;
                               
                        }

                        case DayaRefNames.HYDRAULICS_AVAILABLE:
                            {
                                isHydAvail = Convert.ToBoolean(dataRef.value);
                                errorh.DisplayInfo("Hydraulics updated " + isHydAvail);
                                if (lblHydPower.InvokeRequired)
                                {
                                    lblHydPower.Invoke(new MethodInvoker(delegate { lblHydPower.Text = isHydAvail ? "On" : "Off"; }));
                                }

                                if (!isHydAvail)
                                {
                                    axisPitch.HydraulicPower = false;
                                    axisYaw.HydraulicPower = false;
                                    axisTiller.HydraulicPower = false;
                                    if (!axisPitch.isCentering && !torquePitch.isManuallySet)
                                    {
                                        await Task.Delay(2000);
                                        speedPitch.SetSpeed(0);
                                    }

                                    if (!torqueYaw.isManuallySet)
                                    {
                                        await Task.Delay(2000);
                                        speedYaw.SetSpeed(0);
                                    }
                                }
                                else
                                {
                                    axisPitch.HydraulicPower = true;
                                    axisYaw.HydraulicPower = true;
                                    axisTiller.HydraulicPower = true;
                                    await Task.Delay(2000);
                                    speedPitch.SetSpeed(Centering_Speed_Pitch);
                                    speedYaw.SetSpeed(Centering_Speed_Yaw);
                                }

                                // move if on ground
                                // DropAxisFromWind();
                                UpdateTorques(false);
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
                                    if ((diff1 > AP_Disconnet_Roll_Threshold || diff2 > AP_Disconnet_Roll_Threshold) && lastRollMoved != -1)
                                    {
                                        item.valueAdjusted = diff1 * 1000;
                                        // Disconnect
                                        errorh.DisplayInfo("A/P overridden by roll. Disconnecting: | " + diff1 + " | " + diff2 + " | value: " + value + " | Previous value: " + lastRollMoved);
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
                                    if ((diff1 > AP_Disconnet_Pitch_Threshold || diff2 > AP_Disconnet_Pitch_Threshold) && lastPitchMoved != -1)
                                    {
                                        item.valueAdjusted = diff1 * 1000;
                                        // Disconnect
                                        errorh.DisplayInfo("A/P overridden by pitch. Disconnecting: | " + diff1 + " | " + diff2 + " | Value: " + value + " | Previous value: " + lastPitchMoved);
                                        DisconnectAPWithTimer();
                                    }

                                    lastPitchMoved = value;
                                    sendDataY = false;

                                } else
                                {
                                    //int pos = (int)(item.Value);
                                    //int newVal = (pos / 100);
                                    //int pos_min = 512;
                                    //int pos_max = 1024;
                                    ////int newval_min = torquePitchMin;
                                    ////int newval_max = torquePitchMax;

                                    //if (newVal < 0)
                                    //{
                                    //    newVal = newVal * -1;
                                    //}

                                    //if (pos < 512)
                                    //{
                                    //    pos = 1024 - pos;
                                    //}

                                    //pos = Math.Max(pos_min, Math.Min(pos, pos_max));
                                    //// ap pos from the input range to the output range
                                    //double proportion = (double)(pos - pos_min) / (pos_max - pos_min);
                                    //newVal = (int)(proportion * (torquePitchMax - torquePitchMin) + torquePitchMin);
                                  
                                    //// bring to 2 towards very center
                                    //if (pos <= 519)
                                    //{
                                    //    newVal = 2;
                                    //}

                                    //if (newVal != pitchTorqueFromPos)
                                    //{
                                    //    pitchTorqueFromPos = newVal;
                                    //    if (isPitchCMD == false)
                                    //    {
                                    //        UpdatePitchTorques(true);
                                    //    }
                                    //}
                                }

                                if (isPitchCMD == true && sendDataY == true)
                                {
                                    double yValue = Math.Round(item.Value * (apPositionPitchFactor * 10) * Direction_Axis_Pitch);
                                    // Skip sudden jumps to 0
                                    if (yValue != 0)
                                    {
                                        item.valueAdjusted = yValue;
                                        moveToY(yValue);
                                        sendDataY = false;
                                    }

                                }

                                break;
                            }

                        //case DayaRefNames.RUDDER_CAPT:
                        //    {
                        //        int pos = (int)(item.Value);
                        //        int newVal = (pos / 100);
                        //        int pos_min = 512;
                        //        int pos_max = 1024;

                        //        if (newVal < 0)
                        //        {
                        //            newVal = newVal * -1;
                        //        }

                        //        if (pos < 512)
                        //        {
                        //            pos = 1024 - pos;
                        //        }

                        //        pos = Math.Max(pos_min, Math.Min(pos, pos_max));
                        //        // ap pos from the input range to the output range
                        //        double proportion = (double)(pos - pos_min) / (pos_max - pos_min);
                        //        newVal = (int)(proportion * (torqueYawMax - torqueYawMin) + torqueYawMin);
                        //        //errorh.DisplayInfo($"rudder {pos} / {proportion} newVal {newVal}");
                        //        if (newVal != yawTorqueFromPos)
                        //        {
                        //            yawTorqueFromPos = newVal;
                        //            // errorh.DisplayInfo($"rudder {pos} / {proportion} newVal {newVal}");
                        //            UpdateYawTorques(true);
                        //        }
                        //        break;
                        //    }

                            //case DayaRefNames.WIND_SPEED:
                            //    {
                            //        // When winds exceed 19 knots and no Hyd power is available, the axis drop due to winds
                            //        if (item.Value > 19 && !isHydAvail)
                            //            {
                            //                errorh.DisplayInfo("Winds " + Convert.ToInt32(item.Value) + ", and Hyd Off dropping axis");
                            //                axisDroppedByWind = true;
                            //                DropAxisFromWind();
                            //            }
                            //        else
                            //            {
                            //                axisDroppedByWind = false;
                            //            }
                            //        break;
                            //    }

                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("failed to update sim var " + ex.Message + " " + ex.Data + " " + ex.StackTrace);
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

        private async void UpdatePitchTorques(bool async)
        {
            if (axisPitch.isCentering || torquePitch.isManuallySet)
            {
                return;
            }
            try
            {
                int newTorque = 0;
                if (isHydAvail)
                {
                    newTorque = pitchTorqueFromPos + additionalAirSpeedTorque;
                } else
                {
                    newTorque = torquePitchHydOff;
                }

                if (isPitchCMD)
                {
                    newTorque = torquePitchMax;
                }

                if (isStalling)
                {
                    newTorque += torqueStallingAdditional;
                }

                if (async)
                {
                    await Task.Run(() => torquePitch.SetTorqueAsync(GetMaxMinPitchTorque(newTorque)));
                }
                else
                {
                    torquePitch.SetTorque(GetMaxMinPitchTorque(newTorque));
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine("failed to update pitch torque " + ex.Message + " " + ex.Data);
                errorh.DisplayError("failed to update pitch torque " + ex.Message);
            }
        }

        private async void UpdateRollTorques(bool async)
        {
            if (axisRoll.isCentering || torqueRoll.isManuallySet)
            {
                return;
            }
            try
            {
                int newTorque = torqueRollMin;
                if (!isHydAvail)
                {
                    newTorque = torqueRollHydOff;
                }
                if (isRollCMD || axisRoll.isTrimming)
                {
                    newTorque = torqueRollMax;
                }


                if (async)
                {
                    await Task.Run(() => torqueRoll.SetTorqueAsync(newTorque));
                }
                else
                {
                    torqueRoll.SetTorque(newTorque);
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine("failed to update YAW torque " + ex.Message + " " + ex.Data);
                errorh.DisplayError("failed to update YAW torque " + ex.Message);
            }
        }


        private async void UpdateYawTorques(bool async)
        {
            if (axisYaw.isCentering || torqueYaw.isManuallySet)
            {
                return;
            }
            try
            {
           //     int newTorque = !isHydAvail ? torqueYawHydOff : yawTorqueFromPos;

                int newTorque = !isHydAvail || axisYaw.isTrimming ? torqueYawHydOff : yawTorqueFromPos;
             //   errorh.DisplayInfo($"update trq rudder {newTorque} {yawTorqueFromPos} {isHydAvail} {axisYaw.isTrimming} ");

                if (async)
                {
                    await Task.Run(() => torqueYaw.SetTorqueAsync(newTorque));
                }
                else
                {
                    torqueYaw.SetTorque(newTorque);
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine("failed to update YAW torque " + ex.Message + " " + ex.Data);
                errorh.DisplayError("failed to update YAW torque " + ex.Message);
            }
        }

        

        // Don't use more than the max or min torques
        private int GetMaxMinPitchTorque(int torque)
        {
            if (torque > torquePitchHydOff)
            {
                return torquePitchHydOff;
            }

            if (torque <=0)
            {
                return 0;
            }

            return torque;
        } 

        private async void updateTillerTorque()
        {
            int torqueTillerBase = isHydAvail ? torqueTillerMax : torqueTillerHydOff;
            await Task.Run(() => torqueTiller.SetTorque(torqueTillerBase - tillerTorqueReduction));
        }

        private async void UpdateTorques(bool async)
        {
            errorh.DisplayInfo("update all torques");
            //int torqueYawBase = isHydAvail ? torqueYawMin : torqueYawHydOff;
            //torqueYaw.SetTorque(torqueYawBase);
            UpdatePitchTorques(async);
            UpdateRollTorques(async);
            UpdateYawTorques(async);
            updateTillerTorque();
        }

        private void DropAxisFromWind()
        {
            DataRefTableItem airSpeed = dataRefs[DayaRefNames.SPEED_IAS];
            double airSpeedValue = Convert.ToDouble(airSpeed.Value);
            // move if on ground and axisDroppedByWind
            if (airSpeedValue < 20 && canMoveAfterCenter())
            {
                axisPitch.ChangeAxisSpeed(80000);
                torquePitch.SetTorque(torquePitchHydOff);
                torqueYaw.SetTorque(torqueYawHydOff);
                axisPitch.MoveToHydPos(axisDroppedByWind);
                axisYaw.MoveToHydPos(axisDroppedByWind);
            }
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


        // Don't move axis until they are centered
        private bool canMoveAfterCenter()
        {
            if (!autoCenterOnStatart || (autoCenterOnStatart && axisPitch.AxisCentered && axisYaw.AxisCentered))
            {
                return true;
            }
            return false;
        }


        private void UpdateTorquePitchLabelBack(int value)
        {
            Invoke(new MethodInvoker(delegate () { lblTorquePitchBack.Text = value.ToString(); }));
        }

        private void UpdateTorquePitchLabelFwd(int value)
        {
            Invoke(new MethodInvoker(delegate () { lblTorquePitchFwd.Text = value.ToString(); }));

        }

        private void UpdateTorqueRollLabelAft(int value)
        {
            Invoke(new MethodInvoker(delegate () { lblTorqueRollAft.Text = value.ToString(); }));
        }

        private void UpdateTorqueRollLabelFwd(int value)
        {
            Invoke(new MethodInvoker(delegate () { lblTorqueRollFwd.Text = value.ToString(); }));

        }

        private void UpdateTorqueYawLabel(int value)
        {
            Invoke(new MethodInvoker(delegate () { lblTorqueYawFwd.Text = value.ToString(); }));

        }

        // Disconnect the AP when the wheel/Column is moved.
        // Wait two seconds before this check can happen again to 
        // avoid immediate second disconnection
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
            axisTiller.MoveTo(0);
            errorh.DisplayInfo("Moved Tiller to 0");
            txtbxRollPosition.Text = "0";
            txtbxPitchPosition.Text = "0";
            txtbxYawPosition.Text = "0";
            txtbxTillerPosition.Text = "0";
        }

        private void btnGoTo_Click(object sender, EventArgs e)
        {
            try {
                torquePitch.isManuallySet = true;
                torquePitch.SetTorque(torquePitchMax);
                torqueRoll.isManuallySet = true;
                torqueRoll.SetTorque(torqueRollMax);
                axisRoll.MoveTo(Convert.ToDouble(txtbxRollPosition.Text));
                axisPitch.MoveTo(Convert.ToDouble(txtbxPitchPosition.Text));
                axisYaw.MoveTo(Convert.ToDouble(txtbxYawPosition.Text));
                axisTiller.MoveTo(Convert.ToDouble(txtbxTillerPosition.Text));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("failed to move " + ex.Message + " " + ex.Data);
            }
        }

        private async void StartDynamicTorques()
        {
            torquePitch.ResetHome();
            torqueRoll.ResetHome();

            torquePitch.StartDynamicTorque(torquePitchMin, torquePitchMax);
            torqueRoll.StartDynamicTorque(torqueRollMin, torqueRollMax);
            torqueYaw.StartDynamicTorque(torqueYawMin, torqueYawMax);
            //var taskPitch = Task.Run(() => torquePitch.DynamicTorque(torquePitchMin, torquePitchMax));
            //var taskRoll = Task.Run(() => torqueRoll.DynamicTorque(torqueRollMin, torqueRollMax));
            //var taskYaw = Task.Run(() => torqueYaw.DynamicTorque(torqueYawMin, torqueYawMax));
        }

        private void chkAutoConnect_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.AutoConnect = chkAutoConnect.Checked;
            //Save Setting
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
                torquePitch.isManuallySet = true;
                torquePitch.SetTorque(Int32.Parse(txbPitchTorque.Text));
            }

            if (txbYawTorque.Text != "")
            {
                torqueYaw.SetTorque(Int32.Parse(txbYawTorque.Text));
            }

            if (txbTillerTorque.Text != "")
            {
                torqueTiller.SetTorque(Int32.Parse(txbTillerTorque.Text));
            }
        }

        private void btnTorqueDefault_Click(object sender, EventArgs e)
        {
            torquePitch.isManuallySet = false;
            torqueRoll.isManuallySet = false;
            torqueRoll.SetTorque(torqueRollMin);
            torquePitch.SetTorque(torquePitchMin);
            torqueYaw.SetTorque(torqueYawMin);
            torqueTiller.SetTorque(torqueTillerMax);
        }

        private void btnSpeedTest_Click(object sender, EventArgs e)
        {
            if (txbPitchSpeedTest.Text != "")
            {
                speedPitch.SetSpeed(Int32.Parse(txbPitchSpeedTest.Text));
                Properties.Settings.Default.Centering_Speed_Pitch = Int32.Parse(txbPitchSpeedTest.Text);
            }

            if (txbRollSpeedTest.Text != "")
            {
                speedRoll.SetSpeed(Int32.Parse(txbRollSpeedTest.Text));
                Properties.Settings.Default.Centering_Speed_Roll = Int32.Parse(txbRollSpeedTest.Text);
            }

            if (txbYawSpeedTest.Text != "")
            {
                speedYaw.SetSpeed(Int32.Parse(txbYawSpeedTest.Text));
                Properties.Settings.Default.Centering_Speed_Yaw = Int32.Parse(txbYawSpeedTest.Text);
            }

            if (txbTillerSpeedTest.Text != "")
            {
                speedTiller.SetSpeed(Int32.Parse(txbTillerSpeedTest.Text));
                Properties.Settings.Default.Centering_Speed_Tiller = Int32.Parse(txbTillerSpeedTest.Text);
            }
            Properties.Settings.Default.Save();
        }

        private void btnRecenterSpeedRead_Click(object sender, EventArgs e)
        {
            txbPitchSpeedTest.Text = speedPitch.GetSpeed().ToString();
            txbRollSpeedTest.Text = speedRoll.GetSpeed().ToString();
            txbYawSpeedTest.Text = speedYaw.GetSpeed().ToString();
            txbTillerSpeedTest.Text = speedTiller.GetSpeed().ToString();
        }


        private void btnBounceSet_Click(object sender, EventArgs e)
        {
            if (txbBouncePitch.Text != "")
            {
                speedPitch.SetBounceGain(Int32.Parse(txbBouncePitch.Text));
                Properties.Settings.Default.Dampening_Pitch = Int32.Parse(txbBouncePitch.Text);
            }

            if (txbBounceRoll.Text != "")
            {
                speedRoll.SetBounceGain(Int32.Parse(txbBounceRoll.Text));
                Properties.Settings.Default.Dampening_Roll = Int32.Parse(txbBounceRoll.Text);
            }

            if (txbBounceYaw.Text != "")
            {
                speedYaw.SetBounceGain(Int32.Parse(txbBounceYaw.Text));
                Properties.Settings.Default.Dampening_Yaw = Int32.Parse(txbBounceYaw.Text);
            }

            if (txbBounceTiller.Text != "")
            {
                speedTiller.SetBounceGain(Int32.Parse(txbBounceTiller.Text));
                Properties.Settings.Default.Dampening_Tiller = Int32.Parse(txbBounceTiller.Text);
            }

            Properties.Settings.Default.Save();
        }

        private void btnBounceGet_Click(object sender, EventArgs e)
        {

            txbBouncePitch.Text = speedPitch.GetBounceGain().ToString();
            txbBounceRoll.Text = speedRoll.GetBounceGain().ToString();
            txbBounceYaw.Text = speedYaw.GetBounceGain().ToString();
            txbBounceTiller.Text = speedTiller.GetBounceGain().ToString();
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
            SetAppSettings(false);

        }

        private void ShowFormError(string message)
        {
            Invoke(new MethodInvoker(delegate () { rtxtLog.Text = message + rtxtLog.Text; }));
        }

    
        // Sef center flight controls bases on prosim control position
        private void btnCenterControls_Click(object sender, EventArgs e)
        {
            errorh.DisplayInfo("Center all axis from command");
            centerAllAxis();
        }

        private async void centerAllAxis()
        {
            axisPitch.isCentering = true;
            axisRoll.isCentering = true;    
            axisYaw.isCentering = true;
            axisTiller.isCentering = true;

            torquePitch.isManuallySet = true;
            torqueRoll.isManuallySet = true;
            torqueYaw.isManuallySet = true;
            torqueTiller.isManuallySet = true;

            speedPitch.SetSpeed(Properties.Settings.Default.Centering_Speed_Pitch);
            speedYaw.SetSpeed(Properties.Settings.Default.Centering_Speed_Yaw);
            torqueRoll.SetTorque(torqueRollMax);
            torqueYaw.SetTorque(torqueYawHydOff);
            torqueTiller.SetTorque(torqueTillerMax);
            torquePitch.SetTorque(torquePitchMax);

            var taskHomeRoll = axisRoll.MoveToHome();
            var taskHomePitch = axisPitch.MoveToHome();
            var taskHomeYaw = axisYaw.MoveToHome();
            var taskHomeTiller = axisTiller.MoveToHome();
            await Task.WhenAll(taskHomeRoll, taskHomePitch, taskHomeYaw, taskHomeTiller);

            await Task.Delay(4000);

            if (connection.isConnected)
            {
                var taskPitch = Task.Run(() => axisPitch.CenterAxis(
                     DayaRefNames.ELEVATOR_CPTN,
                     Properties.Settings.Default.Center_Calibration_Speed_Pitch,
                     axisDroppedByWind
                 ));

                var taskRoll = Task.Run(() => axisRoll.CenterAxis(
                     DayaRefNames.AILERON_CPTN,
                     Properties.Settings.Default.Center_Calibration_Speed_Roll,
                     axisDroppedByWind
                 ));

                var taskYaw = Task.Run(() => axisYaw.CenterAxis(
                    DayaRefNames.RUDDER_CAPT,
                    Properties.Settings.Default.Center_Calibration_Speed_Yaw,
                    axisDroppedByWind
                ));

                var taskTiller = Task.Run(() => axisTiller.CenterAxis(
                    DayaRefNames.TILLER_CAPT,
                    Properties.Settings.Default.Center_Calibration_Speed_Tiller,
                    axisDroppedByWind
                ));

                errorh.DisplayInfo("Starting centering");

                // Await all at once
                await Task.WhenAll(taskPitch, taskRoll,taskYaw, taskTiller);
                
                errorh.DisplayInfo("Finished centering");


            } else
            {
                errorh.DisplayError("Cannot center axes when Prosim is not connected");
            }

            axisPitch.isCentering = false;
            axisRoll.isCentering = false;
            axisYaw.isCentering = false;
            axisTiller.isCentering = false;
            torquePitch.isManuallySet = false;
            torqueRoll.isManuallySet = false;
            torqueYaw.isManuallySet = false;
            torqueTiller.isManuallySet = false;

            errorh.DisplayInfo("Reset Torque");
            if (!isHydAvail)
            {
                errorh.DisplayInfo("Reset Pitch Centering Speed to 0");
                speedPitch.SetSpeed(0);
                errorh.DisplayInfo("Reset Pitch Centering Speed to 0");
                speedYaw.SetSpeed(0);
            }
            UpdateTorques(false);
            torquePitch.ResetHome();
            torqueRoll.ResetHome();
            torqueYaw.ResetHome();
            torqueTiller.ResetHome();
        }

        private void chkAutoCenter_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Auto_Center_On_Start = chkAutoCenter.Checked;
            Properties.Settings.Default.Save();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

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
        public const string IS_STALLING = "system.gates.B_STICKSHAKER";


        public const string AILERON_CPTN = "system.analog.A_FC_AILERON_CAPT";
        public const string ELEVATOR_CPTN = "system.analog.A_FC_ELEVATOR_CAPT";
        public const string RUDDER_CAPT = "system.analog.A_FC_RUDDER_CAPT";
        public const string TILLER_CAPT = "system.analog.A_FC_TILLER_CAPT";


        public const string PITCH_CMD = "system.gates.B_PITCH_CMD";
        public const string ROLL_CMD = "system.gates.B_ROLL_CMD";
        
        public const string HYD_PRESS = "aircraft.hidraulics.sysA.pressure";
        public const string HYDRAULICS_AVAILABLE = "system.gates.B_HYDRAULICS_AVAILABLE";
        
        public const string THRUST_1 = "aircraft.engines.1.thrust";
        public const string THRUST_2 = "aircraft.engines.2.thrust";

        public const string SPEED_IAS = "aircraft.speed.ias";
        public const string SPEED_GROUND = "aircraft.speed.ground";

        public const string PITCH = "aircraft.pitch";

        public const string MCP_AP_DISENGAGE = "system.switches.S_MCP_AP_DISENGAGE";

        public const string WIND_SPEED = "environment.wind.speed";

    }

}
