using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace NeoNexus.WinDemo
{
    public static class Config
    {
        private static readonly Lazy<AppConfig> _config = new Lazy<AppConfig>(LoadConfig);

        public static AppConfig Current => _config.Value;

        private static AppConfig LoadConfig()
        {
            var builder = new ConfigurationBuilder();

            // Add appsettings.json from app folder (if present)
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var path = Path.Combine(basePath, "appsettings.json");
            if (File.Exists(path))
            {
                builder.AddJsonFile(path, optional: false, reloadOnChange: true);
            }
            else
            {
                // fallback to project folder's appsettings.json when running from source
                var projectPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\..\\win\\NeoNexus.WinDemo");
                var projectJson = Path.Combine(projectPath, "appsettings.json");
                if (File.Exists(projectJson))
                    builder.AddJsonFile(projectJson, optional: true, reloadOnChange: true);
            }

            var configuration = builder.Build();
            var cfg = new AppConfig();
            configuration.Bind(cfg);
            ExpandEnvironmentVariables(cfg);
            return cfg;
        }

        private static void ExpandEnvironmentVariables(AppConfig cfg)
        {
            if (cfg == null) return;

            cfg.Download.DefaultDownloadFolder = Environment.ExpandEnvironmentVariables(cfg.Download.DefaultDownloadFolder ?? string.Empty);
            cfg.Cache.CacheFolder = Environment.ExpandEnvironmentVariables(cfg.Cache.CacheFolder ?? string.Empty);
            cfg.Logging.LogFolder = Environment.ExpandEnvironmentVariables(cfg.Logging.LogFolder ?? string.Empty);
        }
    }
}
