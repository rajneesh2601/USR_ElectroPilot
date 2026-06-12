using System;
using System.Drawing;
using System.Windows.Forms;
using USR_ElectroPilot.Helpers;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Controls
{
    public partial class WagonControl : UserControl
    {
        private WagonModel _wagon;

        public WagonControl()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        public WagonModel Wagon
        {
            get { return _wagon; }
            set
            {
                _wagon = value;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.Clear(UiHelper.PanelColor);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            var code = _wagon == null ? "Wagon" : _wagon.WagonCode;
            var state = _wagon == null ? Constants.WagonReady : _wagon.State;
            var color = GetStateColor(state);
            var badgeRect = new Rectangle(12, 48, Width - 24, 32);

            using (var textBrush = new SolidBrush(UiHelper.ForeColor))
            using (var mutedBrush = new SolidBrush(Color.FromArgb(170, 180, 190)))
            using (var badgeBrush = new SolidBrush(color))
            using (var borderPen = new Pen(color, 2F))
            {
                e.Graphics.DrawRectangle(borderPen, 4, 4, Width - 9, Height - 9);
                e.Graphics.DrawString(code, new Font("Segoe UI Semibold", 11F, FontStyle.Bold), textBrush, 12, 13);
                e.Graphics.FillRectangle(badgeBrush, badgeRect);
                e.Graphics.DrawString(state, new Font("Segoe UI Semibold", 10F, FontStyle.Bold), Brushes.White, badgeRect.Left + 10, badgeRect.Top + 7);

                var tankText = _wagon != null && _wagon.CurrentTankId.HasValue
                    ? "Tank " + _wagon.CurrentTankId.Value
                    : "No tank assigned";
                e.Graphics.DrawString(tankText, UiHelper.DefaultFont, mutedBrush, 12, 96);
            }
        }

        private static Color GetStateColor(string state)
        {
            if (string.Equals(state, Constants.WagonRunning, StringComparison.OrdinalIgnoreCase))
            {
                return UiHelper.AccentColor;
            }

            if (string.Equals(state, Constants.WagonComplete, StringComparison.OrdinalIgnoreCase))
            {
                return Color.FromArgb(80, 170, 90);
            }

            if (string.Equals(state, Constants.WagonFault, StringComparison.OrdinalIgnoreCase))
            {
                return Color.FromArgb(220, 70, 70);
            }

            return Color.FromArgb(90, 110, 130);
        }
    }
}
