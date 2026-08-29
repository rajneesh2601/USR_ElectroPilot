using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace USR_ElectroPilot.Controls
{
    public class ScadaHeaderItem : Label
    {
        public ScadaHeaderItem()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            AutoSize = false;
            Dock = DockStyle.Fill;
            BackColor = Color.FromArgb(5, 16, 23);
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            TextAlign = ContentAlignment.MiddleCenter;
        }

        public string IconKey { get; set; }
        public bool ShowStatusDot { get; set; }
        public bool BadgeStyle { get; set; }
        public bool ShowDropDownArrow { get; set; }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            Invalidate();
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var bounds = ClientRectangle;
            bounds.Inflate(-4, -10);

            if (BadgeStyle)
            {
                using (var back = new LinearGradientBrush(bounds, Color.FromArgb(10, 50, 72), Color.FromArgb(7, 31, 45), LinearGradientMode.Vertical))
                using (var border = new Pen(ForeColor, 1F))
                {
                    graphics.FillRectangle(back, bounds);
                    graphics.DrawRectangle(border, bounds.Left, bounds.Top, bounds.Width - 1, bounds.Height - 1);
                }
            }

            var iconRect = new Rectangle(bounds.Left + 10, bounds.Top + (bounds.Height - 22) / 2, 22, 22);
            if (!string.IsNullOrWhiteSpace(IconKey))
            {
                DrawIcon(graphics, iconRect, ForeColor, IconKey);
            }

            var textLeft = string.IsNullOrWhiteSpace(IconKey) ? bounds.Left + 8 : iconRect.Right + 8;
            var textWidth = bounds.Right - textLeft - (ShowStatusDot ? 22 : ShowDropDownArrow ? 20 : 6);
            var lines = (Text ?? string.Empty).Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            using (var textBrush = new SolidBrush(ForeColor))
            using (var mutedBrush = new SolidBrush(Color.FromArgb(190, 208, 218)))
            using (var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap })
            {
                if (lines.Length > 1)
                {
                    using (var mainFont = new Font(Font.FontFamily, 9.5F, FontStyle.Bold))
                    using (var subFont = new Font(Font.FontFamily, 7.5F, FontStyle.Regular))
                    {
                        graphics.DrawString(lines[0], mainFont, textBrush, new RectangleF(textLeft, bounds.Top + 4, textWidth, 20), format);
                        graphics.DrawString(lines[1], subFont, mutedBrush, new RectangleF(textLeft, bounds.Top + 24, textWidth, 18), format);
                    }
                }
                else
                {
                    graphics.DrawString(Text, Font, textBrush, new RectangleF(textLeft, bounds.Top, textWidth, bounds.Height), format);
                }
            }

            if (ShowStatusDot)
            {
                using (var dotBrush = new SolidBrush(ForeColor))
                {
                    graphics.FillEllipse(dotBrush, bounds.Right - 16, bounds.Top + bounds.Height / 2 - 5, 10, 10);
                }
            }

            if (ShowDropDownArrow)
            {
                using (var arrowPen = new Pen(ForeColor, 1.8F))
                {
                    var centerX = bounds.Right - 13;
                    var centerY = bounds.Top + bounds.Height / 2;
                    graphics.DrawLine(arrowPen, centerX - 5, centerY - 2, centerX, centerY + 3);
                    graphics.DrawLine(arrowPen, centerX, centerY + 3, centerX + 5, centerY - 2);
                }
            }
        }

        private static void DrawIcon(Graphics graphics, Rectangle rect, Color color, string key)
        {
            using (var pen = new Pen(color, 1.8F))
            using (var brush = new SolidBrush(color))
            {
                if (string.Equals(key, "Plant", StringComparison.OrdinalIgnoreCase))
                {
                    graphics.DrawEllipse(pen, rect.Left + 3, rect.Top + 3, rect.Width - 6, rect.Height - 6);
                    graphics.FillEllipse(brush, rect.Left + 9, rect.Top + 9, 4, 4);
                    return;
                }

                if (string.Equals(key, "Mode", StringComparison.OrdinalIgnoreCase))
                {
                    graphics.DrawEllipse(pen, rect.Left + 2, rect.Top + 2, rect.Width - 4, rect.Height - 4);
                    using (var font = new Font("Segoe UI", 8F, FontStyle.Bold))
                    {
                        graphics.DrawString("A", font, brush, rect.Left + 6, rect.Top + 4);
                    }

                    return;
                }

                if (string.Equals(key, "Hoist", StringComparison.OrdinalIgnoreCase))
                {
                    graphics.DrawRectangle(pen, rect.Left + 3, rect.Top + 4, rect.Width - 6, 6);
                    graphics.DrawLine(pen, rect.Left + rect.Width / 2, rect.Top + 10, rect.Left + rect.Width / 2, rect.Bottom - 4);
                    graphics.DrawArc(pen, rect.Left + 7, rect.Bottom - 9, 8, 8, 35, 255);
                    return;
                }

                if (string.Equals(key, "Clock", StringComparison.OrdinalIgnoreCase))
                {
                    graphics.DrawEllipse(pen, rect.Left + 3, rect.Top + 3, rect.Width - 6, rect.Height - 6);
                    graphics.DrawLine(pen, rect.Left + rect.Width / 2, rect.Top + 7, rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
                    graphics.DrawLine(pen, rect.Left + rect.Width / 2, rect.Top + rect.Height / 2, rect.Right - 7, rect.Bottom - 8);
                    return;
                }

                if (string.Equals(key, "User", StringComparison.OrdinalIgnoreCase))
                {
                    graphics.DrawEllipse(pen, rect.Left + 7, rect.Top + 4, 8, 8);
                    graphics.DrawArc(pen, rect.Left + 4, rect.Top + 13, 14, 11, 200, 140);
                    return;
                }

                graphics.DrawEllipse(pen, rect.Left + 4, rect.Top + 4, rect.Width - 8, rect.Height - 8);
            }
        }
    }
}
