using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace USR_ElectroPilot.Controls
{
    public class ScadaBrandHeader : Control
    {
        public ScadaBrandHeader()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            BackColor = Color.FromArgb(5, 16, 23);
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 19F, FontStyle.Bold);
            Cursor = Cursors.Default;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            using (var usrBrush = new SolidBrush(Color.DeepSkyBlue))
            using (var whiteBrush = new SolidBrush(Color.White))
            using (var mutedPen = new Pen(Color.FromArgb(24, 67, 82), 1F))
            {
                graphics.DrawLine(mutedPen, Width - 1, 12, Width - 1, Height - 12);
                graphics.DrawString("USR", Font, usrBrush, 24, 16);
                graphics.DrawString("ElectroPilot", Font, whiteBrush, 76, 16);
            }
        }
    }
}
