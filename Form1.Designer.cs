
namespace LoadForceSim
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
            this.connectButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.hostnameInput = new System.Windows.Forms.TextBox();
            this.connectionStatusLabel = new System.Windows.Forms.Label();
            this.dataRefView = new System.Windows.Forms.DataGridView();
            this.name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.valueConverted = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fillTableWorker = new System.ComponentModel.BackgroundWorker();
            this.btnGoTo = new System.Windows.Forms.Button();
            this.txtbxPitch = new System.Windows.Forms.TextBox();
            this.btnCenterOut = new System.Windows.Forms.Button();
            this.txtbxRoll = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.chkAutoConnect = new System.Windows.Forms.CheckBox();
            this.txbRollTorque = new System.Windows.Forms.TextBox();
            this.btnTorqueTest = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabStatus = new System.Windows.Forms.TabPage();
            this.tabConfig = new System.Windows.Forms.TabPage();
            this.tabTest = new System.Windows.Forms.TabPage();
            this.label7 = new System.Windows.Forms.Label();
            this.txtMinPitchPos = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.txtMinRoll = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.btnSaveServoConfigs = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnSpeedTestDefault = new System.Windows.Forms.Button();
            this.txbPitchSpeedTest = new System.Windows.Forms.TextBox();
            this.txbRollSpeedTest = new System.Windows.Forms.TextBox();
            this.btnSpeedTest = new System.Windows.Forms.Button();
            this.btnTorqueDefault = new System.Windows.Forms.Button();
            this.txbPitchTorque = new System.Windows.Forms.TextBox();
            this.lblTorquePitchFwd = new System.Windows.Forms.Label();
            this.lblTorquePitchBack = new System.Windows.Forms.Label();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.valueDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.valueConvertedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.descriptionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataRefTableItemBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dataRefView)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabStatus.SuspendLayout();
            this.tabTest.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataRefTableItemBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(333, 27);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(75, 23);
            this.connectButton.TabIndex = 5;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "ProSim Host";
            // 
            // hostnameInput
            // 
            this.hostnameInput.Location = new System.Drawing.Point(94, 29);
            this.hostnameInput.Name = "hostnameInput";
            this.hostnameInput.Size = new System.Drawing.Size(233, 20);
            this.hostnameInput.TabIndex = 3;
            // 
            // connectionStatusLabel
            // 
            this.connectionStatusLabel.AutoSize = true;
            this.connectionStatusLabel.Location = new System.Drawing.Point(13, 56);
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
            this.valueDataGridViewTextBoxColumn,
            this.valueConvertedDataGridViewTextBoxColumn,
            this.descriptionDataGridViewTextBoxColumn});
            this.dataRefView.DataSource = this.dataRefTableItemBindingSource;
            this.dataRefView.Location = new System.Drawing.Point(6, 17);
            this.dataRefView.Name = "dataRefView";
            this.dataRefView.ReadOnly = true;
            this.dataRefView.RowHeadersVisible = false;
            this.dataRefView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataRefView.Size = new System.Drawing.Size(728, 267);
            this.dataRefView.TabIndex = 5;
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
            this.btnGoTo.Location = new System.Drawing.Point(298, 49);
            this.btnGoTo.Name = "btnGoTo";
            this.btnGoTo.Size = new System.Drawing.Size(60, 23);
            this.btnGoTo.TabIndex = 8;
            this.btnGoTo.Text = "Move";
            this.btnGoTo.UseVisualStyleBackColor = true;
            this.btnGoTo.Click += new System.EventHandler(this.btnGoTo_Click);
            // 
            // txtbxPitch
            // 
            this.txtbxPitch.Location = new System.Drawing.Point(142, 49);
            this.txtbxPitch.Name = "txtbxPitch";
            this.txtbxPitch.Size = new System.Drawing.Size(65, 20);
            this.txtbxPitch.TabIndex = 7;
            this.txtbxPitch.Text = "0";
            // 
            // btnCenterOut
            // 
            this.btnCenterOut.Location = new System.Drawing.Point(371, 49);
            this.btnCenterOut.Name = "btnCenterOut";
            this.btnCenterOut.Size = new System.Drawing.Size(75, 23);
            this.btnCenterOut.TabIndex = 9;
            this.btnCenterOut.Text = "Center ";
            this.btnCenterOut.UseVisualStyleBackColor = true;
            this.btnCenterOut.Click += new System.EventHandler(this.btnCenterOut_Click);
            // 
            // txtbxRoll
            // 
            this.txtbxRoll.Location = new System.Drawing.Point(220, 50);
            this.txtbxRoll.Name = "txtbxRoll";
            this.txtbxRoll.Size = new System.Drawing.Size(65, 20);
            this.txtbxRoll.TabIndex = 10;
            this.txtbxRoll.Text = "0";
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
            this.chkAutoConnect.Location = new System.Drawing.Point(414, 31);
            this.chkAutoConnect.Name = "chkAutoConnect";
            this.chkAutoConnect.Size = new System.Drawing.Size(90, 17);
            this.chkAutoConnect.TabIndex = 13;
            this.chkAutoConnect.Text = "Auto-connect";
            this.chkAutoConnect.UseVisualStyleBackColor = true;
            this.chkAutoConnect.CheckedChanged += new System.EventHandler(this.chkAutoConnect_CheckedChanged);
            // 
            // txbRollTorque
            // 
            this.txbRollTorque.Location = new System.Drawing.Point(220, 114);
            this.txbRollTorque.Name = "txbRollTorque";
            this.txbRollTorque.Size = new System.Drawing.Size(65, 20);
            this.txbRollTorque.TabIndex = 14;
            // 
            // btnTorqueTest
            // 
            this.btnTorqueTest.Location = new System.Drawing.Point(298, 114);
            this.btnTorqueTest.Name = "btnTorqueTest";
            this.btnTorqueTest.Size = new System.Drawing.Size(60, 23);
            this.btnTorqueTest.TabIndex = 15;
            this.btnTorqueTest.Text = "Test";
            this.btnTorqueTest.UseVisualStyleBackColor = true;
            this.btnTorqueTest.Click += new System.EventHandler(this.btnUpdateTorque_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabStatus);
            this.tabControl1.Controls.Add(this.tabConfig);
            this.tabControl1.Controls.Add(this.tabTest);
            this.tabControl1.Location = new System.Drawing.Point(16, 84);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(748, 316);
            this.tabControl1.TabIndex = 17;
            // 
            // tabStatus
            // 
            this.tabStatus.Controls.Add(this.dataRefView);
            this.tabStatus.Location = new System.Drawing.Point(4, 22);
            this.tabStatus.Name = "tabStatus";
            this.tabStatus.Padding = new System.Windows.Forms.Padding(3);
            this.tabStatus.Size = new System.Drawing.Size(740, 290);
            this.tabStatus.TabIndex = 0;
            this.tabStatus.Text = "Status";
            this.tabStatus.UseVisualStyleBackColor = true;
            // 
            // tabConfig
            // 
            this.tabConfig.Location = new System.Drawing.Point(4, 22);
            this.tabConfig.Name = "tabConfig";
            this.tabConfig.Padding = new System.Windows.Forms.Padding(3);
            this.tabConfig.Size = new System.Drawing.Size(740, 290);
            this.tabConfig.TabIndex = 2;
            this.tabConfig.Text = "Config";
            this.tabConfig.UseVisualStyleBackColor = true;
            // 
            // tabTest
            // 
            this.tabTest.Controls.Add(this.label7);
            this.tabTest.Controls.Add(this.txtMinPitchPos);
            this.tabTest.Controls.Add(this.button1);
            this.tabTest.Controls.Add(this.txtMinRoll);
            this.tabTest.Controls.Add(this.button2);
            this.tabTest.Controls.Add(this.btnSaveServoConfigs);
            this.tabTest.Controls.Add(this.label6);
            this.tabTest.Controls.Add(this.label5);
            this.tabTest.Controls.Add(this.label4);
            this.tabTest.Controls.Add(this.btnSpeedTestDefault);
            this.tabTest.Controls.Add(this.txbPitchSpeedTest);
            this.tabTest.Controls.Add(this.txbRollSpeedTest);
            this.tabTest.Controls.Add(this.btnSpeedTest);
            this.tabTest.Controls.Add(this.btnTorqueDefault);
            this.tabTest.Controls.Add(this.txbPitchTorque);
            this.tabTest.Controls.Add(this.txtbxPitch);
            this.tabTest.Controls.Add(this.txbRollTorque);
            this.tabTest.Controls.Add(this.btnTorqueTest);
            this.tabTest.Controls.Add(this.label2);
            this.tabTest.Controls.Add(this.btnGoTo);
            this.tabTest.Controls.Add(this.txtbxRoll);
            this.tabTest.Controls.Add(this.btnCenterOut);
            this.tabTest.Controls.Add(this.label3);
            this.tabTest.Location = new System.Drawing.Point(4, 22);
            this.tabTest.Name = "tabTest";
            this.tabTest.Padding = new System.Windows.Forms.Padding(3);
            this.tabTest.Size = new System.Drawing.Size(740, 290);
            this.tabTest.TabIndex = 1;
            this.tabTest.Text = "Servo Config";
            this.tabTest.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(62, 83);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(64, 13);
            this.label7.TabIndex = 30;
            this.label7.Text = "Min Position";
            this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtMinPitchPos
            // 
            this.txtMinPitchPos.Location = new System.Drawing.Point(142, 78);
            this.txtMinPitchPos.Name = "txtMinPitchPos";
            this.txtMinPitchPos.Size = new System.Drawing.Size(65, 20);
            this.txtMinPitchPos.TabIndex = 26;
            this.txtMinPitchPos.Text = "0";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(298, 78);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(60, 23);
            this.button1.TabIndex = 27;
            this.button1.Text = "Move";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // txtMinRoll
            // 
            this.txtMinRoll.Location = new System.Drawing.Point(220, 79);
            this.txtMinRoll.Name = "txtMinRoll";
            this.txtMinRoll.Size = new System.Drawing.Size(65, 20);
            this.txtMinRoll.TabIndex = 29;
            this.txtMinRoll.Text = "0";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(371, 78);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 28;
            this.button2.Text = "Center ";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // btnSaveServoConfigs
            // 
            this.btnSaveServoConfigs.Location = new System.Drawing.Point(648, 15);
            this.btnSaveServoConfigs.Name = "btnSaveServoConfigs";
            this.btnSaveServoConfigs.Size = new System.Drawing.Size(75, 23);
            this.btnSaveServoConfigs.TabIndex = 25;
            this.btnSaveServoConfigs.Text = "Save";
            this.btnSaveServoConfigs.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(44, 158);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(85, 13);
            this.label6.TabIndex = 24;
            this.label6.Text = "Rebound Speed";
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(51, 117);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 13);
            this.label5.TabIndex = 23;
            this.label5.Text = "Torque Lowest";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(62, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 13);
            this.label4.TabIndex = 22;
            this.label4.Text = "Max Position";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // btnSpeedTestDefault
            // 
            this.btnSpeedTestDefault.Location = new System.Drawing.Point(371, 154);
            this.btnSpeedTestDefault.Name = "btnSpeedTestDefault";
            this.btnSpeedTestDefault.Size = new System.Drawing.Size(75, 23);
            this.btnSpeedTestDefault.TabIndex = 21;
            this.btnSpeedTestDefault.Text = "Revert";
            this.btnSpeedTestDefault.UseVisualStyleBackColor = true;
            // 
            // txbPitchSpeedTest
            // 
            this.txbPitchSpeedTest.Location = new System.Drawing.Point(142, 154);
            this.txbPitchSpeedTest.Name = "txbPitchSpeedTest";
            this.txbPitchSpeedTest.Size = new System.Drawing.Size(65, 20);
            this.txbPitchSpeedTest.TabIndex = 20;
            // 
            // txbRollSpeedTest
            // 
            this.txbRollSpeedTest.Location = new System.Drawing.Point(220, 154);
            this.txbRollSpeedTest.Name = "txbRollSpeedTest";
            this.txbRollSpeedTest.Size = new System.Drawing.Size(65, 20);
            this.txbRollSpeedTest.TabIndex = 18;
            // 
            // btnSpeedTest
            // 
            this.btnSpeedTest.Location = new System.Drawing.Point(298, 154);
            this.btnSpeedTest.Name = "btnSpeedTest";
            this.btnSpeedTest.Size = new System.Drawing.Size(60, 23);
            this.btnSpeedTest.TabIndex = 19;
            this.btnSpeedTest.Text = "Test";
            this.btnSpeedTest.UseVisualStyleBackColor = true;
            this.btnSpeedTest.Click += new System.EventHandler(this.btnSpeedTest_Click);
            // 
            // btnTorqueDefault
            // 
            this.btnTorqueDefault.Location = new System.Drawing.Point(371, 114);
            this.btnTorqueDefault.Name = "btnTorqueDefault";
            this.btnTorqueDefault.Size = new System.Drawing.Size(75, 23);
            this.btnTorqueDefault.TabIndex = 17;
            this.btnTorqueDefault.Text = "Revert";
            this.btnTorqueDefault.UseVisualStyleBackColor = true;
            this.btnTorqueDefault.Click += new System.EventHandler(this.btnTorqueDefault_Click);
            // 
            // txbPitchTorque
            // 
            this.txbPitchTorque.Location = new System.Drawing.Point(142, 114);
            this.txbPitchTorque.Name = "txbPitchTorque";
            this.txbPitchTorque.Size = new System.Drawing.Size(65, 20);
            this.txbPitchTorque.TabIndex = 16;
            // 
            // lblTorquePitchFwd
            // 
            this.lblTorquePitchFwd.AutoSize = true;
            this.lblTorquePitchFwd.Location = new System.Drawing.Point(529, 35);
            this.lblTorquePitchFwd.Name = "lblTorquePitchFwd";
            this.lblTorquePitchFwd.Size = new System.Drawing.Size(31, 13);
            this.lblTorquePitchFwd.TabIndex = 18;
            this.lblTorquePitchFwd.Text = "Pitch";
            // 
            // lblTorquePitchBack
            // 
            this.lblTorquePitchBack.AutoSize = true;
            this.lblTorquePitchBack.Location = new System.Drawing.Point(576, 36);
            this.lblTorquePitchBack.Name = "lblTorquePitchBack";
            this.lblTorquePitchBack.Size = new System.Drawing.Size(31, 13);
            this.lblTorquePitchBack.TabIndex = 19;
            this.lblTorquePitchBack.Text = "Pitch";
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
            // valueConvertedDataGridViewTextBoxColumn
            // 
            this.valueConvertedDataGridViewTextBoxColumn.DataPropertyName = "ValueConverted";
            this.valueConvertedDataGridViewTextBoxColumn.HeaderText = "Valu eConverted";
            this.valueConvertedDataGridViewTextBoxColumn.Name = "valueConvertedDataGridViewTextBoxColumn";
            this.valueConvertedDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // descriptionDataGridViewTextBoxColumn
            // 
            this.descriptionDataGridViewTextBoxColumn.DataPropertyName = "Description";
            this.descriptionDataGridViewTextBoxColumn.HeaderText = "Description";
            this.descriptionDataGridViewTextBoxColumn.Name = "descriptionDataGridViewTextBoxColumn";
            this.descriptionDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // dataRefTableItemBindingSource
            // 
            this.dataRefTableItemBindingSource.AllowNew = false;
            this.dataRefTableItemBindingSource.DataSource = typeof(LoadForceSim.DataRefTableItem);
            this.dataRefTableItemBindingSource.Filter = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(776, 492);
            this.Controls.Add(this.lblTorquePitchBack);
            this.Controls.Add(this.lblTorquePitchFwd);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.chkAutoConnect);
            this.Controls.Add(this.connectionStatusLabel);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.hostnameInput);
            this.Name = "Form1";
            this.Text = "Load Form Sim";
            this.Shown += new System.EventHandler(this.Form1_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.dataRefView)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabStatus.ResumeLayout(false);
            this.tabTest.ResumeLayout(false);
            this.tabTest.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataRefTableItemBindingSource)).EndInit();
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
        private System.Windows.Forms.TextBox txtbxPitch;
        private System.Windows.Forms.Button btnCenterOut;
        private System.Windows.Forms.TextBox txtbxRoll;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkAutoConnect;
        private System.Windows.Forms.TextBox txbRollTorque;
        private System.Windows.Forms.Button btnTorqueTest;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabStatus;
        private System.Windows.Forms.TabPage tabTest;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn valueDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn valueConvertedDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn descriptionDataGridViewTextBoxColumn;
        private System.Windows.Forms.TextBox txbPitchTorque;
        private System.Windows.Forms.Button btnTorqueDefault;
        private System.Windows.Forms.TabPage tabConfig;
        private System.Windows.Forms.Button btnSpeedTestDefault;
        private System.Windows.Forms.TextBox txbPitchSpeedTest;
        private System.Windows.Forms.TextBox txbRollSpeedTest;
        private System.Windows.Forms.Button btnSpeedTest;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnSaveServoConfigs;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtMinPitchPos;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtMinRoll;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label lblTorquePitchFwd;
        private System.Windows.Forms.Label lblTorquePitchBack;
    }
}

