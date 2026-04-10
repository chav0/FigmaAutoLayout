using System;
using Figma.Objects;
using Figma.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static Figma.Objects.FigmaObjectType;

namespace Figma.PipelineSteps
{
    [Serializable]
    public class ImagePipelineStep : FigmaLayoutPipelineObjectStepBase
    {
        public override void Execute(ObjectLayoutContext context)
        {
            var gameObject = context.GameObject;
            var figmaObject = context.FigmaObject;
            if ((figmaObject.type & GRAPHIC) == NONE)
                return;

            if (figmaObject.isMask)
            {
                switch (figmaObject.type)
                {
                    case CANVAS:
                    case FRAME:
                    case RECTANGLE:
                    case IMAGE:
                        gameObject.AddComponent<RectMask2D>();
                        return;
                    case LINE:
                    case REGULAR_POLYGON:
                    case VECTOR:
                    case STAR:
                    case ELLIPSE:
                        var mask = gameObject.AddComponent<Mask>();
                        mask.showMaskGraphic = false;
                        break;
                }
            }

            if (!FigmaColorHelper.NeedAddImage(figmaObject.fills))
                return;

            var image = gameObject.AddComponent<Image>();
            var color = FigmaColorHelper.CalculateColor(figmaObject.fills);
            color = new Color(color.r, color.g, color.b, color.a * figmaObject.opacity.GetValueOrDefault(1f));
            image.color = color;

            var sprite = FindSprite(figmaObject, context.SpriteMap);
            image.sprite = sprite;

            if (sprite != null)
                image.type = sprite.border != Vector4.zero ? Image.Type.Sliced : Image.Type.Simple;

            if (figmaObject.name.ToLower().Contains("button"))
            {
                var button = gameObject.AddComponent<Button>();
                button.image = image;
            }
            else
            {
                image.raycastTarget = false;
            }
        }

        private static Sprite FindSprite(FigmaObject figmaObject, FigmaSpriteMap spriteMap)
        {
            var mapped = spriteMap?.Find(figmaObject.name);
            if (mapped != null)
                return mapped;

            foreach (var fill in figmaObject.fills)
            {
                if (fill.type != "IMAGE")
                    continue;

                var byRef = spriteMap?.Find(fill.imageRef);
                if (byRef != null)
                    return byRef;

                var bySearch = FindSpriteInProject(fill.imageRef);
                if (bySearch != null)
                    return bySearch;
            }

            return FindSpriteInProject(figmaObject.name);
        }

        private static Sprite FindSpriteInProject(string spriteName)
        {
            var guids = AssetDatabase.FindAssets($"t:Sprite {spriteName}", new[] { "Assets" });
            foreach (var guid in guids)
            {
                var sprite = (Sprite)AssetDatabase.LoadAssetAtPath(
                    AssetDatabase.GUIDToAssetPath(guid), typeof(Sprite));
                if (sprite != null && sprite.name.Equals(spriteName))
                    return sprite;
            }

            return null;
        }
    }
}
