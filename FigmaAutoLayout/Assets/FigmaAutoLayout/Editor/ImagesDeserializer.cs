using System.Collections.Generic;
using UnityEngine;

namespace Blobler
{
    internal static class ImagesDeserializer
    {
        public static FigmaImages Parse(string jsonString)
        {
            var file = JsonUtility.FromJson<FigmaImages>(jsonString);
            return file; 
        }
    }

    internal class FigmaImages
    {
        public bool error;
        public int status;
        public FigmaImageDictionary meta;
    }

    internal class FigmaImageDictionary
    {
        public Dictionary<string, string> images; 
    }
}
