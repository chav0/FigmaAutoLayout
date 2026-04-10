using System;
using System.Linq;
using System.Threading.Tasks;
using Figma.Objects;
using Figma.Utils;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Figma
{
    public partial class FigmaAutoLayoutWindow
    {
        private TextField _spritesFolderField;
        private ObjectField _existingSpriteField;

        private void SetupSprites()
        {
            _spritesFolderField = rootVisualElement.Q<TextField>("sprites-folder-path");
            var btnBrowse = rootVisualElement.Q<Button>("btn-browse-sprites");
            var btnSave = rootVisualElement.Q<Button>("btn-save-sprite");

            _spritesFolderField.value = _settings.SpritesFolderPath;

            var spriteFieldContainer = rootVisualElement.Q<VisualElement>("existing-sprite-field");
            _existingSpriteField = new ObjectField("Existing Sprite") { objectType = typeof(Sprite) };
            spriteFieldContainer.Add(_existingSpriteField);

            btnBrowse.clicked += () =>
            {
                var abs = EditorUtility.OpenFolderPanel("Select Sprites Folder", _spritesFolderField.value, "");

                if (!string.IsNullOrEmpty(abs) && abs.StartsWith(Application.dataPath))
                    _spritesFolderField.value = "Assets" + abs.Substring(Application.dataPath.Length);
            };

            btnSave.clicked += SaveSprite;
        }

        private void RefreshSpritesUI()
        {
            if (_spritesFolderField != null && string.IsNullOrEmpty(_spritesFolderField.value))
                _spritesFolderField.value = _settings.SpritesFolderPath;
        }

        private void ClearExistingSprite()
        {
            if (_existingSpriteField != null)
                _existingSpriteField.value = null;
        }

        private void SaveSprite()
        {
            if (_parsedFile == null || _client == null)
                return;

            var node = SelectedFigmaObject;
            if (node == null)
                return;

            var existingSprite = _existingSpriteField?.value as Sprite;
            if (existingSprite != null)
            {
                LinkExistingSprite(node, existingSprite);
                ClearExistingSprite();
                return;
            }

            if (node.type == FigmaObjectType.COMPONENT_SET && node.children is { Length: > 0 })
            {
                _ = SaveVariantSprites(node);
                return;
            }

            var tex = _framePreview.image as Texture2D;

            _spriteExporter.Export(tex, node, _parsedFile);
            SaveSettings();
            ClearExistingSprite();
        }

        private void LinkExistingSprite(FigmaObject node, Sprite sprite)
        {
            var id = ResolveComponentId(node);
            _settings.SpriteMap.Add(node.name, sprite, id);
            SaveSettings();
            Debug.Log($"[FigmaAutoLayout] Linked sprite '{sprite.name}' to '{node.name}'");
        }

        private string ResolveComponentId(FigmaObject node)
        {
            if (_parsedFile == null)
                return null;

            switch (node.type)
            {
                case FigmaObjectType.COMPONENT:
                case FigmaObjectType.COMPONENT_SET:
                    return _parsedFile.GetComponentKey(node.id);
                case FigmaObjectType.INSTANCE:
                    return _parsedFile.GetComponentKey(node.componentId);
                default:
                    return null;
            }
        }

        private async Task SaveVariantSprites(FigmaObject componentSet)
        {
            var children = componentSet.children;
            var nodeIds = children.Select(c => c.id).ToArray();

            SetImportStatus("loading", $"Downloading {nodeIds.Length} variant sprites...");

            try
            {
                var results = await _client.GetNodesImagesAsync(_fileKey, nodeIds, ct: _cts?.Token ?? default);
                var saved = _spriteExporter.ExportVariants(results, children, _parsedFile);
                SaveSettings();

                SetImportStatus();
                Debug.Log($"[FigmaAutoLayout] Saved {saved} variant sprites.");
            }
            catch (OperationCanceledException)
            {
                SetImportStatus();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[FigmaAutoLayout] Variant sprites failed: {e.Message}");
                SetImportStatus("error", $"Variant sprites failed: {e.Message}");
            }
        }
    }
}
