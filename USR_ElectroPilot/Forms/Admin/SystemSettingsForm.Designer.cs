namespace USR_ElectroPilot.Forms.Admin
{
    partial class SystemSettingsForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView gridSettings;
        private System.Windows.Forms.Panel editPanel;
        private System.Windows.Forms.Label lblKey;
        private System.Windows.Forms.Label lblValue;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.TextBox txtKey;
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnSave;
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
            this.gridSettings = new System.Windows.Forms.DataGridView();
            this.editPanel = new System.Windows.Forms.Panel();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.txtKey = new System.Windows.Forms.TextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblValue = new System.Windows.Forms.Label();
            this.lblKey = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.gridSettings)).BeginInit();
            this.editPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridSettings
            // 
            this.gridSettings.AllowUserToAddRows = false;
            this.gridSettings.AllowUserToDeleteRows = false;
            this.gridSettings.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridSettings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridSettings.Location = new System.Drawing.Point(0, 0);
            this.gridSettings.MultiSelect = false;
            this.gridSettings.Name = "gridSettings";
            this.gridSettings.ReadOnly = true;
            this.gridSettings.RowHeadersVisible = false;
            this.gridSettings.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridSettings.Size = new System.Drawing.Size(584, 421);
            this.gridSettings.TabIndex = 0;
            this.gridSettings.SelectionChanged += new System.EventHandler(this.GridSettings_SelectionChanged);
            // 
            // editPanel
            // 
            this.editPanel.Controls.Add(this.btnRefresh);
            this.editPanel.Controls.Add(this.btnSave);
            this.editPanel.Controls.Add(this.btnNew);
            this.editPanel.Controls.Add(this.txtDescription);
            this.editPanel.Controls.Add(this.txtValue);
            this.editPanel.Controls.Add(this.txtKey);
            this.editPanel.Controls.Add(this.lblDescription);
            this.editPanel.Controls.Add(this.lblValue);
            this.editPanel.Controls.Add(this.lblKey);
            this.editPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.editPanel.Location = new System.Drawing.Point(584, 0);
            this.editPanel.Name = "editPanel";
            this.editPanel.Padding = new System.Windows.Forms.Padding(16);
            this.editPanel.Size = new System.Drawing.Size(300, 421);
            this.editPanel.TabIndex = 1;
            // 
            // buttons
            // 
            this.btnNew.Location = new System.Drawing.Point(20, 292);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(60, 30);
            this.btnNew.TabIndex = 6;
            this.btnNew.Text = "New";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.BtnNew_Click);
            this.btnSave.Location = new System.Drawing.Point(86, 292);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(88, 30);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            this.btnRefresh.Location = new System.Drawing.Point(180, 292);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(88, 30);
            this.btnRefresh.TabIndex = 8;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
            // 
            // inputs
            // 
            this.txtKey.Location = new System.Drawing.Point(20, 51);
            this.txtKey.Name = "txtKey";
            this.txtKey.Size = new System.Drawing.Size(248, 20);
            this.txtKey.TabIndex = 1;
            this.txtValue.Location = new System.Drawing.Point(20, 111);
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(248, 20);
            this.txtValue.TabIndex = 3;
            this.txtDescription.Location = new System.Drawing.Point(20, 171);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(248, 90);
            this.txtDescription.TabIndex = 5;
            // 
            // labels
            // 
            this.lblKey.AutoSize = true;
            this.lblKey.Location = new System.Drawing.Point(20, 32);
            this.lblKey.Text = "Key";
            this.lblValue.AutoSize = true;
            this.lblValue.Location = new System.Drawing.Point(20, 92);
            this.lblValue.Text = "Value";
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(20, 152);
            this.lblDescription.Text = "Description";
            // 
            // SystemSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 421);
            this.Controls.Add(this.gridSettings);
            this.Controls.Add(this.editPanel);
            this.MinimumSize = new System.Drawing.Size(820, 420);
            this.Name = "SystemSettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "System Settings";
            this.Load += new System.EventHandler(this.SystemSettingsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridSettings)).EndInit();
            this.editPanel.ResumeLayout(false);
            this.editPanel.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
