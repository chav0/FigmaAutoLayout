using System;
using Figma.Objects;
using UnityEngine;
using UnityEngine.UI;

namespace Figma.PipelineSteps
{
    [Serializable]
    internal class GridPipelineStep : FigmaLayoutPipelineObjectStepBase
    {
        public override void Execute(ObjectLayoutContext context)
        {
            var figmaObject = context.FigmaObject;
            if (figmaObject.layoutMode != FigmaLayoutMode.GRID)
                return;

            var grid = context.GameObject.AddComponent<GridLayoutGroup>();

            grid.padding = new RectOffset(
                (int)figmaObject.paddingLeft,
                (int)figmaObject.paddingRight,
                (int)figmaObject.paddingTop,
                (int)figmaObject.paddingBottom);

            grid.spacing = new Vector2(figmaObject.gridColumnGap, figmaObject.gridRowGap);

            if (figmaObject.gridColumnCount > 0)
            {
                grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                grid.constraintCount = figmaObject.gridColumnCount;
            }
            else if (figmaObject.gridRowCount > 0)
            {
                grid.constraint = GridLayoutGroup.Constraint.FixedRowCount;
                grid.constraintCount = figmaObject.gridRowCount;
            }
            else
            {
                grid.constraint = GridLayoutGroup.Constraint.Flexible;
            }

            grid.cellSize = CalculateCellSize(figmaObject);
            grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            grid.startAxis = GridLayoutGroup.Axis.Horizontal;
            grid.childAlignment = ResolveAlignment(figmaObject);
        }

        private static Vector2 CalculateCellSize(FigmaObject figmaObject)
        {
            var bb = figmaObject.absoluteBoundingBox;
            if (bb.width == null || bb.height == null)
                return new Vector2(100, 100);

            var cols = figmaObject.gridColumnCount > 0 ? figmaObject.gridColumnCount : 1;
            var rows = figmaObject.gridRowCount > 0 ? figmaObject.gridRowCount : 1;

            var totalW = bb.width.Value - figmaObject.paddingLeft - figmaObject.paddingRight;
            var totalH = bb.height.Value - figmaObject.paddingTop - figmaObject.paddingBottom;

            var cellW = (totalW - figmaObject.gridColumnGap * (cols - 1)) / cols;
            var cellH = (totalH - figmaObject.gridRowGap * (rows - 1)) / rows;

            return new Vector2(Mathf.Max(cellW, 1f), Mathf.Max(cellH, 1f));
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
                _ => TextAnchor.UpperLeft
            };
        }
    }
}
