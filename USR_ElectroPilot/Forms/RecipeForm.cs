using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using USR_ElectroPilot.Helpers;
using USR_ElectroPilot.Models;
using USR_ElectroPilot.Services;

namespace USR_ElectroPilot.Forms
{
    public partial class RecipeForm : Form
    {
        private readonly RecipeService _recipeService = new RecipeService();
        private List<RecipeModel> _recipes = new List<RecipeModel>();

        public RecipeForm()
        {
            InitializeComponent();
        }

        private void RecipeForm_Load(object sender, EventArgs e)
        {
            UiHelper.ApplyDarkTheme(this);
            cboActiveFilter.Items.AddRange(new object[] { "All", "Active", "Inactive" });
            cboActiveFilter.SelectedIndex = 0;
            chkActive.Checked = true;
            RefreshRecipes();
            ClearEditor();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            RefreshRecipes();
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void BtnNew_Click(object sender, EventArgs e)
        {
            ClearEditor();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateRecipe())
            {
                return;
            }

            var recipe = new RecipeModel
            {
                Name = txtName.Text.Trim(),
                Description = txtDescription.Text.Trim(),
                TargetVoltage = Convert.ToDouble(numVoltage.Value),
                TargetCurrentAmps = Convert.ToDouble(numCurrent.Value),
                DurationMinutes = Convert.ToInt32(numDuration.Value),
                IsActive = chkActive.Checked
            };

            _recipeService.AddRecipe(recipe);
            RefreshRecipes();
            ClearEditor();
            MessageBox.Show("Recipe saved.", Constants.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void GridRecipes_SelectionChanged(object sender, EventArgs e)
        {
            if (gridRecipes.CurrentRow == null || gridRecipes.CurrentRow.DataBoundItem == null)
            {
                return;
            }

            var recipe = gridRecipes.CurrentRow.DataBoundItem as RecipeModel;
            if (recipe == null)
            {
                return;
            }

            txtName.Text = recipe.Name;
            txtDescription.Text = recipe.Description;
            numVoltage.Value = Clamp(recipe.TargetVoltage, numVoltage.Minimum, numVoltage.Maximum);
            numCurrent.Value = Clamp(recipe.TargetCurrentAmps, numCurrent.Minimum, numCurrent.Maximum);
            numDuration.Value = Clamp(recipe.DurationMinutes, numDuration.Minimum, numDuration.Maximum);
            chkActive.Checked = recipe.IsActive;
        }

        private void RefreshRecipes()
        {
            _recipes = _recipeService.GetRecipes();
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            IEnumerable<RecipeModel> query = _recipes;

            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                var search = txtSearch.Text.Trim();
                query = query.Where(r =>
                    (!string.IsNullOrEmpty(r.Name) && r.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (!string.IsNullOrEmpty(r.Description) && r.Description.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0));
            }

            if (cboActiveFilter.Text == "Active")
            {
                query = query.Where(r => r.IsActive);
            }
            else if (cboActiveFilter.Text == "Inactive")
            {
                query = query.Where(r => !r.IsActive);
            }

            gridRecipes.DataSource = query.OrderBy(r => r.Name).ToList();
            FormatGrid();
        }

        private void FormatGrid()
        {
            if (gridRecipes.Columns["Id"] != null)
            {
                gridRecipes.Columns["Id"].Width = 55;
            }

            if (gridRecipes.Columns["CreatedAt"] != null)
            {
                gridRecipes.Columns["CreatedAt"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm";
            }

            if (gridRecipes.Columns["Description"] != null)
            {
                gridRecipes.Columns["Description"].FillWeight = 160;
            }
        }

        private bool ValidateRecipe()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Recipe name is required.", Constants.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtName.Focus();
                return false;
            }

            if (numDuration.Value <= 0)
            {
                MessageBox.Show("Duration must be greater than zero.", Constants.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                numDuration.Focus();
                return false;
            }

            return true;
        }

        private void ClearEditor()
        {
            txtName.Clear();
            txtDescription.Clear();
            numVoltage.Value = 0;
            numCurrent.Value = 0;
            numDuration.Value = 1;
            chkActive.Checked = true;
            txtName.Focus();
        }

        private static decimal Clamp(double value, decimal min, decimal max)
        {
            var decimalValue = Convert.ToDecimal(value);
            if (decimalValue < min)
            {
                return min;
            }

            return decimalValue > max ? max : decimalValue;
        }
    }
}
