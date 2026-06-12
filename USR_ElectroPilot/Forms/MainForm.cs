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

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            UiHelper.ApplyDarkTheme(this);
            Text = Constants.ApplicationName + " - " + AppSession.Username;
            lblUser.Text = AppSession.Username + " (" + AppSession.Role + ")";
            RefreshDashboard();
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

        private void RefreshDashboard()
        {
            try
            {
                var status = _dashboardService.GetPlantStatus();
                LoadStatusCards(status);
                LoadTanks();
                LoadWagons();
                LoadRectifiers();
            }
            catch (Exception ex)
            {
                Logger.Error("Dashboard refresh failed", ex);
                MessageBox.Show("Dashboard refresh failed. Check Logs folder.", Constants.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                pnlTanks.Controls.Add(new TankControl { Tank = tank, Margin = new Padding(8) });
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
    }
}
