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

        public ObjectLayoutContext(GameObject gameObject, FigmaObject figmaObject, Transform parentTransform, FigmaObject rootFrame)
        {
            GameObject = gameObject;
            FigmaObject = figmaObject;
            ParentTransform = parentTransform;
            RootFrame = rootFrame;
        }
    }
}
