using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace TerrainConverter.Editor.Core
{
    /// <summary>
    /// Mutable intermediate mesh representation.
    /// Modifiers operate on it before a <see cref="Mesh"/> is created, so the expensive
    /// Unity mesh upload happens exactly once.
    /// </summary>
    public sealed class TerrainMeshData
    {
        private const int MaxUInt16VertexCount = ushort.MaxValue + 1;

        public TerrainMeshData(Vector3[] vertices, Vector2[] uvs, int[] triangles, Vector3 size)
        {
            Vertices = vertices ?? throw new ArgumentNullException(nameof(vertices));
            Uvs = uvs ?? throw new ArgumentNullException(nameof(uvs));
            Triangles = triangles ?? throw new ArgumentNullException(nameof(triangles));

            if (uvs.Length != vertices.Length)
            {
                throw new ArgumentException("UV count must match vertex count.", nameof(uvs));
            }

            if (triangles.Length % 3 != 0)
            {
                throw new ArgumentException("Triangle index count must be a multiple of 3.", nameof(triangles));
            }

            Size = size;
        }

        public Vector3[] Vertices { get; }

        public Vector2[] Uvs { get; }

        public int[] Triangles { get; }

        /// <summary>Size of the source terrain in local units (width, height, length).</summary>
        public Vector3 Size { get; }

        public Mesh ToMesh(string name)
        {
            var mesh = new Mesh
            {
                name = name,
                indexFormat = Vertices.Length > MaxUInt16VertexCount ? IndexFormat.UInt32 : IndexFormat.UInt16
            };

            mesh.SetVertices(Vertices);
            mesh.SetUVs(0, Uvs);
            mesh.SetTriangles(Triangles, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();

            return mesh;
        }
    }
}
