using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace USR_ElectroPilot.Helpers
{
    public static class CsvExporter
    {
        public static void AddExportButton(DataGridView grid, string defaultFileName)
        {
            if (grid == null)
            {
                throw new ArgumentNullException("grid");
            }

            var button = new Button
            {
                Name = "btnExportCsv",
                Text = "Export CSV",
                Size = new Size(96, 28),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = UiHelper.AccentColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            button.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 110);

            PositionExportButton(grid, button);
            grid.Controls.Add(button);
            button.BringToFront();

            grid.Resize += delegate
            {
                PositionExportButton(grid, button);
                button.BringToFront();
            };

            button.Click += delegate
            {
                ExportWithDialog(grid, defaultFileName);
            };
        }

        public static void ExportDataGridView(DataGridView grid, string filePath)
        {
            if (grid == null)
            {
                throw new ArgumentNullException("grid");
            }

            var lines = new List<string>();
            var headers = new List<string>();

            foreach (DataGridViewColumn column in grid.Columns)
            {
                if (column.Visible)
                {
                    headers.Add(Escape(column.HeaderText));
                }
            }

            lines.Add(string.Join(",", headers.ToArray()));

            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }

                var values = new List<string>();
                foreach (DataGridViewColumn column in grid.Columns)
                {
                    if (column.Visible)
                    {
                        values.Add(Escape(Convert.ToString(row.Cells[column.Index].Value)));
                    }
                }

                lines.Add(string.Join(",", values.ToArray()));
            }

            File.WriteAllLines(filePath, lines.ToArray(), Encoding.UTF8);
        }

        private static void ExportWithDialog(DataGridView grid, string defaultFileName)
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                dialog.DefaultExt = "csv";
                dialog.AddExtension = true;
                dialog.FileName = BuildFileName(defaultFileName);

                if (dialog.ShowDialog(grid.FindForm()) != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    ExportDataGridView(grid, dialog.FileName);
                    MessageBox.Show("CSV export completed.", Constants.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    Logger.Error("CSV export failed", ex);
                    MessageBox.Show("CSV export failed. Check Logs folder.", Constants.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private static string BuildFileName(string defaultFileName)
        {
            var safeName = string.IsNullOrWhiteSpace(defaultFileName) ? "export" : defaultFileName.Trim();
            foreach (var invalidChar in Path.GetInvalidFileNameChars())
            {
                safeName = safeName.Replace(invalidChar, '_');
            }

            return safeName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
        }

        private static void PositionExportButton(DataGridView grid, Button button)
        {
            button.Location = new Point(Math.Max(4, grid.ClientSize.Width - button.Width - 18), 4);
        }

        private static string Escape(string value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }
    }
}
