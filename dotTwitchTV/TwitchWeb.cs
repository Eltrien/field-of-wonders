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
using System.Threading;

namespace dotTwitchTV
{

    public class TwitchWeb : IChatDescription, IChatGameQuery
    {
        private const string loginPopupUrl = "http://ru.twitch.tv/user/login_popup";
        private const string urlEditPage = @"http://www.twitch.tv/{0}/update";
        private const string urlGetDesc = @"http://api.twitch.tv/api/channels/{0}/ember?on_site=1";
        private const string domain = "twitch.tv";
        private const string editParams = @"title={0}&meta_game={1}";
        private const string loginUrl = "https://secure.twitch.tv/user/login";
        private const string oauthUrl = "http://api.twitch.tv/api/me?on_site=1";
        private const string urlSearchGame = @"http://www.twitch.tv/discovery/search?term={0}";
        private const string reAuthToken = @"^.*authenticity_token.*?value=""(.*?)""";
        private const string loginParams = "utf8=%E2%9C%93&authenticity_token={0}%3D&redirect_on_login=&embed_form=false&user%5Blogin%5D={1}&user%5Bpassword%5D={2}";
        private const string reDescription = @"<textarea.*?id='status'.*?>(.*?)</textarea>";
        private const string reGame = @"<input.*?id='gameselector_input'.*?name='gameselector'[^>]+value='(.*?)'";
        private object loginLock = new object();
        private object queryGameLock = new object();
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
            GameList = new List<KeyValuePair<string, string>>();
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
        public string Gameid
        {
            get;
            set;
        }
        public void GetDescription()
        {
            var content = HtmlGet(String.Format(urlGetDesc, User.ToLower()), String.Empty);

            if (String.IsNullOrEmpty(content))
                return;

            JToken ember = JToken.Parse(content);
            if (ember == null)
                return;

            var desc = ember["status"].Value<String>();
            var game = ember["game"].Value<String>();

            if (!String.IsNullOrEmpty(desc))
            {
                ShortDescription = desc;
                LongDescription = ShortDescription;
            }

            if( !String.IsNullOrEmpty(game))
                Game = game;


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
                    loginWC.ContentType = ContentType.UrlEncodedUTF8;                    

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
                    loginWC.ContentType = ContentType.UrlEncodedUTF8;

                    return loginWC.UploadString(url, parameters);
                }
                catch (Exception e)
                {
                    Debug.Print("TwitchWeb HtmlPost error: {0}", e.Message);
                    return String.Empty;
                }
            }
        }



        public event EventHandler<EventArgs> OnGameList;

        public void SearchGame(string name)
        {
            queryGameList(name);
        }

        private void queryGameList(String name)
        {
            lock (queryGameLock)
            {
                GameList.Clear();

                var content = HtmlGet(String.Format(urlSearchGame, HttpUtility.UrlEncode(name)), String.Empty);
                Debug.Print("Twitch games starting from {0}: {1}", name, content);

                if (String.IsNullOrEmpty(content))
                    return;

                try
                {
                    JArray games = JArray.Parse(content);
                    if (games.Count <= 0)
                        return;

                    foreach (JObject game in games)
                    {
                        GameList.Add(new KeyValuePair<String, String>(game["id"].ToString(), game["name"].ToString()));
                    }
                }
                catch (Exception e)
                {
                    Debug.Print("TwitchWeb queryGameList error: {0}", e.Message);
                }

                if (OnGameList != null)
                    OnGameList(this, EventArgs.Empty);

            }
        }
        public string GameId
        {
            get;
            set;
        }
        public List<KeyValuePair<string, string>> GameList
        {
            get;
            set;
        }
    }


}
