using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace USR_ElectroPilot.Controls
{
    public class ScadaNavButton : Button
    {
        private bool _selected;

        public ScadaNavButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            BackColor = Color.FromArgb(7, 24, 34);
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            Margin = new Padding(0, 0, 0, 8);
            Width = 112;
            Height = 62;
        }

        public bool Selected
        {
            get { return _selected; }
            set
            {
                if (_selected == value)
                {
                    return;
                }

                _selected = value;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            var graphics = pevent.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var bounds = ClientRectangle;
            var backColor = Selected ? Color.FromArgb(0, 91, 143) : Color.FromArgb(7, 33, 45);
            using (var background = new SolidBrush(backColor))
            {
                graphics.FillRectangle(background, bounds);
            }

            var border = Selected ? Color.DeepSkyBlue : Color.FromArgb(18, 95, 125);
            using (var pen = new Pen(border, Selected ? 2F : 1F))
            {
                graphics.DrawRectangle(pen, 0, 0, bounds.Width - 1, bounds.Height - 1);
            }

            if (Selected)
            {
                using (var stripe = new SolidBrush(Color.DeepSkyBlue))
                {
                    graphics.FillRectangle(stripe, 0, 0, 4, bounds.Height);
                }
            }

            var iconColor = Selected ? Color.White : Color.DeepSkyBlue;
            DrawIcon(graphics, new Rectangle(34, 8, 44, 28), iconColor, Convert.ToString(Tag ?? Text));

            using (var brush = new SolidBrush(Color.White))
            using (var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                graphics.DrawString(Text, Font, brush, new RectangleF(6, 39, bounds.Width - 12, 18), format);
            }
        }

        private static void DrawIcon(Graphics graphics, Rectangle rect, Color color, string key)
        {
            using (var pen = new Pen(color, 2F))
            using (var brush = new SolidBrush(color))
            {
                if (string.Equals(key, "Menu", StringComparison.OrdinalIgnoreCase))
                {
                    var y = rect.Top + 7;
                    graphics.DrawLine(pen, rect.Left + 9, y, rect.Right - 9, y);
                    graphics.DrawLine(pen, rect.Left + 9, y + 8, rect.Right - 9, y + 8);
                    graphics.DrawLine(pen, rect.Left + 9, y + 16, rect.Right - 9, y + 16);
                    return;
                }

                if (string.Equals(key, "Overview", StringComparison.OrdinalIgnoreCase))
                {
                    graphics.DrawLines(pen, new[] { new Point(rect.Left + 5, rect.Top + 16), new Point(rect.Left + rect.Width / 2, rect.Top + 4), new Point(rect.Right - 5, rect.Top + 16) });
                    graphics.DrawRectangle(pen, rect.Left + 10, rect.Top + 15, rect.Width - 20, rect.Height - 10);
                    return;
                }

                if (string.Equals(key, "View", StringComparison.OrdinalIgnoreCase))
                {
                    graphics.DrawEllipse(pen, rect.Left + 7, rect.Top + 8, rect.Width - 14, rect.Height - 13);
                    graphics.FillEllipse(brush, rect.Left + rect.Width / 2 - 4, rect.Top + rect.Height / 2 - 4, 8, 8);
                    return;
                }

                if (string.Equals(key, "Process", StringComparison.OrdinalIgnoreCase))
                {
                    DrawNode(graphics, pen, brush, rect.Left + 10, rect.Top + 8);
                    DrawNode(graphics, pen, brush, rect.Left + rect.Width / 2, rect.Top + 20);
                    DrawNode(graphics, pen, brush, rect.Right - 10, rect.Top + 8);
                    graphics.DrawLine(pen, rect.Left + 14, rect.Top + 11, rect.Left + rect.Width / 2 - 4, rect.Top + 20);
                    graphics.DrawLine(pen, rect.Left + rect.Width / 2 + 4, rect.Top + 20, rect.Right - 14, rect.Top + 11);
                    return;
                }

                if (string.Equals(key, "Tanks", StringComparison.OrdinalIgnoreCase))
                {
                    graphics.DrawEllipse(pen, rect.Left + 9, rect.Top + 3, rect.Width - 18, 9);
                    graphics.DrawLine(pen, rect.Left + 9, rect.Top + 8, rect.Left + 9, rect.Bottom - 7);
                    graphics.DrawLine(pen, rect.Right - 9, rect.Top + 8, rect.Right - 9, rect.Bottom - 7);
                    graphics.DrawArc(pen, rect.Left + 9, rect.Bottom - 12, rect.Width - 18, 9, 0, 180);
                    return;
                }

                if (string.Equals(key, "Hoist", StringComparison.OrdinalIgnoreCase))
                {
                    graphics.DrawLine(pen, rect.Left + 6, rect.Top + 5, rect.Right - 6, rect.Top + 5);
                    graphics.DrawLine(pen, rect.Left + rect.Width / 2, rect.Top + 5, rect.Left + rect.Width / 2, rect.Bottom - 8);
                    graphics.DrawArc(pen, rect.Left + rect.Width / 2 - 5, rect.Bottom - 12, 10, 10, 0, 220);
                    return;
                }

                if (string.Equals(key, "Alarms", StringComparison.OrdinalIgnoreCase))
                {
                    graphics.DrawArc(pen, rect.Left + 12, rect.Top + 8, rect.Width - 24, rect.Height - 10, 200, 140);
                    graphics.DrawLine(pen, rect.Left + 14, rect.Bottom - 9, rect.Right - 14, rect.Bottom - 9);
                    graphics.FillEllipse(brush, rect.Left + rect.Width / 2 - 3, rect.Bottom - 6, 6, 5);
                    return;
                }

                if (string.Equals(key, "Trends", StringComparison.OrdinalIgnoreCase))
                {
                    graphics.DrawLine(pen, rect.Left + 7, rect.Bottom - 5, rect.Left + 7, rect.Top + 5);
                    graphics.DrawLine(pen, rect.Left + 7, rect.Bottom - 5, rect.Right - 4, rect.Bottom - 5);
                    graphics.DrawLines(pen, new[] { new Point(rect.Left + 10, rect.Bottom - 10), new Point(rect.Left + 20, rect.Top + 18), new Point(rect.Left + 30, rect.Top + 21), new Point(rect.Right - 6, rect.Top + 8) });
                    return;
                }

                if (string.Equals(key, "Reports", StringComparison.OrdinalIgnoreCase))
                {
                    graphics.DrawRectangle(pen, rect.Left + 12, rect.Top + 4, rect.Width - 22, rect.Height - 8);
                    graphics.DrawLine(pen, rect.Left + 18, rect.Top + 12, rect.Right - 14, rect.Top + 12);
                    graphics.DrawLine(pen, rect.Left + 18, rect.Top + 19, rect.Right - 14, rect.Top + 19);
                    return;
                }

                if (string.Equals(key, "Recipe", StringComparison.OrdinalIgnoreCase))
                {
                    graphics.DrawRectangle(pen, rect.Left + 12, rect.Top + 4, rect.Width - 22, rect.Height - 8);
                    graphics.DrawLine(pen, rect.Left + 18, rect.Top + 11, rect.Right - 15, rect.Top + 11);
                    graphics.DrawLine(pen, rect.Left + 18, rect.Top + 18, rect.Right - 20, rect.Top + 18);
                    graphics.DrawLine(pen, rect.Right - 18, rect.Bottom - 8, rect.Right - 7, rect.Bottom - 17);
                    return;
                }

                graphics.DrawEllipse(pen, rect.Left + 12, rect.Top + 5, rect.Width - 24, rect.Height - 10);
                graphics.FillEllipse(brush, rect.Left + rect.Width / 2 - 4, rect.Top + rect.Height / 2 - 4, 8, 8);
                graphics.DrawLine(pen, rect.Left + rect.Width / 2, rect.Top + 2, rect.Left + rect.Width / 2, rect.Top + 8);
                graphics.DrawLine(pen, rect.Left + rect.Width / 2, rect.Bottom - 2, rect.Left + rect.Width / 2, rect.Bottom - 8);
                graphics.DrawLine(pen, rect.Left + 6, rect.Top + rect.Height / 2, rect.Left + 12, rect.Top + rect.Height / 2);
                graphics.DrawLine(pen, rect.Right - 6, rect.Top + rect.Height / 2, rect.Right - 12, rect.Top + rect.Height / 2);
            }
        }

        private static void DrawNode(Graphics graphics, Pen pen, Brush brush, int x, int y)
        {
            graphics.DrawEllipse(pen, x - 4, y - 4, 8, 8);
            graphics.FillEllipse(brush, x - 2, y - 2, 4, 4);
        }
    }
}
