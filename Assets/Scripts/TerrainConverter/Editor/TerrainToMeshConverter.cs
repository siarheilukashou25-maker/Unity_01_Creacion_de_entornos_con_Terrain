using System;
using System.Collections.Generic;
using TerrainConverter.Editor.Building;
using TerrainConverter.Editor.Core;
using TerrainConverter.Editor.Modifiers;
using TerrainConverter.Editor.Output;
using UnityEngine;

namespace TerrainConverter.Editor
{
    /// <summary>
    /// Orchestrates the conversion: build mesh data → apply modifiers → save asset → create scene object.
    /// Every step is delegated to an abstraction, so each one can be replaced or tested in isolation.
    /// </summary>
    public sealed class TerrainToMeshConverter
    {
        private const string MeshNameSuffix = "_Mesh";
        private const string FlippedNameSuffix = "_Flipped";

        private readonly ITerrainMeshBuilder _meshBuilder;
        private readonly IMeshAssetWriter _assetWriter;
        private readonly IMeshObjectFactory _objectFactory;

        public TerrainToMeshConverter(
            ITerrainMeshBuilder meshBuilder,
            IMeshAssetWriter assetWriter,
            IMeshObjectFactory objectFactory)
        {
            _meshBuilder = meshBuilder ?? throw new ArgumentNullException(nameof(meshBuilder));
            _assetWriter = assetWriter ?? throw new ArgumentNullException(nameof(assetWriter));
            _objectFactory = objectFactory ?? throw new ArgumentNullException(nameof(objectFactory));
        }

        /// <summary>Composition root with the default implementations.</summary>
        public static TerrainToMeshConverter CreateDefault()
        {
            return new TerrainToMeshConverter(
                new HeightmapMeshBuilder(),
                new MeshAssetWriter(),
                new MeshObjectFactory());
        }

        public GameObject Convert(Terrain terrain, TerrainConversionOptions options)
        {
            if (terrain == null)
            {
                throw new ArgumentNullException(nameof(terrain));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (terrain.terrainData == null)
            {
                throw new InvalidOperationException($"Terrain '{terrain.name}' has no TerrainData assigned.");
            }

            TerrainMeshData meshData = _meshBuilder.Build(terrain.terrainData, options.Resolution);

            foreach (IMeshModifier modifier in CreateModifiers(options))
            {
                modifier.Apply(meshData);
            }

            string objectName = BuildName(terrain.name, options);
            Mesh mesh = _assetWriter.Save(meshData.ToMesh(objectName), options.OutputFolder);
            Vector3 position = terrain.transform.position + GetPivotOffset(meshData.Size, options);

            return _objectFactory.Create(objectName, mesh, options.Material, position, options.AddCollider);
        }

        private static IEnumerable<IMeshModifier> CreateModifiers(TerrainConversionOptions options)
        {
            if (options.FlipVertical)
            {
                yield return new FlipVerticalModifier();
            }

            if (options.CenterPivot)
            {
                yield return new CenterPivotModifier();
            }
        }

        private static Vector3 GetPivotOffset(Vector3 terrainSize, TerrainConversionOptions options)
        {
            return options.CenterPivot ? CenterPivotModifier.GetPivotOffset(terrainSize) : Vector3.zero;
        }

        private static string BuildName(string terrainName, TerrainConversionOptions options)
        {
            return options.FlipVertical
                ? terrainName + MeshNameSuffix + FlippedNameSuffix
                : terrainName + MeshNameSuffix;
        }
    }
}
