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

namespace dotCybergame
{
    public class Cybergame
    {
        #region Constants
        private const string userAgent = "Mozilla/5.0 (Windows NT 6.0; WOW64; rv:14.0) Gecko/20100101 Firefox/14.0.1";
        private const string channelInfoUrl = "http://api.cybergame.tv/p/statusv2/?channel={0}";
        private const string channelUrl = "http://cybergame.tv/{0}";
        private const string chatUrl = "http://cybergame.tv/v1/{0}/?chatonly=1";
        private const string adminAjaxUrlNew = "http://cybergame.tv/v1/wp-admin";
        private const string adminAjaxUrl = "http://cybergame.tv/v1/wp-admin/admin-ajax.php";

        private const string loginUrl = "http://cybergame.tv/login.php";
        private const string oldLoginUrl = "http://cybergame.tv/v1/cabinet_login.php";
        private const string cabinetUrl = "http://cybergame.tv/cabinet.php";
        private const string sendMessageUrl = "http://cybergame.tv/v1/wp-admin/admin-ajax.php";
        private const string reChatUpdateMessageNonce = @",""quick_chat_update_messages_nonce"":""([^""]+)"",";
        private const string reChatNewMessageNonce = @",""quick_chat_new_message_nonce"":""([^""]+)""";
        private const string reChatLastTimestamp = @",""quick_chat_last_timestamp"":(\d+),";
        private const int pollIntervalChat = 3000;
        private const int pollIntervalStats = 20000;

        #endregion     
        #region Private properties
        private string lastTimestamp;
        private string chatUpdateNonce;
        private string chatNewMessageNonce;
        private Timer statsDownloader;
        private Timer chatDownloader;
        private CookieAwareWebClient loginWC;
        private CookieAwareWebClient chatWC;
        private CookieAwareWebClient statsWC;
        private bool prevOnlineState = false;
        private Channel currentChannelStats;
        private string _user;
        private string _password;
        private object chatLock = new object();
        private object messageLock = new object();
        private object loginLock = new object();
        private object statsLock = new object();
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
        public event EventHandler<MessageReceivedEventArgs> OnMessage;
        #endregion

        #region Public methods
        public Cybergame(string user, string password)
        {
            _user = user;
            _password = password;
            loginWC = new CookieAwareWebClient();
            statsWC = new CookieAwareWebClient();
            chatWC = new CookieAwareWebClient();
            loginWC.Headers["User-Agent"] = userAgent;

            chatDownloader = new Timer(new TimerCallback(chatDownloader_Tick), null, Timeout.Infinite, Timeout.Infinite);
            statsDownloader = new Timer(new TimerCallback(statsDownloader_Tick), null, Timeout.Infinite, Timeout.Infinite);

        }

        private void chatDownloader_Tick(object o)
        {
            DownloadChat();
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
            chatDownloader.Change(0, pollIntervalChat);
            statsDownloader.Change(0,pollIntervalStats);
        }
        public void Stop()
        {
            chatDownloader.Change(Timeout.Infinite, Timeout.Infinite);
            statsDownloader.Change(Timeout.Infinite, Timeout.Infinite);
        }
        public bool Login()
        {
            lock (chatLock)
            {
                isLoggedIn = false;
                if (String.IsNullOrEmpty(_user) || String.IsNullOrEmpty(_password))
                    return false;

                chatWC = new CookieAwareWebClient();

                chatWC.Headers["Cache-Control"] = "no-cache";
                chatWC.DownloadString(loginUrl);
                chatWC.setCookie("kt_is_visited", "1", ".cybergame.tv" );
                chatWC.setCookie("kt_tcookie", "1", ".cybergame.tv");
                chatWC.setCookie("quick_chat_alias", _user, ".cybergame.tv");
                //string loginParams = String.Format("rememberme={0}&redirect_to=/{1}&log={1}&pwd={2}&submit=Войти", "forever", _user, _password);
                string loginParams = String.Format("action=login&username={0}&pass={1}", _user, _password);

                //chatWC.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";


                var res = chatWC.postFormDataLowLevel(loginUrl, loginParams);
                //var res = chatWC.UploadString(loginUrl, loginParams);
                if (string.IsNullOrEmpty(res))
                    return false;

                res = chatWC.DownloadString(loginUrl);
                if (!res.Contains("logout.php"))
                    return false;

                oldLogin();

                var result = chatWC.DownloadString(String.Format(chatUrl, _user));

                lastTimestamp = TimeUtils.UnixTimestamp();
                chatUpdateNonce = Re.GetSubString(result, reChatUpdateMessageNonce, 1);
                chatNewMessageNonce = Re.GetSubString(result, reChatNewMessageNonce, 1);

                lastTimestamp = Re.GetSubString(result, reChatLastTimestamp, 1);

                if (OnLogin != null)
                    OnLogin(this, EventArgs.Empty);

                //chatWC.Cookies = loginWC.Cookies;
                statsWC.Cookies = chatWC.Cookies;

                isLoggedIn = true;

                return true;
            }
        }
        public void oldLogin()
        {
            string loginParams = String.Format("rememberme={0}&redirect_to=/{1}&log={1}&pwd={2}&submit=Войти", "forever", _user, _password);
            lock (chatLock)
            {
                var res = chatWC.postFormDataLowLevel(oldLoginUrl, loginParams);
            }
        }
        public bool SendMessage(string message)
        {
            lock (chatLock)
            {
                if (!isLoggedIn)
                    return false;

                try
                {
                    UTF8Encoding encoder = new UTF8Encoding();
                    byte[] bytes = Encoding.UTF8.GetBytes(message);
                    string utf8msg = HttpUtility.UrlEncode(encoder.GetString(bytes));

                    chatWC.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    chatWC.Headers["X-Requested-With"] = "XMLHttpRequest";
                    chatWC.Headers[HttpRequestHeader.Accept] = "*/*";
                    string messageParams = String.Format("action=quick-chat-ajax-new-message&message={0}&room={1}&quick_chat_new_message_nonce={2}", utf8msg, _user, chatNewMessageNonce);
                    var result = chatWC.UploadString(sendMessageUrl, messageParams);

                    if (String.IsNullOrEmpty(result))
                        return false;

                    var newNonce = Re.GetSubString(result, reChatNewMessageNonce, 1);
                    if (string.IsNullOrEmpty(newNonce))
                        return false;
                    else
                        chatNewMessageNonce = newNonce;

                    Debug.Print("{0} {1}", messageParams, result);
                }
                catch(Exception e)
                {
                    Debug.Print("Exception in Cybergame SendMessage(): {0}", e.Message);
                }

                return true;
            }
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
        private void DownloadChat()
        {
            lock (chatLock)
            {
                //if (!isLoggedIn)
                //    return;

                if (String.IsNullOrEmpty(chatUpdateNonce))
                    return;


                string updateChatParams = String.Format(
                    "action=quick-chat-ajax-update-messages&quick_chat_last_timestamp={0}&quick_chat_rooms%5B%5D={1}&quick_chat_update_messages_nonce={2}", lastTimestamp, _user, chatUpdateNonce);
                chatWC.Headers["X-Requested-With"] = "XMLHttpRequest";
                chatWC.Headers[HttpRequestHeader.Referer] = String.Format(channelUrl, _user);
                chatWC.Headers[HttpRequestHeader.UserAgent] = userAgent;
                chatWC.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                chatWC.Headers[HttpRequestHeader.Accept] = "*/*";
                chatWC.Headers[HttpRequestHeader.AcceptLanguage] = "ru";
                chatWC.Headers[HttpRequestHeader.AcceptEncoding] = "deflate";
                chatWC.KeepAlive = true;

                string result = string.Empty;
                try
                {
                    result = chatWC.UploadString(adminAjaxUrl, updateChatParams);
                }
                catch( Exception e ) {
                    Debug.Print("Cybergame chat update exception: {0}", e.Message);
                }

                if (String.IsNullOrEmpty(result))
                    return;

                var chatObject = JsonGenerics.ParseJson<Chat>.ReadObject(result);
                if (chatObject == null)
                {
                    Debug.Print("Error parsing Cybergame json" + result);
                    return;
                }

                chatUpdateNonce = chatObject.updateid;
                if (chatObject.messages != null)
                {
                    lastTimestamp = chatObject.MaxTimestamp().ToString();
                    foreach (Message msg in chatObject.messages)
                    {
                        if (!String.IsNullOrEmpty(msg.message))
                            OnMessage(this, new MessageReceivedEventArgs(msg));
                    }
                }
            }

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
        #endregion
        #region Public properties
        public string Viewers
        {
            get { return Alive() ? currentChannelStats.viewers:"0"; }
            set { }
        }
        #endregion

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
