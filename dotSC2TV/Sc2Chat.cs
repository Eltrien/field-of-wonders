using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using dotWebClient;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Net;
using System.Web;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using dotUtilities;


namespace dotSC2TV
{

    public class Smile
    {
        public string Code;
        public string Image;
        public int Width;
        public int Height;
        public Bitmap bmp;
    }
    public class Sc2Chat
    {
        #region "Private constants and properties"
        #region Constants
        private const int pollInterval = 5000;
        private const string channelsUrl = "http://chat.sc2tv.ru/memfs/channels.json?_={0}";
        private const string channelEditUrl = "http://sc2tv.ru/node/add/userstream";
        private const string channelEditUrl2 = "http://sc2tv.ru/node/{0}/edit";
        private const string loginUrl = "http://sc2tv.ru/node";
        private const string messagesUrl = "http://chat.sc2tv.ru/memfs/channel-{0}.json?_={1}";
        private const string smilesJSUrl = "http://chat.sc2tv.ru/js/smiles.js";
        private const string smilesImagesUrl = "http://chat.sc2tv.ru/img/{0}";
        private const string sendMessageUrl = "http://chat.sc2tv.ru/gate.php";
        private const string chatTokenUrl = "http://chat.sc2tv.ru/gate.php?task=GetUserInfo&ref=http://sc2tv.ru/";
        private const string urlUserStream = "http://sc2tv.ru/node/add/userstream";


        private const string reHiddenFormId = @".*hidden.*form_build_id.*id=""(.*?)"".*$";
        private const string reChannelIsLive = @"<input type=""text""[^>]*?id=""edit-field-channel-status.*?""[^>]*?value=""(.*?)""";
        private const string reChannelTitle = @"<input type=""text"".*?id=""edit-title""[^>]*?value=""(.*?)""";
        private const string reChannelType = @"<select .*?id=""edit-field-channel-type-value"".*?<option value=""([^>]*?)"" selected=""selected"".*?</select>";
        private const string reChannelName = @"<input type=""text"".*?id=""edit-field-channel-name-0-value""[^>]*?value=""(.*?)""";
        private const string reChannelAutoUpdate = @"<input type=""checkbox"".*?id=""edit-field-channel-autoupdate-value""[^>]*?value=""(.*?)""[^>]*?checked=""checked"".*?/>";
        private const string reChannelWithoutComments = @"<input type=""checkbox"".*?id=""edit-field-channel-without-comments-value""[^>]*?value=""(.*?)""[^>]*?checked=""checked""";
        private const string reChannelGame = @"<select .*?id=""edit-taxonomy-1"".*<option value=""([^>]*?)"" selected=""selected"".*?</select>";
        private const string reChannelLongInfo = @"<textarea .*?id=""edit-body"".*?>(.*?)</textarea>";
        private const string reChannelShortInfo = @"<textarea .*?id=""edit-teaser"".*?>(.*?)</textarea>";
        private const string reChannelURLAlias = @"<input type=""checkbox"".*?id=""edit-pathauto-perform-alias"".*?value=""(.*?)""[^>]*?checked=""checked""";
        private const string reChannelURLPath = @"<input type=""text"".*?id=""edit-path""[^>]*?value=""(.*?)""";
        private const string reChannelChanged = @"<input type=""hidden"".*?id=""edit-changed""[^>]*?value=""(.*?)""";
        private const string reChannelFormBuildId = @"<input type=""hidden"".*?id=""form-................................""[^>]*?value=""(.*?)""";
        private const string reChannelFormToken = @"<input type=""hidden"".*?id=""edit-userstream-node-form-form-token""[^>]*?value=""(.*?)""";
        private const string reChannelFormId = @"<input type=""hidden"".*?id=""edit-userstream-node-form""[^>]*?value=""(.*?)""";
        private const string reStreamId = @"http://sc2tv.ru/node/(\d+)?/edit";
        #endregion

        private const string userAgent = "Mozilla/5.0 (Windows NT 6.0; WOW64; rv:14.0) Gecko/20100101 Firefox/14.0.1";
        private const string cookieForTest = "drupal_uid";
        private const string mainDomain = "http://sc2tv.ru";
        private const string chatDomain = "http://chat.sc2tv.ru";
        private string _lastStatus = null;
        private CookieAwareWebClient loginWC;
        private CookieAwareWebClient chatWC;
        private CookieAwareWebClient settingsWC;
        private UInt32 currentChannelId = 0;
        private Timer chatDownloader;
        private object chatLock = new object();
        private object messageLock = new object();
        private object streamListLock = new object();
        private object loginLock = new object();
        private object settingsLock = new object();
        private bool _channelIsLive;

        #region Classes
        private class LambdaComparer<T> : IEqualityComparer<T>
        {
            private readonly Func<T, T, bool> _lambdaComparer;
            private readonly Func<T, int> _lambdaHash;

            public LambdaComparer(Func<T, T, bool> lambdaComparer) :
                this(lambdaComparer, o => 0)
            {
            }

            public LambdaComparer(Func<T, T, bool> lambdaComparer, Func<T, int> lambdaHash)
            {
                if (lambdaComparer == null)
                    throw new ArgumentNullException("lambdaComparer");
                if (lambdaHash == null)
                    throw new ArgumentNullException("lambdaHash");

                _lambdaComparer = lambdaComparer;
                _lambdaHash = lambdaHash;
            }

            public bool Equals(T x, T y)
            {
                return _lambdaComparer(x, y);
            }

            public int GetHashCode(T obj)
            {
                return _lambdaHash(obj);
            }
        }
        #endregion
        #endregion
        #region "Events"
        public event EventHandler<Sc2Event> Logon;
        public event EventHandler<Sc2Event> ChannelList;
        public event EventHandler<Sc2MessageEvent> MessageReceived;
        public class Sc2Event : EventArgs
        {
            public Sc2Event()
            {
            }
        }
        public class Sc2MessageEvent : EventArgs
        {
            public ChatMessage message;
            public Sc2MessageEvent(ChatMessage m)
            {
                message = m;
            }
        }

        private void DefaultEvent(EventHandler<Sc2Event> sc2Event, Sc2Event e)
        {
            EventHandler<Sc2Event> handler = sc2Event;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        private void MessageEvent(EventHandler<Sc2MessageEvent> sc2Event, Sc2MessageEvent e)
        {
            EventHandler<Sc2MessageEvent> handler = sc2Event;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        protected virtual void OnLogon(Sc2Event e)
        {
            DefaultEvent(Logon, e);
        }
        protected virtual void OnChannelList(Sc2Event e)
        {
            DefaultEvent(ChannelList, e);
        }
        protected virtual void OnMessageReceived(Sc2MessageEvent e)
        {
            MessageEvent(MessageReceived, e);
        }
        #endregion
        #region "Public properties"
        public Channels channelList;
        public ChatMessages chat;
        public List<Smile> smiles = new List<Smile>();
        public bool LoggedIn = false;
        #endregion

        #region "Public methods"

        public Sc2Chat( uint lastMsgId )
        {
            LastMessageId = lastMsgId;
            settingsWC = new CookieAwareWebClient();
            loginWC = new CookieAwareWebClient();
            chatWC = new CookieAwareWebClient();
            loginWC.Headers["User-Agent"] = userAgent;
            loginWC.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded; charset=UTF-8";
            chat = new ChatMessages();
            chatDownloader = new Timer(new TimerCallback(chatDownloader_Tick), null, Timeout.Infinite, Timeout.Infinite);
        }
        private void chatDownloader_Tick(object o)
        {
            DownloadChat(false);
        }
        public void Start()
        {
            chatDownloader.Change(0, pollInterval);
        }
        public void Stop()
        {
            chatDownloader.Change(Timeout.Infinite, Timeout.Infinite);
        }
        public string sanitizeMessage(string message, bool cutSmiles = false)
        {
            var sanitizePatterns = new string[] { 
                @"<(.|\n)+?>",
                @"&quot;"
            };

            var sanitizeSmiles = new string[] {
                @":s:.*?:"
            };

            foreach( string p in sanitizePatterns)
                message = Regex.Replace(message, p, "", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            if( cutSmiles )
            {
                foreach( string p in sanitizeSmiles )
                    message = Regex.Replace(message, p, "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            }
            return message.Trim();
        }
        private void ResetLastError()
        {
            _lastStatus = null;
        }

        /// <summary>
        /// Download and parse chat messages from sc2tv.ru by given chat id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UInt32 ChannelId
        {
            get { return currentChannelId; }
            set {
                if (currentChannelId != value)
                {
                    currentChannelId = value;
                    DownloadChat(true);
                }
            }
        }
        public UInt32 LastMessageId
        {
            get;
            set;
        }
        public bool DownloadChat(bool reload)
        {
            lock( chatLock )
            {
                chatWC.Cookies = loginWC.Cookies;

                if (reload)
                {
                    _lastStatus = null;
                    chat.messages = null;
                }

                var url = String.Format(messagesUrl, ChannelId, TimeUtils.UnixTimestamp());
                System.IO.Stream stream = chatWC.downloadURL(url);

                _lastStatus = chatWC.LastWebError;
                if (_lastStatus == "ProtocolError")
                {
                    //Chat json isn't available at the moment
                    return false;
                }

                if (stream == null)
                {
                    Debug.Print(String.Format("Sc2tv: stream is null"));
                    return false;
                }

                var newchat = ParseJson<ChatMessages>.ReadObject(stream);

                if (newchat == null )
                {
                    return false;
                }
                else if (newchat.messages.Count <= 0)
                {
                    return false;
                }


                if (chat.messages == null)
                {
                    chat.messages = new List<ChatMessage>();
                    chat.messages = newchat.messages.Where(msg => msg.id < LastMessageId).ToList();
                }

                // Find new messages
                var newmessages = newchat.messages.Except(
                    chat.messages, new LambdaComparer<ChatMessage>( (x,y) => x.id == y.id ) );

                chat = newchat;

                if( newmessages.Count() > 0 )
                    LastMessageId = newchat.MaxID();

                // Put "to" nickname into separate property
                foreach (var m in newmessages)
                {
                    var re = @"<b>(.*)?</b>,";
                    var matchesToUser = Regex.Matches(m.message, re, RegexOptions.IgnoreCase | RegexOptions.Multiline);

                    if (matchesToUser.Count > 0)
                    {
                        if (matchesToUser[0].Groups.Count > 0)
                        {
                            m.to = matchesToUser[0].Groups[1].Value;
                            m.message = Regex.Replace(m.message, re, "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        }
                    }
                    OnMessageReceived(new Sc2MessageEvent(m));
                }
                return true;
            }
        }
        public bool updateSmiles()
        {
            
            using (CookieAwareWebClient cwc = new CookieAwareWebClient())
            {
                System.IO.Stream stream = cwc.downloadURL(smilesJSUrl);
                using ( System.IO.StreamReader reader = new System.IO.StreamReader(stream) )
                {
                    if (stream == null)
                        return false;

                    List<object> list = JSEvaluator.EvalArrayObject(reader.ReadToEnd());
                smiles.Clear();
                foreach (object obj in list)
                {
                    Smile smile = new Smile();
                    smile.Code = JSEvaluator.ReadPropertyValue(obj, "code");
                    smile.Image = JSEvaluator.ReadPropertyValue(obj, "img");
                    smile.Width = int.Parse(JSEvaluator.ReadPropertyValue(obj, "width"));
                    smile.Height = int.Parse(JSEvaluator.ReadPropertyValue(obj, "height"));
                    try
                    {
                        Bitmap srcimage = new Bitmap(cwc.downloadURL(String.Format(smilesImagesUrl, smile.Image)));
                        srcimage = resizeImage(srcimage,new Size(smile.Width,smile.Height));
                        smile.bmp = new Bitmap(smile.Width, smile.Height);
                        using (Graphics g = Graphics.FromImage(smile.bmp))
                        {
                            g.DrawImage(srcimage,1,1);
                        }
                    }
                    catch
                    {
                        smile.bmp = new Bitmap(30,30);
                        using( Graphics g = Graphics.FromImage(smile.bmp) )
                        {
                            g.Clear(Color.White);
                            g.DrawRectangle(new Pen(Color.Black), new Rectangle(0,0,28,28));
                            g.DrawString(smile.Code, new Font("Microsoft Sans Serif", 7), Brushes.Black, new RectangleF(0, 0, 28, 28));
                        }
                        Debug.Print("Exception in updateSmiles()");
                    }
                    smiles.Add(smile);
                }
            }
            
            return true;
            }
        }
        public bool updateStreamList( )
        {
            lock (streamListLock)
            {
                using (CookieAwareWebClient cwc = new CookieAwareWebClient())
                {
                    System.IO.Stream stream = cwc.downloadURL(String.Format(channelsUrl, (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds));
                    if (stream == null)
                        return false;

                    channelList = ParseJson<Channels>.ReadObject(stream);
                    if (channelList == null)
                        return false;

                    OnChannelList(new Sc2Event());
                }

                return true;
            }
        }
        public String GetStreamID()
        {
            if (!LoggedIn)
                return String.Empty;

            var content = loginWC.DownloadString(urlUserStream);

            String streamId = GetSubString(content, reStreamId, 1);

            if (String.IsNullOrEmpty(streamId))
                return String.Empty;

            return streamId;
        }
        public void Login(string login, string password )
        {
            lock (loginLock)
            {
                LoggedIn = false;

                if (loginWC.gotCookies(cookieForTest, mainDomain))
                {
                    LoggedIn = true;
                    return;
                }
                string formBuildId = getLoginFormId();

                if (String.IsNullOrEmpty(formBuildId))
                {
                    Debug.Print("Can't find Form Build ID. Check RE");
                    return;
                }
                else if (formBuildId != null)
                {
                    try
                    {
                        string loginParams = "name=" + login + "&pass=" + password + "&form_build_id=" + formBuildId + "&form_id=user_login_block";

                        loginWC.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded; charset=UTF-8";
                        loginWC.UploadString(loginUrl, loginParams);

                        if (loginWC.gotCookies(cookieForTest, mainDomain))
                        {
                            loginWC.setCookie("chat-img", "1", "chat.sc2tv.ru");
                            loginWC.setCookie("chat_channel_id", ChannelId.ToString(), "chat.sc2tv.ru");
                            loginWC.setCookie("chat-on", "1", "chat.sc2tv.ru");
                            loginWC.DownloadString(chatTokenUrl);

                            loginWC.setCookie("chat_token", loginWC.CookieValue("chat_token", chatDomain + "/gate.php"), "chat.sc2tv.ru");
                            LoggedIn = true;
                            settingsWC.Cookies = loginWC.Cookies;
                            OnLogon(new Sc2Event());
                        }

                    }
                    catch
                    {
                        Debug.Print("Exception in Sc2 Login()");
                    }
                }

            }
        }    
        public String ChannelTitle
        {
            get;set;
        }
        public String ChannelType
        {
            get;set;
        }
        public String ChannelName
        {
            get;set;
        }
        public bool ChannelAutoUpdate
        {
            get;
            set;
        }
        public bool ChannelWithoutComments
        {
            get;
            set;
        }
        public bool ChannelIsLive
        {
            get {
                LoadStreamSettings();
                return _channelIsLive;
            }
            set
            {
                _channelIsLive = value;
            }
        }
        public String ChannelGame
        {
            get;
            set;
        }
        public String ChannelLongInfo
        {
            get;
            set;
        }
        public String ChannelShortInfo
        {
            get;
            set;
        }
        public bool ChannelURLAlias
        {
            get;
            set;
        }
        public String ChannelURLPath
        {
            get;
            set;
        }

        public String ChannelChanged
        {
            get;
            set;
        }
        public String ChannelFormBuildId
        {
            get;
            set;
        }
        public String ChannelFormToken
        {
            get;
            set;
        }
        public String ChannelFormId
        {
            get;
            set;
        }
        public void LoadStreamSettings()
        {
            if (!LoggedIn)
                return;
            lock (settingsLock)
            {
                String html = null;
                var url = String.Format("{0}?_={1}", channelEditUrl, (new DateTime(1970, 1, 1)).Ticks);
                try
                {
                    html = settingsWC.DownloadString(url);
                }
                catch (WebException e)
                {
                    Debug.Print(String.Format("Exception in LoadStreamSettings() {0} {1}", e.Message, url));
                    return;
                }
                MatchCollection reChannelStatusValue = Regex.Matches(html, reChannelIsLive, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                ChannelTitle = GetSubString(html, reChannelTitle, 1);
                ChannelType = GetSubString(html, reChannelType, 1);
                ChannelName = GetSubString(html, reChannelName, 1);
                ChannelAutoUpdate = GetSubString(html, reChannelAutoUpdate, 1) == "1";
                ChannelWithoutComments = GetSubString(html, reChannelWithoutComments, 1) == "1";
                ChannelIsLive = GetSubString(html, reChannelIsLive, 1) != "0";
                ChannelGame = GetSubString(html, reChannelGame, 1);
                ChannelLongInfo = GetSubString(html, reChannelLongInfo, 1);
                ChannelShortInfo = GetSubString(html, reChannelShortInfo, 1);
                ChannelURLAlias = GetSubString(html, reChannelURLAlias, 1) == "1";
                ChannelURLPath = GetSubString(html, reChannelURLPath, 1);
                ChannelChanged = GetSubString(html, reChannelChanged, 1);
                ChannelFormBuildId = GetSubString(html, reChannelFormBuildId, 1);
                ChannelFormToken = GetSubString(html, reChannelFormToken, 1);
                ChannelFormId = GetSubString(html, reChannelFormId, 1);
                Debug.Print("Sc2tv: settings loaded");
            }
        }
        public void SaveStreamSettings()
        {
            if (ChannelId == 0)
                return;
            var postData = new PostData();
            postData.Params.Add(new PostDataParam( "title",ChannelTitle,PostDataParamType.Field));
            postData.Params.Add(new PostDataParam( "field_channel_type[value]",ChannelType,PostDataParamType.Field));
            postData.Params.Add(new PostDataParam( "field_channel_name[0][value]",ChannelName,PostDataParamType.Field));            
            postData.Params.Add(new PostDataParam( "field_channel_status[0][value]",_channelIsLive?"1":"0",PostDataParamType.Field));            
            postData.Params.Add(new PostDataParam( "field_channel_id[0]","",PostDataParamType.Field));
            postData.Params.Add(new PostDataParam( "taxonomy[1][]",ChannelGame,PostDataParamType.Field));
            postData.Params.Add(new PostDataParam( "changed",ChannelChanged,PostDataParamType.Field));
            postData.Params.Add(new PostDataParam( "form_build_id",ChannelFormBuildId,PostDataParamType.Field));
            postData.Params.Add(new PostDataParam( "form_token",ChannelFormToken,PostDataParamType.Field));
            postData.Params.Add(new PostDataParam( "form_id",ChannelFormId,PostDataParamType.Field));
            postData.Params.Add(new PostDataParam( "body",HttpUtility.HtmlDecode( ChannelLongInfo ),PostDataParamType.Field ));
            postData.Params.Add(new PostDataParam( "teaser",HttpUtility.HtmlDecode( ChannelShortInfo ),PostDataParamType.Field));
            postData.Params.Add(new PostDataParam( "path",ChannelURLPath,PostDataParamType.Field));
            postData.Params.Add(new PostDataParam("op", "Сохранить", PostDataParamType.Field));
            if (ChannelAutoUpdate)
                postData.Params.Add(new PostDataParam("field_channel_autoupdate[value]", "1", PostDataParamType.Field));
            if (ChannelWithoutComments)
                postData.Params.Add(new PostDataParam("field_channel_without_comments[value]", "1", PostDataParamType.Field));
            if (ChannelURLAlias)
                postData.Params.Add(new PostDataParam( "pathauto_perform_alias","1",PostDataParamType.Field));

            Debug.Print(loginWC.PostMultipart(String.Format(channelEditUrl2, ChannelId), postData.GetPostData(), postData.Boundary));
            Debug.Print("Sc2tv: Settings saved");

        }
        public void setLiveStatus(bool status)
        {
            ChannelIsLive = status;
            SaveStreamSettings();
            var i = 0;
            while (ChannelIsLive != status)
            {
                Thread.Sleep(1000);
                i++;
                if (i > 3) break;
            }
        }
        public bool SendMessage(string message)
        {
            lock (messageLock)
            {
                try
                {
                    UTF8Encoding encoder = new UTF8Encoding();
                    byte[] bytes = Encoding.UTF8.GetBytes(message);
                    string utf8msg = HttpUtility.UrlEncode(encoder.GetString(bytes));

                    loginWC.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded; charset=UTF-8";

                    string messageParams = "task=WriteMessage&message=" + utf8msg + "&channel_id=" + ChannelId + "&token=" + loginWC.CookieValue("chat_token", chatDomain);
                    loginWC.UploadString(String.Format(sendMessageUrl, ChannelId), messageParams);
                    return true;
                }
                catch
                {
                    Debug.Print("Exception in Sc2 sendMessage()");
                }

                return false;
            }
        }
        #endregion

        #region "Private methods"
        private string getLoginFormId( string html = "" )
        {
            if (html == "")
            {
                try
                {
                    html = loginWC.DownloadString(loginUrl);
                }
                catch
                {
                    Debug.Print("Exception in Sc2 getLoginFormId()");
                    return null;
                }
            }

            MatchCollection reFormBuildId = Regex.Matches(html, reHiddenFormId, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            
            if (reFormBuildId.Count <= 0)
                return "";

            else if (reFormBuildId[0].Groups.Count <= 0)
                return "";

            return reFormBuildId[0].Groups[1].Value;       
        }
        private static Bitmap resizeImage(Image imgToResize, Size size)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)size.Width / (float)sourceWidth);
            nPercentH = ((float)size.Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap b = new Bitmap(destWidth, destHeight);
            using (Graphics g = Graphics.FromImage((Image)b))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            }

            return b;
        }
        private string GetSubString(string input, string re, int index)
        {            
            var match = Regex.Match(input, re, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (!match.Success)
                return null;

            if (match.Groups.Count <= index)
                return null;

            var result = match.Groups[index].Value;

            if (String.IsNullOrEmpty(result))
            {
                Debug.Print( String.Format("RE: {0}. Result = NULL", re ));
                return null;
            }
                       
            return result;

        }
        #endregion

    }


}
