namespace USR_ElectroPilot.Forms
{
    partial class TrendForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel topPanel;
        private System.Windows.Forms.Label lblMetric;
        private System.Windows.Forms.Label lblTank;
        private System.Windows.Forms.Label lblFrom;
        private System.Windows.Forms.Label lblTo;
        private System.Windows.Forms.ComboBox cboMetric;
        private System.Windows.Forms.ComboBox cboTank;
        private System.Windows.Forms.DateTimePicker dtFrom;
        private System.Windows.Forms.DateTimePicker dtTo;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label lblSummary;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartTrends;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            this.topPanel = new System.Windows.Forms.Panel();
            this.lblSummary = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.dtTo = new System.Windows.Forms.DateTimePicker();
            this.dtFrom = new System.Windows.Forms.DateTimePicker();
            this.cboTank = new System.Windows.Forms.ComboBox();
            this.cboMetric = new System.Windows.Forms.ComboBox();
            this.lblTo = new System.Windows.Forms.Label();
            this.lblFrom = new System.Windows.Forms.Label();
            this.lblTank = new System.Windows.Forms.Label();
            this.lblMetric = new System.Windows.Forms.Label();
            this.chartTrends = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.topPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartTrends)).BeginInit();
            this.SuspendLayout();
            // 
            // topPanel
            // 
            this.topPanel.Controls.Add(this.lblSummary);
            this.topPanel.Controls.Add(this.btnRefresh);
            this.topPanel.Controls.Add(this.dtTo);
            this.topPanel.Controls.Add(this.dtFrom);
            this.topPanel.Controls.Add(this.cboTank);
            this.topPanel.Controls.Add(this.cboMetric);
            this.topPanel.Controls.Add(this.lblTo);
            this.topPanel.Controls.Add(this.lblFrom);
            this.topPanel.Controls.Add(this.lblTank);
            this.topPanel.Controls.Add(this.lblMetric);
            this.topPanel.AutoScroll = true;
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point(0, 0);
            this.topPanel.Name = "topPanel";
            this.topPanel.Padding = new System.Windows.Forms.Padding(12);
            this.topPanel.Size = new System.Drawing.Size(1044, 78);
            this.topPanel.TabIndex = 0;
            // 
            // lblSummary
            // 
            this.lblSummary.AutoSize = true;
            this.lblSummary.MaximumSize = new System.Drawing.Size(260, 0);
            this.lblSummary.Location = new System.Drawing.Point(763, 43);
            this.lblSummary.Name = "lblSummary";
            this.lblSummary.Size = new System.Drawing.Size(92, 13);
            this.lblSummary.TabIndex = 3;
            this.lblSummary.Text = "No data available.";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(653, 35);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(90, 28);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
            // 
            // dtTo
            // 
            this.dtTo.CustomFormat = "yyyy-MM-dd HH:mm";
            this.dtTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtTo.Location = new System.Drawing.Point(477, 39);
            this.dtTo.Name = "dtTo";
            this.dtTo.Size = new System.Drawing.Size(160, 20);
            this.dtTo.TabIndex = 7;
            // 
            // dtFrom
            // 
            this.dtFrom.CustomFormat = "yyyy-MM-dd HH:mm";
            this.dtFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtFrom.Location = new System.Drawing.Point(301, 39);
            this.dtFrom.Name = "dtFrom";
            this.dtFrom.Size = new System.Drawing.Size(160, 20);
            this.dtFrom.TabIndex = 5;
            // 
            // cboTank
            // 
            this.cboTank.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTank.Location = new System.Drawing.Point(207, 39);
            this.cboTank.Name = "cboTank";
            this.cboTank.Size = new System.Drawing.Size(78, 21);
            this.cboTank.TabIndex = 3;
            this.cboTank.SelectedIndexChanged += new System.EventHandler(this.CboTank_SelectedIndexChanged);
            // 
            // cboMetric
            // 
            this.cboMetric.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMetric.Location = new System.Drawing.Point(16, 39);
            this.cboMetric.Name = "cboMetric";
            this.cboMetric.Size = new System.Drawing.Size(185, 21);
            this.cboMetric.TabIndex = 1;
            this.cboMetric.SelectedIndexChanged += new System.EventHandler(this.CboMetric_SelectedIndexChanged);
            // 
            // lblTo
            // 
            this.lblTo.AutoSize = true;
            this.lblTo.Location = new System.Drawing.Point(477, 20);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(20, 13);
            this.lblTo.TabIndex = 6;
            this.lblTo.Text = "To";
            // 
            // lblFrom
            // 
            this.lblFrom.AutoSize = true;
            this.lblFrom.Location = new System.Drawing.Point(301, 20);
            this.lblFrom.Name = "lblFrom";
            this.lblFrom.Size = new System.Drawing.Size(30, 13);
            this.lblFrom.TabIndex = 4;
            this.lblFrom.Text = "From";
            // 
            // lblTank
            // 
            this.lblTank.AutoSize = true;
            this.lblTank.Location = new System.Drawing.Point(207, 20);
            this.lblTank.Name = "lblTank";
            this.lblTank.Size = new System.Drawing.Size(32, 13);
            this.lblTank.TabIndex = 2;
            this.lblTank.Text = "Tank";
            // 
            // lblMetric
            // 
            this.lblMetric.AutoSize = true;
            this.lblMetric.Location = new System.Drawing.Point(16, 20);
            this.lblMetric.Name = "lblMetric";
            this.lblMetric.Size = new System.Drawing.Size(36, 13);
            this.lblMetric.TabIndex = 0;
            this.lblMetric.Text = "Metric";
            // 
            // chartTrends
            // 
            chartArea1.Name = "TrendArea";
            this.chartTrends.ChartAreas.Add(chartArea1);
            this.chartTrends.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "TrendLegend";
            this.chartTrends.Legends.Add(legend1);
            this.chartTrends.Location = new System.Drawing.Point(0, 78);
            this.chartTrends.Name = "chartTrends";
            this.chartTrends.Size = new System.Drawing.Size(1044, 483);
            this.chartTrends.TabIndex = 1;
            this.chartTrends.Text = "Trend chart";
            // 
            // TrendForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1044, 561);
            this.Controls.Add(this.chartTrends);
            this.Controls.Add(this.topPanel);
            this.MinimumSize = new System.Drawing.Size(900, 500);
            this.Name = "TrendForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Trends";
            this.Load += new System.EventHandler(this.TrendForm_Load);
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartTrends)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
