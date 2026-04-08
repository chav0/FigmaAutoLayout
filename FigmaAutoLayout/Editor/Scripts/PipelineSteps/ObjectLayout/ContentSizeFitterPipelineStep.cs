using System;
using Figma.Objects;
using UnityEngine.UI;

namespace Figma.PipelineSteps
{
	[Serializable]
    internal class ContentSizeFitterPipelineStep : FigmaLayoutPipelineObjectStepBase
    {
        public override void Execute(ObjectLayoutContext context)
        {
            var figmaObject = context.FigmaObject;
            if (figmaObject.layoutMode == FigmaLayoutMode.NONE)
                return;

            if (figmaObject.counterAxisSizingMode == FigmaSizing.FIXED &&
                figmaObject.primaryAxisSizingMode == FigmaSizing.FIXED)
                return;

            var contentSizeFitter = context.GameObject.AddComponent<ContentSizeFitter>(); 
            switch (figmaObject.layoutMode)
            {
                case FigmaLayoutMode.HORIZONTAL:
                case FigmaLayoutMode.GRID:
                    contentSizeFitter.horizontalFit = figmaObject.primaryAxisSizingMode == FigmaSizing.AUTO
                        ? ContentSizeFitter.FitMode.PreferredSize
                        : ContentSizeFitter.FitMode.Unconstrained; 
                    
                    contentSizeFitter.verticalFit = figmaObject.counterAxisSizingMode == FigmaSizing.AUTO
                        ? ContentSizeFitter.FitMode.PreferredSize
                        : ContentSizeFitter.FitMode.Unconstrained; 
                    break;
                case FigmaLayoutMode.VERTICAL:
                    contentSizeFitter.verticalFit = figmaObject.primaryAxisSizingMode == FigmaSizing.AUTO
                        ? ContentSizeFitter.FitMode.PreferredSize
                        : ContentSizeFitter.FitMode.Unconstrained; 
                    
                    contentSizeFitter.horizontalFit = figmaObject.counterAxisSizingMode == FigmaSizing.AUTO
                        ? ContentSizeFitter.FitMode.PreferredSize
                        : ContentSizeFitter.FitMode.Unconstrained; 
                    break;
            }
        }
    }
}
