using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace USR_ElectroPilot.ThreeD.RealModels
{
    public sealed class RealModelAssetManifest
    {
        public const string DefaultRelativeFolder = @"Assets\Models";

        private static readonly string[] SupportedExtensions = { ".obj", ".stl", ".fbx", ".dae", ".glb", ".gltf" };

        public IReadOnlyList<RealModelPart> Parts { get; private set; }

        public RealModelAssetManifest()
        {
            Parts = CreateDefaultParts();
        }

        public static IReadOnlyList<RealModelPart> CreateDefaultParts()
        {
            return new[]
            {
                new RealModelPart("hoist_h1", "Hoist H1 portal body", true, SupportedExtensions),
                new RealModelPart("motor_gearbox", "Hoist motor and gearbox", true, SupportedExtensions),
                new RealModelPart("plate_rack", "Rack carrier", true, SupportedExtensions),
                new RealModelPart("hanging_plate", "Hanging work plate", true, SupportedExtensions),
                new RealModelPart("tank_standard", "Standard process tank", false, SupportedExtensions),
                new RealModelPart("pipe_manifold", "Pipe manifold", false, SupportedExtensions),
                new RealModelPart("pump", "Pump and valve set", false, SupportedExtensions),
                new RealModelPart("walkway", "Walkway and guard rail", false, SupportedExtensions)
            };
        }

        public IReadOnlyList<RealModelAssetStatus> FindAvailableAssets(string applicationBasePath)
        {
            var modelFolder = GetAssetFolder(applicationBasePath);

            return Parts
                .Select(part => new RealModelAssetStatus(part, FindFirstAssetPath(modelFolder, part)))
                .ToArray();
        }

        public string GetAssetFolder(string applicationBasePath)
        {
            var basePath = string.IsNullOrWhiteSpace(applicationBasePath)
                ? AppDomain.CurrentDomain.BaseDirectory
                : applicationBasePath;

            return Path.Combine(basePath, DefaultRelativeFolder);
        }

        public SharpDxProbeResult GetSharpDxProbeResult()
        {
            return new SharpDxProbeResult(
                typeof(HelixToolkit.Wpf.SharpDX.Viewport3DX).Assembly.GetName().Name,
                typeof(HelixToolkit.SharpDX.MeshGeometry3D).Assembly.GetName().Name,
                typeof(SharpDX.Result).Assembly.GetName().Name);
        }

        private static string FindFirstAssetPath(string modelFolder, RealModelPart part)
        {
            if (!Directory.Exists(modelFolder))
            {
                return null;
            }

            return part.SupportedExtensions
                .Select(extension => Path.Combine(modelFolder, part.Key + extension))
                .FirstOrDefault(File.Exists);
        }
    }

    public sealed class RealModelPart
    {
        public RealModelPart(string key, string displayName, bool isDynamic, IEnumerable<string> supportedExtensions)
        {
            Key = key;
            DisplayName = displayName;
            IsDynamic = isDynamic;
            SupportedExtensions = supportedExtensions.ToArray();
        }

        public string Key { get; private set; }

        public string DisplayName { get; private set; }

        public bool IsDynamic { get; private set; }

        public IReadOnlyList<string> SupportedExtensions { get; private set; }
    }

    public sealed class RealModelAssetStatus
    {
        public RealModelAssetStatus(RealModelPart part, string fullPath)
        {
            Part = part;
            FullPath = fullPath;
        }

        public RealModelPart Part { get; private set; }

        public string FullPath { get; private set; }

        public bool IsAvailable
        {
            get { return !string.IsNullOrWhiteSpace(FullPath); }
        }
    }

    public sealed class SharpDxProbeResult
    {
        public SharpDxProbeResult(string wpfSharpDxAssembly, string coreSharpDxAssembly, string sharpDxAssembly)
        {
            WpfSharpDxAssembly = wpfSharpDxAssembly;
            CoreSharpDxAssembly = coreSharpDxAssembly;
            SharpDxAssembly = sharpDxAssembly;
        }

        public string WpfSharpDxAssembly { get; private set; }

        public string CoreSharpDxAssembly { get; private set; }

        public string SharpDxAssembly { get; private set; }
    }
}
