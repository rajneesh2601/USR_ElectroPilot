using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using USR_ElectroPilot.Helpers;
using USR_ElectroPilot.Services;

namespace USR_ElectroPilot.Forms
{
    public partial class TrendForm : Form
    {
        private readonly TankService _tankService = new TankService();
        private readonly RectifierService _rectifierService = new RectifierService();

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
                XValueType = ChartValueType.String
            };

            if (cboMetric.Text.StartsWith("Tank", StringComparison.OrdinalIgnoreCase))
            {
                AddTankPoints(series);
            }
            else
            {
                AddRectifierPoints(series);
            }

            chartTrends.Series.Add(series);
            lblSummary.Text = series.Points.Count == 0
                ? "No data available."
                : string.Format("{0} point(s), average {1:0.##}", series.Points.Count, series.Points.Average(p => p.YValues[0]));
            chartTrends.ChartAreas[0].RecalculateAxesScale();
        }

        private void AddTankPoints(Series series)
        {
            foreach (var tank in _tankService.GetTanks().OrderBy(t => t.TankNumber))
            {
                var label = string.IsNullOrWhiteSpace(tank.Name) ? "Tank " + tank.TankNumber : tank.Name;
                double value;

                switch (cboMetric.Text)
                {
                    case "Tank Temperature":
                        value = tank.TemperatureCelsius;
                        break;
                    case "Tank Voltage":
                        value = tank.Voltage;
                        break;
                    case "Tank Current":
                        value = tank.CurrentAmps;
                        break;
                    default:
                        value = tank.CapacityLiters <= 0 ? 0 : (tank.CurrentLevelLiters / tank.CapacityLiters) * 100;
                        break;
                }

                series.Points.AddXY(label, value);
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
            area.AxisX.Interval = 1;
            area.AxisX.TitleForeColor = UiHelper.ForeColor;
            area.AxisY.TitleForeColor = UiHelper.ForeColor;
        }
    }
}
