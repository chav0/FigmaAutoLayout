namespace Blobler.Objects
{
    public class FigmaStroke 
    {
        
    }

    public enum FigmaAlign
    {
        INSIDE,
        CENTER,
        OUTSIDE, 
        LEFT, 
        RIGHT, 
        JUSTIFIED,
        TOP,
        BOTTOM
    }

    public enum FigmaLayoutAlign
    {
        FIXED, // min
        CENTER, // center
        MAX, // max
        SPACE_BETWEEN // auto size between elements
    }

    public enum FigmaSizing
    {
        AUTO,
        FIXED
    }
}
