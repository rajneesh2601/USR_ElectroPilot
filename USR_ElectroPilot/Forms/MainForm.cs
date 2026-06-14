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
        private ScadaOverviewControl _scadaOverview;
        private TankModel _selectedTank;

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

            foreach (var tank in tanks)
            {
                _tankService.UpdateTank(tank);
                _tankHistoryService.RecordSnapshot(tank);
                RaiseStateAlarmIfNeeded(tank, previousStatuses.ContainsKey(tank.Id) ? previousStatuses[tank.Id] : string.Empty);
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
            pnlStatus.Controls.Add(CreateStatusCard("Tanks", status.ActiveTankCount.ToString(), "Active"));
            pnlStatus.Controls.Add(CreateStatusCard("Alarms", status.ActiveAlarmCount.ToString(), "Active"));
            pnlStatus.Controls.Add(CreateStatusCard("Rectifiers", status.RunningRectifierCount.ToString(), "Running"));
            pnlStatus.Controls.Add(CreateStatusCard("Loads", status.QueuedLoadCount.ToString(), "Queued"));
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

            _scadaOverview.BindData(_tankService.GetTanks(), _wagonService.GetWagons());
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
