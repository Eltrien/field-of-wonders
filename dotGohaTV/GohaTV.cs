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

namespace dotGohaTV
{
    public enum GohaTVResult
    {
        On,
        Off,
        Unknown
    }

    public class GohaTV
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
        //private const string switchUrl = gohaTVDomain + "/app/tv/data.php/streamer/change/{0}/auto/ru.js";
        private const string switchUrl = gohaTVDomain + @"/app/tv/data-v2.php/stream/setStatus/{0}/{1}/gohaTV.mvc.js";
        private const string reLiveStatus = @"(\{""stop""|\[\])";
        private const string On = "on";
        private const string Off = "off";
        private const string OffNull = "null";

        #endregion
        #region Private properties
        private CookieAwareWebClient wc;
        private String uid;
        private String userid;
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
            string loginParams =
                String.Format(
                    "vb_login_username={0}&cookieuser=1&vb_login_password=&s=&securitytoken=guest&do=login&vb_login_md5password={1}&vb_login_md5password_utf={2}",
                    user, GetMd5Hash(password), GetMd5Hash(password)
                );

            wc.ContentType = ContentType.UrlEncoded;
            try
            {
                wc.UploadString(loginUrl, loginParams);
            }
            catch
            {
                Debug.Print("Error connecting to goha.ru");
                return false;
            }

            var result = wc.DownloadString(uidgetUrl);
            if( String.IsNullOrEmpty(result) )
                return false;

            uid = GetSubString(result, @".*ghfuid='(.*?)';", 1);

            if( String.IsNullOrEmpty(uid) )
                return false;

            result = wc.DownloadString(String.Format(authUrl,uid));

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
            var result = wc.DownloadString(String.Format(finalAuth, userid));
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
            if( StreamStatus == On )
                wc.postFormDataLowLevel(String.Format(switchUrl, uid, userid),
                             HttpUtility.UrlEncode(@"startstop=stop&data[start][epoch-stop]=" + ts + "&data[start][stop]=&data[start][comment]=&data[startstop][epoch-start]=&data[startstop][start]=&data[startstop][epoch-stop]=&data[startstop][stop]=" + ts + "&data[startstop][comment]=&streamProvider=default&referrer=http://www.goha.tv/#!/my/stream").Replace(@"%3d", @"=").Replace(@"%26", "&"));
            else
                wc.postFormDataLowLevel(String.Format(switchUrl, uid, userid),
                              HttpUtility.UrlEncode( @"startstop=start&data[start][epoch-stop]=&data[start][stop]=&data[start][comment]=&data[startstop][epoch-start]=&data[startstop][start]=&data[startstop][epoch-stop]=&data[startstop][stop]=&data[startstop][comment]=&streamProvider=default&referrer=http://www.goha.tv/#!/my/stream" ).Replace(@"%3d",@"=").Replace(@"%26","&"));
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



    }
}
