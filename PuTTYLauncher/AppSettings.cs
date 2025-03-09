
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PuTTYLauncher
{
    class AppSettings
    {
        public static string DefaultSettingsFile = Path.Combine(Path.GetDirectoryName(PuTTYLauncher.ExecutableFile) ?? "", "appsettings.json");
        public static string UserSettingsFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PuTTYLauncher.json");
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
                    MessageBox.Show($"Failed to load settings from {SettingsFile}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return settings ?? null;
        }

        public void SaveToFile()
        {
            if (initialLoadSettings == true)
                return;

            if (SettingsFile.Equals(DefaultSettingsFile))
                SettingsFile = UserSettingsFile;

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            try
            {
                string json = JsonSerializer.Serialize(this, options);
                File.WriteAllText(SettingsFile, json);
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
                        SaveToFile();
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
                        SaveToFile();
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

                    SaveToFile();
                }
            }
        }
        private void Profile_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            SaveToFile();
        }
    }
}
