using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using USR_ElectroPilot.Models;
using USR_ElectroPilot.ThreeD.Materials;

namespace USR_ElectroPilot.ThreeD.Components
{
    public static class PortalHoist3DBuilder
    {
        public static void AddHoist(HelixViewport3D viewport, HoistModel hoist, double x, bool emergencyStop)
        {
            var status = emergencyStop ? "EmergencyStop" : hoist == null || string.IsNullOrWhiteSpace(hoist.Status) ? "Idle" : hoist.Status;
            var liftProgress = GetDefaultLiftProgress(status);
            AddHoist(viewport, hoist, x, emergencyStop, liftProgress);
        }

        public static void AddHoist(HelixViewport3D viewport, HoistModel hoist, double x, bool emergencyStop, double liftProgress)
        {
            if (viewport == null)
            {
                throw new ArgumentNullException("viewport");
            }

            var status = emergencyStop ? "EmergencyStop" : hoist == null || string.IsNullOrWhiteSpace(hoist.Status) ? "Idle" : hoist.Status;
            var clampedLiftProgress = Math.Max(0, Math.Min(1, liftProgress));
            var liftZ = 2.16 - (0.74 * clampedLiftProgress);
            var hoistMaterial = PlantMaterialLibrary.HoistForEmergency(emergencyStop);

            AddPortalFrame(viewport, x, hoistMaterial);
            AddTrolley(viewport, x, hoistMaterial);
            LiftAssembly3DBuilder.AddLiftAssembly(viewport, x, liftZ, hoistMaterial);
            AddStatusStack(viewport, x, status);
        }

        private static double GetDefaultLiftProgress(string status)
        {
            return string.Equals(status, "Processing", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(status, "Lowering", StringComparison.OrdinalIgnoreCase)
                ? 1
                : 0;
        }

        private static void AddPortalFrame(HelixViewport3D viewport, double x, Material hoistMaterial)
        {
            AddBox(viewport, new Point3D(x, 0, 2.58), 1.46, 3.02, 0.24, hoistMaterial);
            AddBox(viewport, new Point3D(x, -1.53, 2.58), 1.52, 0.055, 0.3, hoistMaterial);
            AddBox(viewport, new Point3D(x, 1.53, 2.58), 1.52, 0.055, 0.3, hoistMaterial);
            AddBox(viewport, new Point3D(x, 0, 2.37), 1.26, 2.7, 0.08, PlantMaterialLibrary.DarkMetal);
            AddBox(viewport, new Point3D(x, 0, 2.16), 1.0, 2.45, 0.08, hoistMaterial);
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(x - 0.6, -1.3, 2.28), new Point3D(x + 0.6, -1.3, 2.28), 0.055, PlantMaterialLibrary.DarkMetal, 14);
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(x - 0.6, 1.3, 2.28), new Point3D(x + 0.6, 1.3, 2.28), 0.055, PlantMaterialLibrary.DarkMetal, 14);
            AddBeamBoltRow(viewport, x, -1.565, 2.66);
            AddBeamBoltRow(viewport, x, -1.565, 2.48);
            AddHoistNameBillboard(viewport, x);

            AddLeg(viewport, x - 0.67, -1.28, hoistMaterial);
            AddLeg(viewport, x + 0.67, -1.28, hoistMaterial);
            AddLeg(viewport, x - 0.67, 1.28, hoistMaterial);
            AddLeg(viewport, x + 0.67, 1.28, hoistMaterial);
            AddSideConnectionPlates(viewport, x, hoistMaterial);
            AddSideDriveAssembly(viewport, x, hoistMaterial);
        }

        private static void AddLeg(HelixViewport3D viewport, double x, double y, Material hoistMaterial)
        {
            AddBox(viewport, new Point3D(x, y, 2.12), 0.16, 0.18, 0.86, hoistMaterial);
            AddBox(viewport, new Point3D(x, y, 1.72), 0.36, 0.24, 0.12, PlantMaterialLibrary.DarkMetal);
            AddBox(viewport, new Point3D(x - 0.12, y, 1.62), 0.09, 0.18, 0.09, PlantMaterialLibrary.HoistMotor);
            AddBox(viewport, new Point3D(x + 0.12, y, 1.62), 0.09, 0.18, 0.09, PlantMaterialLibrary.HoistMotor);
            AddBox(viewport, new Point3D(x, y, 1.84), 0.24, 0.28, 0.05, hoistMaterial);

            AddBolt(viewport, x - 0.06, y - 0.1, 1.88);
            AddBolt(viewport, x + 0.06, y - 0.1, 1.88);
            AddBolt(viewport, x - 0.06, y + 0.1, 1.88);
            AddBolt(viewport, x + 0.06, y + 0.1, 1.88);
        }

        private static void AddTrolley(HelixViewport3D viewport, double x, Material hoistMaterial)
        {
            AddBox(viewport, new Point3D(x, 0, 2.18), 0.46, 0.4, 0.24, PlantMaterialLibrary.DarkMetal);
            AddBox(viewport, new Point3D(x, 0, 2.3), 0.34, 0.52, 0.08, hoistMaterial);
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(x, 0, 2.28), new Point3D(x, 0, 2.62), 0.105, PlantMaterialLibrary.DarkMetal, 22);
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(x, 0, 2.62), new Point3D(x, 0, 2.76), 0.075, PlantMaterialLibrary.Rubber, 18);
            AddLiftMotorCoolingFins(viewport, x);
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(x - 0.18, -0.18, 2.18), new Point3D(x + 0.18, -0.18, 2.18), 0.1, PlantMaterialLibrary.BrushedSteel, 20);

            AddWheelBogie(viewport, x - 0.36, -1.28);
            AddWheelBogie(viewport, x + 0.36, -1.28);
            AddWheelBogie(viewport, x - 0.36, 1.28);
            AddWheelBogie(viewport, x + 0.36, 1.28);

            AddBox(viewport, new Point3D(x - 0.16, 0, 2.0), 0.06, 0.16, 0.46, hoistMaterial);
            AddBox(viewport, new Point3D(x + 0.16, 0, 2.0), 0.06, 0.16, 0.46, hoistMaterial);
            AddBox(viewport, new Point3D(x, -0.18, 1.92), 0.46, 0.04, 0.68, PlantMaterialLibrary.DarkMetal);
            AddBox(viewport, new Point3D(x, 0.18, 1.92), 0.46, 0.04, 0.68, PlantMaterialLibrary.DarkMetal);
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(x - 0.23, -0.24, 1.72), new Point3D(x - 0.23, -0.24, 2.2), 0.018, PlantMaterialLibrary.ChainSteel, 10);
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(x + 0.23, -0.24, 1.72), new Point3D(x + 0.23, -0.24, 2.2), 0.018, PlantMaterialLibrary.ChainSteel, 10);
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(x - 0.23, 0.24, 1.72), new Point3D(x - 0.23, 0.24, 2.2), 0.018, PlantMaterialLibrary.ChainSteel, 10);
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(x + 0.23, 0.24, 1.72), new Point3D(x + 0.23, 0.24, 2.2), 0.018, PlantMaterialLibrary.ChainSteel, 10);
        }

        private static void AddBeamBoltRow(HelixViewport3D viewport, double x, double y, double z)
        {
            for (var i = 0; i < 6; i++)
            {
                var boltX = x - 0.56 + i * 0.224;
                IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(boltX, y, z), new Point3D(boltX, y - 0.035, z), 0.018, PlantMaterialLibrary.DarkMetal, 10);
            }
        }

        private static void AddHoistNameBillboard(HelixViewport3D viewport, double x)
        {
            var label = new TextVisual3D
            {
                Text = "USR H1 500KG",
                Position = new Point3D(x - 0.48, -1.62, 2.63),
                TextDirection = new Vector3D(1, 0, 0),
                UpDirection = new Vector3D(0, 0, 1),
                Height = 0.16,
                FontSize = 13,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Black,
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Padding = new Thickness(0),
                IsDoubleSided = true
            };

            viewport.Children.Add(label);
        }

        private static void AddSideDriveAssembly(HelixViewport3D viewport, double x, Material hoistMaterial)
        {
            const double frontY = -1.58;
            const double z = 2.58;

            AddBox(viewport, new Point3D(x + 0.76, frontY + 0.015, z), 0.18, 0.22, 0.42, hoistMaterial);
            AddBox(viewport, new Point3D(x + 0.86, frontY - 0.005, z), 0.26, 0.2, 0.34, PlantMaterialLibrary.HoistMotor);
            AddBox(viewport, new Point3D(x + 0.82, frontY + 0.08, z - 0.25), 0.42, 0.16, 0.1, hoistMaterial);
            AddBox(viewport, new Point3D(x + 0.7, frontY + 0.055, z - 0.08), 0.22, 0.11, 0.14, hoistMaterial);
            AddBox(viewport, new Point3D(x + 0.94, frontY + 0.055, z + 0.19), 0.34, 0.08, 0.08, hoistMaterial);
            AddBox(viewport, new Point3D(x + 0.94, frontY + 0.055, z - 0.19), 0.34, 0.08, 0.08, hoistMaterial);

            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(x + 0.58, frontY - 0.04, z), new Point3D(x + 0.76, frontY - 0.04, z), 0.045, PlantMaterialLibrary.DarkMetal, 16);
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(x + 0.96, frontY - 0.04, z), new Point3D(x + 1.03, frontY - 0.04, z), 0.055, PlantMaterialLibrary.DarkMetal, 18);

            AddFinnedMotor(viewport, x + 1.03, x + 1.28, frontY - 0.04, z);
            AddGearFace(viewport, x + 0.86, frontY - 0.125, z);

            AddBolt(viewport, x + 0.76, frontY - 0.12, z + 0.15);
            AddBolt(viewport, x + 0.94, frontY - 0.12, z + 0.15);
            AddBolt(viewport, x + 0.76, frontY - 0.12, z - 0.15);
            AddBolt(viewport, x + 0.94, frontY - 0.12, z - 0.15);
            AddBolt(viewport, x + 0.76, frontY - 0.12, z);
            AddBolt(viewport, x + 0.94, frontY - 0.12, z);
        }

        private static void AddLiftMotorCoolingFins(HelixViewport3D viewport, double x)
        {
            for (var i = 0; i < 5; i++)
            {
                var z = 2.32 + i * 0.055;
                IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(x, 0, z), new Point3D(x, 0, z + 0.012), 0.12, PlantMaterialLibrary.Rubber, 22);
            }
        }

        private static void AddFinnedMotor(HelixViewport3D viewport, double startX, double endX, double y, double z)
        {
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(startX, y, z), new Point3D(endX, y, z), 0.145, PlantMaterialLibrary.HoistMotor, 28);
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(startX - 0.035, y, z), new Point3D(startX + 0.02, y, z), 0.155, PlantMaterialLibrary.DarkMetal, 24);
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(endX - 0.025, y, z), new Point3D(endX + 0.04, y, z), 0.155, PlantMaterialLibrary.DarkMetal, 24);

            for (var i = 0; i < 5; i++)
            {
                var finX = startX + 0.045 + i * 0.045;
                IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(finX - 0.007, y, z), new Point3D(finX + 0.007, y, z), 0.168, PlantMaterialLibrary.DarkMetal, 24);
            }

            AddBox(viewport, new Point3D((startX + endX) / 2.0, y + 0.1, z - 0.18), 0.24, 0.1, 0.08, PlantMaterialLibrary.DarkMetal);
        }

        private static void AddGearFace(HelixViewport3D viewport, double x, double y, double z)
        {
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(x, y, z), new Point3D(x, y - 0.035, z), 0.12, PlantMaterialLibrary.DarkMetal, 24);
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(x, y - 0.04, z), new Point3D(x, y - 0.075, z), 0.055, PlantMaterialLibrary.HoistMotor, 18);
        }

        private static void AddStatusStack(HelixViewport3D viewport, double x, string status)
        {
            var y = -1.48;
            AddBox(viewport, new Point3D(x + 0.56, y + 0.018, 2.32), 0.11, 0.035, 0.3, PlantMaterialLibrary.DarkMetal);
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(x + 0.56, y - 0.02, 2.42), new Point3D(x + 0.56, y - 0.065, 2.42), 0.04, PlantMaterialLibrary.StatusFor(status), 16);
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(x + 0.56, y - 0.02, 2.32), new Point3D(x + 0.56, y - 0.065, 2.32), 0.04, PlantMaterialLibrary.StatusWarning, 16);
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(x + 0.56, y - 0.02, 2.22), new Point3D(x + 0.56, y - 0.065, 2.22), 0.04, PlantMaterialLibrary.StatusFault, 16);
        }

        private static void AddSideConnectionPlates(HelixViewport3D viewport, double x, Material hoistMaterial)
        {
            AddBox(viewport, new Point3D(x - 0.67, -1.28, 2.5), 0.3, 0.035, 0.22, hoistMaterial);
            AddBox(viewport, new Point3D(x + 0.67, -1.28, 2.5), 0.3, 0.035, 0.22, hoistMaterial);
            AddBox(viewport, new Point3D(x - 0.67, 1.28, 2.5), 0.3, 0.035, 0.22, hoistMaterial);
            AddBox(viewport, new Point3D(x + 0.67, 1.28, 2.5), 0.3, 0.035, 0.22, hoistMaterial);

            AddBolt(viewport, x - 0.77, -1.31, 2.55);
            AddBolt(viewport, x - 0.57, -1.31, 2.55);
            AddBolt(viewport, x + 0.57, -1.31, 2.55);
            AddBolt(viewport, x + 0.77, -1.31, 2.55);
            AddBolt(viewport, x - 0.77, 1.31, 2.55);
            AddBolt(viewport, x - 0.57, 1.31, 2.55);
            AddBolt(viewport, x + 0.57, 1.31, 2.55);
            AddBolt(viewport, x + 0.77, 1.31, 2.55);
        }

        private static void AddWheelBogie(HelixViewport3D viewport, double x, double y)
        {
            AddBox(viewport, new Point3D(x, y, 1.8), 0.22, 0.16, 0.1, PlantMaterialLibrary.DarkMetal);
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(x - 0.07, y - 0.065, 1.68), new Point3D(x - 0.07, y + 0.065, 1.68), 0.055, PlantMaterialLibrary.Rubber, 18);
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(x + 0.07, y - 0.065, 1.68), new Point3D(x + 0.07, y + 0.065, 1.68), 0.055, PlantMaterialLibrary.Rubber, 18);
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(x - 0.07, y - 0.072, 1.68), new Point3D(x - 0.07, y + 0.072, 1.68), 0.025, PlantMaterialLibrary.BrushedSteel, 14);
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(x + 0.07, y - 0.072, 1.68), new Point3D(x + 0.07, y + 0.072, 1.68), 0.025, PlantMaterialLibrary.BrushedSteel, 14);
        }

        private static void AddBolt(HelixViewport3D viewport, double x, double y, double z)
        {
            AddBox(viewport, new Point3D(x, y, z), 0.035, 0.018, 0.035, PlantMaterialLibrary.DarkMetal);
        }

        private static void AddBox(HelixViewport3D viewport, Point3D center, double length, double width, double height, Material material)
        {
            var box = new BoxVisual3D
            {
                Center = center,
                Length = length,
                Width = width,
                Height = height,
                Material = material,
                BackMaterial = material
            };
            viewport.Children.Add(box);
        }
    }
}
