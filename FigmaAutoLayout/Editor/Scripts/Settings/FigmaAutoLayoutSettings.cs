using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Figma
{
    public class FigmaAutoLayoutSettings : ScriptableObject
    {
        private const string DefaultAssetPath = "Assets/FigmaAutoLayoutSettings.asset";
        private const string SearchFilter = "t:FigmaAutoLayoutSettings";

        [Tooltip("Folder where generated UI prefabs will be saved")]
        [SerializeField] private DefaultAsset prefabFolder;

        [Tooltip("Folder where exported sprites will be saved")]
        [SerializeField] private DefaultAsset spritesFolder;

        [SerializeField] private List<FigmaLayoutPipelineProfile> pipelines = new()
        {
            new FigmaLayoutPipelineProfile { Id = "Screen" },
            new FigmaLayoutPipelineProfile { Id = "Component" }
        };

        [SerializeField] private FigmaComponentList componentList = new();

        public string PrefabFolderPath => PrefabFolder != null ? AssetDatabase.GetAssetPath(prefabFolder) : "";
        public string SpritesFolderPath => SpritesFolder != null ? AssetDatabase.GetAssetPath(spritesFolder) : "";

        public DefaultAsset PrefabFolder
        {
            get => prefabFolder;
            set => prefabFolder = value;
        }

        public DefaultAsset SpritesFolder
        {
            get => spritesFolder;
            set => spritesFolder = value;
        }

        public List<FigmaLayoutPipelineProfile> Pipelines => pipelines;

        public FigmaLayoutPipelineProfile GetPipeline(string id) =>
            pipelines.FirstOrDefault(p => p.Id == id);

        public string[] PipelineIds => pipelines.Select(p => p.Id).ToArray();

        public FigmaComponentList ComponentList => componentList;

        public static FigmaAutoLayoutSettings GetOrCreate()
        {
            var guids = AssetDatabase.FindAssets(SearchFilter);
            if (guids.Length > 0)
                return AssetDatabase.LoadAssetAtPath<FigmaAutoLayoutSettings>(AssetDatabase.GUIDToAssetPath(guids[0]));

            var settings = CreateInstance<FigmaAutoLayoutSettings>();
            
            AssetDatabase.CreateAsset(settings, DefaultAssetPath);
            AssetDatabase.SaveAssets();
            
            Debug.Log($"[FigmaAutoLayout] Created settings at {DefaultAssetPath}");
            
            return settings;
        }
    }
}
