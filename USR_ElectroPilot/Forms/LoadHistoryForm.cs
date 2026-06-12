using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using USR_ElectroPilot.Helpers;
using USR_ElectroPilot.Models;
using USR_ElectroPilot.Services;

namespace USR_ElectroPilot.Forms
{
    public partial class LoadHistoryForm : Form
    {
        private readonly LoadService _loadService = new LoadService();
        private List<LoadModel> _loads = new List<LoadModel>();

        public LoadHistoryForm()
        {
            InitializeComponent();
        }

        private void LoadHistoryForm_Load(object sender, EventArgs e)
        {
            UiHelper.ApplyDarkTheme(this);
            cboStatus.Items.AddRange(new object[] { "All", "Queued", "Running", "Complete", "Completed", "Cancelled", "Fault" });
            cboStatus.SelectedIndex = 0;
            dtFrom.Value = DateTime.Today.AddDays(-7);
            dtTo.Value = DateTime.Today.AddDays(1).AddSeconds(-1);
            RefreshLoads();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            RefreshLoads();
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void RefreshLoads()
        {
            _loads = _loadService.GetLoads();
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            IEnumerable<LoadModel> query = _loads;

            if (!string.IsNullOrWhiteSpace(txtLoadNumber.Text))
            {
                var loadNumber = txtLoadNumber.Text.Trim();
                query = query.Where(l => l.LoadNumber != null && l.LoadNumber.IndexOf(loadNumber, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (cboStatus.Text != "All")
            {
                query = query.Where(l => string.Equals(l.Status, cboStatus.Text, StringComparison.OrdinalIgnoreCase));
            }

            query = query.Where(l => l.CreatedAt >= dtFrom.Value && l.CreatedAt <= dtTo.Value);
            gridLoads.DataSource = query.OrderByDescending(l => l.CreatedAt).ToList();
            FormatGrid();
            ApplyRowColors();
        }

        private void FormatGrid()
        {
            if (gridLoads.Columns["Id"] != null)
            {
                gridLoads.Columns["Id"].Width = 55;
            }

            if (gridLoads.Columns["CreatedAt"] != null)
            {
                gridLoads.Columns["CreatedAt"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm";
            }

            if (gridLoads.Columns["StartedAt"] != null)
            {
                gridLoads.Columns["StartedAt"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm";
            }

            if (gridLoads.Columns["CompletedAt"] != null)
            {
                gridLoads.Columns["CompletedAt"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm";
            }
        }

        private void ApplyRowColors()
        {
            foreach (DataGridViewRow row in gridLoads.Rows)
            {
                var status = Convert.ToString(row.Cells["Status"].Value);

                if (string.Equals(status, "Fault", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(status, "Cancelled", StringComparison.OrdinalIgnoreCase))
                {
                    row.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(110, 40, 40);
                    row.DefaultCellStyle.ForeColor = System.Drawing.Color.White;
                }
                else if (string.Equals(status, "Running", StringComparison.OrdinalIgnoreCase))
                {
                    row.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(45, 85, 95);
                    row.DefaultCellStyle.ForeColor = System.Drawing.Color.White;
                }
                else if (string.Equals(status, "Complete", StringComparison.OrdinalIgnoreCase) ||
                         string.Equals(status, "Completed", StringComparison.OrdinalIgnoreCase))
                {
                    row.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(45, 85, 55);
                    row.DefaultCellStyle.ForeColor = System.Drawing.Color.White;
                }
            }
        }
    }
}
