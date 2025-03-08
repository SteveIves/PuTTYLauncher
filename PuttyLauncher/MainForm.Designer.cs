namespace PuttyLauncher
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
            checkRunAtLogin = new CheckBox();
            listBoxProfiles = new ListBox();
            groupBoxMain = new GroupBox();
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
            groupBoxSettings.Controls.Add(checkRunAtLogin);
            groupBoxSettings.Location = new Point(12, 0);
            groupBoxSettings.Name = "groupBoxSettings";
            groupBoxSettings.Size = new Size(958, 81);
            groupBoxSettings.TabIndex = 0;
            groupBoxSettings.TabStop = false;
            groupBoxSettings.Text = "Settings";
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
            // listBoxProfiles
            // 
            listBoxProfiles.FormattingEnabled = true;
            listBoxProfiles.Location = new Point(35, 36);
            listBoxProfiles.Name = "listBoxProfiles";
            listBoxProfiles.Size = new Size(340, 384);
            listBoxProfiles.TabIndex = 1;
            listBoxProfiles.SelectedIndexChanged += listBoxProfiles_SelectedIndexChanged;
            listBoxProfiles.DoubleClick += listBoxProfiles_DoubleClick;
            // 
            // groupBoxMain
            // 
            groupBoxMain.Controls.Add(textBoxPassword);
            groupBoxMain.Controls.Add(textBoxUsername);
            groupBoxMain.Controls.Add(comboBoxPuttySession);
            groupBoxMain.Controls.Add(textBoxProfileName);
            groupBoxMain.Controls.Add(lblPassword);
            groupBoxMain.Controls.Add(lblUsername);
            groupBoxMain.Controls.Add(lblPuttySession);
            groupBoxMain.Controls.Add(lblProfileName);
            groupBoxMain.Controls.Add(listBoxProfiles);
            groupBoxMain.Location = new Point(12, 87);
            groupBoxMain.Name = "groupBoxMain";
            groupBoxMain.Size = new Size(958, 454);
            groupBoxMain.TabIndex = 2;
            groupBoxMain.TabStop = false;
            groupBoxMain.Text = "Connection Profiles";
            // 
            // textBoxPassword
            // 
            textBoxPassword.Location = new Point(523, 133);
            textBoxPassword.Name = "textBoxPassword";
            textBoxPassword.PasswordChar = '*';
            textBoxPassword.Size = new Size(239, 27);
            textBoxPassword.TabIndex = 9;
            textBoxPassword.Enter += textBoxPassword_Enter;
            textBoxPassword.Leave += textBoxPassword_Leave;
            // 
            // textBoxUsername
            // 
            textBoxUsername.Location = new Point(523, 100);
            textBoxUsername.Name = "textBoxUsername";
            textBoxUsername.Size = new Size(239, 27);
            textBoxUsername.TabIndex = 8;
            textBoxUsername.Enter += textBoxUsername_Enter;
            textBoxUsername.Leave += textBoxUsername_Leave;
            // 
            // comboBoxPuttySession
            // 
            comboBoxPuttySession.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxPuttySession.FormattingEnabled = true;
            comboBoxPuttySession.Location = new Point(523, 66);
            comboBoxPuttySession.Name = "comboBoxPuttySession";
            comboBoxPuttySession.Size = new Size(239, 28);
            comboBoxPuttySession.TabIndex = 7;
            comboBoxPuttySession.Enter += comboBoxPuttySession_Enter;
            comboBoxPuttySession.Leave += comboBoxPuttySession_Leave;
            // 
            // textBoxProfileName
            // 
            textBoxProfileName.Location = new Point(523, 33);
            textBoxProfileName.Name = "textBoxProfileName";
            textBoxProfileName.Size = new Size(239, 27);
            textBoxProfileName.TabIndex = 6;
            textBoxProfileName.Enter += textBoxProfileName_Enter;
            textBoxProfileName.Leave += textBoxProfileName_Leave;
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
            StartPosition = FormStartPosition.CenterScreen;
            Text = "PuTTY Launcher";
            WindowState = FormWindowState.Minimized;
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
        private ListBox listBoxProfiles;
        private GroupBox groupBoxMain;
        private Label lblProfileName;
        private Label lblPassword;
        private Label lblUsername;
        private Label lblPuttySession;
        private TextBox textBoxPassword;
        private TextBox textBoxUsername;
        private ComboBox comboBoxPuttySession;
        private TextBox textBoxProfileName;
        private Button buttonOpenProfile;
    }
}
