namespace USR_ElectroPilot.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton btnRefresh;
        private System.Windows.Forms.ToolStripButton btnAddTank;
        private System.Windows.Forms.ToolStripButton btnEditTank;
        private System.Windows.Forms.ToolStripButton btnRemoveTank;
        private System.Windows.Forms.ToolStripButton btnAddRow;
        private System.Windows.Forms.ToolStripButton btnAutoMode;
        private System.Windows.Forms.ToolStripButton btnManualMode;
        private System.Windows.Forms.ToolStripButton btnStartCycle;
        private System.Windows.Forms.ToolStripButton btnStopCycle;
        private System.Windows.Forms.ToolStripButton btnEmergencyStop;
        private System.Windows.Forms.ToolStripButton btnReset;
        private System.Windows.Forms.ToolStripButton btnStartAll;
        private System.Windows.Forms.ToolStripButton btnStopAll;
        private System.Windows.Forms.ToolStripButton btnResetAlarms;
        private System.Windows.Forms.ToolStripButton btnLogout;
        private System.Windows.Forms.ToolStripLabel lblUser;
        private System.Windows.Forms.ToolStripLabel lblClock;
        private System.Windows.Forms.ToolStripLabel lblPlantStatus;
        private System.Windows.Forms.FlowLayoutPanel pnlStatus;
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage tabTanks;
        private System.Windows.Forms.TabPage tabWagons;
        private System.Windows.Forms.TabPage tabRectifiers;
        private System.Windows.Forms.TabPage tabAlarms;
        private System.Windows.Forms.Panel pnlTanks;
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
            this.components = new System.ComponentModel.Container();

            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btnRefresh = new System.Windows.Forms.ToolStripButton();
            this.btnAddTank = new System.Windows.Forms.ToolStripButton();
            this.btnEditTank = new System.Windows.Forms.ToolStripButton();
            this.btnRemoveTank = new System.Windows.Forms.ToolStripButton();
            this.btnAddRow = new System.Windows.Forms.ToolStripButton();
            this.btnAutoMode = new System.Windows.Forms.ToolStripButton();
            this.btnManualMode = new System.Windows.Forms.ToolStripButton();
            this.btnStartCycle = new System.Windows.Forms.ToolStripButton();
            this.btnStopCycle = new System.Windows.Forms.ToolStripButton();
            this.btnEmergencyStop = new System.Windows.Forms.ToolStripButton();
            this.btnReset = new System.Windows.Forms.ToolStripButton();
            this.btnStartAll = new System.Windows.Forms.ToolStripButton();
            this.btnStopAll = new System.Windows.Forms.ToolStripButton();
            this.btnResetAlarms = new System.Windows.Forms.ToolStripButton();
            this.btnLogout = new System.Windows.Forms.ToolStripButton();
            this.lblUser = new System.Windows.Forms.ToolStripLabel();
            this.lblClock = new System.Windows.Forms.ToolStripLabel();
            this.lblPlantStatus = new System.Windows.Forms.ToolStripLabel();
            this.pnlStatus = new System.Windows.Forms.FlowLayoutPanel();
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabTanks = new System.Windows.Forms.TabPage();
            this.pnlTanks = new System.Windows.Forms.Panel();
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
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.btnRefresh,
                this.btnAddTank,
                this.btnEditTank,
                this.btnRemoveTank,
                this.btnAddRow,
                this.btnAutoMode,
                this.btnManualMode,
                this.btnStartCycle,
                this.btnStopCycle,
                this.btnEmergencyStop,
                this.btnReset,
                this.btnStartAll,
                this.btnStopAll,
                this.btnResetAlarms,
                this.btnLogout,
                this.lblUser,
                this.lblClock,
                this.lblPlantStatus
            });
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
            // btnAddTank
            //
            this.btnAddTank.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnAddTank.Name = "btnAddTank";
            this.btnAddTank.Size = new System.Drawing.Size(61, 22);
            this.btnAddTank.Text = "Add Tank";
            this.btnAddTank.Click += new System.EventHandler(this.BtnAddTank_Click);

            //
            // btnEditTank
            //
            this.btnEditTank.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnEditTank.Name = "btnEditTank";
            this.btnEditTank.Size = new System.Drawing.Size(59, 22);
            this.btnEditTank.Text = "Edit Tank";
            this.btnEditTank.Click += new System.EventHandler(this.BtnEditTank_Click);

            //
            // btnRemoveTank
            //
            this.btnRemoveTank.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnRemoveTank.Name = "btnRemoveTank";
            this.btnRemoveTank.Size = new System.Drawing.Size(82, 22);
            this.btnRemoveTank.Text = "Remove Tank";
            this.btnRemoveTank.Click += new System.EventHandler(this.BtnRemoveTank_Click);

            //
            // btnAddRow
            //
            this.btnAddRow.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnAddRow.Name = "btnAddRow";
            this.btnAddRow.Size = new System.Drawing.Size(58, 22);
            this.btnAddRow.Text = "Add Row";
            this.btnAddRow.Click += new System.EventHandler(this.BtnAddRow_Click);

            //
            // btnAutoMode
            //
            this.btnAutoMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnAutoMode.Name = "btnAutoMode";
            this.btnAutoMode.Size = new System.Drawing.Size(67, 22);
            this.btnAutoMode.Text = "Auto Mode";
            this.btnAutoMode.Click += new System.EventHandler(this.BtnAutoMode_Click);

            //
            // btnManualMode
            //
            this.btnManualMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnManualMode.Name = "btnManualMode";
            this.btnManualMode.Size = new System.Drawing.Size(81, 22);
            this.btnManualMode.Text = "Manual Mode";
            this.btnManualMode.Click += new System.EventHandler(this.BtnManualMode_Click);

            //
            // btnStartCycle
            //
            this.btnStartCycle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnStartCycle.Name = "btnStartCycle";
            this.btnStartCycle.Size = new System.Drawing.Size(68, 22);
            this.btnStartCycle.Text = "Start Cycle";
            this.btnStartCycle.Click += new System.EventHandler(this.BtnStartCycle_Click);

            //
            // btnStopCycle
            //
            this.btnStopCycle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnStopCycle.Name = "btnStopCycle";
            this.btnStopCycle.Size = new System.Drawing.Size(66, 22);
            this.btnStopCycle.Text = "Stop Cycle";
            this.btnStopCycle.Click += new System.EventHandler(this.BtnStopCycle_Click);

            //
            // btnEmergencyStop
            //
            this.btnEmergencyStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnEmergencyStop.Name = "btnEmergencyStop";
            this.btnEmergencyStop.Size = new System.Drawing.Size(92, 22);
            this.btnEmergencyStop.Text = "Emergency Stop";
            this.btnEmergencyStop.Click += new System.EventHandler(this.BtnEmergencyStop_Click);

            //
            // btnReset
            //
            this.btnReset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(39, 22);
            this.btnReset.Text = "Reset";
            this.btnReset.Click += new System.EventHandler(this.BtnReset_Click);

            //
            // btnStartAll
            //
            this.btnStartAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnStartAll.Name = "btnStartAll";
            this.btnStartAll.Size = new System.Drawing.Size(51, 22);
            this.btnStartAll.Text = "Start All";
            this.btnStartAll.Click += new System.EventHandler(this.BtnStartAll_Click);

            //
            // btnStopAll
            //
            this.btnStopAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnStopAll.Name = "btnStopAll";
            this.btnStopAll.Size = new System.Drawing.Size(50, 22);
            this.btnStopAll.Text = "Stop All";
            this.btnStopAll.Click += new System.EventHandler(this.BtnStopAll_Click);

            //
            // btnResetAlarms
            //
            this.btnResetAlarms.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnResetAlarms.Name = "btnResetAlarms";
            this.btnResetAlarms.Size = new System.Drawing.Size(75, 22);
            this.btnResetAlarms.Text = "Reset Alarms";
            this.btnResetAlarms.Click += new System.EventHandler(this.BtnResetAlarms_Click);

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
            // lblClock
            //
            this.lblClock.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.lblClock.Name = "lblClock";
            this.lblClock.Size = new System.Drawing.Size(34, 22);
            this.lblClock.Text = "Clock";

            //
            // lblPlantStatus
            //
            this.lblPlantStatus.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.lblPlantStatus.Name = "lblPlantStatus";
            this.lblPlantStatus.Size = new System.Drawing.Size(70, 22);
            this.lblPlantStatus.Text = "Plant: Ready";

            //
            // pnlStatus
            //
            this.pnlStatus.AutoScroll = true;
            this.pnlStatus.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlStatus.Location = new System.Drawing.Point(890, 25);
            this.pnlStatus.Name = "pnlStatus";
            this.pnlStatus.Padding = new System.Windows.Forms.Padding(8);
            this.pnlStatus.Size = new System.Drawing.Size(214, 656);
            this.pnlStatus.TabIndex = 1;
            this.pnlStatus.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.pnlStatus.WrapContents = false;

            //
            // tabs
            //
            this.tabs.Controls.Add(this.tabTanks);
            this.tabs.Controls.Add(this.tabWagons);
            this.tabs.Controls.Add(this.tabRectifiers);
            this.tabs.Controls.Add(this.tabAlarms);
            this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabs.Location = new System.Drawing.Point(0, 25);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(890, 656);
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
            this.pnlTanks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTanks.Location = new System.Drawing.Point(3, 3);
            this.pnlTanks.Name = "pnlTanks";
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
            this.alarmGrid.BackgroundColor = System.Drawing.Color.FromArgb(34, 40, 49);
            this.alarmGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.alarmGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alarmGrid.EnableHeadersVisualStyles = false;
            this.alarmGrid.Location = new System.Drawing.Point(0, 0);
            this.alarmGrid.Name = "alarmGrid";
            this.alarmGrid.ReadOnly = true;
            this.alarmGrid.RowHeadersVisible = false;
            this.alarmGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.alarmGrid.Size = new System.Drawing.Size(1096, 512);
            this.alarmGrid.TabIndex = 0;

            //
            // simulatorTimer
            //
            this.simulatorTimer.Interval = 500;
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
