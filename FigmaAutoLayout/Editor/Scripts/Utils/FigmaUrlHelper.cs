using System;
using System.Text.RegularExpressions;

namespace Figma.Utils
{
    internal static class FigmaUrlHelper
    {
        private static readonly Regex FileKeyPattern = new(@"/(design|file|proto)/([^/]+)(?:/branch/([^/]+))?", RegexOptions.Compiled);

        internal static string ExtractFileKey(string figmaFileUrl)
        {
            if (string.IsNullOrWhiteSpace(figmaFileUrl))
                throw new ArgumentException("Figma file URL cannot be empty.", nameof(figmaFileUrl));

            var uri = new Uri(figmaFileUrl);
            var match = FileKeyPattern.Match(uri.AbsolutePath);

            if (!match.Success)
                throw new ArgumentException($"Invalid Figma URL format. Expected: https://www.figma.com/design/:key/:title, got: {figmaFileUrl}", nameof(figmaFileUrl));

            return match.Groups[3].Success ? match.Groups[3].Value : match.Groups[2].Value;
        }
    }
}
