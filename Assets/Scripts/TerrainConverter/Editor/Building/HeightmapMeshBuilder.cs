using System;
using TerrainConverter.Editor.Core;
using UnityEngine;

namespace TerrainConverter.Editor.Building
{
    /// <summary>
    /// Builds a regular grid mesh from the terrain heightmap.
    /// The result is in terrain local space: origin at the terrain corner, X/Z along width/length.
    /// </summary>
    public sealed class HeightmapMeshBuilder : ITerrainMeshBuilder
    {
        private const int MinVerticesPerSide = 2;
        private const int IndicesPerQuad = 6;

        public TerrainMeshData Build(TerrainData terrainData, MeshResolution resolution)
        {
            if (terrainData == null)
            {
                throw new ArgumentNullException(nameof(terrainData));
            }

            int heightmapResolution = terrainData.heightmapResolution;
            int lastSampleIndex = heightmapResolution - 1;
            int step = (int)resolution;
            int verticesPerSide = lastSampleIndex / step + 1;

            if (verticesPerSide < MinVerticesPerSide)
            {
                throw new InvalidOperationException(
                    $"Heightmap resolution {heightmapResolution} is too small for sampling step {step}.");
            }

            float[,] heights = terrainData.GetHeights(0, 0, heightmapResolution, heightmapResolution);
            Vector3 size = terrainData.size;

            var vertices = new Vector3[verticesPerSide * verticesPerSide];
            var uvs = new Vector2[vertices.Length];

            for (int row = 0; row < verticesPerSide; row++)
            {
                int sampleZ = Mathf.Min(row * step, lastSampleIndex);
                float normalizedZ = (float)sampleZ / lastSampleIndex;

                for (int column = 0; column < verticesPerSide; column++)
                {
                    int sampleX = Mathf.Min(column * step, lastSampleIndex);
                    float normalizedX = (float)sampleX / lastSampleIndex;
                    int vertexIndex = row * verticesPerSide + column;

                    // TerrainData.GetHeights is indexed as [z, x] and returns normalized [0..1] heights.
                    vertices[vertexIndex] = new Vector3(
                        normalizedX * size.x,
                        heights[sampleZ, sampleX] * size.y,
                        normalizedZ * size.z);

                    uvs[vertexIndex] = new Vector2(normalizedX, normalizedZ);
                }
            }

            int[] triangles = BuildGridTriangles(verticesPerSide);

            return new TerrainMeshData(vertices, uvs, triangles, size);
        }

        private static int[] BuildGridTriangles(int verticesPerSide)
        {
            int quadsPerSide = verticesPerSide - 1;
            var triangles = new int[quadsPerSide * quadsPerSide * IndicesPerQuad];
            int triangleIndex = 0;

            for (int row = 0; row < quadsPerSide; row++)
            {
                for (int column = 0; column < quadsPerSide; column++)
                {
                    int bottomLeft = row * verticesPerSide + column;
                    int bottomRight = bottomLeft + 1;
                    int topLeft = bottomLeft + verticesPerSide;
                    int topRight = topLeft + 1;

                    // Clockwise winding when viewed from above (+Y), which Unity treats as front-facing.
                    triangles[triangleIndex++] = bottomLeft;
                    triangles[triangleIndex++] = topLeft;
                    triangles[triangleIndex++] = bottomRight;

                    triangles[triangleIndex++] = bottomRight;
                    triangles[triangleIndex++] = topLeft;
                    triangles[triangleIndex++] = topRight;
                }
            }

            return triangles;
        }
    }
}
