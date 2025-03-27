
using System.Diagnostics;
using Microsoft.Win32;
using System.Reflection;
using System.Runtime.InteropServices;
using System.IO.Pipes;

namespace PuTTYLauncher
{
    internal static class PuTTYLauncher
    {
        public static string ExecutableFile = String.Empty;

        public static AppSettings? Settings { get; private set; }

        private const string autoRunRegistryKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string pipeName = "PuTTYLauncherPipe";

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string? processPath = Environment.ProcessPath;
            if (processPath != null)
                ExecutableFile = Path.ChangeExtension(processPath, ".exe");

            // Check if another instance is already running and if so, tell it to show its window, then exit
            bool createdNew;
            Mutex mutex = new Mutex(true, $"Global\\PuTTYLauncher", out createdNew);

            if (!createdNew)
            {
                try
                {
                    using (var client = new NamedPipeClientStream(".", pipeName, PipeDirection.Out))
                    {
                        client.Connect(1000); // 1 second timeout
                        using (var writer = new StreamWriter(client) { AutoFlush = true })
                        {
                            writer.WriteLine("SHOW");
                        }
                    }
                }
                catch
                {
                    // If the pipe isn't available, ignore error
                }

                // Then exit this instance
                Application.Exit();
                return;
            }

            // Load settings
            try
            {
                Settings = AppSettings.LoadFromFile();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }

            // Do we have settings?
            if (Settings == null)
            {
                MessageBox.Show("Failed to load user settings!", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }

            // Check we have a PuTTY executable
            if (!File.Exists(Settings.PuTTYPath))
            {
                MessageBox.Show("PuTTY not found. Check appsettings.json", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }

            // Check the run at startup status is correct
            if (Settings.RunAtLogin && !IsProgramInStartup())
                AddProgramToStartup();
            else if (!Settings.RunAtLogin && IsProgramInStartup())
                RemoveProgramFromStartup();

            // Run the main form (it will start hidden, but put an icon in the system tray)
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        /// <summary>
        /// Start the named pipe server to listen for messages from other instances
        /// </summary>
        private static void StartPipeServer()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    using (var server = new NamedPipeServerStream(pipeName, PipeDirection.In))
                    {
                        server.WaitForConnection();
                        using (var reader = new StreamReader(server))
                        {
                            var command = reader.ReadLine();
                            if (command == "SHOW")
                            {
                                ShowMainWindow();
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Show the main window
        /// </summary>
        private static void ShowMainWindow()
        {
            // Needs to be run on UI thread
            Application.OpenForms[0]?.BeginInvoke((Action)(() =>
            {
                var form = Application.OpenForms[0];

                if (form?.WindowState == FormWindowState.Minimized)
                    form.WindowState = FormWindowState.Normal;

                form?.Show();
                form?.BringToFront();
                form?.Activate();
            }));
        }

        public static string GetDefaultPuTTYExecutable()
        {
            return Path.Combine(GetDefaultPuTTYFolder(), "putty.exe");
        }

        public static string GetDefaultPuTTYFolder()
        {
            string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            string systemDrive = Environment.GetEnvironmentVariable("SystemDrive") ?? "C:\\";

            if (RuntimeInformation.OSArchitecture == Architecture.X64 ||
                RuntimeInformation.OSArchitecture == Architecture.X86)
            {
                if (Directory.Exists(Path.Combine(programFiles, "PuTTY")))
                    return Path.Combine(programFiles, "PuTTY");

                if (Directory.Exists(Path.Combine(programFilesX86, "PuTTY")))
                    return Path.Combine(programFilesX86, "PuTTY");
            }
            else if (RuntimeInformation.OSArchitecture == Architecture.Arm64 ||
                     RuntimeInformation.OSArchitecture == Architecture.Arm)
            {
                if (Directory.Exists(Path.Combine(programFiles, "PuTTY")))
                    return Path.Combine(programFiles, "PuTTY");
            }

            return systemDrive;
        }

        /// <summary>
        /// Launch PuTTY with the given profile
        /// </summary>
        /// <param name="profile">Profile to launch</param>
        public static void LaunchPutty(ConnectionProfile profile)
        {
            // Can't happen, but suppresses "might be null" warnings
            if (Settings == null)
                return;

            string args = profile.Name.Equals("Default Settings")
                ? $"-load \"{profile.Session}\""
                : $"-load \"{profile.Session}\" -l {profile.User} -pw {DPAPIEncryption.Decrypt(profile.Password)}";

            var psi = new ProcessStartInfo
            {
                FileName = Settings.PuTTYPath,
                Arguments = args,
                UseShellExecute = false
            };
            Process.Start(psi);
        }

        /// <summary>
        /// Check whether the program is set to run at startup
        /// </summary>
        /// <returns></returns>
        static bool IsProgramInStartup()
        {
            using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(autoRunRegistryKey, false))
            {
                return key?.GetValue(Application.ProductName) != null;
            }
        }

        /// <summary>
        /// Add the program to run at startup
        /// </summary>
        static void AddProgramToStartup()
        {
            using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(autoRunRegistryKey, true))
            {
                if (key != null)
                {
                    key.SetValue(Application.ProductName, $"\"{ExecutableFile}\"");
                }
            }
        }

        /// <summary>
        /// Remove the program from running at startup
        /// </summary>
        static void RemoveProgramFromStartup()
        {
            if (Application.ProductName != null)
            {
                using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(autoRunRegistryKey, true))
                {
                    if (key?.GetValue(Application.ProductName) != null)
                    {
                        key.DeleteValue(Application.ProductName);
                    }
                }
            }
        }

        /// <summary>
        /// Get a list of PuTTY sessions from the registry
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetPuttySessions()
        {
            const string puttyRegKey = @"Software\SimonTatham\PuTTY\Sessions";
            using RegistryKey? key = Registry.CurrentUser.OpenSubKey(puttyRegKey);

            if (key != null)
            {
                foreach (var sessionName in key.GetSubKeyNames())
                {
                    yield return sessionName.Replace("%20", " ");
                }
            }
        }
    }
}
