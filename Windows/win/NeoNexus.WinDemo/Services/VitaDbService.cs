using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NeoNexus.WinDemo.Services
{
    /// <summary>
    /// VitaDB fetcher with simple file caching.
    /// Cache uses Config.Current.Cache.CacheFolder and TTL from CacheTtlSeconds.
    /// </summary>
    public static class VitaDbService
    {
        private static readonly HttpClient http = new HttpClient { Timeout = TimeSpan.FromSeconds(20) };

        public const string HomebrewsUrl = "https://rinnegatamante.eu/vitadb/list_hbs_json.php";
        public const string PluginsUrl = "https://rinnegatamante.eu/vitadb/list_plugins_json.php";
        public const string ToolsUrl = "https://rinnegatamante.eu/vitadb/list_tools_json.php";

        /// <summary>
        /// Fetch list from VitaDB with caching.
        /// </summary>
        public static async Task<JsonElement[]> FetchListAsync(string url)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url));
            var cacheFolder = Config.Current.Cache.CacheFolder;
            Directory.CreateDirectory(cacheFolder);

            string cacheFile = Path.Combine(cacheFolder, UrlToCacheFileName(url));
            int ttl = Math.Max(0, Config.Current.Cache.CacheTtlSeconds);

            // Serve from cache if fresh
            if (File.Exists(cacheFile))
            {
                var info = new FileInfo(cacheFile);
                var age = DateTime.UtcNow - info.LastWriteTimeUtc;
                if (age.TotalSeconds <= ttl)
                {
                    var cachedBytes = await File.ReadAllBytesAsync(cacheFile).ConfigureAwait(false);
                    return ParseJson(cachedBytes);
                }
            }

            // Download
            using (var res = await http.GetAsync(url).ConfigureAwait(false))
            {
                res.EnsureSuccessStatusCode();
                var bytes = await res.Content.ReadAsByteArrayAsync().ConfigureAwait(false);

                // Persist to cache
                try { await File.WriteAllBytesAsync(cacheFile, bytes).ConfigureAwait(false); } catch { /* best-effort */ }

                return ParseJson(bytes);
            }
        }

        private static JsonElement[] ParseJson(byte[] bytes)
        {
            var doc = JsonDocument.Parse(bytes);
            if (doc.RootElement.ValueKind == JsonValueKind.Array)
            {
                var arr = doc.RootElement;
                var output = new JsonElement[arr.GetArrayLength()];
                int i = 0;
                foreach (var el in arr.EnumerateArray()) output[i++] = el;
                return output;
            }
            return new[] { doc.RootElement };
        }

        private static string UrlToCacheFileName(string url)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(url));
                return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant() + ".json";
            }
        }

        public static string GetTitle(JsonElement item)
        {
            if (item.ValueKind != JsonValueKind.Object) return item.ToString();
            if (item.TryGetProperty("title", out var t) && t.ValueKind == JsonValueKind.String) return t.GetString()!;
            if (item.TryGetProperty("name", out t) && t.ValueKind == JsonValueKind.String) return t.GetString()!;
            if (item.TryGetProperty("label", out t) && t.ValueKind == JsonValueKind.String) return t.GetString()!;
            foreach (var p in item.EnumerateObject())
            {
                if (p.Value.ValueKind == JsonValueKind.String) return p.Value.GetString()!;
            }
            return item.ToString();
        }
    }
}
