using System;
using System.Drawing;
using System.Windows.Forms;
using USR_ElectroPilot.Helpers;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Controls
{
    public partial class TankControl : UserControl
    {
        private TankModel _tank;

        public TankControl()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        public TankModel Tank
        {
            get { return _tank; }
            set
            {
                _tank = value;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.Clear(UiHelper.PanelColor);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            var title = _tank == null ? "Tank" : _tank.Name;
            var chemical = _tank == null ? string.Empty : _tank.ChemicalName;
            var levelPercent = GetLevelPercent();
            var status = _tank == null ? Constants.StatusNormal : _tank.Status;

            using (var titleBrush = new SolidBrush(UiHelper.ForeColor))
            using (var mutedBrush = new SolidBrush(Color.FromArgb(170, 180, 190)))
            using (var borderPen = new Pen(GetStatusColor(status), 2F))
            using (var levelBrush = new SolidBrush(GetLevelColor(status)))
            {
                e.Graphics.DrawString(title, new Font("Segoe UI Semibold", 10F, FontStyle.Bold), titleBrush, 10, 8);
                e.Graphics.DrawString(chemical, UiHelper.DefaultFont, mutedBrush, 10, 30);

                var tankRect = new Rectangle(18, 58, Width - 36, Height - 100);
                e.Graphics.DrawRectangle(borderPen, tankRect);

                var fillHeight = Convert.ToInt32(tankRect.Height * levelPercent);
                var fillRect = new Rectangle(tankRect.Left + 2, tankRect.Bottom - fillHeight + 1, tankRect.Width - 3, Math.Max(0, fillHeight - 2));
                e.Graphics.FillRectangle(levelBrush, fillRect);

                var percentText = Math.Round(levelPercent * 100) + "%";
                var percentSize = e.Graphics.MeasureString(percentText, UiHelper.DefaultFont);
                e.Graphics.DrawString(percentText, UiHelper.DefaultFont, titleBrush, tankRect.Left + (tankRect.Width - percentSize.Width) / 2, tankRect.Top + (tankRect.Height - percentSize.Height) / 2);

                var footer = _tank == null
                    ? status
                    : string.Format("{0:0.0} C  {1:0.0} V  {2:0.0} A", _tank.TemperatureCelsius, _tank.Voltage, _tank.CurrentAmps);

                e.Graphics.DrawString(footer, UiHelper.DefaultFont, mutedBrush, 10, Height - 32);
            }
        }

        private double GetLevelPercent()
        {
            if (_tank == null || _tank.CapacityLiters <= 0)
            {
                return 0;
            }

            var percent = _tank.CurrentLevelLiters / _tank.CapacityLiters;
            if (percent < 0)
            {
                return 0;
            }

            return percent > 1 ? 1 : percent;
        }

        private static Color GetStatusColor(string status)
        {
            if (string.Equals(status, Constants.StatusFault, StringComparison.OrdinalIgnoreCase))
            {
                return Color.FromArgb(220, 70, 70);
            }

            if (string.Equals(status, Constants.StatusWarning, StringComparison.OrdinalIgnoreCase))
            {
                return Color.FromArgb(240, 180, 60);
            }

            return UiHelper.AccentColor;
        }

        private static Color GetLevelColor(string status)
        {
            var color = GetStatusColor(status);
            return Color.FromArgb(190, color);
        }
    }
}
