using Figma.Objects;
using UnityEngine;

namespace Figma.PipelineSteps
{
    public class PostLayoutContext
    {
        public GameObject Prefab { get; }
        public FigmaObject RootFrame { get; }

        public PostLayoutContext(GameObject prefab, FigmaObject rootFrame)
        {
            Prefab = prefab;
            RootFrame = rootFrame;
        }
    }
}
