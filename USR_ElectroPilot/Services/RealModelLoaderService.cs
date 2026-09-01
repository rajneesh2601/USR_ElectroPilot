using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using HelixToolkit.Wpf;
using USR_ElectroPilot.ThreeD.RealModels;

namespace USR_ElectroPilot.Services
{
    public sealed class RealModelLoaderService
    {
        private static readonly HashSet<string> WpfLoadableExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".obj",
            ".stl"
        };

        public RealModelLoadResult LoadWpfModel(RealModelAssetStatus assetStatus, RealModelTransform transform, Dispatcher dispatcher)
        {
            if (assetStatus == null || assetStatus.Part == null)
            {
                return RealModelLoadResult.Missing("Unknown model part.");
            }

            if (!assetStatus.IsAvailable)
            {
                return RealModelLoadResult.Missing(assetStatus.Part, assetStatus.Part.Key + " model file is not available.");
            }

            var extension = Path.GetExtension(assetStatus.FullPath);
            if (!WpfLoadableExtensions.Contains(extension))
            {
                return RealModelLoadResult.Unsupported(
                    assetStatus.Part,
                    assetStatus.Part.Key + " uses " + extension + ". Convert to OBJ/STL or add a verified importer before runtime loading.");
            }

            try
            {
                var importer = new ModelImporter();
                var importedModel = importer.Load(assetStatus.FullPath, dispatcher ?? Dispatcher.CurrentDispatcher, true);
                if (importedModel == null)
                {
                    return RealModelLoadResult.Failed(assetStatus.Part, assetStatus.Part.Key + " loaded no geometry.");
                }

                var model = importedModel.CloneCurrentValue();
                model.Transform = (transform ?? RealModelTransform.Identity).CreateTransform();
                if (model.CanFreeze)
                {
                    model.Freeze();
                }
                return RealModelLoadResult.Loaded(assetStatus.Part, assetStatus.FullPath, model);
            }
            catch (Exception ex)
            {
                return RealModelLoadResult.Failed(assetStatus.Part, assetStatus.Part.Key + " import failed: " + ex.Message);
            }
        }

        public IReadOnlyList<RealModelLoadResult> LoadAvailableWpfModels(
            IEnumerable<RealModelAssetStatus> assetStatuses,
            IDictionary<string, RealModelTransform> transforms,
            Dispatcher dispatcher)
        {
            var results = new List<RealModelLoadResult>();
            if (assetStatuses == null)
            {
                return results;
            }

            foreach (var status in assetStatuses)
            {
                RealModelTransform transform = null;
                if (status != null && status.Part != null && transforms != null)
                {
                    transforms.TryGetValue(status.Part.Key, out transform);
                }

                results.Add(LoadWpfModel(status, transform, dispatcher));
            }

            return results;
        }

        public string CreateReadinessReport(IEnumerable<RealModelLoadResult> results)
        {
            if (results == null)
            {
                return "No real model asset checks were run.";
            }

            var resultList = results.Where(r => r != null && r.Part != null).ToList();
            if (resultList.Count == 0)
            {
                return "No real model assets are configured.";
            }

            var loaded = resultList.Where(r => r.State == RealModelLoadState.Loaded).Select(r => r.Part.Key).ToArray();
            var missing = resultList.Where(r => r.State == RealModelLoadState.Missing).Select(r => r.Part.Key).ToArray();
            var unsupported = resultList.Where(r => r.State == RealModelLoadState.UnsupportedFormat).Select(r => r.Part.Key).ToArray();
            var failed = resultList.Where(r => r.State == RealModelLoadState.Failed).Select(r => r.Part.Key).ToArray();

            return "Loaded=" + JoinOrNone(loaded) +
                "; Missing=" + JoinOrNone(missing) +
                "; Unsupported=" + JoinOrNone(unsupported) +
                "; Failed=" + JoinOrNone(failed);
        }

        private static string JoinOrNone(IEnumerable<string> values)
        {
            var valueList = values == null ? new string[0] : values.ToArray();
            return valueList.Length == 0 ? "none" : string.Join(",", valueList);
        }
    }

    public sealed class RealModelTransform
    {
        public static readonly RealModelTransform Identity = new RealModelTransform();

        public RealModelTransform()
        {
            ScaleX = 1.0;
            ScaleY = 1.0;
            ScaleZ = 1.0;
        }

        public double ScaleX { get; set; }

        public double ScaleY { get; set; }

        public double ScaleZ { get; set; }

        public double RotationXDegrees { get; set; }

        public double RotationYDegrees { get; set; }

        public double RotationZDegrees { get; set; }

        public double OffsetX { get; set; }

        public double OffsetY { get; set; }

        public double OffsetZ { get; set; }

        public Transform3D CreateTransform()
        {
            var group = new Transform3DGroup();
            group.Children.Add(new ScaleTransform3D(ScaleX, ScaleY, ScaleZ));
            group.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), RotationXDegrees)));
            group.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 1, 0), RotationYDegrees)));
            group.Children.Add(new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), RotationZDegrees)));
            group.Children.Add(new TranslateTransform3D(OffsetX, OffsetY, OffsetZ));
            return group;
        }
    }

    public sealed class RealModelLoadResult
    {
        private RealModelLoadResult(RealModelLoadState state, RealModelPart part, string path, Model3DGroup model, string message)
        {
            State = state;
            Part = part;
            Path = path;
            Model = model;
            Message = message;
        }

        public RealModelLoadState State { get; private set; }

        public RealModelPart Part { get; private set; }

        public string Path { get; private set; }

        public Model3DGroup Model { get; private set; }

        public string Message { get; private set; }

        public bool ShouldUseFallback
        {
            get { return State != RealModelLoadState.Loaded; }
        }

        public static RealModelLoadResult Loaded(RealModelPart part, string path, Model3DGroup model)
        {
            return new RealModelLoadResult(RealModelLoadState.Loaded, part, path, model, "Loaded");
        }

        public static RealModelLoadResult Missing(string message)
        {
            return new RealModelLoadResult(RealModelLoadState.Missing, null, null, null, message);
        }

        public static RealModelLoadResult Missing(RealModelPart part, string message)
        {
            return new RealModelLoadResult(RealModelLoadState.Missing, part, null, null, message);
        }

        public static RealModelLoadResult Unsupported(RealModelPart part, string message)
        {
            return new RealModelLoadResult(RealModelLoadState.UnsupportedFormat, part, null, null, message);
        }

        public static RealModelLoadResult Failed(RealModelPart part, string message)
        {
            return new RealModelLoadResult(RealModelLoadState.Failed, part, null, null, message);
        }
    }

    public enum RealModelLoadState
    {
        Loaded,
        Missing,
        UnsupportedFormat,
        Failed
    }
}
