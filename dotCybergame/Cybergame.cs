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
        private const string adminAjaxUrl = "http://cybergame.tv/wp-admin/admin-ajax.php";
        private const string loginUrl = "http://cybergame.tv/cabinet_login.php";
        private const string cabinetUrl = "http://cybergame.tv/cabinet.php";
        private const string sendMessageUrl = "http://cybergame.tv/wp-admin/admin-ajax.php";
        private const string reChatUpdateMessageNonce = @",""quick_chat_update_messages_nonce"":""([^""]+)"",";
        private const string reChatNewMessageNonce = @",""quick_chat_new_message_nonce"":""([^""]+)""";
        private const string reChatLastTimestamp = @",""quick_chat_last_timestamp"":(\d+),";

        #endregion     
        #region Private properties
        private string lastTimestamp;
        private string chatUpdateNonce;
        private string chatNewMessageNonce;
        private Timer statsDownloader;
        private Timer chatDownloader;
        private CookieAwareWebClient statsWc;
        private CookieAwareWebClient chatWc;
        private bool prevOnlineState = false;
        private Channel currentChannel;
        private string _user;
        private string _password;
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
            statsWc = new CookieAwareWebClient();
            statsWc.Headers["User-Agent"] = userAgent;
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
        public bool Login()
        {
            isLoggedIn = false;
            if (String.IsNullOrEmpty(_user) || String.IsNullOrEmpty(_password))
                return false;
            
            chatWc = new CookieAwareWebClient();
            
            chatWc.Headers["Cache-Control"] = "no-cache";
            string loginParams = String.Format("rememberme={0}&redirect_to=/{1}&log={1}&pwd={2}&submit=Войти", "forever", _user, _password);

            chatWc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";            
            var res = chatWc.postFormDataLowLevel(loginUrl, loginParams);

            if (string.IsNullOrEmpty(res))
                return false;
            
            if (!res.Contains("wordpress_logged_in_"))
                return false;

            var result = chatWc.DownloadString(String.Format(channelUrl, _user));
            lastTimestamp = TimeUtils.UnixTimestamp();
            chatUpdateNonce = Re.GetSubString(result, reChatUpdateMessageNonce, 1);
            chatNewMessageNonce = Re.GetSubString(result, reChatNewMessageNonce, 1);

            lastTimestamp = Re.GetSubString(result, reChatLastTimestamp, 1);

            if (OnLogin != null)
                OnLogin(this, EventArgs.Empty);

            chatDownloader = new Timer(new TimerCallback(chatDownloader_Tick), null, 0, 3000);
            statsDownloader = new Timer(new TimerCallback(statsDownloader_Tick), null, 0, 20000);

            isLoggedIn = true;

            return true;
        }
        public bool SendMessage(string message)
        {
            if (!isLoggedIn)
                return false;

            try
            {
                UTF8Encoding encoder = new UTF8Encoding();
                byte[] bytes = Encoding.UTF8.GetBytes(message);
                string utf8msg = HttpUtility.UrlEncode(encoder.GetString(bytes));

                chatWc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                chatWc.Headers["X-Requested-With"] = "XMLHttpRequest";
                chatWc.Headers[HttpRequestHeader.Accept] = "*/*";
                string messageParams = String.Format("action=quick-chat-ajax-new-message&message={0}&room={1}&quick_chat_new_message_nonce={2}", utf8msg, _user, chatNewMessageNonce);
                waitChatWC();
                var result = chatWc.UploadString(sendMessageUrl, messageParams);

                if( String.IsNullOrEmpty(result )) 
                    return false;

                var newNonce = Re.GetSubString(result, reChatNewMessageNonce, 1 );
                if (string.IsNullOrEmpty(newNonce))
                    return false;
                else
                    chatNewMessageNonce = newNonce;

                Debug.Print("{0} {1}", messageParams, result);
            }
            catch
            {
                Debug.Print("Exception in Cybergame SendMessage()");
            }

            return true;
        }
        private void waitChatWC()
        {
            int tries = 0;
            while (chatWc.IsBusy)
            {
                Thread.Sleep(100);
                tries++;
                if (tries > 100)
                {
                    break;
                }
            }
        }
        public bool StartAdvertising()
        {
            if (!isLoggedIn)
                return false;

            try
            {
                UTF8Encoding encoder = new UTF8Encoding();
                chatWc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded; charset=UTF-8";

                string adsParams = String.Format("xjxfun=start_ad&xjxr={0}", TimeUtils.UnixTimestamp());
                waitChatWC();
                var result = chatWc.UploadString(cabinetUrl, adsParams);
            }
            catch
            {
                Debug.Print("Exception in Cybergame StartAdvertising()");
            }

            return true;
        }
        private void DownloadChat()
        {
            if (!isLoggedIn)
                return;

            if( String.IsNullOrEmpty(chatUpdateNonce) )
                return;


            string updateChatParams = String.Format(
                "action=quick-chat-ajax-update-messages&quick_chat_last_timestamp={0}&quick_chat_rooms%5B%5D={1}&quick_chat_update_messages_nonce={2}", lastTimestamp, _user, chatUpdateNonce);
            chatWc.Headers["X-Requested-With"] = "XMLHttpRequest";
            chatWc.Headers[HttpRequestHeader.Referer] = String.Format(channelUrl, _user);
            chatWc.Headers[HttpRequestHeader.UserAgent] = userAgent;
            chatWc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            chatWc.Headers[HttpRequestHeader.Accept] = "*/*";
            chatWc.Headers[HttpRequestHeader.AcceptLanguage] = "ru";
            chatWc.Headers[HttpRequestHeader.AcceptEncoding] = "deflate";
            chatWc.KeepAlive = true;

            waitChatWC();
            string result = string.Empty;
            try
            {
                result = chatWc.UploadString(adminAjaxUrl, updateChatParams);
            }
            catch { }

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
                    if( !String.IsNullOrEmpty(msg.message) )
                        OnMessage(this, new MessageReceivedEventArgs(msg));
                }
            }
           

        }
        private void DownloadStats( string channel )
        {
            if (String.IsNullOrEmpty(_user))
                return;
            
            try
            {
                statsWc.Headers["Cache-Control"] = "no-cache";
                var stream = statsWc.downloadURL(String.Format(channelInfoUrl, channel ));
                if (stream == null)
                {
                    Debug.Print("Can't download channel info of {0} result stream is null. Url: {1}", _user, channelInfoUrl);
                    return;
                }

                var tempChannel = JsonGenerics.ParseJson<Channel>.ReadObject(stream);

                                
                if (tempChannel == null)
                    Debug.Print("Can't parse json of {0}. Url: {1}", _user, channelInfoUrl);

                stream.Close();
                stream.Dispose();

                if (isAlive() && tempChannel == null)
                {
                    if (prevOnlineState != isAlive())
                    {
                        prevOnlineState = isAlive();
                        OnOffline(new EventArgs());
                    }
                }
                else if (!isAlive() && tempChannel != null)
                {
                    if (prevOnlineState != isAlive())
                    {
                        prevOnlineState = isAlive();
                        OnLive(new EventArgs());
                    }
                }
                currentChannel = tempChannel;


            }
            catch 
            { 
            }            
        }
        private bool isAlive()
        {
            if (currentChannel == null)
                return false;

            if (currentChannel.online == "0")
                return false;

            if (currentChannel.online == "1")
                return true;

            return false;
        }
        #endregion
        #region Public properties
        public string Viewers
        {
            get { return isAlive() ? currentChannel.viewers:"0"; }
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
