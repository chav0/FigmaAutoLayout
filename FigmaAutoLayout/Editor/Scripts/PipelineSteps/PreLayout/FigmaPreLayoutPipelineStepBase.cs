using System;

namespace Figma.PipelineSteps
{
    [Serializable]
    public abstract class FigmaPreLayoutPipelineStepBase
    {
        public abstract void Execute(PreLayoutContext context);
    }
}
