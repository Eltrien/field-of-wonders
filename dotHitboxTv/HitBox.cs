using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using dotWebSocket;
using dotWebClient;
using System.Diagnostics;
using System.Web;
using System.Collections;
using dot.Json.Linq;
using dotUtilities;

namespace dotHitboxTv
{
    public class HitBox : UbiWebSocket
    {
        private object loginLock;
        private const int PING_PERIOD = 25000;
        private const String domain = @"hitbox.tv";
        private const String baseUrl = @"http://" + domain;
        private const String loginUrl = baseUrl + "/api/auth/login";
        private const String socketDomain = "chat.hitbox.tv";
        private const String homeUrl = baseUrl;
        private String _user, _password;
        private string userId;
        private string hash1;
        private string token;
        public event EventHandler<HitBoxMessage> OnMessageReceived;
        public event EventHandler<EventArgs> OnLogin;

        private CookieAwareWebClient loginWC;
        public HitBox(string user, string password)
        {
            loginLock = new object();
            loginWC = new CookieAwareWebClient();
            _user = user;
            _password = password;
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
            
        }
        public bool Login()
        {
            Debug.Print("Hitbox logging in");
            lock (loginLock)
            {
                try
                {
                    IsLoggedIn = false;

                    if (String.IsNullOrEmpty(_user) || String.IsNullOrEmpty(_password))
                    {
                        Debug.Print("Hitbox: Username or password is empty");
                        return false;
                    }
                    Debug.Print("Hitbox: get home page {0}", homeUrl);
                    var result = loginWC.DownloadString(homeUrl);

                    var loginParams = String.Format("login={0}&pass={1}", _user, _password);
                    loginWC.ContentType = ContentType.UrlEncodedUTF8;
                    loginWC.Headers["Accept"] = @"application/json, text/plain, */*";

                    Debug.Print("Hitbox: send login info to {0}", loginUrl);
                    result = loginWC.UploadString(loginUrl, loginParams);

                    if (String.IsNullOrEmpty(result) || !result.Contains("user_id"))
                    {
                        Debug.Print("Hitbox: login failed");
                        return false;
                    }
                    JToken loginObj = JToken.Parse(result);

                    if (loginObj == null)
                    {
                        Debug.Print("Hitbox: Unable to parse login info");
                        return false;
                    }

                    userId = loginObj["user_id"].ToString();
                    hash1 = loginObj["user_password"].ToString();
                    token = loginObj["authToken"].ToString();

                    Domain = socketDomain;
                    Port = "8000";
                    Path = "/chat";
                    Cookies = loginWC.CookiesStrings;
                    Connect();
                    IsLoggedIn = true;
                }
                catch (Exception e)
                {
                    Debug.Print("HitBox: login exception {0} {1} {2} ", loginWC.URL, e.Message, e.StackTrace.ToString());
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
            Debug.Print("HitBox message: {0}", message);
            try
            {
                if (message.Contains(@"{""method"":""chatMsg"""))
                {
                    JToken msgObj = JToken.Parse(message);
                    if (msgObj == null)
                        return;

                    ReceiveMessage(msgObj);
                }
           

            }
            catch(Exception e)
            {
                Debug.Print("HitBox: error parsing message: {0} {1} {2}", message, e.Message, e.StackTrace);
            }
        }

        public void SendMessage(string message)
        {
            String cmd = @"{""method"":""chatMsg"",""params"":{""channel"":""{{0}}"",""name"":""{{1}}"",""nameColor"":""8000FF"",""text"":""{{2}}""}}";
            Send(String.Format(cmd, _user, _user, message));
        }
        private void ReceiveMessage( JToken msg )
        {
            try
            {                
                var ts = (long)msg["time"];
                var from = (string)msg["name"];
                var text = (string)msg["text"];
                if (ts > LastTimeStamp && OnMessageReceived != null)
                {
                    LastTimeStamp = ts;
                    OnMessageReceived(this, new HitBoxMessage() { Text = text, User = from, TimeStamp = ts.ToString() });
                }
            }
            catch(Exception e) {
                Debug.Print("HitBox: error parsing a message");
            }
        }

        public void SendChatLogin()
        {
            var command = String.Format(
                            @"{""method"":""joinChannel"",""params"":{""channel"":""{{0}}"",""name"":""{{1}}"",""token"":""{{2}}"",""isAdmin"":true}}"
                            , _user, _user, token);
            Send(command);
        }
        public override void OnConnect()
        {
            Debug.Print("HitBox connected to websocket");
            SendChatLogin();
            PingInterval = PING_PERIOD;
        }

    }
    public class HitBoxMessage : EventArgs
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
