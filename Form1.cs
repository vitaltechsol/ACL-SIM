using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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

        enum season
        {
            asdf,
            wer
        }

 

        public Form1()
        {
            InitializeComponent();
            // Register to receive connect and disconnect events
            connection.onConnect += connection_onConnect;
            connection.onDisconnect += connection_onDisconnect;
        }

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


            // Use the ordered list to add items to the table
            //foreach (DataRefDescription description in descriptions.OrderBy(d => d.Name))
            //{
            //    this.add_data_ref(description);
            //}


            this.add_data_ref(DayaRefNames.SPOLER_LEFT);
            this.add_data_ref(DayaRefNames.B_PITCH_CMD);
            this.add_data_ref(DayaRefNames.PITCH);
            this.add_data_ref(DayaRefNames.THRUST_1);

          //  "simulator.jetway.toggle";


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
                DataRefTableItem item = new DataRefTableItem() { dataRef = dataRef, Description = "", DataType ="" };
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
                }
                catch
                {

                }

                // Signal the DataRefTable to update the row, so the new value is displayed
                Invoke(new MethodInvoker(delegate () {
                    if (!IsDisposed)
                        dataRefTableItemBindingSource.ResetItem(item.index);
                }));
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

    }

    public static class DayaRefNames
    {

        public const string SPOLER_LEFT = "aircraft.flightControls.spoilerLeft";
        public const string B_PITCH_CMD = "system.gates.B_PITCH_CMD";
        public const string THRUST_1 = "aircraft.engines.1.thrust";
        public const string PITCH = "aircraft.pitch";


    }

}
