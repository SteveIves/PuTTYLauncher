
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PuTTYLauncher
{
    class AppSettings
    {
        public string SettingsFile { get; private set; } = String.Empty;
        private static bool initialLoadSettings = true;

        public static AppSettings? LoadFromFile(string SettingsFile)
        {
            AppSettings? settings = null;

            // Do we have a settings file?
            if (File.Exists(SettingsFile))
            {
                // Load the settings file
                try
                {
                    string jsonString = File.ReadAllText(SettingsFile);
                    settings = JsonSerializer.Deserialize<AppSettings>(jsonString);
                    if (settings != null)
                    {
                        settings.SettingsFile = SettingsFile;
                        initialLoadSettings = false;
                    }
                }
                catch (Exception)
                {
                }
            }

            return settings ?? null;
        }
        private async void saveToFile()
        {
            if (initialLoadSettings == true)
                return;

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            try
            {
                string json = JsonSerializer.Serialize(this, options);
                await File.WriteAllTextAsync(PuTTYLauncher.DefaultSettingsFile, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Run at login

        private bool runAtLogin = false;
        public bool RunAtLogin
        {
            get => runAtLogin;
            set
            {
                if (runAtLogin != value)
                {
                    runAtLogin = value;
                    if (!initialLoadSettings)
                    {
                        saveToFile();
                    }
                }
            }
        }

        //PuTTY path

        private string puTTYPath = "C:\\Program Files\\PuTTY\\putty.exe";

        public string PuTTYPath
        {
            get => puTTYPath;
            set
            {
                if (puTTYPath != value)
                {
                    puTTYPath = value;
                    if (!initialLoadSettings)
                    {
                        saveToFile();
                    }
                }
            }
        }

        // Profiles

        private List<ConnectionProfile> profiles = new List<ConnectionProfile>();

        public List<ConnectionProfile> Profiles
        {
            get => profiles;
            set
            {
                if (profiles != value)
                {
                    // Unsubscribe from old list
                    foreach (var profile in profiles)
                    {
                        profile.PropertyChanged -= Profile_PropertyChanged;
                    }

                    profiles = value;

                    // Subscribe to new list
                    foreach (var profile in profiles)
                    {
                        profile.PropertyChanged += Profile_PropertyChanged;
                    }

                    saveToFile();
                }
            }
        }
        private void Profile_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            saveToFile();
        }
    }
}
