using System;

namespace Figma.PipelineSteps
{
    [Serializable]
    public abstract class FigmaLayoutPipelineObjectStepBase
    {
        public abstract void Execute(ObjectLayoutContext context);
    }
}
