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
        Goha,
        Music,
        Youtube,
        GamersTv,
        JetSet,
        Hashd
    }
    /// <summary>
    /// Thread safe writing to textbox
    /// </summary>
    public class Log
    {
        private ExRichTextBox tb;
        delegate void SetTextCallback(MainForm.UbiMessage message, ChatIcon icon, bool highlight = false, Color? foreColor = null, Color? backColor = null);
        delegate void ReplaceSmileCodeCB(String code, Bitmap bmp);
        private object lockWriteLine;
        private Properties.Settings settings;
        /// <summary>
        /// Provied Textbox object to the constructor
        /// </summary>
        /// <param name="logTb"></param>
        /// 
        public Log(ExRichTextBox logTb)
        {
            settings = Properties.Settings.Default;
            lockWriteLine = new object();
            tb = logTb;
            tb.Clear();
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
                while (start > 0)
                {
                    if (start > 0)
                    {
                        tb.SelectionStart = start;
                        tb.SelectionLength = code.Length;
                        //tb.Cut();
                        tb.InsertImage(bmp);
                    }
                    start = tb.Text.Substring(0).IndexOf(code);
                }
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
                case ChatIcon.Hashd:
                    return Properties.Resources.hashd;
                case ChatIcon.Music:
                    return Properties.Resources.music;
                case ChatIcon.Youtube:
                    return Properties.Resources.youtube;
                case ChatIcon.GamersTv:
                    return Properties.Resources.gamerstvicon;
                case ChatIcon.JetSet:
                    return Properties.Resources.jetset;
                default:
                    return null;
            }
        }


        /// <summary>
        /// Writes a line to the textbox. Automaticall adds newline character
        /// </summary>
        /// <param name="text"></param>
        public void WriteLine(Ubiquitous.MainForm.UbiMessage message, ChatIcon icon = ChatIcon.Default, bool highlight = false, Color? foreColor = null, Color? backColor = null)
        {
            if (tb == null)
                return;
            if (tb.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(WriteLine);
                try
                {
                    tb.Parent.Invoke(d, new object[] { message, icon, highlight, foreColor, backColor });
                }
                catch { }
            }
            else
            {

                Bitmap chatIcon = GetChatBitmap(icon);


                if (tb.Text.Length > 0)
                    tb.AppendText(Environment.NewLine);
                

                if (!string.IsNullOrEmpty(message.Text))
                {
                    if (tb.TimeStamp)
                        tb.AppendTextAsRtf(DateTime.Now.GetDateTimeFormats('T')[0] + " ", tb.Font, tb.TimeColor);
                    //tb.AppendText(DateTime.Now.GetDateTimeFormats('T')[0] + " ");

                    if (chatIcon != null)
                        tb.InsertImage(chatIcon);

                    tb.AppendTextAsRtf(" ", tb.Font, tb.TextColor);

                    if (message.TextOnly)
                    {
                        if (highlight)
                            tb.AppendTextAsRtf(message.Text, tb.PersonalMessageFont, tb.PersonalMessageColor, tb.PersonalMessageBack);
                        else if (foreColor.HasValue && backColor.HasValue)
                            tb.AppendTextAsRtf(message.Text, tb.Font, foreColor.Value, backColor.Value);
                        else
                            tb.AppendTextAsRtf(message.Text, tb.Font, tb.TextColor);
                    }
                    else
                    {
                        String messageFormat = String.Empty;

                        if (!String.IsNullOrEmpty(message.FromGroupName))
                            messageFormat = settings.appearanceGrpMessageFormat;
                        else if (!String.IsNullOrEmpty(message.FromName))
                            messageFormat = settings.appearanceMsgFormat;

                        String[] messageFormatParts = messageFormat.Split('%');
                        String suffix = String.Empty;
                        foreach (String messageFormatPart in messageFormatParts)
                        {
                            if( messageFormatPart.StartsWith("t") )
                            {
                                suffix = messageFormatPart.Length > 1 ? messageFormatPart.Substring(1) : String.Empty;

                                if (highlight)
                                    tb.AppendTextAsRtf(message.Text, tb.PersonalMessageFont, tb.PersonalMessageColor, tb.PersonalMessageBack);
                                else if (foreColor.HasValue && backColor.HasValue)
                                    tb.AppendTextAsRtf(message.Text, tb.Font, foreColor.Value, backColor.Value);
                                else
                                    tb.AppendTextAsRtf(message.Text, tb.Font, tb.TextColor);

                                if (!String.IsNullOrEmpty(suffix))
                                    tb.AppendTextAsRtf(suffix, tb.Font, tb.TextColor);
                            }
                            else if (messageFormatPart.StartsWith("sg"))
                            {
                                suffix = messageFormatPart.Length > 2 ? messageFormatPart.Substring(2) : String.Empty;
                                tb.AppendTextAsRtf(message.FromGroupName == null ? String.Empty : message.FromGroupName, tb.Font, message.NickColor);
                                if (!String.IsNullOrEmpty(suffix))
                                    tb.AppendTextAsRtf(suffix, tb.Font, tb.TextColor);
                            }
                            else if (messageFormatPart.StartsWith("s"))
                            {
                                suffix = messageFormatPart.Length > 1 ? messageFormatPart.Substring(1) : String.Empty;
                                tb.AppendTextAsRtf(message.FromName == null ? String.Empty : message.FromName, tb.Font, message.NickColor);
                                if (!String.IsNullOrEmpty(suffix))
                                    tb.AppendTextAsRtf(suffix, tb.Font, tb.TextColor);
                            }
                            else if( messageFormatPart.StartsWith("d") )
                            {
                                suffix = messageFormatPart.Length > 1 ? messageFormatPart.Substring(1) : String.Empty;
                                tb.AppendTextAsRtf(message.ToName == null ? String.Empty : "->" + message.ToName, tb.Font, message.NickColor);
                                
                                if( !String.IsNullOrEmpty(suffix))
                                    tb.AppendTextAsRtf(suffix, tb.Font, tb.TextColor);
                            }
                            else if( messageFormatPart.StartsWith("c") )
                            {
                                suffix = messageFormatPart.Length > 1 ? messageFormatPart.Substring(1) : String.Empty;
                                tb.AppendTextAsRtf(message.FromEndPoint.ToString(), tb.Font, message.NickColor);
                                if (!String.IsNullOrEmpty(suffix))
                                    tb.AppendTextAsRtf(suffix, tb.Font, tb.TextColor);
                            }
                            
                        }

                    }


                    //tb.AppendText(" " + text);
                }
                tb.SelectionStart = tb.Text.Length;
                tb.SelectionLength = 0;
                tb.ScrollToEnd();

            }
        }
    }
}
