namespace USR_ElectroPilot.Forms.Admin
{
    partial class AdminPanelForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage tabUsers;
        private System.Windows.Forms.TabPage tabActivity;
        private System.Windows.Forms.TabPage tabAudit;
        private System.Windows.Forms.TabPage tabSettings;
        private System.Windows.Forms.TabPage tabAlarms;
        private System.Windows.Forms.TabPage tabReports;

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
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabUsers = new System.Windows.Forms.TabPage();
            this.tabActivity = new System.Windows.Forms.TabPage();
            this.tabAudit = new System.Windows.Forms.TabPage();
            this.tabSettings = new System.Windows.Forms.TabPage();
            this.tabAlarms = new System.Windows.Forms.TabPage();
            this.tabReports = new System.Windows.Forms.TabPage();
            this.tabs.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.tabUsers);
            this.tabs.Controls.Add(this.tabActivity);
            this.tabs.Controls.Add(this.tabAudit);
            this.tabs.Controls.Add(this.tabSettings);
            this.tabs.Controls.Add(this.tabAlarms);
            this.tabs.Controls.Add(this.tabReports);
            this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabs.Location = new System.Drawing.Point(0, 0);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(884, 561);
            this.tabs.TabIndex = 0;
            // 
            // tabUsers
            // 
            this.tabUsers.Location = new System.Drawing.Point(4, 22);
            this.tabUsers.Name = "tabUsers";
            this.tabUsers.Padding = new System.Windows.Forms.Padding(12);
            this.tabUsers.Size = new System.Drawing.Size(876, 535);
            this.tabUsers.TabIndex = 0;
            this.tabUsers.Text = "Users";
            this.tabUsers.UseVisualStyleBackColor = true;
            // 
            // tabActivity
            // 
            this.tabActivity.Location = new System.Drawing.Point(4, 22);
            this.tabActivity.Name = "tabActivity";
            this.tabActivity.Padding = new System.Windows.Forms.Padding(12);
            this.tabActivity.Size = new System.Drawing.Size(876, 535);
            this.tabActivity.TabIndex = 1;
            this.tabActivity.Text = "Activity";
            this.tabActivity.UseVisualStyleBackColor = true;
            // 
            // tabAudit
            // 
            this.tabAudit.Location = new System.Drawing.Point(4, 22);
            this.tabAudit.Name = "tabAudit";
            this.tabAudit.Padding = new System.Windows.Forms.Padding(12);
            this.tabAudit.Size = new System.Drawing.Size(876, 535);
            this.tabAudit.TabIndex = 2;
            this.tabAudit.Text = "Audit";
            this.tabAudit.UseVisualStyleBackColor = true;
            // 
            // tabSettings
            // 
            this.tabSettings.Location = new System.Drawing.Point(4, 22);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.Padding = new System.Windows.Forms.Padding(12);
            this.tabSettings.Size = new System.Drawing.Size(876, 535);
            this.tabSettings.TabIndex = 3;
            this.tabSettings.Text = "Settings";
            this.tabSettings.UseVisualStyleBackColor = true;
            // 
            // tabAlarms
            // 
            this.tabAlarms.Location = new System.Drawing.Point(4, 22);
            this.tabAlarms.Name = "tabAlarms";
            this.tabAlarms.Padding = new System.Windows.Forms.Padding(12);
            this.tabAlarms.Size = new System.Drawing.Size(876, 535);
            this.tabAlarms.TabIndex = 4;
            this.tabAlarms.Text = "Alarms";
            this.tabAlarms.UseVisualStyleBackColor = true;
            // 
            // tabReports
            // 
            this.tabReports.Location = new System.Drawing.Point(4, 22);
            this.tabReports.Name = "tabReports";
            this.tabReports.Padding = new System.Windows.Forms.Padding(12);
            this.tabReports.Size = new System.Drawing.Size(876, 535);
            this.tabReports.TabIndex = 5;
            this.tabReports.Text = "Reports";
            this.tabReports.UseVisualStyleBackColor = true;
            // 
            // AdminPanelForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 561);
            this.Controls.Add(this.tabs);
            this.MinimumSize = new System.Drawing.Size(760, 480);
            this.Name = "AdminPanelForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Admin Panel";
            this.Load += new System.EventHandler(this.AdminPanelForm_Load);
            this.tabs.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
