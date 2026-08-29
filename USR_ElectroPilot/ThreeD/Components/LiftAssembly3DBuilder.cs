using System;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using USR_ElectroPilot.ThreeD.Materials;

namespace USR_ElectroPilot.ThreeD.Components
{
    public static class LiftAssembly3DBuilder
    {
        public static void AddLiftAssembly(HelixViewport3D viewport, double x, double liftZ, Material hoistMaterial)
        {
            if (viewport == null)
            {
                throw new ArgumentNullException("viewport");
            }

            AddBox(viewport, new Point3D(x, 0, liftZ), 0.32, 1.46, 0.09, hoistMaterial);
            AddBox(viewport, new Point3D(x, 0, liftZ - 0.08), 0.2, 1.34, 0.055, PlantMaterialLibrary.DarkMetal);
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(x, -0.66, liftZ - 0.13), new Point3D(x, 0.66, liftZ - 0.13), 0.036, PlantMaterialLibrary.DarkMetal, 14);
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(x, -0.6, liftZ - 0.22), new Point3D(x, 0.6, liftZ - 0.22), 0.032, PlantMaterialLibrary.CopperCarrier, 18);

            AddHook(viewport, x, -0.56, liftZ);
            AddHook(viewport, x, 0.56, liftZ);
            AddPlateRack(viewport, x, liftZ);
        }

        private static void AddHook(HelixViewport3D viewport, double x, double y, double liftZ)
        {
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(x, y, liftZ + 0.58), new Point3D(x, y, liftZ - 0.02), 0.018, PlantMaterialLibrary.DarkMetal, 12);
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(x - 0.07, y, liftZ - 0.04), new Point3D(x + 0.07, y, liftZ - 0.04), 0.018, PlantMaterialLibrary.DarkMetal, 12);
        }

        private static void AddPlateRack(HelixViewport3D viewport, double x, double liftZ)
        {
            const double rackLength = 1.36;

            var topZ = liftZ - 0.34;
            var frontY = -rackLength / 2.0;
            var rearY = rackLength / 2.0;

            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(x, -0.56, liftZ - 0.08), new Point3D(x, -0.56, topZ), 0.018, PlantMaterialLibrary.DarkMetal, 12);
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(x, 0.56, liftZ - 0.08), new Point3D(x, 0.56, topZ), 0.018, PlantMaterialLibrary.DarkMetal, 12);
            AddBox(viewport, new Point3D(x, 0, topZ - 0.05), 0.12, rackLength - 0.18, 0.09, PlantMaterialLibrary.CopperCarrier);
            AddBox(viewport, new Point3D(x - 0.18, 0, topZ - 0.06), 0.035, rackLength - 0.18, 0.055, PlantMaterialLibrary.CopperCarrier);
            AddBox(viewport, new Point3D(x + 0.18, 0, topZ - 0.06), 0.035, rackLength - 0.18, 0.055, PlantMaterialLibrary.CopperCarrier);
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(x - 0.26, frontY + 0.09, topZ - 0.04), new Point3D(x - 0.26, rearY - 0.09, topZ - 0.04), 0.014, PlantMaterialLibrary.ChainSteel, 10);
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(x + 0.26, frontY + 0.09, topZ - 0.04), new Point3D(x + 0.26, rearY - 0.09, topZ - 0.04), 0.014, PlantMaterialLibrary.ChainSteel, 10);

            for (var i = 0; i < 13; i++)
            {
                var py = frontY + 0.14 + i * ((rackLength - 0.28) / 12.0);
                AddHangingBoard(viewport, x, py, topZ);
            }
        }

        private static void AddHangingBoard(HelixViewport3D viewport, double x, double y, double topZ)
        {
            var chainTopZ = topZ - 0.04;
            var boardTopZ = topZ - 0.08;
            var boardCenterZ = boardTopZ - 0.3;

            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(x - 0.16, y, chainTopZ), new Point3D(x - 0.16, y, boardTopZ), 0.009, PlantMaterialLibrary.ChainSteel, 8);
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(x + 0.16, y, chainTopZ), new Point3D(x + 0.16, y, boardTopZ), 0.009, PlantMaterialLibrary.ChainSteel, 8);
            IndustrialShape3DBuilder.AddCylinder(viewport, new Point3D(x - 0.2, y, boardTopZ), new Point3D(x + 0.2, y, boardTopZ), 0.012, PlantMaterialLibrary.CopperCarrier, 10);

            AddBox(viewport, new Point3D(x, y, boardCenterZ), 0.44, 0.035, 0.6, PlantMaterialLibrary.PlateMetal);
            AddBox(viewport, new Point3D(x - 0.14, y - 0.02, boardCenterZ + 0.1), 0.035, 0.009, 0.26, PlantMaterialLibrary.CopperCarrier);
            AddBox(viewport, new Point3D(x + 0.14, y - 0.02, boardCenterZ + 0.1), 0.035, 0.009, 0.26, PlantMaterialLibrary.CopperCarrier);
            AddBox(viewport, new Point3D(x, y - 0.021, boardCenterZ - 0.15), 0.3, 0.009, 0.035, PlantMaterialLibrary.CopperCarrier);
            AddBox(viewport, new Point3D(x, y - 0.022, boardCenterZ + 0.02), 0.28, 0.009, 0.026, PlantMaterialLibrary.CopperCarrier);
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
