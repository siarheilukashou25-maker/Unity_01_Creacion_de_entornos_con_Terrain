using System;
using TerrainConverter.Editor.Core;
using UnityEngine;

namespace TerrainConverter.Editor.Modifiers
{
    /// <summary>
    /// Mirrors the mesh along the Y axis so the terrain base stays at Y = 0 and peaks point down.
    /// Triangle winding is reversed so faces keep pointing outwards after the mirror.
    /// </summary>
    public sealed class FlipVerticalModifier : IMeshModifier
    {
        public void Apply(TerrainMeshData meshData)
        {
            if (meshData == null)
            {
                throw new ArgumentNullException(nameof(meshData));
            }

            MirrorVertices(meshData.Vertices);
            ReverseWinding(meshData.Triangles);
        }

        private static void MirrorVertices(Vector3[] vertices)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 vertex = vertices[i];
                vertices[i] = new Vector3(vertex.x, -vertex.y, vertex.z);
            }
        }

        private static void ReverseWinding(int[] triangles)
        {
            for (int i = 0; i < triangles.Length; i += 3)
            {
                (triangles[i + 1], triangles[i + 2]) = (triangles[i + 2], triangles[i + 1]);
            }
        }
    }
}
