using System;
using System.IO;
using Newtonsoft.Json;

namespace ResticUI
{
    public class AppSettings
    {
        public string ResticRepository { get; set; } = string.Empty;
        public string ResticPasswordFile { get; set; } = string.Empty;
        public string BackupScriptPath { get; set; } = string.Empty;

        private static readonly string SettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");

        public static AppSettings Load()
        {
            if (File.Exists(SettingsPath))
            {
                try
                {
                    string json = File.ReadAllText(SettingsPath);
                    return JsonConvert.DeserializeObject<AppSettings>(json) ?? new AppSettings();
                }
                catch
                {
                    return new AppSettings();
                }
            }
            return new AppSettings();
        }

        public void Save()
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(SettingsPath, json);
        }
    }
}
