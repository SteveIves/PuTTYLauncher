using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PuTTYLauncher
{
    public partial class LicenseForm : Form
    {
        public LicenseForm()
        {
            InitializeComponent();
        }

        private void LicenseForm_Load(object sender, EventArgs e)
        {
            Text = $"{Application.ProductName} License";
            textBoxLicense.SelectionStart = 0 ;
            textBoxLicense.SelectionLength = 0 ;
        }
    }
}
