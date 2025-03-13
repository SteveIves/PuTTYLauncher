namespace PuTTYLauncher
{
    partial class LicenseForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LicenseForm));
            textBoxLicense = new TextBox();
            SuspendLayout();
            // 
            // textBoxLicense
            // 
            textBoxLicense.Font = new Font("Courier New", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBoxLicense.Location = new Point(12, 12);
            textBoxLicense.Multiline = true;
            textBoxLicense.Name = "textBoxLicense";
            textBoxLicense.ReadOnly = true;
            textBoxLicense.Size = new Size(718, 534);
            textBoxLicense.TabIndex = 0;
            textBoxLicense.Text = resources.GetString("textBoxLicense.Text");
            // 
            // LicenseForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(731, 558);
            Controls.Add(textBoxLicense);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "LicenseForm";
            StartPosition = FormStartPosition.CenterParent;
            Load += LicenseForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBoxLicense;
    }
}