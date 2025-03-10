
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PuTTYLauncher
{
    class AppSettings
    {
        private static string DefaultSettingsFile = Path.Combine(Path.GetDirectoryName(PuTTYLauncher.ExecutableFile) ?? "", "appsettings.json");
        private static string SettingsFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PuTTYLauncher.json");
        private static bool initialLoadSettings = true;

        public static AppSettings? LoadFromFile()
        {
            AppSettings? settings = null;

            // Do we have a user settings file?

            if (!File.Exists(SettingsFile))
            {
                try
                {
                    File.Copy(DefaultSettingsFile, SettingsFile);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException($"Failed to create user settings file: {ex.Message}");
                }
            }

            // Load the settings file

            try
            {
                string jsonString = File.ReadAllText(SettingsFile);
                settings = JsonSerializer.Deserialize<AppSettings>(jsonString);
                if (settings != null)
                {
                    initialLoadSettings = false;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to load settings: {ex.Message}");
            }

            return settings ?? null;
        }

        public bool SaveToFile()
        {
            bool saved = false;

            if (initialLoadSettings == true)
                return false;

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            try
            {
                string json = JsonSerializer.Serialize(this, options);
                File.WriteAllText(SettingsFile, json);
                saved = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save settings: {ex.Message}", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return saved;
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
