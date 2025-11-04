using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using NeoNexus.WinDemo.Models;

namespace NeoNexus.WinDemo.Services
{
    public static class CbpsService
    {
        private static readonly HttpClient http = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };

        /// <summary>
        /// Fetch and parse CBPS CSV with caching.
        /// </summary>
        public static async Task<List<CbpsEntry>> FetchAndParseCsvAsync(string rawCsvUrl)
        {
            var cacheFolder = Config.Current.Cache.CacheFolder;
            Directory.CreateDirectory(cacheFolder);

            string cacheFile = Path.Combine(cacheFolder, UrlToCacheFileName(rawCsvUrl));
            int ttl = Math.Max(0, Config.Current.Cache.CacheTtlSeconds);

            if (File.Exists(cacheFile))
            {
                var info = new FileInfo(cacheFile);
                if ((DateTime.UtcNow - info.LastWriteTimeUtc).TotalSeconds <= ttl)
                {
                    var cached = await File.ReadAllTextAsync(cacheFile).ConfigureAwait(false);
                    return ParseCsv(cached);
                }
            }

            using (var res = await http.GetAsync(rawCsvUrl).ConfigureAwait(false))
            {
                res.EnsureSuccessStatusCode();
                var csv = await res.Content.ReadAsStringAsync().ConfigureAwait(false);

                try { await File.WriteAllTextAsync(cacheFile, csv).ConfigureAwait(false); } catch { /* best-effort */ }

                return ParseCsv(csv);
            }
        }

        public static List<CbpsEntry> ParseCsv(string csvText)
        {
            var lines = SplitLines(csvText);
            if (lines.Count == 0) return new List<CbpsEntry>();

            var header = ParseCsvLine(lines[0]);
            var entries = new List<CbpsEntry>(lines.Count - 1);

            for (int i = 1; i < lines.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;
                var fields = ParseCsvLine(lines[i]);
                var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                for (int f = 0; f < header.Count; f++)
                {
                    var key = header[f];
                    var val = f < fields.Count ? fields[f] : string.Empty;
                    map[key] = val;
                }
                entries.Add(new CbpsEntry(map));
            }
            return entries;
        }

        private static List<string> SplitLines(string text)
        {
            var lines = new List<string>();
            using var sr = new StringReader(text);
            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                lines.Add(line);
            }
            return lines;
        }

        private static List<string> ParseCsvLine(string line)
        {
            var fields = new List<string>();
            if (line == null) return fields;

            int len = line.Length;
            var cur = new System.Text.StringBuilder();
            bool inQuotes = false;
            for (int i = 0; i < len; i++)
            {
                char c = line[i];
                if (inQuotes)
                {
                    if (c == '"')
                    {
                        if (i + 1 < len && line[i + 1] == '"')
                        {
                            cur.Append('"');
                            i++;
                        }
                        else
                        {
                            inQuotes = false;
                        }
                    }
                    else
                    {
                        cur.Append(c);
                    }
                }
                else
                {
                    if (c == '"')
                    {
                        inQuotes = true;
                    }
                    else if (c == ',')
                    {
                        fields.Add(cur.ToString());
                        cur.Clear();
                    }
                    else
                    {
                        cur.Append(c);
                    }
                }
            }
            fields.Add(cur.ToString());
            return fields;
        }

        private static string UrlToCacheFileName(string url)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(url));
                return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant() + ".csv";
            }
        }
    }
}
