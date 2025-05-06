using System.Globalization;

namespace Figma.Utils
{
    public static class FormatHelper
    {
        public static string FormatPrefabName(string prefabName) => prefabName
            .Replace(" ", "")
            .Replace("/", "");

        public static string FormatVariantPrefabName(string variantName)
        {
            variantName = variantName
                .Replace(" ", "")
                .Replace("/", "");
            
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
            variantName = variantName.Replace(" ", "").Replace("/", "");
            var properties = variantName.Split(',');
            foreach (var property in properties)
            {
                if (property.StartsWith("Color", true, CultureInfo.InvariantCulture))
                    return property.Split('=')[1];
            }

            return variantName; 
        }

        public static string FormatPath(string path) => 
            path[^1] == '/' ? path[..^1] : path;
    }
}