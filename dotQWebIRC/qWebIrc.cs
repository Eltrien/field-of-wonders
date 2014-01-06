using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dotWebClient;
using dot.Json;
using dot.Json.Linq;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace dotQWebIRC
{
    public class qWebIrc
    {
        private CookieAwareWebClient wc, wcSend;
        private object lockWC,lockSend;
        private string _user, _hash;
        private UInt32 _actionIndex;
        private bool stop;
        private Timer timerIRCQuery;
        private Timer timerPingTimeout;
        private Timer timerPing;
        private const int POLL_PERIOD = 10;
        private const int PING_TIMEOUT = 60000;
        private const int PING_INTERVAL = 60000;
        public qWebIrc()
        {
            stop = false;
            PingTimeout = PING_TIMEOUT;
            PingInterval = PING_INTERVAL;
            lockWC = new object();
            lockSend = new object();
            CachePrevent = random_string();
            ActionIndex = 0;
            wc = new CookieAwareWebClient();
            wcSend = new CookieAwareWebClient();

            timerIRCQuery = new Timer(timerQueryTick, null, Timeout.Infinite, Timeout.Infinite);
            timerPingTimeout = new Timer(timerPingTimeoutTick, null, Timeout.Infinite, Timeout.Infinite);
            timerPing = new Timer(timerPingTick, null, Timeout.Infinite, Timeout.Infinite);
            
        }
        public CookieContainer Cookies
        {
            set { wc.Cookies = value; wcSend.Cookies = wc.Cookies; }
            get { return wc.Cookies; }
        }
        public string BaseURL
        {
            get;
            set;
        }
        public int PingInterval
        {
            get;
            set;
        }
        public virtual void Auth( string user, string hash )
        {
            stop = false;
            _user = user;
            _hash = hash;
            SendAuthInfo();
        }
        public int PingTimeout
        {
            get;
            set;
        }
        public void SendAuthInfo()
        {
            if (stop)
                return;
            lock (lockWC)
            {
                try
                {
                    wc.ContentType = ContentType.UrlEncodedUTF8;
                    var result = wc.UploadString(AuthURL, LoginParams);
                    var seedArray = GetArray(result);
                    if (seedArray == null)
                    {
                        OnFailAuth();
                        return;
                    }

                    Seed = (string)seedArray[1];
                    if (String.IsNullOrEmpty(Seed))
                    {
                        OnFailAuth();
                        return;
                    }
                    timerIRCQuery.Change(0, Timeout.Infinite);
                }
                catch (Exception e)
                {
                    Debug.Print("qWebIrc SendAuthInfo error: {0}", e.Message);
                }
            }
        }
        public virtual void OnFailAuth()
        {
        }
        public virtual void OnSuccessAuth()
        {
        }
        private void timerQueryTick(object sender)
        {
            if (stop)
                return;
            
            ReadResponse();
        }
        private void timerPingTimeoutTick(object sender)
        {
            if (stop)
                return;
            
            OnPingTimeout();
        }
        private void timerPingTick(object sender)
        {
            Ping("localhost.localdomain");
            timerPingTimeout.Change(PingTimeout, Timeout.Infinite);
        }
        public virtual void OnPingTimeout()
        {
        }
        private String CachePrevent
        {
            get;
            set;
        }
        private UInt32 ActionIndex
        {
            get { _actionIndex++; return _actionIndex-1; }
            set { _actionIndex = value; }
        }
        private String Seed
        {
            get;
            set;
        }
        private void StopTimers()
        {
            stop = true;
            timerPingTimeout.Change(Timeout.Infinite, Timeout.Infinite);
            timerPing.Change(Timeout.Infinite, Timeout.Infinite);
            timerIRCQuery.Change(Timeout.Infinite, Timeout.Infinite);

        }
        private void ReadResponse()
        {
            if (stop)
                return;

            lock (lockWC)
            {
                try
                {
                    wc.ContentType = ContentType.UrlEncodedUTF8;
                    var result = wc.UploadString(QueryURL, QueryParams);
                    Debug.Print(result);
                    var response = GetArray(result);
                    
                    if (response != null && response.Count > 0)
                    {
                        foreach (JArray line in response)
                        {
                            if (line.Count > 0)
                            {
                                String command = (string)line[0];

                                switch (command.ToLower())
                                {
                                    case "disconnect":
                                        {
                                            StopTimers();
                                            OnDisconnect();
                                        }
                                        break;
                                    case "connect":
                                        OnConnect();
                                        break;
                                    case "c":
                                        {
                                            String subcommand = (string)line[1];
                                            switch (subcommand.ToLower())
                                            {                                                
                                                case "433":
                                                    {
                                                        OnNotice("NickServ", _user, "Nickname is already in use.");
                                                    }
                                                    break;
                                                case "ping":
                                                    {
                                                        var dst = (string)line[3][0];
                                                        Pong(dst);

                                                    }
                                                    break;
                                                case "pong":
                                                    {
                                                        timerPingTimeout.Change(Timeout.Infinite, Timeout.Infinite);
                                                    }
                                                    break;
                                                case "notice":
                                                    {
                                                        var source = (string)line[2];
                                                        var dst = (string)line[3][0];
                                                        var text = (string)line[3][1];
                                                        OnNotice(source, dst, text);
                                                    }
                                                    break;
                                                case "join":
                                                    {
                                                        OnJoin();
                                                        timerPing.Change(0, PingInterval);
                                                    }
                                                    break;
                                                case "privmsg":
                                                    {
                                                        var from = string.Concat(((string)line[2]).TakeWhile( c => c != '!' ));
                                                        var text = (string)line[3][1];
                                                        OnPrivMessage(from, text );
                                                    }
                                                    break;
                                            }
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }
                catch( Exception e )
                {
                    Debug.Print("QWebIrc: Read response: " + e.Message);
                    Debug.Print(e.StackTrace);
                    OnError();
                    return;
                }
                if( !stop )
                    timerIRCQuery.Change(0, Timeout.Infinite);
            }
        }
        public virtual void OnDisconnect()
        {
        }
        public virtual void OnError()
        {

        }
        public void ReleaseNickName(string user, string password)
        {
            SendToUser("nickserv!localhost.localdomain", String.Format("RECOVER {0} {1}", user,password));
            SendToUser("nickserv!localhost.localdomain", String.Format("RELEASE {0} {1}", user,password));

        }
        public void Quit(string quitmessage)
        {
            RawMessage(String.Format("QUIT :{0}",quitmessage));
        }
        public void RawMessage(string message)
        {
            lock (lockSend)
            {
                try
                {
                    wcSend.ContentType = ContentType.UrlEncodedUTF8;
                    var result = wcSend.UploadString(PostURL, QueryParams + String.Format("&c={0}", message));
                    Debug.Print(result);
                }
                catch (Exception e)
                {
                    Debug.Print("QWebIrc: error sending raw message {0} {1} {2} ", message, e.Message, e.StackTrace);
                }
            }
        }
        public void SendToUser(string user, string message)
        {
            Debug.Print("QWebIrc: send message to user");
            RawMessage(String.Format("PRIVMSG {0} :{1}", user, message));
        }
        public void SendToChannel(string message)
        {
            Debug.Print("QWebIrc: send message " + message);
            RawMessage(String.Format("PRIVMSG #{0} :{1}", _user, message));            
        }

        public void Pong(string dst)
        {
            Debug.Print("QWebIrc: pong " + dst);
            RawMessage(String.Format("PONG :{0}", dst));
        }
        public void Ping(string dst)
        {
            Debug.Print("QWebIrc: ping " + dst);
            RawMessage(String.Format("PING :{0}", dst));
        }

        public void HideIP()
        {
            RawMessage(String.Format("MODE {0} +x", _user));
        }
        public void Join( String channel)
        {
            RawMessage(String.Format("JOIN #{0}", channel));
        }
        public virtual void OnPrivMessage(String from, String text)
        {
        }
        public virtual void OnJoin()
        {

        }
        public virtual void OnNotice(String source, String dst, String text)
        {
            
        }
        public virtual void OnConnect()
        {
        }
        private String LoginParams
        {
            get { return String.Format("nick={0}&password={1}", _user, _hash); }
        }
        private String QueryParams
        {
            get { return String.Format("s={0}", Seed); }
        }
        private String DefaultURLParams
        {
            get { return String.Format("r={0}&t={1}", CachePrevent, ActionIndex); }
        }
        private String AuthURL
        {
            get { return String.Format("{0}n?{1}", BaseURL, DefaultURLParams); }
        }
        private String QueryURL
        {
            get { return String.Format("{0}s?{1}", BaseURL, DefaultURLParams); }
        }
        private String PostURL
        {
            get { return String.Format("{0}p?{1}", BaseURL, DefaultURLParams); }
        }
        private string random_string()
        {
            var chars = "abcdef0123456789";
            StringBuilder builder = new StringBuilder();
            var random = new Random();

            for (var i = 0; i < 32; i++)
                builder.Append(chars[random.Next(0, chars.Length - 1)]);

            return builder.ToString();
        }
        private JArray GetArray(string str)
        {
            try
            {
                if (str.StartsWith("["))
                {
                    str = @"{""a"":" + str + "}";
                    return (JArray)(JObject.Parse(str)["a"]);
                }
                else
                    return null;
            }
            catch
            {
                Debug.Print("qWebIrc: json parse failed. String: " + str);
                return null;
            }
        }
    }
}
