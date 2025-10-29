using System.Collections.Generic;

namespace NeoNexus.WinDemo.Models
{
    public class CbpsEntry
    {
        private readonly Dictionary<string, string> map;

        public CbpsEntry(Dictionary<string, string> map)
        {
            this.map = map ?? new Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase);
        }

        public string Id => Get("id");
        public string Title => Get("title");
        public string Credits => Get("credits");
        public string DownloadIcon0 => Get("download_icon0");
        public string DownloadUrl => Get("download_url");
        public string Type => Get("type");
        public string Visible => Get("visible");

        public string Get(string key)
        {
            if (key == null) return string.Empty;
            if (map.TryGetValue(key, out var v)) return v ?? string.Empty;
            return string.Empty;
        }

        public override string ToString() => $"{Id} - {Title} ({Type})";
    }
}
