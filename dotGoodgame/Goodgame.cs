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
using WebSocket4Net;
using dotUtilities;
using System.Diagnostics;
using dot.Json.Linq;
using dot.Json;

namespace dotGoodgame
{
    public class Goodgame
    {
        private const string domain = "goodgame.ru";
        private const string chatUrl = "http://www." + domain + "/chat/{0}/";
        private const string channelUrl = @"http://www." + domain + "/channel/{0}";
        private const string loginUrl = @"http://" + domain + "/ajax/login/";
        private const string editUlr = @"http://" + domain + "/channel/{0}/edit/";
        private const string statsUrl = @"http://goodgame.ru/api/getchannelstatus?id={0}&fmt=json";
        private const int maxServerNum = 0x1e3;
        private const int pollInterval = 20000;
        private String[] urlReplace = new String[] { ".", "-" };
        private int _chatId;
        private int _userId;
        private string _user;
        private string _userToken;
        private string _channel;
        private string _password;
        private bool _loadHistory;
        private object loginLock = new object();
        private object statsLock = new object();
        private object messageLock = new object();
        private int viewers;
        private CookieAwareWebClient loginWC, statsWC, chatWC;
        private Timer statsDownloader;
        private WebSocket socket;

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

        public Goodgame( string user, string password, bool loadHistory = false )
        {
            loginWC = new CookieAwareWebClient();
            statsWC = new CookieAwareWebClient();
            chatWC = new CookieAwareWebClient();

            _loadHistory = loadHistory;
            _chatId = -1;
            _userId = -1;
            _userToken = null;
            _user = user;
            _password = password;
            viewers = 0;
            FlashViewers = "0";
            
            statsDownloader = new Timer(new TimerCallback(statsDownloader_Tick), null, Timeout.Infinite, Timeout.Infinite);

        }

        #region Goodgame Events
        public event EventHandler<EventArgs> OnLogin;
        public event EventHandler<EventArgs> OnDisconnect;
        public event EventHandler<Message> OnMessageReceived;
        public event EventHandler<TextEventArgs> OnError;

        #endregion

        #region Public methods
        public bool Started
        {
            get;
            set;
        }
        private void statsDownloader_Tick(object o)
        {
            DownloadStats(_user);
        }
        public bool isLoggedIn
        {
            get;
            set;
        }
        public void Start()
        {
            ConnectWebsocket();
            Started = true;
        }
        public void Stop()
        {
            statsDownloader.Change(Timeout.Infinite, Timeout.Infinite);
            try
            {
                socket.Close();
            }
            catch { }
            Started = false;
        }
        public String URLUser
        {
            get
            {
                if (String.IsNullOrEmpty(_user))
                    return String.Empty;

                var result = _user;
                foreach (var c in urlReplace)
                {
                    _user = _user.Replace(c, "");
                }
                return _user;
            }
        }
        public bool Login()
        {
            lock (loginLock)
            {
                isLoggedIn = false;

                if (String.IsNullOrEmpty(_user) || String.IsNullOrEmpty(_password))
                    return false;

                var result = loginWC.DownloadString(String.Format(chatUrl, URLUser));

                string loginParams = "login=" + _user + "&password=" + _password + "&remember=1";
                loginWC.setCookie("fixed", "1", domain);
                loginWC.setCookie("auto_login_name", _user, domain);

                loginWC.ContentType = ContentType.UrlEncoded;
                loginWC.Headers["X-Requested-With"] = "XMLHttpRequest";

                loginWC.UploadString(loginUrl, loginParams);

                result = loginWC.DownloadString(String.Format(chatUrl, URLUser));
                if (String.IsNullOrEmpty(result))
                    return false;
                
                int.TryParse(loginWC.CookieValue("uid", "http://" + domain), out _userId);
                int.TryParse(Re.GetSubString(result, @"channelId: (\d+?),", 1), out _chatId);
                _channel = _chatId.ToString();
                _userToken = Re.GetSubString(result, @"token: '(.*?)',", 1);

                var editContent = loginWC.DownloadString(String.Format(editUlr, URLUser));
                var serviceUrls = new String[] { "twitch.tv", "cybergame.tv", "hashd.tv", "youtube.com" };
                //<input type="text" name="video_urls[22473]" value="http://twitch.tv/xedoc"

                ServiceNames = String.Empty;
                foreach( var service in serviceUrls )
                {
                    var substr = Re.GetSubString(editContent, @"<input.*?video_urls\[.*?value=.*?(" + service + @")[^\""]*",1);
                    if (!String.IsNullOrEmpty(substr))
                    {
                        ServiceNames = ServiceNames + substr;
                    }
                }
                


                isLoggedIn = true;

                return true;
            }
        }

        public String ServiceNames
        {
            get;
            set;
        }
        private void ConnectWebsocket()
        {
            try
            {
                socket = new WebSocket(
                    String.Format("ws://{0}:8080/chat/{1}/{2}/websocket", domain, random_number(), random_string()),
                    "",
                    loginWC.CookiesStrings,
                    null,
                    null,
                    @"http://" + domain,
                    WebSocketVersion.DraftHybi10
                    );
                socket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(socket_MessageReceived);
                socket.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(socket_Error);
                socket.Open();
            }
            catch(Exception e) {

                Debug.Print(String.Format("Goodgame websocket connection failed. {0}", e.Message));
            }
        }

        void socket_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            try
            {
                this.Stop();
                Debug.Print(e.Exception.InnerException.Message);
                if (OnError != null)
                    OnError(this, new TextEventArgs(e.Exception.ToString()));

                if (OnDisconnect != null)
                    OnDisconnect(this, EventArgs.Empty);

                this.Start();
            }
            catch { }
            
        }
        private void SendAuth()
        {
            var command = @"[""{\""type\"":\""auth\"",\""data\"":{\""user_id\"":\""" + _userId.ToString() + @"\"",\""token\"":\""" + _userToken + @"\""}}""]";
            socket.Send(command);
        }
        private void SwitchChannel( )
        { 
            var command = @"[""{\""type\"":\""join\"",\""data\"":{\""channel_id\"":" + _channel + @"}}""]";
            socket.Send(command);
        }
        private void GetHistory()
        {
            var command = @"[""{\""type\"":\""get_channel_history\"",\""data\"":{\""channel_id\"":" + _channel + @"}}""]";
            socket.Send(command);
        }
        private void GetFlashCounters()
        {

            lock( statsLock )
            {
                var flashViewers = 0;
                try
                {
                    var jsonStats = JObject.Parse(statsWC.DownloadString(String.Format(statsUrl, _channel)));
                    if (jsonStats != null)
                    {
                        var userStats = (JObject)jsonStats[_channel.ToString()];
                        FlashViewers = userStats["viewers"].ToString();
                    }
                }
                catch (Exception e)
                {
                    Debug.Print(String.Format("Goodgame: can't parse channel stats json. Error: {0}", e.Message));
                }


            }
        }

        private void GetWSCounters()
        {
            var command = @"[""{\""type\"":\""get_channel_counters\"",\""data\"":{\""channel_id\"":" + _channel + @"}}""]";
            socket.Send(command);
        }
        public void SendMessage(string message)
        {
            var command = @"[""{\""type\"":\""send_message\"",\""data\"":{\""channel_id\"":" + _channel + @",\""text\"":\""" + message + @"\""}}""]";
            socket.Send(command);
        }
        public string User
        {
            get { return _user; }
        }
        void socket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            lock( messageLock )
            {
                if (string.IsNullOrEmpty(e.Message))
                    return;

                if( e.Message == "o")
                {
                    SendAuth();
                    return;
                }
                Debug.Print(e.Message); 
                if (e.Message.Contains(@"a[""{\""type\"":\"""))
                {
                    String type = null;
                    JObject data = null;
                    try
                    {
                        var first = JArray.Parse(e.Message.Substring(1));
                        if (first != null)
                        {
                            var pair = JObject.Parse((String)first[0]);
                            if (pair != null)
                            {
                                type = (String)pair["type"];
                                data = (JObject)pair["data"];
                                switch (type.ToLower())
                                {
                                    case "channel_counters":
                                        var channel_id = data["channel_id"].ToString();
                                        var clients_in_channel = data["clients_in_channel"].ToString();
                                        var users_in_channel = data["users_in_channel"].ToString();
                                        int.TryParse(clients_in_channel, out viewers);
                                        break;
                                    case "success_auth":
                                        SwitchChannel();
                                        break;
                                    case "success_join":
                                        if (OnLogin != null)
                                            OnLogin(this, EventArgs.Empty);
                                        GetHistory();
                                        statsDownloader.Change(0, pollInterval);
                                        break;
                                    case "message":
                                        var user = data["user_name"].ToString();
                                        var text = HttpUtility.HtmlDecode(data["text"].ToString());
                                        if (user != _user)
                                        {
                                            Message msg = new Message() { Text = text, User = user };

                                            if (text.Length > user.Length && 
                                                text.Substring(0, user.Length).ToLower() + "," == user.ToLower() + ",")
                                            {
                                                msg.ToName = user.ToLower();
                                            }

                                            if (OnMessageReceived != null)
                                                OnMessageReceived(this, new Message() { Text = text, User = user });
                                        }

                                        break;
                                    case "private_message":
                                        {
                                            var private_user = data["user_name"].ToString();
                                            var private_text = HttpUtility.HtmlDecode(data["text"].ToString());
                                            var to_name = data["target_name"].ToString();

                                            if (private_user != _user)
                                            {
                                                Message msg = new Message() { Text = private_text, User = private_user, ToName = to_name};

                                                if (OnMessageReceived != null)
                                                    OnMessageReceived(this, msg);
                                            }

                                        }
                                        break;

                                }
                            }

                        }
                    }
                    catch { }
                }
            }

        }

        private void DownloadStats(string channel)
        {
            if (String.IsNullOrEmpty(_user))
                return;

            lock (statsLock)
            {
                GetWSCounters();
                GetFlashCounters();

            }
            if (socket == null || !socket.Handshaked)
            {
                Started = false;
                Start();
            }
        }
        private string random_number()
        {
            var num = new Random();
            return (num.Next(0, maxServerNum)).ToString("000");
        }
        private string random_string()
        {
            var chars = "abcdefghijklmnopqrstuvwxyz0123456789_";
            StringBuilder builder = new StringBuilder();
            var random = new Random();
            
            for (var i = 0; i < 8; i++)
                builder.Append(chars[random.Next(0, chars.Length - 1)]);

            return builder.ToString();
        }
        #endregion
        #region Public properties
        public string FlashViewers
        {
            get;set;
        }
        public string Viewers
        {
            get { return viewers.ToString(); }
            set { }
        }
        #endregion

        public class Message : EventArgs
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
            public string ToName
            {
                get;
                set;
            }
        }
    }
}
