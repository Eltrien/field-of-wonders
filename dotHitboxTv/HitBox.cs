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
        private const int STATS_PERIOD = 30000;
        private const String domain = @"hitbox.tv";
        private const String baseUrl = @"http://" + domain;
        private const String loginUrl = @"http://www.hitbox.tv/api/auth/login";

        private const String apiUrl = @"http://api." + domain + @"/media/live/{0}";
        private const String socketDomain = "chat.hitbox.tv";
        private const String homeUrl = baseUrl;
        private String _user, _password;
        private string userId;
        private string hash1;
        private string token;
        private Timer timerStats;
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
            timerStats = new Timer(timerStatsTick, null, 0,STATS_PERIOD);
        }
        
        public void Stop()
        {
            if( timerStats != null )
                timerStats.Change(Timeout.Infinite, Timeout.Infinite);
        }
        public UInt32 Viewers
        {
            get;
            set;
        }
        private void timerStatsTick(object state)
        {
            lock (loginLock)
            {
                if( !String.IsNullOrEmpty( _user ))
                {
                    try
                    {
                       var result = loginWC.DownloadString( String.Format(apiUrl, _user ));
                        JToken statsObj = JToken.Parse(result);
                        if (statsObj == null)
                        {
                            Debug.Print("Hitbox: Unable to parse stats json");
                            return;
                        }

                        JToken livestreams = statsObj["livestream"];
                        UInt32 counter = 0;
                        foreach (JObject cat in livestreams)
                        {
                            counter += cat["category_viewers"].Value<UInt32>();
                        }
                        Viewers = counter;
                    }
                    catch(Exception e )
                    {
                        Debug.Print("Hitbox: Stats download error {0} {1}", String.Format(apiUrl,_user), e.InnerException.Message);
                    }
                }
            }
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

                    loginWC.setCookie("lang", @"%22en_US%22", domain);

                    var result = loginWC.DownloadString(homeUrl);

                    var loginParams = String.Format(@"{{""login"":""{0}"",""pass"":""{1}"",""rememberme"":""""}}", _user, _password);


                    Debug.Print("Hitbox: send login info to {0}", loginUrl);
                    loginWC.Headers["Accept"] = @"application/json, text/plain, */*";
                    loginWC.Headers["Content-Type"] = @"application/json;charset=UTF-8";
                    loginWC.Headers["Accept-Encoding"] = "gzip,deflate,sdch";
                    

                    byte[] data = Encoding.UTF8.GetBytes(loginParams);
                    var byteresult = loginWC.UploadData(loginUrl, "POST", data);
                    result = Encoding.UTF8.GetString(byteresult);

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
                    return false;
                }
                if (OnLogin != null)
                    OnLogin(this, EventArgs.Empty);

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
                if (message.Contains(@"chatMsg"))
                {
                    JObject msgObj = JObject.Parse(message);
                    if (msgObj == null)
                        return;

                    ReceiveMessage(msgObj["params"].ToObject<JObject>());
                }
           

            }
            catch(Exception e)
            {
                Debug.Print("HitBox: error parsing message: {0} {1} {2}", message, e.Message, e.StackTrace);
            }
        }

        public void SendMessage(string message)
        {
            String cmd = @"{{""method"":""chatMsg"",""params"":{{""channel"":""{0}"",""name"":""{1}"",""nameColor"":""8000FF"",""text"":""{2}""}}}}";
            Send(String.Format(cmd, _user, _user, message));
        }
        private void ReceiveMessage( JObject msg )
        {
            try
            {                
                var ts = msg["time"].Value<long>();
                var from = msg["name"].Value<string>();
                var text = msg["text"].Value<string>();
                if (ts > LastTimeStamp && OnMessageReceived != null)
                {
                    LastTimeStamp = ts;
                    if( !from.Equals(_user, StringComparison.CurrentCultureIgnoreCase))
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
                            @"{{""method"":""joinChannel"",""params"":{{""channel"":""{0}"",""name"":""{1}"",""token"":""{2}"",""isAdmin"":true}}}}"
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
