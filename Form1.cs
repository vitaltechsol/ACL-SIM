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
        static SerialPort port;
        int baud = 115200;
        bool isRollCMD = false;
        bool isPitchCMD = false;
        bool isHydAvail = false;
        bool isElecHydPump1On = false;
        bool isElecHydPump2On = false;


        int offsetX = 7000;
        int offsetY = 500;
        int hydOffPitchPosition = -9500;
        int maxX = 4000;
        int maxY = 8000;
        int minX = -4000;
        int minY = -12000;
        Timer timerX;
        static bool sendDataX = false;
        Timer timerY;
        static bool sendDataY = false;


        public Form1()
        {
            InitializeComponent();
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


            if (SerialPort.GetPortNames().Count() >= 0)
            {
                foreach (string p in SerialPort.GetPortNames())
                {
                    Debug.WriteLine(p);
                }
            }

            BeginSerial(baud, portName);
            port.Open();
           
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
            try
            {
                connection.Connect(hostnameInput.Text);
                updateStatusLabel();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error connecting to ProSim737 System: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            this.add_data_ref(DayaRefNames.ROLL_CMD);
            this.add_data_ref(DayaRefNames.PITCH_CMD);

            this.add_data_ref(DayaRefNames.PITCH_CMD);

            this.add_data_ref(DayaRefNames.PITCH_CMD);
            this.add_data_ref(DayaRefNames.PITCH_CMD);


            this.add_data_ref(DayaRefNames.HYD_AVIAL);
            this.add_data_ref(DayaRefNames.HYD_PRESS);

            this.add_data_ref(DayaRefNames.S_OH_ELEC_HYD_PUMP_1);
            this.add_data_ref(DayaRefNames.S_OH_ELEC_HYD_PUMP_2);

            //  "simulator.ajetway.toggle";


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
                        case DayaRefNames.PITCH:
                            {
                                if (isPitchCMD == true  && sendDataY == true)
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
                                }
                                isRollCMD = Convert.ToBoolean(dataRef.value);
                                Debug.WriteLine("updated isPitchCMD "  + isRollCMD);
                                break;
                            }

                        case DayaRefNames.PITCH_CMD:
                            {
                                if (isPitchCMD == true)
                                {
                                    // Reset Position
                                    moveToY(0);
                                }
                                isPitchCMD = Convert.ToBoolean(dataRef.value);
                                Debug.WriteLine("updated isPitchCMD " + isPitchCMD);
                                break;
                            }

                        case DayaRefNames.S_OH_ELEC_HYD_PUMP_1:
                            {
                              
                                isElecHydPump1On = Convert.ToBoolean(dataRef.value);
                                Debug.WriteLine("updated isElecHydPump1On " + isElecHydPump1On);
                                if (isElecHydPump1On == false && isElecHydPump2On == false)
                                {
                                    // When hydraulics are off move to max pitch
                                    moveToY(hydOffPitchPosition);
                                } else
                                {
                                    // reset position
                                    speedPitch(80000);
                                    moveToY(0);
                                }

                                break;
                            }

                        case DayaRefNames.S_OH_ELEC_HYD_PUMP_2:
                            {

                                isElecHydPump2On = Convert.ToBoolean(dataRef.value);
                                Debug.WriteLine("updated isElecHydPump2On " + isElecHydPump2On);
                                if (isElecHydPump1On == false && isElecHydPump2On == false)
                                {
                                    // When hydraulics are off move to max pitch
                                    moveToY(hydOffPitchPosition);
                                }
                                else
                                {
                                    // reset position
                                    moveToY(0);
                                }

                                break;
                            }


                    }


                }
                catch
                {

                }

                // Signal the DataRefTable to update the row, so the new value is displayed
                Invoke(new MethodInvoker(delegate ()
                {
                    if (!IsDisposed)
                        dataRefTableItemBindingSource.ResetItem(item.index);
                }));
            }
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
        private void speedPitch(double value)
        {
     
            string arduLine = "<PITCH_SPEED, 0, " + value + ">";
            port.Write(arduLine);
            Debug.WriteLine("updated pitch speed " + value);


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
        public const string PITCH_CMD = "system.gates.B_PITCH_CMD";
        public const string ROLL_CMD = "system.gates.B_ROLL_CMD";

        public const string HYD_PRESS = "aircraft.hidraulics.sysA.pressure";
        public const string HYD_AVIAL = "system.gates.B_HYDRAULICS_AVAILABLE";
        public const string S_OH_ELEC_HYD_PUMP_1 = "system.switches.S_OH_ELEC_HYD_PUMP_1";
        public const string S_OH_ELEC_HYD_PUMP_2 = "system.switches.S_OH_ELEC_HYD_PUMP_2";





        public const string THRUST_1 = "aircraft.engines.1.thrust";

        public const string PITCH = "aircraft.pitch";

    }

}
