using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;

namespace Prism.WinFormDialogs
{
    public partial class HastebinResultDialogForm : Form
    {
        readonly string url;

        public HastebinResultDialogForm(string hbUrl)
        {
            InitializeComponent();

            textBoxURL.Text = url = hbUrl;
        }

        void button2_Click(object sender, EventArgs e)
        {
            Process.Start(url);
        }
    }
}
