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
        }
    }
}
