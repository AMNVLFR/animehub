using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;

namespace AnimeHub.Helpers
{
    public class TmdbService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl;
        private readonly IMemoryCache _cache;

        public TmdbService(HttpClient httpClient, IConfiguration configuration, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Tmdb:ApiKey"];
            _baseUrl = configuration["Tmdb:BaseUrl"];
            _cache = cache;
        }

        public async Task<TmdbSearchResult?> SearchAnimeAsync(string query, string language = "ja")
        {
            var url = $"{_baseUrl}search/tv?api_key={_apiKey}&query={query}&language={language}";
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TmdbSearchResult>(json);
            }
            return null;
        }

        public async Task<TmdbTvDetails?> GetAnimeDetailsAsync(int tmdbId, string language = "ja")
        {
            var cacheKey = $"tmdb_details_{tmdbId}_{language}";
            if (_cache.TryGetValue(cacheKey, out TmdbTvDetails? cachedResult))
            {
                return cachedResult;
            }

            var url = $"{_baseUrl}tv/{tmdbId}?api_key={_apiKey}&language={language}";
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<TmdbTvDetails>(json);
                _cache.Set(cacheKey, result, TimeSpan.FromHours(1));
                return result;
            }
            return null;
        }

        public async Task<TmdbImages?> GetAnimeImagesAsync(int tmdbId)
        {
            var cacheKey = $"tmdb_images_{tmdbId}";
            if (_cache.TryGetValue(cacheKey, out TmdbImages? cachedResult))
            {
                return cachedResult;
            }

            var url = $"{_baseUrl}tv/{tmdbId}/images?api_key={_apiKey}";
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<TmdbImages>(json);
                _cache.Set(cacheKey, result, TimeSpan.FromHours(1));
                return result;
            }
            return null;
        }
    }

    // DTOs
    public class TmdbSearchResult
    {
        public List<TmdbTvShow> Results { get; set; } = new();
    }

    public class TmdbTvShow
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string OriginalName { get; set; } = string.Empty;
        public string Overview { get; set; } = string.Empty;
        public string FirstAirDate { get; set; } = string.Empty;
        public double VoteAverage { get; set; }
        public string PosterPath { get; set; } = string.Empty;
        public string BackdropPath { get; set; } = string.Empty;
    }

    public class TmdbTvDetails : TmdbTvShow
    {
        public List<TmdbSeason> Seasons { get; set; } = new();
        public List<TmdbGenre> Genres { get; set; } = new();
        public string Status { get; set; } = string.Empty;
        public int NumberOfSeasons { get; set; }
        public int NumberOfEpisodes { get; set; }
    }

    public class TmdbSeason
    {
        public int SeasonNumber { get; set; }
        public string Name { get; set; } = string.Empty;
        public int EpisodeCount { get; set; }
    }

    public class TmdbGenre
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class TmdbImages
    {
        public List<TmdbImage> Backdrops { get; set; } = new();
        public List<TmdbImage> Posters { get; set; } = new();
    }

    public class TmdbImage
    {
        public string FilePath { get; set; } = string.Empty;
        public double VoteAverage { get; set; }
    }
}