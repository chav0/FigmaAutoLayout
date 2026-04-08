using Figma.Exporters;
using Figma.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Figma
{
    public partial class FigmaAutoLayoutWindow : EditorWindow
    {
        private FigmaAutoLayoutSettings _settings;
        private readonly FigmaTokenStorage _tokenStorage = new();
        private PrefabExporter _prefabExporter;
        private SpriteExporter _spriteExporter;

        [MenuItem("Tools/UI/Figma Auto Layout")]
        public static void ShowWindow()
        {
            var window = GetWindow<FigmaAutoLayoutWindow>();
            window.titleContent = new GUIContent("Figma Auto Layout");
            window.minSize = new Vector2(300, 320);
        }

        private void CreateGUI()
        {
            var guids = AssetDatabase.FindAssets("FigmaAutoLayoutWindow t:VisualTreeAsset");
            if (guids.Length == 0)
            {
                rootVisualElement.Add(new Label("FigmaAutoLayoutWindow.uxml not found in project")
                {
                    style = { color = Color.red, marginTop = 10, marginLeft = 10 }
                });
                return;
            }

            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                AssetDatabase.GUIDToAssetPath(guids[0]));
            visualTree.CloneTree(rootVisualElement);

            _settings = FigmaAutoLayoutSettings.GetOrCreate();
            _prefabExporter = new PrefabExporter(_settings);
            _spriteExporter = new SpriteExporter(_settings);

            SetupAuth();
            SetupHelpPanel();
            SetupImport();
            SetupSprites();
            SetupPrefabs();
        }

        private void OnFocus()
        {
            if (_settings != null && rootVisualElement?.Q("pipeline-dropdown-container") != null)
                RefreshSettingsUI();
        }

        private void OnDestroy()
        {
            EditorApplication.update -= PollValidation;
            EditorApplication.update -= PollImport;
            EditorApplication.update -= PollThumbnail;
            EditorApplication.update -= PollVariantSprites;
            
            _client?.Dispose();
        }
    }
}
