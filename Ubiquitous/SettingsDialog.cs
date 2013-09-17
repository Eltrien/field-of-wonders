using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
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
            textCounterFont.Text = settings.globalCounterFont.ToString();
            textPersonalMsgFont.Text = settings.globalPersonalMessageFont.ToString();
            twitchMeFont.Text = settings.twitchMeFont.ToString();

            buttonBackColor.BackColor = settings.globalChatBackground;
            buttonForeColor.BackColor = settings.globalChatTextColor;
            buttonCounterBackColor.BackColor = settings.globalCounterBackColor;
            buttonCounterForeColor.BackColor = settings.globalCounterTextColor;
            buttonPersonalMsgColor.BackColor = settings.personalMessageColor;
            buttonPersonalMsgBack.BackColor = settings.globalPersonalMessageBack;
            buttonTwitchMeForecolor.BackColor = settings.twitchMeForeColor;
            buttonTwitchMeBackcolor.BackColor = settings.twitchMeBackcolor;


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
            textFontName.Text = settings.globalChatFont.ToString();
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

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = "ubiquitouschat";
            saveFileDialog.DefaultExt = "jpg";
            saveFileDialog.Filter = "JPEG Image(*.jpg)|*.jpg";
            saveFileDialog.ValidateNames = true;
            
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                settings.globalChatImageFilename = saveFileDialog.FileName;
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            var appPath = Path.GetDirectoryName(Application.ExecutablePath);

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = appPath;
            openFileDialog.FileName = "online";
            openFileDialog.DefaultExt = "mp3";
            openFileDialog.Filter = "MP3 file(*.mp3)|*.mp3";
            openFileDialog.ValidateNames = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var choosenFile = openFileDialog.FileName;
                if (Path.GetDirectoryName(choosenFile) == appPath)
                    choosenFile = Path.GetFileName(choosenFile);

                settings.globalSoundOnlineFile = choosenFile;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var appPath = Path.GetDirectoryName(Application.ExecutablePath);

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = appPath;
            openFileDialog.FileName = "offline";
            openFileDialog.DefaultExt = "mp3";
            openFileDialog.Filter = "MP3 file(*.mp3)|*.mp3";
            openFileDialog.ValidateNames = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var choosenFile = openFileDialog.FileName;
                if (Path.GetDirectoryName(choosenFile) == appPath)
                    choosenFile = Path.GetFileName(choosenFile);

                settings.globalSoundOfflineFile = choosenFile;
            }
        }

        private void buttonCounterFont_Click(object sender, EventArgs e)
        {
            fontDialog = new FontDialog();
            fontDialog.Font = settings.globalCounterFont;
            fontDialog.ShowColor = false;
            fontDialog.ShowApply = true;
            fontDialog.ShowEffects = true;
            fontDialog.Apply +=new EventHandler(fontCounterDialog_Apply);

            fontDialog.ShowDialog();

            settings.globalCounterFont = fontDialog.Font;
            textCounterFont.Text = settings.globalCounterFont.ToString();
        }
        void fontCounterDialog_Apply(object sender, EventArgs e)
        {
            settings.globalCounterFont = fontDialog.Font;
            textCounterFont.Text = settings.globalCounterFont.ToString();
        }

        private void buttonCounterForeColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.AllowFullOpen = true;
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                settings.globalCounterTextColor = colorDialog.Color;
                buttonCounterForeColor.BackColor = colorDialog.Color;
            }
        }

        private void buttonCounterBackColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.AllowFullOpen = true;
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                settings.globalCounterBackColor = colorDialog.Color;
                buttonCounterBackColor.BackColor = colorDialog.Color;
            }
        }

        private void buttonPersonalMsgColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.AllowFullOpen = true;
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                settings.personalMessageColor = colorDialog.Color;
                buttonPersonalMsgColor.BackColor = colorDialog.Color;
            }
        }

        private void buttonPersonalMsgBack_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.AllowFullOpen = true;
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                settings.globalPersonalMessageBack = colorDialog.Color;
                buttonPersonalMsgBack.BackColor = colorDialog.Color;
            }
        }

        private void buttonPersonalMsgFont_Click(object sender, EventArgs e)
        {
            fontDialog = new FontDialog();
            fontDialog.Font = settings.globalPersonalMessageFont;
            fontDialog.ShowColor = false;
            fontDialog.ShowApply = true;
            fontDialog.ShowEffects = true;
            fontDialog.Apply +=new EventHandler(fontPersonalMsgDialog_Apply);

            fontDialog.ShowDialog();

            settings.globalPersonalMessageFont = fontDialog.Font;
            textPersonalMsgFont.Text = settings.globalPersonalMessageFont.ToString();
        }
        void fontPersonalMsgDialog_Apply(object sender, EventArgs e)
        {
            settings.globalPersonalMessageFont = fontDialog.Font;
            textPersonalMsgFont.Text = settings.globalPersonalMessageFont.ToString();
        }

        private void buttonTwitchMeFont_Click(object sender, EventArgs e)
        {
            fontDialog = new FontDialog();
            fontDialog.Font = settings.globalPersonalMessageFont;
            fontDialog.ShowColor = false;
            fontDialog.ShowApply = true;
            fontDialog.ShowEffects = true;
            fontDialog.Apply += new EventHandler(fontTwitchMeFontDialog_Apply);

            fontDialog.ShowDialog();

            settings.twitchMeFont = fontDialog.Font;
            twitchMeFont.Text = settings.twitchMeFont.ToString();
        }
        void fontTwitchMeFontDialog_Apply(object sender, EventArgs e)
        {
            settings.twitchMeFont = fontDialog.Font;
            twitchMeFont.Text = settings.twitchMeFont.ToString();
        }
        private void buttonTwitchMeForecolor_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.AllowFullOpen = true;
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                settings.twitchMeForeColor = colorDialog.Color;
                buttonTwitchMeForecolor.BackColor = colorDialog.Color;
            }
        }

        private void buttonTwitchMeBackcolor_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.AllowFullOpen = true;
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                settings.twitchMeBackcolor= colorDialog.Color;
                buttonTwitchMeBackcolor.BackColor = colorDialog.Color;
            }
        }

    }
}
