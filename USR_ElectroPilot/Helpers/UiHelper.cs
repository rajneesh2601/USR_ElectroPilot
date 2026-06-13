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

            root.BackColor = root is Panel || root is GroupBox ? PanelColor : BackColor;
            root.ForeColor = ForeColor;
            root.Font = DefaultFont;

            ApplyControlSpecificTheme(root);

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

        private static void ApplyControlSpecificTheme(Control control)
        {
            var grid = control as DataGridView;
            if (grid != null)
            {
                grid.BackgroundColor = BackColor;
                grid.BorderStyle = BorderStyle.None;
                grid.EnableHeadersVisualStyles = false;
                grid.GridColor = Color.FromArgb(58, 68, 84);
                grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(42, 50, 62);
                grid.ColumnHeadersDefaultCellStyle.ForeColor = ForeColor;
                grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(42, 50, 62);
                grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = ForeColor;
                grid.DefaultCellStyle.BackColor = PanelColor;
                grid.DefaultCellStyle.ForeColor = ForeColor;
                grid.DefaultCellStyle.SelectionBackColor = AccentColor;
                grid.DefaultCellStyle.SelectionForeColor = Color.White;
                grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(30, 36, 44);
                grid.RowHeadersDefaultCellStyle.BackColor = PanelColor;
                grid.RowHeadersDefaultCellStyle.ForeColor = ForeColor;
                return;
            }

            var toolStrip = control as ToolStrip;
            if (toolStrip != null)
            {
                toolStrip.BackColor = PanelColor;
                toolStrip.ForeColor = ForeColor;
                toolStrip.RenderMode = ToolStripRenderMode.System;

                foreach (ToolStripItem item in toolStrip.Items)
                {
                    item.BackColor = PanelColor;
                    item.ForeColor = ForeColor;
                }

                return;
            }

            if (control is TextBoxBase || control is ComboBox || control is NumericUpDown || control is DateTimePicker)
            {
                control.BackColor = Color.FromArgb(18, 22, 28);
                control.ForeColor = ForeColor;
            }

            var button = control as Button;
            if (button != null)
            {
                button.BackColor = Color.FromArgb(48, 58, 72);
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderColor = Color.FromArgb(74, 88, 108);
                button.ForeColor = ForeColor;
            }
        }
    }
}
