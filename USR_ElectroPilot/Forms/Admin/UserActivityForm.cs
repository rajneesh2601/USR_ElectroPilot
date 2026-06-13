using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using USR_ElectroPilot.Helpers;
using USR_ElectroPilot.Models;
using USR_ElectroPilot.Services;

namespace USR_ElectroPilot.Forms.Admin
{
    public partial class UserActivityForm : Form
    {
        private readonly UserActivityService _userActivityService = new UserActivityService();
        private List<UserActivityModel> _activities = new List<UserActivityModel>();

        public UserActivityForm()
        {
            InitializeComponent();
        }

        private void UserActivityForm_Load(object sender, EventArgs e)
        {
            UiHelper.ApplyDarkTheme(this);
            CsvExporter.AddExportButton(gridActivity, "user_activity");
            dtFrom.Value = DateTime.Today.AddDays(-7);
            dtTo.Value = DateTime.Today.AddDays(1).AddSeconds(-1);
            RefreshActivities();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            RefreshActivities();
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void RefreshActivities()
        {
            _activities = _userActivityService.GetRecent(1000);
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            IEnumerable<UserActivityModel> query = _activities;

            if (!string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                query = query.Where(a => a.Username != null && a.Username.IndexOf(txtUsername.Text.Trim(), StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (!string.IsNullOrWhiteSpace(txtActivityType.Text))
            {
                query = query.Where(a => a.ActivityType != null && a.ActivityType.IndexOf(txtActivityType.Text.Trim(), StringComparison.OrdinalIgnoreCase) >= 0);
            }

            query = query.Where(a => a.CreatedAt >= dtFrom.Value && a.CreatedAt <= dtTo.Value);
            gridActivity.DataSource = query.OrderByDescending(a => a.CreatedAt).ToList();
        }
    }
}
