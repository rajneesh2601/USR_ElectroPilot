using System;
using System.Windows.Forms;
using USR_ElectroPilot.Data;
using USR_ElectroPilot.Helpers;

namespace USR_ElectroPilot.Forms
{
    public partial class SplashForm : Form
    {
        private int _progress;

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

            try
            {
                DatabaseHelper.InitializeDatabase();
            }
            catch (Exception ex)
            {
                Logger.Error("Splash initialization failed", ex);
                lblStatus.Text = "Initialization failed. Check Logs folder.";
                progressTimer.Stop();
                return;
            }

            progressTimer.Start();
        }

        private void ProgressTimer_Tick(object sender, EventArgs e)
        {
            _progress += 4;

            if (_progress >= 100)
            {
                _progress = 100;
                progressTimer.Stop();
                lblStatus.Text = "Ready";
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
