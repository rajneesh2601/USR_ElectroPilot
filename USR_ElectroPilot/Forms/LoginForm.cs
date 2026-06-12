using System;
using System.Windows.Forms;
using USR_ElectroPilot.Helpers;
using USR_ElectroPilot.Services;

namespace USR_ElectroPilot.Forms
{
    public partial class LoginForm : Form
    {
        private readonly AuthService _authService = new AuthService();

        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            UiHelper.ApplyDarkTheme(this);
            txtUsername.Focus();
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            TryLogin();
        }

        private void TxtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                TryLogin();
            }
        }

        private void TryLogin()
        {
            string message;
            var username = txtUsername.Text.Trim();
            var password = txtPassword.Text;

            btnLogin.Enabled = false;
            lblStatus.Text = "Checking credentials...";

            try
            {
                if (_authService.Login(username, password, out message))
                {
                    DialogResult = DialogResult.OK;
                    Close();
                    return;
                }

                lblStatus.Text = message;
                txtPassword.SelectAll();
                txtPassword.Focus();
            }
            catch (Exception ex)
            {
                Logger.Error("Login failed unexpectedly", ex);
                lblStatus.Text = "Login failed. Check Logs folder.";
            }
            finally
            {
                btnLogin.Enabled = true;
            }
        }
    }
}
