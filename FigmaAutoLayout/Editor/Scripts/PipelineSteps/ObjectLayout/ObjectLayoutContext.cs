using Figma.Objects;
using UnityEngine;

namespace Figma.PipelineSteps
{
    public class ObjectLayoutContext
    {
        public GameObject GameObject { get; }
        public FigmaObject FigmaObject { get; }
        public Transform ParentTransform { get; }
        public FigmaObject RootFrame { get; }
        public FigmaIconMap IconMap { get; }

        public ObjectLayoutContext(GameObject gameObject, FigmaObject figmaObject,
            Transform parentTransform, FigmaObject rootFrame, FigmaIconMap iconMap = null)
        {
            GameObject = gameObject;
            FigmaObject = figmaObject;
            ParentTransform = parentTransform;
            RootFrame = rootFrame;
            IconMap = iconMap;
        }
    }
}
