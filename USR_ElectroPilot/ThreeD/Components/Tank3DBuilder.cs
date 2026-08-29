using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using USR_ElectroPilot.Models;
using USR_ElectroPilot.ThreeD.Materials;

namespace USR_ElectroPilot.ThreeD.Components
{
    public static class Tank3DBuilder
    {
        public static void AddTank(HelixViewport3D viewport, TankModel tank, double x, double length, double width, double height, bool selected)
        {
            if (viewport == null)
            {
                throw new ArgumentNullException("viewport");
            }

            var wall = 0.08;
            var shell = PlantMaterialLibrary.TankShellFor(selected);
            var level = Math.Max(0.05, Math.Min(0.9, GetLevelPercent(tank)));
            var frontY = -width / 2;
            var rearY = width / 2;
            var leftX = x - length / 2;
            var rightX = x + length / 2;

            AddBox(viewport, new Point3D(x, frontY, height / 2), length, wall, height, shell);
            AddBox(viewport, new Point3D(x, rearY, height / 2), length, wall, height, shell);
            AddBox(viewport, new Point3D(leftX, 0, height / 2), wall, width, height, shell);
            AddBox(viewport, new Point3D(rightX, 0, height / 2), wall, width, height, shell);
            AddBox(viewport, new Point3D(x, 0, 0.03), length, width, 0.06, PlantMaterialLibrary.TankBase);

            AddRim(viewport, x, length, width, height, shell);
            AddBox(viewport, new Point3D(x, 0, Math.Max(0.18, level * height)), length - 0.22, width - 0.22, 0.045, PlantMaterialLibrary.Liquid);
            AddFrontPanel(viewport, tank, x, length, frontY);
            AddEquipment(viewport, tank, x, length, width, height);
            AddSupports(viewport, x, length, width);

            AddProductionTankDetails(viewport, tank, x, length, width, height);
        }

        private static void AddRim(HelixViewport3D viewport, double x, double length, double width, double height, Material shell)
        {
            var z = height + 0.045;
            AddBox(viewport, new Point3D(x, -width / 2, z), length + 0.12, 0.1, 0.09, shell);
            AddBox(viewport, new Point3D(x, width / 2, z), length + 0.12, 0.1, 0.09, shell);
            AddBox(viewport, new Point3D(x - length / 2, 0, z), 0.1, width + 0.12, 0.09, shell);
            AddBox(viewport, new Point3D(x + length / 2, 0, z), 0.1, width + 0.12, 0.09, shell);
        }

        private static void AddFrontPanel(HelixViewport3D viewport, TankModel tank, double x, double length, double frontY)
        {
            AddBox(viewport, new Point3D(x, frontY - 0.045, 0.5), 0.76, 0.035, 0.34, PlantMaterialLibrary.TankLabel);
            AddTankBillboardLabel(viewport, tank, x, frontY);

            var status = tank == null ? null : tank.Status;
            AddSphere(viewport, new Point3D(x - length / 2 + 0.16, frontY - 0.09, 0.36), 0.07, PlantMaterialLibrary.StatusFor(status));
        }

        private static void AddTankBillboardLabel(HelixViewport3D viewport, TankModel tank, double x, double frontY)
        {
            var tankNo = Math.Max(0, tank == null ? 0 : tank.TankNo);
            var label = new BillboardTextVisual3D
            {
                Text = "T" + tankNo + "\n" + GetShortTankLabel(tank),
                Position = new Point3D(x, frontY - 0.13, 0.58),
                Width = 0.44,
                Height = 0.28,
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Black,
                Background = Brushes.WhiteSmoke,
                BorderBrush = Brushes.DimGray,
                BorderThickness = new Thickness(1),
                Padding = new Thickness(1)
            };

            viewport.Children.Add(label);
        }

        private static void AddEquipment(HelixViewport3D viewport, TankModel tank, double x, double length, double width, double height)
        {
            var frontY = -width / 2;
            var rightX = x + length / 2;

            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(x - 0.09, frontY - 0.28, 0.18), new Point3D(x + 0.09, frontY - 0.28, 0.18), 0.08, PlantMaterialLibrary.PumpBodyBlue, 16);
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(rightX - 0.16, frontY - 0.22, 0.3), new Point3D(rightX - 0.16, frontY - 0.22, 0.46), 0.065, PlantMaterialLibrary.PumpBodyBlue, 16);
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(rightX - 0.16, frontY - 0.2, 0.46), new Point3D(rightX - 0.16, frontY - 0.2, 0.7), 0.026, PlantMaterialLibrary.PipeGray, 12);
        }

        private static void AddSupports(HelixViewport3D viewport, double x, double length, double width)
        {
            var footZ = -0.13;
            AddBox(viewport, new Point3D(x - length / 2 + 0.16, -width / 2 + 0.12, footZ), 0.12, 0.12, 0.22, PlantMaterialLibrary.DarkMetal);
            AddBox(viewport, new Point3D(x + length / 2 - 0.16, -width / 2 + 0.12, footZ), 0.12, 0.12, 0.22, PlantMaterialLibrary.DarkMetal);
            AddBox(viewport, new Point3D(x - length / 2 + 0.16, width / 2 - 0.12, footZ), 0.12, 0.12, 0.22, PlantMaterialLibrary.DarkMetal);
            AddBox(viewport, new Point3D(x + length / 2 - 0.16, width / 2 - 0.12, footZ), 0.12, 0.12, 0.22, PlantMaterialLibrary.DarkMetal);
        }

        private static void AddProductionTankDetails(HelixViewport3D viewport, TankModel tank, double x, double length, double width, double height)
        {
            var frontY = -width / 2;
            var rearY = width / 2;
            var leftX = x - length / 2;
            var rightX = x + length / 2;

            for (var i = 0; i < 4; i++)
            {
                var ribX = leftX + 0.22 + i * ((length - 0.44) / 3.0);
                AddBox(viewport, new Point3D(ribX, frontY - 0.065, 0.55), 0.045, 0.045, 0.78, PlantMaterialLibrary.BrushedSteel);
                AddBox(viewport, new Point3D(ribX, rearY + 0.065, 0.55), 0.045, 0.045, 0.78, PlantMaterialLibrary.BrushedSteel);
            }

            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(rightX, frontY - 0.18, 0.42), new Point3D(rightX + 0.18, frontY - 0.18, 0.42), 0.045, PlantMaterialLibrary.PipeGray, 14);
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(rightX + 0.18, frontY - 0.27, 0.42), new Point3D(rightX + 0.18, frontY - 0.09, 0.42), 0.07, PlantMaterialLibrary.ValveHandle, 16);
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(rightX + 0.3, frontY - 0.18, 0.42), new Point3D(rightX + 0.48, frontY - 0.18, 0.42), 0.036, PlantMaterialLibrary.PipeGray, 14);
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(rightX + 0.52, frontY - 0.28, 0.42), new Point3D(rightX + 0.52, frontY - 0.08, 0.42), 0.085, PlantMaterialLibrary.PumpBodyBlue, 18);
            AddBox(viewport, new Point3D(rightX + 0.18, frontY - 0.18, 0.42), 0.025, 0.22, 0.18, PlantMaterialLibrary.BrushedSteel);
            AddBox(viewport, new Point3D(rightX + 0.52, frontY - 0.18, 0.42), 0.025, 0.24, 0.2, PlantMaterialLibrary.BrushedSteel);

            AddBox(viewport, new Point3D(x, frontY - 0.08, 0.22), 0.74, 0.035, 0.08, PlantMaterialLibrary.PipeGray);
            AddBox(viewport, new Point3D(x, frontY - 0.09, 0.32), 0.68, 0.025, 0.04, PlantMaterialLibrary.CopperCarrier);
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

        private static void AddSphere(HelixViewport3D viewport, Point3D center, double radius, Material material)
        {
            var sphere = new SphereVisual3D
            {
                Center = center,
                Radius = radius,
                ThetaDiv = 18,
                PhiDiv = 10,
                Material = material,
                BackMaterial = material
            };

            viewport.Children.Add(sphere);
        }

        private static string GetShortTankLabel(TankModel tank)
        {
            var source = tank == null ? string.Empty : tank.ChemicalName;
            if (string.IsNullOrWhiteSpace(source))
            {
                source = tank == null ? string.Empty : tank.Name;
            }

            if (string.IsNullOrWhiteSpace(source))
            {
                return "Process";
            }

            var trimmed = source.Trim();
            if (trimmed.IndexOf("Loading", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "Load";
            }

            if (trimmed.IndexOf("Cleaning", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "Clean";
            }

            if (trimmed.IndexOf("Rinse", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "Rinse";
            }

            if (trimmed.IndexOf("Acid", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "Acid";
            }

            if (trimmed.IndexOf("Electro", StringComparison.OrdinalIgnoreCase) >= 0 || trimmed.IndexOf("Copper", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "Plate";
            }

            if (trimmed.IndexOf("Dry", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "Dry";
            }

            if (trimmed.IndexOf("Unload", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "Unload";
            }

            if (trimmed.IndexOf("Process", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "Process";
            }

            return trimmed.Length <= 6 ? trimmed : trimmed.Substring(0, 6);
        }

        private static double GetLevelPercent(TankModel tank)
        {
            if (tank == null || tank.CapacityLiters <= 0)
            {
                return 0.65;
            }

            var value = tank.CurrentLevelLiters / tank.CapacityLiters;
            return Math.Max(0, Math.Min(1, value));
        }
    }
}
