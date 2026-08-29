using System;
using System.Windows.Forms;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Forms
{
    internal static class DashboardWindowManager
    {
        private static Form _applicationHost;
        private static int _openDashboardCount;

        public static void RegisterApplicationHost(Form host)
        {
            _applicationHost = host;
        }

        public static void OpenDashboard(UserModel user)
        {
            if (user == null)
            {
                return;
            }

            var dashboard = new MainForm(user);
            _openDashboardCount++;
            dashboard.FormClosed += delegate
            {
                _openDashboardCount--;
                dashboard.Dispose();

                if (_openDashboardCount <= 0 && _applicationHost != null && !_applicationHost.IsDisposed)
                {
                    _applicationHost.Close();
                }
            };
            dashboard.Show();
        }

        public static void ShowLoginAndOpenDashboard(IWin32Window owner)
        {
            using (var loginForm = new LoginForm())
            {
                loginForm.ResetForNextLogin();
                if (loginForm.ShowDialog(owner) == DialogResult.OK)
                {
                    OpenDashboard(loginForm.AuthenticatedUser);
                }
            }
        }
    }
}
