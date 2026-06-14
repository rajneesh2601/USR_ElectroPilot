using System;
using System.Collections.Generic;
using System.Windows.Forms;
using USR_ElectroPilot.Controls;
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
        private ScadaOverviewControl _scadaOverview;
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
        private int _scadaTankRows = 1;
        private bool _processSecondTick;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            UiHelper.ApplyDarkTheme(this);
            CsvExporter.AddExportButton(alarmGrid, "dashboard_alarms");
            UiHelper.ApplyRoleRestrictions(btnAddTank, btnEditTank, btnRemoveTank);
            Text = Constants.ApplicationName + " - " + AppSession.Username;
            lblUser.Text = AppSession.Username + " (" + AppSession.Role + ")";
            _scadaTankRows = _scadaLayoutService.GetTankRows();
            ConfigureAddRowAccess();
            LoadProcessState();
            UpdateModeButtons();
            UpdateHeaderIndicators(null);
            RefreshDashboard();
            simulatorTimer.Start();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            RefreshDashboard();
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            new AuthService().Logout();
            Close();
        }

        private void BtnAddTank_Click(object sender, EventArgs e)
        {
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
            if (_scadaTankRows >= 4)
            {
                MessageBox.Show("Maximum SCADA tank rows reached.", Constants.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _scadaTankRows = _scadaLayoutService.AddTankRow();
            UpdateAddRowText();
            RefreshDashboard();
        }

        private void BtnAutoMode_Click(object sender, EventArgs e)
        {
            _autoMode = true;
            UpdateModeButtons();
            RefreshDashboard();
        }

        private void BtnManualMode_Click(object sender, EventArgs e)
        {
            _autoMode = false;
            _cycleRunning = false;
            _hoistStatusService.Save(GetCurrentStepTankId(), "Idle");
            LoadProcessState();
            UpdateModeButtons();
            RefreshDashboard();
        }

        private void BtnStartCycle_Click(object sender, EventArgs e)
        {
            if (_emergencyStop)
            {
                MessageBox.Show("Reset emergency stop before starting cycle.", Constants.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _autoMode = true;
            _cycleRunning = true;
            _jobService.StartJob(_processSteps);
            LoadProcessState();
            UpdateModeButtons();
            RefreshDashboard();
        }

        private void BtnStopCycle_Click(object sender, EventArgs e)
        {
            _cycleRunning = false;
            _hoistService.StopAll();
            LoadProcessState();
            UpdateModeButtons();
            RefreshDashboard();
        }

        private void BtnEmergencyStop_Click(object sender, EventArgs e)
        {
            _emergencyStop = true;
            _cycleRunning = false;
            _hoistService.EmergencyStop();
            _hoistStatusService.Save(GetCurrentStepTankId(), "EmergencyStop");
            _alarmService.RaiseAlarm("Hoist", "Critical", "Emergency stop activated.");
            LoadProcessState();
            UpdateModeButtons();
            RefreshDashboard();
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            _emergencyStop = false;
            _cycleRunning = false;
            _currentStepIndex = 0;
            _remainingStepSeconds = 0;
            _alarmService.ResetActiveAlarms();
            _hoistService.StopAll();
            _hoistStatusService.Save(GetCurrentStepTankId(), "Idle");
            LoadProcessState();
            UpdateModeButtons();
            RefreshDashboard();
        }

        private void BtnStartAll_Click(object sender, EventArgs e)
        {
            _tankService.StartAllTanks();
            RefreshDashboard();
        }

        private void BtnStopAll_Click(object sender, EventArgs e)
        {
            _tankService.StopAllTanks();
            RefreshDashboard();
        }

        private void BtnResetAlarms_Click(object sender, EventArgs e)
        {
            _alarmService.ResetActiveAlarms();

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
                _tankService.UpdateTank(tank);
                _tankHistoryService.RecordSnapshot(tank);
                RaiseStateAlarmIfNeeded(tank, previousStatuses.ContainsKey(tank.Id) ? previousStatuses[tank.Id] : string.Empty);
                RaiseProcessAlarmIfNeeded(tank);
            }

            RefreshLiveDashboard();
        }

        private void RefreshLiveDashboard()
        {
            var status = _dashboardService.GetPlantStatus();
            UpdateHeaderIndicators(status);
            LoadStatusCards(status);
            LoadTanks();
            pnlRectifiers.Invalidate(true);
            LoadAlarms();
        }

        private void LoadStatusCards(PlantStatusModel status)
        {
            pnlStatus.Controls.Clear();
            pnlStatus.Controls.Add(CreateStatusCard("Total Tanks", status.TotalTankCount.ToString(), "Configured"));
            pnlStatus.Controls.Add(CreateStatusCard("Running Tanks", status.RunningTankCount.ToString(), "Running"));
            pnlStatus.Controls.Add(CreateStatusCard("Fault Tanks", status.FaultTankCount.ToString(), "Fault"));
            pnlStatus.Controls.Add(CreateStatusCard("Process Step", status.CurrentProcessStep, _remainingStepSeconds > 0 ? _remainingStepSeconds + " sec left" : "Ready"));
            pnlStatus.Controls.Add(CreateStatusCard("Hoist Position", status.HoistPosition, status.HoistState));
            pnlStatus.Controls.Add(CreateStatusCard("Active Alarms", status.ActiveAlarmCount.ToString(), "Active"));
        }

        private StatusCardControl CreateStatusCard(string title, string value, string caption)
        {
            return new StatusCardControl
            {
                Title = title,
                Value = value,
                Caption = caption,
                Margin = new Padding(8)
            };
        }

        private void LoadTanks()
        {
            if (_scadaOverview == null)
            {
                _scadaOverview = new ScadaOverviewControl { Dock = DockStyle.Fill };
                _scadaOverview.TankSelected += ScadaOverview_TankSelected;
                _scadaOverview.StartClicked += TankControl_StartClicked;
                _scadaOverview.StopClicked += TankControl_StopClicked;
                _scadaOverview.FaultClicked += TankControl_FaultClicked;
                _scadaOverview.ResetClicked += TankControl_ResetClicked;
                _scadaOverview.RemoveClicked += TankControl_RemoveClicked;
                pnlTanks.Controls.Add(_scadaOverview);
            }

            _scadaOverview.BindData(
                _tankService.GetTanks(),
                _wagonService.GetWagons(),
                _processSteps,
                _hoists,
                _jobs,
                _hoistStatus,
                _hoistVisualIndex,
                GetCurrentStepName(),
                _remainingStepSeconds,
                _autoMode,
                _emergencyStop,
                _scadaTankRows);
            if (_selectedTank != null)
            {
                _scadaOverview.SelectTank(_selectedTank.Id);
            }
        }

        private void ScadaOverview_TankSelected(object sender, TankControlEventArgs e)
        {
            _selectedTank = FindTank(e.TankId);
        }

        private void LoadWagons()
        {
            pnlWagons.Controls.Clear();
            foreach (var hoist in _hoistService.GetHoists())
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
            var tank = FindTank(e.TankId);
            _tankService.StartTank(tank);
            RefreshDashboard();
        }

        private void TankControl_StopClicked(object sender, TankControlEventArgs e)
        {
            var tank = FindTank(e.TankId);
            _tankService.StopTank(tank);
            RefreshDashboard();
        }

        private void TankControl_FaultClicked(object sender, TankControlEventArgs e)
        {
            var tank = FindTank(e.TankId);
            _tankService.MarkTankFault(tank);
            _alarmService.RaiseAlarm(tank.Name, "Critical", "Manual fault set by " + AppSession.Username);
            RefreshDashboard();
        }

        private void TankControl_ResetClicked(object sender, TankControlEventArgs e)
        {
            var tank = FindTank(e.TankId);
            _tankService.ResetTank(tank);
            _alarmService.ResetActiveAlarms();
            RefreshDashboard();
        }

        private void TankControl_RemoveClicked(object sender, TankControlEventArgs e)
        {
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
            _hoists = _hoistService.GetHoists();
            _jobs = _jobService.GetActiveJobs();
            _hoistStatus = _hoistStatusService.GetCurrent();
            var primaryHoist = _hoists.Count == 0 ? null : _hoists[0];
            if (primaryHoist != null)
            {
                _hoistVisualIndex = primaryHoist.PositionIndex;
                _hoistTargetIndex = Math.Max(0, primaryHoist.TargetTankNo - 1);
                _hoistStatus.CurrentTankId = GetTankIdByNumber(primaryHoist.CurrentTankNo);
                _hoistStatus.Status = primaryHoist.Status;
            }
            else
            {
                _hoistVisualIndex = GetTankIndex(_hoistStatus.CurrentTankId);
                _hoistTargetIndex = _hoistVisualIndex;
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
            foreach (var tank in tanks)
            {
                var occupied = IsTankOccupied(tank.TankNumber);
                if (occupied)
                {
                    tank.Status = Constants.StatusRunning;
                }
                else if (string.Equals(tank.Status, Constants.StatusRunning, StringComparison.OrdinalIgnoreCase))
                {
                    tank.Status = Constants.StatusNormal;
                }
            }
        }

        private int? GetCurrentStepTankId()
        {
            if (_processSteps.Count == 0)
            {
                return null;
            }

            return GetTankIdByNumber(_processSteps[Math.Min(_currentStepIndex, _processSteps.Count - 1)].TankNo);
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

        private int? GetTankIdByNumber(int tankNo)
        {
            foreach (var tank in _tankService.GetTanks())
            {
                if (tank.TankNumber == tankNo)
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

        private bool IsTankOccupied(int tankNo)
        {
            foreach (var job in _jobs)
            {
                if (job.CurrentTank == tankNo && !string.Equals(job.Status, "Moving", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private void ApplyTankOccupancyStatus(TankModel tank)
        {
            if (IsTankOccupied(tank.TankNumber))
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
            btnAutoMode.Checked = _autoMode;
            btnManualMode.Checked = !_autoMode;
            btnEmergencyStop.Checked = _emergencyStop;
            btnStartCycle.Enabled = !_emergencyStop;
            btnStopCycle.Enabled = _cycleRunning;
        }

        private void ConfigureAddRowAccess()
        {
            var allowed = AppSession.HasRole(Constants.RoleAdmin);
            btnAddRow.Available = allowed;
            btnAddRow.Visible = allowed;
            btnAddRow.Enabled = allowed;
            UpdateAddRowText();
        }

        private void UpdateAddRowText()
        {
            btnAddRow.Text = "Add Row (" + _scadaTankRows + ")";
        }

        private void UpdateHeaderIndicators(PlantStatusModel status)
        {
            lblClock.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            if (status != null)
            {
                lblPlantStatus.Text = status.ActiveAlarmCount > 0 ? "Plant: Alarm" : "Plant: Normal";
            }
        }
    }
}
