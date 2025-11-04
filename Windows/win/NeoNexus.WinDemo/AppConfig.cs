using System;

namespace NeoNexus.WinDemo
{
    public class AppConfig
    {
        public DownloadConfig Download { get; set; } = new DownloadConfig();
        public CacheConfig Cache { get; set; } = new CacheConfig();
        public VitaDbConfig VitaDB { get; set; } = new VitaDbConfig();
        public CbpsConfig Cbps { get; set; } = new CbpsConfig();
        public LoggingConfig Logging { get; set; } = new LoggingConfig();
    }

    public class DownloadConfig
    {
        public string DefaultDownloadFolder { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads\\NeoNexus";
        public int MaxConcurrentDownloads { get; set; } = 3;
        public string TemporaryExtension { get; set; } = ".part";
    }

    public class CacheConfig
    {
        public string CacheFolder { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\NeoNexus\\cache";
        public int CacheTtlSeconds { get; set; } = 86400;
    }

    public class VitaDbConfig
    {
        public string HomebrewsUrl { get; set; } = "https://rinnegatamante.eu/vitadb/list_hbs_json.php";
        public string PluginsUrl { get; set; } = "https://rinnegatamante.eu/vitadb/list_plugins_json.php";
        public string ToolsUrl { get; set; } = "https://rinnegatamante.eu/vitadb/list_tools_json.php";
    }

    public class CbpsConfig
    {
        public string CsvUrl { get; set; } = "https://raw.githubusercontent.com/Team-CBPS/cbps-db/main/cbpsdb.csv";
    }

    public class LoggingConfig
    {
        public string Level { get; set; } = "Information";
        public string LogFolder { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\NeoNexus\\logs";
        public string LogFile { get; set; } = "neonexus-.log";
        public string RollingInterval { get; set; } = "Day";
    }
}
