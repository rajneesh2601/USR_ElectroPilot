using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
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
        private readonly List<HoistModel> _hoists = new List<HoistModel>();
        private readonly List<JobModel> _jobs = new List<JobModel>();
        private readonly Dictionary<int, Rectangle> _tankHitBoxes = new Dictionary<int, Rectangle>();
        private readonly ContextMenuStrip _tankMenu = new ContextMenuStrip();
        private ToolStripItem _startMenuItem;
        private ToolStripItem _stopMenuItem;
        private ToolStripItem _faultMenuItem;
        private ToolStripItem _resetMenuItem;
        private ToolStripItem _removeMenuItem;
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

        public bool CanOperateTanks { get; set; }
        public bool CanEngineerTanks { get; set; }

        public ScadaOverviewControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            DoubleBuffered = true;
            BackColor = Color.FromArgb(222, 231, 236);
            MinimumSize = new Size(900, 480);
            ConfigureMenu();
        }

        public void BindData(IList<TankModel> tanks, IList<WagonModel> wagons, IList<ProcessStepModel> processSteps, IList<HoistModel> hoists, IList<JobModel> jobs, HoistStatusModel hoistStatus, double hoistPositionIndex, string currentStepName, int remainingSeconds, bool autoMode, bool emergencyStop, int requestedRows)
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
                    var lineCompare = left.LineId.CompareTo(right.LineId);
                    return lineCompare != 0 ? lineCompare : left.TankNo.CompareTo(right.TankNo);
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

            _hoists.Clear();
            if (hoists != null)
            {
                foreach (var hoist in hoists)
                {
                    _hoists.Add(hoist);
                }
            }

            _jobs.Clear();
            if (jobs != null)
            {
                foreach (var job in jobs)
                {
                    _jobs.Add(job);
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
                        UpdateTankMenuAccess();
                        if (!_startMenuItem.Available && !_removeMenuItem.Available)
                        {
                            return;
                        }

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
            g.Clear(Color.FromArgb(5, 14, 20));
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            _tankHitBoxes.Clear();

            DrawReferenceDashboard(g);
        }

        private void DrawReferenceDashboard(Graphics g)
        {
            var tanks = _tanks
                .Where(t => t.IsActive)
                .OrderBy(t => t.LineId <= 0 ? 1 : t.LineId)
                .ThenBy(t => t.TankNo)
                .ToList();

            if (tanks.Count == 0)
            {
                tanks = _tanks
                    .OrderBy(t => t.LineId <= 0 ? 1 : t.LineId)
                    .ThenBy(t => t.TankNo)
                    .ToList();
            }

            using (var titleFont = new Font("Segoe UI Semibold", 15F, FontStyle.Bold))
            using (var midFont = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold))
            using (var smallFont = new Font("Segoe UI", 8F))
            using (var tinyFont = new Font("Segoe UI", 7F))
            using (var valueFont = new Font("Segoe UI Semibold", 20F, FontStyle.Bold))
            {
                var header = new Rectangle(0, 0, Width, 62);
                var leftNav = new Rectangle(0, header.Bottom, 84, Height - header.Height);
                var rightPanel = new Rectangle(Math.Max(Width - 240, 760), header.Bottom + 14, Math.Min(226, Math.Max(0, Width - 760)), Height - header.Height - 24);
                var contentRight = rightPanel.Width > 0 ? rightPanel.Left - 12 : Width - 12;
                var alarmHeight = Math.Max(128, Math.Min(190, Height / 4));
                var controlHeight = 64;
                var viewport = new Rectangle(leftNav.Right + 10, header.Bottom + 14, Math.Max(320, contentRight - leftNav.Right - 10), Math.Max(250, Height - header.Height - alarmHeight - controlHeight - 42));
                var controls = new Rectangle(viewport.Left, viewport.Bottom + 8, viewport.Width, controlHeight);
                var alarms = new Rectangle(viewport.Left, controls.Bottom + 8, viewport.Width, Math.Max(100, Height - controls.Bottom - 14));

                DrawDashboardHeader(g, header, titleFont, midFont, smallFont);
                DrawLeftNavigation(g, leftNav, midFont, tinyFont);
                DrawMachineViewport(g, viewport, tanks, midFont, smallFont, tinyFont);
                DrawCommandRow(g, controls, midFont, smallFont);
                DrawAlarmTable(g, alarms, midFont, smallFont, tinyFont);
                DrawKpiCards(g, rightPanel, midFont, smallFont, valueFont);
            }
        }

        private void DrawDashboardHeader(Graphics g, Rectangle rect, Font titleFont, Font midFont, Font smallFont)
        {
            using (var brush = new LinearGradientBrush(rect, Color.FromArgb(3, 12, 18), Color.FromArgb(10, 25, 34), LinearGradientMode.Horizontal))
            using (var borderPen = new Pen(Color.FromArgb(24, 48, 61)))
            using (var white = new SolidBrush(Color.FromArgb(235, 244, 248)))
            using (var blue = new SolidBrush(Color.FromArgb(0, 158, 255)))
            using (var green = new SolidBrush(Color.FromArgb(47, 226, 95)))
            using (var muted = new SolidBrush(Color.FromArgb(150, 169, 179)))
            {
                g.FillRectangle(brush, rect);
                g.DrawLine(borderPen, rect.Left, rect.Bottom - 1, rect.Right, rect.Bottom - 1);
                g.DrawString("USR", titleFont, blue, rect.Left + 22, rect.Top + 17);
                g.DrawString("ElectroPilot", titleFont, white, rect.Left + 72, rect.Top + 17);
                g.DrawString("Plant:", midFont, white, rect.Left + 350, rect.Top + 22);
                g.DrawString(_emergencyStop ? "Emergency Stop" : GetPlantStateText(), midFont, _emergencyStop ? Brushes.Red : green, rect.Left + 398, rect.Top + 22);
                g.FillEllipse(_emergencyStop ? Brushes.Red : green, rect.Left + 525, rect.Top + 25, 11, 11);
                DrawHeaderModeBadge(g, new Rectangle(rect.Left + 590, rect.Top + 12, 126, 38), _autoMode ? "Auto Mode" : "Manual Mode", midFont);
                g.DrawString("Hoist H1", midFont, blue, rect.Left + 750, rect.Top + 22);
                g.DrawString(GetPrimaryHoistStatus(), midFont, _emergencyStop ? Brushes.Red : green, rect.Left + 822, rect.Top + 22);
                g.FillEllipse(_emergencyStop ? Brushes.Red : green, rect.Left + 898, rect.Top + 25, 11, 11);
                g.DrawString(DateTime.Now.ToString("HH:mm:ss"), midFont, white, Math.Max(rect.Right - 260, 930), rect.Top + 13);
                g.DrawString(DateTime.Now.ToString("MMM dd, yyyy"), smallFont, muted, Math.Max(rect.Right - 260, 930), rect.Top + 35);
                g.DrawString("Operator", midFont, white, Math.Max(rect.Right - 116, 1020), rect.Top + 23);
            }
        }

        private void DrawHeaderModeBadge(Graphics g, Rectangle rect, string text, Font font)
        {
            using (var back = new SolidBrush(Color.FromArgb(21, 46, 62)))
            using (var pen = new Pen(Color.FromArgb(0, 113, 176)))
            using (var cyan = new SolidBrush(Color.FromArgb(0, 175, 255)))
            using (var white = new SolidBrush(Color.FromArgb(230, 240, 245)))
            {
                FillRoundedRectangle(g, back, rect, 4);
                g.DrawRectangle(pen, rect);
                g.DrawEllipse(pen, rect.Left + 14, rect.Top + 9, 20, 20);
                DrawCenteredString(g, _autoMode ? "A" : "M", font, cyan, new Rectangle(rect.Left + 14, rect.Top + 9, 20, 20));
                g.DrawString(text, font, white, rect.Left + 42, rect.Top + 10);
            }
        }

        private void DrawLeftNavigation(Graphics g, Rectangle rect, Font midFont, Font tinyFont)
        {
            using (var back = new SolidBrush(Color.FromArgb(8, 23, 32)))
            using (var active = new SolidBrush(Color.FromArgb(0, 83, 132)))
            using (var border = new Pen(Color.FromArgb(22, 49, 63)))
            using (var cyan = new SolidBrush(Color.FromArgb(0, 178, 255)))
            using (var white = new SolidBrush(Color.FromArgb(225, 236, 241)))
            {
                g.FillRectangle(back, rect);
                g.DrawLine(border, rect.Right - 1, rect.Top, rect.Right - 1, rect.Bottom);
                var items = new[] { "Overview", "Process", "Tanks", "Hoist", "Alarms", "Trends", "Reports", "Settings" };
                for (var i = 0; i < items.Length; i++)
                {
                    var item = new Rectangle(rect.Left, rect.Top + i * 78, rect.Width, 78);
                    if (i == 0)
                    {
                        g.FillRectangle(active, item);
                        g.FillRectangle(cyan, item.Left, item.Top, 4, item.Height);
                    }

                    DrawNavIcon(g, item.Left + 33, item.Top + 21, i == 0 ? Color.FromArgb(0, 195, 255) : Color.FromArgb(225, 236, 241), i);
                    DrawCenteredString(g, items[i], tinyFont, i == 0 ? cyan : white, new Rectangle(item.Left + 3, item.Top + 47, item.Width - 6, 18));
                }
            }
        }

        private void DrawNavIcon(Graphics g, int x, int y, Color color, int index)
        {
            using (var pen = new Pen(color, 1.8F))
            {
                if (index == 0)
                {
                    g.DrawPolygon(pen, new[] { new Point(x - 11, y + 6), new Point(x, y - 8), new Point(x + 11, y + 6) });
                    g.DrawRectangle(pen, x - 8, y + 6, 16, 14);
                    return;
                }

                if (index == 3)
                {
                    g.DrawLine(pen, x - 12, y - 6, x + 12, y - 6);
                    g.DrawLine(pen, x, y - 6, x, y + 12);
                    g.DrawRectangle(pen, x - 6, y + 12, 12, 8);
                    return;
                }

                g.DrawEllipse(pen, x - 10, y - 10, 20, 20);
                g.DrawLine(pen, x - 8, y, x + 8, y);
            }
        }

        private void DrawMachineViewport(Graphics g, Rectangle rect, IList<TankModel> tanks, Font midFont, Font smallFont, Font tinyFont)
        {
            using (var back = new LinearGradientBrush(rect, Color.FromArgb(4, 12, 17), Color.FromArgb(11, 26, 33), LinearGradientMode.Vertical))
            using (var border = new Pen(Color.FromArgb(17, 43, 55)))
            {
                FillRoundedRectangle(g, back, rect, 6);
                g.DrawRectangle(border, rect);
            }

            if (tanks.Count == 0)
            {
                using (var text = new SolidBrush(Color.FromArgb(190, 207, 215)))
                {
                    DrawCenteredString(g, "No tank data configured", midFont, text, rect);
                }

                return;
            }

            var count = tanks.Count;
            var machineLeft = rect.Left + 92;
            var machineRight = rect.Right - 80;
            var usable = Math.Max(260, machineRight - machineLeft);
            var pitch = Math.Max(62, Math.Min(118, usable / Math.Max(1, count)));
            var tankWidth = Math.Max(54, Math.Min(96, pitch - 14));
            var tankHeight = Math.Max(102, Math.Min(148, rect.Height / 3));
            var depthX = Math.Max(20, Math.Min(34, tankWidth / 3));
            var depthY = Math.Max(16, Math.Min(24, tankHeight / 6));
            var baseY = rect.Bottom - 72;
            var startX = machineLeft;

            DrawMachineStructure(g, rect, startX, baseY, count, pitch, tankWidth, depthX, depthY);

            for (var i = 0; i < tanks.Count; i++)
            {
                var x = startX + i * pitch;
                var y = baseY - tankHeight - i * 3;
                DrawIsoTank(g, tanks[i], i, new Rectangle(x, y, tankWidth, tankHeight), depthX, depthY, midFont, smallFont, tinyFont);
            }

            DrawPortalHoists(g, rect, tanks, startX, baseY, pitch, tankWidth, tankHeight, depthX, depthY, midFont, smallFont, tinyFont);
            DrawStackLightAndCabinet(g, rect, startX, baseY, smallFont);
        }

        private void DrawMachineStructure(Graphics g, Rectangle rect, int startX, int baseY, int count, int pitch, int tankWidth, int depthX, int depthY)
        {
            var endX = startX + Math.Max(0, count - 1) * pitch + tankWidth + depthX + 34;
            var railRearY = baseY - 230;
            var railFrontY = baseY - 192;

            using (var bluePen = new Pen(Color.FromArgb(10, 89, 149), 5F))
            using (var blueLightPen = new Pen(Color.FromArgb(42, 133, 194), 2F))
            using (var railShadowPen = new Pen(Color.FromArgb(0, 29, 48), 8F))
            using (var gratePen = new Pen(Color.FromArgb(204, 135, 17), 1F))
            using (var walkwayBrush = new SolidBrush(Color.FromArgb(18, 42, 55)))
            using (var railBrush = new SolidBrush(Color.FromArgb(5, 64, 112)))
            {
                g.DrawLine(railShadowPen, startX - 42, railRearY + 4, endX, railRearY - 18);
                g.DrawLine(bluePen, startX - 42, railRearY, endX, railRearY - 22);
                g.DrawLine(blueLightPen, startX - 42, railRearY - 6, endX, railRearY - 28);
                g.DrawLine(railShadowPen, startX - 42, railFrontY + 6, endX, railFrontY - 16);
                g.DrawLine(bluePen, startX - 42, railFrontY, endX, railFrontY - 22);
                g.DrawLine(blueLightPen, startX - 42, railFrontY - 6, endX, railFrontY - 28);

                var walkway = new[]
                {
                    new Point(startX + 40, baseY + 18),
                    new Point(endX - 14, baseY - 24),
                    new Point(endX + 24, baseY + 18),
                    new Point(startX + 76, baseY + 62)
                };
                g.FillPolygon(walkwayBrush, walkway);
                g.DrawPolygon(bluePen, walkway);

                for (var x = startX + 60; x < endX; x += 22)
                {
                    g.DrawLine(gratePen, x, baseY + 18, x + 36, baseY + 58);
                }

                for (var i = 0; i <= count; i++)
                {
                    var postX = startX + i * pitch;
                    g.FillRectangle(railBrush, postX + 8, baseY - 10 - i * 3, 6, 72);
                }
            }
        }

        private void DrawIsoTank(Graphics g, TankModel tank, int index, Rectangle front, int depthX, int depthY, Font midFont, Font smallFont, Font tinyFont)
        {
            var selected = _selectedTankId.HasValue && _selectedTankId.Value == tank.Id;
            var level = GetLevelPercent(tank);
            var top = new[]
            {
                new Point(front.Left, front.Top),
                new Point(front.Right, front.Top),
                new Point(front.Right + depthX, front.Top - depthY),
                new Point(front.Left + depthX, front.Top - depthY)
            };
            var side = new[]
            {
                new Point(front.Right, front.Top),
                new Point(front.Right + depthX, front.Top - depthY),
                new Point(front.Right + depthX, front.Bottom - depthY),
                new Point(front.Right, front.Bottom)
            };
            var liquidTop = new[]
            {
                new Point(front.Left + 8, front.Top + 14),
                new Point(front.Right - 8, front.Top + 14),
                new Point(front.Right + depthX - 12, front.Top + 14 - depthY),
                new Point(front.Left + depthX + 8, front.Top + 14 - depthY)
            };
            var fillHeight = Convert.ToInt32((front.Height - 30) * level);
            var fillRect = new Rectangle(front.Left + 8, front.Bottom - fillHeight - 8, front.Width - 16, Math.Max(4, fillHeight));
            var hitBox = Rectangle.Union(front, new Rectangle(front.Left, front.Top - depthY, front.Width + depthX, front.Height + depthY));

            using (var faceBrush = new LinearGradientBrush(front, Color.FromArgb(122, 127, 119), Color.FromArgb(78, 84, 80), LinearGradientMode.Vertical))
            using (var sideBrush = new SolidBrush(Color.FromArgb(64, 70, 70)))
            using (var rimBrush = new SolidBrush(Color.FromArgb(174, 178, 166)))
            using (var liquidBrush = new SolidBrush(Color.FromArgb(120, 42, 156, 175)))
            using (var pen = new Pen(selected ? Color.FromArgb(0, 195, 255) : Color.FromArgb(31, 47, 52), selected ? 3F : 1.5F))
            using (var labelBack = new SolidBrush(Color.FromArgb(230, 221, 218, 205)))
            using (var text = new SolidBrush(Color.FromArgb(11, 16, 18)))
            using (var statusBrush = new SolidBrush(GetStatusColor(tank.Status)))
            using (var dark = new SolidBrush(Color.FromArgb(20, 29, 32)))
            {
                g.FillRectangle(faceBrush, front);
                g.FillPolygon(sideBrush, side);
                g.FillPolygon(rimBrush, top);
                g.FillPolygon(liquidBrush, liquidTop);
                g.FillRectangle(liquidBrush, fillRect);
                g.DrawRectangle(pen, front);
                g.DrawPolygon(pen, top);
                g.DrawPolygon(pen, side);

                var label = new Rectangle(front.Left + front.Width / 2 - 25, front.Top + front.Height / 2 - 16, 50, 30);
                g.FillRectangle(labelBack, label);
                g.DrawRectangle(Pens.Gray, label);
                DrawCenteredString(g, tank.TankNo.ToString("00"), midFont, text, new Rectangle(label.Left, label.Top - 2, label.Width, 18));
                DrawCenteredString(g, GetShortProcessName(tank), tinyFont, text, new Rectangle(label.Left + 2, label.Top + 15, label.Width - 4, 13));
                g.FillEllipse(statusBrush, front.Right - 12, front.Top - 22, 12, 12);

                var lower = new Rectangle(front.Left + 8, front.Bottom + 6, front.Width, 52);
                DrawFittedString(g, tank.TemperatureCelsius.ToString("0.0") + " C", tinyFont, Brushes.White, new Rectangle(lower.Left, lower.Top, lower.Width, 12));
                DrawFittedString(g, tank.CurrentAmps.ToString("0.0") + " A  " + tank.Voltage.ToString("0.0") + " V", tinyFont, Brushes.White, new Rectangle(lower.Left, lower.Top + 14, lower.Width, 12));
                DrawFittedString(g, Convert.ToInt32(level * 100) + "%  " + tank.Status, tinyFont, Brushes.White, new Rectangle(lower.Left, lower.Top + 28, lower.Width, 12));

                g.FillRectangle(dark, front.Right + depthX + 2, front.Bottom - 38 - depthY, 10, 24);
                g.FillEllipse(statusBrush, front.Right + depthX - 2, front.Bottom - 18 - depthY, 16, 16);
            }

            _tankHitBoxes[tank.Id] = hitBox;
        }

        private void DrawPortalHoists(Graphics g, Rectangle viewport, IList<TankModel> tanks, int startX, int baseY, int pitch, int tankWidth, int tankHeight, int depthX, int depthY, Font midFont, Font smallFont, Font tinyFont)
        {
            var hoists = _hoists.Count == 0
                ? new List<HoistModel> { new HoistModel { HoistName = "H1", PositionIndex = _hoistPositionIndex, Status = _hoistStatus, IsAuto = _autoMode, CurrentTankNo = Math.Max(1, Convert.ToInt32(_hoistPositionIndex) + 1) } }
                : _hoists.OrderBy(h => h.HoistName).ToList();

            foreach (var hoist in hoists)
            {
                var position = Math.Max(0, Math.Min(tanks.Count - 1, hoist.PositionIndex));
                var centerX = startX + Convert.ToInt32(position * pitch) + tankWidth / 2;
                DrawPortalHoist(g, viewport, hoist, centerX, baseY, tankHeight, midFont, smallFont, tinyFont);
            }
        }

        private void DrawPortalHoist(Graphics g, Rectangle viewport, HoistModel hoist, int centerX, int baseY, int tankHeight, Font midFont, Font smallFont, Font tinyFont)
        {
            var status = _emergencyStop ? "EmergencyStop" : (string.IsNullOrEmpty(hoist.Status) ? "Idle" : hoist.Status);
            var beamWidth = 250;
            var beamHeight = 32;
            var beamY = baseY - tankHeight - 112;
            var legTop = beamY + beamHeight - 2;
            var frontLegBottom = baseY - 132;
            var rearLegBottom = baseY - 172;
            var leftLegX = centerX - 108;
            var rightLegX = centerX + 92;
            var liftLowered = string.Equals(status, "Processing", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(status, "Lowering", StringComparison.OrdinalIgnoreCase);
            var liftY = liftLowered ? baseY - tankHeight + 28 : beamY + 72;

            using (var yellow = new LinearGradientBrush(new Rectangle(centerX - beamWidth / 2, beamY, beamWidth, beamHeight), Color.FromArgb(255, 194, 32), Color.FromArgb(198, 125, 8), LinearGradientMode.Vertical))
            using (var yellowDark = new SolidBrush(Color.FromArgb(190, 117, 5)))
            using (var outline = new Pen(Color.FromArgb(82, 56, 8), 2F))
            using (var shadow = new SolidBrush(Color.FromArgb(70, 0, 0, 0)))
            using (var dark = new SolidBrush(Color.FromArgb(38, 43, 43)))
            using (var cablePen = new Pen(Color.FromArgb(42, 45, 45), 2F))
            using (var green = new SolidBrush(Color.FromArgb(38, 215, 85)))
            using (var red = new SolidBrush(Color.FromArgb(240, 57, 57)))
            using (var cyan = new SolidBrush(Color.FromArgb(0, 185, 255)))
            {
                g.FillEllipse(shadow, centerX - 126, frontLegBottom + 8, 252, 20);
                var beam = new Rectangle(centerX - beamWidth / 2, beamY, beamWidth, beamHeight);
                g.FillRectangle(yellow, beam);
                g.DrawRectangle(outline, beam);
                g.FillRectangle(yellowDark, centerX - beamWidth / 2 + 16, beamY + beamHeight, beamWidth - 32, 9);
                g.DrawString(string.IsNullOrEmpty(hoist.HoistName) ? "WAGON H1" : "WAGON " + hoist.HoistName, midFont, Brushes.Black, centerX - 48, beamY + 7);

                DrawHoistLeg(g, leftLegX, legTop, frontLegBottom, yellowDark, outline);
                DrawHoistLeg(g, rightLegX, legTop - 8, rearLegBottom, yellowDark, outline);
                DrawWheelBogey(g, leftLegX + 4, frontLegBottom, dark, outline);
                DrawWheelBogey(g, rightLegX + 4, rearLegBottom, dark, outline);
                g.FillRectangle(dark, rightLegX + 34, beamY + 4, 42, 26);
                g.FillEllipse(dark, rightLegX + 66, beamY + 3, 28, 28);

                var trolley = new Rectangle(centerX - 18, beamY + 34, 36, 22);
                g.FillRectangle(dark, trolley);
                g.DrawRectangle(outline, trolley);
                g.DrawLine(cablePen, centerX - 7, trolley.Bottom, centerX - 7, liftY);
                g.DrawLine(cablePen, centerX + 7, trolley.Bottom, centerX + 7, liftY);
                g.FillRectangle(yellowDark, centerX - 48, liftY, 96, 12);

                DrawPlateCarrier(g, centerX, liftY + 12, smallFont);
                DrawHoistStatusLamps(g, centerX + beamWidth / 2 - 44, beamY + 7, status, green, red);
                DrawFittedString(g, status.ToUpperInvariant(), tinyFont, cyan, new Rectangle(centerX - 58, beamY - 18, 116, 14));
            }
        }

        private static void DrawHoistLeg(Graphics g, int x, int top, int bottom, Brush brush, Pen outline)
        {
            var rect = new Rectangle(x, top, 24, Math.Max(10, bottom - top));
            g.FillRectangle(brush, rect);
            g.DrawRectangle(outline, rect);
            g.DrawLine(outline, x + 5, top + 10, x + 5, bottom - 6);
            g.DrawLine(outline, x + 19, top + 10, x + 19, bottom - 6);
        }

        private static void DrawWheelBogey(Graphics g, int x, int y, Brush brush, Pen outline)
        {
            g.FillRectangle(brush, x - 10, y - 8, 44, 14);
            g.DrawRectangle(outline, x - 10, y - 8, 44, 14);
            g.FillEllipse(Brushes.DimGray, x - 7, y - 2, 13, 13);
            g.FillEllipse(Brushes.DimGray, x + 17, y - 2, 13, 13);
        }

        private void DrawPlateCarrier(Graphics g, int centerX, int y, Font smallFont)
        {
            using (var metal = new SolidBrush(Color.FromArgb(49, 53, 52)))
            using (var plate = new LinearGradientBrush(new Rectangle(centerX - 74, y + 32, 148, 66), Color.FromArgb(78, 82, 80), Color.FromArgb(32, 35, 34), LinearGradientMode.Horizontal))
            using (var cablePen = new Pen(Color.FromArgb(45, 48, 48), 1.3F))
            using (var holeBrush = new SolidBrush(Color.FromArgb(14, 16, 16)))
            {
                g.FillRectangle(metal, centerX - 82, y, 164, 8);
                for (var i = 0; i < 12; i++)
                {
                    var x = centerX - 72 + i * 13;
                    g.DrawLine(cablePen, x + 5, y + 8, x + 5, y + 32);
                    g.FillRectangle(plate, x, y + 32, 9, 62);
                    g.FillEllipse(holeBrush, x + 2, y + 38, 5, 5);
                    g.DrawRectangle(Pens.Black, x, y + 32, 9, 62);
                }
            }
        }

        private void DrawHoistStatusLamps(Graphics g, int x, int y, string status, Brush green, Brush red)
        {
            for (var i = 0; i < 3; i++)
            {
                g.FillEllipse(IsHoistLampActive(status, i) ? green : red, x, y + i * 10, 8, 8);
            }
        }

        private void DrawStackLightAndCabinet(Graphics g, Rectangle rect, int startX, int baseY, Font smallFont)
        {
            using (var cabinet = new SolidBrush(Color.FromArgb(183, 185, 177)))
            using (var displayBrush = new SolidBrush(Color.FromArgb(24, 48, 58)))
            using (var border = new Pen(Color.FromArgb(47, 55, 56)))
            using (var green = new SolidBrush(Color.FromArgb(35, 220, 78)))
            using (var amber = new SolidBrush(Color.FromArgb(255, 178, 24)))
            using (var red = new SolidBrush(Color.FromArgb(240, 47, 47)))
            {
                var box = new Rectangle(startX - 72, baseY - 92, 52, 80);
                g.FillRectangle(cabinet, box);
                g.DrawRectangle(border, box);
                g.FillRectangle(displayBrush, box.Left + 11, box.Top + 12, 30, 18);
                g.FillEllipse(green, startX - 28, baseY - 205, 13, 13);
                g.FillEllipse(amber, startX - 28, baseY - 190, 13, 13);
                g.FillEllipse(_emergencyStop ? red : green, startX - 28, baseY - 175, 13, 13);
                g.DrawLine(border, startX - 22, baseY - 162, startX - 22, baseY - 94);
            }
        }

        private void DrawCommandRow(Graphics g, Rectangle rect, Font midFont, Font smallFont)
        {
            using (var back = new SolidBrush(Color.FromArgb(9, 26, 35)))
            using (var border = new Pen(Color.FromArgb(20, 52, 66)))
            {
                FillRoundedRectangle(g, back, rect, 6);
                g.DrawRectangle(border, rect);
            }

            var buttons = new[]
            {
                new[] { _autoMode ? "A  Auto" : "A  Auto", _autoMode ? "active" : "normal" },
                new[] { "Manual", !_autoMode ? "active" : "normal" },
                new[] { "Start Job", "start" },
                new[] { "Stop", "normal" },
                new[] { "Emergency Stop", "danger" },
                new[] { "Reset", "normal" },
                new[] { "IP Connection", "connected" }
            };
            var gap = 12;
            var width = Math.Max(90, (rect.Width - gap * (buttons.Length + 1)) / buttons.Length);
            for (var i = 0; i < buttons.Length; i++)
            {
                var button = new Rectangle(rect.Left + gap + i * (width + gap), rect.Top + 13, width, rect.Height - 26);
                DrawCommandButton(g, button, buttons[i][0], buttons[i][1], midFont, smallFont);
            }
        }

        private void DrawCommandButton(Graphics g, Rectangle rect, string text, string state, Font midFont, Font smallFont)
        {
            var backColor = Color.FromArgb(17, 38, 49);
            var borderColor = Color.FromArgb(55, 82, 96);
            var textColor = Color.FromArgb(232, 241, 245);
            if (state == "active")
            {
                backColor = Color.FromArgb(10, 62, 96);
                borderColor = Color.FromArgb(0, 170, 255);
                textColor = Color.FromArgb(0, 190, 255);
            }
            else if (state == "danger")
            {
                backColor = Color.FromArgb(82, 20, 20);
                borderColor = Color.FromArgb(240, 62, 55);
            }
            else if (state == "start")
            {
                borderColor = Color.FromArgb(35, 180, 80);
            }
            else if (state == "connected")
            {
                borderColor = Color.FromArgb(52, 92, 104);
            }

            using (var back = new SolidBrush(backColor))
            using (var pen = new Pen(borderColor))
            using (var textBrush = new SolidBrush(textColor))
            {
                FillRoundedRectangle(g, back, rect, 4);
                g.DrawRectangle(pen, rect);
                DrawCenteredString(g, text, midFont, textBrush, rect);
                if (state == "connected")
                {
                    g.FillEllipse(Brushes.LimeGreen, rect.Right - 18, rect.Top + rect.Height / 2 - 4, 8, 8);
                }
            }
        }

        private void DrawKpiCards(Graphics g, Rectangle rect, Font midFont, Font smallFont, Font valueFont)
        {
            if (rect.Width < 140)
            {
                return;
            }

            var cardGap = 10;
            var cardHeight = Math.Max(96, Math.Min(146, (rect.Height - 6 * cardGap) / 5));
            var cards = new[]
            {
                new[] { "TOTAL TANKS", _tanks.Count.ToString(), "Tanks" },
                new[] { "RUNNING TANKS", _tanks.Count(t => string.Equals(t.Status, Constants.StatusRunning, StringComparison.OrdinalIgnoreCase)).ToString(), "Running" },
                new[] { "HOIST POSITION", GetPrimaryHoistPositionText(), GetPrimaryHoistStatus() },
                new[] { "CURRENT STEP", _currentStepName, _remainingSeconds + "s remaining" },
                new[] { "ACTIVE ALARMS", GetActiveAlarmCount().ToString(), "Critical / Major / Minor" }
            };

            for (var i = 0; i < cards.Length; i++)
            {
                var card = new Rectangle(rect.Left, rect.Top + i * (cardHeight + cardGap), rect.Width, cardHeight);
                DrawKpiCard(g, card, cards[i][0], cards[i][1], cards[i][2], i == 4, midFont, smallFont, valueFont);
            }
        }

        private void DrawKpiCard(Graphics g, Rectangle rect, string title, string value, string caption, bool alarmCard, Font midFont, Font smallFont, Font valueFont)
        {
            using (var back = new LinearGradientBrush(rect, Color.FromArgb(10, 27, 37), Color.FromArgb(7, 18, 25), LinearGradientMode.Vertical))
            using (var pen = new Pen(Color.FromArgb(24, 63, 78)))
            using (var white = new SolidBrush(Color.FromArgb(234, 243, 247)))
            using (var muted = new SolidBrush(Color.FromArgb(160, 180, 190)))
            using (var cyan = new SolidBrush(alarmCard ? Color.FromArgb(255, 64, 64) : Color.FromArgb(0, 176, 255)))
            {
                FillRoundedRectangle(g, back, rect, 6);
                g.DrawRectangle(pen, rect);
                g.DrawString(title, smallFont, white, rect.Left + 16, rect.Top + 14);
                g.DrawLine(pen, rect.Left + 16, rect.Top + 42, rect.Right - 16, rect.Top + 42);
                DrawFittedString(g, value, valueFont, cyan, new Rectangle(rect.Left + 54, rect.Top + 52, rect.Width - 70, 38));
                DrawFittedString(g, caption, smallFont, white, new Rectangle(rect.Left + 54, rect.Top + 88, rect.Width - 70, 18));
                DrawCardIcon(g, rect.Left + 22, rect.Top + 62, alarmCard ? Color.FromArgb(255, 64, 64) : Color.FromArgb(190, 209, 218), alarmCard);
            }
        }

        private void DrawCardIcon(Graphics g, int x, int y, Color color, bool alarm)
        {
            using (var pen = new Pen(color, 2F))
            {
                if (alarm)
                {
                    g.DrawEllipse(pen, x, y, 28, 28);
                    g.DrawLine(pen, x + 14, y - 7, x + 14, y);
                    g.DrawLine(pen, x + 8, y + 31, x + 20, y + 31);
                    return;
                }

                g.DrawRectangle(pen, x + 2, y + 3, 24, 24);
                g.DrawArc(pen, x + 2, y - 4, 24, 14, 0, 180);
                g.DrawArc(pen, x + 2, y + 20, 24, 14, 0, -180);
            }
        }

        private void DrawAlarmTable(Graphics g, Rectangle rect, Font midFont, Font smallFont, Font tinyFont)
        {
            using (var back = new SolidBrush(Color.FromArgb(8, 24, 32)))
            using (var pen = new Pen(Color.FromArgb(21, 55, 70)))
            using (var white = new SolidBrush(Color.FromArgb(232, 240, 244)))
            using (var muted = new SolidBrush(Color.FromArgb(148, 169, 180)))
            {
                FillRoundedRectangle(g, back, rect, 6);
                g.DrawRectangle(pen, rect);
                g.DrawString("Active Alarms", midFont, white, rect.Left + 14, rect.Top + 10);

                var headerY = rect.Top + 38;
                var columns = GetAlarmColumns(rect);
                var labels = new[] { "Time", "Date", "Area", "Equipment", "Alarm Description", "Severity", "Status" };
                for (var i = 0; i < columns.Length && i < labels.Length; i++)
                {
                    DrawFittedString(g, labels[i], smallFont, white, columns[i]);
                }

                g.DrawLine(pen, rect.Left + 10, headerY + 22, rect.Right - 10, headerY + 22);
                var rows = BuildAlarmRows();
                var maxRows = Math.Max(1, (rect.Bottom - headerY - 30) / 22);
                for (var row = 0; row < rows.Count && row < maxRows; row++)
                {
                    var y = headerY + 28 + row * 22;
                    g.DrawLine(pen, rect.Left + 10, y + 18, rect.Right - 10, y + 18);
                    for (var col = 0; col < columns.Length && col < rows[row].Length; col++)
                    {
                        var cell = new Rectangle(columns[col].Left, y, columns[col].Width, 18);
                        var brush = col == 5 ? GetSeverityBrush(rows[row][col]) : (col == 6 ? Brushes.DeepSkyBlue : white);
                        DrawFittedString(g, rows[row][col], tinyFont, brush, cell);
                    }
                }

                if (rows.Count == 0)
                {
                    DrawFittedString(g, "No active alarms", smallFont, muted, new Rectangle(rect.Left + 14, headerY + 34, rect.Width - 28, 18));
                }
            }
        }

        private Rectangle[] GetAlarmColumns(Rectangle rect)
        {
            var left = rect.Left + 14;
            var top = rect.Top + 40;
            var width = rect.Width - 28;
            var parts = new[] { 80, 120, 150, 150, 280, 100, 100 };
            var total = parts.Sum();
            var columns = new Rectangle[parts.Length];
            var x = left;
            for (var i = 0; i < parts.Length; i++)
            {
                var w = Convert.ToInt32(width * (parts[i] / (double)total));
                columns[i] = new Rectangle(x, top, w - 6, 20);
                x += w;
            }

            return columns;
        }

        private List<string[]> BuildAlarmRows()
        {
            var rows = new List<string[]>();
            foreach (var tank in _tanks.Where(t => string.Equals(t.Status, Constants.StatusFault, StringComparison.OrdinalIgnoreCase) || string.Equals(t.Status, Constants.StatusWarning, StringComparison.OrdinalIgnoreCase)).Take(6))
            {
                rows.Add(new[]
                {
                    DateTime.Now.ToString("HH:mm:ss"),
                    DateTime.Now.ToString("MMM dd, yyyy"),
                    "Tank " + tank.TankNo.ToString("00") + " - " + GetShortProcessName(tank),
                    "Level / Temp",
                    string.Equals(tank.Status, Constants.StatusFault, StringComparison.OrdinalIgnoreCase) ? "Tank fault active" : "Tank warning active",
                    string.Equals(tank.Status, Constants.StatusFault, StringComparison.OrdinalIgnoreCase) ? "Critical" : "Major",
                    "Active"
                });
            }

            foreach (var hoist in _hoists.Where(h => string.Equals(h.Status, "Fault", StringComparison.OrdinalIgnoreCase) || string.Equals(h.Status, "EmergencyStop", StringComparison.OrdinalIgnoreCase)).Take(3))
            {
                rows.Add(new[]
                {
                    DateTime.Now.ToString("HH:mm:ss"),
                    DateTime.Now.ToString("MMM dd, yyyy"),
                    hoist.HoistName,
                    "Hoist Drive",
                    "Hoist movement interlock active",
                    "Critical",
                    "Active"
                });
            }

            if (_emergencyStop)
            {
                rows.Insert(0, new[] { DateTime.Now.ToString("HH:mm:ss"), DateTime.Now.ToString("MMM dd, yyyy"), "Plant", "Safety", "Emergency stop activated", "Critical", "Active" });
            }

            if (rows.Count == 0 && _jobs.Count > 0)
            {
                rows.Add(new[] { DateTime.Now.ToString("HH:mm:ss"), DateTime.Now.ToString("MMM dd, yyyy"), "Hoist H1", "Hoist Drive", "Wagon moving / production job active", "Minor", "Acknowledged" });
            }

            return rows;
        }

        private Brush GetSeverityBrush(string severity)
        {
            if (string.Equals(severity, "Critical", StringComparison.OrdinalIgnoreCase))
            {
                return Brushes.Tomato;
            }

            if (string.Equals(severity, "Major", StringComparison.OrdinalIgnoreCase))
            {
                return Brushes.Orange;
            }

            return Brushes.Gold;
        }

        private string GetPrimaryHoistStatus()
        {
            var hoist = _hoists.OrderBy(h => h.HoistName).FirstOrDefault();
            if (hoist != null && !string.IsNullOrEmpty(hoist.Status))
            {
                return hoist.Status;
            }

            return string.IsNullOrEmpty(_hoistStatus) ? "Idle" : _hoistStatus;
        }

        private string GetPrimaryHoistPositionText()
        {
            var hoist = _hoists.OrderBy(h => h.HoistName).FirstOrDefault();
            if (hoist != null)
            {
                return "Tank " + Math.Max(1, hoist.CurrentTankNo).ToString("00");
            }

            return "Tank " + Math.Max(1, Convert.ToInt32(_hoistPositionIndex) + 1).ToString("00");
        }

        private string GetPlantStateText()
        {
            return GetActiveAlarmCount() > 0 ? "Alarm" : "Normal";
        }

        private int GetActiveAlarmCount()
        {
            var tankAlarms = _tanks.Count(t => string.Equals(t.Status, Constants.StatusFault, StringComparison.OrdinalIgnoreCase) || string.Equals(t.Status, Constants.StatusWarning, StringComparison.OrdinalIgnoreCase));
            var hoistAlarms = _hoists.Count(h => string.Equals(h.Status, "Fault", StringComparison.OrdinalIgnoreCase) || string.Equals(h.Status, "EmergencyStop", StringComparison.OrdinalIgnoreCase));
            return tankAlarms + hoistAlarms + (_emergencyStop ? 1 : 0);
        }

        private string GetShortProcessName(TankModel tank)
        {
            var name = tank == null ? string.Empty : (!string.IsNullOrWhiteSpace(tank.ChemicalName) ? tank.ChemicalName : tank.Name);
            if (string.IsNullOrWhiteSpace(name))
            {
                return "PROCESS";
            }

            name = name.Replace("Tank", string.Empty).Replace("Station", "ST").Trim();
            return name.Length > 10 ? name.Substring(0, 10).ToUpperInvariant() : name.ToUpperInvariant();
        }

        private static void FillRoundedRectangle(Graphics g, Brush brush, Rectangle rect, int radius)
        {
            using (var path = CreateRoundedRectangle(rect, radius))
            {
                g.FillPath(brush, path);
            }
        }

        private static GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            var diameter = radius * 2;
            if (diameter <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }

            path.AddArc(rect.Left, rect.Top, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Top, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.Left, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void DrawLineRow(Graphics g, int lineId, string lineLabel, IList<TankModel> lineTanks, int left, int top, int rowHeight, int cellWidth, int tankWidth, Font titleFont, Font smallFont, Font digitalFont, Brush textBrush, Pen railPen, Pen thinRailPen)
        {
            var railY = top + 20;
            var right = Math.Min(Width - 38, left + (cellWidth * Math.Max(1, lineTanks.Count)));

            using (var rowBrush = new SolidBrush(Color.FromArgb(226, 236, 241)))
            using (var rowPen = new Pen(Color.FromArgb(159, 181, 193)))
            using (var rowLabelBrush = new SolidBrush(Color.FromArgb(38, 95, 143)))
            {
                g.FillRectangle(rowBrush, left - 8, top - 12, right - left + 16, rowHeight);
                g.DrawRectangle(rowPen, left - 8, top - 12, right - left + 16, rowHeight);
                g.DrawString(lineLabel, titleFont, rowLabelBrush, left - 2, top - 10);
            }

            g.DrawLine(railPen, left, railY, right, railY);
            g.DrawLine(thinRailPen, left, railY + 5, right, railY + 5);

            for (var column = 0; column < lineTanks.Count; column++)
            {
                DrawTankCell(g, lineTanks[column], left + (column * cellWidth) + ((cellWidth - tankWidth) / 2), top + 35, tankWidth, titleFont, smallFont, digitalFont, textBrush);
            }
        }

        private void DrawTankCell(Graphics g, TankModel tank, int x, int y, int width, Font titleFont, Font smallFont, Font digitalFont, Brush textBrush)
        {
            var borderColor = GetStatusColor(tank.Status);
            var tankRect = new Rectangle(x + 8, y + 36, Math.Max(42, width - 16), 82);
            var selected = _selectedTankId.HasValue && _selectedTankId.Value == tank.Id;

            using (var borderPen = new Pen(selected ? Color.FromArgb(255, 255, 255) : borderColor, selected ? 3F : 2F))
            using (var liquidBrush = new LinearGradientBrush(tankRect, Color.FromArgb(56, 174, 190), Color.FromArgb(105, 205, 214), LinearGradientMode.Vertical))
            using (var tankBackBrush = new SolidBrush(Color.FromArgb(232, 246, 249)))
            using (var redBrush = new SolidBrush(Color.FromArgb(225, 64, 55)))
            using (var greenBrush = new SolidBrush(Color.FromArgb(0, 170, 80)))
            using (var capBrush = new SolidBrush(Color.FromArgb(208, 222, 230)))
            using (var rowBrush = new SolidBrush(Color.FromArgb(8, 18, 20)))
            using (var capTextBrush = new SolidBrush(Color.White))
            {
                g.FillRectangle(capBrush, x + 5, y - 2, width - 10, 22);
                DrawCenteredString(g, "ST-" + GetStationNumber(tank), titleFont, textBrush, new Rectangle(x + 5, y - 2, width - 10, 20));
                g.FillEllipse(string.Equals(tank.Status, Constants.StatusFault, StringComparison.OrdinalIgnoreCase) ? redBrush : greenBrush, x + width - 16, y + 3, 10, 10);

                g.FillRectangle(tankBackBrush, tankRect);
                var level = GetLevelPercent(tank);
                var fillHeight = Convert.ToInt32(tankRect.Height * level);
                g.FillRectangle(liquidBrush, tankRect.Left + 3, tankRect.Bottom - fillHeight, tankRect.Width - 6, Math.Max(0, fillHeight - 3));
                g.DrawRectangle(borderPen, tankRect);

                DrawScadaValueRow(g, rowBrush, new Rectangle(x + 10, y + 23, width - 20, 15), GetActualProcessSeconds(tank).ToString(), digitalFont, Color.FromArgb(55, 210, 100));
                DrawScadaValueRow(g, rowBrush, new Rectangle(x + 10, y + 40, width - 20, 15), GetPreviousProcessSeconds(tank).ToString(), digitalFont, Color.FromArgb(245, 225, 75));
                DrawScadaValueRow(g, rowBrush, new Rectangle(x + 10, y + 57, width - 20, 15), tank.CurrentAmps.ToString("0"), digitalFont, Color.FromArgb(70, 150, 235));
                DrawScadaValueRow(g, rowBrush, new Rectangle(x + 10, y + 101, width - 20, 15), tank.TemperatureCelsius.ToString("0"), digitalFont, Color.FromArgb(235, 80, 70));

                if (string.Equals(tank.Status, Constants.StatusFault, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(tank.Status, Constants.StatusWarning, StringComparison.OrdinalIgnoreCase))
                {
                    g.FillRectangle(redBrush, x + 11, y + 106, width - 22, 9);
                }

                if (IsTankOccupied(tank))
                {
                    using (var occupiedBrush = new SolidBrush(Color.FromArgb(245, 180, 55)))
                    {
                        g.FillRectangle(occupiedBrush, x + 10, y + 107, width - 20, 8);
                    }
                }

                DrawCenteredString(g, tank.Status + "  L" + Convert.ToInt32(GetLevelPercent(tank) * 100) + "%", smallFont, textBrush, new Rectangle(x, y + 124, width, 14));
            }

            _tankHitBoxes[tank.Id] = new Rectangle(x, y, width, 142);
        }

        private void DrawHoists(Graphics g, int lineId, IList<TankModel> lineTanks, int left, int top, int cellWidth, Font titleFont, Font smallFont, Brush textBrush)
        {
            if (lineTanks.Count == 0)
            {
                return;
            }

            if (_hoists.Count == 0)
            {
                var clampedIndex = Math.Max(0, Math.Min(lineTanks.Count - 1, _hoistPositionIndex));
                DrawHoistBody(g, "H1", clampedIndex, _hoistStatus, _autoMode, left, top, cellWidth, titleFont, smallFont, textBrush);
                return;
            }

            foreach (var hoist in _hoists.Where(h => h.LineId == lineId))
            {
                var position = Math.Max(0, Math.Min(lineTanks.Count - 1, hoist.PositionIndex));
                DrawHoistBody(g, hoist.HoistName, position, hoist.Status, hoist.IsAuto, left, top, cellWidth, titleFont, smallFont, textBrush);
            }
        }

        private void DrawHoistBody(Graphics g, string hoistName, double positionIndex, string status, bool isAuto, int left, int top, int cellWidth, Font titleFont, Font smallFont, Brush textBrush)
        {
            var clampedIndex = Math.Max(0, positionIndex);
            var columnPosition = clampedIndex;
            var x = left + Convert.ToInt32(columnPosition * cellWidth) + (cellWidth / 2) - 22;
            var y = top - 40;

            using (var hoistBrush = new SolidBrush(_emergencyStop || string.Equals(status, "EmergencyStop", StringComparison.OrdinalIgnoreCase) ? Color.FromArgb(190, 45, 42) : GetHoistColor(status)))
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
                g.DrawString(hoistName, titleFont, textBrush, x - 2, y - 18);

                for (var i = 0; i < 4; i++)
                {
                    var active = IsHoistLampActive(status, i);
                    g.FillEllipse(active ? greenBrush : redBrush, x + 12, y + 8 + (i * 15), 12, 12);
                }

                DrawDigitalBox(g, new Rectangle(x - 54, y + 34, 40, 18), Math.Max(1, Convert.ToInt32(positionIndex) + 1).ToString(), titleFont);
                g.DrawString("UP", smallFont, textBrush, x + 50, y + 4);
                g.DrawString("UP SLOW", smallFont, textBrush, x + 50, y + 19);
                g.DrawString("DOWN SLOW", smallFont, textBrush, x + 50, y + 34);
                g.DrawString("DOWN", smallFont, textBrush, x + 50, y + 49);
                g.DrawString(status, smallFont, textBrush, x + 50, y + 66);
                g.DrawString(isAuto ? "AUTO" : "MANUAL", titleFont, textBrush, x + 50, y + 82);
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
                g.FillRectangle(panelBrush, 8, 46, 132, Height - 94);
                g.FillRectangle(redBrush, 50, 72, 36, 36);
                g.DrawString("ALARMS", titleFont, textBrush, 40, 116);
                g.DrawString("OPERATOR", smallFont, mutedBrush, 22, 52);

                DrawLegend(g, "ACTU. PRO. TIME", 18, 178, Color.FromArgb(58, 150, 95), titleFont);
                DrawLegend(g, "PREV. PRO. TIME", 18, 202, Color.FromArgb(210, 190, 60), titleFont);
                DrawLegend(g, "ACTU. AMPERE", 18, 226, Color.FromArgb(74, 116, 180), titleFont);
                DrawLegend(g, "TEMPERATURE", 18, 250, Color.FromArgb(204, 91, 73), titleFont);

                g.FillRectangle(infoBrush, 20, 298, 104, 160);
                g.DrawString("Information", smallFont, infoTextBrush, 42, 318);
                DrawFittedString(g, _currentStepName, smallFont, infoTextBrush, new Rectangle(28, 348, 88, 18));
                DrawFittedString(g, "Lines " + GetLineCount() + "  Tanks " + _tanks.Count, smallFont, infoTextBrush, new Rectangle(28, 370, 88, 18));
                DrawFittedString(g, "Hoists " + _hoists.Count + "  Jobs " + _jobs.Count, smallFont, infoTextBrush, new Rectangle(28, 392, 88, 18));
                DrawFittedString(g, "Faults " + GetTankFaultCount() + "  Warn " + GetTankWarningCount(), smallFont, infoTextBrush, new Rectangle(28, 414, 88, 18));
                DrawFittedString(g, _emergencyStop ? "E-STOP" : "Remain " + _remainingSeconds + "s", titleFont, infoTextBrush, new Rectangle(28, 434, 88, 18));
            }
        }

        private void DrawProductionPanels(Graphics g, int left, int top, Font titleFont, Font smallFont)
        {
            if (top > Height - 68)
            {
                return;
            }

            using (var panelBrush = new SolidBrush(Color.FromArgb(232, 238, 242)))
            using (var borderPen = new Pen(Color.FromArgb(62, 132, 166), 2F))
            using (var cellBrush = new SolidBrush(Color.FromArgb(206, 226, 235)))
            using (var textBrush = new SolidBrush(Color.FromArgb(31, 45, 58)))
            {
                var totalWidth = Width - left - 44;
                var panelHeight = Math.Max(112, Math.Min(136, Height - top - 22));
                var wagonWidth = Math.Max(430, Convert.ToInt32(totalWidth * 0.42));
                var cycleWidth = Math.Max(170, Convert.ToInt32(totalWidth * 0.17));
                var detailWidth = Math.Max(320, totalWidth - wagonWidth - cycleWidth - 20);
                DrawWagonStatusPanel(g, new Rectangle(left, top, wagonWidth, panelHeight), panelBrush, borderPen, cellBrush, textBrush, titleFont, smallFont);
                DrawCycleCountPanel(g, new Rectangle(left + wagonWidth + 10, top, cycleWidth, panelHeight), panelBrush, borderPen, cellBrush, textBrush, titleFont, smallFont);
                DrawLineDetailPanel(g, new Rectangle(left + wagonWidth + cycleWidth + 20, top, detailWidth, panelHeight), panelBrush, borderPen, cellBrush, textBrush, titleFont, smallFont);
            }
        }

        private int GetProductionPanelTop()
        {
            return Math.Max(248, Height - 156);
        }

        private void DrawRecipePanel(Graphics g, Rectangle rect, Brush panelBrush, Pen borderPen, Brush cellBrush, Brush textBrush, Font titleFont, Font smallFont)
        {
            DrawPanelShell(g, rect, "Recipe By Line", panelBrush, borderPen, textBrush, titleFont);
            var rows = _processSteps
                .GroupBy(step => step.LineId)
                .OrderBy(group => group.Key)
                .Select(group => "L" + group.Key + ": " + string.Join(" > ", group.OrderBy(step => step.StepNo).Select(step => "T" + step.TankNo + " " + step.DurationSeconds + "s").ToArray()))
                .ToList();
            DrawPanelRows(g, rect, cellBrush, borderPen, textBrush, smallFont, rows);
        }

        private void DrawHoistQueuePanel(Graphics g, Rectangle rect, Brush panelBrush, Pen borderPen, Brush cellBrush, Brush textBrush, Font titleFont, Font smallFont)
        {
            DrawPanelShell(g, rect, "Hoist Queue", panelBrush, borderPen, textBrush, titleFont);
            var rows = _hoists
                .OrderBy(hoist => hoist.LineId)
                .ThenBy(hoist => hoist.HoistName)
                .Select(hoist => hoist.HoistName + "  L" + hoist.LineId + "-T" + hoist.CurrentTankNo + " -> T" + hoist.TargetTankNo + "  " + hoist.Status)
                .ToList();
            DrawPanelRows(g, rect, cellBrush, borderPen, textBrush, smallFont, rows);
        }

        private void DrawJobPanel(Graphics g, Rectangle rect, Brush panelBrush, Pen borderPen, Brush cellBrush, Brush textBrush, Font titleFont, Font smallFont)
        {
            DrawPanelShell(g, rect, "Active Jobs", panelBrush, borderPen, textBrush, titleFont);
            var rows = _jobs
                .OrderBy(job => job.LineId)
                .ThenBy(job => job.JobId)
                .Select(job => "L" + job.LineId + "  T" + job.CurrentTank + "  " + job.Status + "  " + job.RemainingSeconds + "s  " + GetDirectionText(job))
                .ToList();
            DrawPanelRows(g, rect, cellBrush, borderPen, textBrush, smallFont, rows);
        }

        private void DrawOccupancyPanel(Graphics g, Rectangle rect, Brush panelBrush, Pen borderPen, Brush cellBrush, Brush textBrush, Font titleFont, Font smallFont)
        {
            DrawPanelShell(g, rect, "Tank Occupancy By Line", panelBrush, borderPen, textBrush, titleFont);
            var rows = _tanks
                .GroupBy(tank => tank.LineId)
                .OrderBy(group => group.Key)
                .Select(group => BuildOccupancyLine(group.Key, group.OrderBy(tank => tank.TankNo).ToList()))
                .ToList();
            DrawPanelRows(g, rect, cellBrush, borderPen, textBrush, smallFont, rows);
        }

        private void DrawWagonStatusPanel(Graphics g, Rectangle rect, Brush panelBrush, Pen borderPen, Brush cellBrush, Brush textBrush, Font titleFont, Font smallFont)
        {
            DrawPanelShell(g, rect, "Wagon / Hoist Status", panelBrush, borderPen, textBrush, titleFont);
            var headerY = rect.Top + 28;
            var columns = new[] { 68, 92, 92, 92, 64 };
            var labels = new[] { "WAGON", "CURRENT TANK", "WAGON GET", "WAGON PUT", "STEP" };
            var x = rect.Left + 8;

            for (var i = 0; i < labels.Length; i++)
            {
                DrawPanelRow(g, cellBrush, borderPen, textBrush, smallFont, x, headerY, columns[i], labels[i]);
                x += columns[i] + 4;
            }

            var rows = BuildWagonRows();
            if (rows.Count == 0)
            {
                DrawPanelRow(g, cellBrush, borderPen, textBrush, smallFont, rect.Left + 8, headerY + 20, rect.Width - 16, "No active wagon or hoist data");
                return;
            }

            var maxRows = Math.Max(1, (rect.Height - 54) / 20);
            for (var row = 0; row < rows.Count && row < maxRows; row++)
            {
                x = rect.Left + 8;
                var values = rows[row];
                for (var col = 0; col < values.Length && col < columns.Length; col++)
                {
                    DrawPanelRow(g, cellBrush, borderPen, textBrush, smallFont, x, headerY + 20 + (row * 20), columns[col], values[col]);
                    x += columns[col] + 4;
                }
            }
        }

        private void DrawCycleCountPanel(Graphics g, Rectangle rect, Brush panelBrush, Pen borderPen, Brush cellBrush, Brush textBrush, Font titleFont, Font smallFont)
        {
            DrawPanelShell(g, rect, "Cycle Count", panelBrush, borderPen, textBrush, titleFont);

            using (var countBrush = new LinearGradientBrush(new Rectangle(rect.Left + 28, rect.Top + 46, rect.Width - 56, 48), Color.FromArgb(255, 230, 70), Color.FromArgb(240, 83, 45), LinearGradientMode.Vertical))
            using (var countPen = new Pen(Color.FromArgb(0, 145, 80), 3F))
            using (var countFont = new Font("Segoe UI Semibold", 20F, FontStyle.Bold))
            using (var whiteBrush = new SolidBrush(Color.White))
            {
                var countRect = new Rectangle(rect.Left + 28, rect.Top + 46, rect.Width - 56, 48);
                g.FillRectangle(countBrush, countRect);
                g.DrawRectangle(countPen, countRect);
                DrawCenteredString(g, GetCycleCountText(), countFont, whiteBrush, countRect);
            }

            var activeLines = _jobs.Select(job => job.LineId).Distinct().Count();
            DrawPanelRow(g, cellBrush, borderPen, textBrush, smallFont, rect.Left + 8, rect.Bottom - 34, rect.Width - 16, activeLines + " active line(s)");
        }

        private void DrawLineDetailPanel(Graphics g, Rectangle rect, Brush panelBrush, Pen borderPen, Brush cellBrush, Brush textBrush, Font titleFont, Font smallFont)
        {
            DrawPanelShell(g, rect, "Line Details", panelBrush, borderPen, textBrush, titleFont);
            var rows = _tanks
                .GroupBy(tank => tank.LineId)
                .OrderBy(group => group.Key)
                .Select(group => BuildLineDetail(group.Key, group.OrderBy(tank => tank.TankNo).ToList()))
                .ToList();
            DrawPanelRows(g, rect, cellBrush, borderPen, textBrush, smallFont, rows);
        }

        private List<string[]> BuildWagonRows()
        {
            var rows = new List<string[]>();
            foreach (var hoist in _hoists.OrderBy(hoist => hoist.LineId).ThenBy(hoist => hoist.HoistName))
            {
                var job = GetJobForLine(hoist.LineId);
                rows.Add(new[]
                {
                    string.IsNullOrEmpty(hoist.HoistName) ? "H" + hoist.HoistId : hoist.HoistName,
                    GetLineShortCode(hoist.LineId) + "-T" + hoist.CurrentTankNo,
                    "T" + GetHoistFromTank(hoist) + "-T" + GetHoistToTank(hoist),
                    "T" + hoist.TargetTankNo,
                    job == null ? "-" : job.CurrentStep.ToString()
                });
            }

            if (rows.Count == 0)
            {
                foreach (var wagon in _wagons.OrderBy(wagon => wagon.WagonCode))
                {
                    rows.Add(new[]
                    {
                        wagon.WagonCode,
                        GetWagonTankText(wagon),
                        GetWagonTankText(wagon),
                        "-",
                        wagon.State
                    });
                }
            }

            return rows;
        }

        private string BuildLineDetail(int lineId, IList<TankModel> tanks)
        {
            var lineHoists = _hoists.Where(hoist => hoist.LineId == lineId).ToList();
            var activeJobs = _jobs.Count(job => job.LineId == lineId);
            var faults = tanks.Count(tank => string.Equals(tank.Status, Constants.StatusFault, StringComparison.OrdinalIgnoreCase));
            var warnings = tanks.Count(tank => string.Equals(tank.Status, Constants.StatusWarning, StringComparison.OrdinalIgnoreCase));
            var activeTank = tanks.FirstOrDefault(IsTankOccupied);
            var activeText = activeTank == null ? "Idle" : "T" + activeTank.TankNo + " " + activeTank.ChemicalName;
            return GetLineShortCode(lineId) + "  Tanks " + tanks.Count + "  Hoists " + lineHoists.Count + "  Jobs " + activeJobs + "  F:" + faults + " W:" + warnings + "  " + activeText;
        }

        private JobModel GetJobForLine(int lineId)
        {
            return _jobs
                .Where(job => job.LineId == lineId)
                .OrderBy(job => job.JobId)
                .FirstOrDefault();
        }

        private string GetCycleCountText()
        {
            var activeJobs = _jobs.Count;
            if (activeJobs > 0)
            {
                return activeJobs.ToString();
            }

            return _hoists.Count(hoist => !string.Equals(hoist.Status, "Idle", StringComparison.OrdinalIgnoreCase)).ToString();
        }

        private int GetLineCount()
        {
            return _tanks
                .Select(tank => tank.LineId <= 0 ? 1 : tank.LineId)
                .Distinct()
                .Count();
        }

        private int GetTankFaultCount()
        {
            return _tanks.Count(tank => string.Equals(tank.Status, Constants.StatusFault, StringComparison.OrdinalIgnoreCase));
        }

        private int GetTankWarningCount()
        {
            return _tanks.Count(tank => string.Equals(tank.Status, Constants.StatusWarning, StringComparison.OrdinalIgnoreCase));
        }

        private static int GetHoistFromTank(HoistModel hoist)
        {
            return hoist == null || hoist.FromTank <= 0 ? 1 : hoist.FromTank;
        }

        private static int GetHoistToTank(HoistModel hoist)
        {
            if (hoist == null)
            {
                return 1;
            }

            if (hoist.ToTank > 0)
            {
                return hoist.ToTank;
            }

            return hoist.HomeTank > 0 ? hoist.HomeTank : 1;
        }

        private void DrawPanelRows(Graphics g, Rectangle rect, Brush cellBrush, Pen borderPen, Brush textBrush, Font smallFont, IList<string> rows)
        {
            var rowHeight = 16;
            var maxRows = Math.Max(1, (rect.Height - 34) / (rowHeight + 2));
            if (rows.Count == 0)
            {
                DrawPanelRow(g, cellBrush, borderPen, textBrush, smallFont, rect.Left + 8, rect.Top + 28, rect.Width - 16, "No data");
                return;
            }

            for (var i = 0; i < rows.Count && i < maxRows; i++)
            {
                DrawPanelRow(g, cellBrush, borderPen, textBrush, smallFont, rect.Left + 8, rect.Top + 28 + i * (rowHeight + 2), rect.Width - 16, rows[i]);
            }

            if (rows.Count > maxRows)
            {
                DrawPanelRow(g, cellBrush, borderPen, textBrush, smallFont, rect.Left + 8, rect.Top + 28 + (maxRows - 1) * (rowHeight + 2), rect.Width - 16, "+" + (rows.Count - maxRows + 1) + " more lines/items");
            }
        }

        private string BuildOccupancyLine(int lineId, IList<TankModel> tanks)
        {
            var occupied = tanks.Where(IsTankOccupied).Select(tank => "T" + tank.TankNo).ToList();
            var faultCount = tanks.Count(tank => string.Equals(tank.Status, Constants.StatusFault, StringComparison.OrdinalIgnoreCase));
            var warningCount = tanks.Count(tank => string.Equals(tank.Status, Constants.StatusWarning, StringComparison.OrdinalIgnoreCase));
            var state = occupied.Count == 0 ? "Empty" : "Occupied " + string.Join(",", occupied.ToArray());
            if (faultCount > 0 || warningCount > 0)
            {
                state += "  F:" + faultCount + " W:" + warningCount;
            }

            return GetLineShortCode(lineId) + "  " + state + " / " + tanks.Count + " tanks";
        }

        private static string GetDirectionText(JobModel job)
        {
            if (job == null)
            {
                return string.Empty;
            }

            return job.StepDirection < 0 ? "Reverse" : "Forward";
        }

        private static void DrawPanelShell(Graphics g, Rectangle rect, string title, Brush panelBrush, Pen borderPen, Brush textBrush, Font titleFont)
        {
            g.FillRectangle(panelBrush, rect);
            g.DrawRectangle(borderPen, rect);
            g.DrawString(title, titleFont, textBrush, rect.Left + 8, rect.Top + 6);
        }

        private static void DrawPanelRow(Graphics g, Brush cellBrush, Pen borderPen, Brush textBrush, Font font, int x, int y, int width, string text)
        {
            var rowRect = new Rectangle(x, y, width, 16);
            g.FillRectangle(cellBrush, rowRect);
            g.DrawRectangle(borderPen, rowRect);
            DrawFittedString(g, text, font, textBrush, new Rectangle(x + 4, y + 1, width - 8, 14));
        }

        private void DrawLegend(Graphics g, string text, int x, int y, Color color, Font font)
        {
            using (var brush = new SolidBrush(color))
            using (var textBrush = new SolidBrush(Color.FromArgb(22, 32, 42)))
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

        private static void DrawScadaValueRow(Graphics g, Brush backBrush, Rectangle rect, string text, Font font, Color color)
        {
            using (var textBrush = new SolidBrush(color))
            using (var pen = new Pen(Color.FromArgb(60, 80, 70)))
            {
                g.FillRectangle(backBrush, rect);
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
                g.DrawString("PLANT OVERVIEW", titleFont, textBrush, 18, 10);
                g.DrawString((_autoMode ? "AUTO" : "MANUAL") + " | " + _hoistStatus + " | " + _currentStepName, smallFont, mutedBrush, 260, 13);
            }
        }

        private void DrawPlantFrame(Graphics g)
        {
            using (var borderPen = new Pen(Color.FromArgb(71, 93, 110), 2F))
            using (var gridPen = new Pen(Color.FromArgb(198, 213, 221)))
            {
                g.DrawRectangle(borderPen, 4, 44, Width - 9, Height - 49);

                for (var x = 24; x < Width - 42; x += 64)
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
            _startMenuItem = _tankMenu.Items.Add("Start Tank", null, delegate { OnTankEvent(StartClicked, _contextTankId); });
            _stopMenuItem = _tankMenu.Items.Add("Stop Tank", null, delegate { OnTankEvent(StopClicked, _contextTankId); });
            _faultMenuItem = _tankMenu.Items.Add("Set Fault", null, delegate { OnTankEvent(FaultClicked, _contextTankId); });
            _resetMenuItem = _tankMenu.Items.Add("Reset Tank", null, delegate { OnTankEvent(ResetClicked, _contextTankId); });
            _removeMenuItem = _tankMenu.Items.Add("Remove Tank", null, delegate { OnTankEvent(RemoveClicked, _contextTankId); });
            UpdateTankMenuAccess();
        }

        private void UpdateTankMenuAccess()
        {
            SetMenuItemAccess(_startMenuItem, CanOperateTanks);
            SetMenuItemAccess(_stopMenuItem, CanOperateTanks);
            SetMenuItemAccess(_faultMenuItem, CanOperateTanks);
            SetMenuItemAccess(_resetMenuItem, CanOperateTanks);
            SetMenuItemAccess(_removeMenuItem, CanEngineerTanks);
        }

        private static void SetMenuItemAccess(ToolStripItem item, bool visible)
        {
            if (item == null)
            {
                return;
            }

            item.Available = visible;
            item.Visible = visible;
            item.Enabled = visible;
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
                if (_tanks[i].Id == tankId.Value)
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
                    return step.StepNo + ". " + step.ProcessName;
                }
            }

            return tank.ChemicalName;
        }

        private static int GetStationNumber(TankModel tank)
        {
            if (tank == null)
            {
                return 0;
            }

            return tank.TankNo;
        }

        private string GetLineDisplayName(int lineId, IList<TankModel> lineTanks, int displayIndex)
        {
            var lineName = lineTanks == null ? string.Empty : lineTanks.Select(t => t.LineName).FirstOrDefault(name => !string.IsNullOrWhiteSpace(name));
            if (!string.IsNullOrWhiteSpace(lineName))
            {
                return FormatLineDisplayName(lineName, displayIndex);
            }

            return "LINE " + displayIndex;
        }

        private string GetLineShortCode(int lineId)
        {
            var lineName = _tanks
                .Where(tank => tank.LineId == lineId)
                .Select(tank => tank.LineName)
                .FirstOrDefault(name => !string.IsNullOrWhiteSpace(name));
            if (!string.IsNullOrWhiteSpace(lineName))
            {
                var digits = new string(lineName.Where(char.IsDigit).ToArray());
                int parsedLineNo;
                if (int.TryParse(digits, out parsedLineNo) && parsedLineNo > 0 && parsedLineNo <= 99)
                {
                    return "L" + parsedLineNo;
                }

                if (string.IsNullOrEmpty(digits))
                {
                    return lineName;
                }
            }

            var orderedLineIds = _tanks
                .Select(tank => tank.LineId <= 0 ? 1 : tank.LineId)
                .Distinct()
                .OrderBy(id => id)
                .ToList();
            var index = orderedLineIds.IndexOf(lineId <= 0 ? 1 : lineId);
            if (index >= 0)
            {
                return "L" + (index + 1);
            }

            return "L" + lineId;
        }

        private static string FormatLineDisplayName(string lineName, int fallbackIndex)
        {
            var digits = new string(lineName.Where(char.IsDigit).ToArray());
            int parsedLineNo;
            if (int.TryParse(digits, out parsedLineNo) && parsedLineNo > 0 && parsedLineNo <= 99)
            {
                return "LINE " + parsedLineNo;
            }

            if (!string.IsNullOrEmpty(digits))
            {
                return "LINE " + fallbackIndex;
            }

            return lineName.ToUpperInvariant();
        }

        private int GetActualProcessSeconds(TankModel tank)
        {
            foreach (var job in _jobs)
            {
                if (job.LineId == tank.LineId && job.CurrentTank == tank.TankNo && IsOccupyingJobStatus(job.Status))
                {
                    return Math.Max(0, job.RemainingSeconds);
                }
            }

            return 0;
        }

        private int GetPreviousProcessSeconds(TankModel tank)
        {
            foreach (var step in _processSteps)
            {
                if (step.LineId == tank.LineId && step.TankNo == tank.TankNo)
                {
                    return Math.Max(0, step.DurationSeconds);
                }
            }

            return 0;
        }

        private bool IsHoistLampActive(string status, int lampIndex)
        {
            if (_emergencyStop)
            {
                return false;
            }

            if (string.Equals(status, "Moving", StringComparison.OrdinalIgnoreCase))
            {
                return lampIndex < 2;
            }

            if (string.Equals(status, "Processing", StringComparison.OrdinalIgnoreCase))
            {
                return lampIndex == 0 || lampIndex == 2;
            }

            if (string.Equals(status, "Lowering", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(status, "Lifting", StringComparison.OrdinalIgnoreCase))
            {
                return lampIndex != 3;
            }

            return lampIndex == 0;
        }

        private bool IsTankOccupied(TankModel tank)
        {
            foreach (var job in _jobs)
            {
                if (job.LineId == tank.LineId && job.CurrentTank == tank.TankNo && IsOccupyingJobStatus(job.Status))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsOccupyingJobStatus(string status)
        {
            return string.Equals(status, "Lowering", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(status, "Processing", StringComparison.OrdinalIgnoreCase);
        }

        private static Color GetHoistColor(string status)
        {
            if (string.Equals(status, "Fault", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(status, "EmergencyStop", StringComparison.OrdinalIgnoreCase))
            {
                return Color.FromArgb(190, 45, 42);
            }

            if (string.Equals(status, "Processing", StringComparison.OrdinalIgnoreCase))
            {
                return Color.FromArgb(0, 165, 90);
            }

            if (string.Equals(status, "Moving", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(status, "Lowering", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(status, "Lifting", StringComparison.OrdinalIgnoreCase))
            {
                return Color.FromArgb(245, 180, 45);
            }

            return Color.FromArgb(80, 145, 215);
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
