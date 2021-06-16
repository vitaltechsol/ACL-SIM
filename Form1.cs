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


// aircraft.flightControls.spoilerLeft
// aircraft.flightControls.spoilerRight
// system.gates.B_PITCH_CMD



namespace LoadForceSim
{

    public partial class Form1 : Form
    {
        // Our main ProSim connection
        ProSimConnect connection = new ProSimConnect();
        Dictionary<String, DataRefTableItem> dataRefs = new Dictionary<string, DataRefTableItem>();
        static string portName = "COM3";
        static SerialPort port;
        int baud = 115200;
        bool isRollCMD = false;
        bool isPitchCMD = false;
        int offsetX = 7000;
        int offsetY = 500;

        public Form1()
        {
            InitializeComponent();
            // Register to receive connect and disconnect events
            connection.onConnect += connection_onConnect;
            connection.onDisconnect += connection_onDisconnect;

            if (SerialPort.GetPortNames().Count() >= 0)
            {
                foreach (string p in SerialPort.GetPortNames())
                {
                    Debug.WriteLine(p);
                }
            }

            BeginSerial(baud, portName);
            port.Open();
            //port.Write("<Y_POS, 0, 7500>");
            //port.Write("<Y_POS, 0, -7500>");
            //port.Write("<Y_POS, 0, 0>");
            //Thread.Sleep(200);
            //port.Write("<X_POS, 0, -6500>");
            //port.Write("<X_POS, 0, 6500>");
            //port.Write("<X_POS, 0, 0>");



            //while (true)
            //{
            //    string a = port.ReadExisting();
            //    Debug.WriteLine(a);
            //    Thread.Sleep(200);
            //}
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

            this.add_data_ref(DayaRefNames.ROLL_CMD);
            this.add_data_ref(DayaRefNames.PITCH_CMD);

            this.add_data_ref(DayaRefNames.PITCH);
            this.add_data_ref(DayaRefNames.THRUST_1);

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
                                if (isRollCMD == true)
                                {
                                    item.ValueConverted = Convert.ToDouble(dataRef.value) * offsetX;
                                    moveToX(item.ValueConverted);
                                }
                                break;

                            }
                        case DayaRefNames.PITCH:
                            {
                                if (isPitchCMD == true)
                                {
                                    item.ValueConverted = Convert.ToDouble(dataRef.value) * offsetY;
                                    moveToY(item.ValueConverted);

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

                    }

                    //if (name == DayaRefNames.AILERON_LEFT && isRollCMD == true)
                    //{
                    //    item.ValueConverted = Convert.ToDouble(dataRef.value) * 7000;
                    //    string arduLine = "<X_POS, 0, " + item.ValueConverted + "> ";
                    //    port.Write(arduLine);
                    //}

                    //if (name == DayaRefNames.PITCH && isPitchCMD == true)
                    //{
                    //    item.ValueConverted = Convert.ToDouble(dataRef.value) * 500;
                    //    string arduLine = "<Y_POS, 0, " + item.ValueConverted + "> ";
                    //    port.Write(arduLine);
                    //}
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
            string arduLine = "<X_POS, 0, " + value + ">";
            port.Write(arduLine);
        }

        // Pitch
        private void moveToY(double value)
        {
            string arduLine = "<Y_POS, 0, " + value + ">";
            port.Write(arduLine);
        }


        private void btnCenterOut_Click(object sender, EventArgs e)
        {
            moveToX(0);
            moveToY(0);
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

        public const string THRUST_1 = "aircraft.engines.1.thrust";
        public const string PITCH = "aircraft.pitch";

    }

}
