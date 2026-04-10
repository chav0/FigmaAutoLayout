using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Figma.Utils
{
    internal sealed class FigmaTokenStorage
    {
        private const string FileName = "credentials.json";

        private string FilePath => Path.Combine(FigmaAssetPathHelper.StorageDir, FileName);

        internal string LoadToken()
        {
            try
            {
                if (!File.Exists(FilePath))
                    return null;

                var json = File.ReadAllText(FilePath);
                var data = JsonConvert.DeserializeObject<Credentials>(json);
                return data?.token;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[FigmaAutoLayout] Failed to load token: {e.Message}");
                return null;
            }
        }

        internal void SaveToken(string token)
        {
            try
            {
                Directory.CreateDirectory(FigmaAssetPathHelper.StorageDir);
                var json = JsonConvert.SerializeObject(new Credentials { token = token }, Formatting.Indented);
                File.WriteAllText(FilePath, json);
            }
            catch (Exception e)
            {
                Debug.LogError($"[FigmaAutoLayout] Failed to save token: {e.Message}");
            }
        }

        private class Credentials
        {
            public string token;
        }
    }
}
