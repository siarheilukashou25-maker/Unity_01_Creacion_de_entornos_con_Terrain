using System;
using TerrainConverter.Editor.Core;
using UnityEngine;

namespace TerrainConverter.Editor.Modifiers
{
    /// <summary>
    /// Moves the pivot from the terrain corner to its horizontal centre.
    /// Height is left untouched so the base plane stays at Y = 0.
    /// </summary>
    public sealed class CenterPivotModifier : IMeshModifier
    {
        public void Apply(TerrainMeshData meshData)
        {
            if (meshData == null)
            {
                throw new ArgumentNullException(nameof(meshData));
            }

            Vector3 offset = GetPivotOffset(meshData.Size);
            Vector3[] vertices = meshData.Vertices;

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] -= offset;
            }
        }

        /// <summary>
        /// Offset from the terrain corner to the new pivot. Add it to the terrain position
        /// to place the centred mesh exactly where the terrain is.
        /// </summary>
        public static Vector3 GetPivotOffset(Vector3 terrainSize)
        {
            return new Vector3(terrainSize.x * 0.5f, 0f, terrainSize.z * 0.5f);
        }
    }
}
