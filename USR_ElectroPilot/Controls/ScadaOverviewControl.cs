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
        private readonly List<ProcessStepModel> _processSteps = new List<ProcessStepModel>();
        private readonly Dictionary<int, Rectangle> _tankHitBoxes = new Dictionary<int, Rectangle>();
        private readonly ContextMenuStrip _tankMenu = new ContextMenuStrip();
        private int _contextTankId;
        private int? _selectedTankId;
        private double _hoistPositionIndex;
        private string _hoistStatus = "Idle";
        private string _currentStepName = "Idle";
        private int _remainingSeconds;
        private bool _autoMode = true;
        private bool _emergencyStop;

        public event EventHandler<TankControlEventArgs> TankSelected;
        public event EventHandler<TankControlEventArgs> StartClicked;
        public event EventHandler<TankControlEventArgs> StopClicked;
        public event EventHandler<TankControlEventArgs> FaultClicked;
        public event EventHandler<TankControlEventArgs> ResetClicked;
        public event EventHandler<TankControlEventArgs> RemoveClicked;

        public ScadaOverviewControl()
        {
            DoubleBuffered = true;
            BackColor = Color.FromArgb(222, 231, 236);
            MinimumSize = new Size(900, 480);
            ConfigureMenu();
        }

        public void BindData(IList<TankModel> tanks, IList<WagonModel> wagons, IList<ProcessStepModel> processSteps, HoistStatusModel hoistStatus, double hoistPositionIndex, string currentStepName, int remainingSeconds, bool autoMode, bool emergencyStop)
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

            _processSteps.Clear();
            if (processSteps != null)
            {
                foreach (var step in processSteps)
                {
                    _processSteps.Add(step);
                }
            }

            _hoistPositionIndex = hoistPositionIndex;
            _hoistStatus = hoistStatus == null || string.IsNullOrEmpty(hoistStatus.Status) ? "Idle" : hoistStatus.Status;
            _currentStepName = string.IsNullOrEmpty(currentStepName) ? "Idle" : currentStepName;
            _remainingSeconds = remainingSeconds;
            _autoMode = autoMode;
            _emergencyStop = emergencyStop;

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
            using (var textBrush = new SolidBrush(Color.FromArgb(31, 45, 58)))
            using (var railPen = new Pen(Color.FromArgb(38, 95, 143), 5F))
            using (var thinRailPen = new Pen(Color.FromArgb(93, 157, 191), 2F))
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

                DrawHoist(g, plantLeft, plantTop, cellWidth, tanksPerRow, rowGap, titleFont, smallFont, textBrush);
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
                g.DrawString(GetStepName(tank), smallFont, textBrush, x + 2, y + 15);
                g.FillEllipse(string.Equals(tank.Status, Constants.StatusFault, StringComparison.OrdinalIgnoreCase) ? redBrush : greenBrush, x + width - 14, y + 15, 9, 9);

                g.FillRectangle(fillBrush, tankRect);
                var level = GetLevelPercent(tank);
                var fillHeight = Convert.ToInt32(tankRect.Height * level);
                g.FillRectangle(liquidBrush, tankRect.Left + 3, tankRect.Bottom - fillHeight, tankRect.Width - 6, Math.Max(0, fillHeight - 3));
                g.DrawRectangle(borderPen, tankRect);

                DrawDigitalBox(g, new Rectangle(x + 10, y + 44, width - 20, 16), tank.CurrentAmps.ToString("0"), digitalFont);
                DrawDigitalBox(g, new Rectangle(x + 10, y + 64, width - 20, 16), tank.Voltage.ToString("0.0"), digitalFont);
                DrawDigitalBox(g, new Rectangle(x + 10, y + 84, width - 20, 16), tank.TemperatureCelsius.ToString("0"), digitalFont);

                if (string.Equals(tank.Status, Constants.StatusFault, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(tank.Status, Constants.StatusWarning, StringComparison.OrdinalIgnoreCase))
                {
                    g.FillRectangle(redBrush, x + 11, y + 100, width - 22, 10);
                }

                DrawCenteredString(g, tank.Status + "  L" + Convert.ToInt32(GetLevelPercent(tank) * 100) + "%", smallFont, textBrush, new Rectangle(x, y + 124, width, 14));
            }

            _tankHitBoxes[tank.Id] = new Rectangle(x, y, width, 142);
        }

        private void DrawHoist(Graphics g, int left, int top, int cellWidth, int tanksPerRow, int rowGap, Font titleFont, Font smallFont, Brush textBrush)
        {
            if (_tanks.Count == 0)
            {
                return;
            }

            var clampedIndex = Math.Max(0, Math.Min(_tanks.Count - 1, _hoistPositionIndex));
            var row = Convert.ToInt32(Math.Floor(clampedIndex / tanksPerRow));
            var columnPosition = clampedIndex - (row * tanksPerRow);
            var x = left + Convert.ToInt32(columnPosition * cellWidth) + (cellWidth / 2) - 22;
            var y = top + (row * rowGap) - 40;
            DrawHoistBody(g, x, y, titleFont, smallFont, textBrush);
        }

        private void DrawHoistBody(Graphics g, int x, int y, Font titleFont, Font smallFont, Brush textBrush)
        {
            using (var hoistBrush = new SolidBrush(_emergencyStop ? Color.FromArgb(190, 45, 42) : Color.FromArgb(245, 180, 45)))
            using (var darkBrush = new SolidBrush(Color.FromArgb(39, 51, 62)))
            using (var borderPen = new Pen(Color.FromArgb(25, 45, 60), 2F))
            using (var redBrush = new SolidBrush(Color.FromArgb(225, 60, 45)))
            using (var greenBrush = new SolidBrush(Color.FromArgb(0, 170, 70)))
            {
                g.FillRectangle(darkBrush, x - 22, y + 32, 22, 10);
                g.FillRectangle(hoistBrush, x, y, 44, 78);
                g.FillRectangle(darkBrush, x + 15, y + 78, 14, 32);
                g.FillRectangle(darkBrush, x + 7, y + 110, 30, 8);
                g.DrawRectangle(borderPen, x, y, 36, 76);
                g.DrawString("HOIST", titleFont, textBrush, x - 2, y - 18);

                for (var i = 0; i < 4; i++)
                {
                    var active = IsHoistLampActive(i);
                    g.FillEllipse(active ? greenBrush : redBrush, x + 12, y + 8 + (i * 15), 12, 12);
                }

                g.DrawString(_hoistStatus, smallFont, textBrush, x + 50, y + 16);
                g.DrawString(_autoMode ? "AUTO" : "MANUAL", titleFont, textBrush, x + 50, y + 32);
            }
        }

        private void DrawSidePanel(Graphics g, Font titleFont, Font smallFont)
        {
            using (var textBrush = new SolidBrush(Color.FromArgb(31, 45, 58)))
            using (var panelBrush = new SolidBrush(Color.FromArgb(232, 238, 242)))
            using (var redBrush = new SolidBrush(Color.FromArgb(224, 58, 49)))
            {
                g.FillRectangle(redBrush, 28, 20, 36, 36);
                g.DrawString("ALARMS", titleFont, textBrush, 20, 62);

                DrawLegend(g, "PROCESS TIME SEC", 24, 124, Color.FromArgb(58, 150, 95), titleFont);
                DrawLegend(g, "AUTO / MANUAL", 24, 148, Color.FromArgb(92, 157, 191), titleFont);
                DrawLegend(g, "ACTUAL AMPERE", 24, 172, Color.FromArgb(74, 116, 180), titleFont);
                DrawLegend(g, "TEMPERATURE C", 24, 196, Color.FromArgb(204, 91, 73), titleFont);

                g.FillRectangle(panelBrush, 20, 236, 126, 126);
                g.DrawString("Information", smallFont, textBrush, 44, 256);
                g.DrawString(_currentStepName, smallFont, textBrush, 30, 286);
                g.DrawString("Remaining: " + _remainingSeconds + "s", smallFont, textBrush, 30, 308);
                g.DrawString(_emergencyStop ? "E-STOP ACTIVE" : "Safety OK", titleFont, textBrush, 30, 332);
            }
        }

        private void DrawWagonSummary(Graphics g, int left, int top, Font titleFont, Font smallFont)
        {
            if (_wagons.Count == 0)
            {
                return;
            }

            using (var panelBrush = new SolidBrush(Color.FromArgb(232, 238, 242)))
            using (var borderPen = new Pen(Color.FromArgb(62, 132, 166), 2F))
            using (var cellBrush = new SolidBrush(Color.FromArgb(206, 226, 235)))
            using (var textBrush = new SolidBrush(Color.FromArgb(31, 45, 58)))
            {
                var width = Math.Min(380, Width - left - 210);
                var rect = new Rectangle(left, top, width, 58);
                g.FillRectangle(panelBrush, rect);
                g.DrawRectangle(borderPen, rect);
                g.DrawString("Hoist / Carrier Status", titleFont, textBrush, left + 8, top + 6);

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
                g.FillRectangle(brush, x, y, 122, 17);
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

        private string GetStepName(TankModel tank)
        {
            foreach (var step in _processSteps)
            {
                if (step.TankId == tank.Id)
                {
                    return step.StepNo + ". " + step.StepName;
                }
            }

            return tank.ChemicalName;
        }

        private bool IsHoistLampActive(int lampIndex)
        {
            if (_emergencyStop)
            {
                return false;
            }

            if (string.Equals(_hoistStatus, "Moving", StringComparison.OrdinalIgnoreCase))
            {
                return lampIndex < 2;
            }

            if (string.Equals(_hoistStatus, "Processing", StringComparison.OrdinalIgnoreCase))
            {
                return lampIndex == 0 || lampIndex == 2;
            }

            if (string.Equals(_hoistStatus, "Loading", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(_hoistStatus, "Unloading", StringComparison.OrdinalIgnoreCase))
            {
                return lampIndex != 3;
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
