using System;

namespace Figma.PipelineSteps
{
    [Serializable]
    public abstract class FigmaPostLayoutPipelineStepBase
    {
        public abstract void Execute(PostLayoutContext context);
    }
}
