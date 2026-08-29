using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace USR_ElectroPilot.Controls
{
    public class ScadaKpiIcon : Control
    {
        public ScadaKpiIcon()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            BackColor = Color.FromArgb(8, 27, 36);
            ForeColor = Color.DeepSkyBlue;
            Width = 52;
            Height = 52;
        }

        public string IconKey { get; set; }

        public Color AccentColor
        {
            get { return ForeColor; }
            set
            {
                ForeColor = value;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new Rectangle(4, 4, Width - 8, Height - 8);
            using (var glow = new SolidBrush(Color.FromArgb(28, ForeColor)))
            using (var pen = new Pen(ForeColor, 2.4F))
            using (var thinPen = new Pen(Color.FromArgb(170, ForeColor), 1.3F))
            {
                graphics.FillEllipse(glow, rect);

                if (string.Equals(IconKey, "Tanks", StringComparison.OrdinalIgnoreCase))
                {
                    graphics.DrawEllipse(pen, rect.Left + 10, rect.Top + 7, rect.Width - 20, 11);
                    graphics.DrawLine(pen, rect.Left + 10, rect.Top + 13, rect.Left + 10, rect.Bottom - 11);
                    graphics.DrawLine(pen, rect.Right - 10, rect.Top + 13, rect.Right - 10, rect.Bottom - 11);
                    graphics.DrawArc(pen, rect.Left + 10, rect.Bottom - 17, rect.Width - 20, 11, 0, 180);
                    return;
                }

                if (string.Equals(IconKey, "Running", StringComparison.OrdinalIgnoreCase))
                {
                    for (var i = 0; i < 3; i++)
                    {
                        var y = rect.Top + 12 + i * 9;
                        graphics.DrawBezier(pen, rect.Left + 7, y, rect.Left + 16, y + 8, rect.Left + 25, y - 8, rect.Right - 7, y);
                    }
                    return;
                }

                if (string.Equals(IconKey, "Hoist", StringComparison.OrdinalIgnoreCase))
                {
                    graphics.DrawLine(pen, rect.Left + 7, rect.Top + 11, rect.Right - 7, rect.Top + 11);
                    graphics.DrawLine(pen, rect.Left + rect.Width / 2, rect.Top + 11, rect.Left + rect.Width / 2, rect.Bottom - 12);
                    graphics.DrawRectangle(thinPen, rect.Left + 13, rect.Top + 8, rect.Width - 26, 8);
                    graphics.DrawArc(pen, rect.Left + rect.Width / 2 - 6, rect.Bottom - 17, 12, 12, 0, 230);
                    return;
                }

                if (string.Equals(IconKey, "Step", StringComparison.OrdinalIgnoreCase))
                {
                    var centerY = rect.Top + rect.Height / 2;
                    graphics.DrawLine(pen, rect.Left + 9, centerY - 6, rect.Right - 11, centerY - 6);
                    graphics.DrawLine(pen, rect.Right - 11, centerY - 6, rect.Right - 18, centerY - 12);
                    graphics.DrawLine(pen, rect.Right - 11, centerY - 6, rect.Right - 18, centerY);
                    graphics.DrawLine(pen, rect.Left + 9, centerY + 7, rect.Right - 11, centerY + 7);
                    graphics.DrawLine(pen, rect.Right - 11, centerY + 7, rect.Right - 18, centerY + 1);
                    graphics.DrawLine(pen, rect.Right - 11, centerY + 7, rect.Right - 18, centerY + 13);
                    using (var solid = new SolidBrush(ForeColor))
                    {
                        graphics.FillEllipse(solid, rect.Left + 5, centerY - 9, 5, 5);
                        graphics.FillEllipse(solid, rect.Left + 5, centerY + 4, 5, 5);
                    }

                    return;
                }

                graphics.DrawArc(pen, rect.Left + 11, rect.Top + 8, rect.Width - 22, rect.Height - 12, 200, 140);
                graphics.DrawLine(pen, rect.Left + 13, rect.Bottom - 10, rect.Right - 13, rect.Bottom - 10);
                using (var solid = new SolidBrush(ForeColor))
                {
                    graphics.FillEllipse(solid, rect.Left + rect.Width / 2 - 4, rect.Bottom - 7, 8, 6);
                }
            }
        }
    }
}
