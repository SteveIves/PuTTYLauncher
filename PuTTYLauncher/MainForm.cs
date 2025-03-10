
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Collections.Generic;

namespace PuTTYLauncher
{
    public partial class MainForm : Form
    {
        private NotifyIcon notifyIcon;
        private ContextMenuStrip contextMenu;
        private ConnectionProfile? selectedProfile;
        private ConnectionProfile? selectedProfileBackup;
        private bool loadingData = true;
        private bool newProfileMode = false;
        private Icon? appIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        Image? appImage;

        /// <summary>
        /// The main form constructor
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            contextMenu = new ContextMenuStrip();

            // Create a system tray icon
            notifyIcon = new NotifyIcon
            {
                Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath),
                Text = "PuTTY Launcher",
                Visible = true
            };

            // Can't happen, but suppresses "might be null" warnings
            if (PuTTYLauncher.Settings == null || PuTTYLauncher.Settings.Profiles == null)
            {
                MessageBox.Show("Settings not loaded or no profiles defined", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }

            // Set the initial state of the auto-start checkbox

            checkRunAtLogin.Checked = PuTTYLauncher.Settings.RunAtLogin;

            // Load PuTTY sessions into the session picker combo box

            comboBoxPuttySession.Items.Add("(none)");
            foreach (var session in PuTTYLauncher.GetPuttySessions())
            {
                comboBoxPuttySession.Items.Add(session);
            }
            comboBoxPuttySession.SelectedIndex = 0;

            // Load profiles

            appImage = appIcon?.ToBitmap();

            listViewProfiles.Columns.Add("", listViewProfiles.Width - 4);

            if (PuTTYLauncher.Settings.Profiles.Count > 0)
            {
                foreach (var profile in PuTTYLauncher.Settings.Profiles)
                {
                    // Add profile to the context menu
                    contextMenu.Items.Add(
                        new ToolStripMenuItem(
                            profile.Name,
                            appImage,
                            (s, e) => { PuTTYLauncher.LaunchPutty(profile); },
                            profile.Key
                            )
                        );

                    // Add profile to the profiles list
                    listViewProfiles.Items.Add(
                        new ListViewItem()
                        {
                            Name = profile.Key,
                            Text = profile.Name
                        });
                }

                // Pick a profile to start selected
                if (listViewProfiles.Items.Count > 1 && listViewProfiles.Items[0].Text.Equals("Default Settings"))
                {
                    listViewProfiles.Items[1].Selected = true;
                }
                else if (listViewProfiles.Items.Count > 0)
                {
                    listViewProfiles.Items[0].Selected = true;
                }
            }

            // Add a separator and an Exit menu item
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add("Exit", null, OnExit);

            // Assign the context menu to the notify icon
            notifyIcon.ContextMenuStrip = contextMenu;

            // Handle double-click
            notifyIcon.DoubleClick += (s, e) =>
            {
                // Show the main window
                Show();
                WindowState = FormWindowState.Normal;
                ShowInTaskbar = true;
            };

            loadingData = false;
        }

        /// <summary>
        /// The form was activated. Set focus to the profiles list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Activated(object sender, EventArgs e)
        {
            listViewProfiles.Focus();
        }

        /// <summary>
        /// The user picked a different profile from the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listViewProfiles_SelectedIndexChanged(object? sender, EventArgs? e)
        {
            if (PuTTYLauncher.Settings == null || listViewProfiles.SelectedItems.Count != 1)
                return;

            // Find the selected connection profile
            selectedProfile = PuTTYLauncher.Settings.Profiles.FirstOrDefault(p => p.Name.Equals(listViewProfiles.SelectedItems[0].Text));

            // Can't happen, but suppresses "might be null" warnings
            if (selectedProfile == null)
                return;

            // And create a backup copy of the profile so we can detect changes
            selectedProfileBackup = selectedProfile.Copy();

            // Suppress save button processing while we load the profile data
            loadingData = true;

            if (selectedProfile.Name.Equals("Default Settings"))
            {
                textBoxProfileName.Text = selectedProfile.Name;
                comboBoxPuttySession.SelectedItem = selectedProfile.Name;
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
                textBoxProfileName.Text = selectedProfile.Name;
                comboBoxPuttySession.SelectedItem = selectedProfile.Session;
                textBoxUsername.Text = selectedProfile.User;
                textBoxPassword.Text = DPAPIEncryption.Decrypt(selectedProfile.Password);
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
            notifyIcon.Visible = false;
            Application.Exit();
        }

        // Field editing event handlers and logic -----------------------------

        /// <summary>
        /// The user changed the profile name field. Save it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxProfileName_TextChanged(object sender, EventArgs e)
        {
            if (selectedProfile != null && listViewProfiles.SelectedItems.Count == 1 && !loadingData)
            {
                string oldName = selectedProfile.Name;

                selectedProfile.Name = textBoxProfileName.Text;

                if (maybeSaveStatus())
                {
                    //Profile name was changed and saved

                    //Update the list box
                    listViewProfiles.SelectedIndexChanged -= listViewProfiles_SelectedIndexChanged;
                    listViewProfiles.SelectedItems[0].Text = selectedProfile.Name;
                    listViewProfiles.SelectedIndexChanged += listViewProfiles_SelectedIndexChanged;

                    //And update the context menu
                    ToolStripItem? contextMenuItem = contextMenu.Items.Find(listViewProfiles.SelectedItems[0].Name, false)[0];
                    if (contextMenuItem != null)
                    {
                        int idx = contextMenu.Items.IndexOf(contextMenuItem);
                        contextMenu.Items[idx].Text = selectedProfile.Name;
                    }
                }
            }
        }

        /// <summary>
        /// The user changed the PuTTY session field. Save it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxPuttySession_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectedProfile != null && !loadingData && comboBoxPuttySession.SelectedItem != null)
            {
                selectedProfile.Session = (string)comboBoxPuttySession.SelectedItem;
                maybeSaveStatus();
            }
        }

        /// <summary>
        /// The user changed the username field. Save it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxUsername_TextChanged(object sender, EventArgs e)
        {
            if (selectedProfile != null && !loadingData)
            {
                selectedProfile.User = textBoxUsername.Text;
                maybeSaveStatus();
            }
        }

        /// <summary>
        /// The user changed the password field. Encrypt the password and save it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxPassword_TextChanged(object sender, EventArgs e)
        {
            if (selectedProfile != null && !loadingData)
            {
                selectedProfile.Password = DPAPIEncryption.Encrypt(textBoxPassword.Text);
                maybeSaveStatus();
            }
        }

        /// <summary>
        /// Check if the profile has changed and save it if it has
        /// </summary>
        /// <returns></returns>
        private bool maybeSaveStatus()
        {
            bool saved = false;
            if (!loadingData && listViewProfiles.SelectedItems.Count == 1 && selectedProfile != null && selectedProfileBackup != null && PuTTYLauncher.Settings != null)
            {
                if (!selectedProfile.IsSameAs(selectedProfileBackup))
                {
                    //Save settings
                    saved = PuTTYLauncher.Settings.SaveToFile();
                }
            }
            return saved;
        }

        // List double-ckick event handler ------------------------------------

        /// <summary>
        /// The user double-clicked a profile in the list. Launch PuTTY with the selected profile.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listViewProfiles_DoubleClick(object sender, EventArgs e)
        {
            launchSelectedProfile();
        }

        private void launchSelectedProfile()
        {
            if (listViewProfiles.SelectedItems.Count == 1 && PuTTYLauncher.Settings != null)
            {
                var selectedProfile = PuTTYLauncher.Settings.Profiles.FirstOrDefault(p => p.Name.Equals(listViewProfiles.SelectedItems[0].Text));
                if (selectedProfile != null)
                    PuTTYLauncher.LaunchPutty(selectedProfile);
            }
        }

        // Button click event handlers ----------------------------------------

        /// <summary>
        /// The user clicked the "Open" or "Save" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenSave_Click(object sender, EventArgs e)
        {
            if (newProfileMode)
            {
                //We're a Save new profile button

            }
            else
            {
                //We're an Open profile button
                launchSelectedProfile();
            }
        }

        /// <summary>
        /// The user clicked the "New" or "Cancel" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNewCancel_Click(object sender, EventArgs e)
        {
            if (!newProfileMode)
            {
                newProfileMode = true;

                listViewProfiles.Enabled = false;

                textBoxProfileName.Text = String.Empty;
                comboBoxPuttySession.SelectedIndex = 0;
                textBoxUsername.Text = String.Empty;
                textBoxPassword.Text = String.Empty;

                textBoxProfileName.Enabled = true;
                comboBoxPuttySession.Enabled = true;
                textBoxUsername.Enabled = true;
                textBoxPassword.Enabled = true;

                btnOpenSave.Enabled = false;
                btnDeleteProfile.Enabled = false;
                btnOpenSave.Text = "Save";
                btnNewCancel.Text = "Cancel";
                textBoxProfileName.Focus();
            }
            else
            {
                //Validate

                //Save


                //Reset
                newProfileMode = false;
                textBoxProfileName.Text = selectedProfile?.Name;
                comboBoxPuttySession.SelectedItem = selectedProfile?.Session;
                textBoxUsername.Text = selectedProfile?.User;
                if (selectedProfile?.Password != null)
                    textBoxPassword.Text = DPAPIEncryption.Decrypt(selectedProfile.Password);
                comboBoxPuttySession.SelectedIndex = 0;
                textBoxUsername.Text = String.Empty;
                textBoxPassword.Text = String.Empty;
                listViewProfiles.Enabled = true;
                btnOpenSave.Enabled = true;
                btnDeleteProfile.Enabled = true;
                btnOpenSave.Text = "Open";
                btnNewCancel.Text = "New";
                listViewProfiles.Focus();
            }
        }

        /// <summary>
        /// The user clicked the "Delete Profile" button. Delete the selected profile.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeleteProfile_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                "Are you sure you want to delete this profile?",
                Application.ProductName,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                //Delete the profile
                MessageBox.Show(
                    "Well, you can't, because delete profile is not implemented yet!",
                    Application.ProductName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(
                    "Excellent, beause delete profile is not implemented yet!",
                    Application.ProductName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// The user clicked the "Show Password" checkbox. Show or hide the password text.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            textBoxPassword.PasswordChar = checkBoxShowPassword.Checked ? '\0' : '*';
        }
    }
}
