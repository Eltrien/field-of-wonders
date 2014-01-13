using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Threading;
using dotWebClient;
using System.Security.Cryptography;
using System.Diagnostics;
using dotUtilities;
using System.Web;
using System.Net;

using dotInterfaces;
using dotHtmlParser;
using dot.Json.Linq;

namespace dotGohaTV
{
    public enum GohaTVResult
    {
        On,
        Off,
        Unknown
    }

    public class GohaTV : IChatDescription
    {
        #region Constants
        private const string userAgent = "Mozilla/5.0 (Windows NT 6.0; WOW64; rv:14.0) Gecko/20100101 Firefox/14.0.1";
        private const string gohaForumDomain = "http://forums.goha.ru";
        private const string gohaTVDomain = "http://goha.tv";        
        private const string loginUrl = gohaForumDomain + "/10gin.php?do=login";
        private const string uidgetUrl = gohaForumDomain + "/flcheck.php";
        private const string authUrl = gohaTVDomain + "/auth/v3/auth.php/{0}";
        //private const string finalAuth = gohaTVDomain + "/app/tv/data.php/streamer/{0}/ru.js";
        private const string finalAuth = gohaTVDomain + @"/app/tv/data-v2.php/stream/getStatus/{0}/gohaTV.mvc.js";
        private const string urlInfo = gohaTVDomain + @"/app/tv/data-v2.php/stream/getInfo/{0}/gohaTV.mvc.js";
        //private const string switchUrl = gohaTVDomain + "/app/tv/data.php/streamer/change/{0}/auto/ru.js";
        private const string switchUrl = gohaTVDomain + @"/app/tv/data-v2.php/stream/setStatus/{0}/{1}/gohaTV.mvc.js";
        private const string urlProfileEdit = gohaTVDomain + @"/#!/my/profile/edit";
        private const string urlProfileSave = gohaForumDomain + @"/gohatv_edit-v2.php?postid={0}";
        private const string reLiveStatus = @"(\{""stop""|\[\])";
        private const string On = "on";
        private const string Off = "off";
        private const string OffNull = "null";
        private const string reUserInfo = @"^.*?(\{""userinfo"".*).*?\)[^)]*$";

        #endregion
        #region Private properties
        private CookieAwareWebClient wc;
        private String uid;
        private String userid;
        private String _user;

        private object wcLock = new object();
        #endregion
        #region Events
        public event EventHandler<EventArgs> OnLive;
        public event EventHandler<EventArgs> OnOffline;
        public event EventHandler<EventArgs> OnLogin;

        #endregion
        #region Private methods
        private string GetMd5Hash(string input)
        {
            var md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
        private string GetSubString(string input, string re, int index)
        {
            var match = Regex.Match(input, re);
            if (!match.Success)
                return null;

            if (match.Groups.Count <= index )
                return null;

            var result = match.Groups[index].Value;

            if (String.IsNullOrEmpty(result))
                return null;

            return result;

        }
        #endregion
        #region Public methods
        public GohaTV()
        {
            wc = new CookieAwareWebClient();
            wc.Headers["User-Agent"] = userAgent;
            LoggedIn = false;
        }
        public bool Login( string user, string password )
        {
            _user = user.ToLower();
            string loginParams =
                String.Format(
                    "vb_login_username={0}&cookieuser=1&vb_login_password=&s=&securitytoken=guest&do=login&vb_login_md5password={1}&vb_login_md5password_utf={2}",
                    user, GetMd5Hash(password), GetMd5Hash(password)
                );

            wc.ContentType = ContentType.UrlEncoded;
            var result = String.Empty;
            lock (wcLock)
            {
                try
                {
                    wc.UploadString(loginUrl, loginParams);
                }
                catch
                {
                    Debug.Print("Error connecting to goha.ru");
                    return false;
                }

                result = wc.DownloadString(uidgetUrl);
                if (String.IsNullOrEmpty(result))
                    return false;

                uid = GetSubString(result, @".*ghfuid='(.*?)';", 1);

                if (String.IsNullOrEmpty(uid))
                    return false;

                result = wc.DownloadString(String.Format(authUrl, uid));
            }
            userid = GetSubString(result, @".*""userid"":""(\d+?)""", 1);
            
            wc.setCookie("keeponline", "1", "www.goha.tv");
            wc.setCookie("ghfuid", uid, "www.goha.tv");




            //StreamStatus = GetSubString(result, @"""status"":""(.*?)""", 1).ToLower();

            LoggedIn = true;
            if( OnLogin != null )
                OnLogin(this, EventArgs.Empty);

            GetStreamStatus();

            switch (StreamStatus.ToLower())
            {
                case On:
                    if (OnLive != null)
                        OnLive(this, EventArgs.Empty);
                    break;
                case Off:
                    if( OnOffline != null )
                        OnOffline (this, EventArgs.Empty );
                    break;
            }

            return true;
        }
        private void GetStreamStatus()
        {
            var result = string.Empty;
            lock (wcLock)
            {
                result = wc.DownloadString(String.Format(finalAuth, userid));
            }
            if (String.IsNullOrEmpty(result))
                return;

            StreamStatus = GetSubString(result, reLiveStatus, 1).ToLower() == "[]" ? "off" : "on";

            if (String.IsNullOrEmpty(StreamStatus))
                return;

        }
        public GohaTVResult SwitchStream()
        {
            if (!LoggedIn)
                return GohaTVResult.Unknown;
            while (wc.IsBusy) ;
            var ts = TimeUtils.UnixTimestamp();
            
            wc.ContentType = ContentType.UrlEncoded;
            
            GetStreamStatus();
            lock (wcLock)
            {
                if (StreamStatus == On)
                    wc.postFormDataLowLevel(String.Format(switchUrl, uid, userid),
                                 HttpUtility.UrlEncode(@"startstop=stop&data[start][epoch-stop]=" + ts + "&data[start][stop]=&data[start][comment]=&data[startstop][epoch-start]=&data[startstop][start]=&data[startstop][epoch-stop]=&data[startstop][stop]=" + ts + "&data[startstop][comment]=&streamProvider=default&referrer=http://www.goha.tv/#!/my/stream").Replace(@"%3d", @"=").Replace(@"%26", "&"));
                else
                    wc.postFormDataLowLevel(String.Format(switchUrl, uid, userid),
                                  HttpUtility.UrlEncode(@"startstop=start&data[start][epoch-stop]=&data[start][stop]=&data[start][comment]=&data[startstop][epoch-start]=&data[startstop][start]=&data[startstop][epoch-stop]=&data[startstop][stop]=&data[startstop][comment]=&streamProvider=default&referrer=http://www.goha.tv/#!/my/stream").Replace(@"%3d", @"=").Replace(@"%26", "&"));
            }
            GetStreamStatus();

            if (StreamStatus == On)
            {
                if (OnLive != null)
                    OnLive(this, EventArgs.Empty);
                return GohaTVResult.On;
            }
            else
            {
                StreamStatus = Off;
                if (OnOffline != null)
                    OnOffline(this, EventArgs.Empty);
                return GohaTVResult.Off;
            }

            //return GohaTVResult.Unknown;
        }
        #endregion
        #region Public properties
        public bool LoggedIn
        {
            get;
            set;
        }
        public String StreamStatus
        {
            get;
            set;
        }
        #endregion




        public string Game
        {
            get;
            set;
        }

        public string ShortDescription
        {
            get;
            set;
        }

        public string LongDescription
        {
            get;
            set;
        }

        public string GameId
        {
            get;
            set;
        }
        private String PostId
        {
            get;
            set;
        }
        public void GetDescription()
        {
            String content = String.Empty;
            String userInfo = String.Empty;
            content = HttpGet(String.Format(urlInfo,userid));


            userInfo = Re.GetSubString(content, reUserInfo, 1);
            if (String.IsNullOrEmpty(userInfo))
                return;

            try
            {
                JToken objInfo = JToken.Parse(userInfo);
                if (objInfo == null)
                    return;
                    
                objInfo = objInfo["userinfo"];
                PostId = objInfo["postid"].ToString();
                ShortDescription = objInfo["page_content"].ToString();
                LongDescription = objInfo["pc_desc"].ToString();
                Game = objInfo["game"].ToString();

                ChannelFormParams = new List<KeyValuePair<string, string>>();

                ChannelFormParams.Add(new KeyValuePair<string, string>("data[name]", objInfo["name"].ToString()));
                ChannelFormParams.Add(new KeyValuePair<string, string>("data[avatar]", objInfo["avatar"].ToString()));
                ChannelFormParams.Add(new KeyValuePair<string, string>("data[country]", objInfo["country"].ToString()));
                ChannelFormParams.Add(new KeyValuePair<string, string>("data[sex]", objInfo["sex"].ToString()));
                ChannelFormParams.Add(new KeyValuePair<string, string>("data[stream_provider]", objInfo["stream_provider"].ToString()));
                ChannelFormParams.Add(new KeyValuePair<string, string>("data[stream_id]", objInfo["stream_id"].ToString()));
                ChannelFormParams.Add(new KeyValuePair<string, string>("data[chat_twitch]", objInfo["chat_twitch"].ToString()));
                ChannelFormParams.Add(new KeyValuePair<string, string>("data[chat_goodgame]", objInfo["chat_goodgame"].ToString()));
                ChannelFormParams.Add(new KeyValuePair<string, string>("data[chat_sc2]", objInfo["chat_sc2"].ToString()));
                ChannelFormParams.Add(new KeyValuePair<string, string>("data[chat_empire]", objInfo["chat_empire"].ToString()));
                ChannelFormParams.Add(new KeyValuePair<string, string>("referrer", @"http://www.goha.tv/#!/my/profile"));
                ChannelFormParams.Add(new KeyValuePair<string, string>("format", @"format:[hide][forumhide]stream:|{stream_provider}|{stream_id}|{game}|{pc_desc}|[/forumhide][B]Игра:[/B][br]{game}[br][br][B]Компьютер:[/B][br]{pc_desc}[br][br][B]Описание:[/B][/hide][br]{page_content}"));
                ChannelFormParams.Add(new KeyValuePair<string, string>("title", objInfo["name"].ToString()));
                ChannelFormParams.Add(new KeyValuePair<string, string>("data[username]", objInfo["username"].ToString()));
                ChannelFormParams.Add(new KeyValuePair<string, string>("save", "1"));

            }
            catch( Exception e )
            {
                Debug.Print("Goha GetDescription error: {0}", e.Message);
            }
            
        }
        public List<KeyValuePair<String, String>> ChannelFormParams
        {
            get;
            set;
        }

        public void SetDescription()
        {
            if (!LoggedIn || String.IsNullOrEmpty(PostId) || ChannelFormParams == null)
                return;
            
            
            String param = ChannelFormParams.Select(kv => String.Format("{0}={1}", HttpUtility.UrlEncode(kv.Key), HttpUtility.UrlEncode(kv.Value))).Aggregate((i, j) => i + '&' + j);
            param += String.Format("&{0}={1}", HttpUtility.UrlEncode("data[pc_desc]"), HttpUtility.UrlEncode(LongDescription));
            param += String.Format("&{0}={1}", HttpUtility.UrlEncode("data[page_content]"), HttpUtility.UrlEncode(ShortDescription));
            param += String.Format("&{0}={1}", HttpUtility.UrlEncode("data[game]"),  HttpUtility.UrlEncode(Game));

            HttpPost(String.Format(urlProfileSave, PostId), param);
            
        }


        public List<KeyValuePair<string, string>> GameList
        {
            get;
            set;
        }

        private String HttpPost(String url, String param)
        {
            lock (wcLock)
            {
                wc.ContentType = ContentType.UrlEncodedUTF8;
                //wc.Headers["X-Requested-With"] = "XMLHttpRequest";
                try
                {
                    var result = wc.UploadString(url, param);
                    if (!String.IsNullOrEmpty(result))
                        return result;
                    else
                        Debug.Print("Goha: httppost - empty string fetched from {0}", url);
                }
                catch (Exception e)
                {
                    Debug.Print("Goha error posting to {0}: {1}", url, e.Message);
                }
            }
            return String.Empty;
        }
        private String HttpGet(String url)
        {
            lock (wcLock)
            {
                wc.ContentType = ContentType.UrlEncodedUTF8;
                //loginWC.Headers["X-Requested-With"] = "XMLHttpRequest";
                try
                {
                    var result = wc.DownloadString(url);
                    if (!String.IsNullOrEmpty(result))
                        return result;
                    else
                        Debug.Print("Goha: httpget - empty string fetched from {0}", url);
                }
                catch (Exception e)
                {
                    Debug.Print("Goha error fetching {0}: {1}", url, e.Message);
                }
            }
            return String.Empty;
        }
    }
}
