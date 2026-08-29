namespace USR_ElectroPilot.Forms
{
    partial class TankEditForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblLineId;
        private System.Windows.Forms.Label lblTankNumber;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblChemical;
        private System.Windows.Forms.Label lblCapacity;
        private System.Windows.Forms.Label lblLevel;
        private System.Windows.Forms.Label lblTemperature;
        private System.Windows.Forms.Label lblVoltage;
        private System.Windows.Forms.Label lblCurrent;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.NumericUpDown numLineId;
        private System.Windows.Forms.NumericUpDown numTankNumber;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtChemical;
        private System.Windows.Forms.NumericUpDown numCapacity;
        private System.Windows.Forms.NumericUpDown numLevel;
        private System.Windows.Forms.NumericUpDown numTemperature;
        private System.Windows.Forms.NumericUpDown numVoltage;
        private System.Windows.Forms.NumericUpDown numCurrent;
        private System.Windows.Forms.ComboBox cboStatus;
        private System.Windows.Forms.CheckBox chkActive;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblLineId = new System.Windows.Forms.Label();
            this.lblTankNumber = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.lblChemical = new System.Windows.Forms.Label();
            this.lblCapacity = new System.Windows.Forms.Label();
            this.lblLevel = new System.Windows.Forms.Label();
            this.lblTemperature = new System.Windows.Forms.Label();
            this.lblVoltage = new System.Windows.Forms.Label();
            this.lblCurrent = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.numLineId = new System.Windows.Forms.NumericUpDown();
            this.numTankNumber = new System.Windows.Forms.NumericUpDown();
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtChemical = new System.Windows.Forms.TextBox();
            this.numCapacity = new System.Windows.Forms.NumericUpDown();
            this.numLevel = new System.Windows.Forms.NumericUpDown();
            this.numTemperature = new System.Windows.Forms.NumericUpDown();
            this.numVoltage = new System.Windows.Forms.NumericUpDown();
            this.numCurrent = new System.Windows.Forms.NumericUpDown();
            this.cboStatus = new System.Windows.Forms.ComboBox();
            this.chkActive = new System.Windows.Forms.CheckBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numLineId)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTankNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCapacity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTemperature)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numVoltage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCurrent)).BeginInit();
            this.SuspendLayout();
            // 
            // labels
            // 
            this.lblLineId.Location = new System.Drawing.Point(24, 24);
            this.lblLineId.Text = "Line";
            this.lblLineId.Visible = false;
            this.lblTankNumber.Location = new System.Drawing.Point(24, 64);
            this.lblTankNumber.Text = "Tank #";
            this.lblName.Location = new System.Drawing.Point(24, 104);
            this.lblName.Text = "Name";
            this.lblChemical.Location = new System.Drawing.Point(24, 144);
            this.lblChemical.Text = "Chemical";
            this.lblCapacity.Location = new System.Drawing.Point(24, 184);
            this.lblCapacity.Text = "Capacity L";
            this.lblLevel.Location = new System.Drawing.Point(24, 224);
            this.lblLevel.Text = "Level L";
            this.lblTemperature.Location = new System.Drawing.Point(24, 264);
            this.lblTemperature.Text = "Temp C";
            this.lblVoltage.Location = new System.Drawing.Point(24, 304);
            this.lblVoltage.Text = "Voltage";
            this.lblCurrent.Location = new System.Drawing.Point(24, 344);
            this.lblCurrent.Text = "Current";
            this.lblStatus.Location = new System.Drawing.Point(24, 384);
            this.lblStatus.Text = "Status";
            // 
            // inputs
            // 
            this.numLineId.Location = new System.Drawing.Point(128, 22);
            this.numLineId.Maximum = new decimal(new int[] { 99, 0, 0, 0 });
            this.numLineId.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numLineId.Value = new decimal(new int[] { 1, 0, 0, 0 });
            this.numLineId.Visible = false;
            this.numTankNumber.Location = new System.Drawing.Point(128, 62);
            this.numTankNumber.Maximum = new decimal(new int[] { 999, 0, 0, 0 });
            this.numTankNumber.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numTankNumber.Value = new decimal(new int[] { 1, 0, 0, 0 });
            this.txtName.Location = new System.Drawing.Point(128, 102);
            this.txtName.Size = new System.Drawing.Size(220, 20);
            this.txtChemical.Location = new System.Drawing.Point(128, 142);
            this.txtChemical.Size = new System.Drawing.Size(220, 20);
            this.numCapacity.Location = new System.Drawing.Point(128, 182);
            this.numCapacity.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            this.numLevel.Location = new System.Drawing.Point(128, 222);
            this.numLevel.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            this.numTemperature.DecimalPlaces = 1;
            this.numTemperature.Location = new System.Drawing.Point(128, 262);
            this.numTemperature.Maximum = new decimal(new int[] { 200, 0, 0, 0 });
            this.numVoltage.DecimalPlaces = 1;
            this.numVoltage.Location = new System.Drawing.Point(128, 302);
            this.numVoltage.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            this.numCurrent.DecimalPlaces = 1;
            this.numCurrent.Location = new System.Drawing.Point(128, 342);
            this.numCurrent.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            this.cboStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStatus.Items.AddRange(new object[] { "Normal", "Warning", "Fault" });
            this.cboStatus.Location = new System.Drawing.Point(128, 382);
            this.cboStatus.Size = new System.Drawing.Size(220, 21);
            this.chkActive.Location = new System.Drawing.Point(128, 416);
            this.chkActive.Text = "Active";
            // 
            // buttons
            // 
            this.btnOk.Location = new System.Drawing.Point(176, 456);
            this.btnOk.Size = new System.Drawing.Size(82, 30);
            this.btnOk.Text = "OK";
            this.btnOk.Click += new System.EventHandler(this.BtnOk_Click);
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(266, 456);
            this.btnCancel.Size = new System.Drawing.Size(82, 30);
            this.btnCancel.Text = "Cancel";
            // 
            // TankEditForm
            // 
            this.AcceptButton = this.btnOk;
            this.CancelButton = this.btnCancel;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 511);
            this.Controls.Add(this.lblLineId);
            this.Controls.Add(this.lblTankNumber);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.lblChemical);
            this.Controls.Add(this.lblCapacity);
            this.Controls.Add(this.lblLevel);
            this.Controls.Add(this.lblTemperature);
            this.Controls.Add(this.lblVoltage);
            this.Controls.Add(this.lblCurrent);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.numLineId);
            this.Controls.Add(this.numTankNumber);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.txtChemical);
            this.Controls.Add(this.numCapacity);
            this.Controls.Add(this.numLevel);
            this.Controls.Add(this.numTemperature);
            this.Controls.Add(this.numVoltage);
            this.Controls.Add(this.numCurrent);
            this.Controls.Add(this.cboStatus);
            this.Controls.Add(this.chkActive);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TankEditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tank";
            this.Load += new System.EventHandler(this.TankEditForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numLineId)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTankNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCapacity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTemperature)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numVoltage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCurrent)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
