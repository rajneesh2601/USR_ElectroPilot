using System.Drawing;
using System.Windows.Forms;
using USR_ElectroPilot.Helpers;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Controls
{
    public partial class RectifierControl : UserControl
    {
        private RectifierModel _rectifier;

        public RectifierControl()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        public RectifierModel Rectifier
        {
            get { return _rectifier; }
            set
            {
                _rectifier = value;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.Clear(UiHelper.PanelColor);

            var name = _rectifier == null ? "Rectifier" : _rectifier.Name;
            var isFaulted = _rectifier != null && !string.IsNullOrEmpty(_rectifier.FaultCode);
            var stateColor = isFaulted ? Color.FromArgb(220, 70, 70) : (_rectifier != null && _rectifier.IsRunning ? UiHelper.AccentColor : Color.FromArgb(90, 110, 130));
            var stateText = isFaulted ? _rectifier.FaultCode : (_rectifier != null && _rectifier.IsRunning ? "RUN" : "STOP");

            using (var textBrush = new SolidBrush(UiHelper.ForeColor))
            using (var mutedBrush = new SolidBrush(Color.FromArgb(170, 180, 190)))
            using (var stateBrush = new SolidBrush(stateColor))
            using (var borderPen = new Pen(stateColor, 2F))
            {
                e.Graphics.DrawRectangle(borderPen, 4, 4, Width - 9, Height - 9);
                e.Graphics.DrawString(name, new Font("Segoe UI Semibold", 10F, FontStyle.Bold), textBrush, 12, 12);
                e.Graphics.FillEllipse(stateBrush, Width - 42, 16, 18, 18);
                e.Graphics.DrawString(stateText, UiHelper.DefaultFont, textBrush, 12, 45);

                var voltage = _rectifier == null ? "0.0 V" : _rectifier.Voltage.ToString("0.0") + " V";
                var current = _rectifier == null ? "0.0 A" : _rectifier.CurrentAmps.ToString("0.0") + " A";
                e.Graphics.DrawString(voltage, new Font("Segoe UI Semibold", 15F, FontStyle.Bold), textBrush, 12, 70);
                e.Graphics.DrawString(current, new Font("Segoe UI Semibold", 15F, FontStyle.Bold), textBrush, 12, 102);
                e.Graphics.DrawString("Voltage / Current", UiHelper.DefaultFont, mutedBrush, 12, Height - 28);
            }
        }
    }
}
