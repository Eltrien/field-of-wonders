using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dotWebClient;
using dotUtilities;
using dot.Json;
using dot.Json.Linq;
using System.Diagnostics;
using dotInterfaces;
using System.Web;
using System.Net;

namespace dotTwitchTV
{

    public class TwitchWeb : IChatDescription
    {
        private const string loginPopupUrl = "http://ru.twitch.tv/user/login_popup";
        private const string urlEditPage = @"http://www.twitch.tv/{0}/edit";
        private const string domain = "twitch.tv";
        private const string editParams = @"title={0}&meta_game={1}";
        private const string loginUrl = "https://secure.twitch.tv/user/login";
        private const string oauthUrl = "http://api.twitch.tv/api/me?on_site=1";
        private const string reAuthToken = @"^.*authenticity_token.*?value=""(.*?)""";
        private const string loginParams = "utf8=%E2%9C%93&authenticity_token={0}%3D&redirect_on_login=&embed_form=false&user%5Blogin%5D={1}&user%5Bpassword%5D={2}";
        private const string reDescription = @"<textarea.*?id='status'.*?>(.*?)</textarea>";
        private const string reGame = @"<input.*?id='gameselector_input'.*?name='gameselector'[^>]+value='(.*?)'";
        private object loginLock = new object();
        private string api_token, csrf_token;

        private CookieAwareWebClient loginWC;

        public event EventHandler<EventArgs> OnDescriptionSetError;
        public event EventHandler<EventArgs> OnLogin;
        public event EventHandler<EventArgs> OnLoginFailed;
        public TwitchWeb(String user, String password)
        {
            loginWC = new CookieAwareWebClient();
            User = user;
            Password = password;
        }

        private String User
        {
            get;
            set;
        }
        private String Password
        {
            get;
            set;
        }
        public String ChatOAuthKey
        {
            get;
            set;
        }
        public bool Login()
        {
            lock (loginLock)
            {
                loginWC.Headers["X-Requested-With"] = "XMLHttpRequest";
                try
                {

                    String result = loginWC.DownloadString(loginPopupUrl);
                    if (String.IsNullOrEmpty(result))
                        return false;

                    GetCSRFToken(result);
                    if (String.IsNullOrEmpty(csrf_token))
                        return false;

                    loginWC.ContentType = ContentType.UrlEncoded;
                    loginWC.Headers["Accept"] = "text/html, application/xhtml+xml, */*";
                    result = loginWC.UploadString(loginUrl, String.Format(loginParams, csrf_token, User, Password));
                    if (String.IsNullOrEmpty(result))
                        return false;
                    api_token = loginWC.CookieValue("api_token", "http://" + domain);

                    loginWC.Headers["Twitch-Api-Token"] = api_token;
                    loginWC.Headers["Accept"] = "application/vnd.twitchtv.v2+json";

                    result = loginWC.DownloadString(oauthUrl);
                    if (String.IsNullOrEmpty(result))
                        return false;

                    JObject chatOauthJson = JObject.Parse(result);
                    if (chatOauthJson == null)
                        return false;

                    String chatOauthKey = chatOauthJson["chat_oauth_token"].ToString();
                    if (String.IsNullOrEmpty(chatOauthKey))
                        return false;

                    
                    ChatOAuthKey = "oauth:" + chatOauthKey;
                }
                catch (Exception e)
                {
                    Debug.Print("Twitch login exception: {0}", e.Message);
                }

                if (OnLogin != null)
                    OnLogin(this, EventArgs.Empty);
                
                return true;

            }
        }
        private String GetCSRFToken(String content)
        {
            String csrf = Re.GetSubString(content, reAuthToken, 1);
            if (!String.IsNullOrEmpty(csrf))
            {
                csrf_token = csrf;
                loginWC.setCookie("csrf_token", csrf_token, "twitch.tv" + domain);
            }
            return csrf;
        }
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

        public void GetDescription()
        {
            var content = HtmlGet(String.Format(urlEditPage, User.ToLower()),String.Empty);
            csrf_token = GetCSRFToken(content);

            ShortDescription = Re.GetSubString(content, reDescription, 1);
            LongDescription = ShortDescription;
            Game = Re.GetSubString(content, reGame, 1);


        }
        public void SetDescription()
        {
            var content = HtmlPost( String.Format( urlEditPage, User.ToLower()), 
                String.Format(editParams, HttpUtility.UrlEncode( ShortDescription ), HttpUtility.UrlEncode(Game)));

            if (!content.Contains(@"""ok"""))
            {
                if (OnDescriptionSetError != null)
                    OnDescriptionSetError(this, EventArgs.Empty);
            }

        }
        private String HtmlGet( String url, String parameters )
        {
            lock (loginLock)
            {
                try
                {
                    loginWC.Headers["X-Requested-With"] = "XMLHttpRequest";
                    loginWC.Headers["Twitch-Api-Token"] = api_token;
                    loginWC.Headers["X-CSRF-Token"] = csrf_token;
                    if (!String.IsNullOrEmpty(parameters))
                        url = url + parameters;

                    return loginWC.DownloadString(url);
                }
                catch (Exception e)
                {
                    Debug.Print("TwitchWeb HtmlGet error: {0}", e.Message);
                    return String.Empty;
                }
            }
        }
        private String HtmlPost(String url, String parameters)
        {
            lock (loginLock)
            {
                try
                {
                    loginWC.Headers["X-Requested-With"] = "XMLHttpRequest";
                    loginWC.Headers["Twitch-Api-Token"] = api_token;
                    loginWC.Headers["X-CSRF-Token"] = csrf_token;

                    return loginWC.UploadString(url, parameters);
                }
                catch (Exception e)
                {
                    Debug.Print("TwitchWeb HtmlPost error: {0}", e.Message);
                    return String.Empty;
                }
            }
        }


    }


}
