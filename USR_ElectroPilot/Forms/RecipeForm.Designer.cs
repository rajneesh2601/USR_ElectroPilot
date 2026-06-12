namespace USR_ElectroPilot.Forms
{
    partial class RecipeForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel filterPanel;
        private System.Windows.Forms.DataGridView gridRecipes;
        private System.Windows.Forms.Panel editorPanel;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.Label lblActiveFilter;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.ComboBox cboActiveFilter;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label lblEditorTitle;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Label lblVoltage;
        private System.Windows.Forms.Label lblCurrent;
        private System.Windows.Forms.Label lblDuration;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.NumericUpDown numVoltage;
        private System.Windows.Forms.NumericUpDown numCurrent;
        private System.Windows.Forms.NumericUpDown numDuration;
        private System.Windows.Forms.CheckBox chkActive;
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
            this.cboActiveFilter = new System.Windows.Forms.ComboBox();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblActiveFilter = new System.Windows.Forms.Label();
            this.lblSearch = new System.Windows.Forms.Label();
            this.gridRecipes = new System.Windows.Forms.DataGridView();
            this.editorPanel = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.chkActive = new System.Windows.Forms.CheckBox();
            this.numDuration = new System.Windows.Forms.NumericUpDown();
            this.numCurrent = new System.Windows.Forms.NumericUpDown();
            this.numVoltage = new System.Windows.Forms.NumericUpDown();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblDuration = new System.Windows.Forms.Label();
            this.lblCurrent = new System.Windows.Forms.Label();
            this.lblVoltage = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.lblEditorTitle = new System.Windows.Forms.Label();
            this.filterPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridRecipes)).BeginInit();
            this.editorPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDuration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCurrent)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numVoltage)).BeginInit();
            this.SuspendLayout();
            // 
            // filterPanel
            // 
            this.filterPanel.Controls.Add(this.btnRefresh);
            this.filterPanel.Controls.Add(this.btnApply);
            this.filterPanel.Controls.Add(this.cboActiveFilter);
            this.filterPanel.Controls.Add(this.txtSearch);
            this.filterPanel.Controls.Add(this.lblActiveFilter);
            this.filterPanel.Controls.Add(this.lblSearch);
            this.filterPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.filterPanel.Location = new System.Drawing.Point(0, 0);
            this.filterPanel.Name = "filterPanel";
            this.filterPanel.Padding = new System.Windows.Forms.Padding(12);
            this.filterPanel.Size = new System.Drawing.Size(1044, 78);
            this.filterPanel.TabIndex = 0;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(515, 37);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(90, 28);
            this.btnRefresh.TabIndex = 5;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(419, 37);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(90, 28);
            this.btnApply.TabIndex = 4;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.BtnApply_Click);
            // 
            // cboActiveFilter
            // 
            this.cboActiveFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboActiveFilter.Location = new System.Drawing.Point(267, 40);
            this.cboActiveFilter.Name = "cboActiveFilter";
            this.cboActiveFilter.Size = new System.Drawing.Size(130, 21);
            this.cboActiveFilter.TabIndex = 3;
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(16, 41);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(230, 20);
            this.txtSearch.TabIndex = 1;
            // 
            // lblActiveFilter
            // 
            this.lblActiveFilter.AutoSize = true;
            this.lblActiveFilter.Location = new System.Drawing.Point(267, 20);
            this.lblActiveFilter.Name = "lblActiveFilter";
            this.lblActiveFilter.Size = new System.Drawing.Size(37, 13);
            this.lblActiveFilter.TabIndex = 2;
            this.lblActiveFilter.Text = "Status";
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(16, 20);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(84, 13);
            this.lblSearch.TabIndex = 0;
            this.lblSearch.Text = "Name or details";
            // 
            // gridRecipes
            // 
            this.gridRecipes.AllowUserToAddRows = false;
            this.gridRecipes.AllowUserToDeleteRows = false;
            this.gridRecipes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridRecipes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridRecipes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridRecipes.Location = new System.Drawing.Point(0, 78);
            this.gridRecipes.MultiSelect = false;
            this.gridRecipes.Name = "gridRecipes";
            this.gridRecipes.ReadOnly = true;
            this.gridRecipes.RowHeadersVisible = false;
            this.gridRecipes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridRecipes.Size = new System.Drawing.Size(704, 483);
            this.gridRecipes.TabIndex = 1;
            this.gridRecipes.SelectionChanged += new System.EventHandler(this.GridRecipes_SelectionChanged);
            // 
            // editorPanel
            // 
            this.editorPanel.Controls.Add(this.btnSave);
            this.editorPanel.Controls.Add(this.btnNew);
            this.editorPanel.Controls.Add(this.chkActive);
            this.editorPanel.Controls.Add(this.numDuration);
            this.editorPanel.Controls.Add(this.numCurrent);
            this.editorPanel.Controls.Add(this.numVoltage);
            this.editorPanel.Controls.Add(this.txtDescription);
            this.editorPanel.Controls.Add(this.txtName);
            this.editorPanel.Controls.Add(this.lblDuration);
            this.editorPanel.Controls.Add(this.lblCurrent);
            this.editorPanel.Controls.Add(this.lblVoltage);
            this.editorPanel.Controls.Add(this.lblDescription);
            this.editorPanel.Controls.Add(this.lblName);
            this.editorPanel.Controls.Add(this.lblEditorTitle);
            this.editorPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.editorPanel.Location = new System.Drawing.Point(704, 78);
            this.editorPanel.Name = "editorPanel";
            this.editorPanel.Padding = new System.Windows.Forms.Padding(16);
            this.editorPanel.Size = new System.Drawing.Size(340, 483);
            this.editorPanel.TabIndex = 2;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(235, 383);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(82, 30);
            this.btnSave.TabIndex = 13;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(147, 383);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(82, 30);
            this.btnNew.TabIndex = 12;
            this.btnNew.Text = "New";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.BtnNew_Click);
            // 
            // chkActive
            // 
            this.chkActive.AutoSize = true;
            this.chkActive.Checked = true;
            this.chkActive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkActive.Location = new System.Drawing.Point(20, 341);
            this.chkActive.Name = "chkActive";
            this.chkActive.Size = new System.Drawing.Size(56, 17);
            this.chkActive.TabIndex = 11;
            this.chkActive.Text = "Active";
            this.chkActive.UseVisualStyleBackColor = true;
            // 
            // numDuration
            // 
            this.numDuration.Location = new System.Drawing.Point(20, 297);
            this.numDuration.Maximum = new decimal(new int[] {
            10080,
            0,
            0,
            0});
            this.numDuration.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numDuration.Name = "numDuration";
            this.numDuration.Size = new System.Drawing.Size(297, 20);
            this.numDuration.TabIndex = 10;
            this.numDuration.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // numCurrent
            // 
            this.numCurrent.DecimalPlaces = 2;
            this.numCurrent.Location = new System.Drawing.Point(20, 247);
            this.numCurrent.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numCurrent.Name = "numCurrent";
            this.numCurrent.Size = new System.Drawing.Size(297, 20);
            this.numCurrent.TabIndex = 8;
            // 
            // numVoltage
            // 
            this.numVoltage.DecimalPlaces = 2;
            this.numVoltage.Location = new System.Drawing.Point(20, 197);
            this.numVoltage.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numVoltage.Name = "numVoltage";
            this.numVoltage.Size = new System.Drawing.Size(297, 20);
            this.numVoltage.TabIndex = 6;
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(20, 94);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDescription.Size = new System.Drawing.Size(297, 72);
            this.txtDescription.TabIndex = 4;
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(20, 48);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(297, 20);
            this.txtName.TabIndex = 2;
            // 
            // labels
            // 
            this.lblEditorTitle.AutoSize = true;
            this.lblEditorTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblEditorTitle.Location = new System.Drawing.Point(16, 16);
            this.lblEditorTitle.Name = "lblEditorTitle";
            this.lblEditorTitle.Size = new System.Drawing.Size(80, 19);
            this.lblEditorTitle.TabIndex = 0;
            this.lblEditorTitle.Text = "Recipe";
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(20, 32);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(35, 13);
            this.lblName.TabIndex = 1;
            this.lblName.Text = "Name";
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(20, 78);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(60, 13);
            this.lblDescription.TabIndex = 3;
            this.lblDescription.Text = "Description";
            this.lblVoltage.AutoSize = true;
            this.lblVoltage.Location = new System.Drawing.Point(20, 181);
            this.lblVoltage.Name = "lblVoltage";
            this.lblVoltage.Size = new System.Drawing.Size(85, 13);
            this.lblVoltage.TabIndex = 5;
            this.lblVoltage.Text = "Target voltage";
            this.lblCurrent.AutoSize = true;
            this.lblCurrent.Location = new System.Drawing.Point(20, 231);
            this.lblCurrent.Name = "lblCurrent";
            this.lblCurrent.Size = new System.Drawing.Size(105, 13);
            this.lblCurrent.TabIndex = 7;
            this.lblCurrent.Text = "Target current amps";
            this.lblDuration.AutoSize = true;
            this.lblDuration.Location = new System.Drawing.Point(20, 281);
            this.lblDuration.Name = "lblDuration";
            this.lblDuration.Size = new System.Drawing.Size(85, 13);
            this.lblDuration.TabIndex = 9;
            this.lblDuration.Text = "Duration minutes";
            // 
            // RecipeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1044, 561);
            this.Controls.Add(this.gridRecipes);
            this.Controls.Add(this.editorPanel);
            this.Controls.Add(this.filterPanel);
            this.MinimumSize = new System.Drawing.Size(900, 500);
            this.Name = "RecipeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Recipes";
            this.Load += new System.EventHandler(this.RecipeForm_Load);
            this.filterPanel.ResumeLayout(false);
            this.filterPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridRecipes)).EndInit();
            this.editorPanel.ResumeLayout(false);
            this.editorPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDuration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCurrent)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numVoltage)).EndInit();
            this.ResumeLayout(false);
        }
    }
}
