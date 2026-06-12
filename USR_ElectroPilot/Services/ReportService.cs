using System.Windows.Forms;
using USR_ElectroPilot.Helpers;

namespace USR_ElectroPilot.Services
{
    public class ReportService
    {
        public void ExportGrid(DataGridView grid, string filePath)
        {
            CsvExporter.ExportDataGridView(grid, filePath);
        }
    }
}
