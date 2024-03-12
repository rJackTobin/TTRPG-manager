using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace TTRPG_manager
{
    internal class ConfigManager
    {
        private static readonly string ConfigFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppConfig.json");

        public static async Task<AppConfig> LoadConfigAsync()
        {
            if (File.Exists(ConfigFilePath))
            {
                string json = await File.ReadAllTextAsync(ConfigFilePath);
                return JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
            }

            return new AppConfig();
        }

        public static async Task SaveConfigAsync(AppConfig config)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(config, options);
            await File.WriteAllTextAsync(ConfigFilePath, json);
        }
    }
}
