using UnityEngine;

namespace TerrainConverter.Editor.Output
{
    /// <summary>
    /// Creates a scene object that renders a mesh.
    /// </summary>
    public interface IMeshObjectFactory
    {
        GameObject Create(string name, Mesh mesh, Material material, Vector3 position, bool addCollider);
    }
}
