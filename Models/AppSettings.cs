using System;
using System.IO;
using System.Text.Json;

namespace ABNCapture
{
    public class AppSettings
    {
        public string SaveFolder { get; set; } = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "ABN Capture");

        public bool AlwaysSaveToFile { get; set; } = true;
        public bool CopyToClipboard { get; set; } = true;
        public bool StartWithWindows { get; set; } = false;

        // If true: skip the floating toolbar and capture right after the selection is made
        public bool DirectSelection { get; set; } = false;

        public string HotkeyAreaCapture { get; set; } = "Ctrl+Shift+S";
        public string HotkeyFullscreenCapture { get; set; } = "Ctrl+Shift+F";
        public string HotkeyRecordToggle { get; set; } = "Ctrl+Shift+R";

        private static string SettingsPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ABNCapture", "settings.json");

        public static AppSettings Load()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    string json = File.ReadAllText(SettingsPath);
                    var loaded = JsonSerializer.Deserialize<AppSettings>(json);
                    if (loaded != null) return loaded;
                }
            }
            catch
            {
            }

            return new AppSettings();
        }

        public void Save()
        {
            string? dir = Path.GetDirectoryName(SettingsPath);
            if (dir != null) Directory.CreateDirectory(dir);

            string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(SettingsPath, json);
        }
    }
}
