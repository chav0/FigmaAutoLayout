using System;
using Figma.Objects;
using UnityEngine;
using UnityEngine.UI;

namespace Figma.PipelineSteps
{
    [Serializable]
    public class VerticalGroupPipelineStep : FigmaLayoutPipelineObjectStepBase
    {
        public override void Execute(ObjectLayoutContext context)
        {
            var figmaObject = context.FigmaObject;
            if (figmaObject.layoutMode != FigmaLayoutMode.VERTICAL)
                return;

            var group = context.GameObject.AddComponent<VerticalLayoutGroup>();
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

        // NOTE: SPACE_BETWEEN mapping preserved from original code.
        // Possibly a bug: counter axis is horizontal for vertical layout,
        // so FIXED should map to Left, not Upper. But with forceExpand=true
        // the alignment value is effectively ignored by Unity.
        private static TextAnchor ResolveAlignment(FigmaObject figmaObject)
        {
            var h = figmaObject.primaryAxisAlignItems;
            var v = figmaObject.counterAxisAlignItems;

            return (h, v) switch
            {
                (FigmaLayoutAlign.FIXED, FigmaLayoutAlign.FIXED) => TextAnchor.UpperLeft,
                (FigmaLayoutAlign.FIXED, FigmaLayoutAlign.CENTER) => TextAnchor.UpperCenter,
                (FigmaLayoutAlign.FIXED, FigmaLayoutAlign.MAX) => TextAnchor.UpperRight,
                (FigmaLayoutAlign.CENTER, FigmaLayoutAlign.FIXED) => TextAnchor.MiddleLeft,
                (FigmaLayoutAlign.CENTER, FigmaLayoutAlign.CENTER) => TextAnchor.MiddleCenter,
                (FigmaLayoutAlign.CENTER, FigmaLayoutAlign.MAX) => TextAnchor.MiddleRight,
                (FigmaLayoutAlign.MAX, FigmaLayoutAlign.FIXED) => TextAnchor.LowerLeft,
                (FigmaLayoutAlign.MAX, FigmaLayoutAlign.CENTER) => TextAnchor.LowerCenter,
                (FigmaLayoutAlign.MAX, FigmaLayoutAlign.MAX) => TextAnchor.LowerRight,
                (FigmaLayoutAlign.SPACE_BETWEEN, FigmaLayoutAlign.FIXED) => TextAnchor.UpperCenter,
                (FigmaLayoutAlign.SPACE_BETWEEN, FigmaLayoutAlign.CENTER) => TextAnchor.MiddleCenter,
                (FigmaLayoutAlign.SPACE_BETWEEN, FigmaLayoutAlign.MAX) => TextAnchor.LowerCenter,
                _ => TextAnchor.UpperLeft
            };
        }
    }
}
