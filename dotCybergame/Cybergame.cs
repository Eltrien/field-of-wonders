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
        private const string channelUrl = "http://www.cybergame.tv/{0}";
        private const string adminAjaxUrl = "http://www.cybergame.tv/wp-admin/admin-ajax.php";
        private const string loginUrl = "http://www.cybergame.tv/cabinet_login.php";
        private const string reChatUpdateMessageNonce = @",""quick_chat_update_messages_nonce"":""([^""]+)"",";
        private const string reChatLastTimestamp = @",""quick_chat_last_timestamp"":(\d+),";
        #endregion     
        #region Private properties
        private string lastTimestamp;
        private string chatUpdateNonce;
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
        public bool Login()
        {
            if (String.IsNullOrEmpty(_user) || String.IsNullOrEmpty(_password))
                return false;

            chatWc = new CookieAwareWebClient();
            /*
            // I need to find a way to catch per-path cookies for Wordpress. Will implement it later
             
            chatWc.Headers["Cache-Control"] = "no-cache";
            string loginParams = String.Format("rememberme={0}&redirect_to=/{1}&log={1}&pwd={2}&submit=Войти", "forever", user, password);

            chatWc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            //var result = chatWc.UploadString(loginUrl, loginParams);
            var result = chatWc.PostUrlEncoded(loginUrl, loginParams);

            chatUpdateNonce = Re.GetSubString(result, reChatUpdateMessageNonce, 1 );
            
            if( String.IsNullOrEmpty(chatUpdateNonce ))
                return false;
            */

            var result = chatWc.DownloadString(String.Format(channelUrl, _user));
            lastTimestamp = TimeUtils.UnixTimestamp();
            chatUpdateNonce = Re.GetSubString(result, reChatUpdateMessageNonce, 1);
            lastTimestamp = Re.GetSubString(result, reChatLastTimestamp, 1);

            if (OnLogin != null)
                OnLogin(this, EventArgs.Empty);

            chatDownloader = new Timer(new TimerCallback(chatDownloader_Tick), null, 0, 3000);
            statsDownloader = new Timer(new TimerCallback(statsDownloader_Tick), null, 0, 20000);

            return true;
        }
        private void DownloadChat()
        {
            if (chatWc.IsBusy)
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

            var result = chatWc.PostUrlEncoded(adminAjaxUrl, updateChatParams);
            
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
                    if( OnMessage != null)
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
