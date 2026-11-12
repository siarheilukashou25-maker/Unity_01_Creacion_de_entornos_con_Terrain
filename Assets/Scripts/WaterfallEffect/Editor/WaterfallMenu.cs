using System;
using WaterfallEffect.Editor.Assets;
using UnityEditor;
using UnityEngine;

namespace WaterfallEffect.Editor
{
    /// <summary>
    /// <c>GameObject → Effects → Waterfall</c>. Acts as the composition root of the editor tooling.
    /// </summary>
    internal static class WaterfallMenu
    {
        private const string MenuPath = "GameObject/Effects/Waterfall";
        private const int MenuPriority = 10;
        private const string DialogTitle = "Waterfall";

        [MenuItem(MenuPath, false, MenuPriority)]
        private static void CreateWaterfall(MenuCommand command)
        {
            var parent = command.context as GameObject;
            var factory = new WaterfallFactory(new ParticleMaterialProvider(new SoftParticleTextureGenerator()));

            try
            {
                Waterfall waterfall = factory.Create(parent != null ? parent.transform : null, GetSpawnPosition(parent));
                Selection.activeGameObject = waterfall.gameObject;
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                EditorUtility.DisplayDialog(DialogTitle, exception.Message, "OK");
            }
        }

        private static Vector3 GetSpawnPosition(GameObject parent)
        {
            if (parent != null)
            {
                return parent.transform.position;
            }

            SceneView sceneView = SceneView.lastActiveSceneView;
            return sceneView != null ? sceneView.pivot : Vector3.zero;
        }
    }
}
