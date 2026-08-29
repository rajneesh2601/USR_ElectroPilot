namespace USR_ElectroPilot.Forms
{
    partial class LoadHistoryForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel filterPanel;
        private System.Windows.Forms.DataGridView gridLoads;
        private System.Windows.Forms.Label lblLoadNumber;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblFrom;
        private System.Windows.Forms.Label lblTo;
        private System.Windows.Forms.TextBox txtLoadNumber;
        private System.Windows.Forms.ComboBox cboStatus;
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
            this.cboStatus = new System.Windows.Forms.ComboBox();
            this.txtLoadNumber = new System.Windows.Forms.TextBox();
            this.lblTo = new System.Windows.Forms.Label();
            this.lblFrom = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblLoadNumber = new System.Windows.Forms.Label();
            this.gridLoads = new System.Windows.Forms.DataGridView();
            this.filterPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridLoads)).BeginInit();
            this.SuspendLayout();
            // 
            // filterPanel
            // 
            this.filterPanel.Controls.Add(this.btnRefresh);
            this.filterPanel.Controls.Add(this.btnApply);
            this.filterPanel.Controls.Add(this.dtTo);
            this.filterPanel.Controls.Add(this.dtFrom);
            this.filterPanel.Controls.Add(this.cboStatus);
            this.filterPanel.Controls.Add(this.txtLoadNumber);
            this.filterPanel.Controls.Add(this.lblTo);
            this.filterPanel.Controls.Add(this.lblFrom);
            this.filterPanel.Controls.Add(this.lblStatus);
            this.filterPanel.Controls.Add(this.lblLoadNumber);
            this.filterPanel.AutoScroll = true;
            this.filterPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.filterPanel.Location = new System.Drawing.Point(0, 0);
            this.filterPanel.Name = "filterPanel";
            this.filterPanel.Padding = new System.Windows.Forms.Padding(12);
            this.filterPanel.Size = new System.Drawing.Size(1044, 86);
            this.filterPanel.TabIndex = 0;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(763, 39);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(90, 28);
            this.btnRefresh.TabIndex = 9;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(667, 39);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(90, 28);
            this.btnApply.TabIndex = 8;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.BtnApply_Click);
            // 
            // dtTo
            // 
            this.dtTo.CustomFormat = "yyyy-MM-dd HH:mm";
            this.dtTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtTo.Location = new System.Drawing.Point(487, 43);
            this.dtTo.Name = "dtTo";
            this.dtTo.Size = new System.Drawing.Size(160, 20);
            this.dtTo.TabIndex = 7;
            // 
            // dtFrom
            // 
            this.dtFrom.CustomFormat = "yyyy-MM-dd HH:mm";
            this.dtFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtFrom.Location = new System.Drawing.Point(311, 43);
            this.dtFrom.Name = "dtFrom";
            this.dtFrom.Size = new System.Drawing.Size(160, 20);
            this.dtFrom.TabIndex = 5;
            // 
            // cboStatus
            // 
            this.cboStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStatus.Location = new System.Drawing.Point(167, 43);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Size = new System.Drawing.Size(120, 21);
            this.cboStatus.TabIndex = 3;
            // 
            // txtLoadNumber
            // 
            this.txtLoadNumber.Location = new System.Drawing.Point(16, 43);
            this.txtLoadNumber.Name = "txtLoadNumber";
            this.txtLoadNumber.Size = new System.Drawing.Size(130, 20);
            this.txtLoadNumber.TabIndex = 1;
            // 
            // labels
            // 
            this.lblLoadNumber.AutoSize = true;
            this.lblLoadNumber.Location = new System.Drawing.Point(16, 23);
            this.lblLoadNumber.Name = "lblLoadNumber";
            this.lblLoadNumber.Size = new System.Drawing.Size(70, 13);
            this.lblLoadNumber.TabIndex = 0;
            this.lblLoadNumber.Text = "Load number";
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(167, 23);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(37, 13);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "Status";
            this.lblFrom.AutoSize = true;
            this.lblFrom.Location = new System.Drawing.Point(311, 23);
            this.lblFrom.Name = "lblFrom";
            this.lblFrom.Size = new System.Drawing.Size(30, 13);
            this.lblFrom.TabIndex = 4;
            this.lblFrom.Text = "From";
            this.lblTo.AutoSize = true;
            this.lblTo.Location = new System.Drawing.Point(487, 23);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(20, 13);
            this.lblTo.TabIndex = 6;
            this.lblTo.Text = "To";
            // 
            // gridLoads
            // 
            this.gridLoads.AllowUserToAddRows = false;
            this.gridLoads.AllowUserToDeleteRows = false;
            this.gridLoads.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridLoads.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridLoads.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridLoads.Location = new System.Drawing.Point(0, 86);
            this.gridLoads.Name = "gridLoads";
            this.gridLoads.ReadOnly = true;
            this.gridLoads.RowHeadersVisible = false;
            this.gridLoads.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridLoads.Size = new System.Drawing.Size(1044, 475);
            this.gridLoads.TabIndex = 1;
            // 
            // LoadHistoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1044, 561);
            this.Controls.Add(this.gridLoads);
            this.Controls.Add(this.filterPanel);
            this.MinimumSize = new System.Drawing.Size(900, 500);
            this.Name = "LoadHistoryForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Load History";
            this.Load += new System.EventHandler(this.LoadHistoryForm_Load);
            this.filterPanel.ResumeLayout(false);
            this.filterPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridLoads)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
