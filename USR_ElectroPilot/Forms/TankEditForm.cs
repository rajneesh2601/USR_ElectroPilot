using System;
using System.Windows.Forms;
using USR_ElectroPilot.Helpers;
using USR_ElectroPilot.Models;

namespace USR_ElectroPilot.Forms
{
    public partial class TankEditForm : Form
    {
        public TankEditForm()
            : this(null)
        {
        }

        public TankEditForm(TankModel tank)
        {
            InitializeComponent();
            Tank = tank == null ? new TankModel { Status = Constants.StatusNormal, IsActive = true } : CopyTank(tank);
        }

        public TankModel Tank { get; private set; }

        private void TankEditForm_Load(object sender, EventArgs e)
        {
            UiHelper.ApplyDarkTheme(this);
            numLineId.Value = 1;
            txtName.Text = Tank.Name;
            txtChemical.Text = Tank.ChemicalName;
            numTankNumber.Value = Tank.TankNo <= 0 ? 1 : Tank.TankNo;
            numCapacity.Value = ToDecimal(Tank.CapacityLiters, 0, 100000);
            numLevel.Value = ToDecimal(Tank.CurrentLevelLiters, 0, 100000);
            numTemperature.Value = ToDecimal(Tank.TemperatureCelsius, 0, 200);
            numVoltage.Value = ToDecimal(Tank.Voltage, 0, 100);
            numCurrent.Value = ToDecimal(Tank.CurrentAmps, 0, 10000);
            cboStatus.Text = string.IsNullOrEmpty(Tank.Status) ? Constants.StatusNormal : Tank.Status;
            chkActive.Checked = Tank.IsActive;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Tank name is required.", Constants.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Tank.LineId = 1;
            Tank.TankNo = Convert.ToInt32(numTankNumber.Value);
            Tank.Name = txtName.Text.Trim();
            Tank.ChemicalName = txtChemical.Text.Trim();
            Tank.CapacityLiters = Convert.ToDouble(numCapacity.Value);
            Tank.CurrentLevelLiters = Convert.ToDouble(numLevel.Value);
            Tank.TemperatureCelsius = Convert.ToDouble(numTemperature.Value);
            Tank.Voltage = Convert.ToDouble(numVoltage.Value);
            Tank.CurrentAmps = Convert.ToDouble(numCurrent.Value);
            Tank.Status = cboStatus.Text;
            Tank.IsActive = chkActive.Checked;

            DialogResult = DialogResult.OK;
            Close();
        }

        private static TankModel CopyTank(TankModel tank)
        {
            return new TankModel
            {
                Id = tank.Id,
                LineId = tank.LineId,
                TankNo = tank.TankNo,
                Name = tank.Name,
                LineName = tank.LineName,
                ChemicalName = tank.ChemicalName,
                CapacityLiters = tank.CapacityLiters,
                CurrentLevelLiters = tank.CurrentLevelLiters,
                TemperatureCelsius = tank.TemperatureCelsius,
                Voltage = tank.Voltage,
                CurrentAmps = tank.CurrentAmps,
                Status = tank.Status,
                IsActive = tank.IsActive,
                CreatedAt = tank.CreatedAt
            };
        }

        private static decimal ToDecimal(double value, decimal min, decimal max)
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
