using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Figma.Objects;
using Figma.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Figma
{
    public partial class FigmaAutoLayoutWindow
    {
        private TextField _spritesFolderField;
        private Task<Dictionary<string, byte[]>> _variantSpritesTask;
        private FigmaObject[] _pendingVariantChildren;

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

            var frame = _parsedFile.document.children[_selectedPage].children[_selectedFrame];

            if (frame.type == FigmaObjectType.COMPONENT_SET && frame.children is { Length: > 0 })
            {
                SaveVariantSprites(frame);
                return;
            }

            var tex = _framePreview.image as Texture2D;
            
            _spriteExporter.Export(tex, frame.name);
        }

        private void SaveVariantSprites(FigmaObject componentSet)
        {
            _pendingVariantChildren = componentSet.children;
            var nodeIds = _pendingVariantChildren.Select(c => c.id).ToArray();

            SetImportStatus("loading", $"Downloading {nodeIds.Length} variant sprites...");

            EditorApplication.update -= PollVariantSprites;
            
            _variantSpritesTask = Task.Run(async () => await _client.GetNodesThumbnailsAsync(_fileKey, nodeIds));
            
            EditorApplication.update += PollVariantSprites;
        }

        private void PollVariantSprites()
        {
            if (_variantSpritesTask == null || !_variantSpritesTask.IsCompleted)
                return;

            EditorApplication.update -= PollVariantSprites;

            try
            {
                var results = _variantSpritesTask.Result;
                var saved = _spriteExporter.ExportVariants(results, _pendingVariantChildren);

                SetImportStatus(null, null);
                Debug.Log($"[FigmaAutoLayout] Saved {saved} variant sprites.");
            }
            catch (Exception e)
            {
                var inner = e is AggregateException ae ? ae.Flatten().InnerException : e;
                
                Debug.LogWarning($"[FigmaAutoLayout] Variant sprites failed: {inner?.Message}");
                SetImportStatus("error", $"Variant sprites failed: {inner?.Message}");
            }

            _variantSpritesTask = null;
            _pendingVariantChildren = null;
        }
    }
}
