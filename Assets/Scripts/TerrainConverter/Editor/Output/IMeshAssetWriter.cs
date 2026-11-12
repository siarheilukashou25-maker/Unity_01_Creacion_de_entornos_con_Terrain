using UnityEngine;

namespace TerrainConverter.Editor.Output
{
    /// <summary>
    /// Persists a generated mesh as a project asset.
    /// </summary>
    public interface IMeshAssetWriter
    {
        /// <returns>The persisted mesh asset.</returns>
        Mesh Save(Mesh mesh, string folderPath);
    }
}
