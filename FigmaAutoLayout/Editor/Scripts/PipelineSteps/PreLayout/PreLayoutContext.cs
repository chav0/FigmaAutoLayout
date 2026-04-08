using Figma.Objects;
using UnityEngine;

namespace Figma.PipelineSteps
{
    public class PreLayoutContext
    {
        public FigmaObject RootFrame { get; }
        public GameObject RootGameObject { get; set; }

        public PreLayoutContext(FigmaObject rootFrame)
        {
            RootFrame = rootFrame;
        }
    }
}
