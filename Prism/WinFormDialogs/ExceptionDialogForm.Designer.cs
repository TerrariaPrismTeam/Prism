namespace Prism.WinFormDialogs
{
    partial class ExceptionDialogForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExceptionDialogForm));
            this.buttonHastebin = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.textboxDisplayLoaderErrors = new System.Windows.Forms.RichTextBox();
            this.panelMessageBoxIcon = new System.Windows.Forms.Panel();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // buttonHastebin
            // 
            this.buttonHastebin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonHastebin.Location = new System.Drawing.Point(708, 470);
            this.buttonHastebin.Name = "buttonHastebin";
            this.buttonHastebin.Size = new System.Drawing.Size(143, 23);
            this.buttonHastebin.TabIndex = 1;
            this.buttonHastebin.Text = "Upload to Hastebin";
            this.buttonHastebin.UseVisualStyleBackColor = true;
            this.buttonHastebin.Click += new System.EventHandler(this.buttonHastebin_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonClose.Location = new System.Drawing.Point(857, 470);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 2;
            this.buttonClose.Text = "OK";
            this.buttonClose.UseVisualStyleBackColor = true;
            // 
            // textboxDisplayLoaderErrors
            // 
            this.textboxDisplayLoaderErrors.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textboxDisplayLoaderErrors.BackColor = System.Drawing.SystemColors.Window;
            this.textboxDisplayLoaderErrors.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textboxDisplayLoaderErrors.DetectUrls = false;
            this.textboxDisplayLoaderErrors.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textboxDisplayLoaderErrors.Location = new System.Drawing.Point(66, 12);
            this.textboxDisplayLoaderErrors.Name = "textboxDisplayLoaderErrors";
            this.textboxDisplayLoaderErrors.ReadOnly = true;
            this.textboxDisplayLoaderErrors.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.textboxDisplayLoaderErrors.ShowSelectionMargin = true;
            this.textboxDisplayLoaderErrors.Size = new System.Drawing.Size(866, 414);
            this.textboxDisplayLoaderErrors.TabIndex = 3;
            this.textboxDisplayLoaderErrors.Text = "Loader errors go here.";
            // 
            // panelMessageBoxIcon
            // 
            this.panelMessageBoxIcon.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panelMessageBoxIcon.BackgroundImage")));
            this.panelMessageBoxIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.panelMessageBoxIcon.Location = new System.Drawing.Point(12, 12);
            this.panelMessageBoxIcon.Name = "panelMessageBoxIcon";
            this.panelMessageBoxIcon.Size = new System.Drawing.Size(48, 48);
            this.panelMessageBoxIcon.TabIndex = 4;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.BackColor = System.Drawing.SystemColors.Control;
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Location = new System.Drawing.Point(12, 432);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.richTextBox1.ShowSelectionMargin = true;
            this.richTextBox1.Size = new System.Drawing.Size(920, 32);
            this.richTextBox1.TabIndex = 4;
            this.richTextBox1.Text = resources.GetString("richTextBox1.Text");
            this.richTextBox1.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.richTextBox1_LinkClicked);
            // 
            // ExceptionDialogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(944, 501);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.panelMessageBoxIcon);
            this.Controls.Add(this.textboxDisplayLoaderErrors);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.buttonHastebin);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 180);
            this.Name = "ExceptionDialogForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Prism";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button buttonHastebin;
        private System.Windows.Forms.Button buttonClose;
        public System.Windows.Forms.RichTextBox textboxDisplayLoaderErrors;
        private System.Windows.Forms.Panel panelMessageBoxIcon;
        public System.Windows.Forms.RichTextBox richTextBox1;
    }
}