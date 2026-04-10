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
        private readonly FigmaSpriteMap _spriteMap;

        public SpriteExporter(FigmaAutoLayoutSettings settings)
        {
            _spritesPath = settings.SpritesFolderPath;
            _spriteMap = settings.SpriteMap;
        }

        public void Export(Texture2D texture, FigmaObject node, FigmaFile file)
        {
            if (texture == null)
            {
                Debug.LogWarning("[FigmaAutoLayout] No preview loaded — select a frame first.");
                return;
            }

            var id = ResolveComponentKey(node, file);
            SaveSprite(texture, node.name, node.name, id);
        }

        public int ExportVariants(Dictionary<string, byte[]> nodeImages, FigmaObject[] children, FigmaFile file)
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
                var id = ResolveComponentKey(child, file);
                SaveSprite(texture, spriteName, child.name, id);
                saved++;
            }

            return saved;
        }

        private static string ResolveComponentKey(FigmaObject node, FigmaFile file)
        {
            if (file == null)
                return null;

            switch (node.type)
            {
                case FigmaObjectType.COMPONENT:
                case FigmaObjectType.COMPONENT_SET:
                    return file.GetComponentKey(node.id);
                case FigmaObjectType.INSTANCE:
                    return file.GetComponentKey(node.componentId);
                default:
                    return null;
            }
        }

        private void SaveSprite(Texture2D texture, string name, string originalName, string id)
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
                _spriteMap.Add(originalName, sprite, id);

            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(filePath));

            Debug.Log($"[FigmaAutoLayout] Sprite saved: {filePath}");
        }
    }
}
