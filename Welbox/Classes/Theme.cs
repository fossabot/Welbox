using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace Welbox.Classes
{
    public class Theme
    {
        private static readonly string DefaultConfig = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Welbox/Themes/default.json"); 
        public string? DisplayName { get; set; }
        public string? FileName { get; set; }
        public string? BgPath { get; set; }
        public string? HexTextColor { get; set; }
        public string? Source { get; set; }
        public double? FontSize { get; set; }
        public List<LaunchItem>? LaunchItems { get; set; }

        public static Theme GetTheme(string name)
        {
            string themePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Welbox/Themes/"+name+".json");
            var curTheme = new Theme();
            if (File.Exists(themePath))
            {
                string json = File.ReadAllText(themePath);
                curTheme = JsonConvert.DeserializeObject<Theme>(json);
            }
            else if (File.Exists(themePath) == false && !File.Exists(DefaultConfig))
            {
                var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Welbox/Themes");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                curTheme.DisplayName = "Default";
                curTheme.FileName = "default";
                curTheme.Source = "file";
                curTheme.HexTextColor = "#ffffff";
                curTheme.FontSize = 60.0;
                curTheme.LaunchItems = new List<LaunchItem>();
                DownloadImage(
                    "https://images.unsplash.com/photo-1617444114429-fc5dd4bca329?crop=entropy&cs=tinysrgb&fit=crop&fm=jpg&h=1080&ixlib=rb-1.2.1&q=80&w=1920",
                    "defaultbg.jpeg");
                curTheme.BgPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Welbox/Images/defaultbg.jpeg");
                string json = JsonConvert.SerializeObject(curTheme);
                File.WriteAllText(DefaultConfig, json);
            }
            else
            {
                string json = File.ReadAllText(DefaultConfig);
                curTheme = JsonConvert.DeserializeObject<Theme>(json);
            }
            return curTheme;
        }

        private static bool DownloadImage(string url, string filename)
        {
            if (!Directory.Exists(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Welbox/Images")))
                Directory.CreateDirectory(Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Welbox/Images"));
            using WebClient client = new();
            client.DownloadFile(new Uri(url), Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Welbox/Images/"+filename));
            return true;
        }

        public static List<Theme> GetAvailableThemes()
        {
            var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Welbox/Themes");
            var files = Directory.GetFiles(dir);
            List<Theme> themes = new();
            foreach (var file in files)
            {
                string json = File.ReadAllText(file);
                Console.WriteLine(json);
                Theme th = new Theme();
                try
                {
                    th = JsonConvert.DeserializeObject<Theme>(json);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                if (th != null)
                {
                    themes.Add(th);
                }
            }

            return themes;
        }

    }
    
}