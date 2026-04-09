using System.Collections.Generic;
using System.IO;
using Figma.Objects;
using Figma.Utils;
using UnityEditor;
using UnityEngine;

namespace Figma.Exporters
{
    public class SpriteExporter
    {
        private readonly string _spritesPath;
        private readonly FigmaIconMap _iconMap;

        public SpriteExporter(FigmaAutoLayoutSettings settings)
        {
            _spritesPath = settings.SpritesFolderPath;
            _iconMap = settings.IconMap;
        }

        public void Export(Texture2D texture, string frameName)
        {
            if (texture == null)
            {
                Debug.LogWarning("[FigmaAutoLayout] No preview loaded — select a frame first.");
                return;
            }

            SaveSprite(texture, frameName, frameName);
        }

        public void Export(byte[] pngBytes, string frameName)
        {
            var texture = FigmaTextureHelper.CreateFromBytes(pngBytes);
            Export(texture, frameName);
        }

        public int ExportVariants(Dictionary<string, byte[]> nodeImages, FigmaObject[] children)
        {
            if (string.IsNullOrEmpty(_spritesPath))
            {
                Debug.LogWarning("[FigmaAutoLayout] Sprites folder is not set. Configure it in Settings.");
                return 0;
            }

            var saved = 0;

            foreach (var child in children)
            {
                if (!nodeImages.TryGetValue(child.id, out var bytes) || bytes == null)
                    continue;

                var texture = FigmaTextureHelper.CreateFromBytes(bytes);
                if (texture == null)
                    continue;

                var spriteName = FigmaAssetPathHelper.ExtractVariantSpriteName(child.name);
                SaveSprite(texture, spriteName, child.name);
                saved++;
            }

            return saved;
        }

        private void SaveSprite(Texture2D texture, string name, string originalName)
        {
            if (string.IsNullOrEmpty(_spritesPath))
            {
                Debug.LogWarning("[FigmaAutoLayout] Sprites folder is not set. Configure it in Settings.");
                return;
            }

            var filePath = FigmaAssetPathHelper.BuildAssetPath(_spritesPath, name, "png");
            var pngBytes = texture.EncodeToPNG();

            File.WriteAllBytes(filePath, pngBytes);
            AssetDatabase.ImportAsset(filePath, ImportAssetOptions.ForceUpdate);

            var importer = AssetImporter.GetAtPath(filePath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.SaveAndReimport();
            }

            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(filePath);
            if (sprite != null)
                _iconMap.Add(originalName, sprite);

            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(filePath));

            Debug.Log($"[FigmaAutoLayout] Sprite saved: {filePath}");
        }

    }
}
