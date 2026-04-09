using System.Linq;
using Figma.Objects;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Figma
{
    public partial class FigmaAutoLayoutWindow
    {
        private TextField _prefabFolderField;
        private Button _btnCreate;
        private PopupField<string> _pipelineDropdown;
        private string _selectedPipelineId;

        private FigmaLayoutPipelineProfile SelectedPipeline => _settings.GetPipeline(_selectedPipelineId);

        private void SetupPrefabs()
        {
            _prefabFolderField = rootVisualElement.Q<TextField>("prefab-folder-path");
            var btnBrowse = rootVisualElement.Q<Button>("btn-browse-folder");
            _btnCreate = rootVisualElement.Q<Button>("btn-create");

            _prefabFolderField.value = _settings.PrefabFolderPath;

            btnBrowse.clicked += () =>
            {
                var abs = EditorUtility.OpenFolderPanel("Select Prefabs Folder", _prefabFolderField.value, "");

                if (!string.IsNullOrEmpty(abs) && abs.StartsWith(Application.dataPath))
                    _prefabFolderField.value = "Assets" + abs.Substring(Application.dataPath.Length);
            };

            SetupPipelineDropdown();
            _btnCreate.clicked += CreatePrefab;
        }

        private void RefreshPrefabsUI()
        {
            if (_prefabFolderField != null && string.IsNullOrEmpty(_prefabFolderField.value))
                _prefabFolderField.value = _settings.PrefabFolderPath;
            
            SetupPipelineDropdown();
        }

        private void SetupPipelineDropdown()
        {
            var container = rootVisualElement.Q<VisualElement>("pipeline-dropdown-container");
            container.Clear();
            
            var ids = _settings.PipelineIds.ToList();

            if (ids.Count == 0)
            {
                var row = new VisualElement();
                row.AddToClassList("pipeline-error");

                var icon = new Image { image = EditorGUIUtility.IconContent("console.erroricon.sml").image };
                icon.AddToClassList("pipeline-error__icon");
                row.Add(icon);

                var label = new Label("No pipelines found. Add at least one in FigmaAutoLayoutSettings.");
                label.AddToClassList("pipeline-error__label");
                row.Add(label);

                container.Add(row);
                return;
            }

            var index = string.IsNullOrEmpty(_selectedPipelineId) ? 0 : ids.IndexOf(_selectedPipelineId);
            if (index < 0) index = 0;
            _selectedPipelineId = ids[index];

            _pipelineDropdown = new PopupField<string>("Pipeline", ids, index);
            _pipelineDropdown.RegisterValueChangedCallback(evt => _selectedPipelineId = evt.newValue);
            
            container.Add(_pipelineDropdown);
        }

        private void UpdateCreateButtonState()
        {
            var node = SelectedFigmaObject;
            var canExport = node != null && (node.type & FigmaObjectType.EXPORTABLE) != FigmaObjectType.NONE;
            _btnCreate?.SetEnabled(canExport);
        }

        private void CreatePrefab()
        {
            if (_parsedFile == null || _settings == null)
                return;

            var pipeline = SelectedPipeline;
            if (pipeline == null)
            {
                Debug.LogWarning("[FigmaAutoLayout] No pipeline selected.");
                return;
            }

            _prefabExporter.SetPipeline(pipeline);
            _prefabExporter.Export(_parsedFile, SelectedFigmaObject);

            SaveSettings(); 
        }
    }
}
