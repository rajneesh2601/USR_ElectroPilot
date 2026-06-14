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
            Size = new Size(198, 98);
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
            using (var titleFont = new Font("Segoe UI", 9F))
            using (var valueFont = CreateValueFont(Value))
            using (var captionFont = new Font("Segoe UI", 8.5F))
            {
                e.Graphics.DrawRectangle(borderPen, 4, 4, Width - 9, Height - 9);
                DrawFittedText(e.Graphics, Title, titleFont, titleBrush, new Rectangle(12, 10, Width - 24, 18));
                DrawFittedText(e.Graphics, Value, valueFont, valueBrush, new Rectangle(12, 34, Width - 24, 32));
                DrawFittedText(e.Graphics, Caption, captionFont, titleBrush, new Rectangle(12, Height - 25, Width - 24, 18));
            }
        }

        private static Font CreateValueFont(string value)
        {
            var size = string.IsNullOrEmpty(value) || value.Length <= 8 ? 20F : 15F;
            return new Font("Segoe UI Semibold", size, FontStyle.Bold);
        }

        private static void DrawFittedText(Graphics graphics, string text, Font font, Brush brush, Rectangle bounds)
        {
            using (var format = new StringFormat())
            {
                format.Trimming = StringTrimming.EllipsisCharacter;
                format.FormatFlags = StringFormatFlags.NoWrap;
                graphics.DrawString(text, font, brush, bounds, format);
            }
        }
    }
}
