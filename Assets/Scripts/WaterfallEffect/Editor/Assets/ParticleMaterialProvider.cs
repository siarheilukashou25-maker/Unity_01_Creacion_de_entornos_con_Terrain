using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace WaterfallEffect.Editor.Assets
{
    /// <summary>
    /// Creates (once) and returns an alpha-blended particle material for the active render pipeline:
    /// URP when a pipeline asset is assigned, the Built-in pipeline otherwise.
    /// </summary>
    public sealed class ParticleMaterialProvider : IParticleMaterialProvider
    {
        private const string AssetFolder = "Assets/Materials/Waterfall";
        private const string UrpMaterialName = "WaterfallParticle_URP";
        private const string BuiltInMaterialName = "WaterfallParticle_BuiltIn";
        private const string UrpShaderName = "Universal Render Pipeline/Particles/Unlit";
        private const string BuiltInShaderName = "Legacy Shaders/Particles/Alpha Blended";
        private const string TransparentKeyword = "_SURFACE_TYPE_TRANSPARENT";
        private const string RenderTypeTag = "RenderType";
        private const string TransparentTagValue = "Transparent";
        private const float UrpSurfaceTransparent = 1f;
        private const float UrpBlendAlpha = 0f;

        private static readonly int BaseMapId = Shader.PropertyToID("_BaseMap");
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int SurfaceId = Shader.PropertyToID("_Surface");
        private static readonly int BlendId = Shader.PropertyToID("_Blend");
        private static readonly int SrcBlendId = Shader.PropertyToID("_SrcBlend");
        private static readonly int DstBlendId = Shader.PropertyToID("_DstBlend");
        private static readonly int ZWriteId = Shader.PropertyToID("_ZWrite");
        private static readonly int MainTexId = Shader.PropertyToID("_MainTex");
        private static readonly int TintColorId = Shader.PropertyToID("_TintColor");

        // The legacy particle shader doubles the tint, so mid-grey means "no tint".
        private static readonly Color NeutralLegacyTint = new Color(0.5f, 0.5f, 0.5f, 0.5f);

        private readonly ISoftParticleTextureSource _textureSource;

        public ParticleMaterialProvider(ISoftParticleTextureSource textureSource)
        {
            _textureSource = textureSource ?? throw new ArgumentNullException(nameof(textureSource));
        }

        public Material GetSoftParticleMaterial()
        {
            bool usesScriptablePipeline = GraphicsSettings.currentRenderPipeline != null;
            string materialName = usesScriptablePipeline ? UrpMaterialName : BuiltInMaterialName;
            string assetPath = $"{AssetFolder}/{materialName}.mat";

            var existing = AssetDatabase.LoadAssetAtPath<Material>(assetPath);

            if (existing != null)
            {
                return existing;
            }

            Texture2D texture = _textureSource.GetTexture();
            Material material = usesScriptablePipeline
                ? CreateUrpMaterial(texture)
                : CreateBuiltInMaterial(texture);
            material.name = materialName;

            AssetFolderUtility.EnsureFolderExists(AssetFolder);
            AssetDatabase.CreateAsset(material, assetPath);
            AssetDatabase.SaveAssets();

            return material;
        }

        private static Material CreateUrpMaterial(Texture2D texture)
        {
            var material = new Material(FindShader(UrpShaderName));

            material.SetTexture(BaseMapId, texture);
            material.SetColor(BaseColorId, Color.white);
            material.SetFloat(SurfaceId, UrpSurfaceTransparent);
            material.SetFloat(BlendId, UrpBlendAlpha);
            material.SetFloat(SrcBlendId, (float)BlendMode.SrcAlpha);
            material.SetFloat(DstBlendId, (float)BlendMode.OneMinusSrcAlpha);
            material.SetFloat(ZWriteId, 0f);
            material.EnableKeyword(TransparentKeyword);
            material.SetOverrideTag(RenderTypeTag, TransparentTagValue);
            material.renderQueue = (int)RenderQueue.Transparent;

            return material;
        }

        private static Material CreateBuiltInMaterial(Texture2D texture)
        {
            var material = new Material(FindShader(BuiltInShaderName));

            material.SetTexture(MainTexId, texture);
            material.SetColor(TintColorId, NeutralLegacyTint);

            return material;
        }

        private static Shader FindShader(string shaderName)
        {
            return Shader.Find(shaderName)
                ?? throw new InvalidOperationException($"Shader '{shaderName}' was not found.");
        }
    }
}
