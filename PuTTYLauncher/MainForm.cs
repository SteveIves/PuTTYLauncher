
using Microsoft.Win32;

namespace PuTTYLauncher
{
    public partial class MainForm : Form
    {
        private NotifyIcon _notifyIcon;
        private ContextMenuStrip _contextMenu;
        private ConnectionProfile? _selectedProfile;
        private ConnectionProfile? _selectedProfileBackup;
        private bool loadingData = true;

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
            if (PuTTYLauncher.Settings == null || PuTTYLauncher.Settings.Profiles == null)
            {
                MessageBox.Show("Settings not loaded or no profiles defined", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }

            // Add profiles to the context menu
            foreach (var profile in PuTTYLauncher.Settings.Profiles)
            {
                Icon? appIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
                if (appIcon != null)
                {
                    Image appImage = appIcon.ToBitmap();

                    _contextMenu.Items.Add(profile.Name, appImage, (s, e) =>
                    {
                        PuTTYLauncher.LaunchPutty(profile);
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

            checkRunAtLogin.Checked = PuTTYLauncher.Settings.RunAtLogin;

            // Load PuTTY sessions into the session picker combo box
            comboBoxPuttySession.Items.Add("(none)");
            foreach (var session in PuTTYLauncher.GetPuttySessions())
            {
                comboBoxPuttySession.Items.Add(session);
            }
            comboBoxPuttySession.SelectedIndex = 0;

            // Load profiles into the list box
            if (PuTTYLauncher.Settings.Profiles.Count > 0)
            {
                foreach (var profile in PuTTYLauncher.Settings.Profiles)
                {
                    listBoxProfiles.Items.Add(profile.Name);
                }

                listBoxProfiles.SelectedIndex = 0;
            }

            loadingData = false;
        }

        /// <summary>
        /// The user picked a different profile from the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBoxProfiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (PuTTYLauncher.Settings == null || listBoxProfiles.SelectedItem == null)
                return;

            // Find the selected connection profile
            _selectedProfile = PuTTYLauncher.Settings.Profiles.FirstOrDefault(p => p.Name.Equals(listBoxProfiles.SelectedItem));

            // Can't happen, but suppresses "might be null" warnings
            if (_selectedProfile == null)
                return;

            // And create a backup copy of the profile so we can detect changes
            _selectedProfileBackup = _selectedProfile.Copy();

            // Suppress save button processing while we load the profile data
            loadingData = true;

            if (_selectedProfile.Name.Equals("Default Settings"))
            {
                textBoxProfileName.Text = _selectedProfile.Name;
                comboBoxPuttySession.SelectedItem = _selectedProfile.Name;
                textBoxUsername.Text = String.Empty;
                textBoxPassword.Text = String.Empty;
                textBoxProfileName.Enabled = false;
                comboBoxPuttySession.Enabled = false;
                textBoxUsername.Enabled = false;
                textBoxPassword.Enabled = false;
                btnDeleteProfile.Enabled = false;
            }
            else
            {
                textBoxProfileName.Text = _selectedProfile.Name;
                comboBoxPuttySession.SelectedItem = _selectedProfile.Session;
                textBoxUsername.Text = _selectedProfile.User;
                textBoxPassword.Text = _selectedProfile.Password;
                textBoxProfileName.Enabled = true;
                comboBoxPuttySession.Enabled = true;
                textBoxUsername.Enabled = true;
                textBoxPassword.Enabled = true;
                btnDeleteProfile.Enabled = true;
            }

            loadingData = false;
        }

        /// <summary>
        /// The user changed the auto-start setting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxAutoStart_CheckedChanged(object sender, EventArgs e)
        {
            if (PuTTYLauncher.Settings != null)
            {
                PuTTYLauncher.Settings.RunAtLogin = checkRunAtLogin.Checked;
            }
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
        /// The user clicked the Exit menu item. Close the application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnExit(object? sender, EventArgs? e)
        {
            _notifyIcon.Visible = false;
            Application.Exit();
        }

        // Field editing event handlers and logic -----------------------------

        private void textBoxProfileName_TextChanged(object sender, EventArgs e)
        {
            if (_selectedProfile != null && !loadingData)
            {
                _selectedProfile.Name = textBoxProfileName.Text;
                maybeSaveStatus();
            }
        }

        private void comboBoxPuttySession_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_selectedProfile != null && !loadingData)
            {
                _selectedProfile.Session = comboBoxPuttySession.SelectedText;
                maybeSaveStatus();
            }
        }

        private void textBoxUsername_TextChanged(object sender, EventArgs e)
        {
            if (_selectedProfile != null && !loadingData)
            {
                _selectedProfile.User = textBoxUsername.Text;
                maybeSaveStatus();
            }
        }

        private void textBoxPassword_TextChanged(object sender, EventArgs e)
        {
            if (_selectedProfile != null && !loadingData)
            {
                _selectedProfile.Password = textBoxPassword.Text;
                maybeSaveStatus();
            }
        }

        private void maybeSaveStatus()
        {
            if (!loadingData && listBoxProfiles.SelectedItem != null && _selectedProfile != null && _selectedProfileBackup != null && PuTTYLauncher.Settings != null)
            {
                if (!_selectedProfile.IsSameAs(_selectedProfileBackup))
                {
                    //Save
                    PuTTYLauncher.Settings.SaveToFile();
                }
            }
        }

        // List double-ckick event handler ------------------------------------

        /// <summary>
        /// The user double-clicked a profile in the list. Launch PuTTY with the selected profile.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBoxProfiles_DoubleClick(object sender, EventArgs e)
        {
            launchSelectedProfile();
        }

        private void launchSelectedProfile()
        {
            if (listBoxProfiles.SelectedItem != null && listBoxProfiles.SelectedIndex != -1 && PuTTYLauncher.Settings != null)
            {
                var selectedProfile = PuTTYLauncher.Settings.Profiles.FirstOrDefault(p => p.Name.Equals(listBoxProfiles.SelectedItem));
                if (selectedProfile != null)
                    PuTTYLauncher.LaunchPutty(selectedProfile);
            }
        }

        // Button click event handlers ----------------------------------------

        private void btnOpen_Click(object sender, EventArgs e)
        {
            launchSelectedProfile();
        }

        private void btnNewProfile_Click(object sender, EventArgs e)
        {
            MessageBox.Show("New profile not implemented", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnDeleteProfile_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Delete profile not implemented", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
