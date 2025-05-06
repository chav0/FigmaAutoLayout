using System;
using Figma.Creators;
using Figma.Objects;
using UnityEngine;
using UnityEngine.UI;

namespace Figma.Utils
{
	[Serializable]
    internal class ContentSizeFitterCreator : CreatorBase
    {
        public override void Create(GameObject gameObject, FigmaObject figmaObject, Transform parent, FigmaObject frame)
        {
            if (figmaObject.layoutMode == FigmaLayoutMode.NONE)
                return;

            if (figmaObject.counterAxisSizingMode == FigmaSizing.FIXED &&
                figmaObject.primaryAxisSizingMode == FigmaSizing.FIXED)
                return;

            var contentSizeFitter = gameObject.AddComponent<ContentSizeFitter>(); 
            switch (figmaObject.layoutMode)
            {
                case FigmaLayoutMode.HORIZONTAL:
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