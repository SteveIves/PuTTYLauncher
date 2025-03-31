
using System.Diagnostics;

namespace PuTTYLauncher
{
    public partial class MainForm : Form
    {
        private NotifyIcon notifyIcon;
        private ContextMenuStrip contextMenu;
        private ConnectionProfile? selectedProfile;
        private ConnectionProfile? selectedProfileBackup;
        private ListViewItem? pendingListViewItem;
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
                Icon = appIcon,
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

            // Set the initial state of the auto-start and start in tray checkboxes

            checkRunAtLogin.Checked = PuTTYLauncher.Settings.RunAtLogin;
            checkStartInTray.Checked = PuTTYLauncher.Settings.StartInTray;

            // Show the current PuTTY path

            textBoxPuTTYPath.Text = PuTTYLauncher.Settings.PuTTYPath;

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

            // Add a separator and an Edit profiles menu item
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add("Edit profiles", null, (s, e) =>
            {
                Show();
                WindowState = FormWindowState.Normal;
                ShowInTaskbar = true;
            });


            // Add a separator and an Exit menu item
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add("Exit", null, (s, e) =>
            {
                notifyIcon.Visible = false;
                Application.Exit();
            });

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

            //Configure the URIs for the LinkLabels

            linkLabelProjectHome.Links.Add(
                new LinkLabel.Link()
                {
                    Name = "documentation",
                    LinkData = "https://github.com/SteveIves/PuTTYLauncher",
                    Start = 0,
                    Length = linkLabelProjectHome.Text.Length
                });

            linkLabelDownloads.Links.Add(
                new LinkLabel.Link()
                {
                    Name = "downloads",
                    LinkData = "https://github.com/SteveIves/PuTTYLauncher/releases",
                    Start = 0,
                    Length = linkLabelDownloads.Text.Length
                });

            linkLabelLicense.Links.Add(
                new LinkLabel.Link()
                {
                    Name = "license",
                    LinkData = "https://raw.githubusercontent.com/SteveIves/PuTTYLauncher/refs/heads/master/LICENSE.txt",
                    Start = 0,
                    Length = linkLabelLicense.Text.Length
                });

            //Set the initial window visibility
            ShowInTaskbar = !checkStartInTray.Checked;
            WindowState = checkStartInTray.Checked ? FormWindowState.Minimized : FormWindowState.Normal;

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
        /// The user changed the start in tray setting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkStartInTray_CheckedChanged(object sender, EventArgs e)
        {
            if (PuTTYLauncher.Settings != null)
            {
                PuTTYLauncher.Settings.StartInTray = checkStartInTray.Checked;
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

        // Field editing event handlers and logic -----------------------------

        /// <summary>
        /// The user changed the profile name field. Save it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxProfileName_TextChanged(object sender, EventArgs e)
        {
            if (selectedProfile != null && listViewProfiles.SelectedItems.Count == 1 && !loadingData && !newProfileMode)
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
            if (selectedProfile != null && !loadingData && comboBoxPuttySession.SelectedItem != null && !newProfileMode)
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
            if (selectedProfile != null && !loadingData && !newProfileMode)
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
            if (selectedProfile != null && !loadingData && !newProfileMode)
            {
                selectedProfile.Password = DPAPIEncryption.Encrypt(textBoxPassword.Text);
                maybeSaveStatus();
            }
        }

        /// <summary>
        /// The user changed the PuTTY path value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxPuTTYPath_TextChanged(object sender, EventArgs e)
        {
            if (PuTTYLauncher.Settings != null)
            {
                if (File.Exists(textBoxPuTTYPath.Text))
                {
                    PuTTYLauncher.Settings.PuTTYPath = textBoxPuTTYPath.Text;
                }
            }
        }

        /// <summary>
        /// Check if the profile has changed and save it if it has
        /// </summary>
        /// <returns></returns>
        private bool maybeSaveStatus()
        {
            bool saved = false;
            if (!loadingData && !newProfileMode && listViewProfiles.SelectedItems.Count == 1 && selectedProfile != null && selectedProfileBackup != null && PuTTYLauncher.Settings != null)
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
                //Save new profile button clicked

                //Can't happen, but suppresses "might be null" warnings
                if (pendingListViewItem == null || selectedProfile == null || PuTTYLauncher.Settings == null)
                    return;

                //Create the new profile
                selectedProfile = new ConnectionProfile()
                {
                    Name = textBoxProfileName.Text,
                    Session = comboBoxPuttySession.Text,
                    User = textBoxUsername.Text,
                    Password = DPAPIEncryption.Encrypt(textBoxPassword.Text)
                };

                //Add the new profile to settings and save to disk
                PuTTYLauncher.Settings.Profiles.Add(selectedProfile);
                PuTTYLauncher.Settings.SaveToFile();

                //Find the index of the first ToolStripSeparator in the context menu
                int newItemIndex = contextMenu.Items
                    .OfType<ToolStripSeparator>()
                    .Select(item => contextMenu.Items.IndexOf(item))
                    .FirstOrDefault();

                // If no separator is found (can't happen) add to the end
                if (newItemIndex == 0 && !(contextMenu.Items[0] is ToolStripSeparator))
                    newItemIndex = contextMenu.Items.Count;

                // Add profile to the context menu
                contextMenu.Items.Insert(newItemIndex,
                    new ToolStripMenuItem(
                    selectedProfile.Name,
                    appImage,
                        (s, e) => { PuTTYLauncher.LaunchPutty(selectedProfile); },
                        selectedProfile.Key
                        )
                    );

                //Copy the new profile so we can detect changes later
                selectedProfileBackup = selectedProfile.Copy();

                //Update the information in the "New Profile" list item added earlier
                pendingListViewItem.Name = selectedProfile.Key;
                pendingListViewItem.Text = selectedProfile.Name;
                pendingListViewItem = null;

                //Restore normal UI updating when the selected profile changes
                newProfileMode = false;
                listViewProfiles.SelectedIndexChanged += listViewProfiles_SelectedIndexChanged;

                //Reset the UI to normal
                btnDeleteProfile.Enabled = true;
                btnOpenSave.Text = "Open";
                btnNewCancel.Text = "New";

                //Re-enable and focus the prodiles list.
                listViewProfiles.Enabled = true;
                listViewProfiles.Focus();
            }
            else
            {
                //Open profile button clicked
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
                // New profile button clicked

                newProfileMode = true;
                listViewProfiles.SelectedIndexChanged -= listViewProfiles_SelectedIndexChanged;

                listViewProfiles.Enabled = false;

                pendingListViewItem = listViewProfiles.Items.Add(
                    new ListViewItem()
                    {
                        Name = Guid.NewGuid().ToString(),
                        Text = "New Profile"
                    });
                pendingListViewItem.Selected = true;
                pendingListViewItem.EnsureVisible();

                textBoxProfileName.Text = pendingListViewItem.Text;
                comboBoxPuttySession.SelectedIndex = 0;
                textBoxUsername.Text = String.Empty;
                textBoxPassword.Text = String.Empty;

                textBoxProfileName.Enabled = true;
                comboBoxPuttySession.Enabled = true;
                textBoxUsername.Enabled = true;
                textBoxPassword.Enabled = true;

                btnDeleteProfile.Enabled = false;
                btnOpenSave.Text = "Save";
                btnNewCancel.Text = "Cancel";
                textBoxProfileName.Focus();
            }
            else
            {
                //Cancel new profile button clicked

                //Can't happen, but suppresses "might be null" warnings
                if (pendingListViewItem == null || selectedProfile == null)
                    return;

                //Remove the new profile from the list
                listViewProfiles.SelectedItems.Clear();
                listViewProfiles.Items.Remove(pendingListViewItem);

                //Find and select the previously selected profile
                newProfileMode = false;
                listViewProfiles.SelectedIndexChanged += listViewProfiles_SelectedIndexChanged;
                var previouslySelectedListItems = listViewProfiles.Items.Find(selectedProfile.Key, false);
                previouslySelectedListItems[0].Selected = true;
                previouslySelectedListItems[0].EnsureVisible();

                btnDeleteProfile.Enabled = true;
                btnOpenSave.Text = "Open";
                btnNewCancel.Text = "New";

                listViewProfiles.Enabled = true;
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
            //Can't happen, but suppresses "might be null" warnings
            if (selectedProfile == null || PuTTYLauncher.Settings == null)
                return;

            if (MessageBox.Show(
                "Are you sure you want to delete this profile?",
                Application.ProductName,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                //Remove the profile from the context meuu
                contextMenu.Items.RemoveByKey(selectedProfile.Key);

                //Get the index of the item to be deleted
                var idxToRemove = listViewProfiles.Items.IndexOfKey(selectedProfile.Key);

                //Calculate the index of the item to be selected after the current item is removed
                //We don't need to worry about index 0 because the Delete button will not be enabled
                int newSelectedIndex = idxToRemove;
                if (idxToRemove > listViewProfiles.Items.Count - 2)
                    newSelectedIndex = listViewProfiles.Items.Count - 2;

                //Delete the profile from the settings file
                PuTTYLauncher.Settings.Profiles.Remove(selectedProfile);
                PuTTYLauncher.Settings.SaveToFile();

                //Remove the profile from the list
                listViewProfiles.Items.RemoveAt(idxToRemove);

                //Select the next profile
                listViewProfiles.Items[newSelectedIndex].Selected = true;
                listViewProfiles.Items[newSelectedIndex].EnsureVisible();

                // Record the newly selected profile
                selectedProfile = PuTTYLauncher.Settings.Profiles.FirstOrDefault(p => p.Key.Equals(listViewProfiles.SelectedItems[0].Name));

                // Can't happen, but suppresses "might be null" warnings
                if (selectedProfile == null)
                    return;

                // And create a backup copy of the profile so we can detect changes
                selectedProfileBackup = selectedProfile.Copy();

                listViewProfiles.Focus();

            }
        }

        private void buttonFindPuTTYPath_Click(object sender, EventArgs e)
        {
            string defaultFolder = PuTTYLauncher.GetDefaultPuTTYFolder();
            string defaultFile = Path.Combine(defaultFolder, "putty.exe");

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = defaultFolder;
                openFileDialog.Filter = "PuTTY Executable (putty.exe)|putty.exe";
                openFileDialog.Title = "Locate putty.exe";

                // If putty.exe exists in the default location, pre-select it
                if (File.Exists(defaultFile))
                {
                    openFileDialog.FileName = defaultFile;
                }

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    textBoxPuTTYPath.Text = openFileDialog.FileName;
                }
            }
        }

        private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Link != null && e.Link.LinkData != null)
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = e.Link.LinkData.ToString(),
                        UseShellExecute = true
                    });
                }
                catch
                {
                    MessageBox.Show($"Unable to open {e.Link.Name} link.",
                        Application.ProductName,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }
        private void linkLabelLicense_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var dlg = new LicenseForm();
            dlg.ShowDialog();
        }

        private void linkLabelAbout_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var dlg = new AboutForm();
            dlg.ShowDialog();
        }
    }
}
