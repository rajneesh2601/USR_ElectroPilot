using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using USR_ElectroPilot.Helpers;
using USR_ElectroPilot.Models;
using USR_ElectroPilot.Services;

namespace USR_ElectroPilot.Forms
{
    public partial class AlarmHistoryForm : Form
    {
        private readonly AlarmService _alarmService = new AlarmService();
        private List<AlarmModel> _alarms = new List<AlarmModel>();

        public AlarmHistoryForm()
        {
            InitializeComponent();
        }

        private void AlarmHistoryForm_Load(object sender, EventArgs e)
        {
            UiHelper.ApplyDarkTheme(this);
            cboSeverity.Items.AddRange(new object[] { "All", "Info", "Warning", "Critical" });
            cboState.Items.AddRange(new object[] { "All", Constants.AlarmActive, Constants.AlarmAcknowledged, Constants.AlarmShelved });
            cboSeverity.SelectedIndex = 0;
            cboState.SelectedIndex = 0;
            dtFrom.Value = DateTime.Today.AddDays(-7);
            dtTo.Value = DateTime.Today.AddDays(1).AddSeconds(-1);
            RefreshAlarms();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            RefreshAlarms();
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void RefreshAlarms()
        {
            _alarms = _alarmService.GetAlarms();
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            IEnumerable<AlarmModel> query = _alarms;

            if (!string.IsNullOrWhiteSpace(txtSource.Text))
            {
                query = query.Where(a => a.Source != null && a.Source.IndexOf(txtSource.Text.Trim(), StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (cboSeverity.Text != "All")
            {
                query = query.Where(a => string.Equals(a.Severity, cboSeverity.Text, StringComparison.OrdinalIgnoreCase));
            }

            if (cboState.Text != "All")
            {
                query = query.Where(a => string.Equals(a.State, cboState.Text, StringComparison.OrdinalIgnoreCase));
            }

            query = query.Where(a => a.RaisedAt >= dtFrom.Value && a.RaisedAt <= dtTo.Value);
            gridAlarms.DataSource = query.OrderByDescending(a => a.RaisedAt).ToList();
            ApplyRowColors();
        }

        private void ApplyRowColors()
        {
            foreach (DataGridViewRow row in gridAlarms.Rows)
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
