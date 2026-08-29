namespace USR_ElectroPilot.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripDropDownButton mnuPlantOperations;
        private System.Windows.Forms.ToolStripDropDownButton mnuProduction;
        private System.Windows.Forms.ToolStripDropDownButton mnuEngineering;
        private System.Windows.Forms.ToolStripDropDownButton mnuMaintenance;
        private System.Windows.Forms.ToolStripButton btnRefresh;
        private System.Windows.Forms.ToolStripButton btnAddTank;
        private System.Windows.Forms.ToolStripButton btnEditTank;
        private System.Windows.Forms.ToolStripButton btnRemoveTank;
        private System.Windows.Forms.ToolStripButton btnAddRow;
        private System.Windows.Forms.ToolStripButton btnAutoMode;
        private System.Windows.Forms.ToolStripButton btnManualMode;
        private System.Windows.Forms.ToolStripButton btnStartCycle;
        private System.Windows.Forms.ToolStripButton btnStartLine1Job;
        private System.Windows.Forms.ToolStripButton btnStopLine1Job;
        private System.Windows.Forms.ToolStripButton btnStopCycle;
        private System.Windows.Forms.ToolStripButton btnEmergencyStop;
        private System.Windows.Forms.ToolStripButton btnReset;
        private System.Windows.Forms.ToolStripButton btnStartAll;
        private System.Windows.Forms.ToolStripButton btnStopAll;
        private System.Windows.Forms.ToolStripButton btnResetAlarms;
        private System.Windows.Forms.ToolStripButton btnNewJob;
        private System.Windows.Forms.ToolStripButton btnPauseRecipe;
        private System.Windows.Forms.ToolStripButton btnConfiguration;
        private System.Windows.Forms.ToolStripButton btnAddLine;
        private System.Windows.Forms.ToolStripButton btnRecipeEditor;
        private System.Windows.Forms.ToolStripButton btnUserManagement;
        private System.Windows.Forms.ToolStripButton btnAlarmHistory;
        private System.Windows.Forms.ToolStripButton btnReports;
        private System.Windows.Forms.ToolStripButton btnIpConnection;
        private System.Windows.Forms.ToolStripButton btnOpenLogin;
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
            this.mnuPlantOperations = new System.Windows.Forms.ToolStripDropDownButton();
            this.mnuProduction = new System.Windows.Forms.ToolStripDropDownButton();
            this.mnuEngineering = new System.Windows.Forms.ToolStripDropDownButton();
            this.mnuMaintenance = new System.Windows.Forms.ToolStripDropDownButton();
            this.btnRefresh = new System.Windows.Forms.ToolStripButton();
            this.btnAddTank = new System.Windows.Forms.ToolStripButton();
            this.btnEditTank = new System.Windows.Forms.ToolStripButton();
            this.btnRemoveTank = new System.Windows.Forms.ToolStripButton();
            this.btnAddRow = new System.Windows.Forms.ToolStripButton();
            this.btnAutoMode = new System.Windows.Forms.ToolStripButton();
            this.btnManualMode = new System.Windows.Forms.ToolStripButton();
            this.btnStartCycle = new System.Windows.Forms.ToolStripButton();
            this.btnStartLine1Job = new System.Windows.Forms.ToolStripButton();
            this.btnStopLine1Job = new System.Windows.Forms.ToolStripButton();
            this.btnStopCycle = new System.Windows.Forms.ToolStripButton();
            this.btnEmergencyStop = new System.Windows.Forms.ToolStripButton();
            this.btnReset = new System.Windows.Forms.ToolStripButton();
            this.btnStartAll = new System.Windows.Forms.ToolStripButton();
            this.btnStopAll = new System.Windows.Forms.ToolStripButton();
            this.btnResetAlarms = new System.Windows.Forms.ToolStripButton();
            this.btnNewJob = new System.Windows.Forms.ToolStripButton();
            this.btnPauseRecipe = new System.Windows.Forms.ToolStripButton();
            this.btnConfiguration = new System.Windows.Forms.ToolStripButton();
            this.btnAddLine = new System.Windows.Forms.ToolStripButton();
            this.btnRecipeEditor = new System.Windows.Forms.ToolStripButton();
            this.btnUserManagement = new System.Windows.Forms.ToolStripButton();
            this.btnAlarmHistory = new System.Windows.Forms.ToolStripButton();
            this.btnReports = new System.Windows.Forms.ToolStripButton();
            this.btnIpConnection = new System.Windows.Forms.ToolStripButton();
            this.btnOpenLogin = new System.Windows.Forms.ToolStripButton();
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
                this.mnuPlantOperations,
                this.mnuProduction,
                this.mnuEngineering,
                this.mnuMaintenance,
                this.btnRefresh,
                this.btnOpenLogin,
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
            // grouped toolbar menus
            //
            this.mnuPlantOperations.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.mnuPlantOperations.Name = "mnuPlantOperations";
            this.mnuPlantOperations.Text = "Plant Operations";
            this.mnuPlantOperations.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.btnStartAll,
                this.btnStopAll,
                this.btnEmergencyStop,
                this.btnReset
            });
            this.mnuProduction.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.mnuProduction.Name = "mnuProduction";
            this.mnuProduction.Text = "Production";
            this.mnuProduction.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.btnAutoMode,
                this.btnManualMode,
                this.btnNewJob,
                this.btnStartLine1Job,
                this.btnStopLine1Job,
                this.btnStartCycle,
                this.btnPauseRecipe
            });
            this.mnuEngineering.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.mnuEngineering.Name = "mnuEngineering";
            this.mnuEngineering.Text = "Engineering";
            this.mnuEngineering.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.btnConfiguration,
                this.btnAddLine,
                this.btnRecipeEditor,
                this.btnUserManagement
            });
            this.mnuMaintenance.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.mnuMaintenance.Name = "mnuMaintenance";
            this.mnuMaintenance.Text = "Maintenance";
            this.mnuMaintenance.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.btnAlarmHistory,
                this.btnReports,
                this.btnResetAlarms,
                this.btnIpConnection
            });

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
            this.btnAddRow.Text = "Tank Layout";
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
            this.btnStartCycle.Text = "Start Recipe";
            this.btnStartCycle.Click += new System.EventHandler(this.BtnStartCycle_Click);

            //
            // btnStartLine1Job
            //
            this.btnStartLine1Job.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnStartLine1Job.Name = "btnStartLine1Job";
            this.btnStartLine1Job.Size = new System.Drawing.Size(100, 22);
            this.btnStartLine1Job.Text = "Start Line 1 Job";
            this.btnStartLine1Job.Click += new System.EventHandler(this.BtnStartLine1Job_Click);

            //
            // btnStopLine1Job
            //
            this.btnStopLine1Job.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnStopLine1Job.Name = "btnStopLine1Job";
            this.btnStopLine1Job.Size = new System.Drawing.Size(96, 22);
            this.btnStopLine1Job.Text = "Stop Line 1 Job";
            this.btnStopLine1Job.Click += new System.EventHandler(this.BtnStopLine1Job_Click);

            //
            // btnStopCycle
            //
            this.btnStopCycle.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnStopCycle.Name = "btnStopCycle";
            this.btnStopCycle.Size = new System.Drawing.Size(66, 22);
            this.btnStopCycle.Text = "Stop Recipe";
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
            this.btnStartAll.Text = "Start Plant";
            this.btnStartAll.Click += new System.EventHandler(this.BtnStartAll_Click);

            //
            // btnStopAll
            //
            this.btnStopAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnStopAll.Name = "btnStopAll";
            this.btnStopAll.Size = new System.Drawing.Size(50, 22);
            this.btnStopAll.Text = "Stop Plant";
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
            // production / engineering / maintenance menu items
            //
            this.btnNewJob.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnNewJob.Name = "btnNewJob";
            this.btnNewJob.Text = "New Job";
            this.btnNewJob.Click += new System.EventHandler(this.BtnNewJob_Click);
            this.btnPauseRecipe.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnPauseRecipe.Name = "btnPauseRecipe";
            this.btnPauseRecipe.Text = "Pause Recipe";
            this.btnPauseRecipe.Click += new System.EventHandler(this.BtnStopCycle_Click);
            this.btnConfiguration.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnConfiguration.Name = "btnConfiguration";
            this.btnConfiguration.Text = "Plant Setup";
            this.btnConfiguration.Click += new System.EventHandler(this.BtnConfiguration_Click);
            this.btnAddLine.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnAddLine.Name = "btnAddLine";
            this.btnAddLine.Text = "Line Creation Disabled";
            this.btnAddLine.Click += new System.EventHandler(this.BtnAddLine_Click);
            this.btnRecipeEditor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnRecipeEditor.Name = "btnRecipeEditor";
            this.btnRecipeEditor.Text = "Recipe Editor";
            this.btnRecipeEditor.Click += new System.EventHandler(this.BtnRecipeEditor_Click);
            this.btnUserManagement.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnUserManagement.Name = "btnUserManagement";
            this.btnUserManagement.Text = "User Management";
            this.btnUserManagement.Click += new System.EventHandler(this.BtnUserManagement_Click);
            this.btnAlarmHistory.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnAlarmHistory.Name = "btnAlarmHistory";
            this.btnAlarmHistory.Text = "Alarm History";
            this.btnAlarmHistory.Click += new System.EventHandler(this.BtnAlarmHistory_Click);
            this.btnReports.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnReports.Name = "btnReports";
            this.btnReports.Text = "Reports";
            this.btnReports.Click += new System.EventHandler(this.BtnReports_Click);

            this.btnIpConnection.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnIpConnection.Name = "btnIpConnection";
            this.btnIpConnection.Text = "IP Connection";
            this.btnIpConnection.Click += new System.EventHandler(this.BtnIpConnection_Click);

            //
            // btnOpenLogin
            //
            this.btnOpenLogin.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnOpenLogin.Name = "btnOpenLogin";
            this.btnOpenLogin.Size = new System.Drawing.Size(103, 22);
            this.btnOpenLogin.Text = "Login Another";
            this.btnOpenLogin.Click += new System.EventHandler(this.BtnOpenLogin_Click);

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
            this.pnlStatus.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlStatus.Location = new System.Drawing.Point(0, 25);
            this.pnlStatus.Name = "pnlStatus";
            this.pnlStatus.Padding = new System.Windows.Forms.Padding(8);
            this.pnlStatus.Size = new System.Drawing.Size(1104, 108);
            this.pnlStatus.TabIndex = 1;
            this.pnlStatus.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.pnlStatus.WrapContents = true;

            //
            // tabs
            //
            this.tabs.Controls.Add(this.tabTanks);
            this.tabs.Controls.Add(this.tabWagons);
            this.tabs.Controls.Add(this.tabRectifiers);
            this.tabs.Controls.Add(this.tabAlarms);
            this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabs.Location = new System.Drawing.Point(0, 133);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(1104, 548);
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
