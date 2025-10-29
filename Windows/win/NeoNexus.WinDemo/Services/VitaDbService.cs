using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace NeoNexus.WinDemo.Services
{
    public static class VitaDbService
    {
        private static readonly HttpClient http = new HttpClient { Timeout = TimeSpan.FromSeconds(20) };

        public const string HomebrewsUrl = "https://rinnegatamante.eu/vitadb/list_hbs_json.php";
        public const string PluginsUrl = "https://rinnegatamante.eu/vitadb/list_plugins_json.php";
        public const string ToolsUrl = "https://rinnegatamante.eu/vitadb/list_tools_json.php";

        public static async Task<JsonElement[]> FetchListAsync(string url)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url));
            using var res = await http.GetAsync(url).ConfigureAwait(false);
            res.EnsureSuccessStatusCode();
            var bytes = await res.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
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
