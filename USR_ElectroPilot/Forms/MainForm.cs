using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using USR_ElectroPilot.Controls;
using USR_ElectroPilot.Forms.Admin;
using USR_ElectroPilot.Helpers;
using USR_ElectroPilot.Models;
using USR_ElectroPilot.Services;

namespace USR_ElectroPilot.Forms
{
    public partial class MainForm : Form
    {
        private readonly DashboardService _dashboardService = new DashboardService();
        private readonly TankService _tankService = new TankService();
        private readonly WagonService _wagonService = new WagonService();
        private readonly RectifierService _rectifierService = new RectifierService();
        private readonly SimulatorService _simulatorService = new SimulatorService();
        private readonly AlarmService _alarmService = new AlarmService();
        private readonly TankHistoryService _tankHistoryService = new TankHistoryService();
        private readonly ProcessRecipeService _processRecipeService = new ProcessRecipeService();
        private readonly HoistStatusService _hoistStatusService = new HoistStatusService();
        private readonly HoistService _hoistService = new HoistService();
        private readonly JobService _jobService = new JobService();
        private readonly ScadaLayoutService _scadaLayoutService = new ScadaLayoutService();
        private readonly EquipmentTelemetryAlarmService _equipmentTelemetryAlarmService = new EquipmentTelemetryAlarmService();
        private readonly Dictionary<string, StatusCardControl> _statusCards = new Dictionary<string, StatusCardControl>();
        private readonly UserModel _sessionUser;
        private Plant3DHostControl _scadaOverview;
        private TankModel _selectedTank;
        private List<ProcessStepModel> _processSteps = new List<ProcessStepModel>();
        private List<HoistModel> _hoists = new List<HoistModel>();
        private List<JobModel> _jobs = new List<JobModel>();
        private HoistStatusModel _hoistStatus = new HoistStatusModel { Status = "Idle" };
        private bool _autoMode = true;
        private bool _cycleRunning;
        private bool _emergencyStop;
        private int _currentStepIndex;
        private int _remainingStepSeconds;
        private double _hoistVisualIndex;
        private double _hoistTargetIndex;
        private bool _hoistVisualInitialized;
        private int _scadaTankRows = 1;
        private bool _processSecondTick;
        private int _liveRefreshTicks;
        private long _equipmentTelemetrySequence;
        private PlantTelemetrySnapshot _latestEquipmentTelemetry = PlantTelemetrySnapshot.Empty;

        public MainForm()
            : this(AppSession.CurrentUser)
        {
        }

        public MainForm(UserModel sessionUser)
        {
            _sessionUser = sessionUser ?? new UserModel
            {
                Username = "viewer",
                DisplayName = "Viewer",
                Role = Constants.RoleViewer,
                IsActive = true
            };
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            UiHelper.ApplyDarkTheme(this);
            HideLegacyTopNavigation();
            pnlStatus.Visible = false;
            pnlStatus.Height = 0;
            ConfigureMenuUsability();
            CsvExporter.AddExportButton(alarmGrid, "dashboard_alarms");
            Text = Constants.ApplicationName + " - " + _sessionUser.Username;
            lblUser.Text = _sessionUser.Username + " (" + _sessionUser.Role + ")";
            _scadaTankRows = 1;
            ConfigureAddRowAccess();
            LoadProcessState();
            ConfigureRoleAccess();
            UpdateModeButtons();
            UpdateHeaderIndicators(null);
            RefreshDashboard();
            simulatorTimer.Start();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            RefreshDashboard();
        }

        private void BtnOpenLogin_Click(object sender, EventArgs e)
        {
            DashboardWindowManager.ShowLoginAndOpenDashboard(this);
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            new AuthService().Logout(_sessionUser);
            Close();
        }

        private void BtnAddTank_Click(object sender, EventArgs e)
        {
            if (!RequirePermission(CanEngineer(), "Only Admin and Supervisor users can add tanks."))
            {
                return;
            }

            using (var form = new TankEditForm())
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    _tankService.AddTank(form.Tank);
                    RefreshDashboard();
                }
            }
        }

        private void BtnEditTank_Click(object sender, EventArgs e)
        {
            if (!RequirePermission(CanEngineer(), "Only Admin and Supervisor users can edit tanks."))
            {
                return;
            }

            if (_selectedTank == null)
            {
                MessageBox.Show("Select a tank first.", Constants.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var form = new TankEditForm(_selectedTank))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    _tankService.UpdateTank(form.Tank);
                    RefreshDashboard();
                }
            }
        }

        private void BtnRemoveTank_Click(object sender, EventArgs e)
        {
            if (!RequirePermission(CanEngineer(), "Only Admin and Supervisor users can remove tanks."))
            {
                return;
            }

            if (_selectedTank == null)
            {
                MessageBox.Show("Select a tank first.", Constants.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("Remove " + _selectedTank.Name + "?", Constants.ApplicationName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                _tankService.DeleteTank(_selectedTank.Id);
                _selectedTank = null;
                RefreshDashboard();
            }
        }

        private void BtnAddRow_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Multiple tank-row creation is disabled. Use Add Tank to increase the tank count on the main line.", Constants.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnAutoMode_Click(object sender, EventArgs e)
        {
            if (!RequirePermission(CanRunProduction(), "Only Admin, Supervisor, and Operator users can change production mode."))
            {
                return;
            }

            _autoMode = true;
            UpdateModeButtons();
            RefreshDashboard();
        }

        private void BtnManualMode_Click(object sender, EventArgs e)
        {
            if (!RequirePermission(CanRunProduction(), "Only Admin, Supervisor, and Operator users can change production mode."))
            {
                return;
            }

            _autoMode = false;
            _cycleRunning = false;
            _hoistService.StopAll();
            _hoistStatusService.Save(GetCurrentStepTankId(), "Idle");
            LoadProcessState();
            UpdateModeButtons();
            RefreshDashboard();
        }

        private void BtnStartCycle_Click(object sender, EventArgs e)
        {
            if (!RequirePermission(CanRunProduction(), "Only Admin, Supervisor, and Operator users can start production."))
            {
                return;
            }

            if (_emergencyStop)
            {
                MessageBox.Show("Reset emergency stop before starting cycle.", Constants.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            StartLineJob(1);
        }

        private void BtnNewJob_Click(object sender, EventArgs e)
        {
            if (!RequirePermission(CanRunProduction(), "Only Admin, Supervisor, and Operator users can start production jobs."))
            {
                return;
            }

            if (_emergencyStop)
            {
                MessageBox.Show("Reset emergency stop before creating a job.", Constants.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            StartLineJob(1);
        }

        private void BtnStartLine1Job_Click(object sender, EventArgs e)
        {
            StartLineJob(1);
        }

        private void BtnStopLine1Job_Click(object sender, EventArgs e)
        {
            StopLineJob(1);
        }

        private void StartLineJob(int lineId)
        {
            if (!RequirePermission(CanRunProduction(), "Only Admin, Supervisor, and Operator users can start production jobs."))
            {
                return;
            }

            if (_emergencyStop)
            {
                MessageBox.Show("Reset emergency stop before creating a job.", Constants.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!HasRecipeForLine(lineId))
            {
                MessageBox.Show("The main production line has no configured recipe or tanks.", Constants.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!HasHoistForLine(lineId))
            {
                MessageBox.Show("The main production line has no assigned hoist.", Constants.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _autoMode = true;
            _cycleRunning = true;
            _jobService.StartLineJob(_processSteps, lineId);
            LoadProcessState();
            UpdateModeButtons();
            RefreshDashboard();
        }

        private void StopLineJob(int lineId)
        {
            if (!RequirePermission(CanRunProduction(), "Only Admin, Supervisor, and Operator users can stop production jobs."))
            {
                return;
            }

            _jobService.PauseLineJob(lineId);
            _hoistService.StopLine(lineId);
            LoadProcessState();
            _cycleRunning = HasAnyRunningJob();
            UpdateModeButtons();
            RefreshDashboard();
        }

        private void StartAllLineJobs()
        {
            if (!RequirePermission(CanRunProduction(), "Only Admin, Supervisor, and Operator users can start production."))
            {
                return;
            }

            if (_emergencyStop)
            {
                MessageBox.Show("Reset emergency stop before starting recipe.", Constants.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            StartLineJob(1);
        }

        private void BtnStopCycle_Click(object sender, EventArgs e)
        {
            if (!RequirePermission(CanRunProduction(), "Only Admin, Supervisor, and Operator users can pause production."))
            {
                return;
            }

            _cycleRunning = false;
            _hoistService.StopAll();
            _jobService.PauseAllActiveJobs();
            LoadProcessState();
            UpdateModeButtons();
            RefreshDashboard();
        }

        private void BtnEmergencyStop_Click(object sender, EventArgs e)
        {
            if (!RequirePermission(CanOperatePlant(), "Only Admin, Supervisor, and Operator users can trigger emergency stop."))
            {
                return;
            }

            _emergencyStop = true;
            _cycleRunning = false;
            _hoistService.EmergencyStop();
            _tankService.StopAllTanks();
            _jobService.PauseAllActiveJobs();
            _hoistStatusService.Save(GetCurrentStepTankId(), "EmergencyStop");
            _alarmService.RaiseAlarm("Hoist", "Critical", "Emergency stop activated.");
            LoadProcessState();
            UpdateModeButtons();
            RefreshDashboard();
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            if (!RequirePermission(CanOperatePlant(), "Only Admin, Supervisor, and Operator users can reset plant operation."))
            {
                return;
            }

            ResetAllPlantOperation();
        }

        private void ResetAllPlantOperation()
        {
            _emergencyStop = false;
            _cycleRunning = false;
            _currentStepIndex = 0;
            _remainingStepSeconds = 0;
            _alarmService.ResetActiveAlarms(_sessionUser.Username);
            foreach (var tank in _tankService.GetTanks())
            {
                if (string.Equals(tank.Status, Constants.StatusFault, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(tank.Status, Constants.StatusWarning, StringComparison.OrdinalIgnoreCase))
                {
                    _tankService.ResetTank(tank);
                }
            }

            _hoistService.StopAll();
            _jobService.StopAllActiveJobs();
            _hoistStatusService.Save(GetCurrentStepTankId(), "Idle");
            LoadProcessState();
            UpdateModeButtons();
            RefreshDashboard();
        }

        private void ResetLineOperation(int lineId)
        {
            if (!RequirePermission(CanOperatePlant(), "Only Admin, Supervisor, and Operator users can reset line operation."))
            {
                return;
            }

            _jobService.StopLineActiveJobs(lineId);
            _hoistService.StopLine(lineId);
            foreach (var tank in _tankService.GetTanks().Where(tank => tank.LineId == lineId))
            {
                if (string.Equals(tank.Status, Constants.StatusFault, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(tank.Status, Constants.StatusWarning, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(tank.Status, Constants.StatusRunning, StringComparison.OrdinalIgnoreCase))
                {
                    _tankService.ResetTank(tank);
                }
            }

            LoadProcessState();
            _cycleRunning = HasAnyRunningJob();
            UpdateModeButtons();
            RefreshDashboard();
        }

        private void BtnStartAll_Click(object sender, EventArgs e)
        {
            if (!RequirePermission(CanOperatePlant(), "Only Admin, Supervisor, and Operator users can start the plant."))
            {
                return;
            }

            if (_emergencyStop)
            {
                MessageBox.Show("Reset emergency stop before starting the plant.", Constants.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (var tank in _tankService.GetTanks())
            {
                if (string.Equals(tank.Status, Constants.StatusFault, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Plant cannot start while tank faults are active. Reset faults first.", Constants.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            _tankService.StartAllTanks();
            RefreshDashboard();
        }

        private void BtnStopAll_Click(object sender, EventArgs e)
        {
            if (!RequirePermission(CanOperatePlant(), "Only Admin, Supervisor, and Operator users can stop the plant."))
            {
                return;
            }

            _cycleRunning = false;
            _hoistService.StopAll();
            _jobService.StopAllActiveJobs();
            _tankService.StopAllTanks();
            LoadProcessState();
            UpdateModeButtons();
            RefreshDashboard();
        }

        private void BtnResetAlarms_Click(object sender, EventArgs e)
        {
            if (!RequirePermission(CanResetAlarms(), "Only Admin and Supervisor users can reset alarms."))
            {
                return;
            }

            _alarmService.ResetActiveAlarms(_sessionUser.Username);

            foreach (var tank in _tankService.GetTanks())
            {
                if (string.Equals(tank.Status, Constants.StatusFault, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(tank.Status, Constants.StatusWarning, StringComparison.OrdinalIgnoreCase))
                {
                    _tankService.ResetTank(tank);
                }
            }

            RefreshDashboard();
        }

        private void BtnConfiguration_Click(object sender, EventArgs e)
        {
            if (!RequirePermission(CanEngineer(), "Only Admin and Supervisor users can open plant configuration."))
            {
                return;
            }

            MessageBox.Show("Line creation is disabled. Use Add Tank, Edit Tank, Recipe Editor, and hoist configuration for the main line.", Constants.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnAddLine_Click(object sender, EventArgs e)
        {
            if (!RequirePermission(CanEngineer(), "Only Admin and Supervisor users can configure the plant."))
            {
                return;
            }

            MessageBox.Show("Multiple production line creation has been removed. Increase tank count with Add Tank and run the plant with single hoist H1.", Constants.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnRecipeEditor_Click(object sender, EventArgs e)
        {
            if (!RequirePermission(CanEngineer(), "Only Admin and Supervisor users can edit recipes."))
            {
                return;
            }

            using (var form = new RecipeForm())
            {
                form.ShowDialog(this);
            }
        }

        private void BtnUserManagement_Click(object sender, EventArgs e)
        {
            if (!RequirePermission(CanAdmin(), "Only Admin users can manage users."))
            {
                return;
            }

            using (var form = new AdminPanelForm())
            {
                form.ShowDialog(this);
            }
        }

        private void BtnAlarmHistory_Click(object sender, EventArgs e)
        {
            if (!RequirePermission(CanViewMaintenance(), "This role cannot open alarm history."))
            {
                return;
            }

            using (var form = new AlarmHistoryForm())
            {
                form.ShowDialog(this);
            }
        }

        private void BtnReports_Click(object sender, EventArgs e)
        {
            if (!RequirePermission(CanViewMaintenance(), "This role cannot open reports."))
            {
                return;
            }

            using (var form = new LoadHistoryForm())
            {
                form.ShowDialog(this);
            }
        }

        private void BtnIpConnection_Click(object sender, EventArgs e)
        {
            using (var form = new IpConnectionForm())
            {
                form.ShowDialog(this);
            }
        }

        private void ConfigureMenuUsability()
        {
            btnNewJob.Text = "New Job";
            btnStartCycle.Text = "Start Job";
            btnPauseRecipe.Text = "Pause All Jobs";
            btnReset.Text = "Reset All";
            btnAutoMode.ToolTipText = "Enable automatic recipe sequencing.";
            btnManualMode.ToolTipText = "Stop automatic sequencing and leave hoists idle.";
            btnNewJob.ToolTipText = "Create an automatic production job on the main line.";
            btnStartLine1Job.ToolTipText = "Hidden compatibility command.";
            btnStopLine1Job.ToolTipText = "Hidden compatibility command.";
            btnStartCycle.ToolTipText = "Start an automatic production job on the main line.";
            btnPauseRecipe.ToolTipText = "Pause running production jobs and release hoists.";
            btnStartAll.ToolTipText = "Start all healthy tanks. Active tank faults block plant start.";
            btnStopAll.ToolTipText = "Stop tanks and hoists without clearing fault or warning states.";
            btnEmergencyStop.ToolTipText = "Stop tanks, set every hoist to emergency stop, and raise a critical alarm.";
            btnReset.ToolTipText = "Reset the plant, clear emergency stop, reset active alarms, and idle hoists.";
            btnOpenLogin.ToolTipText = "Open a separate dashboard for another user without closing this one.";
            btnConfiguration.ToolTipText = "Line creation is disabled.";
            btnAddLine.ToolTipText = "Multiple line creation is disabled.";
            btnRecipeEditor.ToolTipText = "Open recipe editor.";
            btnUserManagement.ToolTipText = "Open user and administration tools.";
            btnAlarmHistory.ToolTipText = "Open alarm history.";
            btnReports.ToolTipText = "Open reports and load history.";
            btnResetAlarms.ToolTipText = "Reset active alarms and warning/fault tank states.";
            btnIpConnection.ToolTipText = "Open raw TCP/IP connection test screen for future PLC/device communication.";
        }

        private void ConfigureRoleAccess()
        {
            var canOperate = CanOperatePlant();
            var canProduce = CanRunProduction();
            var canEngineer = CanEngineer();
            var canAdmin = CanAdmin();
            var canMaintenance = CanViewMaintenance();
            var canResetAlarms = CanResetAlarms();

            SetMenuVisible(mnuPlantOperations, canOperate);
            SetMenuVisible(mnuProduction, canProduce);
            SetMenuVisible(mnuEngineering, canEngineer || canAdmin);
            SetMenuVisible(mnuMaintenance, canMaintenance || canResetAlarms);

            SetItemVisible(btnStartAll, canOperate);
            SetItemVisible(btnStopAll, canOperate);
            SetItemVisible(btnEmergencyStop, canOperate);
            SetItemVisible(btnReset, canOperate);
            SetItemVisible(btnAutoMode, canProduce);
            SetItemVisible(btnManualMode, canProduce);
            SetItemVisible(btnNewJob, canProduce);
            SetItemVisible(btnStartLine1Job, false);
            SetItemVisible(btnStopLine1Job, false);
            SetItemVisible(btnStartCycle, canProduce);
            SetItemVisible(btnPauseRecipe, canProduce);

            SetItemVisible(btnConfiguration, false);
            SetItemVisible(btnAddLine, false);
            SetItemVisible(btnRecipeEditor, canEngineer);
            SetItemVisible(btnAddTank, canEngineer);
            SetItemVisible(btnEditTank, canEngineer);
            SetItemVisible(btnRemoveTank, canEngineer);
            SetItemVisible(btnUserManagement, canAdmin);

            SetItemVisible(btnAlarmHistory, canMaintenance);
            SetItemVisible(btnReports, canMaintenance);
            SetItemVisible(btnResetAlarms, canResetAlarms);
            SetItemVisible(btnIpConnection, true);
        }

        private static void SetMenuVisible(ToolStripItem item, bool visible)
        {
            item.Available = visible;
            item.Visible = visible;
            item.Enabled = visible;
        }

        private static void SetItemVisible(ToolStripItem item, bool visible)
        {
            item.Available = visible;
            item.Visible = visible;
            item.Enabled = visible;
        }

        private bool CanAdmin()
        {
            return HasSessionRole(Constants.RoleAdmin);
        }

        private bool CanEngineer()
        {
            return HasSessionRole(Constants.RoleAdmin, Constants.RoleSupervisor);
        }

        private bool CanOperatePlant()
        {
            return HasSessionRole(Constants.RoleAdmin, Constants.RoleSupervisor, Constants.RoleOperator);
        }

        private bool CanRunProduction()
        {
            return HasSessionRole(Constants.RoleAdmin, Constants.RoleSupervisor, Constants.RoleOperator);
        }

        private bool CanResetAlarms()
        {
            return HasSessionRole(Constants.RoleAdmin, Constants.RoleSupervisor);
        }

        private bool CanViewMaintenance()
        {
            return HasSessionRole(Constants.RoleAdmin, Constants.RoleSupervisor, Constants.RoleOperator, Constants.RoleViewer);
        }

        private bool HasSessionRole(params string[] roles)
        {
            if (_sessionUser == null || roles == null)
            {
                return false;
            }

            foreach (var role in roles)
            {
                if (string.Equals(_sessionUser.Role, role, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private bool RequirePermission(bool allowed, string message)
        {
            if (allowed)
            {
                return true;
            }

            MessageBox.Show(message, Constants.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        private void RefreshDashboard()
        {
            try
            {
                var status = _dashboardService.GetPlantStatus();
                UpdateHeaderIndicators(status);
                LoadStatusCards(status);
                LoadTanks();
                LoadWagons();
                LoadRectifiers();
                LoadAlarms();
            }
            catch (Exception ex)
            {
                Logger.Error("Dashboard refresh failed", ex);
                MessageBox.Show("Dashboard refresh failed. Check Logs folder.", Constants.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SimulatorTimer_Tick(object sender, EventArgs e)
        {
            var tanks = _tankService.GetTanks();
            var previousStatuses = new Dictionary<int, string>();
            foreach (var tank in tanks)
            {
                previousStatuses[tank.Id] = tank.Status;
            }

            var rectifiers = new List<RectifierModel>();
            foreach (Control control in pnlRectifiers.Controls)
            {
                var rectifierControl = control as RectifierControl;
                if (rectifierControl != null && rectifierControl.Rectifier != null)
                {
                    rectifiers.Add(rectifierControl.Rectifier);
                }
            }

            _simulatorService.SimulateTanks(tanks);
            _simulatorService.SimulateRectifiers(rectifiers);
            SimulateProcessCycle(tanks);

            foreach (var tank in tanks)
            {
                ApplyTankOccupancyStatus(tank);
                _tankService.UpdateTankRuntimeState(tank);
                _tankHistoryService.RecordSnapshot(tank);
                RaiseStateAlarmIfNeeded(tank, previousStatuses.ContainsKey(tank.Id) ? previousStatuses[tank.Id] : string.Empty);
                RaiseProcessAlarmIfNeeded(tank);
            }

            RefreshLiveDashboard();
        }

        private void RefreshLiveDashboard()
        {
            _liveRefreshTicks++;
            var status = _dashboardService.GetPlantStatus();
            UpdateHeaderIndicators(status);
            LoadStatusCards(status);
            LoadTanks();
            pnlRectifiers.Invalidate(true);
            if (_liveRefreshTicks % 4 == 0)
            {
                LoadAlarms();
            }
        }

        private void LoadStatusCards(PlantStatusModel status)
        {
            UpsertStatusCard("Tanks", status.TotalTankCount.ToString(), "Configured");
            UpsertStatusCard("Running", status.RunningTankCount.ToString(), "Active");
            UpsertStatusCard("Alarms", status.ActiveAlarmCount.ToString(), status.FaultTankCount + " tank faults");
            UpsertStatusCard("Jobs", _jobs.Count.ToString(), status.CurrentProcessStep);
            UpsertStatusCard("Hoist", status.HoistPosition, status.HoistState);
        }

        private void UpsertStatusCard(string title, string value, string caption)
        {
            StatusCardControl card;
            if (!_statusCards.TryGetValue(title, out card))
            {
                card = new StatusCardControl { Title = title, Margin = new Padding(8) };
                _statusCards[title] = card;
                pnlStatus.Controls.Add(card);
            }

            card.Value = value;
            card.Caption = caption;
        }

        private void LoadTanks()
        {
            var tanks = _tankService.GetTanks();
            if (_scadaOverview == null)
            {
                pnlTanks.AutoScroll = true;
                _scadaOverview = new Plant3DHostControl(true)
                {
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                    Location = new System.Drawing.Point(0, 0)
                };
                _scadaOverview.TankSelected += ScadaOverview_TankSelected;
                _scadaOverview.StartClicked += TankControl_StartClicked;
                _scadaOverview.StopClicked += TankControl_StopClicked;
                _scadaOverview.FaultClicked += TankControl_FaultClicked;
                _scadaOverview.ResetClicked += TankControl_ResetClicked;
                _scadaOverview.RemoveClicked += TankControl_RemoveClicked;
                _scadaOverview.DashboardCommandRequested += ScadaOverview_DashboardCommandRequested;
                pnlTanks.Resize += PnlTanks_Resize;
                pnlTanks.Controls.Add(_scadaOverview);
            }

            _scadaOverview.CanOperateTanks = CanOperatePlant();
            _scadaOverview.CanEngineerTanks = CanEngineer();
            ResizeScadaOverview(tanks);
            _latestEquipmentTelemetry = _simulatorService.CreateEquipmentSnapshot(tanks, ++_equipmentTelemetrySequence, DateTime.UtcNow);
            foreach (var alarm in _equipmentTelemetryAlarmService.Evaluate(
                _latestEquipmentTelemetry,
                DateTime.UtcNow,
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(3)))
            {
                _alarmService.RaiseAlarm(alarm.Source, alarm.Severity, alarm.Message);
            }

            _scadaOverview.BindData(
                tanks,
                _wagonService.GetWagons(),
                _processSteps,
                _hoists,
                _jobs,
                _hoistStatus,
                _hoistTargetIndex,
                GetCurrentStepName(),
                _remainingStepSeconds,
                _autoMode,
                _emergencyStop,
                _scadaTankRows,
                _latestEquipmentTelemetry);
            if (_selectedTank != null)
            {
                _scadaOverview.SelectTank(_selectedTank.Id);
            }
        }

        private void PnlTanks_Resize(object sender, EventArgs e)
        {
            if (_scadaOverview != null)
            {
                ResizeScadaOverview(_tankService.GetTanks());
            }
        }

        private void ResizeScadaOverview(IList<TankModel> tanks)
        {
            if (_scadaOverview == null)
            {
                return;
            }

            var requiredHeight = 250 + 176;
            requiredHeight = Math.Max(pnlTanks.ClientSize.Height - 4, requiredHeight);
            _scadaOverview.Width = Math.Max(900, pnlTanks.ClientSize.Width - (pnlTanks.VerticalScroll.Visible ? SystemInformation.VerticalScrollBarWidth : 0) - 4);
            _scadaOverview.Height = requiredHeight;
        }

        private void ScadaOverview_TankSelected(object sender, TankControlEventArgs e)
        {
            _selectedTank = FindTank(e.TankId);
        }

        private void ScadaOverview_DashboardCommandRequested(object sender, string command)
        {
            if (string.Equals(command, "Overview", StringComparison.OrdinalIgnoreCase))
            {
                tabs.SelectedTab = tabTanks;
            }
            else if (string.Equals(command, "Refresh", StringComparison.OrdinalIgnoreCase))
            {
                BtnRefresh_Click(sender, EventArgs.Empty);
            }
            else if (string.Equals(command, "Start Plant", StringComparison.OrdinalIgnoreCase))
            {
                BtnStartAll_Click(sender, EventArgs.Empty);
            }
            else if (string.Equals(command, "Stop Plant", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(command, "Stop", StringComparison.OrdinalIgnoreCase))
            {
                BtnStopAll_Click(sender, EventArgs.Empty);
            }
            else if (string.Equals(command, "Start Job", StringComparison.OrdinalIgnoreCase))
            {
                BtnStartCycle_Click(sender, EventArgs.Empty);
            }
            else if (string.Equals(command, "Pause Job", StringComparison.OrdinalIgnoreCase))
            {
                BtnStopCycle_Click(sender, EventArgs.Empty);
            }
            else if (string.Equals(command, "Auto", StringComparison.OrdinalIgnoreCase))
            {
                BtnAutoMode_Click(sender, EventArgs.Empty);
            }
            else if (string.Equals(command, "Manual", StringComparison.OrdinalIgnoreCase))
            {
                BtnManualMode_Click(sender, EventArgs.Empty);
            }
            else if (string.Equals(command, "Emergency Stop", StringComparison.OrdinalIgnoreCase))
            {
                BtnEmergencyStop_Click(sender, EventArgs.Empty);
            }
            else if (string.Equals(command, "Reset", StringComparison.OrdinalIgnoreCase))
            {
                BtnReset_Click(sender, EventArgs.Empty);
            }
            else if (string.Equals(command, "Add Tank", StringComparison.OrdinalIgnoreCase))
            {
                BtnAddTank_Click(sender, EventArgs.Empty);
            }
            else if (string.Equals(command, "Edit Tank", StringComparison.OrdinalIgnoreCase))
            {
                BtnEditTank_Click(sender, EventArgs.Empty);
            }
            else if (string.Equals(command, "Remove Tank", StringComparison.OrdinalIgnoreCase))
            {
                BtnRemoveTank_Click(sender, EventArgs.Empty);
            }
            else if (string.Equals(command, "Recipe", StringComparison.OrdinalIgnoreCase))
            {
                BtnRecipeEditor_Click(sender, EventArgs.Empty);
            }
            else if (string.Equals(command, "Alarms", StringComparison.OrdinalIgnoreCase))
            {
                BtnAlarmHistory_Click(sender, EventArgs.Empty);
            }
            else if (string.Equals(command, "Reset Alarms", StringComparison.OrdinalIgnoreCase))
            {
                BtnResetAlarms_Click(sender, EventArgs.Empty);
            }
            else if (string.Equals(command, "Reports", StringComparison.OrdinalIgnoreCase))
            {
                BtnReports_Click(sender, EventArgs.Empty);
            }
            else if (string.Equals(command, "Users", StringComparison.OrdinalIgnoreCase))
            {
                BtnUserManagement_Click(sender, EventArgs.Empty);
            }
            else if (string.Equals(command, "IP Connect", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(command, "IP Connection", StringComparison.OrdinalIgnoreCase))
            {
                BtnIpConnection_Click(sender, EventArgs.Empty);
            }
            else if (string.Equals(command, "Login", StringComparison.OrdinalIgnoreCase))
            {
                BtnOpenLogin_Click(sender, EventArgs.Empty);
            }
            else if (string.Equals(command, "Logout", StringComparison.OrdinalIgnoreCase))
            {
                BtnLogout_Click(sender, EventArgs.Empty);
            }
        }

        private void LoadWagons()
        {
            pnlWagons.Controls.Clear();
            foreach (var hoist in GetSingleMainHoistList(_hoistService.GetHoists()))
            {
                pnlWagons.Controls.Add(new HoistControl
                {
                    Hoist = hoist,
                    Job = FindJob(hoist.CurrentJobId),
                    Margin = new Padding(8)
                });
            }

            foreach (var wagon in _wagonService.GetWagons())
            {
                pnlWagons.Controls.Add(new WagonControl { Wagon = wagon, Margin = new Padding(8) });
            }
        }

        private void LoadRectifiers()
        {
            pnlRectifiers.Controls.Clear();
            foreach (var rectifier in _rectifierService.GetRectifiers())
            {
                pnlRectifiers.Controls.Add(new RectifierControl { Rectifier = rectifier, Margin = new Padding(8) });
            }
        }

        private void LoadAlarms()
        {
            alarmGrid.AutoGenerateColumns = true;
            alarmGrid.DataSource = _alarmService.GetAlarms();
            ApplyAlarmGridColors();
        }

        private void ApplyAlarmGridColors()
        {
            foreach (DataGridViewRow row in alarmGrid.Rows)
            {
                var severity = Convert.ToString(row.Cells["Severity"].Value);
                var state = Convert.ToString(row.Cells["State"].Value);

                if (string.Equals(state, Constants.AlarmAcknowledged, StringComparison.OrdinalIgnoreCase))
                {
                    row.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(70, 80, 95);
                    row.DefaultCellStyle.ForeColor = UiHelper.ForeColor;
                }
                else if (string.Equals(severity, "Critical", StringComparison.OrdinalIgnoreCase))
                {
                    row.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(110, 40, 40);
                    row.DefaultCellStyle.ForeColor = System.Drawing.Color.White;
                }
                else if (string.Equals(severity, "Warning", StringComparison.OrdinalIgnoreCase))
                {
                    row.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(110, 85, 35);
                    row.DefaultCellStyle.ForeColor = System.Drawing.Color.White;
                }
            }
        }

        private void TankControl_StartClicked(object sender, TankControlEventArgs e)
        {
            if (!RequirePermission(CanOperatePlant(), "Only Admin, Supervisor, and Operator users can start tanks."))
            {
                return;
            }

            var tank = FindTank(e.TankId);
            _tankService.StartTank(tank);
            RefreshDashboard();
        }

        private void TankControl_StopClicked(object sender, TankControlEventArgs e)
        {
            if (!RequirePermission(CanOperatePlant(), "Only Admin, Supervisor, and Operator users can stop tanks."))
            {
                return;
            }

            var tank = FindTank(e.TankId);
            _tankService.StopTank(tank);
            RefreshDashboard();
        }

        private void TankControl_FaultClicked(object sender, TankControlEventArgs e)
        {
            if (!RequirePermission(CanOperatePlant(), "Only Admin, Supervisor, and Operator users can set tank faults."))
            {
                return;
            }

            var tank = FindTank(e.TankId);
            _tankService.MarkTankFault(tank);
            _alarmService.RaiseAlarm(tank.Name, "Critical", "Manual fault set by " + _sessionUser.Username);
            RefreshDashboard();
        }

        private void TankControl_ResetClicked(object sender, TankControlEventArgs e)
        {
            if (!RequirePermission(CanOperatePlant(), "Only Admin, Supervisor, and Operator users can reset tanks."))
            {
                return;
            }

            var tank = FindTank(e.TankId);
            _tankService.ResetTank(tank);
            _alarmService.ResetActiveAlarms(_sessionUser.Username);
            RefreshDashboard();
        }

        private void TankControl_RemoveClicked(object sender, TankControlEventArgs e)
        {
            if (!RequirePermission(CanEngineer(), "Only Admin and Supervisor users can remove tanks."))
            {
                return;
            }

            var tank = FindTank(e.TankId);
            if (MessageBox.Show("Remove " + tank.Name + "?", Constants.ApplicationName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                _tankService.DeleteTank(tank.Id);
                _selectedTank = null;
                RefreshDashboard();
            }
        }

        private TankModel FindTank(int tankId)
        {
            foreach (var tank in _tankService.GetTanks())
            {
                if (tank.Id == tankId)
                {
                    return tank;
                }
            }

            throw new InvalidOperationException("Tank not found: " + tankId);
        }

        private void RaiseStateAlarmIfNeeded(TankModel tank, string previousStatus)
        {
            if (string.Equals(tank.Status, previousStatus, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (string.Equals(tank.Status, Constants.StatusFault, StringComparison.OrdinalIgnoreCase))
            {
                _alarmService.RaiseAlarm(tank.Name, "Critical", "Tank entered fault state.");
            }
            else if (string.Equals(tank.Status, Constants.StatusWarning, StringComparison.OrdinalIgnoreCase))
            {
                _alarmService.RaiseAlarm(tank.Name, "Warning", "Tank entered warning state.");
            }
        }

        private void LoadProcessState()
        {
            _processSteps = _processRecipeService.GetRecipeSteps();
            _hoists = GetSingleMainHoistList(_hoistService.GetHoists());
            _jobs = _jobService.GetActiveJobs();
            _hoistStatus = _hoistStatusService.GetCurrent();
            var primaryHoist = _hoists.Count == 0 ? null : _hoists[0];
            if (primaryHoist != null)
            {
                if (!_hoistVisualInitialized)
                {
                    _hoistVisualIndex = primaryHoist.PositionIndex;
                    _hoistVisualInitialized = true;
                }

                _hoistTargetIndex = primaryHoist.PositionIndex;
                _hoistStatus.CurrentTankId = GetTankIdByNumber(primaryHoist.LineId, primaryHoist.CurrentTankNo);
                _hoistStatus.Status = primaryHoist.Status;
            }
            else
            {
                _hoistVisualIndex = GetTankIndex(_hoistStatus.CurrentTankId);
                _hoistTargetIndex = _hoistVisualIndex;
                _hoistVisualInitialized = false;
            }

            var activeJob = _jobs.Count == 0 ? null : _jobs[0];
            _currentStepIndex = activeJob == null ? 0 : Math.Max(0, activeJob.CurrentStep - 1);
            _remainingStepSeconds = activeJob == null ? 0 : activeJob.RemainingSeconds;
        }

        private void SimulateProcessCycle(List<TankModel> tanks)
        {
            _processSecondTick = !_processSecondTick;
            _hoistService.Tick(_processSteps, _jobs, _autoMode && _cycleRunning, _emergencyStop, _processSecondTick);
            LoadProcessState();
            ApplyRunningTankFromJobs(tanks);

            if (_cycleRunning && _jobs.Count == 0)
            {
                _cycleRunning = false;
                UpdateModeButtons();
            }
        }

        private void ApplyRunningTankFromJobs(List<TankModel> tanks)
        {
            if (!_cycleRunning)
            {
                return;
            }

            foreach (var tank in tanks)
            {
                var occupied = IsTankOccupied(tank.LineId, tank.TankNo);
                if (occupied)
                {
                    tank.Status = Constants.StatusRunning;
                }
            }
        }

        private int? GetCurrentStepTankId()
        {
            if (_processSteps.Count == 0)
            {
                return null;
            }

            var step = _processSteps[Math.Min(_currentStepIndex, _processSteps.Count - 1)];
            return GetTankIdByNumber(step.LineId, step.TankNo);
        }

        private string GetCurrentStepName()
        {
            if (_processSteps.Count == 0)
            {
                return "No Process";
            }

            return _processSteps[Math.Min(_currentStepIndex, _processSteps.Count - 1)].ProcessName;
        }

        private double GetTankIndex(int? tankId)
        {
            if (!tankId.HasValue)
            {
                return 0;
            }

            var tanks = _tankService.GetTanks();
            for (var i = 0; i < tanks.Count; i++)
            {
                if (tanks[i].Id == tankId.Value)
                {
                    return i;
                }
            }

            return 0;
        }

        private void MoveHoistTowardTarget()
        {
            var distance = _hoistTargetIndex - _hoistVisualIndex;
            var step = 0.18;

            if (Math.Abs(distance) <= step)
            {
                _hoistVisualIndex = _hoistTargetIndex;
                return;
            }

            _hoistVisualIndex += distance > 0 ? step : -step;
        }

        private int? GetTankIdByNumber(int lineId, int tankNo)
        {
            foreach (var tank in _tankService.GetTanks())
            {
                if (tank.LineId == lineId && tank.TankNo == tankNo)
                {
                    return tank.Id;
                }
            }

            return null;
        }

        private JobModel FindJob(int? jobId)
        {
            if (!jobId.HasValue)
            {
                return null;
            }

            foreach (var job in _jobs)
            {
                if (job.JobId == jobId.Value)
                {
                    return job;
                }
            }

            return null;
        }

        private bool IsTankOccupied(int lineId, int tankNo)
        {
            foreach (var job in _jobs)
            {
                if (job.LineId == lineId && job.CurrentTank == tankNo && IsOccupyingJobStatus(job.Status))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsOccupyingJobStatus(string status)
        {
            return string.Equals(status, "Lowering", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(status, "Processing", StringComparison.OrdinalIgnoreCase);
        }

        private void ApplyTankOccupancyStatus(TankModel tank)
        {
            if (_cycleRunning && IsTankOccupied(tank.LineId, tank.TankNo))
            {
                tank.Status = Constants.StatusRunning;
            }
        }

        private void RaiseProcessAlarmIfNeeded(TankModel tank)
        {
            if (tank.TemperatureCelsius > 70)
            {
                _alarmService.RaiseAlarm(tank.Name, "Warning", "High temperature detected.");
            }

            if (tank.CapacityLiters > 0 && tank.CurrentLevelLiters < tank.CapacityLiters * 0.35)
            {
                _alarmService.RaiseAlarm(tank.Name, "Warning", "Low chemical level detected.");
            }

            if (_cycleRunning && _remainingStepSeconds > 0)
            {
                var currentStep = _processSteps.Count == 0 ? null : _processSteps[Math.Min(_currentStepIndex, _processSteps.Count - 1)];
                if (currentStep != null && tank.Id == currentStep.TankId && _remainingStepSeconds > currentStep.DurationSeconds + 5)
                {
                    _alarmService.RaiseAlarm(tank.Name, "Warning", "Process timeout detected.");
                }
            }
        }

        private void UpdateModeButtons()
        {
            var canProduce = CanRunProduction();
            btnAutoMode.Checked = _autoMode;
            btnManualMode.Checked = !_autoMode;
            btnEmergencyStop.Checked = _emergencyStop;
            btnAutoMode.Enabled = canProduce;
            btnManualMode.Enabled = canProduce;
            btnNewJob.Enabled = canProduce && !_emergencyStop;
            var anyStoppedLine = HasAnyStartableLine();
            var anyRunningJob = HasAnyRunningJob();
            btnStartCycle.Enabled = canProduce && !_emergencyStop && anyStoppedLine;
            btnStartLine1Job.Enabled = false;
            btnStopLine1Job.Enabled = false;
            btnStopCycle.Enabled = canProduce && anyRunningJob;
            btnPauseRecipe.Enabled = canProduce && anyRunningJob;
        }

        private bool IsLineJobRunning(int lineId)
        {
            foreach (var job in _jobs)
            {
                if (job.LineId == lineId &&
                    !string.Equals(job.Status, "Complete", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(job.Status, "Paused", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private bool HasAnyStartableLine()
        {
            return HasRecipeForLine(1) && HasHoistForLine(1) && !IsLineJobRunning(1);
        }

        private bool HasAnyRunningJob()
        {
            foreach (var job in _jobs)
            {
                if (!string.Equals(job.Status, "Complete", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(job.Status, "Paused", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private bool HasRecipeForLine(int lineId)
        {
            foreach (var step in _processSteps)
            {
                if (step.LineId == lineId)
                {
                    return true;
                }
            }

            return false;
        }

        private bool HasHoistForLine(int lineId)
        {
            foreach (var hoist in _hoists)
            {
                if (hoist.LineId == lineId)
                {
                    return true;
                }
            }

            return false;
        }

        private static List<HoistModel> GetSingleMainHoistList(IEnumerable<HoistModel> hoists)
        {
            if (hoists == null)
            {
                return new List<HoistModel>();
            }

            return hoists
                .Where(hoist => hoist.LineId <= 0 || hoist.LineId == 1)
                .OrderBy(hoist => string.Equals(hoist.HoistName, "H1", StringComparison.OrdinalIgnoreCase) ? 0 : 1)
                .ThenBy(hoist => hoist.HoistId)
                .Take(1)
                .ToList();
        }

        private void ConfigureAddRowAccess()
        {
            var allowed = false;
            btnAddRow.Available = allowed;
            btnAddRow.Visible = allowed;
            btnAddRow.Enabled = allowed;
            UpdateAddRowText();
        }

        private void UpdateAddRowText()
        {
            btnAddRow.Text = "Tank Count";
        }

        private void UpdateHeaderIndicators(PlantStatusModel status)
        {
            lblClock.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            if (status != null)
            {
                lblPlantStatus.Text = status.ActiveAlarmCount > 0 ? "Plant: Alarm" : "Plant: Normal";
            }
        }

        private void HideLegacyTopNavigation()
        {
            toolStrip.Visible = false;
            toolStrip.Height = 0;
            tabs.Controls.Remove(tabWagons);
            tabs.Controls.Remove(tabRectifiers);
            tabs.Controls.Remove(tabAlarms);
            tabs.Appearance = TabAppearance.FlatButtons;
            tabs.ItemSize = new System.Drawing.Size(1, 1);
            tabs.SizeMode = TabSizeMode.Fixed;
        }
    }
}
