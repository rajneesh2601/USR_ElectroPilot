using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using USR_ElectroPilot.Helpers;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Controls
{
    public class ScadaOverviewControl : UserControl
    {
        private readonly List<TankModel> _tanks = new List<TankModel>();
        private readonly List<WagonModel> _wagons = new List<WagonModel>();
        private readonly Dictionary<int, Rectangle> _tankHitBoxes = new Dictionary<int, Rectangle>();
        private readonly ContextMenuStrip _tankMenu = new ContextMenuStrip();
        private int _contextTankId;
        private int? _selectedTankId;

        public event EventHandler<TankControlEventArgs> TankSelected;
        public event EventHandler<TankControlEventArgs> StartClicked;
        public event EventHandler<TankControlEventArgs> StopClicked;
        public event EventHandler<TankControlEventArgs> FaultClicked;
        public event EventHandler<TankControlEventArgs> ResetClicked;
        public event EventHandler<TankControlEventArgs> RemoveClicked;

        public ScadaOverviewControl()
        {
            DoubleBuffered = true;
            BackColor = Color.FromArgb(238, 190, 180);
            MinimumSize = new Size(900, 480);
            ConfigureMenu();
        }

        public void BindData(IList<TankModel> tanks, IList<WagonModel> wagons)
        {
            _tanks.Clear();
            if (tanks != null)
            {
                foreach (var tank in tanks)
                {
                    _tanks.Add(tank);
                }

                _tanks.Sort(delegate (TankModel left, TankModel right)
                {
                    return left.TankNumber.CompareTo(right.TankNumber);
                });
            }

            _wagons.Clear();
            if (wagons != null)
            {
                foreach (var wagon in wagons)
                {
                    _wagons.Add(wagon);
                }
            }

            Invalidate();
        }

        public void SelectTank(int tankId)
        {
            _selectedTankId = tankId;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            foreach (var item in _tankHitBoxes)
            {
                if (item.Value.Contains(e.Location))
                {
                    _selectedTankId = item.Key;
                    OnTankEvent(TankSelected, item.Key);
                    Invalidate();

                    if (e.Button == MouseButtons.Right)
                    {
                        _contextTankId = item.Key;
                        _tankMenu.Show(this, e.Location);
                    }

                    return;
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;
            g.Clear(BackColor);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            _tankHitBoxes.Clear();

            using (var titleFont = new Font("Segoe UI Semibold", 9F, FontStyle.Bold))
            using (var smallFont = new Font("Segoe UI", 7.5F))
            using (var digitalFont = new Font("Consolas", 8.5F, FontStyle.Bold))
            using (var textBrush = new SolidBrush(Color.FromArgb(35, 55, 75)))
            using (var railPen = new Pen(Color.FromArgb(67, 139, 204), 5F))
            using (var thinRailPen = new Pen(Color.FromArgb(67, 139, 204), 2F))
            {
                DrawSidePanel(g, titleFont, smallFont);

                var plantLeft = 170;
                var plantTop = 38;
                var plantRight = Width - 30;
                var usableWidth = Math.Max(500, plantRight - plantLeft);
                var tanksPerRow = Math.Max(4, Math.Min(10, usableWidth / 78));
                var cellWidth = usableWidth / tanksPerRow;
                var rowGap = Math.Max(180, (Height - 130) / 2);

                DrawLineRow(g, 0, plantLeft, plantTop, cellWidth, tanksPerRow, titleFont, smallFont, digitalFont, textBrush, railPen, thinRailPen);
                if (_tanks.Count > tanksPerRow)
                {
                    DrawLineRow(g, tanksPerRow, plantLeft, plantTop + rowGap, cellWidth, tanksPerRow, titleFont, smallFont, digitalFont, textBrush, railPen, thinRailPen);
                }

                DrawWagons(g, plantLeft, plantTop, cellWidth, tanksPerRow, rowGap, titleFont, smallFont, textBrush);
                DrawWagonSummary(g, plantLeft, Height - 80, titleFont, smallFont);
            }
        }

        private void DrawLineRow(Graphics g, int startIndex, int left, int top, int cellWidth, int tanksPerRow, Font titleFont, Font smallFont, Font digitalFont, Brush textBrush, Pen railPen, Pen thinRailPen)
        {
            var railY = top + 20;
            g.DrawLine(railPen, left, railY, Math.Min(Width - 30, left + (cellWidth * tanksPerRow)), railY);
            g.DrawLine(thinRailPen, left, railY + 5, Math.Min(Width - 30, left + (cellWidth * tanksPerRow)), railY + 5);

            for (var column = 0; column < tanksPerRow; column++)
            {
                var index = startIndex + column;
                if (index >= _tanks.Count)
                {
                    break;
                }

                DrawTankCell(g, _tanks[index], left + (column * cellWidth) + 6, top + 35, Math.Min(68, cellWidth - 10), titleFont, smallFont, digitalFont, textBrush);
            }
        }

        private void DrawTankCell(Graphics g, TankModel tank, int x, int y, int width, Font titleFont, Font smallFont, Font digitalFont, Brush textBrush)
        {
            var borderColor = GetStatusColor(tank.Status);
            var tankRect = new Rectangle(x + 8, y + 24, Math.Max(42, width - 16), 94);
            var selected = _selectedTankId.HasValue && _selectedTankId.Value == tank.Id;

            using (var borderPen = new Pen(selected ? Color.White : borderColor, selected ? 3F : 2F))
            using (var fillBrush = new SolidBrush(Color.FromArgb(211, 239, 247)))
            using (var liquidBrush = new SolidBrush(Color.FromArgb(85, 185, 205)))
            using (var blackBrush = new SolidBrush(Color.FromArgb(18, 24, 28)))
            using (var redBrush = new SolidBrush(Color.FromArgb(225, 64, 55)))
            using (var greenBrush = new SolidBrush(Color.FromArgb(0, 170, 80)))
            {
                DrawCenteredString(g, tank.Name, titleFont, textBrush, new Rectangle(x, y, width, 16));
                g.DrawString(tank.TemperatureCelsius.ToString("0.0") + " C", smallFont, textBrush, x + 2, y + 15);
                g.FillEllipse(string.Equals(tank.Status, Constants.StatusFault, StringComparison.OrdinalIgnoreCase) ? redBrush : greenBrush, x + width - 14, y + 15, 9, 9);

                g.FillRectangle(fillBrush, tankRect);
                var level = GetLevelPercent(tank);
                var fillHeight = Convert.ToInt32(tankRect.Height * level);
                g.FillRectangle(liquidBrush, tankRect.Left + 3, tankRect.Bottom - fillHeight, tankRect.Width - 6, Math.Max(0, fillHeight - 3));
                g.DrawRectangle(borderPen, tankRect);

                DrawDigitalBox(g, new Rectangle(x + 10, y + 44, width - 20, 16), tank.CurrentAmps.ToString("0"), digitalFont);
                DrawDigitalBox(g, new Rectangle(x + 10, y + 64, width - 20, 16), tank.Voltage.ToString("0.0"), digitalFont);

                if (string.Equals(tank.Status, Constants.StatusFault, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(tank.Status, Constants.StatusWarning, StringComparison.OrdinalIgnoreCase))
                {
                    g.FillRectangle(redBrush, x + 11, y + 100, width - 22, 10);
                }

                DrawCenteredString(g, tank.Status, smallFont, textBrush, new Rectangle(x, y + 124, width, 14));
            }

            _tankHitBoxes[tank.Id] = new Rectangle(x, y, width, 142);
        }

        private void DrawWagons(Graphics g, int left, int top, int cellWidth, int tanksPerRow, int rowGap, Font titleFont, Font smallFont, Brush textBrush)
        {
            for (var index = 0; index < _wagons.Count; index++)
            {
                var wagon = _wagons[index];
                var tankIndex = FindTankIndex(wagon.CurrentTankId);
                var row = tankIndex >= tanksPerRow ? 1 : 0;
                var column = tankIndex >= 0 ? tankIndex % tanksPerRow : Math.Min(index * 3, tanksPerRow - 1);
                var x = left + (column * cellWidth) + (cellWidth / 2) - 18;
                var y = top + (row * rowGap) - 35;
                DrawWagon(g, wagon, x, y, titleFont, smallFont, textBrush);
            }
        }

        private void DrawWagon(Graphics g, WagonModel wagon, int x, int y, Font titleFont, Font smallFont, Brush textBrush)
        {
            using (var yellowBrush = new SolidBrush(Color.FromArgb(246, 226, 57)))
            using (var darkBrush = new SolidBrush(Color.FromArgb(40, 45, 48)))
            using (var borderPen = new Pen(Color.FromArgb(80, 100, 40), 2F))
            using (var redBrush = new SolidBrush(Color.FromArgb(225, 60, 45)))
            using (var greenBrush = new SolidBrush(Color.FromArgb(0, 170, 70)))
            {
                g.FillRectangle(darkBrush, x - 22, y + 32, 22, 10);
                g.FillRectangle(yellowBrush, x, y, 36, 76);
                g.DrawRectangle(borderPen, x, y, 36, 76);
                g.DrawString(wagon.WagonCode, smallFont, textBrush, x - 2, y - 16);

                for (var i = 0; i < 4; i++)
                {
                    var active = IsWagonActive(wagon, i);
                    g.FillEllipse(active ? greenBrush : redBrush, x + 12, y + 8 + (i * 15), 12, 12);
                }

                g.DrawString(wagon.State, smallFont, textBrush, x + 42, y + 18);
                if (wagon.CurrentTankId.HasValue)
                {
                    g.DrawString("T" + wagon.CurrentTankId.Value, titleFont, textBrush, x + 42, y + 34);
                }
            }
        }

        private void DrawSidePanel(Graphics g, Font titleFont, Font smallFont)
        {
            using (var textBrush = new SolidBrush(Color.FromArgb(35, 55, 75)))
            using (var panelBrush = new SolidBrush(Color.FromArgb(232, 226, 218)))
            using (var redBrush = new SolidBrush(Color.FromArgb(224, 58, 49)))
            using (var yellowBrush = new SolidBrush(Color.FromArgb(242, 216, 35)))
            using (var blueBrush = new SolidBrush(Color.FromArgb(58, 134, 205)))
            {
                g.FillRectangle(redBrush, 28, 20, 36, 36);
                g.DrawString("ALARMS", titleFont, textBrush, 20, 62);

                DrawLegend(g, "ACTUAL PRO. TIME SEC", 24, 130, Color.FromArgb(0, 120, 65), titleFont);
                DrawLegend(g, "PREV. PRO. TIME SEC", 24, 154, Color.FromArgb(218, 196, 42), titleFont);
                DrawLegend(g, "ACTUAL AMPERE", 24, 178, Color.FromArgb(50, 120, 202), titleFont);
                DrawLegend(g, "TEMPERATURE C", 24, 202, Color.FromArgb(188, 45, 48), titleFont);

                g.FillRectangle(panelBrush, 20, 240, 120, 92);
                g.DrawString("Information", smallFont, textBrush, 44, 256);
                g.DrawString("Reset", smallFont, textBrush, 62, 286);
                g.DrawString("READ CHMB 01", smallFont, textBrush, 38, 316);
            }
        }

        private void DrawWagonSummary(Graphics g, int left, int top, Font titleFont, Font smallFont)
        {
            if (_wagons.Count == 0)
            {
                return;
            }

            using (var panelBrush = new SolidBrush(Color.FromArgb(226, 236, 197)))
            using (var borderPen = new Pen(Color.FromArgb(92, 160, 70), 2F))
            using (var cellBrush = new SolidBrush(Color.FromArgb(190, 235, 38)))
            using (var textBrush = new SolidBrush(Color.FromArgb(30, 70, 45)))
            {
                var width = Math.Min(380, Width - left - 210);
                var rect = new Rectangle(left, top, width, 58);
                g.FillRectangle(panelBrush, rect);
                g.DrawRectangle(borderPen, rect);
                g.DrawString("Wagon Status", titleFont, textBrush, left + 8, top + 6);

                for (var i = 0; i < _wagons.Count && i < 4; i++)
                {
                    var wagon = _wagons[i];
                    var cell = new Rectangle(left + 92 + (i * 68), top + 28, 58, 20);
                    g.FillRectangle(cellBrush, cell);
                    g.DrawRectangle(borderPen, cell);
                    DrawCenteredString(g, wagon.WagonCode + " " + GetWagonTankText(wagon), smallFont, textBrush, cell);
                }
            }
        }

        private void DrawLegend(Graphics g, string text, int x, int y, Color color, Font font)
        {
            using (var brush = new SolidBrush(color))
            using (var textBrush = new SolidBrush(Color.FromArgb(35, 55, 75)))
            {
                g.FillRectangle(brush, x, y, 116, 17);
                g.DrawString(text, font, textBrush, x + 4, y + 2);
            }
        }

        private void DrawDigitalBox(Graphics g, Rectangle rect, string text, Font font)
        {
            using (var brush = new SolidBrush(Color.FromArgb(8, 18, 20)))
            using (var pen = new Pen(Color.FromArgb(60, 80, 70)))
            using (var textBrush = new SolidBrush(Color.FromArgb(190, 245, 80)))
            {
                g.FillRectangle(brush, rect);
                g.DrawRectangle(pen, rect);
                DrawCenteredString(g, text, font, textBrush, rect);
            }
        }

        private static void DrawCenteredString(Graphics g, string text, Font font, Brush brush, Rectangle rect)
        {
            using (var format = new StringFormat())
            {
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;
                g.DrawString(text, font, brush, rect, format);
            }
        }

        private void ConfigureMenu()
        {
            _tankMenu.Items.Add("Start", null, delegate { OnTankEvent(StartClicked, _contextTankId); });
            _tankMenu.Items.Add("Stop", null, delegate { OnTankEvent(StopClicked, _contextTankId); });
            _tankMenu.Items.Add("Fault", null, delegate { OnTankEvent(FaultClicked, _contextTankId); });
            _tankMenu.Items.Add("Reset", null, delegate { OnTankEvent(ResetClicked, _contextTankId); });
            _tankMenu.Items.Add("Remove", null, delegate { OnTankEvent(RemoveClicked, _contextTankId); });
        }

        private void OnTankEvent(EventHandler<TankControlEventArgs> handler, int tankId)
        {
            if (handler != null)
            {
                handler(this, new TankControlEventArgs(tankId));
            }
        }

        private int FindTankIndex(int? tankId)
        {
            if (!tankId.HasValue)
            {
                return -1;
            }

            for (var i = 0; i < _tanks.Count; i++)
            {
                if (_tanks[i].Id == tankId.Value || _tanks[i].TankNumber == tankId.Value)
                {
                    return i;
                }
            }

            return -1;
        }

        private static string GetWagonTankText(WagonModel wagon)
        {
            return wagon.CurrentTankId.HasValue ? "T" + wagon.CurrentTankId.Value : "-";
        }

        private static bool IsWagonActive(WagonModel wagon, int lampIndex)
        {
            if (string.Equals(wagon.State, Constants.WagonFault, StringComparison.OrdinalIgnoreCase))
            {
                return lampIndex == 3;
            }

            if (string.Equals(wagon.State, Constants.WagonRunning, StringComparison.OrdinalIgnoreCase))
            {
                return lampIndex < 2;
            }

            return lampIndex == 0;
        }

        private static double GetLevelPercent(TankModel tank)
        {
            if (tank == null || tank.CapacityLiters <= 0)
            {
                return 0;
            }

            var percent = tank.CurrentLevelLiters / tank.CapacityLiters;
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
                return Color.FromArgb(220, 55, 50);
            }

            if (string.Equals(status, Constants.StatusWarning, StringComparison.OrdinalIgnoreCase))
            {
                return Color.FromArgb(238, 170, 35);
            }

            if (string.Equals(status, Constants.StatusRunning, StringComparison.OrdinalIgnoreCase))
            {
                return Color.FromArgb(0, 160, 80);
            }

            return Color.FromArgb(45, 145, 210);
        }
    }
}
