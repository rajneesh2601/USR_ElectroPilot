using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
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
        private int _requestedRows = 1;

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

        public void BindData(IList<TankModel> tanks, IList<WagonModel> wagons, IList<ProcessStepModel> processSteps, HoistStatusModel hoistStatus, double hoistPositionIndex, string currentStepName, int remainingSeconds, bool autoMode, bool emergencyStop, int requestedRows)
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
            _requestedRows = Math.Max(1, Math.Min(4, requestedRows));

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
            g.Clear(Color.FromArgb(211, 224, 230));
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            _tankHitBoxes.Clear();

            using (var titleFont = new Font("Segoe UI Semibold", 9F, FontStyle.Bold))
            using (var smallFont = new Font("Segoe UI", 7.5F))
            using (var digitalFont = new Font("Consolas", 8.5F, FontStyle.Bold))
            using (var textBrush = new SolidBrush(Color.FromArgb(31, 45, 58)))
            using (var railPen = new Pen(Color.FromArgb(38, 95, 143), 5F))
            using (var thinRailPen = new Pen(Color.FromArgb(93, 157, 191), 2F))
            {
                DrawPlantHeader(g, titleFont, smallFont);
                DrawPlantFrame(g);
                DrawSidePanel(g, titleFont, smallFont);

                var plantLeft = 172;
                var plantTop = 82;
                var plantRight = Width - 42;
                var usableWidth = Math.Max(500, plantRight - plantLeft);
                var rowCount = GetVisibleRowCount(usableWidth);
                var tanksPerRow = Math.Max(1, Convert.ToInt32(Math.Ceiling(_tanks.Count / (double)rowCount)));
                var cellWidth = usableWidth / tanksPerRow;
                var rowGap = Math.Max(158, Math.Min(220, (Height - 130) / rowCount));
                var tankWidth = Math.Max(74, Math.Min(96, cellWidth - 14));

                for (var row = 0; row < rowCount; row++)
                {
                    DrawLineRow(g, row, row * tanksPerRow, plantLeft, plantTop + (row * rowGap), cellWidth, tanksPerRow, tankWidth, titleFont, smallFont, digitalFont, textBrush, railPen, thinRailPen);
                }

                DrawHoist(g, plantLeft, plantTop, cellWidth, tanksPerRow, rowGap, titleFont, smallFont, textBrush);
                DrawWagonSummary(g, plantLeft, Height - 80, titleFont, smallFont);
            }
        }

        private void DrawLineRow(Graphics g, int rowNumber, int startIndex, int left, int top, int cellWidth, int tanksPerRow, int tankWidth, Font titleFont, Font smallFont, Font digitalFont, Brush textBrush, Pen railPen, Pen thinRailPen)
        {
            var railY = top + 20;
            var right = Math.Min(Width - 38, left + (cellWidth * tanksPerRow));

            using (var rowBrush = new SolidBrush(Color.FromArgb(226, 236, 241)))
            using (var rowPen = new Pen(Color.FromArgb(159, 181, 193)))
            using (var rowLabelBrush = new SolidBrush(Color.FromArgb(38, 95, 143)))
            {
                g.FillRectangle(rowBrush, left - 8, top - 12, right - left + 16, 156);
                g.DrawRectangle(rowPen, left - 8, top - 12, right - left + 16, 156);
                g.DrawString("LINE " + (rowNumber + 1), titleFont, rowLabelBrush, left - 2, top - 10);
            }

            g.DrawLine(railPen, left, railY, right, railY);
            g.DrawLine(thinRailPen, left, railY + 5, right, railY + 5);

            for (var column = 0; column < tanksPerRow; column++)
            {
                var index = startIndex + column;
                if (index >= _tanks.Count)
                {
                    break;
                }

                DrawTankCell(g, _tanks[index], left + (column * cellWidth) + ((cellWidth - tankWidth) / 2), top + 35, tankWidth, titleFont, smallFont, digitalFont, textBrush);
            }
        }

        private void DrawTankCell(Graphics g, TankModel tank, int x, int y, int width, Font titleFont, Font smallFont, Font digitalFont, Brush textBrush)
        {
            var borderColor = GetStatusColor(tank.Status);
            var tankRect = new Rectangle(x + 8, y + 24, Math.Max(42, width - 16), 94);
            var selected = _selectedTankId.HasValue && _selectedTankId.Value == tank.Id;

            using (var borderPen = new Pen(selected ? Color.FromArgb(255, 255, 255) : borderColor, selected ? 3F : 2F))
            using (var liquidBrush = new LinearGradientBrush(tankRect, Color.FromArgb(56, 174, 190), Color.FromArgb(105, 205, 214), LinearGradientMode.Vertical))
            using (var tankBackBrush = new SolidBrush(Color.FromArgb(232, 246, 249)))
            using (var redBrush = new SolidBrush(Color.FromArgb(225, 64, 55)))
            using (var greenBrush = new SolidBrush(Color.FromArgb(0, 170, 80)))
            using (var capBrush = new SolidBrush(Color.FromArgb(39, 51, 62)))
            using (var capTextBrush = new SolidBrush(Color.White))
            {
                g.FillRectangle(capBrush, x + 5, y - 2, width - 10, 19);
                DrawCenteredString(g, tank.Name, titleFont, capTextBrush, new Rectangle(x + 5, y - 2, width - 10, 18));
                DrawFittedString(g, GetStepName(tank), smallFont, textBrush, new Rectangle(x, y + 15, width - 12, 14));
                g.FillEllipse(string.Equals(tank.Status, Constants.StatusFault, StringComparison.OrdinalIgnoreCase) ? redBrush : greenBrush, x + width - 14, y + 15, 9, 9);

                g.FillRectangle(tankBackBrush, tankRect);
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
            using (var textBrush = new SolidBrush(Color.FromArgb(229, 236, 241)))
            using (var mutedBrush = new SolidBrush(Color.FromArgb(170, 188, 198)))
            using (var panelBrush = new SolidBrush(Color.FromArgb(37, 52, 65)))
            using (var infoBrush = new SolidBrush(Color.FromArgb(226, 236, 241)))
            using (var infoTextBrush = new SolidBrush(Color.FromArgb(31, 45, 58)))
            using (var redBrush = new SolidBrush(Color.FromArgb(224, 58, 49)))
            {
                g.FillRectangle(panelBrush, 8, 46, 150, Height - 94);
                g.FillRectangle(redBrush, 44, 72, 44, 44);
                g.DrawString("ALARMS", titleFont, textBrush, 34, 124);
                g.DrawString("OPERATOR PANEL", smallFont, mutedBrush, 24, 50);

                DrawLegend(g, "PROCESS TIME SEC", 24, 184, Color.FromArgb(58, 150, 95), titleFont);
                DrawLegend(g, "AUTO / MANUAL", 24, 208, Color.FromArgb(92, 157, 191), titleFont);
                DrawLegend(g, "ACTUAL AMPERE", 24, 232, Color.FromArgb(74, 116, 180), titleFont);
                DrawLegend(g, "TEMPERATURE C", 24, 256, Color.FromArgb(204, 91, 73), titleFont);

                g.FillRectangle(infoBrush, 24, 304, 118, 128);
                g.DrawString("Information", smallFont, infoTextBrush, 50, 326);
                DrawFittedString(g, _currentStepName, smallFont, infoTextBrush, new Rectangle(34, 356, 98, 18));
                g.DrawString("Remaining: " + _remainingSeconds + "s", smallFont, infoTextBrush, 34, 380);
                g.DrawString(_emergencyStop ? "E-STOP ACTIVE" : "Safety OK", titleFont, infoTextBrush, 34, 408);
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
            using (var textBrush = new SolidBrush(Color.FromArgb(22, 32, 42)))
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

        private void DrawPlantHeader(Graphics g, Font titleFont, Font smallFont)
        {
            using (var headerBrush = new LinearGradientBrush(new Rectangle(0, 0, Width, 42), Color.FromArgb(28, 39, 52), Color.FromArgb(45, 63, 79), LinearGradientMode.Horizontal))
            using (var accentBrush = new SolidBrush(Color.FromArgb(0, 150, 136)))
            using (var textBrush = new SolidBrush(Color.White))
            using (var mutedBrush = new SolidBrush(Color.FromArgb(188, 205, 214)))
            {
                g.FillRectangle(headerBrush, 0, 0, Width, 42);
                g.FillRectangle(accentBrush, 0, 39, Width, 3);
                g.DrawString("ELECTROPLATING PLANT OVERVIEW", titleFont, textBrush, 18, 10);
                g.DrawString((_autoMode ? "AUTO" : "MANUAL") + " | " + _hoistStatus + " | " + _currentStepName, smallFont, mutedBrush, 260, 13);
            }
        }

        private void DrawPlantFrame(Graphics g)
        {
            using (var borderPen = new Pen(Color.FromArgb(71, 93, 110), 2F))
            using (var gridPen = new Pen(Color.FromArgb(198, 213, 221)))
            {
                g.DrawRectangle(borderPen, 4, 44, Width - 9, Height - 49);

                for (var x = 172; x < Width - 42; x += 64)
                {
                    g.DrawLine(gridPen, x, 48, x, Height - 10);
                }
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

        private static void DrawFittedString(Graphics g, string text, Font font, Brush brush, Rectangle rect)
        {
            using (var format = new StringFormat())
            {
                format.Trimming = StringTrimming.EllipsisCharacter;
                format.FormatFlags = StringFormatFlags.NoWrap;
                g.DrawString(text, font, brush, rect, format);
            }
        }

        private int GetVisibleRowCount(int usableWidth)
        {
            if (_tanks.Count == 0)
            {
                return 1;
            }

            var minCellWidth = 106;
            var maxTanksPerRow = Math.Max(1, usableWidth / minCellWidth);
            var neededRows = Convert.ToInt32(Math.Ceiling(_tanks.Count / (double)maxTanksPerRow));
            return Math.Max(1, Math.Min(4, Math.Max(_requestedRows, neededRows)));
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
