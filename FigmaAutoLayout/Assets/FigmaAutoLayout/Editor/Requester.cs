using System;
using System.IO;
using System.Net;
using Blobler.Objects;
using UnityEditor;
using UnityEngine;

namespace Blobler
{
    public static class Requester
    {
        private const string Url = "https://api.figma.com/v1/files/";
        
        public static FigmaFile FileRequest(string token, string fileURL)
        {
            var request = WebRequest.Create(Url + GetFileNameFromURL(fileURL));
            request.Headers["X-Figma-Token"] = token;

            var response = request.GetResponse();
            var json = "";
            using var stream = response.GetResponseStream();
            if (stream != null)
            {
                using var reader = new StreamReader(stream);
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    json += line;
                }
            }

            response.Close();

            EditorGUIUtility.systemCopyBuffer = json;

            return FigmaDocDeserializer.Parse(json);
        }
        
        public static void ImageRequest(string token, string fileURL)
        {
            var request = WebRequest.Create(Url + GetFileNameFromURL(fileURL) + "/images");
            request.Headers["X-Figma-Token"] = token; 
            
            var response = request.GetResponse();
            var json = "";
            using var stream = response.GetResponseStream();
            if (stream != null)
            {
                using var reader = new StreamReader(stream);
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    json += line;
                }
            }

            var images = ImagesDeserializer.Parse(json);
            response.Close();

            foreach (var image in images.meta.images)
            {
                using var client = new WebClient();
                client.DownloadFile(image.Value, $"{Application.dataPath}/{image.Key}.png");
            }
        }
        
        private static string GetFileNameFromURL(string fileURL)
        {
            var uri = new Uri(fileURL);
            var parts = uri.AbsolutePath.Split('/');
            if (parts.Length == 4 && parts[1] == "file")
            {
                return parts[2]; 
            }

            throw new Exception(
                "Not correct file URL. It looks like https://www.figma.com/file/:key/:title");
        }
    }
}