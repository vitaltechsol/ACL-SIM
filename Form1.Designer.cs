
namespace ACLSim
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;


        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.connectButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.hostnameInput = new System.Windows.Forms.TextBox();
            this.connectionStatusLabel = new System.Windows.Forms.Label();
            this.dataRefView = new System.Windows.Forms.DataGridView();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.valueDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataRefTableItemBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.valueConverted = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fillTableWorker = new System.ComponentModel.BackgroundWorker();
            this.btnGoTo = new System.Windows.Forms.Button();
            this.txtbxPitchPosition = new System.Windows.Forms.TextBox();
            this.btnCenterOut = new System.Windows.Forms.Button();
            this.txtbxRollPosition = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.chkAutoConnect = new System.Windows.Forms.CheckBox();
            this.txbRollTorque = new System.Windows.Forms.TextBox();
            this.btnTorqueTest = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabStatus = new System.Windows.Forms.TabPage();
            this.chkBoxStatus = new System.Windows.Forms.CheckBox();
            this.tabConfig = new System.Windows.Forms.TabPage();
            this.propertyGridSettings = new System.Windows.Forms.PropertyGrid();
            this.tabTest = new System.Windows.Forms.TabPage();
            this.label10 = new System.Windows.Forms.Label();
            this.txbBounceYaw = new System.Windows.Forms.TextBox();
            this.txbYawSpeedTest = new System.Windows.Forms.TextBox();
            this.txbYawTorque = new System.Windows.Forms.TextBox();
            this.txtbxYawPosition = new System.Windows.Forms.TextBox();
            this.btnBounceGet = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.txbBouncePitch = new System.Windows.Forms.TextBox();
            this.txbBounceRoll = new System.Windows.Forms.TextBox();
            this.btnBounceSet = new System.Windows.Forms.Button();
            this.btnRecenterSpeedRead = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txbPitchSpeedTest = new System.Windows.Forms.TextBox();
            this.txbRollSpeedTest = new System.Windows.Forms.TextBox();
            this.btnSpeedSet = new System.Windows.Forms.Button();
            this.btnTorqueDefault = new System.Windows.Forms.Button();
            this.txbPitchTorque = new System.Windows.Forms.TextBox();
            this.lblTorquePitchFwd = new System.Windows.Forms.Label();
            this.lblTorquePitchBack = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lblACLSIMVersion = new System.Windows.Forms.Label();
            this.rtxtLog = new System.Windows.Forms.RichTextBox();
            this.btnCenterControls = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.chkAutoCenter = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataRefView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataRefTableItemBindingSource)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabStatus.SuspendLayout();
            this.tabConfig.SuspendLayout();
            this.tabTest.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // connectButton
            // 
            this.connectButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(143)))), ((int)(((byte)(240)))));
            this.connectButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.connectButton.Location = new System.Drawing.Point(144, 67);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(98, 23);
            this.connectButton.TabIndex = 2;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = false;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point(141, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "ProSim Host";
            // 
            // hostnameInput
            // 
            this.hostnameInput.Location = new System.Drawing.Point(144, 41);
            this.hostnameInput.Name = "hostnameInput";
            this.hostnameInput.Size = new System.Drawing.Size(122, 20);
            this.hostnameInput.TabIndex = 1;
            // 
            // connectionStatusLabel
            // 
            this.connectionStatusLabel.AutoSize = true;
            this.connectionStatusLabel.ForeColor = System.Drawing.SystemColors.Control;
            this.connectionStatusLabel.Location = new System.Drawing.Point(141, 91);
            this.connectionStatusLabel.Name = "connectionStatusLabel";
            this.connectionStatusLabel.Size = new System.Drawing.Size(16, 13);
            this.connectionStatusLabel.TabIndex = 6;
            this.connectionStatusLabel.Text = "...";
            // 
            // dataRefView
            // 
            this.dataRefView.AllowUserToAddRows = false;
            this.dataRefView.AllowUserToDeleteRows = false;
            this.dataRefView.AllowUserToResizeRows = false;
            this.dataRefView.AutoGenerateColumns = false;
            this.dataRefView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataRefView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataRefView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameDataGridViewTextBoxColumn,
            this.valueDataGridViewTextBoxColumn});
            this.dataRefView.DataSource = this.dataRefTableItemBindingSource;
            this.dataRefView.Location = new System.Drawing.Point(6, 17);
            this.dataRefView.Name = "dataRefView";
            this.dataRefView.ReadOnly = true;
            this.dataRefView.RowHeadersVisible = false;
            this.dataRefView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataRefView.Size = new System.Drawing.Size(543, 252);
            this.dataRefView.TabIndex = 5;
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            this.nameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // valueDataGridViewTextBoxColumn
            // 
            this.valueDataGridViewTextBoxColumn.DataPropertyName = "Value";
            this.valueDataGridViewTextBoxColumn.HeaderText = "Value";
            this.valueDataGridViewTextBoxColumn.Name = "valueDataGridViewTextBoxColumn";
            this.valueDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // dataRefTableItemBindingSource
            // 
            this.dataRefTableItemBindingSource.AllowNew = false;
            this.dataRefTableItemBindingSource.DataSource = typeof(ACLSim.DataRefTableItem);
            this.dataRefTableItemBindingSource.Filter = "";
            // 
            // name
            // 
            this.name.Name = "name";
            // 
            // value
            // 
            this.value.Name = "value";
            this.value.ReadOnly = true;
            // 
            // valueConverted
            // 
            this.valueConverted.Name = "valueConverted";
            // 
            // fillTableWorker
            // 
            this.fillTableWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.fillTableWorker_DoWork);
            // 
            // btnGoTo
            // 
            this.btnGoTo.Location = new System.Drawing.Point(377, 49);
            this.btnGoTo.Name = "btnGoTo";
            this.btnGoTo.Size = new System.Drawing.Size(60, 23);
            this.btnGoTo.TabIndex = 3;
            this.btnGoTo.Text = "Move";
            this.btnGoTo.UseVisualStyleBackColor = true;
            this.btnGoTo.Click += new System.EventHandler(this.btnGoTo_Click);
            // 
            // txtbxPitchPosition
            // 
            this.txtbxPitchPosition.Location = new System.Drawing.Point(142, 49);
            this.txtbxPitchPosition.Name = "txtbxPitchPosition";
            this.txtbxPitchPosition.Size = new System.Drawing.Size(65, 20);
            this.txtbxPitchPosition.TabIndex = 1;
            this.txtbxPitchPosition.Text = "0";
            // 
            // btnCenterOut
            // 
            this.btnCenterOut.Location = new System.Drawing.Point(450, 49);
            this.btnCenterOut.Name = "btnCenterOut";
            this.btnCenterOut.Size = new System.Drawing.Size(75, 23);
            this.btnCenterOut.TabIndex = 4;
            this.btnCenterOut.Text = "Center ";
            this.btnCenterOut.UseVisualStyleBackColor = true;
            this.btnCenterOut.Click += new System.EventHandler(this.btnCenterOut_Click);
            // 
            // txtbxRollPosition
            // 
            this.txtbxRollPosition.Location = new System.Drawing.Point(220, 50);
            this.txtbxRollPosition.Name = "txtbxRollPosition";
            this.txtbxRollPosition.Size = new System.Drawing.Size(65, 20);
            this.txtbxRollPosition.TabIndex = 2;
            this.txtbxRollPosition.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(139, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Pitch";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(218, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(25, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Roll";
            // 
            // chkAutoConnect
            // 
            this.chkAutoConnect.AutoSize = true;
            this.chkAutoConnect.ForeColor = System.Drawing.SystemColors.Control;
            this.chkAutoConnect.Location = new System.Drawing.Point(248, 71);
            this.chkAutoConnect.Name = "chkAutoConnect";
            this.chkAutoConnect.Size = new System.Drawing.Size(90, 17);
            this.chkAutoConnect.TabIndex = 13;
            this.chkAutoConnect.Text = "Auto-connect";
            this.chkAutoConnect.UseVisualStyleBackColor = true;
            this.chkAutoConnect.CheckedChanged += new System.EventHandler(this.chkAutoConnect_CheckedChanged);
            // 
            // txbRollTorque
            // 
            this.txbRollTorque.Location = new System.Drawing.Point(220, 90);
            this.txbRollTorque.Name = "txbRollTorque";
            this.txbRollTorque.Size = new System.Drawing.Size(65, 20);
            this.txbRollTorque.TabIndex = 6;
            // 
            // btnTorqueTest
            // 
            this.btnTorqueTest.Location = new System.Drawing.Point(377, 90);
            this.btnTorqueTest.Name = "btnTorqueTest";
            this.btnTorqueTest.Size = new System.Drawing.Size(60, 23);
            this.btnTorqueTest.TabIndex = 7;
            this.btnTorqueTest.Text = "Test";
            this.btnTorqueTest.UseVisualStyleBackColor = true;
            this.btnTorqueTest.Click += new System.EventHandler(this.btnUpdateTorque_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabStatus);
            this.tabControl1.Controls.Add(this.tabConfig);
            this.tabControl1.Controls.Add(this.tabTest);
            this.tabControl1.Location = new System.Drawing.Point(16, 135);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(553, 323);
            this.tabControl1.TabIndex = 17;
            // 
            // tabStatus
            // 
            this.tabStatus.Controls.Add(this.chkBoxStatus);
            this.tabStatus.Controls.Add(this.dataRefView);
            this.tabStatus.Location = new System.Drawing.Point(4, 22);
            this.tabStatus.Name = "tabStatus";
            this.tabStatus.Padding = new System.Windows.Forms.Padding(3);
            this.tabStatus.Size = new System.Drawing.Size(545, 297);
            this.tabStatus.TabIndex = 0;
            this.tabStatus.Text = "Status";
            this.tabStatus.UseVisualStyleBackColor = true;
            // 
            // chkBoxStatus
            // 
            this.chkBoxStatus.AutoSize = true;
            this.chkBoxStatus.Location = new System.Drawing.Point(6, 275);
            this.chkBoxStatus.Name = "chkBoxStatus";
            this.chkBoxStatus.Size = new System.Drawing.Size(86, 17);
            this.chkBoxStatus.TabIndex = 22;
            this.chkBoxStatus.Text = "Show Status";
            this.chkBoxStatus.UseVisualStyleBackColor = true;
            this.chkBoxStatus.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // tabConfig
            // 
            this.tabConfig.Controls.Add(this.propertyGridSettings);
            this.tabConfig.Location = new System.Drawing.Point(4, 22);
            this.tabConfig.Name = "tabConfig";
            this.tabConfig.Padding = new System.Windows.Forms.Padding(3);
            this.tabConfig.Size = new System.Drawing.Size(545, 297);
            this.tabConfig.TabIndex = 2;
            this.tabConfig.Text = "Custom Config";
            this.tabConfig.UseVisualStyleBackColor = true;
            // 
            // propertyGridSettings
            // 
            this.propertyGridSettings.Location = new System.Drawing.Point(6, 21);
            this.propertyGridSettings.Name = "propertyGridSettings";
            this.propertyGridSettings.Size = new System.Drawing.Size(528, 286);
            this.propertyGridSettings.TabIndex = 0;
            this.propertyGridSettings.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGridSettings_PropertyValueChanged_1);
            // 
            // tabTest
            // 
            this.tabTest.Controls.Add(this.label10);
            this.tabTest.Controls.Add(this.txbBounceYaw);
            this.tabTest.Controls.Add(this.txbYawSpeedTest);
            this.tabTest.Controls.Add(this.txbYawTorque);
            this.tabTest.Controls.Add(this.txtbxYawPosition);
            this.tabTest.Controls.Add(this.btnBounceGet);
            this.tabTest.Controls.Add(this.label7);
            this.tabTest.Controls.Add(this.txbBouncePitch);
            this.tabTest.Controls.Add(this.txbBounceRoll);
            this.tabTest.Controls.Add(this.btnBounceSet);
            this.tabTest.Controls.Add(this.btnRecenterSpeedRead);
            this.tabTest.Controls.Add(this.label6);
            this.tabTest.Controls.Add(this.label5);
            this.tabTest.Controls.Add(this.label4);
            this.tabTest.Controls.Add(this.txbPitchSpeedTest);
            this.tabTest.Controls.Add(this.txbRollSpeedTest);
            this.tabTest.Controls.Add(this.btnSpeedSet);
            this.tabTest.Controls.Add(this.btnTorqueDefault);
            this.tabTest.Controls.Add(this.txbPitchTorque);
            this.tabTest.Controls.Add(this.txtbxPitchPosition);
            this.tabTest.Controls.Add(this.txbRollTorque);
            this.tabTest.Controls.Add(this.btnTorqueTest);
            this.tabTest.Controls.Add(this.label2);
            this.tabTest.Controls.Add(this.btnGoTo);
            this.tabTest.Controls.Add(this.txtbxRollPosition);
            this.tabTest.Controls.Add(this.btnCenterOut);
            this.tabTest.Controls.Add(this.label3);
            this.tabTest.Location = new System.Drawing.Point(4, 22);
            this.tabTest.Name = "tabTest";
            this.tabTest.Padding = new System.Windows.Forms.Padding(3);
            this.tabTest.Size = new System.Drawing.Size(545, 297);
            this.tabTest.TabIndex = 1;
            this.tabTest.Text = "Servo Test";
            this.tabTest.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(293, 25);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(28, 13);
            this.label10.TabIndex = 35;
            this.label10.Text = "Yaw";
            // 
            // txbBounceYaw
            // 
            this.txbBounceYaw.Location = new System.Drawing.Point(296, 162);
            this.txbBounceYaw.Name = "txbBounceYaw";
            this.txbBounceYaw.Size = new System.Drawing.Size(65, 20);
            this.txbBounceYaw.TabIndex = 34;
            // 
            // txbYawSpeedTest
            // 
            this.txbYawSpeedTest.Location = new System.Drawing.Point(296, 131);
            this.txbYawSpeedTest.Name = "txbYawSpeedTest";
            this.txbYawSpeedTest.Size = new System.Drawing.Size(65, 20);
            this.txbYawSpeedTest.TabIndex = 33;
            // 
            // txbYawTorque
            // 
            this.txbYawTorque.Location = new System.Drawing.Point(296, 91);
            this.txbYawTorque.Name = "txbYawTorque";
            this.txbYawTorque.Size = new System.Drawing.Size(65, 20);
            this.txbYawTorque.TabIndex = 32;
            // 
            // txtbxYawPosition
            // 
            this.txtbxYawPosition.Location = new System.Drawing.Point(296, 51);
            this.txtbxYawPosition.Name = "txtbxYawPosition";
            this.txtbxYawPosition.Size = new System.Drawing.Size(65, 20);
            this.txtbxYawPosition.TabIndex = 31;
            this.txtbxYawPosition.Text = "0";
            // 
            // btnBounceGet
            // 
            this.btnBounceGet.Location = new System.Drawing.Point(450, 159);
            this.btnBounceGet.Name = "btnBounceGet";
            this.btnBounceGet.Size = new System.Drawing.Size(75, 23);
            this.btnBounceGet.TabIndex = 30;
            this.btnBounceGet.Text = "Read";
            this.btnBounceGet.UseVisualStyleBackColor = true;
            this.btnBounceGet.Click += new System.EventHandler(this.btnBounceGet_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(68, 165);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(61, 13);
            this.label7.TabIndex = 29;
            this.label7.Text = "Dampening";
            this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txbBouncePitch
            // 
            this.txbBouncePitch.Location = new System.Drawing.Point(142, 162);
            this.txbBouncePitch.Name = "txbBouncePitch";
            this.txbBouncePitch.Size = new System.Drawing.Size(65, 20);
            this.txbBouncePitch.TabIndex = 28;
            // 
            // txbBounceRoll
            // 
            this.txbBounceRoll.Location = new System.Drawing.Point(220, 162);
            this.txbBounceRoll.Name = "txbBounceRoll";
            this.txbBounceRoll.Size = new System.Drawing.Size(65, 20);
            this.txbBounceRoll.TabIndex = 26;
            // 
            // btnBounceSet
            // 
            this.btnBounceSet.Location = new System.Drawing.Point(377, 159);
            this.btnBounceSet.Name = "btnBounceSet";
            this.btnBounceSet.Size = new System.Drawing.Size(60, 23);
            this.btnBounceSet.TabIndex = 27;
            this.btnBounceSet.Text = "Set";
            this.btnBounceSet.UseVisualStyleBackColor = true;
            this.btnBounceSet.Click += new System.EventHandler(this.btnBounceSet_Click);
            // 
            // btnRecenterSpeedRead
            // 
            this.btnRecenterSpeedRead.Location = new System.Drawing.Point(450, 130);
            this.btnRecenterSpeedRead.Name = "btnRecenterSpeedRead";
            this.btnRecenterSpeedRead.Size = new System.Drawing.Size(75, 23);
            this.btnRecenterSpeedRead.TabIndex = 25;
            this.btnRecenterSpeedRead.Text = "Read";
            this.btnRecenterSpeedRead.UseVisualStyleBackColor = true;
            this.btnRecenterSpeedRead.Click += new System.EventHandler(this.btnRecenterSpeedRead_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(44, 134);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(88, 13);
            this.label6.TabIndex = 24;
            this.label6.Text = "Re-center Speed";
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(88, 93);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 13);
            this.label5.TabIndex = 23;
            this.label5.Text = "Torque";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(85, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 22;
            this.label4.Text = "Position";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txbPitchSpeedTest
            // 
            this.txbPitchSpeedTest.Location = new System.Drawing.Point(142, 131);
            this.txbPitchSpeedTest.Name = "txbPitchSpeedTest";
            this.txbPitchSpeedTest.Size = new System.Drawing.Size(65, 20);
            this.txbPitchSpeedTest.TabIndex = 20;
            // 
            // txbRollSpeedTest
            // 
            this.txbRollSpeedTest.Location = new System.Drawing.Point(220, 131);
            this.txbRollSpeedTest.Name = "txbRollSpeedTest";
            this.txbRollSpeedTest.Size = new System.Drawing.Size(65, 20);
            this.txbRollSpeedTest.TabIndex = 18;
            // 
            // btnSpeedSet
            // 
            this.btnSpeedSet.Location = new System.Drawing.Point(377, 130);
            this.btnSpeedSet.Name = "btnSpeedSet";
            this.btnSpeedSet.Size = new System.Drawing.Size(60, 23);
            this.btnSpeedSet.TabIndex = 19;
            this.btnSpeedSet.Text = "Set";
            this.btnSpeedSet.UseVisualStyleBackColor = true;
            this.btnSpeedSet.Click += new System.EventHandler(this.btnSpeedTest_Click);
            // 
            // btnTorqueDefault
            // 
            this.btnTorqueDefault.Location = new System.Drawing.Point(450, 90);
            this.btnTorqueDefault.Name = "btnTorqueDefault";
            this.btnTorqueDefault.Size = new System.Drawing.Size(75, 23);
            this.btnTorqueDefault.TabIndex = 8;
            this.btnTorqueDefault.Text = "Revert";
            this.btnTorqueDefault.UseVisualStyleBackColor = true;
            this.btnTorqueDefault.Click += new System.EventHandler(this.btnTorqueDefault_Click);
            // 
            // txbPitchTorque
            // 
            this.txbPitchTorque.Location = new System.Drawing.Point(142, 90);
            this.txbPitchTorque.Name = "txbPitchTorque";
            this.txbPitchTorque.Size = new System.Drawing.Size(65, 20);
            this.txbPitchTorque.TabIndex = 5;
            // 
            // lblTorquePitchFwd
            // 
            this.lblTorquePitchFwd.AutoSize = true;
            this.lblTorquePitchFwd.BackColor = System.Drawing.Color.Transparent;
            this.lblTorquePitchFwd.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTorquePitchFwd.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(223)))), ((int)(((byte)(182)))));
            this.lblTorquePitchFwd.Location = new System.Drawing.Point(362, 20);
            this.lblTorquePitchFwd.Name = "lblTorquePitchFwd";
            this.lblTorquePitchFwd.Size = new System.Drawing.Size(36, 39);
            this.lblTorquePitchFwd.TabIndex = 18;
            this.lblTorquePitchFwd.Text = "0";
            // 
            // lblTorquePitchBack
            // 
            this.lblTorquePitchBack.AutoSize = true;
            this.lblTorquePitchBack.BackColor = System.Drawing.Color.Transparent;
            this.lblTorquePitchBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTorquePitchBack.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(223)))), ((int)(((byte)(184)))));
            this.lblTorquePitchBack.Location = new System.Drawing.Point(481, 20);
            this.lblTorquePitchBack.Name = "lblTorquePitchBack";
            this.lblTorquePitchBack.Size = new System.Drawing.Size(36, 39);
            this.lblTorquePitchBack.TabIndex = 19;
            this.lblTorquePitchBack.Text = "0";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(143)))), ((int)(((byte)(240)))));
            this.label8.Location = new System.Drawing.Point(485, 57);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(60, 13);
            this.label8.TabIndex = 20;
            this.label8.Text = "Fwd torque";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.Transparent;
            this.label9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(143)))), ((int)(((byte)(240)))));
            this.label9.Location = new System.Drawing.Point(366, 57);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 13);
            this.label9.TabIndex = 21;
            this.label9.Text = "Back torque";
            // 
            // lblACLSIMVersion
            // 
            this.lblACLSIMVersion.AutoSize = true;
            this.lblACLSIMVersion.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblACLSIMVersion.Location = new System.Drawing.Point(534, 9);
            this.lblACLSIMVersion.Name = "lblACLSIMVersion";
            this.lblACLSIMVersion.Size = new System.Drawing.Size(31, 13);
            this.lblACLSIMVersion.TabIndex = 22;
            this.lblACLSIMVersion.Text = "1.2.0";
            // 
            // rtxtLog
            // 
            this.rtxtLog.BackColor = System.Drawing.Color.Black;
            this.rtxtLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtxtLog.ForeColor = System.Drawing.SystemColors.ScrollBar;
            this.rtxtLog.Location = new System.Drawing.Point(16, 477);
            this.rtxtLog.Margin = new System.Windows.Forms.Padding(1);
            this.rtxtLog.Name = "rtxtLog";
            this.rtxtLog.ReadOnly = true;
            this.rtxtLog.Size = new System.Drawing.Size(549, 114);
            this.rtxtLog.TabIndex = 0;
            this.rtxtLog.Text = "";
            // 
            // btnCenterControls
            // 
            this.btnCenterControls.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(143)))), ((int)(((byte)(240)))));
            this.btnCenterControls.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCenterControls.Location = new System.Drawing.Point(366, 100);
            this.btnCenterControls.Name = "btnCenterControls";
            this.btnCenterControls.Size = new System.Drawing.Size(90, 23);
            this.btnCenterControls.TabIndex = 3;
            this.btnCenterControls.Text = "Center Controls";
            this.btnCenterControls.UseVisualStyleBackColor = false;
            this.btnCenterControls.Click += new System.EventHandler(this.btnCenterControls_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(6, 10);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(128, 100);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 25;
            this.pictureBox1.TabStop = false;
            // 
            // chkAutoCenter
            // 
            this.chkAutoCenter.AutoSize = true;
            this.chkAutoCenter.BackColor = System.Drawing.Color.Transparent;
            this.chkAutoCenter.ForeColor = System.Drawing.SystemColors.Control;
            this.chkAutoCenter.Location = new System.Drawing.Point(465, 104);
            this.chkAutoCenter.Name = "chkAutoCenter";
            this.chkAutoCenter.Size = new System.Drawing.Size(81, 17);
            this.chkAutoCenter.TabIndex = 26;
            this.chkAutoCenter.Text = "Auto-center";
            this.chkAutoCenter.UseVisualStyleBackColor = false;
            this.chkAutoCenter.CheckedChanged += new System.EventHandler(this.chkAutoCenter_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.ClientSize = new System.Drawing.Size(581, 603);
            this.Controls.Add(this.chkAutoCenter);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnCenterControls);
            this.Controls.Add(this.rtxtLog);
            this.Controls.Add(this.lblACLSIMVersion);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.lblTorquePitchBack);
            this.Controls.Add(this.lblTorquePitchFwd);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.chkAutoConnect);
            this.Controls.Add(this.connectionStatusLabel);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.hostnameInput);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "ACL-SIM";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.dataRefView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataRefTableItemBindingSource)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabStatus.ResumeLayout(false);
            this.tabStatus.PerformLayout();
            this.tabConfig.ResumeLayout(false);
            this.tabTest.ResumeLayout(false);
            this.tabTest.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox hostnameInput;
        private System.Windows.Forms.Label connectionStatusLabel;
        private System.Windows.Forms.DataGridView dataRefView;
        private System.Windows.Forms.DataGridViewTextBoxColumn name;
        private System.Windows.Forms.DataGridViewTextBoxColumn value;
        private System.Windows.Forms.DataGridViewTextBoxColumn valueConverted;
        private System.ComponentModel.BackgroundWorker fillTableWorker;
        private System.Windows.Forms.BindingSource dataRefTableItemBindingSource;
        private System.Windows.Forms.Button btnGoTo;
        private System.Windows.Forms.TextBox txtbxPitchPosition;
        private System.Windows.Forms.Button btnCenterOut;
        private System.Windows.Forms.TextBox txtbxRollPosition;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkAutoConnect;
        private System.Windows.Forms.TextBox txbRollTorque;
        private System.Windows.Forms.Button btnTorqueTest;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabStatus;
        private System.Windows.Forms.TabPage tabTest;
        private System.Windows.Forms.TextBox txbPitchTorque;
        private System.Windows.Forms.TabPage tabConfig;
        private System.Windows.Forms.TextBox txbPitchSpeedTest;
        private System.Windows.Forms.TextBox txbRollSpeedTest;
        private System.Windows.Forms.Button btnSpeedSet;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblTorquePitchFwd;
        private System.Windows.Forms.Label lblTorquePitchBack;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox chkBoxStatus;
        private System.Windows.Forms.PropertyGrid propertyGridSettings;
        private System.Windows.Forms.Button btnTorqueDefault;
        private System.Windows.Forms.Button btnRecenterSpeedRead;
        private System.Windows.Forms.Label lblACLSIMVersion;
        private System.Windows.Forms.Button btnBounceGet;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txbBouncePitch;
        private System.Windows.Forms.TextBox txbBounceRoll;
        private System.Windows.Forms.Button btnBounceSet;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn valueDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn valueConvertedDataGridViewTextBoxColumn;
        private System.Windows.Forms.RichTextBox rtxtLog;
        private System.Windows.Forms.TextBox txbBounceYaw;
        private System.Windows.Forms.TextBox txbYawSpeedTest;
        private System.Windows.Forms.TextBox txbYawTorque;
        private System.Windows.Forms.TextBox txtbxYawPosition;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btnCenterControls;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.CheckBox chkAutoCenter;
    }
}

