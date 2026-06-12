namespace USR_ElectroPilot.Forms.Admin
{
    partial class AuditLogForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel filterPanel;
        private System.Windows.Forms.DataGridView gridAudit;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.Label lblAction;
        private System.Windows.Forms.Label lblEntity;
        private System.Windows.Forms.Label lblFrom;
        private System.Windows.Forms.Label lblTo;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TextBox txtAction;
        private System.Windows.Forms.TextBox txtEntity;
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
            this.txtEntity = new System.Windows.Forms.TextBox();
            this.txtAction = new System.Windows.Forms.TextBox();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblTo = new System.Windows.Forms.Label();
            this.lblFrom = new System.Windows.Forms.Label();
            this.lblEntity = new System.Windows.Forms.Label();
            this.lblAction = new System.Windows.Forms.Label();
            this.lblUsername = new System.Windows.Forms.Label();
            this.gridAudit = new System.Windows.Forms.DataGridView();
            this.filterPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridAudit)).BeginInit();
            this.SuspendLayout();
            // 
            // filterPanel
            // 
            this.filterPanel.Controls.Add(this.btnRefresh);
            this.filterPanel.Controls.Add(this.btnApply);
            this.filterPanel.Controls.Add(this.dtTo);
            this.filterPanel.Controls.Add(this.dtFrom);
            this.filterPanel.Controls.Add(this.txtEntity);
            this.filterPanel.Controls.Add(this.txtAction);
            this.filterPanel.Controls.Add(this.txtUsername);
            this.filterPanel.Controls.Add(this.lblTo);
            this.filterPanel.Controls.Add(this.lblFrom);
            this.filterPanel.Controls.Add(this.lblEntity);
            this.filterPanel.Controls.Add(this.lblAction);
            this.filterPanel.Controls.Add(this.lblUsername);
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
            this.txtEntity.Location = new System.Drawing.Point(319, 43);
            this.txtEntity.Name = "txtEntity";
            this.txtEntity.Size = new System.Drawing.Size(130, 20);
            this.txtEntity.TabIndex = 5;
            this.txtAction.Location = new System.Drawing.Point(167, 43);
            this.txtAction.Name = "txtAction";
            this.txtAction.Size = new System.Drawing.Size(130, 20);
            this.txtAction.TabIndex = 3;
            this.txtUsername.Location = new System.Drawing.Point(16, 43);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(130, 20);
            this.txtUsername.TabIndex = 1;
            // 
            // labels
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new System.Drawing.Point(16, 23);
            this.lblUsername.Text = "Username";
            this.lblAction.AutoSize = true;
            this.lblAction.Location = new System.Drawing.Point(167, 23);
            this.lblAction.Text = "Action";
            this.lblEntity.AutoSize = true;
            this.lblEntity.Location = new System.Drawing.Point(319, 23);
            this.lblEntity.Text = "Entity";
            this.lblFrom.AutoSize = true;
            this.lblFrom.Location = new System.Drawing.Point(471, 23);
            this.lblFrom.Text = "From";
            this.lblTo.AutoSize = true;
            this.lblTo.Location = new System.Drawing.Point(647, 23);
            this.lblTo.Text = "To";
            // 
            // gridAudit
            // 
            this.gridAudit.AllowUserToAddRows = false;
            this.gridAudit.AllowUserToDeleteRows = false;
            this.gridAudit.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridAudit.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridAudit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridAudit.Location = new System.Drawing.Point(0, 86);
            this.gridAudit.Name = "gridAudit";
            this.gridAudit.ReadOnly = true;
            this.gridAudit.RowHeadersVisible = false;
            this.gridAudit.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridAudit.Size = new System.Drawing.Size(1044, 475);
            this.gridAudit.TabIndex = 1;
            // 
            // AuditLogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1044, 561);
            this.Controls.Add(this.gridAudit);
            this.Controls.Add(this.filterPanel);
            this.MinimumSize = new System.Drawing.Size(900, 500);
            this.Name = "AuditLogForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Audit Log";
            this.Load += new System.EventHandler(this.AuditLogForm_Load);
            this.filterPanel.ResumeLayout(false);
            this.filterPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridAudit)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
