using System;
using System.Collections.Generic;
using System.Linq;
using Figma.Creators;
using Figma.Objects;
using UnityEditor;
using UnityEngine;

namespace Figma
{
    public class FigmaParser : EditorWindow
    {
        private const string UiPrefabPathParam = "UiPrefabPath";
        private const string UiLayerParam = "UiLayer";
        private const string FileUrlParam = "FigmaFile";
        private const string TokenParam = "FigmaToken";

        private string _uiPrefabPath;
        private int _uiLayer;
        private string _fileURL;
        private string _token;
        private string _search = string.Empty;
        private string[] _pages;
        private int _selectedPage;
        private string[] _frames;
        private int _serachSelectedFrame;
        private int _selectedFrame;
        private LayoutPipeline _layoutPipeline;
        private ComponentList _componentList;

        private FigmaFile _parsedFile;

        private Vector2 _pagesScrollPos;
        private Vector2 _framesScrollPos;

        [MenuItem("Tools/UI/Figma")]
        public static void Init()
        {
            var window = (FigmaParser) GetWindow(typeof(FigmaParser));
            window.Show();
        }

        void OnGUI()
        {
            GUILayout.Space(10);
            EditorGUILayout.LabelField("FIGMA AUTO LAYOUT", new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 20,
                alignment = TextAnchor.MiddleCenter
            });
            GUILayout.Space(10);

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
            _uiPrefabPath = EditorGUILayout.TextField("Path to UI Prefabs", _uiPrefabPath);
            _uiLayer = EditorGUILayout.IntField("UI Layer Id", _uiLayer);
            EditorGUILayout.EndVertical();

            GUILayout.Space(8);
            
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Figma File", EditorStyles.boldLabel);
            _token = EditorGUILayout.TextField("Figma Token", _token);
            _fileURL = EditorGUILayout.TextField("File URL", _fileURL);
            EditorGUILayout.EndVertical();

            GUILayout.Space(8);

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Layout Pipeline", EditorStyles.boldLabel);
            
            if (_layoutPipeline == null) 
                EditorGUILayout.HelpBox("Please, insert layout pipeline!", MessageType.Error);
            
            _layoutPipeline = (LayoutPipeline)EditorGUILayout.ObjectField("Layout Pipeline", _layoutPipeline,
                typeof(LayoutPipeline), false);

            if (_layoutPipeline == null)
            {
                if (GUILayout.Button("+ Create Layout Pipeline"))
                {
                    _layoutPipeline = CreateAsset<LayoutPipeline>("LayoutPipeline");
                }
            }
            
            EditorGUILayout.EndVertical();

            GUILayout.Space(8);

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Component List", EditorStyles.boldLabel);
            
            if (_componentList == null)
                EditorGUILayout.HelpBox("Please, insert component list to save and find the necessary components", MessageType.Warning);
            
            _componentList = (ComponentList)EditorGUILayout.ObjectField("Component List", _componentList, typeof(ComponentList), false);
            
            if (_componentList == null)
            {
                if (GUILayout.Button("+ Create Component List"))
                {
                    _componentList = CreateAsset<ComponentList>("ComponentList");
                }
            }
            
            EditorGUILayout.EndVertical();

            GUILayout.Space(12);

            GUI.backgroundColor = Color.cyan;
            if (GUILayout.Button("Download Images", GUILayout.Height(30)))
            {
                Requester.ImageRequest(_token, _fileURL);
            }

            GUI.backgroundColor = Color.white;

            GUILayout.Space(5);

            if (_layoutPipeline != null && GUILayout.Button("Parse Figma File", GUILayout.Height(30)))
            {
                _parsedFile = Requester.FileRequest(_token, _fileURL);
                _pages = _parsedFile.document.children.Select(x => x.name).ToArray();
            }

            GUILayout.Space(10);

            if (_parsedFile != null)
            {
                GUILayout.BeginHorizontal();

                var height = Math.Max(_pages?.Length ?? 1, _frames?.Length ?? 1) * 24 + 24;

                _pagesScrollPos = EditorGUILayout.BeginScrollView(_pagesScrollPos, GUILayout.Height(height), GUILayout.MaxHeight(300));
                GUILayout.BeginVertical("box");
                GUILayout.Label("Select Page");
                var selectedPage = GUILayout.SelectionGrid(_selectedPage, _pages, 1);
                if (selectedPage != _selectedPage)
                {
                    _selectedPage = selectedPage;
                    _selectedFrame = 0;
                }
                _frames = _parsedFile.document.children[_selectedPage].children.Select(x => x.name.Substring(0, Math.Min(20, x.name.Length))).ToArray();
                GUILayout.EndVertical();
                EditorGUILayout.EndScrollView();

                _framesScrollPos = EditorGUILayout.BeginScrollView(_framesScrollPos, GUILayout.Height(height), GUILayout.MaxHeight(300));
                GUILayout.BeginVertical("box");
                GUILayout.Label("Select Frame");
                _search = EditorGUILayout.TextField("Search", _search);
                var searchResult = new List<(string, int)>();
                for (var i = 0; i < _frames.Length; i++)
                {
                    var frame = _frames[i];
                    if (frame.ToLower().Contains(_search.ToLower()))
                    {
                        searchResult.Add((frame, i));
                    }
                }

                _frames.Where(x => x.ToLower().Contains(_search.ToLower()));
                _serachSelectedFrame = GUILayout.SelectionGrid(_serachSelectedFrame, searchResult.Select(x => x.Item1).ToArray(), 1);
                _selectedFrame = _serachSelectedFrame < searchResult.Count ? searchResult[_serachSelectedFrame].Item2 : 0;
                GUILayout.EndVertical();
                EditorGUILayout.EndScrollView();

                GUILayout.EndHorizontal();

                if (_pages != null && _pages.Length > 0 && _frames.Length > 0)
                {
                    if (GUILayout.Button("Create"))
                        CreatePrefab();
                }
            }
        }
        
        private T CreateAsset<T>(string defaultName) where T : ScriptableObject
        {
            var asset = CreateInstance<T>();
            var path = EditorUtility.SaveFilePanelInProject("Save " + typeof(T).Name, defaultName, "asset", "Please enter a file name to save the asset to");
            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = asset;
            }

            return asset;
        }

        private void CreatePrefab()
        {
            var prefabCreator = new PrefabCreator(_layoutPipeline.Creators, _uiLayer, _uiPrefabPath, _componentList);
            prefabCreator.CreatePrefabFromFigmaFile(_parsedFile, _selectedPage, _selectedFrame);
        }

        private void OnFocus()
        {
            if (EditorPrefs.HasKey(UiPrefabPathParam))
                _uiPrefabPath = EditorPrefs.GetString(UiPrefabPathParam);

            if (EditorPrefs.HasKey(UiLayerParam))
                _uiLayer = EditorPrefs.GetInt(UiLayerParam);

            if (EditorPrefs.HasKey(TokenParam))
                _token = EditorPrefs.GetString(TokenParam);

            if (EditorPrefs.HasKey(FileUrlParam))
                _fileURL = EditorPrefs.GetString(FileUrlParam);
        }

        private void OnLostFocus()
        {
            SavePrefs();
        }

        private void OnDestroy()
        {
            SavePrefs();
        }

        private void SavePrefs()
        {
            EditorPrefs.SetString(UiPrefabPathParam, _uiPrefabPath);
            EditorPrefs.SetInt(UiLayerParam, _uiLayer);
            EditorPrefs.SetString(TokenParam, _token);
            EditorPrefs.SetString(FileUrlParam, _fileURL);
        }
    }
}
