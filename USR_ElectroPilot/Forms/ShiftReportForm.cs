using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using USR_ElectroPilot.Helpers;
using USR_ElectroPilot.Models;
using USR_ElectroPilot.Services;

namespace USR_ElectroPilot.Forms
{
    public partial class ShiftReportForm : Form
    {
        private readonly ShiftReportService _shiftReportService = new ShiftReportService();
        private List<ShiftReportModel> _reports = new List<ShiftReportModel>();

        public ShiftReportForm()
        {
            InitializeComponent();
        }

        private void ShiftReportForm_Load(object sender, EventArgs e)
        {
            UiHelper.ApplyDarkTheme(this);
            dtFilterFrom.Value = DateTime.Today.AddDays(-7);
            dtFilterTo.Value = DateTime.Today.AddDays(1).AddSeconds(-1);
            dtStarted.Value = DateTime.Now;
            dtEnded.Value = DateTime.Now;
            txtOperator.Text = AppSession.Username;
            RefreshReports();
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            RefreshReports();
        }

        private void BtnNew_Click(object sender, EventArgs e)
        {
            ClearEditor();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateReport())
            {
                return;
            }

            var report = new ShiftReportModel
            {
                ShiftName = txtShiftName.Text.Trim(),
                OperatorUsername = txtOperator.Text.Trim(),
                StartedAt = dtStarted.Value,
                EndedAt = dtEnded.Checked ? (DateTime?)dtEnded.Value : null,
                TotalLoads = Convert.ToInt32(numTotalLoads.Value),
                AlarmCount = Convert.ToInt32(numAlarmCount.Value),
                Notes = txtNotes.Text.Trim()
            };

            _shiftReportService.AddShiftReport(report);
            RefreshReports();
            ClearEditor();
            MessageBox.Show("Shift report saved.", Constants.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void RefreshReports()
        {
            _reports = _shiftReportService.GetShiftReports();
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            IEnumerable<ShiftReportModel> query = _reports;

            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                var search = txtSearch.Text.Trim();
                query = query.Where(r =>
                    (!string.IsNullOrEmpty(r.ShiftName) && r.ShiftName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (!string.IsNullOrEmpty(r.OperatorUsername) && r.OperatorUsername.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (!string.IsNullOrEmpty(r.Notes) && r.Notes.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0));
            }

            query = query.Where(r => r.StartedAt >= dtFilterFrom.Value && r.StartedAt <= dtFilterTo.Value);
            gridReports.DataSource = query.OrderByDescending(r => r.StartedAt).ToList();
            FormatGrid();
        }

        private void FormatGrid()
        {
            if (gridReports.Columns["Id"] != null)
            {
                gridReports.Columns["Id"].Width = 55;
            }

            if (gridReports.Columns["Notes"] != null)
            {
                gridReports.Columns["Notes"].FillWeight = 160;
            }

            if (gridReports.Columns["StartedAt"] != null)
            {
                gridReports.Columns["StartedAt"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm";
            }

            if (gridReports.Columns["EndedAt"] != null)
            {
                gridReports.Columns["EndedAt"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm";
            }

            if (gridReports.Columns["CreatedAt"] != null)
            {
                gridReports.Columns["CreatedAt"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm";
            }
        }

        private bool ValidateReport()
        {
            if (string.IsNullOrWhiteSpace(txtShiftName.Text))
            {
                MessageBox.Show("Shift name is required.", Constants.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtShiftName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtOperator.Text))
            {
                MessageBox.Show("Operator username is required.", Constants.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtOperator.Focus();
                return false;
            }

            if (dtEnded.Checked && dtEnded.Value < dtStarted.Value)
            {
                MessageBox.Show("End time cannot be earlier than start time.", Constants.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                dtEnded.Focus();
                return false;
            }

            return true;
        }

        private void ClearEditor()
        {
            txtShiftName.Clear();
            txtOperator.Text = AppSession.Username;
            dtStarted.Value = DateTime.Now;
            dtEnded.Value = DateTime.Now;
            dtEnded.Checked = false;
            numTotalLoads.Value = 0;
            numAlarmCount.Value = 0;
            txtNotes.Clear();
            txtShiftName.Focus();
        }
    }
}
