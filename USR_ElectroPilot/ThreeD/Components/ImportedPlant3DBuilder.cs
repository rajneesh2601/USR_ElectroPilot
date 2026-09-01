using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using USR_ElectroPilot.Models;
using USR_ElectroPilot.ThreeD.Materials;

namespace USR_ElectroPilot.ThreeD.Components
{
    public static class ImportedPlant3DBuilder
    {
        public const double TankPitch = 1.45;

        public static ImportedPlant3DResult AddLine(
            HelixViewport3D viewport,
            IList<TankModel> tanks,
            IDictionary<string, Model3DGroup> assets)
        {
            if (viewport == null)
            {
                throw new ArgumentNullException("viewport");
            }

            if (tanks == null || tanks.Count == 0)
            {
                return new ImportedPlant3DResult(new TankLine3DResult(0, 0, 0, 0));
            }

            var originX = -((tanks.Count - 1) * TankPitch) / 2.0;
            var railLength = tanks.Count * TankPitch + 1.8;
            var result = new ImportedPlant3DResult(new TankLine3DResult(originX, TankPitch, railLength, tanks.Count));

            AddFoundation(viewport, railLength, result);
            HoistRail3DBuilder.AddTravelRails(viewport, railLength);

            for (var i = 0; i < tanks.Count; i++)
            {
                var x = originX + i * TankPitch;
                AddAssetInstance(viewport, assets, "tank_standard", x, result);
                AddAssetInstance(viewport, assets, "pipe_manifold", x, result);
                AddAssetInstance(viewport, assets, "pump", x, result);
                AddAssetInstance(viewport, assets, "walkway", x, result);
            }

            return result;
        }

        private static void AddFoundation(HelixViewport3D viewport, double railLength, ImportedPlant3DResult result)
        {
            AddBox(viewport, result, new Point3D(0, -0.15, -0.40), railLength + 0.8, 4.8, 0.08, PlantMaterialLibrary.SceneFloor);
            AddBox(viewport, result, new Point3D(0, -0.96, -0.13), railLength - 0.5, 0.10, 0.18, PlantMaterialLibrary.WalkwayBlue);
            AddBox(viewport, result, new Point3D(0, 0.96, -0.13), railLength - 0.5, 0.10, 0.18, PlantMaterialLibrary.WalkwayBlue);
            AddBox(viewport, result, new Point3D(0, -1.22, -0.20), railLength - 0.5, 0.08, 0.12, PlantMaterialLibrary.DarkMetal);
            AddBox(viewport, result, new Point3D(0, 1.22, -0.20), railLength - 0.5, 0.08, 0.12, PlantMaterialLibrary.DarkMetal);
        }

        private static void AddAssetInstance(
            HelixViewport3D viewport,
            IDictionary<string, Model3DGroup> assets,
            string key,
            double x,
            ImportedPlant3DResult result)
        {
            Model3DGroup asset;
            if (assets == null || !assets.TryGetValue(key, out asset) || asset == null)
            {
                throw new InvalidOperationException("Missing imported plant asset: " + key);
            }

            var visual = new ModelVisual3D
            {
                Content = asset,
                Transform = new TranslateTransform3D(x, 0, 0)
            };

            viewport.Children.Add(visual);
            result.Visuals.Add(visual);
            result.Increment(key);
        }

        private static void AddBox(
            HelixViewport3D viewport,
            ImportedPlant3DResult result,
            Point3D center,
            double length,
            double width,
            double height,
            System.Windows.Media.Media3D.Material material)
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
            result.Visuals.Add(box);
        }
    }

    public sealed class ImportedPlant3DResult
    {
        private readonly Dictionary<string, int> _instanceCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        public ImportedPlant3DResult(TankLine3DResult line)
        {
            Line = line;
            Visuals = new List<Visual3D>();
        }

        public TankLine3DResult Line { get; private set; }

        public IList<Visual3D> Visuals { get; private set; }

        public int GetInstanceCount(string key)
        {
            int count;
            return !string.IsNullOrWhiteSpace(key) && _instanceCounts.TryGetValue(key, out count) ? count : 0;
        }

        internal void Increment(string key)
        {
            _instanceCounts[key] = GetInstanceCount(key) + 1;
        }
    }
}
