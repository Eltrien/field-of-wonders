using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dotWebClient;
using dotUtilities;
using dot.Json;
using dot.Json.Linq;
using System.Diagnostics;

namespace dotTwitchTV
{

    public class TwitchOAuth
    {
        private const string loginPopupUrl = "http://ru.twitch.tv/user/login_popup";
        private const string loginUrl = "https://secure.twitch.tv/user/login";
        private const string oauthUrl = "http://api.twitch.tv/api/me?on_site=1";
        private const string reAuthToken = @"^.*authenticity_token.*?value=""(.*?)""";
        private const string loginParams = "utf8=%E2%9C%93&authenticity_token={0}%3D&redirect_on_login=&embed_form=false&user%5Blogin%5D={1}&user%5Bpassword%5D={2}";
        private object loginLock = new object();
        private CookieAwareWebClient loginWC;

        public event EventHandler<EventArgs> OnLogin;
        public event EventHandler<EventArgs> OnLoginFailed;
        public TwitchOAuth(String user, String password)
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

                    String token = Re.GetSubString(result, reAuthToken, 1);
                    if (String.IsNullOrEmpty(token))
                        return false;

                    loginWC.ContentType = ContentType.UrlEncoded;
                    loginWC.Headers["Accept"] = "text/html, application/xhtml+xml, */*";
                    result = loginWC.UploadString(loginUrl, String.Format(loginParams, token, User, Password));
                    if (String.IsNullOrEmpty(result))
                        return false;
                    String api_token = loginWC.CookieValue("api_token", "http://twitch.tv");

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
    }


}
