using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dotWebSocket;
using System.Diagnostics;
using dotWebClient;
using System.Net;
using System.Web;
using dotPhpSerialize;
using System.Collections;
using dotUtilities;
using dot.Json;
using dot.Json.Linq;
using System.Threading;

namespace dotJetSetPro
{
    public class JetSetPro : UbiWebSocket
    {
        private object loginLock;
        private const int PING_PERIOD = 25000;
        private const String domain = @"jetset.pro";
        private const String baseUrl = @"http://" + domain;
        private const String socketDomain = "cloud.jetset.pro";
        private const String loginUrl = baseUrl + "/api/users/login";
        private const String chatJsUrl = baseUrl + "/static/js/chat.js{0}";
        private const String reRoomId = @"^.*?/chat/channel/(\d+)?/.*$";
        private const string reConfigVer = @"^.*?CONFIG_VERSION.*?(\d+).*$";
        private const string reSessId = @"^([^:]+):.*$";
        private const string reRndToken = @"^.*?RND_TOKEN.*?'([^']+)'.*$";
        private const string reToken = @"^.*?\sTOKEN.*?'([^']+)'.*$";
        private const String homeUrl = baseUrl;
        private String configTemplate = @"{{""version"":{0},""updated"":{1},""settings"":{{""panels"":{{}},""notices"":{{}},""chat"":{{""ruler"":false}}}},""inputs"":{{""author_type"":0}}}};";
        private String reChannelUrl = @"href='([^']+)'[^>]+button";
        private const String streamsUrl = "http://jetset.pro/stream";
        private String _user, _password;
        private String _channelUrl;
        private string userId;
        private string hash1, hash2;
        private string token, roomid, rndtoken;
        private PHPSerializer serializer;
        private Timer timerKeepAlive;
        public event EventHandler<JetSetMessage> OnMessageReceived;
        public event EventHandler<EventArgs> OnLogin;

        private CookieAwareWebClient loginWC;
        public JetSetPro(string user, string password)
        {
            loginLock = new object();
            loginWC = new CookieAwareWebClient();
            _user = user;
            _password = password;
            serializer = new PHPSerializer();
        }
        public bool IsLoggedIn
        {
            get;
            set;
        }
        public void Start()
        {

        }
        public void Stop()
        {
            if( timerKeepAlive != null )
                timerKeepAlive.Change(Timeout.Infinite, Timeout.Infinite);
        }
        public bool Login()
        {
            Debug.Print("JetSet.pro logging in");
            lock (loginLock)
            {
                try
                {
                    IsLoggedIn = false;

                    if (String.IsNullOrEmpty(_user) || String.IsNullOrEmpty(_password))
                    {
                        Debug.Print("JetSet.pro: Username or password is empty");
                        return false;
                    }
                    Debug.Print("Jetset.pro: get home page {0}", homeUrl);
                    var result = loginWC.DownloadString(homeUrl);

                    var loginParams = String.Format("login={0}&pass={1}", _user, _password);
                    loginWC.ContentType = ContentType.UrlEncodedUTF8;

                    Debug.Print("Jetset.pro: send login info to {0}", loginUrl);
                    loginWC.UploadString(loginUrl, loginParams);

                    String remember = loginWC.CookiesStrings.FirstOrDefault(item => item.Key == "remember").Value;

                    if (string.IsNullOrEmpty(remember))
                    {
                        Debug.Print("Jetset.pro: login info cookie 'remember' isn't set");
                        return false;
                    }

                    remember = HttpUtility.UrlDecode(remember);


                    ArrayList loginObj = (ArrayList)serializer.Deserialize(remember);

                    if (loginObj == null || loginObj.GetType() != typeof(ArrayList) || loginObj.Count != 3)
                    {
                        Debug.Print("Jetset.pro: unable to parse login json object");
                        return false;
                    }
                    userId = (string)loginObj[0];
                    hash1 = (string)loginObj[1];
                    hash2 = (string)loginObj[2];

                    Debug.Print("Jetset.pro: get home page {0}", homeUrl);
                    result = loginWC.DownloadString(streamsUrl);

                    if (String.IsNullOrEmpty(result))
                    {
                        Debug.Print("Jetset.pro: can't get streams url {0}", streamsUrl);
                        return false;
                    }

                    _channelUrl = Re.GetSubString(result, reChannelUrl, 1);

                    if (String.IsNullOrEmpty(_channelUrl))
                    {
                        Debug.Print("Jetset.pro: can't get channel url {0}", _channelUrl);
                        return false;
                    }

                    result = loginWC.DownloadString(String.Format(chatJsUrl,TimeUtils.UnixTimestamp()));

                    var configVer = Re.GetSubString(result, reConfigVer, 1);
                    var config = String.Format(configTemplate, configVer, TimeUtils.UnixTimestamp());

                    loginWC.setCookie("config", HttpUtility.UrlEncode(config), "jetset.pro");

                    result = loginWC.DownloadString(baseUrl + _channelUrl);

                    roomid = Re.GetSubString(result, reRoomId, 1);
                    token = Re.GetSubString(result, reToken, 1);
                    rndtoken = Re.GetSubString(result, reRndToken, 1);

                    loginWC.setCookie("RND_TOKEN", rndtoken, "jetset.pro");
                    
                    loginWC.CookiesStrings.ForEach(pair => { Debug.Print("{0}: {1}", pair.Key, pair.Value); });

                    var sessionid = loginWC.DownloadString( "http://" + socketDomain + ":8080/socket.io/1/?t=" + TimeUtils.UnixTimestamp());
                    sessionid = Re.GetSubString(sessionid, reSessId, 1);

                    Domain = socketDomain;
                    Port = "8080";
                    Path = "/socket.io/1/websocket/" + sessionid;
                    Cookies = loginWC.CookiesStrings;
                    Connect();
                    IsLoggedIn = true;
                }
                catch (Exception e)
                {
                    Debug.Print("JetSet: login exception {0} {1} {2} ", loginWC.URL, e.Message, e.StackTrace.ToString());
                }

                return true;
            }
        }
        public long LastTimeStamp
        {
            get;
            set;
        }
        public override void OnMessage(string message)
        {            
            Debug.Print("JetSet.pro message: {0}", message);
            try
            {
                if (message.StartsWith("1::"))
                    if (OnLogin != null)
                        OnLogin(this, EventArgs.Empty);

                if (message.StartsWith("5::/chat:"))
                {
                    message = message.Replace("5::/chat:", "");
                    JObject obj = JObject.Parse(message);
                    if (obj == null)
                        return;

                    if ((string)obj["name"] == "LOAD")
                    {
                        foreach( var arg in obj["args"] )
                        {
                            var messages = arg["messages"];
                            foreach (var msg in messages)
                            {
                                if( msg!= null)
                                {

                                    ReceiveMessage(JObject.Parse((string)msg));
                                }
                            }
                        }
                        
                    }
                    else if((string)obj["name"] == "MSG")
                    {
                        foreach( JObject msg in obj["args"] )
                        {
                            ReceiveMessage(msg);
                        }
                    }
                }
           

            }
            catch(Exception e)
            {
                Debug.Print("JetSet: error parsing message: {0} {1} {2}", message, e.Message, e.StackTrace);
            }
        }

        public void SendMessage(string message)
        {
            Send("3::/chat:" + message);
        }
        private void ReceiveMessage( JObject msg )
        {
            try
            {                
                var ts = (long)msg["ts"];
                var from = (string)msg["login"];
                var text = (string)msg["message"];
                if (ts > LastTimeStamp && OnMessageReceived != null)
                {
                    LastTimeStamp = ts;
                    OnMessageReceived(this, new JetSetMessage() { Text = text, User = from, TimeStamp = ts.ToString() });
                }
            }
            catch(Exception e) {
                Debug.Print("JetSet: error parsing a message");
            }
        }

        public void SendChatLogin()
        {
            var command = String.Format(
                            @"5::/chat:{{""name"":""INIT"",""args"":[{{""room_id"":{0},""section"":""channel"",""user_id"":{1},""token"":""{2}""}}]}}"
                            , roomid, userId, token);
            Send(command);
        }
        public void SendHello()
        {
            Send("1::/chat");
        }
        public override void OnConnect()
        {
            Debug.Print("JetSet.pro connected to websocket");
            SendHello();
            SendChatLogin();
            timerKeepAlive = new Timer(timerKeepAliveTick, null, PING_PERIOD, PING_PERIOD);
        }
        private void timerKeepAliveTick(object state)
        {
            Send("2::");
        }
        private string random_string()
        {
            var chars = "abcdefghijklmnopqrstuvwxyz0123456789_";
            StringBuilder builder = new StringBuilder();
            var random = new Random();

            for (var i = 0; i < 20; i++)
                builder.Append(chars[random.Next(0, chars.Length - 1)]);

            return builder.ToString();
        }

    }
    public class JetSetMessage : EventArgs
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
        public string TimeStamp
        {
            get;
            set;
        }
    }
}
