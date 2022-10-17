namespace Blobler.Objects
{
    public class FigmaFile
    {
        public FigmaDocument document;
        public string name;
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
}
