using System;
using System.Collections.Generic;
using System.Windows.Forms;
using USR_ElectroPilot.Data;
using USR_ElectroPilot.Helpers;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Forms.Admin
{
    public partial class UserManagementForm : Form
    {
        private readonly UserRepository _userRepository = new UserRepository();
        private UserModel _selectedUser;

        public UserManagementForm()
        {
            InitializeComponent();
        }

        private void UserManagementForm_Load(object sender, EventArgs e)
        {
            UiHelper.ApplyDarkTheme(this);
            CsvExporter.AddExportButton(gridUsers, "users");
            cboRole.Items.AddRange(new object[]
            {
                Constants.RoleAdmin,
                Constants.RoleSupervisor,
                Constants.RoleOperator,
                Constants.RoleViewer
            });
            cboRole.SelectedIndex = 2;
            RefreshUsers();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            RefreshUsers();
        }

        private void BtnNew_Click(object sender, EventArgs e)
        {
            _selectedUser = null;
            txtUsername.ReadOnly = false;
            txtUsername.Clear();
            txtDisplayName.Clear();
            txtPassword.Clear();
            cboRole.SelectedIndex = 2;
            chkActive.Checked = true;
            txtUsername.Focus();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateUserInput())
            {
                return;
            }

            if (_selectedUser == null)
            {
                var user = new UserModel
                {
                    Username = txtUsername.Text.Trim(),
                    DisplayName = txtDisplayName.Text.Trim(),
                    PasswordHash = PasswordHelper.HashPassword(txtPassword.Text),
                    Role = cboRole.Text,
                    IsActive = chkActive.Checked
                };

                _userRepository.Add(user);
            }
            else
            {
                _selectedUser.DisplayName = txtDisplayName.Text.Trim();
                _selectedUser.Role = cboRole.Text;
                _selectedUser.IsActive = chkActive.Checked;

                if (!string.IsNullOrEmpty(txtPassword.Text))
                {
                    _selectedUser.PasswordHash = PasswordHelper.HashPassword(txtPassword.Text);
                    _selectedUser.FailedLoginCount = 0;
                    _selectedUser.LockoutUntil = null;
                }

                _userRepository.Update(_selectedUser);
            }

            RefreshUsers();
            BtnNew_Click(sender, e);
        }

        private void GridUsers_SelectionChanged(object sender, EventArgs e)
        {
            if (gridUsers.CurrentRow == null)
            {
                return;
            }

            _selectedUser = gridUsers.CurrentRow.DataBoundItem as UserModel;
            if (_selectedUser == null)
            {
                return;
            }

            txtUsername.ReadOnly = true;
            txtUsername.Text = _selectedUser.Username;
            txtDisplayName.Text = _selectedUser.DisplayName;
            txtPassword.Clear();
            cboRole.Text = _selectedUser.Role;
            chkActive.Checked = _selectedUser.IsActive;
        }

        private void RefreshUsers()
        {
            DatabaseHelper.InitializeDatabase();
            var users = new List<UserModel>(_userRepository.GetAll());
            gridUsers.DataSource = users;

            if (gridUsers.Columns["PasswordHash"] != null)
            {
                gridUsers.Columns["PasswordHash"].Visible = false;
            }
        }

        private bool ValidateUserInput()
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Username is required.", Constants.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            if (_selectedUser == null && string.IsNullOrEmpty(txtPassword.Text))
            {
                MessageBox.Show("Password is required for new users.", Constants.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtDisplayName.Text))
            {
                MessageBox.Show("Display name is required.", Constants.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            return true;
        }
    }
}
