using System;
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
        private TankModel _selectedTank;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            UiHelper.ApplyDarkTheme(this);
            CsvExporter.AddExportButton(alarmGrid, "dashboard_alarms");
            Text = Constants.ApplicationName + " - " + AppSession.Username;
            lblUser.Text = AppSession.Username + " (" + AppSession.Role + ")";
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

        private void RefreshDashboard()
        {
            try
            {
                var status = _dashboardService.GetPlantStatus();
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
            var tanks = new System.Collections.Generic.List<TankModel>();
            foreach (Control control in pnlTanks.Controls)
            {
                var tankControl = control as TankControl;
                if (tankControl != null && tankControl.Tank != null)
                {
                    tanks.Add(tankControl.Tank);
                }
            }

            var rectifiers = new System.Collections.Generic.List<RectifierModel>();
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

            pnlTanks.Invalidate(true);
            pnlRectifiers.Invalidate(true);
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
            pnlTanks.Controls.Clear();
            foreach (var tank in _tankService.GetTanks())
            {
                var tankControl = new TankControl { Tank = tank, Margin = new Padding(8) };
                tankControl.Click += delegate { _selectedTank = tank; };
                pnlTanks.Controls.Add(tankControl);
            }
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
    }
}
