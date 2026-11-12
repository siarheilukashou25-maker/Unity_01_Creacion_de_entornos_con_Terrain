using System;
using TerrainConverter.Editor.Core;
using UnityEditor;
using UnityEngine;

namespace TerrainConverter.Editor.UI
{
    /// <summary>
    /// Editor window that collects conversion options and runs <see cref="TerrainToMeshConverter"/>.
    /// Open via <c>Tools → Terrain Converter → Terrain To Mesh</c> or the Terrain component context menu.
    /// </summary>
    public sealed class TerrainToMeshWindow : EditorWindow
    {
        private const string WindowTitle = "Terrain To Mesh";
        private const string MenuPath = "Tools/Terrain Converter/Terrain To Mesh";
        private const string ContextMenuPath = "CONTEXT/Terrain/Convert To Mesh...";
        private const string DefaultOutputFolder = "Assets/Meshes/Terrain";
        private const string AssetsFolderName = "Assets";
        private const float ConvertButtonHeight = 30f;
        private const float BrowseButtonWidth = 28f;
        private static readonly Vector2 MinWindowSize = new Vector2(340f, 300f);

        [SerializeField] private Terrain _terrain;
        [SerializeField] private MeshResolution _resolution = MeshResolution.Half;
        [SerializeField] private bool _flipVertical = true;
        [SerializeField] private bool _centerPivot;
        [SerializeField] private bool _addCollider = true;
        [SerializeField] private Material _material;
        [SerializeField] private string _outputFolder = DefaultOutputFolder;

        private TerrainToMeshConverter _converter;

        [MenuItem(MenuPath)]
        private static void OpenFromMenu()
        {
            GameObject selected = Selection.activeGameObject;
            Open(selected != null ? selected.GetComponent<Terrain>() : null);
        }

        [MenuItem(ContextMenuPath)]
        private static void OpenFromContextMenu(MenuCommand command)
        {
            Open(command.context as Terrain);
        }

        private static void Open(Terrain terrain)
        {
            var window = GetWindow<TerrainToMeshWindow>(false, WindowTitle, true);
            window.minSize = MinWindowSize;

            if (terrain != null)
            {
                window._terrain = terrain;
            }

            window.Show();
        }

        private void OnEnable()
        {
            _converter = TerrainToMeshConverter.CreateDefault();
        }

        private void OnGUI()
        {
            DrawSourceSection();
            EditorGUILayout.Space();
            DrawGeometrySection();
            EditorGUILayout.Space();
            DrawOutputSection();
            EditorGUILayout.Space();
            DrawConvertSection();
        }

        private void DrawSourceSection()
        {
            EditorGUILayout.LabelField("Source", EditorStyles.boldLabel);
            _terrain = (Terrain)EditorGUILayout.ObjectField("Terrain", _terrain, typeof(Terrain), true);
        }

        private void DrawGeometrySection()
        {
            EditorGUILayout.LabelField("Geometry", EditorStyles.boldLabel);
            _resolution = (MeshResolution)EditorGUILayout.EnumPopup("Resolution", _resolution);
            _flipVertical = EditorGUILayout.Toggle(
                new GUIContent("Flip Vertical", "Mirror along Y: peaks point down. Use for the underside of a floating island."),
                _flipVertical);
            _centerPivot = EditorGUILayout.Toggle(
                new GUIContent("Center Pivot", "Place the pivot in the horizontal centre instead of the terrain corner."),
                _centerPivot);

            if (_terrain != null && _terrain.terrainData != null)
            {
                int vertexCount = EstimateVertexCount(_terrain.terrainData.heightmapResolution, _resolution);
                EditorGUILayout.HelpBox($"Vertices: {vertexCount:N0}", MessageType.None);
            }
        }

        private void DrawOutputSection()
        {
            EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);
            _material = (Material)EditorGUILayout.ObjectField(
                new GUIContent("Material", "Leave empty to use the render pipeline default material."),
                _material,
                typeof(Material),
                false);
            _addCollider = EditorGUILayout.Toggle("Add Mesh Collider", _addCollider);

            using (new EditorGUILayout.HorizontalScope())
            {
                _outputFolder = EditorGUILayout.TextField("Mesh Folder", _outputFolder);

                if (GUILayout.Button("...", GUILayout.Width(BrowseButtonWidth)))
                {
                    BrowseOutputFolder();
                }
            }
        }

        private void DrawConvertSection()
        {
            string validationError = Validate();

            if (validationError != null)
            {
                EditorGUILayout.HelpBox(validationError, MessageType.Warning);
            }

            using (new EditorGUI.DisabledScope(validationError != null))
            {
                if (GUILayout.Button("Convert", GUILayout.Height(ConvertButtonHeight)))
                {
                    Convert();
                }
            }
        }

        private string Validate()
        {
            if (_terrain == null)
            {
                return "Select a terrain to convert.";
            }

            if (_terrain.terrainData == null)
            {
                return "The selected terrain has no TerrainData.";
            }

            if (string.IsNullOrWhiteSpace(_outputFolder))
            {
                return "Specify a folder for the mesh asset.";
            }

            return null;
        }

        private void Convert()
        {
            try
            {
                var options = new TerrainConversionOptions(
                    _resolution,
                    _flipVertical,
                    _centerPivot,
                    _addCollider,
                    _material,
                    _outputFolder);

                GameObject result = _converter.Convert(_terrain, options);

                Selection.activeGameObject = result;
                EditorGUIUtility.PingObject(result);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                EditorUtility.DisplayDialog(WindowTitle, exception.Message, "OK");
            }
        }

        private void BrowseOutputFolder()
        {
            string absolutePath = EditorUtility.OpenFolderPanel("Mesh Folder", Application.dataPath, string.Empty);

            if (string.IsNullOrEmpty(absolutePath))
            {
                return;
            }

            string projectRelativePath = ToProjectRelativePath(absolutePath);

            if (projectRelativePath == null)
            {
                EditorUtility.DisplayDialog(WindowTitle, "The folder must be inside the project's Assets folder.", "OK");
                return;
            }

            _outputFolder = projectRelativePath;
            GUI.FocusControl(null);
        }

        private static string ToProjectRelativePath(string absolutePath)
        {
            string normalizedPath = absolutePath.Replace('\\', '/');
            string assetsPath = Application.dataPath.Replace('\\', '/');

            if (string.Equals(normalizedPath, assetsPath, StringComparison.OrdinalIgnoreCase))
            {
                return AssetsFolderName;
            }

            return normalizedPath.StartsWith(assetsPath + "/", StringComparison.OrdinalIgnoreCase)
                ? AssetsFolderName + normalizedPath.Substring(assetsPath.Length)
                : null;
        }

        private static int EstimateVertexCount(int heightmapResolution, MeshResolution resolution)
        {
            int verticesPerSide = (heightmapResolution - 1) / (int)resolution + 1;
            return verticesPerSide * verticesPerSide;
        }
    }
}
