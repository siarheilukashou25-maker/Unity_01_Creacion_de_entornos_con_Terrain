using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace TerrainConverter.Editor.Output
{
    /// <summary>
    /// Creates an undoable scene object with <see cref="MeshFilter"/>, <see cref="MeshRenderer"/>
    /// and, optionally, a <see cref="MeshCollider"/>.
    /// </summary>
    public sealed class MeshObjectFactory : IMeshObjectFactory
    {
        private const string BuiltInDefaultMaterialPath = "Default-Diffuse.mat";
        private const string UndoName = "Create Terrain Mesh";

        public GameObject Create(string name, Mesh mesh, Material material, Vector3 position, bool addCollider)
        {
            if (mesh == null)
            {
                throw new ArgumentNullException(nameof(mesh));
            }

            var gameObject = new GameObject(name);
            gameObject.transform.position = position;

            gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
            gameObject.AddComponent<MeshRenderer>().sharedMaterial = material != null ? material : ResolveDefaultMaterial();

            if (addCollider)
            {
                gameObject.AddComponent<MeshCollider>().sharedMesh = mesh;
            }

            Undo.RegisterCreatedObjectUndo(gameObject, UndoName);

            return gameObject;
        }

        private static Material ResolveDefaultMaterial()
        {
            RenderPipelineAsset pipeline = GraphicsSettings.currentRenderPipeline;

            return pipeline != null
                ? pipeline.defaultMaterial
                : AssetDatabase.GetBuiltinExtraResource<Material>(BuiltInDefaultMaterialPath);
        }
    }
}
