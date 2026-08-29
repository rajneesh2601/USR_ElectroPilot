using System;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;

namespace USR_ElectroPilot.ThreeD.Components
{
    public static class IndustrialShape3DBuilder
    {
        public static void AddCylinder(HelixViewport3D viewport, Point3D start, Point3D end, double radius, Material material)
        {
            AddCylinder(viewport, start, end, radius, material, 18);
        }

        public static void AddCylinder(HelixViewport3D viewport, Point3D start, Point3D end, double radius, Material material, int segments)
        {
            if (viewport == null)
            {
                throw new ArgumentNullException("viewport");
            }

            if (radius <= 0)
            {
                return;
            }

            var axis = end - start;
            if (axis.Length <= 0.0001)
            {
                return;
            }

            segments = Math.Max(8, segments);
            axis.Normalize();

            var reference = Math.Abs(Vector3D.DotProduct(axis, new Vector3D(0, 0, 1))) > 0.92
                ? new Vector3D(0, 1, 0)
                : new Vector3D(0, 0, 1);
            var right = Vector3D.CrossProduct(axis, reference);
            right.Normalize();
            var up = Vector3D.CrossProduct(right, axis);
            up.Normalize();

            var mesh = new MeshGeometry3D();
            for (var i = 0; i < segments; i++)
            {
                var angle = (Math.PI * 2.0 * i) / segments;
                var radial = (Math.Cos(angle) * right) + (Math.Sin(angle) * up);
                mesh.Positions.Add(start + radial * radius);
                mesh.Positions.Add(end + radial * radius);
            }

            for (var i = 0; i < segments; i++)
            {
                var next = (i + 1) % segments;
                var startA = i * 2;
                var endA = startA + 1;
                var startB = next * 2;
                var endB = startB + 1;

                mesh.TriangleIndices.Add(startA);
                mesh.TriangleIndices.Add(endA);
                mesh.TriangleIndices.Add(endB);

                mesh.TriangleIndices.Add(startA);
                mesh.TriangleIndices.Add(endB);
                mesh.TriangleIndices.Add(startB);
            }

            var startCenter = mesh.Positions.Count;
            mesh.Positions.Add(start);
            var endCenter = mesh.Positions.Count;
            mesh.Positions.Add(end);

            for (var i = 0; i < segments; i++)
            {
                var next = (i + 1) % segments;
                var startA = i * 2;
                var startB = next * 2;
                var endA = startA + 1;
                var endB = startB + 1;

                mesh.TriangleIndices.Add(startCenter);
                mesh.TriangleIndices.Add(startB);
                mesh.TriangleIndices.Add(startA);

                mesh.TriangleIndices.Add(endCenter);
                mesh.TriangleIndices.Add(endA);
                mesh.TriangleIndices.Add(endB);
            }

            viewport.Children.Add(new ModelVisual3D
            {
                Content = new GeometryModel3D(mesh, material)
                {
                    BackMaterial = material
                }
            });
        }
    }
}
