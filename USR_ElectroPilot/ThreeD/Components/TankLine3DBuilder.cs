using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using USR_ElectroPilot.Models;
using USR_ElectroPilot.ThreeD.Materials;

namespace USR_ElectroPilot.ThreeD.Components
{
    public static class TankLine3DBuilder
    {
        public static TankLine3DResult AddLine(HelixViewport3D viewport, IList<TankModel> tanks, int? selectedTankId)
        {
            if (viewport == null)
            {
                throw new ArgumentNullException("viewport");
            }

            if (tanks == null || tanks.Count == 0)
            {
                return new TankLine3DResult(0, 0, 0, 0);
            }

            const double pitch = 1.45;
            const double tankLength = 1.15;
            const double tankWidth = 1.85;
            const double tankHeight = 1.0;

            var originX = -((tanks.Count - 1) * pitch) / 2.0;
            var railLength = tanks.Count * pitch + 1.8;
            var floorLength = railLength + 0.85;

            AddFoundation(viewport, floorLength);
            AddTankSupportFrame(viewport, railLength);
            HoistRail3DBuilder.AddTravelRails(viewport, railLength);
            AddServicePlatform(viewport, railLength);
            AddAccessStairs(viewport, railLength);

            for (var i = 0; i < tanks.Count; i++)
            {
                var tank = tanks[i];
                var selected = selectedTankId.HasValue && selectedTankId.Value == tank.Id;
                Tank3DBuilder.AddTank(viewport, tank, originX + i * pitch, tankLength, tankWidth, tankHeight, selected);
            }

            AddProcessManifold(viewport, originX, pitch, tanks.Count, railLength);

            return new TankLine3DResult(originX, pitch, railLength, tanks.Count);
        }

        private static void AddFoundation(HelixViewport3D viewport, double floorLength)
        {
            AddBox(viewport, new Point3D(0, -0.1, -0.28), floorLength, 4.45, 0.05, PlantMaterialLibrary.SceneFloor);
            AddBox(viewport, new Point3D(0, -0.08, -0.245), floorLength - 0.7, 2.5, 0.012, PlantMaterialLibrary.SoftContactShadow);
            AddBox(viewport, new Point3D(0, -2.03, -0.145), floorLength - 0.9, 0.82, 0.012, PlantMaterialLibrary.SoftContactShadow);
        }

        private static void AddTankSupportFrame(HelixViewport3D viewport, double railLength)
        {
            AddBox(viewport, new Point3D(0, -0.96, -0.05), railLength - 0.5, 0.08, 0.1, PlantMaterialLibrary.WalkwayBlue);
            AddBox(viewport, new Point3D(0, 0.96, -0.05), railLength - 0.5, 0.08, 0.1, PlantMaterialLibrary.WalkwayBlue);
            AddBox(viewport, new Point3D(0, -1.2, -0.12), railLength - 0.5, 0.06, 0.08, PlantMaterialLibrary.DarkMetal);
            AddBox(viewport, new Point3D(0, 1.2, -0.12), railLength - 0.5, 0.06, 0.08, PlantMaterialLibrary.DarkMetal);

            var crossCount = Math.Max(2, Convert.ToInt32(Math.Ceiling(railLength / 1.4)));
            var startX = -railLength / 2 + 0.45;
            var step = (railLength - 0.9) / Math.Max(1, crossCount - 1);
            for (var i = 0; i < crossCount; i++)
            {
                var x = startX + i * step;
                AddBox(viewport, new Point3D(x, 0, -0.08), 0.06, 2.35, 0.08, PlantMaterialLibrary.DarkMetal);
                AddBox(viewport, new Point3D(x, -1.86, -0.02), 0.08, 0.26, 0.34, PlantMaterialLibrary.WalkwayBlue);
            }
        }

        private static void AddServicePlatform(HelixViewport3D viewport, double railLength)
        {
            AddBox(viewport, new Point3D(0, -1.85, -0.12), railLength, 0.08, 0.08, PlantMaterialLibrary.WalkwayBlue);
            AddBox(viewport, new Point3D(0, -2.05, -0.18), railLength, 0.7, 0.05, PlantMaterialLibrary.WalkwayGrating);
            AddWalkwayGrating(viewport, railLength);

            var guardRailY = -2.38;
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(-railLength / 2, guardRailY, 0.52), new Point3D(railLength / 2, guardRailY, 0.52), 0.026, PlantMaterialLibrary.WalkwayBlue, 14);
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(-railLength / 2, guardRailY, 0.28), new Point3D(railLength / 2, guardRailY, 0.28), 0.024, PlantMaterialLibrary.WalkwayBlue, 14);
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(-railLength / 2, -1.74, 0.42), new Point3D(railLength / 2, -1.74, 0.42), 0.022, PlantMaterialLibrary.WalkwayBlue, 14);

            var postCount = Math.Max(2, Convert.ToInt32(Math.Ceiling(railLength / 1.6)));
            var startX = -railLength / 2 + 0.25;
            var step = (railLength - 0.5) / Math.Max(1, postCount - 1);
            for (var i = 0; i < postCount; i++)
            {
                AddBox(viewport, new Point3D(startX + i * step, guardRailY, 0.04), 0.04, 0.04, 0.48, PlantMaterialLibrary.WalkwayBlue);
                AddBox(viewport, new Point3D(startX + i * step, -1.74, 0.12), 0.035, 0.035, 0.6, PlantMaterialLibrary.WalkwayBlue);
            }
        }

        private static void AddWalkwayGrating(HelixViewport3D viewport, double railLength)
        {
            var slatCount = Math.Max(4, Convert.ToInt32(Math.Ceiling(railLength / 0.35)));
            var startX = -railLength / 2 + 0.18;
            var step = (railLength - 0.36) / Math.Max(1, slatCount - 1);
            for (var i = 0; i < slatCount; i++)
            {
                AddBox(viewport, new Point3D(startX + i * step, -2.05, -0.13), 0.035, 0.64, 0.025, PlantMaterialLibrary.DarkMetal);
            }

            AddBox(viewport, new Point3D(0, -1.84, -0.11), railLength - 0.2, 0.03, 0.03, PlantMaterialLibrary.DarkMetal);
            AddBox(viewport, new Point3D(0, -2.26, -0.11), railLength - 0.2, 0.03, 0.03, PlantMaterialLibrary.DarkMetal);
        }

        private static void AddAccessStairs(HelixViewport3D viewport, double railLength)
        {
            var startX = -railLength / 2 + 0.35;
            var baseY = -2.72;

            AddBox(viewport, new Point3D(startX, baseY, -0.22), 0.75, 0.18, 0.05, PlantMaterialLibrary.WalkwayGrating);
            AddBox(viewport, new Point3D(startX + 0.15, baseY + 0.16, -0.14), 0.75, 0.18, 0.05, PlantMaterialLibrary.WalkwayGrating);
            AddBox(viewport, new Point3D(startX + 0.3, baseY + 0.32, -0.06), 0.75, 0.18, 0.05, PlantMaterialLibrary.WalkwayGrating);
            AddBox(viewport, new Point3D(startX + 0.45, baseY + 0.48, 0.02), 0.75, 0.18, 0.05, PlantMaterialLibrary.WalkwayGrating);
            AddBox(viewport, new Point3D(startX + 0.62, baseY + 0.62, 0.06), 0.7, 0.2, 0.05, PlantMaterialLibrary.WalkwayGrating);

            AddBox(viewport, new Point3D(startX - 0.42, baseY + 0.24, -0.02), 0.05, 0.9, 0.05, PlantMaterialLibrary.WalkwayBlue);
            AddBox(viewport, new Point3D(startX + 0.86, baseY + 0.24, -0.02), 0.05, 0.9, 0.05, PlantMaterialLibrary.WalkwayBlue);
            AddBox(viewport, new Point3D(startX - 0.42, baseY + 0.24, 0.22), 0.04, 0.9, 0.04, PlantMaterialLibrary.WalkwayBlue);
            AddBox(viewport, new Point3D(startX + 0.86, baseY + 0.24, 0.22), 0.04, 0.9, 0.04, PlantMaterialLibrary.WalkwayBlue);
            AddBox(viewport, new Point3D(startX + 0.22, baseY + 0.28, 0.4), 1.32, 0.04, 0.04, PlantMaterialLibrary.WalkwayBlue);
        }

        private static void AddProcessManifold(HelixViewport3D viewport, double originX, double pitch, int tankCount, double railLength)
        {
            const double pipeY = -1.48;
            const double pipeZ = 0.36;

            var pipeStart = -railLength / 2 + 0.5;
            var pipeEnd = railLength / 2 - 0.5;
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(pipeStart, pipeY, pipeZ), new Point3D(pipeEnd, pipeY, pipeZ), 0.04, PlantMaterialLibrary.PipeGray, 18);
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(pipeStart + 0.12, pipeY - 0.12, pipeZ - 0.12), new Point3D(pipeEnd - 0.12, pipeY - 0.12, pipeZ - 0.12), 0.026, PlantMaterialLibrary.PipeGray, 14);

            for (var i = 0; i < tankCount; i++)
            {
                var x = originX + i * pitch;
                AddTankPipeDrop(viewport, x, pipeY, pipeZ, i);
            }
        }

        private static void AddTankPipeDrop(HelixViewport3D viewport, double x, double pipeY, double pipeZ, int index)
        {
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(x, pipeY, pipeZ), new Point3D(x, pipeY + 0.43, pipeZ), 0.028, PlantMaterialLibrary.PipeGray, 14);
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(x, pipeY + 0.43, pipeZ), new Point3D(x, pipeY + 0.43, pipeZ + 0.25), 0.026, PlantMaterialLibrary.PipeGray, 14);
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(x - 0.07, pipeY + 0.43, pipeZ + 0.25), new Point3D(x + 0.07, pipeY + 0.43, pipeZ + 0.25), 0.06, PlantMaterialLibrary.PumpBodyBlue, 16);
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(x + 0.15, pipeY + 0.34, pipeZ + 0.27), new Point3D(x + 0.15, pipeY + 0.52, pipeZ + 0.27), 0.032, PlantMaterialLibrary.ValveHandle, 12);
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(x + 0.2, pipeY + 0.43, pipeZ + 0.27), new Point3D(x + 0.48, pipeY + 0.43, pipeZ + 0.27), 0.032, PlantMaterialLibrary.PipeGray, 14);
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(x + 0.48, pipeY + 0.43, pipeZ + 0.18), new Point3D(x + 0.48, pipeY + 0.43, pipeZ + 0.36), 0.085, PlantMaterialLibrary.PumpBodyBlue, 18);
            AddBox(viewport, new Point3D(x + 0.48, pipeY + 0.43, pipeZ + 0.12), 0.24, 0.22, 0.04, PlantMaterialLibrary.PipeGray);
            AddBox(viewport, new Point3D(x + 0.32, pipeY + 0.43, pipeZ + 0.27), 0.035, 0.18, 0.12, PlantMaterialLibrary.PipeGray);
            AddBox(viewport, new Point3D(x - 0.03, pipeY + 0.43, pipeZ + 0.25), 0.02, 0.16, 0.12, PlantMaterialLibrary.BrushedSteel);
            AddBox(viewport, new Point3D(x + 0.03, pipeY + 0.43, pipeZ + 0.25), 0.02, 0.16, 0.12, PlantMaterialLibrary.BrushedSteel);
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

    public sealed class TankLine3DResult
    {
        public TankLine3DResult(double originX, double pitch, double railLength, int tankCount)
        {
            OriginX = originX;
            Pitch = pitch;
            RailLength = railLength;
            TankCount = tankCount;
        }

        public double OriginX { get; private set; }
        public double Pitch { get; private set; }
        public double RailLength { get; private set; }
        public int TankCount { get; private set; }

        public double GetTankX(double positionIndex)
        {
            if (TankCount <= 0)
            {
                return 0;
            }

            var clamped = Math.Max(0, Math.Min(TankCount - 1, positionIndex));
            return OriginX + clamped * Pitch;
        }
    }
}
