using System.Drawing;
using System.Windows.Forms;
using USR_ElectroPilot.Helpers;

namespace USR_ElectroPilot.Controls
{
    public class StatusCardControl : UserControl
    {
        private string _title = "Metric";
        private string _value = "0";
        private string _caption = string.Empty;

        public StatusCardControl()
        {
            DoubleBuffered = true;
            Size = new Size(180, 90);
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; Invalidate(); }
        }

        public string Value
        {
            get { return _value; }
            set { _value = value; Invalidate(); }
        }

        public string Caption
        {
            get { return _caption; }
            set { _caption = value; Invalidate(); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.Clear(UiHelper.PanelColor);

            using (var borderPen = new Pen(UiHelper.AccentColor, 2F))
            using (var titleBrush = new SolidBrush(Color.FromArgb(170, 180, 190)))
            using (var valueBrush = new SolidBrush(UiHelper.ForeColor))
            {
                e.Graphics.DrawRectangle(borderPen, 4, 4, Width - 9, Height - 9);
                e.Graphics.DrawString(Title, UiHelper.DefaultFont, titleBrush, 12, 10);
                e.Graphics.DrawString(Value, new Font("Segoe UI Semibold", 20F, FontStyle.Bold), valueBrush, 12, 30);
                e.Graphics.DrawString(Caption, UiHelper.DefaultFont, titleBrush, 12, Height - 24);
            }
        }
    }
}
