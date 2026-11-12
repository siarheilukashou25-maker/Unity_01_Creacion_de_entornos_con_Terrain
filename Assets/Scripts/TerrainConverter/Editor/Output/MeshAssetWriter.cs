using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TerrainConverter.Editor.Output
{
    /// <summary>
    /// Saves meshes as <c>.asset</c> files, creating the target folder hierarchy when needed
    /// and never overwriting existing assets.
    /// </summary>
    public sealed class MeshAssetWriter : IMeshAssetWriter
    {
        private const string MeshAssetExtension = ".asset";

        public Mesh Save(Mesh mesh, string folderPath)
        {
            if (mesh == null)
            {
                throw new ArgumentNullException(nameof(mesh));
            }

            if (string.IsNullOrWhiteSpace(folderPath))
            {
                throw new ArgumentException("Folder path must be specified.", nameof(folderPath));
            }

            EnsureFolderExists(folderPath);

            string assetPath = AssetDatabase.GenerateUniqueAssetPath($"{folderPath}/{mesh.name}{MeshAssetExtension}");
            AssetDatabase.CreateAsset(mesh, assetPath);
            AssetDatabase.SaveAssets();

            return mesh;
        }

        private static void EnsureFolderExists(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                return;
            }

            string parentFolder = Path.GetDirectoryName(folderPath)?.Replace('\\', '/');

            if (string.IsNullOrEmpty(parentFolder))
            {
                throw new InvalidOperationException($"Cannot create folder '{folderPath}': it has no valid parent.");
            }

            EnsureFolderExists(parentFolder);
            AssetDatabase.CreateFolder(parentFolder, Path.GetFileName(folderPath));
        }
    }
}
