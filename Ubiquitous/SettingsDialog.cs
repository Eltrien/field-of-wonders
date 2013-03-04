using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using SC2TV.RTFControl;
using dotUtilities;

namespace Ubiquitous
{
    public partial class SettingsDialog : Form
    {
        private Properties.Settings settings;
        private FontDialog fontDialog, fontDialogTime;

        public SettingsDialog()
        {
            settings = Properties.Settings.Default;
            InitializeComponent();
        }

        private void settingsTree1_Load(object sender, EventArgs e)
        {

        }

        private void SettingsDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            settings.Save();

        }

        private void settingsPage9_Paint(object sender, PaintEventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            try
            {
                System.Diagnostics.Process.Start("http://www.obsremote.com/download.html");
            }
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                    MessageBox.Show(noBrowser.Message);
            }
            catch (System.Exception other)
            {
                MessageBox.Show(other.Message);
            }
        }

        private void SettingsDialog_Shown(object sender, EventArgs e)
        {
            textFontName.Text = settings.globalChatFont.ToString();
            textFontTimestamp.Text = settings.globalTimestampFont.ToString();
            buttonBackColor.BackColor = settings.globalChatBackground;
            buttonForeColor.BackColor = settings.globalChatTextColor;
            buttonTimeColor.BackColor = settings.globalTimestampForeground;
        }

        private void buttonChatFont_Click(object sender, EventArgs e)
        {
            fontDialog = new FontDialog();
            fontDialog.Font = settings.globalChatFont;           
            fontDialog.ShowColor = false;
            fontDialog.ShowApply = true;
            fontDialog.ShowEffects = true;
            fontDialog.Apply += new EventHandler(fontDialog_Apply);
            
            fontDialog.ShowDialog();

            settings.globalChatFont = fontDialog.Font;
        }

        void fontDialog_Apply(object sender, EventArgs e)
        {
            settings.globalChatFont = fontDialog.Font;
        }

        private void buttonForeColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.AllowFullOpen = true;
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                settings.globalChatTextColor = colorDialog.Color;
                buttonForeColor.BackColor = colorDialog.Color;
            }
            
        }
        
        private void buttonBackColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.AllowFullOpen = true;
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                settings.globalChatBackground = colorDialog.Color;
                buttonBackColor.BackColor = colorDialog.Color;
            }
        }

        private void label31_Click(object sender, EventArgs e)
        {

        }

        private void buttonTimeColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.AllowFullOpen = true;
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                settings.globalTimestampForeground = colorDialog.Color;
                buttonTimeColor.BackColor = colorDialog.Color;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            fontDialogTime = new FontDialog();
            fontDialogTime.Font = settings.globalChatFont;
            fontDialogTime.ShowColor = false;
            fontDialogTime.ShowApply = true;
            fontDialogTime.ShowEffects = true;
            fontDialogTime.Apply += new EventHandler(fontDialogTime_Apply);

            fontDialogTime.ShowDialog();

            settings.globalTimestampFont = fontDialogTime.Font;
        }

        void fontDialogTime_Apply(object sender, EventArgs e)
        {
            settings.globalTimestampFont = fontDialogTime.Font;
        }

        private void button2_Click(object sender, EventArgs e)
        {

            SaveFileDialog openFileDialog = new SaveFileDialog();
            openFileDialog.FileName = "ubiquitouschat";
            openFileDialog.DefaultExt = "jpg";
            openFileDialog.Filter = "JPEG Image(*.jpg)|*.jpg";
            openFileDialog.ValidateNames = true;
            
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                settings.globalChatImageFilename = openFileDialog.FileName;
            }
        }



    }
}
