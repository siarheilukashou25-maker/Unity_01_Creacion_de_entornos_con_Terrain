using System;
using UnityEngine;

namespace TerrainConverter.Editor.Core
{
    /// <summary>
    /// Immutable set of parameters for a single terrain-to-mesh conversion.
    /// </summary>
    public sealed class TerrainConversionOptions
    {
        private const string AssetsRoot = "Assets";

        public TerrainConversionOptions(
            MeshResolution resolution,
            bool flipVertical,
            bool centerPivot,
            bool addCollider,
            Material material,
            string outputFolder)
        {
            if (string.IsNullOrWhiteSpace(outputFolder))
            {
                throw new ArgumentException("Output folder must be specified.", nameof(outputFolder));
            }

            string normalizedFolder = outputFolder.Replace('\\', '/').TrimEnd('/');

            if (normalizedFolder != AssetsRoot && !normalizedFolder.StartsWith(AssetsRoot + "/", StringComparison.Ordinal))
            {
                throw new ArgumentException($"Output folder must be inside '{AssetsRoot}'.", nameof(outputFolder));
            }

            Resolution = resolution;
            FlipVertical = flipVertical;
            CenterPivot = centerPivot;
            AddCollider = addCollider;
            Material = material;
            OutputFolder = normalizedFolder;
        }

        public MeshResolution Resolution { get; }

        /// <summary>Mirror the mesh along Y so peaks point down (underside of a floating island).</summary>
        public bool FlipVertical { get; }

        /// <summary>Move the pivot from the terrain corner to the horizontal centre.</summary>
        public bool CenterPivot { get; }

        public bool AddCollider { get; }

        /// <summary>Material for the generated renderer; <c>null</c> means the render pipeline default.</summary>
        public Material Material { get; }

        public string OutputFolder { get; }
    }
}
