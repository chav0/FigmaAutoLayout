using System;
using Figma.Objects;
using UnityEngine;

namespace Figma.Creators
{
	[Serializable]
    internal class RectTransformCreator : CreatorBase
    {
        public override void Create(GameObject gameObject, FigmaObject figmaObject, Transform parent, FigmaObject frame)
        {
            var rect = gameObject.GetComponent<RectTransform>();
            if (rect == null)
                rect = gameObject.AddComponent<RectTransform>();

            if (parent == null)
                return;

            var xPos = 0f;
            var yPos = 0f;
            var xAnchorMin = 0f;
            var yAnchorMin = 0f; 
            var xAnchorMax = 0f;
            var yAnchorMax = 0f;
            var xPivot = 0f;
            var yPivot = 0f;

            var objectX = figmaObject.absoluteBoundingBox.x ?? 0; 
            var objectY = figmaObject.absoluteBoundingBox.y ?? 0; 
            
            var objectW = figmaObject.absoluteBoundingBox.width ?? 0; 
            var objectH = figmaObject.absoluteBoundingBox.height ?? 0; 
            
            var frameX = frame.absoluteBoundingBox.x ?? 0; 
            var frameY = frame.absoluteBoundingBox.y ?? 0; 

            switch (figmaObject.constraints.horizontal)
            {
                case FigmaSeparatedConstraints.LEFT:
                    xPivot = 0f;
                    xAnchorMin = 0f;
                    xAnchorMax = 0f;
                    xPos = objectX - frameX;
                    break;
                case FigmaSeparatedConstraints.RIGHT:
                    xPivot = 1f;
                    xAnchorMin = 1f;
                    xAnchorMax = 1f;
                    xPos = objectX - frameX + objectW;
                    break;
                case FigmaSeparatedConstraints.CENTER:
                    xPivot = 0.5f;
                    xAnchorMin = 0.5f;
                    xAnchorMax = 0.5f;
                    xPos = objectX - frameX + objectW / 2f;
                    break;
                case FigmaSeparatedConstraints.LEFT_RIGHT:
                    xPivot = 0.5f;
                    xAnchorMin = 0f;
                    xAnchorMax = 1f;
                    xPos = objectX - frameX + objectW / 2f;
                    break;
                case FigmaSeparatedConstraints.SCALE:
                    Debug.LogWarning($"Scale vertical constraints found in {figmaObject.name}. It does not supported in Unity. It will be changed to Left-Right");
                    xPivot = 0.5f;
                    xAnchorMin = 0f;
                    xAnchorMax = 1f;
                    xPos = objectX - frameX + objectW / 2f;
                    break;
            }
            
            switch (figmaObject.constraints.vertical)
            {
                case FigmaSeparatedConstraints.BOTTOM:
                    yPivot = 0f;
                    yAnchorMin = 0f;
                    yAnchorMax = 0f;
                    yPos = frameY - objectY - objectH;
                    break;
                case FigmaSeparatedConstraints.TOP:
                    yPivot = 1f;
                    yAnchorMin = 1f;
                    yAnchorMax = 1f;
                    yPos = frameY - objectY;
                    break;
                case FigmaSeparatedConstraints.CENTER:
                    yPivot = 0.5f;
                    yAnchorMin = 0.5f;
                    yAnchorMax = 0.5f;
                    yPos = frameY - objectY - objectH / 2f;
                    break;
                case FigmaSeparatedConstraints.TOP_BOTTOM:
                    yPivot = 0.5f;
                    yAnchorMin = 0f;
                    yAnchorMax = 1f;
                    yPos = frameY - objectY - objectH / 2f;
                    break;
                case FigmaSeparatedConstraints.SCALE:
                    Debug.LogWarning($"Scale vertical constraints found in {figmaObject.name}. It does not supported in Unity. It will be changed to Top-Bottom");
                    yPivot = 0.5f;
                    yAnchorMin = 0f;
                    yAnchorMax = 1f;
                    yPos = frameY - objectY - objectH / 2f;
                    break;
            }
            
            rect.pivot = new Vector2(xPivot, yPivot); 
            rect.anchorMin = new Vector2(xAnchorMin, yAnchorMin); 
            rect.anchorMax = new Vector2(xAnchorMax, yAnchorMax);
            rect.anchoredPosition = new Vector2(xPos, yPos);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, objectW);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, objectH);
            rect.transform.SetParent(parent, true);
            
            switch (figmaObject.layoutMode)
            {
                case FigmaLayoutMode.HORIZONTAL:

                    
                    break;
                
                case FigmaLayoutMode.VERTICAL:

                    break;
            }
        }
    }
}
