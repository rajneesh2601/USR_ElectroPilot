namespace USR_ElectroPilot.Forms
{
    partial class ShiftReportForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel filterPanel;
        private System.Windows.Forms.DataGridView gridReports;
        private System.Windows.Forms.Panel editorPanel;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.Label lblFilterFrom;
        private System.Windows.Forms.Label lblFilterTo;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.DateTimePicker dtFilterFrom;
        private System.Windows.Forms.DateTimePicker dtFilterTo;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label lblEditorTitle;
        private System.Windows.Forms.Label lblShiftName;
        private System.Windows.Forms.Label lblOperator;
        private System.Windows.Forms.Label lblStarted;
        private System.Windows.Forms.Label lblEnded;
        private System.Windows.Forms.Label lblTotalLoads;
        private System.Windows.Forms.Label lblAlarmCount;
        private System.Windows.Forms.Label lblNotes;
        private System.Windows.Forms.TextBox txtShiftName;
        private System.Windows.Forms.TextBox txtOperator;
        private System.Windows.Forms.DateTimePicker dtStarted;
        private System.Windows.Forms.DateTimePicker dtEnded;
        private System.Windows.Forms.NumericUpDown numTotalLoads;
        private System.Windows.Forms.NumericUpDown numAlarmCount;
        private System.Windows.Forms.TextBox txtNotes;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnSave;

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
            this.filterPanel = new System.Windows.Forms.Panel();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.dtFilterTo = new System.Windows.Forms.DateTimePicker();
            this.dtFilterFrom = new System.Windows.Forms.DateTimePicker();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblFilterTo = new System.Windows.Forms.Label();
            this.lblFilterFrom = new System.Windows.Forms.Label();
            this.lblSearch = new System.Windows.Forms.Label();
            this.gridReports = new System.Windows.Forms.DataGridView();
            this.editorPanel = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.numAlarmCount = new System.Windows.Forms.NumericUpDown();
            this.numTotalLoads = new System.Windows.Forms.NumericUpDown();
            this.dtEnded = new System.Windows.Forms.DateTimePicker();
            this.dtStarted = new System.Windows.Forms.DateTimePicker();
            this.txtOperator = new System.Windows.Forms.TextBox();
            this.txtShiftName = new System.Windows.Forms.TextBox();
            this.lblNotes = new System.Windows.Forms.Label();
            this.lblAlarmCount = new System.Windows.Forms.Label();
            this.lblTotalLoads = new System.Windows.Forms.Label();
            this.lblEnded = new System.Windows.Forms.Label();
            this.lblStarted = new System.Windows.Forms.Label();
            this.lblOperator = new System.Windows.Forms.Label();
            this.lblShiftName = new System.Windows.Forms.Label();
            this.lblEditorTitle = new System.Windows.Forms.Label();
            this.filterPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridReports)).BeginInit();
            this.editorPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAlarmCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTotalLoads)).BeginInit();
            this.SuspendLayout();
            // 
            // filterPanel
            // 
            this.filterPanel.Controls.Add(this.btnRefresh);
            this.filterPanel.Controls.Add(this.btnApply);
            this.filterPanel.Controls.Add(this.dtFilterTo);
            this.filterPanel.Controls.Add(this.dtFilterFrom);
            this.filterPanel.Controls.Add(this.txtSearch);
            this.filterPanel.Controls.Add(this.lblFilterTo);
            this.filterPanel.Controls.Add(this.lblFilterFrom);
            this.filterPanel.Controls.Add(this.lblSearch);
            this.filterPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.filterPanel.Location = new System.Drawing.Point(0, 0);
            this.filterPanel.Name = "filterPanel";
            this.filterPanel.Padding = new System.Windows.Forms.Padding(12);
            this.filterPanel.Size = new System.Drawing.Size(1124, 86);
            this.filterPanel.TabIndex = 0;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(667, 39);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(90, 28);
            this.btnRefresh.TabIndex = 7;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(571, 39);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(90, 28);
            this.btnApply.TabIndex = 6;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.BtnApply_Click);
            // 
            // dtFilterTo
            // 
            this.dtFilterTo.CustomFormat = "yyyy-MM-dd HH:mm";
            this.dtFilterTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtFilterTo.Location = new System.Drawing.Point(391, 43);
            this.dtFilterTo.Name = "dtFilterTo";
            this.dtFilterTo.Size = new System.Drawing.Size(160, 20);
            this.dtFilterTo.TabIndex = 5;
            // 
            // dtFilterFrom
            // 
            this.dtFilterFrom.CustomFormat = "yyyy-MM-dd HH:mm";
            this.dtFilterFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtFilterFrom.Location = new System.Drawing.Point(215, 43);
            this.dtFilterFrom.Name = "dtFilterFrom";
            this.dtFilterFrom.Size = new System.Drawing.Size(160, 20);
            this.dtFilterFrom.TabIndex = 3;
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(16, 43);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(178, 20);
            this.txtSearch.TabIndex = 1;
            // 
            // filter labels
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(16, 23);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(117, 13);
            this.lblSearch.TabIndex = 0;
            this.lblSearch.Text = "Shift/operator/notes";
            this.lblFilterFrom.AutoSize = true;
            this.lblFilterFrom.Location = new System.Drawing.Point(215, 23);
            this.lblFilterFrom.Name = "lblFilterFrom";
            this.lblFilterFrom.Size = new System.Drawing.Size(30, 13);
            this.lblFilterFrom.TabIndex = 2;
            this.lblFilterFrom.Text = "From";
            this.lblFilterTo.AutoSize = true;
            this.lblFilterTo.Location = new System.Drawing.Point(391, 23);
            this.lblFilterTo.Name = "lblFilterTo";
            this.lblFilterTo.Size = new System.Drawing.Size(20, 13);
            this.lblFilterTo.TabIndex = 4;
            this.lblFilterTo.Text = "To";
            // 
            // gridReports
            // 
            this.gridReports.AllowUserToAddRows = false;
            this.gridReports.AllowUserToDeleteRows = false;
            this.gridReports.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridReports.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridReports.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridReports.Location = new System.Drawing.Point(0, 86);
            this.gridReports.Name = "gridReports";
            this.gridReports.ReadOnly = true;
            this.gridReports.RowHeadersVisible = false;
            this.gridReports.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridReports.Size = new System.Drawing.Size(764, 515);
            this.gridReports.TabIndex = 1;
            // 
            // editorPanel
            // 
            this.editorPanel.Controls.Add(this.btnSave);
            this.editorPanel.Controls.Add(this.btnNew);
            this.editorPanel.Controls.Add(this.txtNotes);
            this.editorPanel.Controls.Add(this.numAlarmCount);
            this.editorPanel.Controls.Add(this.numTotalLoads);
            this.editorPanel.Controls.Add(this.dtEnded);
            this.editorPanel.Controls.Add(this.dtStarted);
            this.editorPanel.Controls.Add(this.txtOperator);
            this.editorPanel.Controls.Add(this.txtShiftName);
            this.editorPanel.Controls.Add(this.lblNotes);
            this.editorPanel.Controls.Add(this.lblAlarmCount);
            this.editorPanel.Controls.Add(this.lblTotalLoads);
            this.editorPanel.Controls.Add(this.lblEnded);
            this.editorPanel.Controls.Add(this.lblStarted);
            this.editorPanel.Controls.Add(this.lblOperator);
            this.editorPanel.Controls.Add(this.lblShiftName);
            this.editorPanel.Controls.Add(this.lblEditorTitle);
            this.editorPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.editorPanel.Location = new System.Drawing.Point(764, 86);
            this.editorPanel.Name = "editorPanel";
            this.editorPanel.Padding = new System.Windows.Forms.Padding(16);
            this.editorPanel.Size = new System.Drawing.Size(360, 515);
            this.editorPanel.TabIndex = 2;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(255, 453);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(82, 30);
            this.btnSave.TabIndex = 16;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(167, 453);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(82, 30);
            this.btnNew.TabIndex = 15;
            this.btnNew.Text = "New";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.BtnNew_Click);
            // 
            // txtNotes
            // 
            this.txtNotes.Location = new System.Drawing.Point(20, 335);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtNotes.Size = new System.Drawing.Size(317, 95);
            this.txtNotes.TabIndex = 14;
            // 
            // numAlarmCount
            // 
            this.numAlarmCount.Location = new System.Drawing.Point(20, 291);
            this.numAlarmCount.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numAlarmCount.Name = "numAlarmCount";
            this.numAlarmCount.Size = new System.Drawing.Size(317, 20);
            this.numAlarmCount.TabIndex = 12;
            // 
            // numTotalLoads
            // 
            this.numTotalLoads.Location = new System.Drawing.Point(20, 241);
            this.numTotalLoads.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numTotalLoads.Name = "numTotalLoads";
            this.numTotalLoads.Size = new System.Drawing.Size(317, 20);
            this.numTotalLoads.TabIndex = 10;
            // 
            // dtEnded
            // 
            this.dtEnded.Checked = false;
            this.dtEnded.CustomFormat = "yyyy-MM-dd HH:mm";
            this.dtEnded.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtEnded.Location = new System.Drawing.Point(20, 191);
            this.dtEnded.Name = "dtEnded";
            this.dtEnded.ShowCheckBox = true;
            this.dtEnded.Size = new System.Drawing.Size(317, 20);
            this.dtEnded.TabIndex = 8;
            // 
            // dtStarted
            // 
            this.dtStarted.CustomFormat = "yyyy-MM-dd HH:mm";
            this.dtStarted.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtStarted.Location = new System.Drawing.Point(20, 141);
            this.dtStarted.Name = "dtStarted";
            this.dtStarted.Size = new System.Drawing.Size(317, 20);
            this.dtStarted.TabIndex = 6;
            // 
            // txtOperator
            // 
            this.txtOperator.Location = new System.Drawing.Point(20, 94);
            this.txtOperator.Name = "txtOperator";
            this.txtOperator.Size = new System.Drawing.Size(317, 20);
            this.txtOperator.TabIndex = 4;
            // 
            // txtShiftName
            // 
            this.txtShiftName.Location = new System.Drawing.Point(20, 48);
            this.txtShiftName.Name = "txtShiftName";
            this.txtShiftName.Size = new System.Drawing.Size(317, 20);
            this.txtShiftName.TabIndex = 2;
            // 
            // editor labels
            // 
            this.lblEditorTitle.AutoSize = true;
            this.lblEditorTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblEditorTitle.Location = new System.Drawing.Point(16, 16);
            this.lblEditorTitle.Name = "lblEditorTitle";
            this.lblEditorTitle.Size = new System.Drawing.Size(87, 19);
            this.lblEditorTitle.TabIndex = 0;
            this.lblEditorTitle.Text = "Shift Report";
            this.lblShiftName.AutoSize = true;
            this.lblShiftName.Location = new System.Drawing.Point(20, 32);
            this.lblShiftName.Name = "lblShiftName";
            this.lblShiftName.Size = new System.Drawing.Size(58, 13);
            this.lblShiftName.TabIndex = 1;
            this.lblShiftName.Text = "Shift name";
            this.lblOperator.AutoSize = true;
            this.lblOperator.Location = new System.Drawing.Point(20, 78);
            this.lblOperator.Name = "lblOperator";
            this.lblOperator.Size = new System.Drawing.Size(91, 13);
            this.lblOperator.TabIndex = 3;
            this.lblOperator.Text = "Operator username";
            this.lblStarted.AutoSize = true;
            this.lblStarted.Location = new System.Drawing.Point(20, 125);
            this.lblStarted.Name = "lblStarted";
            this.lblStarted.Size = new System.Drawing.Size(56, 13);
            this.lblStarted.TabIndex = 5;
            this.lblStarted.Text = "Started at";
            this.lblEnded.AutoSize = true;
            this.lblEnded.Location = new System.Drawing.Point(20, 175);
            this.lblEnded.Name = "lblEnded";
            this.lblEnded.Size = new System.Drawing.Size(53, 13);
            this.lblEnded.TabIndex = 7;
            this.lblEnded.Text = "Ended at";
            this.lblTotalLoads.AutoSize = true;
            this.lblTotalLoads.Location = new System.Drawing.Point(20, 225);
            this.lblTotalLoads.Name = "lblTotalLoads";
            this.lblTotalLoads.Size = new System.Drawing.Size(62, 13);
            this.lblTotalLoads.TabIndex = 9;
            this.lblTotalLoads.Text = "Total loads";
            this.lblAlarmCount.AutoSize = true;
            this.lblAlarmCount.Location = new System.Drawing.Point(20, 275);
            this.lblAlarmCount.Name = "lblAlarmCount";
            this.lblAlarmCount.Size = new System.Drawing.Size(65, 13);
            this.lblAlarmCount.TabIndex = 11;
            this.lblAlarmCount.Text = "Alarm count";
            this.lblNotes.AutoSize = true;
            this.lblNotes.Location = new System.Drawing.Point(20, 319);
            this.lblNotes.Name = "lblNotes";
            this.lblNotes.Size = new System.Drawing.Size(35, 13);
            this.lblNotes.TabIndex = 13;
            this.lblNotes.Text = "Notes";
            // 
            // ShiftReportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1124, 601);
            this.Controls.Add(this.gridReports);
            this.Controls.Add(this.editorPanel);
            this.Controls.Add(this.filterPanel);
            this.MinimumSize = new System.Drawing.Size(960, 560);
            this.Name = "ShiftReportForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Shift Reports";
            this.Load += new System.EventHandler(this.ShiftReportForm_Load);
            this.filterPanel.ResumeLayout(false);
            this.filterPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridReports)).EndInit();
            this.editorPanel.ResumeLayout(false);
            this.editorPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAlarmCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTotalLoads)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
