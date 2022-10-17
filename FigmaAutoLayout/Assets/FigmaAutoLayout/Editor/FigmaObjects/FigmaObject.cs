﻿namespace Blobler.Objects
{
    public class FigmaObject
    {
        public string id;
        public string name;
        public bool visible = true;
        public FigmaObjectType type;
        public FigmaColor backgroundColor; 
        public FigmaObject[] children;
        public FigmaBox absoluteBoundingBox;
        public FigmaConstraints constraints;
        public FigmaFill[] fills;
        public FigmaFill[] strokes;
        public float strokeWeight;
        public float? opacity;
        public FigmaAlign strokeAlign;
        public string characters;
        public FigmaTextStyle style; 
        public bool isMask; 
        public FigmaLayoutMode layoutMode; 
        public FigmaLayoutAlign counterAxisAlignItems; // layout mode alignment
        public FigmaLayoutAlign primaryAxisAlignItems; // layout mode alignment
        public FigmaSizing counterAxisSizingMode; 
        public FigmaSizing primaryAxisSizingMode; 
        public float itemSpacing; 
        public float paddingLeft; 
        public float paddingRight;
        public float paddingTop;
        public float paddingBottom;
        public string componentId; 
    }
}
