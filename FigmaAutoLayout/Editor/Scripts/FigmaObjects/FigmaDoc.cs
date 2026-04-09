using System.Collections.Generic;

namespace Figma.Objects
{
    public class FigmaFile
    {
        public FigmaDocument document;
        public string name;
        public Dictionary<string, FigmaComponentMeta> components;

        public string GetComponentKey(string nodeId)
        {
            if (components != null && components.TryGetValue(nodeId, out var meta))
                return meta.key;
            return null;
        }
    }

    public class FigmaDocument
    {
        public FigmaPage[] children;
        public FigmaObjectType type;
        public string name;
    }

    public class FigmaPage
    {
        public string name;
        public FigmaObjectType type;
        public FigmaObject[] children;
    }

    public class FigmaComponentMeta
    {
        public string key;
        public string name;
        public string description;
        public string componentSetId;
    }
}
