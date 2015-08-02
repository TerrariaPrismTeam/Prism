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
    public partial class ExceptionDialogForm : Form
    {
        public ExceptionDialogForm(string title, string msg)
        {
            InitializeComponent();

            this.Text = title;
            textboxDisplayLoaderErrors.Text = msg;   
        }

        private void buttonHastebin_Click(object sender, EventArgs e)
        {
            string url = Util.HastebinHelper.QuickUpload(textboxDisplayLoaderErrors.Text);
            if (url != null)
            {
                using (var dlg = new HastebinResultDialogForm(url))
                {
                    Clipboard.SetText(url);
                    dlg.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("Unable to match Hastebin request regex. They may have changed the way uploading to the site works...", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }          
        }

        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
        }

        private void richTextBox1_ContentsResized(object sender, ContentsResizedEventArgs e)
        {
            
        }
    }
}
