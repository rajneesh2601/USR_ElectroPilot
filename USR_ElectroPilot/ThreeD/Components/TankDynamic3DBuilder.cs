using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using USR_ElectroPilot.Models;
using USR_ElectroPilot.ThreeD.Materials;

namespace USR_ElectroPilot.ThreeD.Components
{
    public static class TankDynamic3DBuilder
    {
        public static void AddTankState(HelixViewport3D viewport, TankModel tank, double x, bool selected, EquipmentOperatingState motorState, IList<Visual3D> visuals)
        {
            if (viewport == null)
            {
                throw new ArgumentNullException("viewport");
            }

            var level = GetLevelPercent(tank);
            var liquidZ = 0.15 + (level * 0.78);
            var liquid = new BoxVisual3D
            {
                Center = new Point3D(x, 0, liquidZ),
                Length = 0.96,
                Width = 1.65,
                Height = 0.035,
                Material = GetLiquidMaterial(tank),
                BackMaterial = GetLiquidMaterial(tank)
            };
            Add(viewport, visuals, liquid);

            var lamp = new SphereVisual3D
            {
                Center = new Point3D(x - 0.43, -1.09, 0.76),
                Radius = 0.052,
                ThetaDiv = 20,
                PhiDiv = 12,
                Material = PlantMaterialLibrary.StatusFor(tank == null ? null : tank.Status),
                BackMaterial = PlantMaterialLibrary.StatusFor(tank == null ? null : tank.Status)
            };
            Add(viewport, visuals, lamp);

            var shortLabel = GetShortTankLabel(tank).ToUpperInvariant();
            var label = new BillboardTextVisual3D
            {
                Text = "T" + Math.Max(0, tank == null ? 0 : tank.TankNo).ToString("00") + "\n" + shortLabel,
                Position = new Point3D(x + 0.06, -1.16, 0.86),
                Width = 0.56,
                Height = 0.24,
                FontSize = 13,
                FontWeight = FontWeights.Bold,
                Foreground = selected ? Brushes.DeepSkyBlue : Brushes.White,
                Background = new SolidColorBrush(Color.FromArgb(235, 7, 28, 38)),
                BorderBrush = selected ? Brushes.DeepSkyBlue : Brushes.LightGray,
                BorderThickness = new Thickness(1),
                Padding = new Thickness(2)
            };
            Add(viewport, visuals, label);

            AddMotorState(viewport, x, motorState, visuals);

            if (selected)
            {
                AddSelectionRim(viewport, x, visuals);
            }
        }

        private static void AddMotorState(HelixViewport3D viewport, double x, EquipmentOperatingState state, IList<Visual3D> visuals)
        {
            var material = GetEquipmentStatusMaterial(state);
            var lamp = new SphereVisual3D
            {
                Center = new Point3D(x + 0.38, -1.43, 0.34),
                Radius = 0.055,
                ThetaDiv = 20,
                PhiDiv = 12,
                Material = material,
                BackMaterial = material
            };
            Add(viewport, visuals, lamp);

            var label = new BillboardTextVisual3D
            {
                Text = "M " + GetEquipmentStatusText(state),
                Position = new Point3D(x + 0.38, -1.49, 0.48),
                Width = 0.36,
                Height = 0.13,
                FontSize = 10,
                FontWeight = FontWeights.Bold,
                Foreground = GetEquipmentStatusBrush(state),
                Background = new SolidColorBrush(Color.FromArgb(225, 5, 20, 28)),
                BorderBrush = GetEquipmentStatusBrush(state),
                BorderThickness = new Thickness(1),
                Padding = new Thickness(1)
            };
            Add(viewport, visuals, label);
        }

        private static Material GetEquipmentStatusMaterial(EquipmentOperatingState state)
        {
            if (state == EquipmentOperatingState.Running) return PlantMaterialLibrary.StatusRunning;
            if (state == EquipmentOperatingState.Fault) return PlantMaterialLibrary.StatusFault;
            if (state == EquipmentOperatingState.Warning || state == EquipmentOperatingState.Local) return PlantMaterialLibrary.StatusWarning;
            return PlantMaterialLibrary.DarkMetal;
        }

        private static Brush GetEquipmentStatusBrush(EquipmentOperatingState state)
        {
            if (state == EquipmentOperatingState.Running) return Brushes.LimeGreen;
            if (state == EquipmentOperatingState.Fault) return Brushes.Tomato;
            if (state == EquipmentOperatingState.Warning || state == EquipmentOperatingState.Local) return Brushes.Gold;
            return Brushes.LightGray;
        }

        private static string GetEquipmentStatusText(EquipmentOperatingState state)
        {
            if (state == EquipmentOperatingState.Running) return "RUN";
            if (state == EquipmentOperatingState.Stopped) return "STOP";
            if (state == EquipmentOperatingState.Fault) return "FAULT";
            if (state == EquipmentOperatingState.Warning) return "WARN";
            if (state == EquipmentOperatingState.Local) return "LOCAL";
            if (state == EquipmentOperatingState.Stale) return "STALE";
            if (state == EquipmentOperatingState.BadData) return "BAD";
            return "UNKNOWN";
        }

        private static void AddSelectionRim(HelixViewport3D viewport, double x, IList<Visual3D> visuals)
        {
            AddBox(viewport, visuals, new Point3D(x, -0.99, 1.15), 1.30, 0.045, 0.035);
            AddBox(viewport, visuals, new Point3D(x, 0.99, 1.15), 1.30, 0.045, 0.035);
            AddBox(viewport, visuals, new Point3D(x - 0.64, 0, 1.15), 0.045, 2.0, 0.035);
            AddBox(viewport, visuals, new Point3D(x + 0.64, 0, 1.15), 0.045, 2.0, 0.035);
        }

        private static void AddBox(HelixViewport3D viewport, IList<Visual3D> visuals, Point3D center, double length, double width, double height)
        {
            var box = new BoxVisual3D
            {
                Center = center,
                Length = length,
                Width = width,
                Height = height,
                Material = PlantMaterialLibrary.TankShellSelected,
                BackMaterial = PlantMaterialLibrary.TankShellSelected
            };
            Add(viewport, visuals, box);
        }

        private static void Add(HelixViewport3D viewport, IList<Visual3D> visuals, Visual3D visual)
        {
            viewport.Children.Add(visual);
            if (visuals != null)
            {
                visuals.Add(visual);
            }
        }

        private static Material GetLiquidMaterial(TankModel tank)
        {
            var chemical = tank == null ? string.Empty : tank.ChemicalName ?? string.Empty;
            if (chemical.IndexOf("acid", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return PlantMaterialLibrary.LiquidGreen;
            }

            if (chemical.IndexOf("copper", StringComparison.OrdinalIgnoreCase) >= 0 ||
                chemical.IndexOf("plate", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return PlantMaterialLibrary.LiquidBrown;
            }

            return PlantMaterialLibrary.LiquidBlue;
        }

        private static double GetLevelPercent(TankModel tank)
        {
            if (tank == null || tank.CapacityLiters <= 0)
            {
                return 0.65;
            }

            return Math.Max(0.05, Math.Min(0.92, tank.CurrentLevelLiters / tank.CapacityLiters));
        }

        private static string GetShortTankLabel(TankModel tank)
        {
            var source = tank == null ? string.Empty : tank.ChemicalName;
            if (string.IsNullOrWhiteSpace(source))
            {
                source = tank == null ? string.Empty : tank.Name;
            }

            if (string.IsNullOrWhiteSpace(source)) return "Process";
            var value = source.Trim();
            if (value.IndexOf("Loading", StringComparison.OrdinalIgnoreCase) >= 0) return "Load";
            if (value.IndexOf("Cleaning", StringComparison.OrdinalIgnoreCase) >= 0) return "Clean";
            if (value.IndexOf("Rinse", StringComparison.OrdinalIgnoreCase) >= 0) return "Rinse";
            if (value.IndexOf("Acid", StringComparison.OrdinalIgnoreCase) >= 0) return "Acid";
            if (value.IndexOf("Electro", StringComparison.OrdinalIgnoreCase) >= 0 || value.IndexOf("Copper", StringComparison.OrdinalIgnoreCase) >= 0) return "Plate";
            if (value.IndexOf("Dry", StringComparison.OrdinalIgnoreCase) >= 0) return "Dry";
            if (value.IndexOf("Unload", StringComparison.OrdinalIgnoreCase) >= 0) return "Unload";
            if (value.IndexOf("Process", StringComparison.OrdinalIgnoreCase) >= 0) return "Process";
            return value.Length <= 7 ? value : value.Substring(0, 7);
        }
    }
}
