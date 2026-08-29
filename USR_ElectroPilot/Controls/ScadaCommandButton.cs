using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace USR_ElectroPilot.Controls
{
    public class ScadaCommandButton : Button
    {
        public ScadaCommandButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            BackColor = Color.FromArgb(10, 40, 54);
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            Margin = new Padding(4);
        }

        public bool Danger { get; set; }
        public string IconKey { get; set; }
        public bool Active { get; set; }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            var graphics = pevent.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var enabled = Enabled;
            var accent = Danger ? Color.Tomato : GetAccentColor(IconKey);
            if (!enabled && !Active)
            {
                accent = Color.FromArgb(95, 112, 122);
            }

            var background = Danger
                ? Color.FromArgb(105, 21, 21)
                : Color.FromArgb(10, 39, 53);
            if (Active && !Danger)
            {
                background = Color.FromArgb(0, 75, 112);
            }
            else if (Active)
            {
                background = Color.FromArgb(150, 32, 28);
            }

            if (!enabled && !Active)
            {
                background = Color.FromArgb(26, 39, 47);
            }

            using (var brush = new SolidBrush(background))
            {
                graphics.FillRectangle(brush, ClientRectangle);
            }

            using (var edge = new Pen(accent, Active ? 2.4F : Danger ? 1.8F : 1.4F))
            {
                graphics.DrawRectangle(edge, 0, 0, Width - 1, Height - 1);
            }

            if (Active && enabled)
            {
                using (var activeBrush = new SolidBrush(accent))
                {
                    graphics.FillRectangle(activeBrush, 0, 0, 5, Height);
                }
            }

            using (var iconBack = new SolidBrush(Color.FromArgb(enabled ? 34 : 18, accent)))
            {
                graphics.FillEllipse(iconBack, 15, (Height - 28) / 2, 28, 28);
            }

            DrawCommandIcon(graphics, new Rectangle(18, (Height - 22) / 2, 22, 22), accent, IconKey);

            using (var textBrush = new SolidBrush(enabled || Active ? Color.White : Color.FromArgb(130, 145, 152)))
            using (var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter })
            {
                graphics.DrawString(Text, Font, textBrush, new RectangleF(52, 0, Width - 60, Height), format);
            }
        }

        private static Color GetAccentColor(string key)
        {
            if (string.Equals(key, "Auto", StringComparison.OrdinalIgnoreCase))
            {
                return Color.DeepSkyBlue;
            }

            if (string.Equals(key, "Manual", StringComparison.OrdinalIgnoreCase))
            {
                return Color.Gainsboro;
            }

            if (string.Equals(key, "Start Job", StringComparison.OrdinalIgnoreCase))
            {
                return Color.LimeGreen;
            }

            if (string.Equals(key, "Stop Plant", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(key, "Stop", StringComparison.OrdinalIgnoreCase))
            {
                return Color.Tomato;
            }

            if (string.Equals(key, "Reset", StringComparison.OrdinalIgnoreCase))
            {
                return Color.LightSteelBlue;
            }

            if (string.Equals(key, "IP Connect", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(key, "IP Connection", StringComparison.OrdinalIgnoreCase))
            {
                return Color.DeepSkyBlue;
            }

            if (string.Equals(key, "Alarms", StringComparison.OrdinalIgnoreCase))
            {
                return Color.DeepSkyBlue;
            }

            return Color.DeepSkyBlue;
        }

        private static void DrawCommandIcon(Graphics graphics, Rectangle rect, Color color, string key)
        {
            using (var pen = new Pen(color, 2F))
            using (var brush = new SolidBrush(color))
            {
                if (string.Equals(key, "Auto", StringComparison.OrdinalIgnoreCase))
                {
                    graphics.DrawEllipse(pen, rect.Left + 2, rect.Top + 2, rect.Width - 4, rect.Height - 4);
                    using (var font = new Font("Segoe UI", 8F, FontStyle.Bold))
                    {
                        graphics.DrawString("A", font, brush, rect.Left + 6, rect.Top + 4);
                    }

                    return;
                }

                if (string.Equals(key, "Manual", StringComparison.OrdinalIgnoreCase))
                {
                    graphics.DrawLine(pen, rect.Left + 6, rect.Bottom - 5, rect.Left + 6, rect.Top + 8);
                    graphics.DrawLine(pen, rect.Left + 10, rect.Bottom - 5, rect.Left + 10, rect.Top + 5);
                    graphics.DrawLine(pen, rect.Left + 14, rect.Bottom - 6, rect.Left + 14, rect.Top + 8);
                    graphics.DrawArc(pen, rect.Left + 5, rect.Bottom - 12, 14, 10, 0, 180);
                    return;
                }

                if (string.Equals(key, "Start Job", StringComparison.OrdinalIgnoreCase))
                {
                    var points = new[] { new Point(rect.Left + 6, rect.Top + 4), new Point(rect.Right - 4, rect.Top + rect.Height / 2), new Point(rect.Left + 6, rect.Bottom - 4) };
                    graphics.FillPolygon(brush, points);
                    return;
                }

                if (string.Equals(key, "Stop Plant", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(key, "Stop", StringComparison.OrdinalIgnoreCase))
                {
                    graphics.FillRectangle(brush, rect.Left + 6, rect.Top + 6, rect.Width - 12, rect.Height - 12);
                    return;
                }

                if (string.Equals(key, "Emergency Stop", StringComparison.OrdinalIgnoreCase))
                {
                    var points = new[] { new Point(rect.Left + rect.Width / 2, rect.Top + 3), new Point(rect.Right - 3, rect.Bottom - 4), new Point(rect.Left + 3, rect.Bottom - 4) };
                    graphics.DrawPolygon(pen, points);
                    graphics.DrawLine(pen, rect.Left + rect.Width / 2, rect.Top + 8, rect.Left + rect.Width / 2, rect.Bottom - 9);
                    graphics.FillEllipse(brush, rect.Left + rect.Width / 2 - 2, rect.Bottom - 7, 4, 4);
                    return;
                }

                if (string.Equals(key, "Reset", StringComparison.OrdinalIgnoreCase))
                {
                    graphics.DrawArc(pen, rect.Left + 4, rect.Top + 4, rect.Width - 8, rect.Height - 8, 35, 285);
                    graphics.DrawLine(pen, rect.Right - 5, rect.Top + 6, rect.Right - 4, rect.Top + 13);
                    graphics.DrawLine(pen, rect.Right - 5, rect.Top + 6, rect.Right - 12, rect.Top + 7);
                    return;
                }

                if (string.Equals(key, "IP Connect", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(key, "IP Connection", StringComparison.OrdinalIgnoreCase))
                {
                    graphics.DrawEllipse(pen, rect.Left + 3, rect.Top + 7, 8, 8);
                    graphics.DrawEllipse(pen, rect.Right - 11, rect.Top + 7, 8, 8);
                    graphics.DrawLine(pen, rect.Left + 11, rect.Top + 11, rect.Right - 11, rect.Top + 11);
                    graphics.DrawLine(pen, rect.Left + rect.Width / 2, rect.Top + 4, rect.Left + rect.Width / 2, rect.Bottom - 4);
                    return;
                }

                if (string.Equals(key, "Alarms", StringComparison.OrdinalIgnoreCase))
                {
                    graphics.DrawArc(pen, rect.Left + 5, rect.Top + 6, rect.Width - 10, rect.Height - 8, 200, 140);
                    graphics.DrawLine(pen, rect.Left + 7, rect.Bottom - 7, rect.Right - 7, rect.Bottom - 7);
                    graphics.FillEllipse(brush, rect.Left + rect.Width / 2 - 2, rect.Bottom - 5, 4, 3);
                    return;
                }

                graphics.DrawEllipse(pen, rect.Left + 4, rect.Top + 4, rect.Width - 8, rect.Height - 8);
            }
        }
    }
}
