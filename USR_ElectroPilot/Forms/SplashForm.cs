using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using USR_ElectroPilot.Data;
using USR_ElectroPilot.Helpers;

namespace USR_ElectroPilot.Forms
{
    public partial class SplashForm : Form
    {
        private int _progress;
        private bool _databaseReady;
        private bool _databaseFailed;
        private bool _openingLogin;

        public SplashForm()
        {
            InitializeComponent();
        }

        private void SplashForm_Load(object sender, EventArgs e)
        {
            UiHelper.ApplyDarkTheme(this);
            lblTitle.Text = Constants.ApplicationName;
            lblSubtitle.Text = "Powered By " + Constants.CompanyName;
            lblStatus.Text = "Initializing database...";
            progressTimer.Start();

            Task.Run(new Action(DatabaseHelper.InitializeDatabase)).ContinueWith(task =>
            {
                if (IsDisposed || !IsHandleCreated)
                {
                    return;
                }

                BeginInvoke((MethodInvoker)delegate
                {
                    if (task.IsFaulted)
                    {
                        var ex = task.Exception == null ? null : task.Exception.GetBaseException();
                        Logger.Error("Splash initialization failed", ex);
                        _databaseFailed = true;
                        lblStatus.Text = "Initialization failed. Check Logs folder.";
                        progressTimer.Stop();
                        return;
                    }

                    _databaseReady = true;
                    lblStatus.Text = "Ready";
                });
            });
        }

        private void ProgressTimer_Tick(object sender, EventArgs e)
        {
            if (_databaseFailed || _openingLogin)
            {
                return;
            }

            _progress += _databaseReady ? 20 : 8;
            if (!_databaseReady && _progress > 90)
            {
                _progress = 90;
            }

            if (_progress >= 100)
            {
                _progress = 100;
                progressTimer.Stop();
                lblStatus.Text = "Ready";
                _openingLogin = true;

                DashboardWindowManager.RegisterApplicationHost(this);
                using (var loginForm = new LoginForm())
                {
                    Hide();
                    loginForm.ResetForNextLogin();
                    if (loginForm.ShowDialog() == DialogResult.OK)
                    {
                        DashboardWindowManager.OpenDashboard(loginForm.AuthenticatedUser);
                    }
                    else
                    {
                        Close();
                    }
                }
            }
            else if (_progress >= 70)
            {
                lblStatus.Text = "Loading services...";
            }
            else if (_progress >= 35)
            {
                lblStatus.Text = "Preparing operator console...";
            }

            progressBar.Value = _progress;
        }
    }
}
