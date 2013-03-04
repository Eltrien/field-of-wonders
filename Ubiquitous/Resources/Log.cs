using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using SC2TV.RTFControl;
using System.Drawing;

namespace Ubiquitous
{
    public enum ChatIcon
    {
        Default,
        Sc2Tv,
        TwitchTv,
        Skype,
        Console,
        Steam,
        Admin,
        Battlelog,
        Goodgame,
        Empire,
        Cybergame,
        Goha
    }
    /// <summary>
    /// Thread safe writing to textbox
    /// </summary>
    class Log
    {
        private ExRichTextBox tb;
        delegate void SetTextCallback(string text, ChatIcon icon);
        delegate void ReplaceSmileCodeCB(String code, Bitmap bmp);


        /// <summary>
        /// Provied Textbox object to the constructor
        /// </summary>
        /// <param name="logTb"></param>
        /// 
        public Log(ExRichTextBox logTb)
        {
            tb = logTb;
        }
        public void ReplaceSmileCode(String code, Bitmap bmp)
        {
            if (tb.InvokeRequired)
            {
                ReplaceSmileCodeCB d = new ReplaceSmileCodeCB(ReplaceSmileCode);
                try
                {
                    tb.Parent.Invoke(d, new object[] { code,bmp });
                }
                catch { }
            }
            else
            {
                int start = tb.Text.Substring(0).IndexOf(code);

                tb.SelectionStart = start;
                tb.SelectionLength = code.Length;                
                //tb.Cut();
                tb.InsertImage(bmp);
            }
        }
        public Bitmap GetChatBitmap( ChatIcon icon)
        {            
            switch (icon)
            {
                case ChatIcon.Sc2Tv:
                    return Properties.Resources.sc2icon;
                case ChatIcon.TwitchTv:
                    return Properties.Resources.twitchicon;
                case ChatIcon.Steam:
                    return Properties.Resources.steamicon;
                case ChatIcon.Skype:
                    return Properties.Resources.skypeicon;
                case ChatIcon.Admin:
                    return Properties.Resources.adminicon;
                case ChatIcon.Goodgame:
                    return Properties.Resources.goodgameicon;
                case ChatIcon.Battlelog:
                    return Properties.Resources.bf3icon;
                case ChatIcon.Empire:
                    return Properties.Resources.empire;
                case ChatIcon.Goha:
                    return Properties.Resources.goha;
                case ChatIcon.Cybergame:
                    return Properties.Resources.cybergame;
                case ChatIcon.Default:
                    return Properties.Resources.adminicon;
                default:
                    return null;
            }
        }
        /// <summary>
        /// Writes a line to the textbox. Automaticall adds newline character
        /// </summary>
        /// <param name="text"></param>
        public void WriteLine(string text, ChatIcon icon = ChatIcon.Default)
        {
            if (tb == null)
                return;
            if (tb.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(WriteLine);
                try
                {
                    tb.Parent.Invoke(d, new object[] { text, icon});
                }
                catch { }
            }
            else
            {
                Bitmap chatIcon = GetChatBitmap(icon);


                if( tb.Text.Length > 0 )
                    tb.AppendText(Environment.NewLine);


                if (text != null)
                {
                    if (tb.TimeStamp)
                        tb.AppendTextAsRtf(DateTime.Now.GetDateTimeFormats('T')[0] + " ", tb.Font, tb.TimeColor);

                    if(chatIcon != null)
                        tb.InsertImage( chatIcon );

                    tb.AppendTextAsRtf(" " + text,tb.Font,tb.TextColor);
                }
                tb.SelectionStart = tb.Text.Length;
                tb.SelectionLength = 0;
                var bw = new BGWorker(tb.ScrollToEnd, null);
            }
        }
    }
}
