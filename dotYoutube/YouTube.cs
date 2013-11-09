using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Security.Cryptography;
using dotWebClient;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Threading;
using dotUtilities;
using System.Diagnostics;
using dot.Json.Linq;
using dot.Json;
using System.Xml.Linq;

namespace dotYoutube
{
    public class YouTube
    {
        private const string domain = "youtube.com";
        private const string loginUrl = @"https://accounts.google.com/ServiceLogin?passive=true&service=youtube&continue=http://www.youtube.com/signin?action_handle_signin=true&app=desktop&feature=sign_in_button&hl=en_US&next=%2F&hl=en_US&uilel=3";
        private const string chatUrl = "http://www." + domain + @"/live_comments?action_get_comments=1&video_id={0}&lt={1}&format=json";
        private const string statsUrl = @"http://www." + domain + "/live_stats?v={0}&t={1}";
        private object statsLock = new object();
        private object messageLock = new object();
        private object loginLock = new object();
        private const int CHATPOLL_INTERVAL = 10000;
        private const int STATSPOLL_INTERVAL = 30000;
        private int viewers;
        private CookieAwareWebClient statsWC, chatWC, loginWC;
        private Timer statsDownloader;
        private Timer chatDownloader;
        private string channelId;
        public class TextEventArgs : EventArgs
        {
            private string _text;
            public TextEventArgs(string text)
            {
                _text = text;
            }
            public string Text
            {
                get { return _text; }
            }
        }
        
        public YouTube(string videoId, string lastTime)
        {
            statsWC = new CookieAwareWebClient();
            chatWC = new CookieAwareWebClient();
            chatWC.Encoding = new System.Text.UTF8Encoding(false);
            loginWC = new CookieAwareWebClient();
            loginWC.Encoding = new System.Text.UTF8Encoding(false);

            channelId = videoId;
            LastTime = lastTime;
            viewers = 0;
            
            statsDownloader = new Timer(new TimerCallback(statsDownloader_Tick), null, Timeout.Infinite, Timeout.Infinite);
            chatDownloader = new Timer(new TimerCallback(chatDownloader_Tick), null, Timeout.Infinite, Timeout.Infinite);

        }
        public bool Login(string user, string password)
        {
            
            /*var result = loginWC.DownloadString(loginUrl);
            
            var botguardClass = Re.GetSubString(result, @"(<script.*?>.*?botguard.bg.prototype.invoke.*?</script>)",1);
            var botguardInit = Re.GetSubString(result, @"<script.*?>(.*?new botguard.bg.*?)</script>",1);

            var doc = Browser.ParseContent(botguardClass);
            
            */
            return true;
        }
        #region Events
        public event EventHandler<EventArgs> OnLogin;
        public event EventHandler<EventArgs> OnDisconnect;
        public event EventHandler<YoutubeMessage> OnMessageReceived;
        public event EventHandler<TextEventArgs> OnError;

        #endregion

        #region Public methods

        private void statsDownloader_Tick(object o)
        {
            DownloadStats();
        }
        private void chatDownloader_Tick(object o)
        {
            DownloadChat();
            //DownloadStats(_user);
        }
        public bool isLoggedIn
        {
            get;
            set;
        }
        public void Start()
        {
            if (String.IsNullOrEmpty(channelId))
                return;

            statsDownloader.Change(0, STATSPOLL_INTERVAL);
            chatDownloader.Change(0, CHATPOLL_INTERVAL);
        }
        public void Stop()
        {
            statsDownloader.Change(Timeout.Infinite, Timeout.Infinite);
            chatDownloader.Change(Timeout.Infinite, Timeout.Infinite);
        }
        private void DownloadStats()
        {
            if (String.IsNullOrEmpty(channelId))
                return;
            lock (statsLock)
            {
                String result = String.Empty;
                try
                {

                    result = statsWC.DownloadString(String.Format(statsUrl, channelId, TimeUtils.UnixTimestamp()));
                }
                catch {
                }

                if (String.IsNullOrEmpty(result))
                    return;
                UInt32 viewers = 0;
                UInt32.TryParse(result, out viewers);
                Viewers = viewers;
            }
        }

        private void DownloadChat()
        {
            if (String.IsNullOrEmpty(channelId))
                 return;

            lock (messageLock)
            {
                String result = String.Empty;
                try
                {
                    result = chatWC.DownloadString(String.Format(chatUrl, channelId, (String.IsNullOrEmpty(LastTime) ? TimeUtils.UnixTimestamp() : LastTime)));
                }
                catch
                {
                    Debug.Print("Youtube chatdownload exception");
                    return;
                }
                //result = result.Replace("\t", " ");
                if (String.IsNullOrEmpty(result))
                    return;

                var doc = XDocument.Parse(result);
                if( doc == null )
                    return;

                string chatJson = (string)doc.Root.Element("html_content").Value;

                try
                {
                    var generalInfo = JObject.Parse(chatJson);

                    if (generalInfo != null)
                    {
                        LastTime = generalInfo["latest_time"].ToString();
                        if (String.IsNullOrEmpty(LastTime))
                            return;

                        var comments = JArray.Parse(generalInfo["comments"].ToString());

                        foreach (var comment in comments)
                        {
                            var author_name = comment["author_name"].ToString();
                            var comment_text = comment["comment"].ToString();
                            comment_text = comment_text.Replace("\xfeff", "");

                            if (String.IsNullOrEmpty(author_name) || 
                                String.IsNullOrEmpty(comment_text))
                                return;
                            
                            if (OnMessageReceived != null)
                                OnMessageReceived(this, new YoutubeMessage() { User = author_name, Text = comment_text });
                        }
                    }
                }
                catch { }

            }
        }
        private void DownloadStats(string channel)
        {
            // if (String.IsNullOrEmpty(_user))
            //     return;

            lock (statsLock)
            {
                //GetWSCounters();
                //GetFlashCounters();

            }
        }

        #endregion
        #region Public properties
        public String LastTime
        {
            get;
            set;
        }
        public UInt32 Viewers
        {
            get;
            set;
        }

        #endregion

        public class YoutubeMessage : EventArgs
        {
            public string Text
            {
                get;
                set;
            }
            public string User
            {
                get;
                set;
            }
        }
        private class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding { get { return Encoding.UTF8; } }
        }
    }
}
