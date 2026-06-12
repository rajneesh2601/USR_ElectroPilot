using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using USR_ElectroPilot.Data;
using USR_ElectroPilot.Helpers;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Forms.Admin
{
    public partial class AuditLogForm : Form
    {
        private readonly AuditRepository _auditRepository = new AuditRepository();
        private List<AuditLogModel> _logs = new List<AuditLogModel>();

        public AuditLogForm()
        {
            InitializeComponent();
        }

        private void AuditLogForm_Load(object sender, EventArgs e)
        {
            UiHelper.ApplyDarkTheme(this);
            dtFrom.Value = DateTime.Today.AddDays(-30);
            dtTo.Value = DateTime.Today.AddDays(1).AddSeconds(-1);
            RefreshLogs();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            RefreshLogs();
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void RefreshLogs()
        {
            DatabaseHelper.InitializeDatabase();
            _logs = _auditRepository.GetAll();
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            IEnumerable<AuditLogModel> query = _logs;

            if (!string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                query = query.Where(l => l.Username != null && l.Username.IndexOf(txtUsername.Text.Trim(), StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (!string.IsNullOrWhiteSpace(txtAction.Text))
            {
                query = query.Where(l => l.Action != null && l.Action.IndexOf(txtAction.Text.Trim(), StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (!string.IsNullOrWhiteSpace(txtEntity.Text))
            {
                query = query.Where(l => l.EntityType != null && l.EntityType.IndexOf(txtEntity.Text.Trim(), StringComparison.OrdinalIgnoreCase) >= 0);
            }

            query = query.Where(l => l.CreatedAt >= dtFrom.Value && l.CreatedAt <= dtTo.Value);
            gridAudit.DataSource = query.OrderByDescending(l => l.CreatedAt).ToList();
        }
    }
}
