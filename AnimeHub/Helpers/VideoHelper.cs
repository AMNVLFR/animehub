using System.Text.RegularExpressions;

namespace AnimeHub.Helpers
{
    public static class VideoHelper
    {
        public static string? GetEmbedUrl(string? videoUrl)
        {
            if (string.IsNullOrEmpty(videoUrl)) return null;

            // YouTube URLs
            var youtubeMatch = Regex.Match(videoUrl, @"(?:youtube\.com\/watch\?v=|youtu\.be\/|youtube\.com\/embed\/)([a-zA-Z0-9_-]{11})");
            if (youtubeMatch.Success)
            {
                return $"https://www.youtube.com/embed/{youtubeMatch.Groups[1].Value}";
            }

            // Vimeo URLs
            var vimeoMatch = Regex.Match(videoUrl, @"(?:vimeo\.com\/)([0-9]+)");
            if (vimeoMatch.Success)
            {
                return $"https://player.vimeo.com/video/{vimeoMatch.Groups[1].Value}";
            }

            // Dailymotion URLs
            var dailymotionMatch = Regex.Match(videoUrl, @"(?:dailymotion\.com\/video\/)([a-zA-Z0-9]+)");
            if (dailymotionMatch.Success)
            {
                return $"https://www.dailymotion.com/embed/video/{dailymotionMatch.Groups[1].Value}";
            }

            // Streamable URLs
            var streamableMatch = Regex.Match(videoUrl, @"(?:streamable\.com\/)([a-zA-Z0-9]+)");
            if (streamableMatch.Success)
            {
                return $"https://streamable.com/e/{streamableMatch.Groups[1].Value}";
            }

            // Bunny.net iframe URLs (already embed URLs)
            if (videoUrl.StartsWith("https://iframe.mediadelivery.net/"))
            {
                return videoUrl;
            }

            return null;
        }

        public static bool IsValidVideoUrl(string? url)
        {
            if (string.IsNullOrEmpty(url)) return false;

            // YouTube URLs
            if (Regex.IsMatch(url, @"(?:youtube\.com\/watch\?v=|youtu\.be\/|youtube\.com\/embed\/)[a-zA-Z0-9_-]{11}"))
            {
                return true;
            }

            // Vimeo URLs
            if (Regex.IsMatch(url, @"(?:vimeo\.com\/)[0-9]+"))
            {
                return true;
            }

            // Dailymotion URLs
            if (Regex.IsMatch(url, @"(?:dailymotion\.com\/video\/)[a-zA-Z0-9]+"))
            {
                return true;
            }

            // Streamable URLs
            if (Regex.IsMatch(url, @"(?:streamable\.com\/)[a-zA-Z0-9]+"))
            {
                return true;
            }

            // Bunny.net iframe URLs
            if (url.StartsWith("https://iframe.mediadelivery.net/"))
            {
                return true;
            }

            return false;
        }
    }
}