using System;
using Figma.Objects;
using UnityEngine;
using UnityEngine.UI;

namespace Figma.PipelineSteps
{
    [Serializable]
    internal class HorizontalGroupPipelineStep : FigmaLayoutPipelineObjectStepBase
    {
        public override void Execute(ObjectLayoutContext context)
        {
            var figmaObject = context.FigmaObject;
            if (figmaObject.layoutMode != FigmaLayoutMode.HORIZONTAL)
                return;

            var group = context.GameObject.AddComponent<HorizontalLayoutGroup>();
            group.spacing = figmaObject.itemSpacing;
            group.padding = new RectOffset(
                (int)figmaObject.paddingLeft,
                (int)figmaObject.paddingRight,
                (int)figmaObject.paddingTop,
                (int)figmaObject.paddingBottom);

            var isSpaceBetween = figmaObject.primaryAxisAlignItems == FigmaLayoutAlign.SPACE_BETWEEN;

            group.childAlignment = ResolveAlignment(figmaObject);
            group.childControlHeight = false;
            group.childControlWidth = false;
            group.childForceExpandWidth = isSpaceBetween;
            group.childForceExpandHeight = isSpaceBetween;
        }

        private static TextAnchor ResolveAlignment(FigmaObject figmaObject)
        {
            var h = figmaObject.primaryAxisAlignItems;
            var v = figmaObject.counterAxisAlignItems;

            return (h, v) switch
            {
                (FigmaLayoutAlign.FIXED, FigmaLayoutAlign.FIXED) => TextAnchor.UpperLeft,
                (FigmaLayoutAlign.FIXED, FigmaLayoutAlign.CENTER) => TextAnchor.MiddleLeft,
                (FigmaLayoutAlign.FIXED, FigmaLayoutAlign.MAX) => TextAnchor.LowerLeft,
                (FigmaLayoutAlign.CENTER, FigmaLayoutAlign.FIXED) => TextAnchor.UpperCenter,
                (FigmaLayoutAlign.CENTER, FigmaLayoutAlign.CENTER) => TextAnchor.MiddleCenter,
                (FigmaLayoutAlign.CENTER, FigmaLayoutAlign.MAX) => TextAnchor.LowerCenter,
                (FigmaLayoutAlign.MAX, FigmaLayoutAlign.FIXED) => TextAnchor.UpperRight,
                (FigmaLayoutAlign.MAX, FigmaLayoutAlign.CENTER) => TextAnchor.MiddleRight,
                (FigmaLayoutAlign.MAX, FigmaLayoutAlign.MAX) => TextAnchor.LowerRight,
                (FigmaLayoutAlign.SPACE_BETWEEN, FigmaLayoutAlign.FIXED) => TextAnchor.UpperCenter,
                (FigmaLayoutAlign.SPACE_BETWEEN, FigmaLayoutAlign.CENTER) => TextAnchor.MiddleCenter,
                (FigmaLayoutAlign.SPACE_BETWEEN, FigmaLayoutAlign.MAX) => TextAnchor.LowerCenter,
                _ => TextAnchor.UpperLeft
            };
        }
    }
}
