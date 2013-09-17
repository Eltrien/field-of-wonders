namespace Ubiquitous
{
    partial class DebugForm
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
            this.exRichTextBoxDebug = new SC2TV.RTFControl.ExRichTextBox();
            this.SuspendLayout();
            // 
            // exRichTextBoxDebug
            // 
            this.exRichTextBoxDebug.BackColor = global::Ubiquitous.Properties.Settings.Default.globalChatBackground;
            this.exRichTextBoxDebug.DataBindings.Add(new System.Windows.Forms.Binding("TextColor", global::Ubiquitous.Properties.Settings.Default, "globalChatTextColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.exRichTextBoxDebug.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::Ubiquitous.Properties.Settings.Default, "globalChatBackground", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.exRichTextBoxDebug.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::Ubiquitous.Properties.Settings.Default, "globalChatTextColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.exRichTextBoxDebug.Dock = System.Windows.Forms.DockStyle.Fill;
            this.exRichTextBoxDebug.ForeColor = global::Ubiquitous.Properties.Settings.Default.globalChatTextColor;
            this.exRichTextBoxDebug.HighlightColor = System.Drawing.Color.White;
            this.exRichTextBoxDebug.Location = new System.Drawing.Point(0, 0);
            this.exRichTextBoxDebug.Name = "exRichTextBoxDebug";
            this.exRichTextBoxDebug.RawTextColor = null;
            this.exRichTextBoxDebug.SaveToImage = false;
            this.exRichTextBoxDebug.SaveToImageFileName = null;
            this.exRichTextBoxDebug.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.exRichTextBoxDebug.Size = new System.Drawing.Size(711, 458);
            this.exRichTextBoxDebug.TabIndex = 0;
            this.exRichTextBoxDebug.Text = "";
            this.exRichTextBoxDebug.TextColor = global::Ubiquitous.Properties.Settings.Default.globalChatTextColor;
            this.exRichTextBoxDebug.TimeColor = System.Drawing.Color.Empty;
            this.exRichTextBoxDebug.TimeStamp = false;
            // 
            // DebugForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(711, 458);
            this.Controls.Add(this.exRichTextBoxDebug);
            this.Name = "DebugForm";
            this.Text = "Debug messages";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DebugForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private SC2TV.RTFControl.ExRichTextBox exRichTextBoxDebug;
    }
}