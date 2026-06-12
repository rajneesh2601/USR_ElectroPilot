namespace USR_ElectroPilot.Forms
{
    partial class AlarmHistoryForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel filterPanel;
        private System.Windows.Forms.DataGridView gridAlarms;
        private System.Windows.Forms.Label lblSource;
        private System.Windows.Forms.Label lblSeverity;
        private System.Windows.Forms.Label lblState;
        private System.Windows.Forms.Label lblFrom;
        private System.Windows.Forms.Label lblTo;
        private System.Windows.Forms.TextBox txtSource;
        private System.Windows.Forms.ComboBox cboSeverity;
        private System.Windows.Forms.ComboBox cboState;
        private System.Windows.Forms.DateTimePicker dtFrom;
        private System.Windows.Forms.DateTimePicker dtTo;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnRefresh;

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
            this.filterPanel = new System.Windows.Forms.Panel();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.dtTo = new System.Windows.Forms.DateTimePicker();
            this.dtFrom = new System.Windows.Forms.DateTimePicker();
            this.cboState = new System.Windows.Forms.ComboBox();
            this.cboSeverity = new System.Windows.Forms.ComboBox();
            this.txtSource = new System.Windows.Forms.TextBox();
            this.lblTo = new System.Windows.Forms.Label();
            this.lblFrom = new System.Windows.Forms.Label();
            this.lblState = new System.Windows.Forms.Label();
            this.lblSeverity = new System.Windows.Forms.Label();
            this.lblSource = new System.Windows.Forms.Label();
            this.gridAlarms = new System.Windows.Forms.DataGridView();
            this.filterPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridAlarms)).BeginInit();
            this.SuspendLayout();
            // 
            // filterPanel
            // 
            this.filterPanel.Controls.Add(this.btnRefresh);
            this.filterPanel.Controls.Add(this.btnApply);
            this.filterPanel.Controls.Add(this.dtTo);
            this.filterPanel.Controls.Add(this.dtFrom);
            this.filterPanel.Controls.Add(this.cboState);
            this.filterPanel.Controls.Add(this.cboSeverity);
            this.filterPanel.Controls.Add(this.txtSource);
            this.filterPanel.Controls.Add(this.lblTo);
            this.filterPanel.Controls.Add(this.lblFrom);
            this.filterPanel.Controls.Add(this.lblState);
            this.filterPanel.Controls.Add(this.lblSeverity);
            this.filterPanel.Controls.Add(this.lblSource);
            this.filterPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.filterPanel.Location = new System.Drawing.Point(0, 0);
            this.filterPanel.Name = "filterPanel";
            this.filterPanel.Padding = new System.Windows.Forms.Padding(12);
            this.filterPanel.Size = new System.Drawing.Size(1044, 86);
            this.filterPanel.TabIndex = 0;
            // 
            // buttons and inputs
            // 
            this.btnApply.Location = new System.Drawing.Point(827, 39);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(90, 28);
            this.btnApply.TabIndex = 10;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.BtnApply_Click);
            this.btnRefresh.Location = new System.Drawing.Point(923, 39);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(90, 28);
            this.btnRefresh.TabIndex = 11;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
            this.dtTo.CustomFormat = "yyyy-MM-dd HH:mm";
            this.dtTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtTo.Location = new System.Drawing.Point(647, 43);
            this.dtTo.Name = "dtTo";
            this.dtTo.Size = new System.Drawing.Size(160, 20);
            this.dtTo.TabIndex = 9;
            this.dtFrom.CustomFormat = "yyyy-MM-dd HH:mm";
            this.dtFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtFrom.Location = new System.Drawing.Point(471, 43);
            this.dtFrom.Name = "dtFrom";
            this.dtFrom.Size = new System.Drawing.Size(160, 20);
            this.dtFrom.TabIndex = 7;
            this.cboState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboState.Location = new System.Drawing.Point(319, 43);
            this.cboState.Name = "cboState";
            this.cboState.Size = new System.Drawing.Size(130, 21);
            this.cboState.TabIndex = 5;
            this.cboSeverity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSeverity.Location = new System.Drawing.Point(167, 43);
            this.cboSeverity.Name = "cboSeverity";
            this.cboSeverity.Size = new System.Drawing.Size(130, 21);
            this.cboSeverity.TabIndex = 3;
            this.txtSource.Location = new System.Drawing.Point(16, 43);
            this.txtSource.Name = "txtSource";
            this.txtSource.Size = new System.Drawing.Size(130, 20);
            this.txtSource.TabIndex = 1;
            // 
            // labels
            // 
            this.lblSource.AutoSize = true;
            this.lblSource.Location = new System.Drawing.Point(16, 23);
            this.lblSource.Text = "Source";
            this.lblSeverity.AutoSize = true;
            this.lblSeverity.Location = new System.Drawing.Point(167, 23);
            this.lblSeverity.Text = "Severity";
            this.lblState.AutoSize = true;
            this.lblState.Location = new System.Drawing.Point(319, 23);
            this.lblState.Text = "State";
            this.lblFrom.AutoSize = true;
            this.lblFrom.Location = new System.Drawing.Point(471, 23);
            this.lblFrom.Text = "From";
            this.lblTo.AutoSize = true;
            this.lblTo.Location = new System.Drawing.Point(647, 23);
            this.lblTo.Text = "To";
            // 
            // gridAlarms
            // 
            this.gridAlarms.AllowUserToAddRows = false;
            this.gridAlarms.AllowUserToDeleteRows = false;
            this.gridAlarms.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridAlarms.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridAlarms.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridAlarms.Location = new System.Drawing.Point(0, 86);
            this.gridAlarms.Name = "gridAlarms";
            this.gridAlarms.ReadOnly = true;
            this.gridAlarms.RowHeadersVisible = false;
            this.gridAlarms.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridAlarms.Size = new System.Drawing.Size(1044, 475);
            this.gridAlarms.TabIndex = 1;
            // 
            // AlarmHistoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1044, 561);
            this.Controls.Add(this.gridAlarms);
            this.Controls.Add(this.filterPanel);
            this.MinimumSize = new System.Drawing.Size(900, 500);
            this.Name = "AlarmHistoryForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Alarm History";
            this.Load += new System.EventHandler(this.AlarmHistoryForm_Load);
            this.filterPanel.ResumeLayout(false);
            this.filterPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridAlarms)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
