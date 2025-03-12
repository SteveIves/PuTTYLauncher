namespace PuTTYLauncher
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            groupBoxSettings = new GroupBox();
            checkStartInTray = new CheckBox();
            checkRunAtLogin = new CheckBox();
            groupBoxMain = new GroupBox();
            checkBoxShowPassword = new CheckBox();
            listViewProfiles = new ListView();
            btnOpenSave = new Button();
            btnNewCancel = new Button();
            btnDeleteProfile = new Button();
            textBoxPassword = new TextBox();
            textBoxUsername = new TextBox();
            comboBoxPuttySession = new ComboBox();
            textBoxProfileName = new TextBox();
            lblPassword = new Label();
            lblUsername = new Label();
            lblPuttySession = new Label();
            lblProfileName = new Label();
            groupBoxSettings.SuspendLayout();
            groupBoxMain.SuspendLayout();
            SuspendLayout();
            // 
            // groupBoxSettings
            // 
            groupBoxSettings.Controls.Add(checkStartInTray);
            groupBoxSettings.Controls.Add(checkRunAtLogin);
            groupBoxSettings.Location = new Point(12, 0);
            groupBoxSettings.Name = "groupBoxSettings";
            groupBoxSettings.Size = new Size(958, 81);
            groupBoxSettings.TabIndex = 0;
            groupBoxSettings.TabStop = false;
            groupBoxSettings.Text = "Settings";
            // 
            // checkStartInTray
            // 
            checkStartInTray.AutoSize = true;
            checkStartInTray.Location = new Point(170, 37);
            checkStartInTray.Name = "checkStartInTray";
            checkStartInTray.Size = new Size(156, 24);
            checkStartInTray.TabIndex = 1;
            checkStartInTray.Text = "Start in system tray";
            checkStartInTray.UseVisualStyleBackColor = true;
            checkStartInTray.CheckedChanged += checkStartInTray_CheckedChanged;
            // 
            // checkRunAtLogin
            // 
            checkRunAtLogin.AutoSize = true;
            checkRunAtLogin.Location = new Point(35, 37);
            checkRunAtLogin.Name = "checkRunAtLogin";
            checkRunAtLogin.Size = new Size(111, 24);
            checkRunAtLogin.TabIndex = 0;
            checkRunAtLogin.Text = "Run at login";
            checkRunAtLogin.UseVisualStyleBackColor = true;
            checkRunAtLogin.CheckedChanged += checkBoxAutoStart_CheckedChanged;
            // 
            // groupBoxMain
            // 
            groupBoxMain.Controls.Add(checkBoxShowPassword);
            groupBoxMain.Controls.Add(listViewProfiles);
            groupBoxMain.Controls.Add(btnOpenSave);
            groupBoxMain.Controls.Add(btnNewCancel);
            groupBoxMain.Controls.Add(btnDeleteProfile);
            groupBoxMain.Controls.Add(textBoxPassword);
            groupBoxMain.Controls.Add(textBoxUsername);
            groupBoxMain.Controls.Add(comboBoxPuttySession);
            groupBoxMain.Controls.Add(textBoxProfileName);
            groupBoxMain.Controls.Add(lblPassword);
            groupBoxMain.Controls.Add(lblUsername);
            groupBoxMain.Controls.Add(lblPuttySession);
            groupBoxMain.Controls.Add(lblProfileName);
            groupBoxMain.Location = new Point(12, 87);
            groupBoxMain.Name = "groupBoxMain";
            groupBoxMain.Size = new Size(958, 454);
            groupBoxMain.TabIndex = 2;
            groupBoxMain.TabStop = false;
            groupBoxMain.Text = "Connection Profiles";
            // 
            // checkBoxShowPassword
            // 
            checkBoxShowPassword.AutoSize = true;
            checkBoxShowPassword.Location = new Point(794, 166);
            checkBoxShowPassword.Name = "checkBoxShowPassword";
            checkBoxShowPassword.Size = new Size(132, 24);
            checkBoxShowPassword.TabIndex = 7;
            checkBoxShowPassword.Text = "Show &Password";
            checkBoxShowPassword.UseVisualStyleBackColor = true;
            checkBoxShowPassword.CheckedChanged += checkBoxShowPassword_CheckedChanged;
            // 
            // listViewProfiles
            // 
            listViewProfiles.FullRowSelect = true;
            listViewProfiles.HeaderStyle = ColumnHeaderStyle.None;
            listViewProfiles.Location = new Point(16, 33);
            listViewProfiles.MultiSelect = false;
            listViewProfiles.Name = "listViewProfiles";
            listViewProfiles.Size = new Size(363, 393);
            listViewProfiles.TabIndex = 2;
            listViewProfiles.UseCompatibleStateImageBehavior = false;
            listViewProfiles.View = View.Details;
            listViewProfiles.SelectedIndexChanged += listViewProfiles_SelectedIndexChanged;
            listViewProfiles.DoubleClick += listViewProfiles_DoubleClick;
            // 
            // btnOpenSave
            // 
            btnOpenSave.Location = new Point(523, 212);
            btnOpenSave.Name = "btnOpenSave";
            btnOpenSave.Size = new Size(94, 29);
            btnOpenSave.TabIndex = 8;
            btnOpenSave.Text = "&Open";
            btnOpenSave.UseVisualStyleBackColor = true;
            btnOpenSave.Click += btnOpenSave_Click;
            // 
            // btnNewCancel
            // 
            btnNewCancel.Location = new Point(623, 212);
            btnNewCancel.Name = "btnNewCancel";
            btnNewCancel.Size = new Size(94, 29);
            btnNewCancel.TabIndex = 9;
            btnNewCancel.Text = "&New";
            btnNewCancel.UseVisualStyleBackColor = true;
            btnNewCancel.Click += btnNewCancel_Click;
            // 
            // btnDeleteProfile
            // 
            btnDeleteProfile.Enabled = false;
            btnDeleteProfile.Location = new Point(832, 212);
            btnDeleteProfile.Name = "btnDeleteProfile";
            btnDeleteProfile.Size = new Size(94, 29);
            btnDeleteProfile.TabIndex = 10;
            btnDeleteProfile.Text = "&Delete";
            btnDeleteProfile.UseVisualStyleBackColor = true;
            btnDeleteProfile.Click += btnDeleteProfile_Click;
            // 
            // textBoxPassword
            // 
            textBoxPassword.Location = new Point(523, 133);
            textBoxPassword.Name = "textBoxPassword";
            textBoxPassword.PasswordChar = '*';
            textBoxPassword.Size = new Size(403, 27);
            textBoxPassword.TabIndex = 6;
            textBoxPassword.TextChanged += textBoxPassword_TextChanged;
            // 
            // textBoxUsername
            // 
            textBoxUsername.Location = new Point(523, 100);
            textBoxUsername.Name = "textBoxUsername";
            textBoxUsername.Size = new Size(403, 27);
            textBoxUsername.TabIndex = 5;
            textBoxUsername.TextChanged += textBoxUsername_TextChanged;
            // 
            // comboBoxPuttySession
            // 
            comboBoxPuttySession.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxPuttySession.FormattingEnabled = true;
            comboBoxPuttySession.Location = new Point(523, 66);
            comboBoxPuttySession.Name = "comboBoxPuttySession";
            comboBoxPuttySession.Size = new Size(403, 28);
            comboBoxPuttySession.TabIndex = 4;
            comboBoxPuttySession.SelectedIndexChanged += comboBoxPuttySession_SelectedIndexChanged;
            // 
            // textBoxProfileName
            // 
            textBoxProfileName.Location = new Point(523, 33);
            textBoxProfileName.Name = "textBoxProfileName";
            textBoxProfileName.Size = new Size(403, 27);
            textBoxProfileName.TabIndex = 3;
            textBoxProfileName.TextChanged += textBoxProfileName_TextChanged;
            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.Location = new Point(403, 136);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(70, 20);
            lblPassword.TabIndex = 5;
            lblPassword.Text = "Password";
            // 
            // lblUsername
            // 
            lblUsername.AutoSize = true;
            lblUsername.Location = new Point(403, 103);
            lblUsername.Name = "lblUsername";
            lblUsername.Size = new Size(75, 20);
            lblUsername.TabIndex = 4;
            lblUsername.Text = "Username";
            // 
            // lblPuttySession
            // 
            lblPuttySession.AutoSize = true;
            lblPuttySession.Location = new Point(403, 69);
            lblPuttySession.Name = "lblPuttySession";
            lblPuttySession.Size = new Size(102, 20);
            lblPuttySession.TabIndex = 3;
            lblPuttySession.Text = "PuTTY Session";
            // 
            // lblProfileName
            // 
            lblProfileName.AutoSize = true;
            lblProfileName.Location = new Point(403, 36);
            lblProfileName.Name = "lblProfileName";
            lblProfileName.Size = new Size(93, 20);
            lblProfileName.TabIndex = 2;
            lblProfileName.Text = "Profile name";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(982, 553);
            Controls.Add(groupBoxMain);
            Controls.Add(groupBoxSettings);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MainForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "PuTTY Launcher";
            WindowState = FormWindowState.Minimized;
            Activated += MainForm_Activated;
            FormClosing += MainForm_FormClosing;
            groupBoxSettings.ResumeLayout(false);
            groupBoxSettings.PerformLayout();
            groupBoxMain.ResumeLayout(false);
            groupBoxMain.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBoxSettings;
        private CheckBox checkRunAtLogin;
        private GroupBox groupBoxMain;
        private Label lblProfileName;
        private Label lblPassword;
        private Label lblUsername;
        private Label lblPuttySession;
        private TextBox textBoxPassword;
        private TextBox textBoxUsername;
        private ComboBox comboBoxPuttySession;
        private TextBox textBoxProfileName;
        private Button btnDeleteProfile;
        private Button btnNewCancel;
        private Button btnOpenSave;
        private ListView listViewProfiles;
        private CheckBox checkBoxShowPassword;
        private CheckBox checkStartInTray;
    }
}
