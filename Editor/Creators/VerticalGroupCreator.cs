using System;
using Figma.Objects;
using UnityEngine;
using UnityEngine.UI;

namespace Figma.Creators
{
	[Serializable]
    internal class VerticalGroupCreator : CreatorBase
    {
        public override void Create(GameObject gameObject, FigmaObject figmaObject, Transform parent, FigmaObject frame)
        {
            if (figmaObject.layoutMode != FigmaLayoutMode.VERTICAL)
                return;

            var verticalLayoutGroup = gameObject.AddComponent<VerticalLayoutGroup>();
            verticalLayoutGroup.spacing = figmaObject.itemSpacing;
            verticalLayoutGroup.padding = new RectOffset((int) figmaObject.paddingLeft, (int) figmaObject.paddingRight,
                (int) figmaObject.paddingTop, (int) figmaObject.paddingBottom);

            switch (figmaObject.primaryAxisAlignItems)
            {
                case FigmaLayoutAlign.CENTER:
                    switch (figmaObject.counterAxisAlignItems)
                    {
                        case FigmaLayoutAlign.CENTER:
                            verticalLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
                            break;
                        case FigmaLayoutAlign.FIXED:
                            verticalLayoutGroup.childAlignment = TextAnchor.MiddleLeft;
                            break;
                        case FigmaLayoutAlign.MAX:
                            verticalLayoutGroup.childAlignment = TextAnchor.MiddleRight;
                            break;
                    }

                    verticalLayoutGroup.childControlHeight = false;
                    verticalLayoutGroup.childControlWidth = false;
                    verticalLayoutGroup.childForceExpandWidth = false;
                    verticalLayoutGroup.childForceExpandHeight = false;
                    break;
                case FigmaLayoutAlign.FIXED:
                    switch (figmaObject.counterAxisAlignItems)
                    {
                        case FigmaLayoutAlign.CENTER:
                            verticalLayoutGroup.childAlignment = TextAnchor.UpperCenter;
                            break;
                        case FigmaLayoutAlign.FIXED:
                            verticalLayoutGroup.childAlignment = TextAnchor.UpperLeft;
                            break;
                        case FigmaLayoutAlign.MAX:
                            verticalLayoutGroup.childAlignment = TextAnchor.UpperRight;
                            break;
                    }

                    verticalLayoutGroup.childControlHeight = false;
                    verticalLayoutGroup.childControlWidth = false;
                    verticalLayoutGroup.childForceExpandWidth = false;
                    verticalLayoutGroup.childForceExpandHeight = false;
                    break;
                case FigmaLayoutAlign.MAX:
                    switch (figmaObject.counterAxisAlignItems)
                    {
                        case FigmaLayoutAlign.CENTER:
                            verticalLayoutGroup.childAlignment = TextAnchor.LowerCenter;
                            break;
                        case FigmaLayoutAlign.FIXED:
                            verticalLayoutGroup.childAlignment = TextAnchor.LowerLeft;
                            break;
                        case FigmaLayoutAlign.MAX:
                            verticalLayoutGroup.childAlignment = TextAnchor.LowerRight;
                            break;
                    }

                    verticalLayoutGroup.childControlHeight = false;
                    verticalLayoutGroup.childControlWidth = false;
                    verticalLayoutGroup.childForceExpandWidth = false;
                    verticalLayoutGroup.childForceExpandHeight = false;
                    break;

                case FigmaLayoutAlign.SPACE_BETWEEN:
                {
                    switch (figmaObject.counterAxisAlignItems)
                    {
                        case FigmaLayoutAlign.CENTER:
                            verticalLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
                            break;
                        case FigmaLayoutAlign.FIXED:
                            verticalLayoutGroup.childAlignment = TextAnchor.UpperCenter;
                            break;
                        case FigmaLayoutAlign.MAX:
                            verticalLayoutGroup.childAlignment = TextAnchor.LowerCenter;
                            break;
                    }

                    verticalLayoutGroup.childControlHeight = false;
                    verticalLayoutGroup.childControlWidth = false;
                    verticalLayoutGroup.childForceExpandWidth = true;
                    verticalLayoutGroup.childForceExpandHeight = true;
                    break;
                }
            }
        }
    }
}