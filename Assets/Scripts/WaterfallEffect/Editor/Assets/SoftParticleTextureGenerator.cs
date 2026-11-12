using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace WaterfallEffect.Editor.Assets
{
    /// <summary>
    /// Generates a radial soft particle texture once and stores it as a PNG asset,
    /// so the effect has no dependency on third-party content.
    /// </summary>
    public sealed class SoftParticleTextureGenerator : ISoftParticleTextureSource
    {
        private const string AssetPath = "Assets/Materials/Waterfall/WaterfallSoftParticle.png";
        private const int TextureSize = 128;
        private const float FalloffExponent = 2f;
        private const byte MaxChannelValue = 255;

        public Texture2D GetTexture()
        {
            var existing = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetPath);

            if (existing != null)
            {
                return existing;
            }

            AssetFolderUtility.EnsureFolderExists(AssetFolderUtility.GetFolder(AssetPath));
            File.WriteAllBytes(ToAbsolutePath(AssetPath), GeneratePng());
            AssetDatabase.ImportAsset(AssetPath, ImportAssetOptions.ForceSynchronousImport);
            ConfigureImporter();

            return AssetDatabase.LoadAssetAtPath<Texture2D>(AssetPath)
                ?? throw new InvalidOperationException($"Failed to import '{AssetPath}'.");
        }

        private static byte[] GeneratePng()
        {
            var texture = new Texture2D(TextureSize, TextureSize, TextureFormat.RGBA32, false);

            try
            {
                var pixels = new Color32[TextureSize * TextureSize];
                float halfSize = (TextureSize - 1) * 0.5f;

                for (int y = 0; y < TextureSize; y++)
                {
                    for (int x = 0; x < TextureSize; x++)
                    {
                        float distance = new Vector2((x - halfSize) / halfSize, (y - halfSize) / halfSize).magnitude;
                        float alpha = Mathf.Pow(Mathf.Clamp01(1f - distance), FalloffExponent);

                        pixels[y * TextureSize + x] = new Color32(
                            MaxChannelValue,
                            MaxChannelValue,
                            MaxChannelValue,
                            (byte)Mathf.RoundToInt(alpha * MaxChannelValue));
                    }
                }

                texture.SetPixels32(pixels);
                texture.Apply();

                return texture.EncodeToPNG();
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(texture);
            }
        }

        private static void ConfigureImporter()
        {
            if (!(AssetImporter.GetAtPath(AssetPath) is TextureImporter importer))
            {
                throw new InvalidOperationException($"'{AssetPath}' was not imported as a texture.");
            }

            importer.textureType = TextureImporterType.Default;
            importer.alphaSource = TextureImporterAlphaSource.FromInput;
            importer.alphaIsTransparency = true;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.mipmapEnabled = true;
            importer.SaveAndReimport();
        }

        private static string ToAbsolutePath(string assetPath)
        {
            string projectRoot = Directory.GetParent(Application.dataPath)?.FullName
                ?? throw new InvalidOperationException("Cannot resolve the project root folder.");

            return Path.Combine(projectRoot, assetPath);
        }
    }
}
