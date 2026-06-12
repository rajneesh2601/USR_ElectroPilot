using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace USR_ElectroPilot.Helpers
{
    public static class CsvExporter
    {
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
