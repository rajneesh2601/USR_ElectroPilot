using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using USR_ElectroPilot.Helpers;
using USR_ElectroPilot.Models;
using USR_ElectroPilot.Services;

namespace USR_ElectroPilot.Forms
{
    public partial class TrendForm : Form
    {
        private readonly TankService _tankService = new TankService();
        private readonly TankHistoryService _tankHistoryService = new TankHistoryService();
        private readonly RectifierService _rectifierService = new RectifierService();
        private List<TankModel> _tanks = new List<TankModel>();

        public TrendForm()
        {
            InitializeComponent();
        }

        private void TrendForm_Load(object sender, EventArgs e)
        {
            UiHelper.ApplyDarkTheme(this);
            ConfigureChartTheme();
            cboMetric.Items.AddRange(new object[]
            {
                "Tank Level %",
                "Tank Temperature",
                "Tank Voltage",
                "Tank Current",
                "Rectifier Voltage",
                "Rectifier Current"
            });
            dtFrom.Value = DateTime.Now.AddHours(-1);
            dtTo.Value = DateTime.Now;
            LoadTankChoices();
            cboMetric.SelectedIndex = 0;
            RefreshChart();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            RefreshChart();
        }

        private void CboMetric_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsHandleCreated)
            {
                cboTank.Enabled = cboMetric.Text.StartsWith("Tank", StringComparison.OrdinalIgnoreCase);
                RefreshChart();
            }
        }

        private void CboTank_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsHandleCreated)
            {
                RefreshChart();
            }
        }

        private void RefreshChart()
        {
            chartTrends.Series.Clear();
            var series = new Series(cboMetric.Text)
            {
                ChartType = SeriesChartType.Spline,
                BorderWidth = 3,
                Color = UiHelper.AccentColor,
                XValueType = ChartValueType.DateTime
            };

            if (cboMetric.Text.StartsWith("Tank", StringComparison.OrdinalIgnoreCase))
            {
                AddTankHistoryPoints(series);
            }
            else
            {
                series.XValueType = ChartValueType.String;
                AddRectifierPoints(series);
            }

            chartTrends.Series.Add(series);
            lblSummary.Text = series.Points.Count == 0
                ? "No data available."
                : string.Format("{0} point(s), average {1:0.##}", series.Points.Count, series.Points.Average(p => p.YValues[0]));
            chartTrends.ChartAreas[0].RecalculateAxesScale();
        }

        private void LoadTankChoices()
        {
            _tanks = _tankService.GetTanks().OrderBy(t => t.LineId).ThenBy(t => t.TankNo).ToList();
            cboTank.DisplayMember = "Name";
            cboTank.ValueMember = "Id";
            cboTank.DataSource = _tanks;
        }

        private void AddTankHistoryPoints(Series series)
        {
            var tank = cboTank.SelectedItem as TankModel;
            if (tank == null)
            {
                return;
            }

            foreach (var point in _tankHistoryService.GetHistory(tank.Id, dtFrom.Value, dtTo.Value))
            {
                series.Points.AddXY(point.RecordedAt, GetTankHistoryValue(point, tank));
            }
        }

        private double GetTankHistoryValue(TankHistoryModel point, TankModel tank)
        {
            switch (cboMetric.Text)
            {
                case "Tank Temperature":
                    return point.TemperatureCelsius;
                case "Tank Voltage":
                    return point.Voltage;
                case "Tank Current":
                    return point.CurrentAmps;
                default:
                    return tank.CapacityLiters <= 0 ? 0 : (point.LevelLiters / tank.CapacityLiters) * 100;
            }
        }

        private void AddRectifierPoints(Series series)
        {
            foreach (var rectifier in _rectifierService.GetRectifiers().OrderBy(r => r.Name))
            {
                var label = string.IsNullOrWhiteSpace(rectifier.Name) ? "Rectifier " + rectifier.Id : rectifier.Name;
                var value = cboMetric.Text == "Rectifier Current" ? rectifier.CurrentAmps : rectifier.Voltage;
                series.Points.AddXY(label, value);
            }
        }

        private void ConfigureChartTheme()
        {
            chartTrends.BackColor = UiHelper.BackColor;
            chartTrends.BorderlineColor = UiHelper.PanelColor;
            chartTrends.Legends[0].BackColor = UiHelper.BackColor;
            chartTrends.Legends[0].ForeColor = UiHelper.ForeColor;

            var area = chartTrends.ChartAreas[0];
            area.BackColor = UiHelper.PanelColor;
            area.AxisX.LabelStyle.ForeColor = UiHelper.ForeColor;
            area.AxisY.LabelStyle.ForeColor = UiHelper.ForeColor;
            area.AxisX.MajorGrid.LineColor = Color.FromArgb(60, 70, 85);
            area.AxisY.MajorGrid.LineColor = Color.FromArgb(60, 70, 85);
            area.AxisX.LineColor = UiHelper.ForeColor;
            area.AxisY.LineColor = UiHelper.ForeColor;
            area.AxisX.LabelStyle.Format = "HH:mm:ss";
            area.AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
            area.AxisX.TitleForeColor = UiHelper.ForeColor;
            area.AxisY.TitleForeColor = UiHelper.ForeColor;
        }
    }
}
