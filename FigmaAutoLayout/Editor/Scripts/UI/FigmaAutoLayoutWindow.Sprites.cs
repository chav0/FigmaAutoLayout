using System;
using System.Linq;
using System.Threading.Tasks;
using Figma.Objects;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Figma
{
    public partial class FigmaAutoLayoutWindow
    {
        private TextField _spritesFolderField;

        private void SetupSprites()
        {
            _spritesFolderField = rootVisualElement.Q<TextField>("sprites-folder-path");
            var btnBrowse = rootVisualElement.Q<Button>("btn-browse-sprites");
            var btnSave = rootVisualElement.Q<Button>("btn-save-sprite");

            _spritesFolderField.value = _settings.SpritesFolderPath;

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

        private void SaveSprite()
        {
            if (_parsedFile == null || _client == null)
                return;

            var node = SelectedFigmaObject;
            if (node == null)
                return;

            if (node.type == FigmaObjectType.COMPONENT_SET && node.children is { Length: > 0 })
            {
                _ = SaveVariantSprites(node);
                return;
            }

            var tex = _framePreview.image as Texture2D;

            _spriteExporter.Export(tex, node.name);
            SaveSettings();
        }

        private async Task SaveVariantSprites(FigmaObject componentSet)
        {
            var children = componentSet.children;
            var nodeIds = children.Select(c => c.id).ToArray();

            SetImportStatus("loading", $"Downloading {nodeIds.Length} variant sprites...");

            try
            {
                var results = await _client.GetNodesImagesAsync(_fileKey, nodeIds, ct: _cts?.Token ?? default);
                var saved = _spriteExporter.ExportVariants(results, children);
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
