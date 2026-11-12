using System;
using WaterfallEffect.Editor.Assets;
using UnityEditor;
using UnityEngine;

namespace WaterfallEffect.Editor
{
    /// <summary>
    /// Builds the waterfall hierarchy: a root with <see cref="Waterfall"/> and one child per particle layer.
    /// </summary>
    public sealed class WaterfallFactory
    {
        private const string RootName = "Waterfall";
        private const string StreamName = "Stream";
        private const string MistName = "Mist";
        private const string SplashName = "Splash";
        private const string UndoName = "Create Waterfall";

        private readonly IParticleMaterialProvider _materialProvider;

        public WaterfallFactory(IParticleMaterialProvider materialProvider)
        {
            _materialProvider = materialProvider ?? throw new ArgumentNullException(nameof(materialProvider));
        }

        public Waterfall Create(Transform parent, Vector3 worldPosition)
        {
            // Resolve assets first: if this fails, no half-built objects are left in the scene.
            Material material = _materialProvider.GetSoftParticleMaterial();

            var root = new GameObject(RootName);

            if (parent != null)
            {
                root.transform.SetParent(parent, false);
                GameObjectUtility.EnsureUniqueNameForSibling(root);
            }

            root.transform.position = worldPosition;

            ParticleSystem stream = CreateLayerObject(root.transform, StreamName, material);
            ParticleSystem mist = CreateLayerObject(root.transform, MistName, material);
            ParticleSystem splash = CreateLayerObject(root.transform, SplashName, material);

            var waterfall = root.AddComponent<Waterfall>();
            waterfall.Initialize(stream, mist, splash);

            Undo.RegisterCreatedObjectUndo(root, UndoName);

            return waterfall;
        }

        private static ParticleSystem CreateLayerObject(Transform parent, string name, Material material)
        {
            var layerObject = new GameObject(name);
            layerObject.transform.SetParent(parent, false);

            var system = layerObject.AddComponent<ParticleSystem>();
            layerObject.GetComponent<ParticleSystemRenderer>().sharedMaterial = material;

            return system;
        }
    }
}
