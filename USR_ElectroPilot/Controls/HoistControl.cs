using System;
using System.Drawing;
using System.Windows.Forms;
using USR_ElectroPilot.Helpers;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Controls
{
    public class HoistControl : UserControl
    {
        private HoistModel _hoist;
        private JobModel _job;

        public HoistControl()
        {
            DoubleBuffered = true;
            Size = new Size(220, 130);
        }

        public HoistModel Hoist
        {
            get { return _hoist; }
            set { _hoist = value; Invalidate(); }
        }

        public JobModel Job
        {
            get { return _job; }
            set { _job = value; Invalidate(); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.Clear(UiHelper.PanelColor);

            var status = _hoist == null ? "Idle" : _hoist.Status;
            var stateColor = GetStateColor(status);
            var title = _hoist == null ? "HOIST" : _hoist.HoistName;
            var currentTank = _hoist == null ? 0 : _hoist.CurrentTankNo;
            var targetTank = _hoist == null ? 0 : _hoist.TargetTankNo;
            var jobText = _job == null ? "No active job" : _job.JobNumber + "  Step " + _job.CurrentStep;

            using (var borderPen = new Pen(stateColor, 2F))
            using (var stateBrush = new SolidBrush(stateColor))
            using (var textBrush = new SolidBrush(UiHelper.ForeColor))
            using (var mutedBrush = new SolidBrush(Color.FromArgb(170, 180, 190)))
            using (var titleFont = new Font("Segoe UI Semibold", 11F, FontStyle.Bold))
            using (var valueFont = new Font("Segoe UI Semibold", 9F, FontStyle.Bold))
            {
                e.Graphics.DrawRectangle(borderPen, 4, 4, Width - 9, Height - 9);
                e.Graphics.FillRectangle(stateBrush, 12, 14, 12, 86);
                e.Graphics.DrawString(title, titleFont, textBrush, 34, 12);
                e.Graphics.DrawString("Position: T" + currentTank, UiHelper.DefaultFont, mutedBrush, 34, 42);
                e.Graphics.DrawString("Target: T" + targetTank, UiHelper.DefaultFont, mutedBrush, 34, 62);
                e.Graphics.DrawString("Status: " + status, valueFont, textBrush, 34, 82);
                e.Graphics.DrawString("Job: " + jobText, UiHelper.DefaultFont, mutedBrush, 34, 104);
            }
        }

        private static Color GetStateColor(string status)
        {
            if (string.Equals(status, "Fault", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(status, "EmergencyStop", StringComparison.OrdinalIgnoreCase))
            {
                return Color.FromArgb(220, 55, 50);
            }

            if (string.Equals(status, "Moving", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(status, "Lowering", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(status, "Lifting", StringComparison.OrdinalIgnoreCase))
            {
                return Color.FromArgb(235, 180, 55);
            }

            if (string.Equals(status, "Processing", StringComparison.OrdinalIgnoreCase))
            {
                return Color.FromArgb(0, 165, 90);
            }

            return Color.FromArgb(80, 145, 215);
        }
    }
}
