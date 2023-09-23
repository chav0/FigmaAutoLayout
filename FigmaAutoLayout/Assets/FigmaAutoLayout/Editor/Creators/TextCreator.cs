using System;
using Blobler.Objects;
using Blobler.Utils;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Blobler.Creators
{
	[Serializable]
    internal class TextCreator : CreatorBase
    {
        public override void Create(GameObject gameObject, FigmaObject figmaObject, Transform parent, FigmaObject frame)
        {
            if (figmaObject.type != FigmaObjectType.TEXT)
                return;
            
            var text = gameObject.AddComponent<TextMeshProUGUI>();
            text.fontSize = figmaObject.style.fontSize;
            text.color = ColorHelper.CalculateColor(figmaObject.fills);
            text.font = FindFont(figmaObject.style.fontFamily, figmaObject.style.fontPostScriptName);
            text.text = figmaObject.characters;
            text.alignment = GetAlignment(figmaObject.style.textAlignHorizontal, figmaObject.style.textAlignVertical);
            text.characterSpacing = figmaObject.style.letterSpacing / figmaObject.style.fontSize * 100;
            text.paragraphSpacing = figmaObject.style.paragraphSpacing / figmaObject.style.fontSize * 100;
            text.fontStyle = GetCase(figmaObject.style.textCase);
        }
        
        private TMP_FontAsset FindFont(string fontFamily, string fontPostScript)
        {
            try
            {
                var splittedPostScript = fontPostScript.Split('-');
                var styleName = splittedPostScript[1];
                var guids = AssetDatabase.FindAssets("t:TMP_FontAsset", new[] {"Assets"});
                foreach (var guid in guids)
                {
                    var font = (TMP_FontAsset) AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(TMP_FontAsset));
                    if (font.faceInfo.familyName == fontFamily && font.faceInfo.styleName == styleName)
                        return font;
                }
            }
            catch
            {
                Debug.LogError($"{fontPostScript} has strange format.");
            }

            Debug.LogWarning($"Font {fontPostScript} not found!");
            return null; 
        }

        private TextAlignmentOptions GetAlignment(FigmaAlign horizontal, FigmaAlign vertical)
        {
            switch (horizontal)
            {
                case FigmaAlign.CENTER:
                    switch (vertical)
                    {
                        case FigmaAlign.CENTER:
                            return TextAlignmentOptions.Center;
                        case FigmaAlign.TOP:
                            return TextAlignmentOptions.Top; 
                        case FigmaAlign.BOTTOM:
                            return TextAlignmentOptions.Bottom;
                    }
                    break;
                case FigmaAlign.LEFT:
                    switch (vertical)
                    {
                        case FigmaAlign.CENTER:
                            return TextAlignmentOptions.Left;
                        case FigmaAlign.TOP:
                            return TextAlignmentOptions.TopLeft; 
                        case FigmaAlign.BOTTOM:
                            return TextAlignmentOptions.BottomLeft;
                    }
                    break;
                case FigmaAlign.RIGHT:
                    switch (vertical)
                    {
                        case FigmaAlign.CENTER:
                            return TextAlignmentOptions.Right;
                        case FigmaAlign.TOP:
                            return TextAlignmentOptions.TopRight; 
                        case FigmaAlign.BOTTOM:
                            return TextAlignmentOptions.BottomRight;
                    }
                    break;
                case FigmaAlign.JUSTIFIED:
                    switch (vertical)
                    {
                        case FigmaAlign.CENTER:
                            return TextAlignmentOptions.Justified;
                        case FigmaAlign.TOP:
                            return TextAlignmentOptions.TopJustified; 
                        case FigmaAlign.BOTTOM:
                            return TextAlignmentOptions.BottomJustified;
                    }
                    break;
            }

            return TextAlignmentOptions.Baseline; 
        }

        private FontStyles GetCase(TextCase textCase)
        {
            switch (textCase)
            {
                case TextCase.NONE:
                    return FontStyles.Normal;
                case TextCase.UPPER:
                    return FontStyles.UpperCase;
                case TextCase.LOWER:
                    return FontStyles.LowerCase;
                case TextCase.TITLE:
                    return FontStyles.Normal;
            }

            return FontStyles.Normal; 
        }
    }
}
