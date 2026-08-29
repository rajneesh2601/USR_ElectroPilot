using System;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using USR_ElectroPilot.ThreeD.Materials;

namespace USR_ElectroPilot.ThreeD.Components
{
    public static class HoistRail3DBuilder
    {
        private const double FrontRailY = -1.28;
        private const double RearRailY = 1.28;
        private const double RailZ = 1.72;

        public static void AddTravelRails(HelixViewport3D viewport, double railLength)
        {
            if (viewport == null)
            {
                throw new ArgumentNullException("viewport");
            }

            AddRail(viewport, FrontRailY, railLength);
            AddRail(viewport, RearRailY, railLength);
            AddRailPosts(viewport, railLength);
        }

        private static void AddRail(HelixViewport3D viewport, double y, double railLength)
        {
            AddBox(viewport, new Point3D(0, y, RailZ), railLength, 0.08, 0.08, PlantMaterialLibrary.RailBlue);
        }

        private static void AddRailPosts(HelixViewport3D viewport, double railLength)
        {
            var postCount = Math.Max(2, Convert.ToInt32(Math.Ceiling(railLength / 2.5)));
            var startX = -railLength / 2 + 0.4;
            var step = (railLength - 0.8) / Math.Max(1, postCount - 1);

            for (var i = 0; i < postCount; i++)
            {
                var x = startX + i * step;
                AddBox(viewport, new Point3D(x, RearRailY, 0.74), 0.07, 0.07, 1.9, PlantMaterialLibrary.WalkwayBlue);
                AddBox(viewport, new Point3D(x, FrontRailY, 0.74), 0.07, 0.07, 1.9, PlantMaterialLibrary.WalkwayBlue);
            }
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
