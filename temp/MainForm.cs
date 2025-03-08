
using Microsoft.Win32;

namespace PuttyLauncher
{
    public partial class MainForm : Form
    {
        private NotifyIcon _notifyIcon;
        private ContextMenuStrip _contextMenu;

        /// <summary>
        /// The main form constructor
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            // Create a system tray icon
            _notifyIcon = new NotifyIcon
            {
                Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath),
                Text = "PuTTY Launcher",
                Visible = true
            };

            // Create context menu
            _contextMenu = new ContextMenuStrip();

            // Can't happen, but suppresses "might be null" warnings
            if (PuttyLauncher.Settings == null || PuttyLauncher.Settings.Profiles == null)
            {
                MessageBox.Show("Settings not loaded or no profiles defined", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }

            // Add profiles to the context menu
            foreach (var profile in PuttyLauncher.Settings.Profiles)
            {
                Icon? appIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
                if (appIcon != null)
                {
                    Image appImage = appIcon.ToBitmap();

                    _contextMenu.Items.Add(profile.Name, appImage, (s, e) =>
                    {
                        PuttyLauncher.LaunchPutty(profile);
                    });
                }
            }

            // Add a separator and an Exit menu item
            _contextMenu.Items.Add(new ToolStripSeparator());
            _contextMenu.Items.Add("Exit", null, OnExit);

            // Assign the context menu to the notify icon
            _notifyIcon.ContextMenuStrip = _contextMenu;

            // Handle double-click
            _notifyIcon.DoubleClick += (s, e) =>
            {
                // Show the main window
                Show();
                WindowState = FormWindowState.Normal;
                ShowInTaskbar = true;
            };

            // Load UI controls

            checkRunAtLogin.Checked = PuttyLauncher.Settings.RunAtLogin;

            // Load PuTTY sessions into the session picker combo box
            comboBoxPuttySession.Items.Add("(none)");
            foreach (var session in GetPuttySessions())
            {
                comboBoxPuttySession.Items.Add(session);
            }
            comboBoxPuttySession.SelectedIndex = 0;

            // Load profiles into the list box
            if (PuttyLauncher.Settings.Profiles.Count > 0)
            {
                foreach (var profile in PuttyLauncher.Settings.Profiles)
                {
                    listBoxProfiles.Items.Add(profile.Name);
                }

                listBoxProfiles.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// The user clicked the Exit menu item. Close the application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnExit(object? sender, EventArgs? e)
        {
            _notifyIcon.Visible = false;
            Application.Exit();
        }

        /// <summary>
        /// The user clicked the X button. Hide the window and minimize to the system tray
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Check if the user clicked the X button
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
                ShowInTaskbar = false;
            }
        }

        /// <summary>
        /// The user changed the auto-start setting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxAutoStart_CheckedChanged(object sender, EventArgs e)
        {
            if (PuttyLauncher.Settings != null)
            {
                PuttyLauncher.Settings.RunAtLogin = checkRunAtLogin.Checked;
            }
        }

        /// <summary>
        /// The user has picked a different profile from the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBoxProfiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (PuttyLauncher.Settings == null)
                return;

            //Find the selected connection profile
            var selectedProfile = PuttyLauncher.Settings.Profiles.FirstOrDefault(p => p.Name.Equals(listBoxProfiles.SelectedItem));
            textBoxProfileName.Text = selectedProfile?.Name;
            textBoxUsername.Text = selectedProfile?.User;
            textBoxPassword.Text = selectedProfile?.Password;
            comboBoxPuttySession.SelectedItem = selectedProfile?.Session;
        }

        /// <summary>
        /// Get a list of PuTTY sessions from the registry
        /// </summary>
        /// <returns></returns>
        static IEnumerable<string> GetPuttySessions()
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

        /// <summary>
        /// Handle profile name changes
        /// </summary>

        private string savedProfileName = String.Empty;
        private void textBoxProfileName_Enter(object sender, EventArgs e)
        {
            savedProfileName = textBoxProfileName.Text;
        }

        private void textBoxProfileName_Leave(object sender, EventArgs e)
        {
            if (!textBoxProfileName.Text.Equals(savedProfileName))
            {
                //:TODO: Update the profile name
            }
        }

        /// <summary>
        /// Handle putty session name changes
        /// </summary>

        private string savedPuttySession = String.Empty;
        private void comboBoxPuttySession_Enter(object sender, EventArgs e)
        {
            savedPuttySession = comboBoxPuttySession.Text;
        }

        private void comboBoxPuttySession_Leave(object sender, EventArgs e)
        {
            if (PuttyLauncher.Settings == null)
                return;

            if (!comboBoxPuttySession.Text.Equals(savedPuttySession))
            {
                var selectedProfile = PuttyLauncher.Settings.Profiles.FirstOrDefault(p => p.Name.Equals(listBoxProfiles.SelectedItem));
                if (selectedProfile != null)
                    selectedProfile.Session = comboBoxPuttySession.Text;
            }
        }

        /// <summary>
        /// Handle username changes
        /// </summary>

        private string savedUsername = String.Empty;
        private void textBoxUsername_Enter(object sender, EventArgs e)
        {
            savedUsername = textBoxUsername.Text;
        }

        private void textBoxUsername_Leave(object sender, EventArgs e)
        {
            if (PuttyLauncher.Settings == null)
                return;

            if (!textBoxUsername.Text.Equals(savedUsername))
            {
                var selectedProfile = PuttyLauncher.Settings.Profiles.FirstOrDefault(p => p.Name.Equals(listBoxProfiles.SelectedItem));
                if (selectedProfile != null)
                    selectedProfile.User = textBoxUsername.Text;
            }
        }

        /// <summary>
        /// Handle passowrd changes
        /// </summary>

        private string savedPassword = String.Empty;
        private void textBoxPassword_Enter(object sender, EventArgs e)
        {
            savedPassword = textBoxPassword.Text;
        }

        private void textBoxPassword_Leave(object sender, EventArgs e)
        {
            if (!textBoxPassword.Text.Equals(savedPassword))
            {
                if (PuttyLauncher.Settings == null || listBoxProfiles.SelectedItem == null)
                    return;

                var selectedProfile = PuttyLauncher.Settings.Profiles.FirstOrDefault(p => p.Name.Equals((string)(listBoxProfiles.SelectedItem)));

                if (selectedProfile != null)
                    selectedProfile.Password = textBoxPassword.Text;
            }
        }

        /// <summary>
        /// The user double-clicked a profile in the list. Launch PuTTY with the selected profile.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBoxProfiles_DoubleClick(object sender, EventArgs e)
        {
            if (listBoxProfiles.SelectedIndex != -1)
            {
                if (PuttyLauncher.Settings == null || listBoxProfiles.SelectedItem == null)
                    return;

                var selectedProfile = PuttyLauncher.Settings.Profiles.FirstOrDefault(p => p.Name.Equals((string)(listBoxProfiles.SelectedItem)));

                if (selectedProfile != null)
                    PuttyLauncher.LaunchPutty(selectedProfile);
            }
        }
    }
}
