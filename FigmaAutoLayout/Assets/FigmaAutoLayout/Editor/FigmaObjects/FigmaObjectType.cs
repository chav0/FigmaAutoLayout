using System;

namespace Blobler.Objects
{
    [Flags]
    public enum FigmaObjectType
    {
        NONE = 0,
        DOCUMENT = 1,
        CANVAS = 2,
        FRAME = 4,
        RECTANGLE = 8,
        LINE = 16,
        REGULAR_POLYGON = 32,
        VECTOR = 64,
        STAR = 128,
        ELLIPSE = 256,
        TEXT = 512,
        IMAGE = 1024,
        GROUP = 2048, 
        INSTANCE = 4096, 
        COMPONENT = 8192,
        COMPONENT_SET = 16384,
        BOOLEAN_OPERATION = 32768,
        
        GRAPHIC = FRAME | RECTANGLE | LINE | REGULAR_POLYGON | VECTOR | STAR | ELLIPSE | IMAGE | GROUP | INSTANCE | COMPONENT,
    }
}