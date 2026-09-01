using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using USR_ElectroPilot.Helpers;
using USR_ElectroPilot.Models;
using USR_ElectroPilot.Services;
using USR_ElectroPilot.ThreeD.Views;

namespace USR_ElectroPilot.Controls
{
    public class Plant3DHostControl : UserControl
    {
        private readonly ElementHost _elementHost;
        private readonly bool _deferPlantViewInitialization;
        private readonly Label _viewportLoadingLabel;
        private Plant3DView _plantView;
        private readonly Label _plantStateLabel;
        private readonly Label _modeLabel;
        private readonly Label _hoistLabel;
        private readonly Label _communicationLabel;
        private readonly Label _clockLabel;
        private readonly Label _operatorLabel;
        private readonly Label _totalTankValue;
        private readonly Label _runningTankValue;
        private readonly Label _hoistPositionValue;
        private readonly Label _currentStepValue;
        private readonly Label _activeAlarmValue;
        private readonly Label _criticalAlarmValue;
        private readonly Label _majorAlarmValue;
        private readonly Label _minorAlarmValue;
        private readonly Label _selectedEquipmentTitle;
        private readonly Label _selectedLevelTemperatureValue;
        private readonly Label _selectedElectricalValue;
        private readonly Label _selectedMotorValue;
        private readonly Label _selectedQualityValue;
        private readonly DataGridView _alarmGrid;
        private readonly ContextMenuStrip _viewMenu;
        private readonly ContextMenuStrip _tankMenu;
        private readonly ContextMenuStrip _recipeMenu;
        private readonly ContextMenuStrip _settingsMenu;
        private readonly ContextMenuStrip _accountMenu;
        private readonly List<Button> _operatorButtons = new List<Button>();
        private readonly List<ScadaNavButton> _navigationButtons = new List<ScadaNavButton>();
        private PendingPlantData _pendingPlantData;
        private int? _pendingSelectedTankId;
        private bool _disposed;
        private bool _boundAutoMode = true;
        private bool _boundEmergencyStop;
        private bool _boundHasActiveProduction;
        private bool _boundHasRunningTank;
        private int _boundActiveAlarmCount;

        public event EventHandler<TankControlEventArgs> TankSelected;
        public event EventHandler<TankControlEventArgs> StartClicked;
        public event EventHandler<TankControlEventArgs> StopClicked;
        public event EventHandler<TankControlEventArgs> FaultClicked;
        public event EventHandler<TankControlEventArgs> ResetClicked;
        public event EventHandler<TankControlEventArgs> RemoveClicked;
        public event EventHandler<string> DashboardCommandRequested;

        public bool CanOperateTanks { get; set; }
        public bool CanEngineerTanks { get; set; }

        public Plant3DHostControl()
            : this(false)
        {
        }

        public Plant3DHostControl(bool deferPlantViewInitialization)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);

            _deferPlantViewInitialization = deferPlantViewInitialization;

            _elementHost = new ElementHost
            {
                Dock = DockStyle.Fill,
                Visible = !deferPlantViewInitialization
            };
            _viewportLoadingLabel = new Label
            {
                Dock = DockStyle.Fill,
                Text = "Loading 3D plant view...",
                BackColor = Color.FromArgb(4, 13, 18),
                ForeColor = Color.DeepSkyBlue,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = deferPlantViewInitialization
            };

            if (!deferPlantViewInitialization)
            {
                EnsurePlantViewCreated();
            }

            _plantStateLabel = CreateHeaderValue("Plant: Normal", Color.LimeGreen, "Plant", true, false);
            _modeLabel = CreateHeaderValue("Auto Mode", Color.DeepSkyBlue, "Mode", false, true);
            _hoistLabel = CreateHeaderValue("Hoist H1  Idle", Color.LimeGreen, "Hoist", true, false);
            _communicationLabel = CreateHeaderValue("Data UNKNOWN", Color.Gold, "Connection", true, false);
            _clockLabel = CreateHeaderValue(DateTime.Now.ToString("HH:mm:ss"), Color.White, "Clock", false, false);
            _operatorLabel = CreateHeaderValue("Operator", Color.White, "User", false, false, true);
            _totalTankValue = CreateKpiValue("0", Color.DeepSkyBlue);
            _runningTankValue = CreateKpiValue("0", Color.LimeGreen);
            _hoistPositionValue = CreateKpiValue("Tank 01", Color.DeepSkyBlue);
            _currentStepValue = CreateKpiValue("Idle", Color.DeepSkyBlue);
            _activeAlarmValue = CreateKpiValue("0", Color.Tomato);
            _criticalAlarmValue = CreateSeverityValue("0", Color.Tomato);
            _majorAlarmValue = CreateSeverityValue("0", Color.Orange);
            _minorAlarmValue = CreateSeverityValue("0", Color.Gold);
            _selectedEquipmentTitle = CreateEquipmentDetailLabel("No tank selected", Color.DeepSkyBlue, true);
            _selectedLevelTemperatureValue = CreateEquipmentDetailLabel("Level -- | Temp --", Color.White, false);
            _selectedElectricalValue = CreateEquipmentDetailLabel("Electrical --", Color.White, false);
            _selectedMotorValue = CreateEquipmentDetailLabel("Motor UNKNOWN", Color.LightGray, true);
            _selectedQualityValue = CreateEquipmentDetailLabel("Data UNKNOWN", Color.LightGray, false);
            _alarmGrid = BuildAlarmGrid();
            _viewMenu = BuildViewMenu();
            _tankMenu = BuildTankMenu();
            _recipeMenu = BuildRecipeMenu();
            _settingsMenu = BuildSettingsMenu();
            _accountMenu = BuildAccountMenu();

            BuildShell();
        }

        public void BindData(IList<TankModel> tanks, IList<WagonModel> wagons, IList<ProcessStepModel> processSteps, IList<HoistModel> hoists, IList<JobModel> jobs, HoistStatusModel hoistStatus, double hoistPositionIndex, string currentStepName, int remainingSeconds, bool autoMode, bool emergencyStop, int requestedRows, PlantTelemetrySnapshot telemetrySnapshot = null)
        {
            var tankList = tanks == null
                ? new List<TankModel>()
                : tanks.Where(t => t.IsActive).OrderBy(t => t.LineId <= 0 ? 1 : t.LineId).ThenBy(t => t.TankNo).ToList();
            if (tankList.Count == 0 && tanks != null)
            {
                tankList = tanks.OrderBy(t => t.LineId <= 0 ? 1 : t.LineId).ThenBy(t => t.TankNo).ToList();
            }

            var hoistList = hoists == null
                ? new List<HoistModel>()
                : hoists.OrderBy(h => string.Equals(h.HoistName, "H1", StringComparison.OrdinalIgnoreCase) ? 0 : 1).ThenBy(h => h.HoistId).Take(1).ToList();
            var jobList = jobs == null ? new List<JobModel>() : jobs.ToList();
            var telemetryAlarms = new EquipmentTelemetryAlarmService().Evaluate(
                telemetrySnapshot,
                DateTime.UtcNow,
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(3));
            var criticalAlarms = tankList.Count(t => IsFaultStatus(t.Status)) + hoistList.Count(h => IsFaultStatus(h.Status)) + (emergencyStop ? 1 : 0) +
                telemetryAlarms.Count(a => string.Equals(a.Severity, "Critical", StringComparison.OrdinalIgnoreCase));
            var majorAlarms = tankList.Count(t => string.Equals(t.Status, Constants.StatusWarning, StringComparison.OrdinalIgnoreCase)) +
                telemetryAlarms.Count(a => string.Equals(a.Severity, "Major", StringComparison.OrdinalIgnoreCase)) +
                hoistList.Count(h => string.Equals(h.Status, Constants.StatusWarning, StringComparison.OrdinalIgnoreCase));
            var minorAlarms = telemetryAlarms.Count(a => string.Equals(a.Severity, "Minor", StringComparison.OrdinalIgnoreCase));
            var activeAlarms = criticalAlarms + majorAlarms + minorAlarms;
            var primaryHoist = hoistList.FirstOrDefault();
            var hoistStatusText = primaryHoist == null ? (hoistStatus == null ? "Idle" : hoistStatus.Status) : primaryHoist.Status;
            var hoistTankNo = Math.Max(1, Convert.ToInt32(Math.Round(hoistPositionIndex, MidpointRounding.AwayFromZero)) + 1);
            var stepText = string.IsNullOrWhiteSpace(currentStepName) ? "Idle" : currentStepName;
            _boundAutoMode = autoMode;
            _boundEmergencyStop = emergencyStop;
            _boundHasActiveProduction = jobList.Any(IsActiveProductionJob);
            _boundHasRunningTank = tankList.Any(t => string.Equals(t.Status, Constants.StatusRunning, StringComparison.OrdinalIgnoreCase));
            _boundActiveAlarmCount = activeAlarms;

            _plantStateLabel.Text = emergencyStop ? "Plant: Emergency Stop" : activeAlarms > 0 ? "Plant: Alarm" : "Plant: Normal";
            _plantStateLabel.ForeColor = emergencyStop || activeAlarms > 0 ? Color.Tomato : Color.LimeGreen;
            _modeLabel.Text = autoMode ? "Auto Mode" : "Manual Mode";
            _modeLabel.ForeColor = autoMode ? Color.DeepSkyBlue : Color.Gold;
            _hoistLabel.Text = "Hoist H1  " + (string.IsNullOrWhiteSpace(hoistStatusText) ? "Idle" : hoistStatusText);
            _hoistLabel.ForeColor = GetStatusColor(hoistStatusText, emergencyStop);
            _clockLabel.Text = DateTime.Now.ToString("HH:mm:ss") + Environment.NewLine + DateTime.Now.ToString("MMM dd, yyyy");
            UpdateCommunicationHeartbeat(telemetrySnapshot);
            _totalTankValue.Text = tankList.Count.ToString();
            _runningTankValue.Text = tankList.Count(t => string.Equals(t.Status, Constants.StatusRunning, StringComparison.OrdinalIgnoreCase)).ToString();
            _hoistPositionValue.Text = "Tank " + hoistTankNo.ToString("00");
            _currentStepValue.Text = remainingSeconds > 0 ? stepText + "  |  " + remainingSeconds + "s" : stepText;
            _activeAlarmValue.Text = activeAlarms.ToString();
            _activeAlarmValue.ForeColor = activeAlarms > 0 ? Color.Tomato : Color.LimeGreen;
            _criticalAlarmValue.Text = criticalAlarms.ToString();
            _majorAlarmValue.Text = majorAlarms.ToString();
            _minorAlarmValue.Text = minorAlarms.ToString();
            _alarmGrid.DataSource = BuildAlarmTable(tankList, jobList, emergencyStop, telemetryAlarms);

            UpdateButtonState();
            var pending = new PendingPlantData
            {
                Tanks = tankList,
                ProcessSteps = processSteps == null ? new List<ProcessStepModel>() : processSteps.ToList(),
                Hoists = hoistList,
                Jobs = jobList,
                HoistStatus = hoistStatus,
                HoistPositionIndex = hoistPositionIndex,
                CurrentStepName = currentStepName,
                RemainingSeconds = remainingSeconds,
                AutoMode = autoMode,
                EmergencyStop = emergencyStop,
                TelemetrySnapshot = telemetrySnapshot
            };

            _pendingPlantData = pending;
            UpdateSelectedEquipmentPanel();
            if (_plantView != null)
            {
                ApplyPendingPlantData();
            }
            else if (_deferPlantViewInitialization && IsHandleCreated)
            {
                BeginInvoke((MethodInvoker)EnsurePlantViewCreated);
            }
        }

        public void SelectTank(int tankId)
        {
            _pendingSelectedTankId = tankId;
            UpdateSelectedEquipmentPanel();
            if (_plantView != null)
            {
                _plantView.SelectTank(tankId);
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (_deferPlantViewInitialization && _plantView == null)
            {
                BeginInvoke((MethodInvoker)EnsurePlantViewCreated);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _disposed = true;
                if (_plantView != null)
                {
                    _plantView.TankSelected -= PlantView_TankSelected;
                    _plantView.Dispose();
                }

                _elementHost.Child = null;
                _elementHost.Dispose();
                _viewMenu.Dispose();
                _tankMenu.Dispose();
                _recipeMenu.Dispose();
                _settingsMenu.Dispose();
                _accountMenu.Dispose();
            }

            base.Dispose(disposing);
        }

        private void EnsurePlantViewCreated()
        {
            if (_plantView != null || _disposed)
            {
                return;
            }

            _plantView = new Plant3DView();
            _plantView.TankSelected += PlantView_TankSelected;
            if (_pendingSelectedTankId.HasValue)
            {
                _plantView.SelectTank(_pendingSelectedTankId.Value);
            }

            _elementHost.Child = _plantView;
            _elementHost.Visible = true;
            _viewportLoadingLabel.Visible = false;
            ApplyPendingPlantData();
        }

        private void ApplyPendingPlantData()
        {
            if (_plantView == null || _pendingPlantData == null)
            {
                return;
            }

            var data = _pendingPlantData;
            _plantView.UpdatePlant(
                data.Tanks,
                data.ProcessSteps,
                data.Hoists,
                data.Jobs,
                data.HoistStatus,
                data.HoistPositionIndex,
                data.CurrentStepName,
                data.RemainingSeconds,
                data.AutoMode,
                data.EmergencyStop,
                data.TelemetrySnapshot);
        }

        private void BuildShell()
        {
            BackColor = Color.FromArgb(3, 11, 16);

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(3, 11, 16),
                ColumnCount = 3,
                RowCount = 4,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 126));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 244));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 64));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 66));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 180));

            var header = BuildHeader();
            root.Controls.Add(header, 0, 0);
            root.SetColumnSpan(header, 3);

            var nav = BuildNavigation();
            root.Controls.Add(nav, 0, 1);
            root.SetRowSpan(nav, 3);

            root.Controls.Add(BuildViewportPanel(), 1, 1);
            root.Controls.Add(BuildCommandRow(), 1, 2);
            root.Controls.Add(BuildAlarmPanel(), 1, 3);

            var kpiPanel = BuildKpiPanel();
            root.Controls.Add(kpiPanel, 2, 1);
            root.SetRowSpan(kpiPanel, 3);

            Controls.Add(root);
        }

        private Control BuildHeader()
        {
            var header = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(5, 16, 23),
                ColumnCount = 7,
                RowCount = 1
            };
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 334));
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180));
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 168));
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 230));
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 164));
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160));

            var title = new ScadaBrandHeader
            {
                Dock = DockStyle.Fill
            };
            header.Controls.Add(title, 0, 0);
            header.Controls.Add(_plantStateLabel, 1, 0);
            header.Controls.Add(_modeLabel, 2, 0);
            header.Controls.Add(_hoistLabel, 3, 0);
            header.Controls.Add(_communicationLabel, 4, 0);
            header.Controls.Add(_clockLabel, 5, 0);
            _operatorLabel.Cursor = Cursors.Hand;
            _operatorLabel.ContextMenuStrip = _accountMenu;
            _operatorLabel.Click += AccountMenu_Click;
            header.Controls.Add(_operatorLabel, 6, 0);
            return header;
        }

        private Control BuildNavigation()
        {
            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(7, 24, 34),
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(4, 4, 4, 4)
            };

            foreach (var text in new[]
            {
                "Overview", "Process", "View", "Tanks", "Recipe", "Hoist", "Alarms", "Trends", "Reports", "Settings"
            })
            {
                panel.Controls.Add(CreateNavigationButton(text));
            }

            return panel;
        }

        private ScadaNavButton CreateNavigationButton(string text)
        {
            var button = new ScadaNavButton
            {
                Text = text,
                Tag = text,
                Selected = string.Equals(text, "Overview", StringComparison.OrdinalIgnoreCase)
            };

            button.Click += NavigationButton_Click;
            button.ContextMenuStrip = GetNavigationMenu(text);
            if (button.ContextMenuStrip != null)
            {
                button.MouseEnter += NavigationMenuButton_MouseEnter;
            }

            _navigationButtons.Add(button);
            return button;
        }

        private Control BuildViewportPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(8, 27, 36),
                Padding = new Padding(8, 8, 8, 4)
            };

            panel.Controls.Add(_viewportLoadingLabel);
            _elementHost.Dock = DockStyle.Fill;
            panel.Controls.Add(_elementHost);
            _elementHost.BringToFront();
            return panel;
        }

        private Control BuildCommandRow()
        {
            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(8, 27, 36),
                ColumnCount = 7,
                RowCount = 1,
                Padding = new Padding(10, 10, 10, 10),
                Margin = new Padding(8, 4, 8, 4)
            };
            for (var i = 0; i < 7; i++)
            {
                panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            }

            foreach (var text in new[] { "Auto", "Manual", "Start Job", "Stop", "Emergency Stop", "Reset", "IP Connection" })
            {
                panel.Controls.Add(CreateCommandButton(text, string.Equals(text, "Emergency Stop", StringComparison.OrdinalIgnoreCase), 0, 0));
            }

            return panel;
        }

        private Control BuildAlarmPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(8, 27, 36),
                Padding = new Padding(12),
                Margin = new Padding(8, 4, 8, 8)
            };

            var title = new Label
            {
                Dock = DockStyle.Top,
                Height = 24,
                Text = "Active Alarms",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            _alarmGrid.Dock = DockStyle.Fill;
            panel.Controls.Add(_alarmGrid);
            panel.Controls.Add(title);
            return panel;
        }

        private Control BuildKpiPanel()
        {
            var outer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(3, 11, 16),
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(4, 8, 8, 8)
            };
            outer.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            outer.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));

            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(3, 11, 16),
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(0)
            };
            panel.HorizontalScroll.Enabled = false;
            panel.HorizontalScroll.Visible = false;

            panel.Controls.Add(CreateKpiCard("TOTAL TANKS", _totalTankValue, "Tanks", "Tanks", Color.DeepSkyBlue));
            panel.Controls.Add(CreateKpiCard("RUNNING TANKS", _runningTankValue, "Running", "Running", Color.LimeGreen));
            panel.Controls.Add(CreateKpiCard("HOIST POSITION", _hoistPositionValue, "Current", "Hoist", Color.DeepSkyBlue));
            panel.Controls.Add(CreateSelectedEquipmentCard());
            panel.Controls.Add(CreateAlarmKpiCard());
            panel.Layout += KpiPanel_Layout;

            outer.Controls.Add(panel, 0, 0);
            outer.Controls.Add(CreateRightSidebarActionButton(), 0, 1);
            return outer;
        }

        private static void KpiPanel_Layout(object sender, LayoutEventArgs e)
        {
            var panel = sender as FlowLayoutPanel;
            if (panel == null)
            {
                return;
            }

            var width = Math.Max(180, panel.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - panel.Padding.Horizontal - 2);
            foreach (Control child in panel.Controls)
            {
                child.Width = width;
            }

            panel.HorizontalScroll.Enabled = false;
            panel.HorizontalScroll.Visible = false;
        }

        private static Control WrapPanel(Control child, Padding margin)
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(8, 27, 36),
                Padding = margin
            };
            panel.Controls.Add(child);
            return panel;
        }

        private Button CreateCommandButton(string text, bool danger, int width, int height)
        {
            Button button;
            if (width <= 0 && height <= 0)
            {
                button = new ScadaCommandButton
                {
                    Text = text,
                    Tag = text,
                    Danger = danger,
                    IconKey = text
                };
            }
            else
            {
                button = new Button
                {
                    Text = text,
                    Tag = text,
                    FlatStyle = FlatStyle.Flat,
                    BackColor = danger ? Color.FromArgb(112, 24, 24) : Color.FromArgb(13, 43, 58),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold)
                };
                button.FlatAppearance.BorderColor = danger ? Color.Tomato : Color.DeepSkyBlue;
            }

            button.Dock = width <= 0 ? DockStyle.Fill : DockStyle.None;
            button.Width = width <= 0 ? 120 : width;
            button.Height = height <= 0 ? 42 : height;
            button.Margin = new Padding(4);
            button.Click += CommandButton_Click;
            _operatorButtons.Add(button);
            return button;
        }

        private Button CreateRightSidebarActionButton()
        {
            var button = new ScadaCommandButton
            {
                Dock = DockStyle.Fill,
                Text = "View All Alarms",
                Tag = "Alarms",
                IconKey = "Alarms",
                Margin = new Padding(0, 6, 0, 0)
            };
            button.Click += CommandButton_Click;
            _operatorButtons.Add(button);
            return button;
        }

        private ContextMenuStrip BuildViewMenu()
        {
            var menu = CreateScadaContextMenu();
            menu.Items.Add(CreateMenuItem("Fit Plant", "Fit Plant"));
            menu.Items.Add(CreateMenuItem("Front View", "Front View"));
            menu.Items.Add(CreateMenuItem("Top View", "Top View"));
            menu.Items.Add(CreateMenuItem("Left View", "Left View"));
            menu.Items.Add(CreateMenuItem("Right View", "Right View"));
            return menu;
        }

        private ContextMenuStrip BuildTankMenu()
        {
            var menu = CreateScadaContextMenu();
            menu.Items.Add(CreateMenuItem("Add Tank", "Add Tank"));
            menu.Items.Add(CreateMenuItem("Edit Tank", "Edit Tank"));
            menu.Items.Add(CreateMenuItem("Remove Tank", "Remove Tank"));
            return menu;
        }

        private ContextMenuStrip BuildRecipeMenu()
        {
            var menu = CreateScadaContextMenu();
            menu.Items.Add(CreateMenuItem("Create Recipe", "Recipe"));
            menu.Items.Add(CreateMenuItem("Edit Recipe", "Recipe"));
            return menu;
        }

        private ContextMenuStrip BuildSettingsMenu()
        {
            var menu = CreateScadaContextMenu();
            menu.Items.Add(CreateMenuItem("User Management", "Users"));
            return menu;
        }

        private ContextMenuStrip BuildAccountMenu()
        {
            var menu = CreateScadaContextMenu();
            menu.Items.Add(CreateMenuItem("Login Another", "Login"));
            menu.Items.Add(CreateMenuItem("Logout", "Logout"));
            return menu;
        }

        private ContextMenuStrip CreateScadaContextMenu()
        {
            return new ContextMenuStrip
            {
                BackColor = Color.FromArgb(7, 24, 34),
                ForeColor = Color.White,
                ShowImageMargin = false,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Renderer = new ToolStripProfessionalRenderer(new ScadaMenuColorTable())
            };
        }

        private ContextMenuStrip GetNavigationMenu(string text)
        {
            if (string.Equals(text, "View", StringComparison.OrdinalIgnoreCase))
            {
                return _viewMenu;
            }

            if (string.Equals(text, "Tanks", StringComparison.OrdinalIgnoreCase))
            {
                return _tankMenu;
            }

            if (string.Equals(text, "Recipe", StringComparison.OrdinalIgnoreCase))
            {
                return _recipeMenu;
            }

            if (string.Equals(text, "Settings", StringComparison.OrdinalIgnoreCase))
            {
                return _settingsMenu;
            }

            return null;
        }

        private ToolStripMenuItem CreateMenuItem(string text, string command)
        {
            var item = new ToolStripMenuItem(text)
            {
                Tag = command,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(7, 24, 34)
            };
            item.Click += MenuItem_Click;
            return item;
        }

        private static Label CreateHeaderValue(string text, Color color, string iconKey, bool showStatusDot, bool badgeStyle)
        {
            return CreateHeaderValue(text, color, iconKey, showStatusDot, badgeStyle, false);
        }

        private static Label CreateHeaderValue(string text, Color color, string iconKey, bool showStatusDot, bool badgeStyle, bool showDropDownArrow)
        {
            return new ScadaHeaderItem
            {
                Text = text,
                ForeColor = color,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                IconKey = iconKey,
                ShowStatusDot = showStatusDot,
                BadgeStyle = badgeStyle,
                ShowDropDownArrow = showDropDownArrow
            };
        }

        private static Label CreateHeaderBadge(string text)
        {
            var label = CreateHeaderValue(text, Color.White, null, false, true);
            label.BackColor = Color.FromArgb(7, 34, 50);
            label.BorderStyle = BorderStyle.FixedSingle;
            return label;
        }

        private static Label CreateKpiValue(string text, Color color)
        {
            return new Label
            {
                Dock = DockStyle.Top,
                Height = 38,
                Text = text,
                ForeColor = color,
                Font = new Font("Segoe UI", 22F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        private static Label CreateEquipmentDetailLabel(string text, Color color, bool bold)
        {
            return new Label
            {
                AutoSize = false,
                Text = text,
                ForeColor = color,
                Font = new Font("Segoe UI", 9F, bold ? FontStyle.Bold : FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        private Control CreateSelectedEquipmentCard()
        {
            var panel = new Panel
            {
                Name = "SelectedEquipmentCard",
                Width = 226,
                Height = 180,
                BackColor = Color.FromArgb(8, 27, 36),
                Margin = new Padding(0, 0, 0, 10),
                Padding = new Padding(16, 10, 16, 8)
            };

            _selectedEquipmentTitle.Location = new Point(16, 38);
            _selectedEquipmentTitle.Size = new Size(194, 24);
            _currentStepValue.Dock = DockStyle.None;
            _currentStepValue.Location = new Point(16, 62);
            _currentStepValue.Size = new Size(194, 24);
            _currentStepValue.AutoSize = false;
            _currentStepValue.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            _currentStepValue.TextAlign = ContentAlignment.MiddleLeft;
            _selectedLevelTemperatureValue.Location = new Point(16, 86);
            _selectedLevelTemperatureValue.Size = new Size(194, 21);
            _selectedElectricalValue.Location = new Point(16, 107);
            _selectedElectricalValue.Size = new Size(194, 21);
            _selectedMotorValue.Location = new Point(16, 128);
            _selectedMotorValue.Size = new Size(194, 21);
            _selectedQualityValue.Location = new Point(16, 149);
            _selectedQualityValue.Size = new Size(194, 21);

            panel.Controls.Add(_selectedQualityValue);
            panel.Controls.Add(_selectedMotorValue);
            panel.Controls.Add(_selectedElectricalValue);
            panel.Controls.Add(_selectedLevelTemperatureValue);
            panel.Controls.Add(_currentStepValue);
            panel.Controls.Add(_selectedEquipmentTitle);
            panel.Controls.Add(new Panel
            {
                BackColor = Color.FromArgb(18, 88, 114),
                Location = new Point(16, 35),
                Size = new Size(194, 1)
            });
            panel.Controls.Add(new Label
            {
                Location = new Point(16, 10),
                Size = new Size(194, 24),
                Text = "SELECTED EQUIPMENT",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            });

            return panel;
        }

        private static Label CreateSeverityValue(string text, Color color)
        {
            return new Label
            {
                AutoSize = false,
                Size = new Size(44, 18),
                Text = text,
                ForeColor = color,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };
        }

        private static Control CreateKpiCard(string title, Label value, string caption, string iconKey, Color accentColor)
        {
            var panel = new Panel
            {
                Width = 226,
                Height = 124,
                BackColor = Color.FromArgb(8, 27, 36),
                Margin = new Padding(0, 0, 0, 10),
                Padding = new Padding(16, 10, 16, 8)
            };

            var icon = new ScadaKpiIcon
            {
                IconKey = iconKey,
                AccentColor = accentColor,
                Location = new Point(14, title == "CURRENT STEP" ? 48 : 42),
                Size = title == "CURRENT STEP" ? new Size(38, 38) : new Size(46, 46)
            };

            value.Dock = DockStyle.None;
            value.Location = new Point(76, 35);
            value.Size = new Size(140, 48);
            value.AutoSize = false;
            value.TextAlign = ContentAlignment.MiddleLeft;
            if (title == "CURRENT STEP")
            {
                value.Location = new Point(72, 32);
                value.Size = new Size(144, 58);
                value.Font = new Font("Segoe UI", 14.5F, FontStyle.Bold);
            }
            else if (title == "HOIST POSITION")
            {
                value.Font = new Font("Segoe UI", 19F, FontStyle.Bold);
            }

            panel.Controls.Add(new Label
            {
                Location = new Point(76, title == "CURRENT STEP" ? 95 : 82),
                Size = new Size(140, 24),
                Text = caption,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8.5F),
                TextAlign = ContentAlignment.MiddleLeft
            });
            panel.Controls.Add(value);
            panel.Controls.Add(icon);
            panel.Controls.Add(new Panel
            {
                BackColor = Color.FromArgb(18, 88, 114),
                Location = new Point(16, 35),
                Size = new Size(194, 1)
            });
            panel.Controls.Add(new Label
            {
                Location = new Point(16, 10),
                Size = new Size(194, 24),
                Text = title,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            });

            return panel;
        }

        private Control CreateAlarmKpiCard()
        {
            var panel = new Panel
            {
                Width = 226,
                Height = 136,
                BackColor = Color.FromArgb(8, 27, 36),
                Margin = new Padding(0, 0, 0, 10),
                Padding = new Padding(16, 10, 16, 8)
            };

            var icon = new ScadaKpiIcon
            {
                IconKey = "Alarms",
                AccentColor = Color.Tomato,
                Location = new Point(14, 42),
                Size = new Size(46, 46)
            };

            _activeAlarmValue.Dock = DockStyle.None;
            _activeAlarmValue.Location = new Point(76, 35);
            _activeAlarmValue.Size = new Size(140, 48);
            _activeAlarmValue.AutoSize = false;
            _activeAlarmValue.TextAlign = ContentAlignment.MiddleLeft;

            panel.Controls.Add(new Label
            {
                Location = new Point(76, 82),
                Size = new Size(140, 22),
                Text = "Alarms",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8.5F),
                TextAlign = ContentAlignment.MiddleLeft
            });

            AddSeverityColumn(panel, _criticalAlarmValue, "Critical", 20, Color.Tomato);
            AddSeverityColumn(panel, _majorAlarmValue, "Major", 86, Color.Orange);
            AddSeverityColumn(panel, _minorAlarmValue, "Minor", 152, Color.Gold);

            panel.Controls.Add(_activeAlarmValue);
            panel.Controls.Add(icon);
            panel.Controls.Add(new Panel
            {
                BackColor = Color.FromArgb(18, 88, 114),
                Location = new Point(16, 35),
                Size = new Size(194, 1)
            });
            panel.Controls.Add(new Label
            {
                Location = new Point(16, 10),
                Size = new Size(194, 24),
                Text = "ACTIVE ALARMS",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            });

            return panel;
        }

        private static void AddSeverityColumn(Control panel, Label value, string text, int x, Color color)
        {
            value.Location = new Point(x, 104);
            value.ForeColor = color;
            panel.Controls.Add(value);
            panel.Controls.Add(new Label
            {
                Location = new Point(x - 4, 119),
                Size = new Size(52, 14),
                Text = text,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 7F),
                TextAlign = ContentAlignment.MiddleCenter
            });
        }

        private static DataGridView BuildAlarmGrid()
        {
            var grid = new DataGridView
            {
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.FromArgb(8, 27, 36),
                BorderStyle = BorderStyle.None,
                EnableHeadersVisualStyles = false
            };
            grid.RowTemplate.Height = 27;
            grid.ColumnHeadersHeight = 30;
            grid.GridColor = Color.FromArgb(20, 66, 82);
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(8, 27, 36);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            grid.DefaultCellStyle.BackColor = Color.FromArgb(8, 27, 36);
            grid.DefaultCellStyle.ForeColor = Color.White;
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 8.5F);
            grid.DefaultCellStyle.Padding = new Padding(4, 2, 4, 2);
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(5, 20, 28);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 83, 132);
            grid.DefaultCellStyle.SelectionForeColor = Color.White;
            grid.CellFormatting += AlarmGrid_CellFormatting;

            foreach (var column in new[]
            {
                new { Name = "Time", Header = "Time", Fill = 70 },
                new { Name = "Date", Header = "Date", Fill = 85 },
                new { Name = "Area", Header = "Area", Fill = 110 },
                new { Name = "Equipment", Header = "Equipment", Fill = 120 },
                new { Name = "AlarmDescription", Header = "Alarm Description", Fill = 210 },
                new { Name = "Severity", Header = "Severity", Fill = 80 },
                new { Name = "Status", Header = "Status", Fill = 80 }
            })
            {
                grid.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DataPropertyName = column.Name,
                    HeaderText = column.Header,
                    FillWeight = column.Fill
                });
            }

            return grid;
        }

        private static void AlarmGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var grid = sender as DataGridView;
            if (grid == null || e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            var columnName = grid.Columns[e.ColumnIndex].DataPropertyName;
            var valueText = Convert.ToString(e.Value);
            if (string.Equals(columnName, "Severity", StringComparison.OrdinalIgnoreCase))
            {
                e.CellStyle.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
                if (string.Equals(valueText, "Critical", StringComparison.OrdinalIgnoreCase))
                {
                    e.CellStyle.ForeColor = Color.Tomato;
                }
                else if (string.Equals(valueText, "Major", StringComparison.OrdinalIgnoreCase))
                {
                    e.CellStyle.ForeColor = Color.Orange;
                }
                else if (string.Equals(valueText, "Minor", StringComparison.OrdinalIgnoreCase))
                {
                    e.CellStyle.ForeColor = Color.Gold;
                }
            }
            else if (string.Equals(columnName, "Status", StringComparison.OrdinalIgnoreCase))
            {
                e.CellStyle.ForeColor = string.Equals(valueText, "Active", StringComparison.OrdinalIgnoreCase)
                    ? Color.DeepSkyBlue
                    : Color.LimeGreen;
            }
        }

        private static DataTable BuildAlarmTable(IList<TankModel> tanks, IList<JobModel> jobs, bool emergencyStop, IList<AlarmModel> telemetryAlarms)
        {
            var table = new DataTable();
            table.Columns.Add("Time");
            table.Columns.Add("Date");
            table.Columns.Add("Area");
            table.Columns.Add("Equipment");
            table.Columns.Add("AlarmDescription");
            table.Columns.Add("Severity");
            table.Columns.Add("Status");

            if (emergencyStop)
            {
                AddAlarmRow(table, "Plant", "Safety", "Emergency stop active", "Critical", "Active");
            }

            foreach (var alarm in (telemetryAlarms ?? new AlarmModel[0]).Take(5))
            {
                AddAlarmRow(table, "Equipment", alarm.Source, alarm.Message, alarm.Severity, alarm.State);
            }

            foreach (var tank in tanks.Where(t => IsAlarmStatus(t.Status)).Take(Math.Max(0, 5 - table.Rows.Count)))
            {
                AddAlarmRow(
                    table,
                    "Tank " + tank.TankNo.ToString("00"),
                    "Level / Temp",
                    string.Equals(tank.Status, Constants.StatusFault, StringComparison.OrdinalIgnoreCase) ? "Tank fault active" : "Tank warning active",
                    string.Equals(tank.Status, Constants.StatusFault, StringComparison.OrdinalIgnoreCase) ? "Critical" : "Major",
                    "Active");
            }

            if (table.Rows.Count == 0 && jobs.Count > 0)
            {
                AddAlarmRow(table, "Hoist H1", "Hoist Drive", "Production job running", "Minor", "Acknowledged");
            }

            return table;
        }

        private static void AddAlarmRow(DataTable table, string area, string equipment, string description, string severity, string status)
        {
            table.Rows.Add(DateTime.Now.ToString("HH:mm:ss"), DateTime.Now.ToString("MMM dd, yyyy"), area, equipment, description, severity, status);
        }

        private void UpdateButtonState()
        {
            foreach (var button in _operatorButtons)
            {
                var text = Convert.ToString(button.Tag);
                var engineer = text == "Add Tank" || text == "Edit Tank" || text == "Remove Tank" || text == "Recipe" || text == "Users";
                var operate = text == "Start Plant" || text == "Stop Plant" || text == "Stop" || text == "Start Job" || text == "Pause Job" ||
                    text == "Auto" || text == "Manual" || text == "Reset" || text == "Emergency Stop" || text == "Reset Alarms";

                var enabled = engineer ? CanEngineerTanks : operate ? CanOperateTanks : true;
                if (enabled)
                {
                    if (string.Equals(text, "Start Job", StringComparison.OrdinalIgnoreCase))
                    {
                        enabled = !_boundEmergencyStop && !_boundHasActiveProduction;
                    }
                    else if (string.Equals(text, "Stop", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(text, "Stop Plant", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(text, "Pause Job", StringComparison.OrdinalIgnoreCase))
                    {
                        enabled = !_boundEmergencyStop && (_boundHasActiveProduction || _boundHasRunningTank);
                    }
                    else if (string.Equals(text, "Emergency Stop", StringComparison.OrdinalIgnoreCase))
                    {
                        enabled = !_boundEmergencyStop;
                    }
                    else if (string.Equals(text, "Reset", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(text, "Reset Alarms", StringComparison.OrdinalIgnoreCase))
                    {
                        enabled = _boundEmergencyStop || _boundHasActiveProduction || _boundHasRunningTank || _boundActiveAlarmCount > 0;
                    }
                }

                button.Enabled = enabled;
                ApplyCommandActiveState(button, text);
            }
        }

        private void ApplyCommandActiveState(Button button, string command)
        {
            var scadaButton = button as ScadaCommandButton;
            if (scadaButton == null)
            {
                return;
            }

            scadaButton.Active =
                string.Equals(command, "Auto", StringComparison.OrdinalIgnoreCase) && _boundAutoMode && !_boundEmergencyStop ||
                string.Equals(command, "Manual", StringComparison.OrdinalIgnoreCase) && !_boundAutoMode && !_boundEmergencyStop ||
                string.Equals(command, "Start Job", StringComparison.OrdinalIgnoreCase) && _boundHasActiveProduction && !_boundEmergencyStop ||
                (string.Equals(command, "Stop", StringComparison.OrdinalIgnoreCase) || string.Equals(command, "Stop Plant", StringComparison.OrdinalIgnoreCase)) && !_boundHasActiveProduction && !_boundHasRunningTank && !_boundEmergencyStop ||
                string.Equals(command, "Emergency Stop", StringComparison.OrdinalIgnoreCase) && _boundEmergencyStop;

            scadaButton.Invalidate();
        }

        private static bool IsActiveProductionJob(JobModel job)
        {
            if (job == null)
            {
                return false;
            }

            return !string.Equals(job.Status, "Complete", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(job.Status, "Paused", StringComparison.OrdinalIgnoreCase);
        }

        private void CommandButton_Click(object sender, EventArgs e)
        {
            var button = sender as Button;
            var command = button == null ? string.Empty : Convert.ToString(button.Tag);
            if (HandleLocalDashboardCommand(command))
            {
                return;
            }

            if (DashboardCommandRequested != null && !string.IsNullOrWhiteSpace(command))
            {
                DashboardCommandRequested(this, command);
            }
        }

        private bool HandleLocalDashboardCommand(string command)
        {
            if (IsViewCommand(command))
            {
                EnsurePlantViewCreated();
                if (_plantView == null)
                {
                    return true;
                }
            }

            if (string.Equals(command, "Fit Plant", StringComparison.OrdinalIgnoreCase))
            {
                _plantView.FitPlant();
                return true;
            }

            if (string.Equals(command, "Front View", StringComparison.OrdinalIgnoreCase))
            {
                _plantView.SetFrontView();
                return true;
            }

            if (string.Equals(command, "Top View", StringComparison.OrdinalIgnoreCase))
            {
                _plantView.SetTopView();
                return true;
            }

            if (string.Equals(command, "Left View", StringComparison.OrdinalIgnoreCase))
            {
                _plantView.SetLeftView();
                return true;
            }

            if (string.Equals(command, "Right View", StringComparison.OrdinalIgnoreCase))
            {
                _plantView.SetRightView();
                return true;
            }

            return false;
        }

        private static bool IsViewCommand(string command)
        {
            return string.Equals(command, "Fit Plant", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(command, "Front View", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(command, "Top View", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(command, "Left View", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(command, "Right View", StringComparison.OrdinalIgnoreCase);
        }

        private void NavigationMenuButton_MouseEnter(object sender, EventArgs e)
        {
            var control = sender as Control;
            if (control == null || control.ContextMenuStrip == null)
            {
                return;
            }

            if (!control.ContextMenuStrip.Visible)
            {
                control.ContextMenuStrip.Show(control, new Point(control.Width - 2, 0));
            }
        }

        private void AccountMenu_Click(object sender, EventArgs e)
        {
            var control = sender as Control;
            if (control == null)
            {
                return;
            }

            _accountMenu.Show(control, new Point(8, control.Height - 2));
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripItem;
            var command = item == null ? string.Empty : Convert.ToString(item.Tag);
            if (HandleLocalDashboardCommand(command))
            {
                return;
            }

            if (DashboardCommandRequested != null && !string.IsNullOrWhiteSpace(command))
            {
                DashboardCommandRequested(this, command);
            }
        }

        private void NavigationButton_Click(object sender, EventArgs e)
        {
            var button = sender as ScadaNavButton;
            if (button == null)
            {
                return;
            }

            if (button.ContextMenuStrip != null)
            {
                button.ContextMenuStrip.Show(button, new Point(button.Width - 2, 0));
                return;
            }

            foreach (var navButton in _navigationButtons)
            {
                navButton.Selected = ReferenceEquals(navButton, button);
            }

            var command = Convert.ToString(button.Tag);
            if (DashboardCommandRequested != null && !string.IsNullOrWhiteSpace(command))
            {
                DashboardCommandRequested(this, command);
            }
        }

        private void PlantView_TankSelected(object sender, int tankId)
        {
            _pendingSelectedTankId = tankId;
            UpdateSelectedEquipmentPanel();
            if (TankSelected != null)
            {
                TankSelected(this, new TankControlEventArgs(tankId));
            }
        }

        private void UpdateSelectedEquipmentPanel()
        {
            var data = _pendingPlantData;
            var tank = data == null || !_pendingSelectedTankId.HasValue
                ? null
                : data.Tanks.FirstOrDefault(t => t.Id == _pendingSelectedTankId.Value);
            if (tank == null)
            {
                _selectedEquipmentTitle.Text = "No tank selected";
                _selectedLevelTemperatureValue.Text = "Level -- | Temp --";
                _selectedElectricalValue.Text = "Electrical --";
                _selectedMotorValue.Text = "Motor UNKNOWN";
                _selectedMotorValue.ForeColor = Color.LightGray;
                _selectedQualityValue.Text = "Data UNKNOWN";
                _selectedQualityValue.ForeColor = Color.LightGray;
                return;
            }

            var chemical = string.IsNullOrWhiteSpace(tank.ChemicalName) ? tank.Name : tank.ChemicalName;
            var level = tank.CapacityLiters <= 0 ? 0 : Math.Max(0, Math.Min(100, tank.CurrentLevelLiters * 100.0 / tank.CapacityLiters));
            _selectedEquipmentTitle.Text = "T" + tank.TankNo.ToString("00") + "  " + chemical;
            _selectedLevelTemperatureValue.Text = "Level " + level.ToString("0") + "%  |  Temp " + tank.TemperatureCelsius.ToString("0.0") + " C";
            _selectedElectricalValue.Text = tank.Voltage.ToString("0.0") + " V  |  " + tank.CurrentAmps.ToString("0.0") + " A";

            var telemetry = data.TelemetrySnapshot;
            var motor = telemetry == null
                ? null
                : telemetry.Motors.FirstOrDefault(m =>
                    (m.TankId.HasValue && m.TankId.Value == tank.Id) ||
                    (m.TankNo.HasValue && m.TankNo.Value == tank.TankNo));
            var state = EquipmentTelemetryStateResolver.Resolve(motor, DateTime.UtcNow, TimeSpan.FromSeconds(5));
            _selectedMotorValue.Text = (motor == null ? "Motor" : motor.EquipmentId) + "  " + state.ToString().ToUpperInvariant();
            _selectedMotorValue.ForeColor = GetEquipmentStateColor(state);

            var quality = motor == null ? TelemetryQuality.Unknown : motor.Quality;
            var ageSeconds = motor == null || motor.LastUpdatedUtc == DateTime.MinValue
                ? 0
                : Math.Max(0, (DateTime.UtcNow - motor.LastUpdatedUtc).TotalSeconds);
            _selectedQualityValue.Text = "Data " + quality.ToString().ToUpperInvariant() + (motor == null ? string.Empty : "  |  " + ageSeconds.ToString("0") + "s");
            _selectedQualityValue.ForeColor = quality == TelemetryQuality.Good ? Color.LimeGreen : quality == TelemetryQuality.Bad ? Color.Tomato : Color.Gold;
        }

        private static Color GetEquipmentStateColor(EquipmentOperatingState state)
        {
            if (state == EquipmentOperatingState.Running) return Color.LimeGreen;
            if (state == EquipmentOperatingState.Fault || state == EquipmentOperatingState.BadData) return Color.Tomato;
            if (state == EquipmentOperatingState.Warning || state == EquipmentOperatingState.Local || state == EquipmentOperatingState.Stale) return Color.Gold;
            return Color.LightGray;
        }

        private void UpdateCommunicationHeartbeat(PlantTelemetrySnapshot snapshot)
        {
            var health = PlantTelemetryHealthResolver.Resolve(snapshot, DateTime.UtcNow, TimeSpan.FromSeconds(5));
            var ageSeconds = Math.Max(0, health.Age.TotalSeconds);
            _communicationLabel.Text = "Data " + health.Quality.ToString().ToUpperInvariant() +
                (health.Sequence <= 0 ? string.Empty : "  " + ageSeconds.ToString("0") + "s  #" + health.Sequence);
            _communicationLabel.ForeColor = health.Quality == TelemetryQuality.Good
                ? Color.LimeGreen
                : health.Quality == TelemetryQuality.Bad
                    ? Color.Tomato
                    : Color.Gold;
        }

        protected virtual void OnStartClicked(int tankId)
        {
            var handler = StartClicked;
            if (handler != null)
            {
                handler(this, new TankControlEventArgs(tankId));
            }
        }

        protected virtual void OnStopClicked(int tankId)
        {
            var handler = StopClicked;
            if (handler != null)
            {
                handler(this, new TankControlEventArgs(tankId));
            }
        }

        protected virtual void OnFaultClicked(int tankId)
        {
            var handler = FaultClicked;
            if (handler != null)
            {
                handler(this, new TankControlEventArgs(tankId));
            }
        }

        protected virtual void OnResetClicked(int tankId)
        {
            var handler = ResetClicked;
            if (handler != null)
            {
                handler(this, new TankControlEventArgs(tankId));
            }
        }

        protected virtual void OnRemoveClicked(int tankId)
        {
            var handler = RemoveClicked;
            if (handler != null)
            {
                handler(this, new TankControlEventArgs(tankId));
            }
        }

        private static bool IsAlarmStatus(string status)
        {
            return string.Equals(status, Constants.StatusFault, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(status, Constants.StatusWarning, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(status, "Fault", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(status, "EmergencyStop", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsFaultStatus(string status)
        {
            return string.Equals(status, Constants.StatusFault, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(status, "Fault", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(status, "EmergencyStop", StringComparison.OrdinalIgnoreCase);
        }

        private static Color GetStatusColor(string status, bool emergencyStop)
        {
            if (emergencyStop || string.Equals(status, "EmergencyStop", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(status, Constants.StatusFault, StringComparison.OrdinalIgnoreCase))
            {
                return Color.Tomato;
            }

            if (string.Equals(status, Constants.StatusWarning, StringComparison.OrdinalIgnoreCase))
            {
                return Color.Gold;
            }

            if (string.Equals(status, Constants.StatusRunning, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(status, "Moving", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(status, "Processing", StringComparison.OrdinalIgnoreCase))
            {
                return Color.LimeGreen;
            }

            return Color.DeepSkyBlue;
        }

        private sealed class PendingPlantData
        {
            public IList<TankModel> Tanks { get; set; }
            public IList<ProcessStepModel> ProcessSteps { get; set; }
            public IList<HoistModel> Hoists { get; set; }
            public IList<JobModel> Jobs { get; set; }
            public HoistStatusModel HoistStatus { get; set; }
            public double HoistPositionIndex { get; set; }
            public string CurrentStepName { get; set; }
            public int RemainingSeconds { get; set; }
            public bool AutoMode { get; set; }
            public bool EmergencyStop { get; set; }
            public PlantTelemetrySnapshot TelemetrySnapshot { get; set; }
        }

        private sealed class ScadaMenuColorTable : ProfessionalColorTable
        {
            public override Color MenuItemSelected
            {
                get { return Color.FromArgb(0, 88, 132); }
            }

            public override Color MenuItemBorder
            {
                get { return Color.DeepSkyBlue; }
            }

            public override Color ToolStripDropDownBackground
            {
                get { return Color.FromArgb(7, 24, 34); }
            }

            public override Color ImageMarginGradientBegin
            {
                get { return Color.FromArgb(7, 24, 34); }
            }

            public override Color ImageMarginGradientMiddle
            {
                get { return Color.FromArgb(7, 24, 34); }
            }

            public override Color ImageMarginGradientEnd
            {
                get { return Color.FromArgb(7, 24, 34); }
            }
        }
    }
}
