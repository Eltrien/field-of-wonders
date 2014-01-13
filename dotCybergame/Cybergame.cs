using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Web;
using System.Net;
using System.Threading;
using dotWebClient;
using dotUtilities;
using dotWebSocket;
using dotInterfaces;
using System.Configuration;
using System.Xml.Serialization;
using dot.Json;
using dot.Json.Linq;
using dotHtmlParser;

namespace dotCybergame
{
    public class Cybergame : UbiWebSocket,IChatDescription
    {
        #region Constants
        private const string userAgent = "Mozilla/5.0 (Windows NT 6.0; WOW64; rv:14.0) Gecko/20100101 Firefox/14.0.1";
        private const string domain = "cybergame.tv";
        private const string channelInfoUrl = "http://api.cybergame.tv/p/statusv2/?channel={0}";
        private const string channelUrl = "http://" + domain + "/{0}";
        private const string chatUrl = "http://" + domain + "/{0}/";
        private const string adminAjaxUrlNew = "http://" + domain + "/v1/wp-admin";
        private const string adminAjaxUrl = "http://" + domain + "/v1/wp-admin/admin-ajax.php";
        private const int maxServerNum = 0x1e3;
        private const string qwebBaseURL = @"http://" + domain + ":9090/e/";

        private const string loginUrl = "http://" + domain + "/login.php";
        private const string oldLoginUrl = "http://" + domain + "/v1/cabinet_login.php";
        private const string cabinetUrl = "http://" + domain + "/my_profile_edit/";
        private const string sendMessageUrl = "http://" + domain + "/v1/wp-admin/admin-ajax.php";
        private const string cabinetUpdateUrl = @"http://" + domain + "/my_profile_edit/?mode=async&rand={0}";

        private const string reKName = @"kname[^""]+""([^""]+)""";
        private const string reKHash = @"khash[^""]+""([^""]+)""";

        private const string reChatUpdateMessageNonce = @",""quick_chat_update_messages_nonce"":""([^""]+)"",";
        private const string reChatNewMessageNonce = @",""quick_chat_new_message_nonce"":""([^""]+)""";
        private const string reChatLastTimestamp = @",""quick_chat_last_timestamp"":(\d+),";

        private const int pollIntervalChat = 3000;
        private const int pollIntervalStats = 20000;

        #endregion     
        #region Private properties
        private string khash,kname;
        private Timer statsDownloader;
        private CookieAwareWebClient loginWC;
        private CookieAwareWebClient chatWC;
        private CookieAwareWebClient statsWC;
        private Channel currentChannelStats;
        private string _user;
        private string _password;
        private string _channelId;
        private object chatLock = new object();
        private object messageLock = new object();
        private object loginLock = new object();
        private object statsLock = new object();
        private object restartLock = new object();

        private bool stopped = true;


        #endregion
        #region Events
        public event EventHandler<EventArgs> Live;
        public event EventHandler<EventArgs> Offline;
        public event EventHandler<EventArgs> OnLogin;
        private void DefaultEvent(EventHandler<EventArgs> evnt, EventArgs e)
        {
            EventHandler<EventArgs> handler = evnt;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        private void OnLive( EventArgs e)
        {
            DefaultEvent(Live, e);
        }
        private void OnOffline(EventArgs e)
        {
            DefaultEvent(Offline, e);
        }
        public event EventHandler<MessageReceivedEventArgs> OnChatMessage;
        #endregion

        #region Public methods
        public Cybergame(string user, string password)
        {
            _user = user.ToLower();
            _password = password;
            
            loginWC = new CookieAwareWebClient();
            statsWC = new CookieAwareWebClient();
            chatWC = new CookieAwareWebClient();
            loginWC.Headers["User-Agent"] = userAgent;
            isLoggedIn = false;
            
            GameList = new List<KeyValuePair<string, string>>();
            statsDownloader = new Timer(new TimerCallback(statsDownloader_Tick), null, Timeout.Infinite, Timeout.Infinite);

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
            stopped = false;
            statsDownloader.Change(0,pollIntervalStats);
        }
        public void Stop()
        {
            stopped = true;
            statsDownloader.Change(Timeout.Infinite, Timeout.Infinite);           
            
        }
        
        public bool Login()
        {
            if (stopped)
                return false;

            lock (chatLock)
            {
                try
                {

                    if (String.IsNullOrEmpty(_user) || String.IsNullOrEmpty(_password))
                        return false;

                    chatWC = new CookieAwareWebClient();

                    chatWC.Headers["Cache-Control"] = "no-cache";
                    chatWC.DownloadString(loginUrl);
                    chatWC.setCookie("kt_is_visited", "1", ".cybergame.tv");
                    chatWC.setCookie("kt_tcookie", "1", ".cybergame.tv");

                    string loginParams = String.Format("action=login&username={0}&pass={1}&remember_me=1", _user, _password);

                    var res = chatWC.postFormDataLowLevel(loginUrl, loginParams);
                    
                    if (string.IsNullOrEmpty(res))
                    {
                        Debug.Print("Cybergame: couldn't get login URL");
                        return false;
                    }

                    res = chatWC.DownloadString(loginUrl);
                    if (!res.Contains("logout.php"))
                    {
                        Debug.Print("Cybergame: wrong credentials");
                        return false;
                    }


                    var result = chatWC.DownloadString(String.Format(chatUrl, String.Empty));
                    
                    kname = Re.GetSubString(result, reKName, 1);
                    khash = Re.GetSubString(result, reKHash, 1);

                    if (String.IsNullOrEmpty(khash))
                    {
                        Debug.Print("Cybergame: couldn't get khash");
                        return false;
                    }

                    chatWC.setCookie("khame", kname, ".cybergame.tv");
                    chatWC.setCookie("khash", khash, ".cybergame.tv");


                    statsWC.Cookies = chatWC.Cookies;
                    loginWC.Cookies = chatWC.Cookies;

                    Debug.Print("Cybergame: connecting web socket");
                    Domain = domain;
                    Port = "8080";
                    Path = String.Format("/{0}/{1}/websocket", random_number(), random_string());
                    Cookies = statsWC.CookiesStrings;
                    Connect();

                    return true;
                }
                catch (Exception e)
                {
                    Debug.Print(String.Format("Cybergame: login exception {0}", e.Message));
                    return false;
                }
            }
        }
        private void SendAuth()
        {
            String cmd = @"[""{\""command\"":\""login\"",\""message\"":\""{\\\""login\\\"":\\\""" + kname + @"\\\"",\\\""password\\\"":\\\""" + khash + @"\\\"",\\\""channel\\\"":\\\""#" + kname + @"\\\""}\""}""]";
            Send( cmd );
        }
        public override void OnMessage(String message)
        {
            lock (messageLock)
            {
                if (string.IsNullOrEmpty(message))
                    return;
             
                Debug.Print("Cybergame message: {0}", message);
                if (message == "o")
                {
                    SendAuth();
                    return;
                }
                if (message.Contains(@"a[""{\""command\"":\"""))
                {
                    try
                    {
                        JArray commands = JArray.Parse( message.Substring(1) );
                        if( commands.Count <= 0 )
                            return;

                        foreach (String strCmd in commands)
                        {

                            JToken cmd = JToken.Parse(strCmd);
                            switch ((String)cmd["command"])
                            {
                                   
                                case "auth":
                                    {
                                        if (!((String)cmd["message"]["message"]).Contains(@"Your nick isn't registered"))
                                        {
                                            isLoggedIn = true;
                                            if (OnLogin != null)
                                                OnLogin(this, EventArgs.Empty);
                                        }

                                    }
                                    break;
                                case "chatMessage":
                                    {

                                        if( OnChatMessage != null )
                                        {
                                            JToken objMsg = JToken.Parse(cmd["message"].ToString());

                                            if (objMsg == null)
                                                return;

                                            String user = objMsg["from"].ToString();
                                            String text = objMsg["text"].ToString();

                                            var chatMessage = new Message() { message = text, alias = user };
                                            OnChatMessage(this, new MessageReceivedEventArgs(chatMessage));
                                        }
                                    }
                                    break;
                                    default:
                                        Debug.Print("Cybergame unparsed message: " + message);
                                        break;

                            }
                        }
                    }
                    catch( Exception e )
                    {
                        Debug.Print("Cybergame OnMessage error: {0}", e.Message);
                    }
                }
            }
        }
        public override void OnDisconnect()
        {
            if (stopped)
                return;
            isLoggedIn = false;

            ThreadPool.QueueUserWorkItem( f=> Restart(1000));
        }
        public override void OnConnect()
        {

        }
        public void Restart(int timeout)
        {

            lock (restartLock)
            {

                if (stopped)
                    return;

                Thread.Sleep(timeout);
                if (isLoggedIn)
                    return;

                Login();
            }
        }

        public bool SendMessage(string message)
        {
            String template = @"[""{{\""command\"":\""chatMessage\"",\""message\"":\""{{\\\""message\\\"":\\\""{0}\\\""}}\""}}""]";
            Send(String.Format(template, message));
            return true;
        }
        public bool StartAdvertising()
        {
            if (!isLoggedIn)
                return false;

            try
            {
                UTF8Encoding encoder = new UTF8Encoding();
                loginWC.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded; charset=UTF-8";

                string adsParams = String.Format("xjxfun=start_ad&xjxr={0}", TimeUtils.UnixTimestamp());
                var result = loginWC.UploadString(cabinetUrl, adsParams);
            }
            catch( Exception e )
            {
                Debug.Print("Exception in Cybergame StartAdvertising(): {0}", e.Message);
            }

            return true;
        }
        private void DownloadStats( string channel )
        {
            if (String.IsNullOrEmpty(_user))
                return;
            lock (statsLock)
            {
                var prevChannelStats = currentChannelStats;
                try
                {
                    statsWC.Headers["Cache-Control"] = "no-cache";
                    var url = String.Format(channelInfoUrl, channel);
                    using (var stream = statsWC.downloadURL(url))
                    {
                        if (statsWC.LastWebError == "ProtocolError")
                        {
                            return;
                        }

                        if (stream == null)
                        {
                            Debug.Print("Cybergame: Can't download channel info of {0} result is null. Url: {1}", channel, channelInfoUrl);
                        }
                        else
                        {
                            currentChannelStats = JsonGenerics.ParseJson<Channel>.ReadObject(stream);

                        }

                        if (!Alive() && prevChannelStats != currentChannelStats)
                            OnOffline(EventArgs.Empty);
                        else if (Alive() && prevChannelStats != currentChannelStats)
                            OnLive(EventArgs.Empty);
                    }
                }
                catch
                {
                    Debug.Print("Cybergame: Exception in Downloadstats");
                }
            }
        }
        private bool Alive()
        {
            if (currentChannelStats == null)
                return false;

            if (currentChannelStats.online == "0")
                return false;

            if (currentChannelStats.online == "1")
                return true;

            return false;
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
        private String XHttpPost(String url, String param)
        {
            lock (loginLock)
            {
                loginWC.ContentType = ContentType.UrlEncodedUTF8;
                loginWC.Headers["X-Requested-With"] = "XMLHttpRequest";
                try
                {
                    var result = loginWC.UploadString(url,param);
                    if (!String.IsNullOrEmpty(result))
                        return result;
                    else
                        Debug.Print("Cybergame: xhttppost - empty string fetched from {0}", url);
                }
                catch (Exception e)
                {
                    Debug.Print("Cybergame error posting to {0}: {1}", url, e.Message);
                }
            }
            return String.Empty;
        }
        private String HttpGet(String url)
        {
            lock (loginLock)
            {
                loginWC.ContentType = ContentType.UrlEncodedUTF8;
                //loginWC.Headers["X-Requested-With"] = "XMLHttpRequest";
                try
                {
                    var result = loginWC.DownloadString(url);
                    if (!String.IsNullOrEmpty(result))
                        return result;
                    else
                        Debug.Print("Cybergame: httpget - empty string fetched from {0}", url);
                }
                catch (Exception e)
                {
                    Debug.Print("Cybergame error fetching {0}: {1}", url, e.Message);
                }
            }
            return String.Empty;
        }
        private List<KeyValuePair<String, String>> ChannelFormParams
        {
            get;
            set;
        }
        #endregion
        #region Public properties
        public string Viewers
        {
            get { return Alive() ? currentChannelStats.viewers:"0"; }
            set { }
        }
        #endregion

#region IChatDescription
        public void SetDescription()
        {
            if (ChannelFormParams == null)
                return;

            String param = "a=save_profile&channel_game={0}&channel_desc={1}&channel={2}&display_name={3}&channel_name={4}";
            var displayName = ChannelFormParams.Where(k => k.Key.StartsWith("display_name_", StringComparison.CurrentCultureIgnoreCase)).Select(v => v.Value).FirstOrDefault();
            var channelName = ChannelFormParams.Where(k => k.Key.StartsWith("channel_name_", StringComparison.CurrentCultureIgnoreCase)).Select(v => v.Value).FirstOrDefault();
            if (String.IsNullOrEmpty(displayName))
                displayName = kname;
            
            GameId = GameList.Where(v => v.Value == Game).Select(g => g.Key).FirstOrDefault();

            var result = XHttpPost(cabinetUpdateUrl, String.Format(param, GameId,HttpUtility.UrlEncode(ShortDescription),_channelId,HttpUtility.UrlEncode(displayName),HttpUtility.UrlEncode(channelName)));
        }

        public void GetDescription()
        {
            if (!isLoggedIn)
                return;

            var result = HttpGet(cabinetUrl);
            if (!String.IsNullOrEmpty(result))
            {
                ShortDescription = HtmlParser.GetInnerText(@"//textarea[@name='channel_desc']", result);
                GameList = HtmlParser.GetOptions(@"//select[@name='channel_game']/option",result);                
                Game = HtmlParser.GetSiblingInnerText(@"//select/option[@selected='selected']", result);
                _channelId = HtmlParser.GetAttribute(@"//input[@name='channel']","value", result);
                GameId = GameList.Where(v => v.Value == Game).Select(g => g.Key).FirstOrDefault();
                ChannelFormParams = HtmlParser.FormParams(@"", result);
                //foreach (var p in ChannelFormParams)
                //{
                //    Debug.Print("Cybergame channel form param: {0} = {1}", p.Key, p.Value);
                //}
                if (String.IsNullOrEmpty(Game))
                {
                    Game = "другое";
                    GameId = "1";
                }
                



            }
        }
        public string Game
        {
            get;
            set;
        }
        public string GameId
        {
            get;
            set;
        }
        public string LongDescription
        {
            get;
            set;
        }
        public string ShortDescription
        {
            get;
            set;
        }
#endregion
        public List<KeyValuePair<String, String>> GameList
        {
            get;
            set;
        }
    }

    public class MessageReceivedEventArgs : EventArgs
    {
        public MessageReceivedEventArgs(Message message)
        {
            Message = message;
        }

        public Message Message { get; private set; }
    }
}
