using System;
using System.Linq;
using Blobler.Objects;
using Figma.Creators;
using UnityEditor;
using UnityEngine;

namespace Blobler
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
        private string[] _pages;
        private int _selectedPage; 
        private string[] _frames;
        private int _selectedFrame;
        private LayoutPipeline _layoutPipeline; 
        private ComponentList _componentList; 
        
        private FigmaFile _parsedFile;
        
        private Vector2 _pagesScrollPos;
        private Vector2 _framesScrollPos;

        [MenuItem("Tools/UI/BLOBLER")]
        public static void Init()
        {
            var window = (FigmaParser) GetWindow(typeof(FigmaParser));
            window.Show();
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("BLOBLER");
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.MinHeight(10f));

            EditorGUILayout.Space();
            GUILayout.Label("Settings");
            _uiPrefabPath = EditorGUILayout.TextField("Path to UI Prefabs", _uiPrefabPath);
            _uiLayer = EditorGUILayout.IntField("UI Layer Id", _uiLayer);
            
            EditorGUILayout.Space();
            GUILayout.Label("File");
            _token = EditorGUILayout.TextField("Figma Token", _token);
            _fileURL = EditorGUILayout.TextField("File URL", _fileURL);
            
            EditorGUILayout.Space();
            GUILayout.Label("Layout pipeline");
            _layoutPipeline = (LayoutPipeline) EditorGUILayout.ObjectField("Layout Pipeline", _layoutPipeline, typeof(LayoutPipeline), true);

            if (_layoutPipeline == null)
	            EditorGUILayout.HelpBox("Please, insert layout pipeline!", MessageType.Error);

            EditorGUILayout.Space();
            _componentList = (ComponentList) EditorGUILayout.ObjectField("Component list", _componentList, typeof(ComponentList), true);

            if (_componentList == null)
                EditorGUILayout.HelpBox("Please, insert component list to save and find the necessary components", MessageType.Warning); 
            
            if (GUILayout.Button("Download images"))
            {
                Requester.ImageRequest(_token, _fileURL);
            }
            
            if(_layoutPipeline != null)
	            if (GUILayout.Button("Parse"))
	            {
		            _parsedFile = Requester.FileRequest(_token, _fileURL);
		            _pages = _parsedFile.document.children.Select(x => x.name).ToArray();
	            }

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
                _frames = _parsedFile.document.children[_selectedPage].children.Select(x => x.name).ToArray();
                GUILayout.EndVertical();
                EditorGUILayout.EndScrollView();

                _framesScrollPos = EditorGUILayout.BeginScrollView(_framesScrollPos, GUILayout.Height(height), GUILayout.MaxHeight(300));
                GUILayout.BeginVertical("box");
                GUILayout.Label("Select Frame"); 
                _selectedFrame = GUILayout.SelectionGrid(_selectedFrame, _frames, 1);
                GUILayout.EndVertical();
                EditorGUILayout.EndScrollView();
                
                GUILayout.EndHorizontal();
                
                if(_pages != null && _pages.Length > 0 && _frames.Length > 0)
                {
                    if (GUILayout.Button("Create"))
                        CreatePrefab();
                }
            }

            EditorGUILayout.EndVertical();
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
