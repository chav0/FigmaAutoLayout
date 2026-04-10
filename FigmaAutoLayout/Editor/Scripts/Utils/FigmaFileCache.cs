using System;
using System.IO;
using Figma.Objects;
using Newtonsoft.Json;
using UnityEngine;

namespace Figma.Utils
{
    internal static class FigmaFileCache
    {
        private const string FileName = "last_import.json";

        private static string FilePath => Path.Combine(FigmaAssetPathHelper.StorageDir, FileName);

        internal static void Save(FigmaFile file, string fileUrl)
        {
            try
            {
                Directory.CreateDirectory(FigmaAssetPathHelper.StorageDir);
                var wrapper = new CacheWrapper { url = fileUrl, file = file };
                var json = JsonConvert.SerializeObject(wrapper);
                File.WriteAllText(FilePath, json);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[FigmaAutoLayout] Failed to save cache: {e.Message}");
            }
        }

        internal static (FigmaFile file, string url) Load()
        {
            try
            {
                if (!File.Exists(FilePath))
                    return (null, null);

                var json = File.ReadAllText(FilePath);
                var wrapper = JsonConvert.DeserializeObject<CacheWrapper>(json);
                return (wrapper?.file, wrapper?.url);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[FigmaAutoLayout] Failed to load cache: {e.Message}");
                return (null, null);
            }
        }

        private class CacheWrapper
        {
            public string url;
            public FigmaFile file;
        }
    }
}
