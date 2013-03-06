using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace Ubiquitous
{
    public partial class DebugForm : Form
    {
        
        private Properties.Settings settings;
        public DebugForm()
        {
            settings = Properties.Settings.Default;
            
            InitializeComponent();
            
            //hack to get window created
            var h = this.Handle;
            
            Log debugLog = new Log(exRichTextBoxDebug);
            var maskPasswords = new String[]{
                settings.battlelogPassword,
                settings.cyberPassword,
                settings.empirePassword,
                settings.GohaPassword,
                settings.goodgamePassword,
                settings.Sc2tvPassword,
                settings.SteamBotPassword,
                settings.TwitchPassword
            };
            Debug.Listeners.Add(new TextWriterTraceListener(new LogStreamWriter(debugLog, maskPasswords)));
        }

        private void DebugForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

    }
}
