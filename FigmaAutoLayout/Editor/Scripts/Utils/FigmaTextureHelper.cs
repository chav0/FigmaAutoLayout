using UnityEngine;

namespace Figma.Utils
{
    public static class FigmaTextureHelper
    {
        public static Texture2D CreateFromBytes(byte[] pngBytes)
        {
            if (pngBytes == null || pngBytes.Length == 0)
                return null;

            var tex = new Texture2D(2, 2);
            return tex.LoadImage(pngBytes) ? tex : null;
        }
    }
}
