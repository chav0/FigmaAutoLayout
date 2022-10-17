using System;
using Blobler.Objects;
using UnityEngine;
using UnityEngine.UI;

namespace Blobler.Creators
{
    internal class HorizontalGroupCreator : ICreator
    {
        public void Create(GameObject gameObject, FigmaObject figmaObject, Transform parent, FigmaObject frame)
        {
            if (figmaObject.layoutMode != FigmaLayoutMode.HORIZONTAL)
                return;

            var horizontalLayoutGroup = gameObject.AddComponent<HorizontalLayoutGroup>();
            horizontalLayoutGroup.spacing = figmaObject.itemSpacing;
            horizontalLayoutGroup.padding = new RectOffset((int) figmaObject.paddingLeft, (int) figmaObject.paddingRight,
                (int) figmaObject.paddingTop, (int) figmaObject.paddingBottom);

            switch (figmaObject.primaryAxisAlignItems)
            {
                case FigmaLayoutAlign.CENTER:
                    switch (figmaObject.counterAxisAlignItems)
                    {
                        case FigmaLayoutAlign.CENTER:
                            horizontalLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
                            break;
                        case FigmaLayoutAlign.FIXED:
                            horizontalLayoutGroup.childAlignment = TextAnchor.UpperCenter;
                            break;
                        case FigmaLayoutAlign.MAX:
                            horizontalLayoutGroup.childAlignment = TextAnchor.LowerCenter;
                            break;
                    }

                    horizontalLayoutGroup.childControlHeight = false;
                    horizontalLayoutGroup.childControlWidth = false;
                    horizontalLayoutGroup.childForceExpandWidth = false;
                    horizontalLayoutGroup.childForceExpandHeight = false;
                    break;
                case FigmaLayoutAlign.FIXED:
                    switch (figmaObject.counterAxisAlignItems)
                    {
                        case FigmaLayoutAlign.CENTER:
                            horizontalLayoutGroup.childAlignment = TextAnchor.MiddleLeft;
                            break;
                        case FigmaLayoutAlign.FIXED:
                            horizontalLayoutGroup.childAlignment = TextAnchor.UpperLeft;
                            break;
                        case FigmaLayoutAlign.MAX:
                            horizontalLayoutGroup.childAlignment = TextAnchor.LowerLeft;
                            break;
                    }

                    horizontalLayoutGroup.childControlHeight = false;
                    horizontalLayoutGroup.childControlWidth = false;
                    horizontalLayoutGroup.childForceExpandWidth = false;
                    horizontalLayoutGroup.childForceExpandHeight = false;
                    break;
                case FigmaLayoutAlign.MAX:
                    switch (figmaObject.counterAxisAlignItems)
                    {
                        case FigmaLayoutAlign.CENTER:
                            horizontalLayoutGroup.childAlignment = TextAnchor.MiddleRight;
                            break;
                        case FigmaLayoutAlign.FIXED:
                            horizontalLayoutGroup.childAlignment = TextAnchor.UpperRight;
                            break;
                        case FigmaLayoutAlign.MAX:
                            horizontalLayoutGroup.childAlignment = TextAnchor.LowerRight;
                            break;
                    }

                    horizontalLayoutGroup.childControlHeight = false;
                    horizontalLayoutGroup.childControlWidth = false;
                    horizontalLayoutGroup.childForceExpandWidth = false;
                    horizontalLayoutGroup.childForceExpandHeight = false;
                    break;

                case FigmaLayoutAlign.SPACE_BETWEEN:
                {
                    switch (figmaObject.counterAxisAlignItems)
                    {
                        case FigmaLayoutAlign.CENTER:
                            horizontalLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
                            break;
                        case FigmaLayoutAlign.FIXED:
                            horizontalLayoutGroup.childAlignment = TextAnchor.UpperCenter;
                            break;
                        case FigmaLayoutAlign.MAX:
                            horizontalLayoutGroup.childAlignment = TextAnchor.LowerCenter;
                            break;
                    }

                    horizontalLayoutGroup.childControlHeight = false;
                    horizontalLayoutGroup.childControlWidth = false;
                    horizontalLayoutGroup.childForceExpandWidth = true;
                    horizontalLayoutGroup.childForceExpandHeight = true;
                    break;
                }
            }
        }
    }
}