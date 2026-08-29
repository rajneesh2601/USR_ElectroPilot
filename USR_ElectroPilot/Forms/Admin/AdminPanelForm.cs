using System.Windows.Forms;
using USR_ElectroPilot.Helpers;

namespace USR_ElectroPilot.Forms.Admin
{
    public partial class AdminPanelForm : Form
    {
        public AdminPanelForm()
        {
            InitializeComponent();
        }

        private void AdminPanelForm_Load(object sender, System.EventArgs e)
        {
            UiHelper.ApplyDarkTheme(this);
            LoadTab(tabUsers, new UserManagementForm());
            LoadTab(tabActivity, new UserActivityForm());
            LoadTab(tabAudit, new AuditLogForm());
            LoadTab(tabSettings, new SystemSettingsForm());
            LoadTab(tabAlarms, new AlarmHistoryForm());
            LoadTab(tabReports, new LoadHistoryForm());
        }

        private static void LoadTab(TabPage tab, Form form)
        {
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            form.MinimumSize = System.Drawing.Size.Empty;
            tab.Controls.Clear();
            tab.Controls.Add(form);
            form.Show();
        }
    }
}
