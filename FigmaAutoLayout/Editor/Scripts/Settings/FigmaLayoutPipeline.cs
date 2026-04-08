using System;
using System.Collections.Generic;
using Figma.PipelineSteps;
using UnityEngine;

namespace Figma
{
    [Serializable]
    public class FigmaLayoutPipelineProfile
    {
        [SerializeField] private string id;

        [SerializeReference, SubclassSelector]
        private List<FigmaPreLayoutPipelineStepBase> preLayoutSteps = new();

        [SerializeReference, SubclassSelector]
        private List<FigmaLayoutPipelineObjectStepBase> objectLayoutSteps = new()
        {
            new TextPipelineStep(),
            new RectTransformPipelineStep(),
            new ImagePipelineStep(),
            new VerticalGroupPipelineStep(),
            new HorizontalGroupPipelineStep(),
            new GridPipelineStep(),
            new ContentSizeFitterPipelineStep()
        };

        [SerializeReference, SubclassSelector]
        private List<FigmaPostLayoutPipelineStepBase> postLayoutSteps = new();

        public string Id
        {
            get => id;
            set => id = value;
        }

        public List<FigmaPreLayoutPipelineStepBase> PreLayoutSteps => preLayoutSteps;
        public List<FigmaLayoutPipelineObjectStepBase> ObjectLayoutSteps => objectLayoutSteps;
        public List<FigmaPostLayoutPipelineStepBase> PostLayoutSteps => postLayoutSteps;
    }
}
