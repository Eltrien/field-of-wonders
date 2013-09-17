using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dotWebClient;
using System.Threading;
using System.Diagnostics;
using dotUtilities;
using System.Web;
using System.Net;
using WebSocket4Net;
using dot.Json.Linq;
using dot.Json;

namespace dotHashd
{
    public class Hashd
    {
        #region Constants
        private const string userAgent = "Mozilla/5.0 (Windows NT 6.0; WOW64; rv:14.0) Gecko/20100101 Firefox/14.0.1";
        private const string hashDomain = "hashd.tv";
        private const string channelInfoUrl = "http://api." + hashDomain + "/v1/stream/{0}";
        private const string loginUrl = "http://" + hashDomain + "/signin";
        private const string loginParams = @"utf8=%E2%9C%93&authenticity_token={2}&username={0}&password={1}&commit=Log+in";

        private const int maxServerNum = 0x1e3;

        private const string reAssetsHost = @"Hashd.assetsHost[^']+'([^']+)'";
        private const string reUserName = @"username = ""([^""]+)"";";
        private const string reAuthToken = @"<input name=""authenticity_token""[^>]+value=""([^""])""";
        private const string reMessage = @"{\\""message\\"":\\""(.*?)\\"",\\""user\\"":{\\""id\\"":\\"".*?\\"",\\""chatNameColor\\"":\\"".*?\\"",\\""username\\"":\\""(.*?)\\"",";
        private const int pollIntervalStats = 20000;
        private const int pingInterval = 5000;

        #endregion
        #region Private properties
        private string lastTimestamp;
        private string chatUpdateNonce;
        private string chatNewMessageNonce;
        private Timer statsDownloader;
        private Timer pingTimer;
        private CookieAwareWebClient statsWC, loginWC;
        private bool prevOnlineState = false;
        private Channel currentChannelStats;
        private string _login;
        private string _user;
        private string _password;
        private string _rnd_number, _rnd_string;
        private object loginLock = new object();
        private object statsLock = new object();
        private WebSocket socket;
        private int _opcode;
        #endregion
        #region Events
        public event EventHandler<EventArgs> Live;
        public event EventHandler<EventArgs> Offline;
        public event EventHandler<EventArgs> OnLogin;
        public event EventHandler<EventArgs> OnError;
        public event EventHandler<HashdMessageEventArgs> OnMessage;
        private void DefaultEvent(EventHandler<EventArgs> evnt, EventArgs e)
        {
            EventHandler<EventArgs> handler = evnt;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        private void OnLive(EventArgs e)
        {
            DefaultEvent(Live, e);
        }
        private void OnOffline(EventArgs e)
        {
            DefaultEvent(Offline, e);
        }
        #endregion

        #region Public methods
        public Hashd(string login, string password)
        {
            _login = login;
            _password = password;
            statsWC = new CookieAwareWebClient();
            loginWC = new CookieAwareWebClient();
            statsDownloader = new Timer(new TimerCallback(statsDownloader_Tick), null, Timeout.Infinite, Timeout.Infinite);
            pingTimer = new Timer(new TimerCallback(pingTimer_Tick), null, Timeout.Infinite, Timeout.Infinite);

        }

        private void statsDownloader_Tick(object o)
        {
            DownloadStats(_user);
        }
        private void pingTimer_Tick(object o)
        {
            Ping();
        }
        public bool isLoggedIn
        {
            get;
            set;
        }
        public void Start()
        {

            ConnectWebsocket();
            statsDownloader.Change(0, pollIntervalStats);
        }
        public void Stop()
        {
            statsDownloader.Change(Timeout.Infinite, Timeout.Infinite);
            pingTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }
        public bool Login()
        {
            lock (loginLock)
            {
                _opcode = 0;
                List<KeyValuePair<string, string>> cookies = new List<KeyValuePair<string,string>>();

                isLoggedIn = false;
                if (String.IsNullOrEmpty(_login) || String.IsNullOrEmpty(_password))
                    return false;

                var result = loginWC.DownloadString(loginUrl);
                if (String.IsNullOrEmpty(result))
                    return false;

                var auth_token = Re.GetSubString(result, reAuthToken, 1);
                if (String.IsNullOrEmpty(result))
                    return false;

                loginWC.ContentType = ContentType.UrlEncoded;

                result = loginWC.UploadString(loginUrl, String.Format(loginParams, HttpUtility.UrlEncode(_login), HttpUtility.UrlEncode(_password), HttpUtility.UrlEncode(auth_token)));

                var assetsHost = Re.GetSubString(result, reAssetsHost, 1);
                if (String.IsNullOrEmpty(assetsHost))
                    return false;

                _user = Re.GetSubString(result, reUserName, 1);
                if (String.IsNullOrEmpty(_user))
                    return false;
                isLoggedIn = true;

                return true;
            }
        }

        private void ConnectWebsocket()
        {
            _rnd_number = random_hashd_number();
            _rnd_string = random_hashd_string();

            socket = new WebSocket(
                String.Format("ws://{0}:9999/chat/{1}/{2}/websocket", hashDomain,_rnd_number, random_hashd_string()),
                "",
                loginWC.CookiesStrings,
                null,
                null,
                @"http://" + hashDomain,
                WebSocketVersion.DraftHybi10
                );
            socket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(socket_MessageReceived);
            socket.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(socket_Error);
            socket.Open();

        }

        private string OpCode
        {
            get { _opcode++; return _opcode.ToString(); }
        }

        void socket_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            if (OnError != null)
                OnError(this, EventArgs.Empty);
        }

        private void SwitchChannel( string channel)
        { 
            var channelCommand = @"[""{\""opcodeID\"":1,\""data\"":{\""channel\"":\""" + channel + @"\""}}""]";
            socket.Send(channelCommand);
        }
        private void SendSessionID()
        {
            var sessionCookie = loginWC.CookiesStrings.Where(m => m.Key == "_session_id").FirstOrDefault();
            var sessionCommand = @"[""{\""opcodeID\"":2,\""data\"":{\""sessionID\"":\""" + sessionCookie.Value + @"\"",\""type\"":2}}""]";
            socket.Send(sessionCommand);
        }
        public string User
        {
            get { return _user; }
        }
        public void SendMessage(string text)
        {
            var sendCommand = @"[""{\""opcodeID\"":3,\""data\"":{\""message\"":\""" + text + @"\""}}""]";
            socket.Send(sendCommand);
        }
        void socket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Message))
                return;

            if (e.Message == "o")
            {
                if (OnLogin != null)
                    OnLogin(this, EventArgs.Empty);
                pingTimer.Change(pingInterval, pingInterval);

                SwitchChannel(_user);
                Thread.Sleep(1000);
                SendSessionID();
                return;
            }

            if (e.Message.Contains(@"a[""{\""opcodeID\"""))
            {

                int opcodeId = -1;
                JObject data = null;
                try
                {
                    var first = JArray.Parse(e.Message.Substring(1));
                    if (first != null)
                    {
                        var pair = JObject.Parse((String)first[0]);
                        if (pair != null)
                        {
                            opcodeId = (int)pair["opcodeID"];
                            data = (JObject)pair["data"];
                            switch (opcodeId)
                            {
                                case 4:
                                    var text = data["message"].ToString();
                                    var userobj = data["user"];
                                    var user = userobj["username"].ToString();

                                    
                                    if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(user))
                                        return;
                                    if (user != _user)
                                    {
                                        if (OnMessage != null)
                                            OnMessage(this, new HashdMessageEventArgs(new Message() { User = user, Text = text }));
                                    }
                                    break;
                            }
                        }
                    }
                    }
                    catch{}



            }

        }
        private void Ping()
        {
            var command = @"[""{\""opcodeID\"":11,\""data\"":{}}""]";

            //loginWC.XMLHttpRequest(String.Format(@"http://hashd.tv:9999/chat/{0}/{1}/xhr_send", _rnd_number, _rnd_string), command);
            socket.Send(command );
        }
        private void DownloadStats(string channel)
        {
            if (String.IsNullOrEmpty(_user))
                return;
            lock (statsLock)
            {
                var prevChannelStats = currentChannelStats;
                try
                {
                    statsWC.Headers["Cache-Control"] = "no-cache";
                    var url = String.Format(channelInfoUrl, channel.ToLower());
                    using (var stream = statsWC.downloadURL(url))
                    {
                        if (statsWC.LastWebError == "ProtocolError")
                        {
                            return;
                        }

                        if (stream == null)
                        {
                            Debug.Print("Hashd: Can't download channel info of {0} result is null. Url: {1}", channel, channelInfoUrl);
                        }
                        else
                        {
                            currentChannelStats = JsonGenerics.ParseJson<Channel>.ReadObject(stream);
                        }

                        if (!Alive && prevChannelStats != currentChannelStats)
                            OnOffline(EventArgs.Empty);
                        else if (Alive && prevChannelStats != currentChannelStats)
                             OnLive(EventArgs.Empty);

                    }
                }
                catch
                {
                    Debug.Print("Hashd: Exception in Downloadstats");
                }
            }
        }
        private bool Alive
        {
            get
            {
                if (currentChannelStats == null)
                    return false;

                return currentChannelStats.live;
            }
        }
        private string random_hashd_number()
        {
            var num = new Random();
            return (num.Next(0, maxServerNum)).ToString("000");
        }
        private string random_hashd_string()
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
        public string Viewers
        {
            get { return Alive ? String.Format("{0}",currentChannelStats.currentViewers) : "0"; }
            set { }
        }
        #endregion

        public class HashdMessageEventArgs : EventArgs
        {
            public HashdMessageEventArgs(Message message)
            {
                Message = message;
            }

            public Message Message { get; private set; }
        }

        public class Message
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

    }

}
