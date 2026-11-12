using UnityEngine;

namespace TerrainConverter.Editor.Core
{
    /// <summary>
    /// Heightmap sampling step used when building a mesh.
    /// The numeric value is the step in heightmap samples between two neighbouring vertices.
    /// </summary>
    public enum MeshResolution
    {
        [InspectorName("Full (1:1)")]
        Full = 1,

        [InspectorName("Half (1:2)")]
        Half = 2,

        [InspectorName("Quarter (1:4)")]
        Quarter = 4,

        [InspectorName("Eighth (1:8)")]
        Eighth = 8
    }
}
