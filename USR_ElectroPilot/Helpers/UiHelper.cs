using System.Drawing;
using System.Windows.Forms;

namespace USR_ElectroPilot.Helpers
{
    public static class UiHelper
    {
        public static readonly Color BackColor = Color.FromArgb(24, 28, 35);
        public static readonly Color PanelColor = Color.FromArgb(34, 40, 49);
        public static readonly Color ForeColor = Color.FromArgb(235, 238, 245);
        public static readonly Color AccentColor = Color.FromArgb(0, 150, 136);
        public static readonly Font DefaultFont = new Font("Segoe UI", 9F);

        public static void ApplyDarkTheme(Control root)
        {
            if (root == null)
            {
                return;
            }

            root.BackColor = BackColor;
            root.ForeColor = ForeColor;
            root.Font = DefaultFont;

            foreach (Control child in root.Controls)
            {
                ApplyDarkTheme(child);
            }
        }

        public static void ApplyRoleRestrictions(Control adminControl)
        {
            if (adminControl != null)
            {
                adminControl.Enabled = AppSession.HasRole(Constants.RoleAdmin, Constants.RoleSupervisor);
                adminControl.Visible = adminControl.Enabled;
            }
        }

        public static void ApplyRoleRestrictions(params ToolStripItem[] adminItems)
        {
            var allowed = AppSession.HasRole(Constants.RoleAdmin, Constants.RoleSupervisor);

            if (adminItems == null)
            {
                return;
            }

            foreach (var item in adminItems)
            {
                if (item != null)
                {
                    item.Enabled = allowed;
                    item.Available = allowed;
                    item.Visible = allowed;
                }
            }
        }
    }
}
