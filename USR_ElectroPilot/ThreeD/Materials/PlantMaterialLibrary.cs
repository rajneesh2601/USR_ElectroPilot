using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using USR_ElectroPilot.Helpers;

namespace USR_ElectroPilot.ThreeD.Materials
{
    public static class PlantMaterialLibrary
    {
        public static readonly Material SceneFloor = Create(Color.FromRgb(6, 20, 26), Color.FromRgb(35, 55, 62), 8);
        public static readonly Material PaintedBlueSteel = Create(Color.FromRgb(0, 78, 138), Color.FromRgb(92, 165, 220), 48);
        public static readonly Material PaintedYellowSteel = Create(Color.FromRgb(255, 186, 12), Color.FromRgb(255, 238, 112), 58);
        public static readonly Material ChemicalTankGray = Create(Color.FromRgb(145, 149, 139), Color.FromRgb(235, 238, 226), 34);
        public static readonly Material BrushedSteel = Create(Color.FromRgb(112, 118, 116), Color.FromRgb(220, 230, 225), 52);
        public static readonly Material DarkPipeMetal = Create(Color.FromRgb(31, 35, 36), Color.FromRgb(110, 118, 118), 30);
        public static readonly Material ProcessPipeGray = Create(Color.FromRgb(94, 101, 96), Color.FromRgb(185, 194, 185), 30);
        public static readonly Material PumpBlue = Create(Color.FromRgb(0, 83, 145), Color.FromRgb(90, 172, 235), 42);
        public static readonly Material ValveOrange = Create(Color.FromRgb(224, 92, 12), Color.FromRgb(255, 180, 80), 32);
        public static readonly Material DarkPlateMetal = Create(Color.FromRgb(74, 78, 76), Color.FromRgb(190, 200, 195), 45);
        public static readonly Material PlatingBoardGreen = Create(Color.FromRgb(20, 92, 58), Color.FromRgb(95, 180, 122), 36);
        public static readonly Material ChainSteel = Create(Color.FromRgb(70, 64, 50), Color.FromRgb(210, 194, 148), 36);
        public static readonly Material CopperCarrier = Create(Color.FromRgb(158, 86, 28), Color.FromRgb(255, 188, 95), 42);
        public static readonly Material Rubber = Create(Color.FromRgb(18, 20, 20), Color.FromRgb(58, 62, 62), 8);
        public static readonly Material LiquidBlue = Create(Color.FromArgb(155, 48, 158, 176), Color.FromRgb(150, 238, 245), 12);
        public static readonly Material LiquidGreen = Create(Color.FromArgb(150, 65, 160, 120), Color.FromRgb(150, 230, 190), 12);
        public static readonly Material LiquidBrown = Create(Color.FromArgb(145, 132, 86, 42), Color.FromRgb(220, 170, 95), 10);
        public static readonly Material WalkwayGrating = Create(Color.FromRgb(174, 112, 12), Color.FromRgb(230, 185, 70), 22);
        public static readonly Material RunningGreen = Create(Color.FromRgb(42, 215, 86), Color.FromRgb(165, 255, 180), 30);
        public static readonly Material WarningAmber = Create(Color.FromRgb(238, 176, 28), Color.FromRgb(255, 232, 110), 30);
        public static readonly Material AlarmRed = Create(Color.FromRgb(232, 65, 53), Color.FromRgb(255, 145, 125), 25);
        public static readonly Material SoftContactShadow = Create(Color.FromArgb(115, 0, 0, 0), Color.FromArgb(0, 0, 0, 0), 1);

        public static readonly Material RailBlue = PaintedBlueSteel;
        public static readonly Material WalkwayBlue = PaintedBlueSteel;
        public static readonly Material TankShell = ChemicalTankGray;
        public static readonly Material TankShellSelected = Create(Color.FromRgb(114, 156, 152), Color.FromRgb(220, 245, 238), 38);
        public static readonly Material TankBase = Create(Color.FromRgb(105, 111, 103), Color.FromRgb(178, 187, 176), 18);
        public static readonly Material Liquid = LiquidBlue;
        public static readonly Material TankLabel = Create(Color.FromRgb(226, 221, 205), Color.FromRgb(255, 255, 245), 10);
        public static readonly Material PumpDark = ProcessPipeGray;
        public static readonly Material PumpBodyBlue = PumpBlue;
        public static readonly Material PipeGray = ProcessPipeGray;
        public static readonly Material ValveHandle = ValveOrange;
        public static readonly Material HoistYellow = PaintedYellowSteel;
        public static readonly Material HoistEmergency = AlarmRed;
        public static readonly Material HoistMotor = BrushedSteel;
        public static readonly Material DarkMetal = DarkPipeMetal;
        public static readonly Material PlateMetal = PlatingBoardGreen;
        public static readonly Material StatusNormal = Create(Color.FromRgb(0, 178, 121), Color.FromRgb(120, 255, 205), 28);
        public static readonly Material StatusRunning = RunningGreen;
        public static readonly Material StatusWarning = WarningAmber;
        public static readonly Material StatusFault = AlarmRed;

        public static Material TankShellFor(bool selected)
        {
            return selected ? TankShellSelected : TankShell;
        }

        public static Material HoistForEmergency(bool emergencyStop)
        {
            return emergencyStop ? HoistEmergency : HoistYellow;
        }

        public static Material StatusFor(string status)
        {
            if (string.Equals(status, Constants.StatusFault, StringComparison.OrdinalIgnoreCase))
            {
                return StatusFault;
            }

            if (string.Equals(status, Constants.StatusWarning, StringComparison.OrdinalIgnoreCase))
            {
                return StatusWarning;
            }

            if (string.Equals(status, Constants.StatusRunning, StringComparison.OrdinalIgnoreCase))
            {
                return StatusRunning;
            }

            return StatusNormal;
        }

        private static Material Create(Color diffuse, Color specular, double specularPower)
        {
            var group = new MaterialGroup();

            var diffuseBrush = new SolidColorBrush(diffuse);
            diffuseBrush.Freeze();
            group.Children.Add(new DiffuseMaterial(diffuseBrush));

            var specularBrush = new SolidColorBrush(specular);
            specularBrush.Freeze();
            group.Children.Add(new SpecularMaterial(specularBrush, specularPower));

            group.Freeze();
            return group;
        }
    }
}
