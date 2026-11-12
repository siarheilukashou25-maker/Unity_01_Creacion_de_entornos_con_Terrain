using TerrainConverter.Editor.Core;
using UnityEngine;

namespace TerrainConverter.Editor.Building
{
    /// <summary>
    /// Builds raw mesh data from terrain data.
    /// </summary>
    public interface ITerrainMeshBuilder
    {
        TerrainMeshData Build(TerrainData terrainData, MeshResolution resolution);
    }
}
