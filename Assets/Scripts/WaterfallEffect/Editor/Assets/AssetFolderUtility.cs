using System;
using System.IO;
using UnityEditor;

namespace WaterfallEffect.Editor.Assets
{
    internal static class AssetFolderUtility
    {
        public static void EnsureFolderExists(string folderPath)
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

        public static string GetFolder(string assetPath)
        {
            return Path.GetDirectoryName(assetPath)?.Replace('\\', '/')
                ?? throw new ArgumentException($"'{assetPath}' has no folder.", nameof(assetPath));
        }
    }
}
