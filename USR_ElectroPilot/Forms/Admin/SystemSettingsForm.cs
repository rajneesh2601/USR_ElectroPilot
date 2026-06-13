using System;
using System.Windows.Forms;
using USR_ElectroPilot.Data;
using USR_ElectroPilot.Helpers;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Forms.Admin
{
    public partial class SystemSettingsForm : Form
    {
        private readonly SystemSettingRepository _settingsRepository = new SystemSettingRepository();

        public SystemSettingsForm()
        {
            InitializeComponent();
        }

        private void SystemSettingsForm_Load(object sender, EventArgs e)
        {
            UiHelper.ApplyDarkTheme(this);
            CsvExporter.AddExportButton(gridSettings, "system_settings");
            RefreshSettings();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            RefreshSettings();
        }

        private void BtnNew_Click(object sender, EventArgs e)
        {
            txtKey.ReadOnly = false;
            txtKey.Clear();
            txtValue.Clear();
            txtDescription.Clear();
            txtKey.Focus();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtKey.Text))
            {
                MessageBox.Show("Setting key is required.", Constants.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _settingsRepository.Upsert(new SystemSettingModel
            {
                SettingKey = txtKey.Text.Trim(),
                SettingValue = txtValue.Text,
                Description = txtDescription.Text
            });

            RefreshSettings();
        }

        private void GridSettings_SelectionChanged(object sender, EventArgs e)
        {
            if (gridSettings.CurrentRow == null)
            {
                return;
            }

            var setting = gridSettings.CurrentRow.DataBoundItem as SystemSettingModel;
            if (setting == null)
            {
                return;
            }

            txtKey.ReadOnly = true;
            txtKey.Text = setting.SettingKey;
            txtValue.Text = setting.SettingValue;
            txtDescription.Text = setting.Description;
        }

        private void RefreshSettings()
        {
            DatabaseHelper.InitializeDatabase();
            gridSettings.DataSource = _settingsRepository.GetAll();
        }
    }
}
