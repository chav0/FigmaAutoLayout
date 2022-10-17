using System.Globalization;

namespace Figma.Creators
{
    public static class Formatter
    {
        public static string FormatPrefabName(string prefabName) => prefabName.Replace(" ", "");

        public static string FormatVariantPrefabName(string variantName)
        {
            variantName = variantName.Replace(" ", "");
            var properties = variantName.Split(',');
            foreach (var property in properties)
            {
                if (property.StartsWith("Prefab", true, CultureInfo.InvariantCulture))
                    return property.Split('=')[1];
            }

            return variantName; 
        }

        public static string FormatVariantColor(string variantName)
        {
            variantName = variantName.Replace(" ", "");
            var properties = variantName.Split(',');
            foreach (var property in properties)
            {
                if (property.StartsWith("Color", true, CultureInfo.InvariantCulture))
                    return property.Split('=')[1];
            }

            return variantName; 
        }

        public static string FormatPath(string path)
        {
            if (path[path.Length - 1] == '/')
                return path.Substring(0, path.Length - 1);

            return path; 
        }
    }
}