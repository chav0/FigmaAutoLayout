using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Figma.Objects;
using Newtonsoft.Json;

namespace Figma
{
    public sealed class FigmaApiClient : IDisposable
    {
        private const string BaseUrl = "https://api.figma.com/v1/";
        private const string TokenHeader = "X-Figma-Token";

        private readonly HttpClient _httpClient;

        public FigmaApiClient(string token)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
            _httpClient.DefaultRequestHeaders.Add(TokenHeader, token);
        }

        public async Task<FigmaFile> GetFileAsync(string fileKey, CancellationToken ct = default)
        {
            using var response = await _httpClient.GetAsync($"files/{fileKey}", ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<FigmaFile>(json);
        }

        public async Task<byte[]> GetNodeThumbnailAsync(string fileKey, string nodeId, float scale = 1f, CancellationToken ct = default)
        {
            var encodedIds = Uri.EscapeDataString(nodeId);
            var scaleStr = scale.ToString(CultureInfo.InvariantCulture);

            using var response = await _httpClient.GetAsync($"images/{fileKey}?ids={encodedIds}&format=png&scale={scaleStr}", ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<ImageExportResponse>(json);

            if (result?.images == null
                || !result.images.TryGetValue(nodeId, out var imageUrl)
                || string.IsNullOrEmpty(imageUrl))
                return null;

            return await GetBytesAsync(imageUrl, ct).ConfigureAwait(false);
        }

        public async Task<Dictionary<string, byte[]>> GetNodesThumbnailsAsync(string fileKey, string[] nodeIds, float scale = 1f, CancellationToken ct = default)
        {
            var encodedIds = Uri.EscapeDataString(string.Join(",", nodeIds));
            var scaleStr = scale.ToString(CultureInfo.InvariantCulture);

            using var response = await _httpClient.GetAsync($"images/{fileKey}?ids={encodedIds}&format=png&scale={scaleStr}", ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<ImageExportResponse>(json);

            var textures = new Dictionary<string, byte[]>();
            if (result?.images == null)
                return textures;

            foreach (var kvp in result.images)
            {
                ct.ThrowIfCancellationRequested();
                if (string.IsNullOrEmpty(kvp.Value))
                    continue;
                var bytes = await GetBytesAsync(kvp.Value, ct).ConfigureAwait(false);
                textures[kvp.Key] = bytes;
            }

            return textures;
        }

        public async Task<FigmaUser> GetCurrentUserAsync(CancellationToken ct = default)
        {
            using var response = await _httpClient.GetAsync("me", ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<FigmaUser>(json);
        }

        private async Task<byte[]> GetBytesAsync(string url, CancellationToken ct)
        {
            using var response = await _httpClient.GetAsync(url, ct).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
        }

        private class ImageExportResponse
        {
            public string err;
            public Dictionary<string, string> images;
        }
        
        public void Dispose() => _httpClient?.Dispose();
    }
}
