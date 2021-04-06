using System;
using System.IO;
using Newtonsoft.Json;

namespace Welbox.Classes
{
    public class Config
    {
        public string? SelectedTheme { get; set; }
        public string? Language { get; set; }

        public static Config GetConfig()
        {
            string configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Welbox/appconfig.json");
            //CHECK FOR CONFIG
            Config config = new();
            if (File.Exists(configPath) == false)
            {
                var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Welbox");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                
                config.SelectedTheme = "default";
                config.Language = "en";
                string json = JsonConvert.SerializeObject(config);
                File.WriteAllText(configPath, json);
                return config;
            }
            else
            {
                string json = File.ReadAllText(configPath);
                config = JsonConvert.DeserializeObject<Config>(json);
                return config;
            }
        }
    }
}