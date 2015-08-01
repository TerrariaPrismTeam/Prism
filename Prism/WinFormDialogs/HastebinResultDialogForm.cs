using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Prism.WinFormDialogs
{
    public partial class HastebinResultDialogForm : Form
    {
        string url;

        public HastebinResultDialogForm(string hbUrl)
        {
            InitializeComponent();

            textBoxURL.Text = url = hbUrl;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Process.Start(url);
        }
    }
}
