namespace USR_ElectroPilot.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton btnRefresh;
        private System.Windows.Forms.ToolStripButton btnLogout;
        private System.Windows.Forms.ToolStripLabel lblUser;
        private System.Windows.Forms.FlowLayoutPanel pnlStatus;
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage tabTanks;
        private System.Windows.Forms.TabPage tabWagons;
        private System.Windows.Forms.TabPage tabRectifiers;
        private System.Windows.Forms.TabPage tabAlarms;
        private System.Windows.Forms.FlowLayoutPanel pnlTanks;
        private System.Windows.Forms.FlowLayoutPanel pnlWagons;
        private System.Windows.Forms.FlowLayoutPanel pnlRectifiers;
        private System.Windows.Forms.DataGridView alarmGrid;
        private System.Windows.Forms.Timer simulatorTimer;

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
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btnRefresh = new System.Windows.Forms.ToolStripButton();
            this.btnLogout = new System.Windows.Forms.ToolStripButton();
            this.lblUser = new System.Windows.Forms.ToolStripLabel();
            this.pnlStatus = new System.Windows.Forms.FlowLayoutPanel();
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabTanks = new System.Windows.Forms.TabPage();
            this.pnlTanks = new System.Windows.Forms.FlowLayoutPanel();
            this.tabWagons = new System.Windows.Forms.TabPage();
            this.pnlWagons = new System.Windows.Forms.FlowLayoutPanel();
            this.tabRectifiers = new System.Windows.Forms.TabPage();
            this.pnlRectifiers = new System.Windows.Forms.FlowLayoutPanel();
            this.tabAlarms = new System.Windows.Forms.TabPage();
            this.alarmGrid = new System.Windows.Forms.DataGridView();
            this.simulatorTimer = new System.Windows.Forms.Timer(this.components);
            this.toolStrip.SuspendLayout();
            this.tabs.SuspendLayout();
            this.tabTanks.SuspendLayout();
            this.tabWagons.SuspendLayout();
            this.tabRectifiers.SuspendLayout();
            this.tabAlarms.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.alarmGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnRefresh,
            this.btnLogout,
            this.lblUser});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(1104, 25);
            this.toolStrip.TabIndex = 0;
            // 
            // btnRefresh
            // 
            this.btnRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(50, 22);
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
            // 
            // btnLogout
            // 
            this.btnLogout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(49, 22);
            this.btnLogout.Text = "Logout";
            this.btnLogout.Click += new System.EventHandler(this.BtnLogout_Click);
            // 
            // lblUser
            // 
            this.lblUser.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(29, 22);
            this.lblUser.Text = "User";
            // 
            // pnlStatus
            // 
            this.pnlStatus.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlStatus.Location = new System.Drawing.Point(0, 25);
            this.pnlStatus.Name = "pnlStatus";
            this.pnlStatus.Padding = new System.Windows.Forms.Padding(8);
            this.pnlStatus.Size = new System.Drawing.Size(1104, 118);
            this.pnlStatus.TabIndex = 1;
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.tabTanks);
            this.tabs.Controls.Add(this.tabWagons);
            this.tabs.Controls.Add(this.tabRectifiers);
            this.tabs.Controls.Add(this.tabAlarms);
            this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabs.Location = new System.Drawing.Point(0, 143);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(1104, 538);
            this.tabs.TabIndex = 2;
            // 
            // tabTanks
            // 
            this.tabTanks.Controls.Add(this.pnlTanks);
            this.tabTanks.Location = new System.Drawing.Point(4, 22);
            this.tabTanks.Name = "tabTanks";
            this.tabTanks.Padding = new System.Windows.Forms.Padding(3);
            this.tabTanks.Size = new System.Drawing.Size(1096, 512);
            this.tabTanks.TabIndex = 0;
            this.tabTanks.Text = "Tanks";
            this.tabTanks.UseVisualStyleBackColor = true;
            // 
            // pnlTanks
            // 
            this.pnlTanks.AutoScroll = true;
            this.pnlTanks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTanks.Location = new System.Drawing.Point(3, 3);
            this.pnlTanks.Name = "pnlTanks";
            this.pnlTanks.Padding = new System.Windows.Forms.Padding(8);
            this.pnlTanks.Size = new System.Drawing.Size(1090, 506);
            this.pnlTanks.TabIndex = 0;
            // 
            // tabWagons
            // 
            this.tabWagons.Controls.Add(this.pnlWagons);
            this.tabWagons.Location = new System.Drawing.Point(4, 22);
            this.tabWagons.Name = "tabWagons";
            this.tabWagons.Padding = new System.Windows.Forms.Padding(3);
            this.tabWagons.Size = new System.Drawing.Size(1096, 512);
            this.tabWagons.TabIndex = 1;
            this.tabWagons.Text = "Wagons";
            this.tabWagons.UseVisualStyleBackColor = true;
            // 
            // pnlWagons
            // 
            this.pnlWagons.AutoScroll = true;
            this.pnlWagons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlWagons.Location = new System.Drawing.Point(3, 3);
            this.pnlWagons.Name = "pnlWagons";
            this.pnlWagons.Padding = new System.Windows.Forms.Padding(8);
            this.pnlWagons.Size = new System.Drawing.Size(1090, 506);
            this.pnlWagons.TabIndex = 0;
            // 
            // tabRectifiers
            // 
            this.tabRectifiers.Controls.Add(this.pnlRectifiers);
            this.tabRectifiers.Location = new System.Drawing.Point(4, 22);
            this.tabRectifiers.Name = "tabRectifiers";
            this.tabRectifiers.Size = new System.Drawing.Size(1096, 512);
            this.tabRectifiers.TabIndex = 2;
            this.tabRectifiers.Text = "Rectifiers";
            this.tabRectifiers.UseVisualStyleBackColor = true;
            // 
            // pnlRectifiers
            // 
            this.pnlRectifiers.AutoScroll = true;
            this.pnlRectifiers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRectifiers.Location = new System.Drawing.Point(0, 0);
            this.pnlRectifiers.Name = "pnlRectifiers";
            this.pnlRectifiers.Padding = new System.Windows.Forms.Padding(8);
            this.pnlRectifiers.Size = new System.Drawing.Size(1096, 512);
            this.pnlRectifiers.TabIndex = 0;
            // 
            // tabAlarms
            // 
            this.tabAlarms.Controls.Add(this.alarmGrid);
            this.tabAlarms.Location = new System.Drawing.Point(4, 22);
            this.tabAlarms.Name = "tabAlarms";
            this.tabAlarms.Size = new System.Drawing.Size(1096, 512);
            this.tabAlarms.TabIndex = 3;
            this.tabAlarms.Text = "Alarms";
            this.tabAlarms.UseVisualStyleBackColor = true;
            // 
            // alarmGrid
            // 
            this.alarmGrid.AllowUserToAddRows = false;
            this.alarmGrid.AllowUserToDeleteRows = false;
            this.alarmGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.alarmGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.alarmGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alarmGrid.Location = new System.Drawing.Point(0, 0);
            this.alarmGrid.Name = "alarmGrid";
            this.alarmGrid.ReadOnly = true;
            this.alarmGrid.Size = new System.Drawing.Size(1096, 512);
            this.alarmGrid.TabIndex = 0;
            // 
            // simulatorTimer
            // 
            this.simulatorTimer.Interval = 1000;
            this.simulatorTimer.Tick += new System.EventHandler(this.SimulatorTimer_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1104, 681);
            this.Controls.Add(this.tabs);
            this.Controls.Add(this.pnlStatus);
            this.Controls.Add(this.toolStrip);
            this.MinimumSize = new System.Drawing.Size(960, 600);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "USR ElectroPilot";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.tabs.ResumeLayout(false);
            this.tabTanks.ResumeLayout(false);
            this.tabWagons.ResumeLayout(false);
            this.tabRectifiers.ResumeLayout(false);
            this.tabAlarms.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.alarmGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
