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
        private const string AddImagesParam = "AddImages"; 
        private const string AddTMPParam = "AddTMP"; 
        private const string AddLayoutGroupsParam = "AddLayoutGroups"; 
        private const string AddContentSizeFittersParam = "AddContentSizeFitters"; 
            
        private string _uiPrefabPath;
        private int _uiLayer;
        private string _fileURL;
        private string _token;
        private string[] _pages;
        private int _selectedPage; 
        private string[] _frames;
        private int _selectedFrame;
        private bool _addImages;
        private bool _addTMP; 
        private bool _addLayoutGroups;
        private bool _addContentSizeFitters;
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
            GUILayout.Label("Layout Settings");
            _addImages = EditorGUILayout.Toggle("Add Images", _addImages); 
            _addTMP = EditorGUILayout.Toggle("Add TMP", _addTMP);
            _addLayoutGroups = EditorGUILayout.Toggle("Add Layout Groups", _addLayoutGroups); 
            _addContentSizeFitters = EditorGUILayout.Toggle("Add ContentSizeFitter", _addContentSizeFitters); 
            
            EditorGUILayout.Space();
            GUILayout.Label("Component List");
            _componentList = (ComponentList) EditorGUILayout.ObjectField("Component list", _componentList, typeof(ComponentList), true);

            if (_componentList == null)
                EditorGUILayout.HelpBox("Please, insert component list to save and find the necessary components", MessageType.Warning); 
            
            if (GUILayout.Button("Download images"))
            {
                Requester.ImageRequest(_token, _fileURL);
            }
            
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
            var prefabCreator = new PrefabCreator(_addImages, _addTMP, _addLayoutGroups, _addContentSizeFitters, _uiLayer, _uiPrefabPath, _componentList); 
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
            
            if (EditorPrefs.HasKey(AddImagesParam))
                _addImages = EditorPrefs.GetBool(AddImagesParam);
            
            if (EditorPrefs.HasKey(AddTMPParam))
                _addTMP = EditorPrefs.GetBool(AddTMPParam);
            
            if (EditorPrefs.HasKey(AddLayoutGroupsParam))
                _addLayoutGroups = EditorPrefs.GetBool(AddLayoutGroupsParam);
            
            if (EditorPrefs.HasKey(AddContentSizeFittersParam))
                _addContentSizeFitters = EditorPrefs.GetBool(AddContentSizeFittersParam);
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
            EditorPrefs.SetBool(AddImagesParam, _addImages);
            EditorPrefs.SetBool(AddTMPParam, _addTMP);
            EditorPrefs.SetBool(AddLayoutGroupsParam, _addLayoutGroups);
            EditorPrefs.SetBool(AddContentSizeFittersParam, _addContentSizeFitters);
        }
    }
}
