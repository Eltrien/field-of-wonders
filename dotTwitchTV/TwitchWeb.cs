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
            //{"mature":null,"abuse_reported":null,"status":"[RU/EN] Simulator Battles - War Thunder ","display_name":"Xedoc","game":"War Thunder",
            //"delay":0,"_id":7795079,"name":"xedoc","created_at":"2009-08-17T17:17:39Z","updated_at":"2014-02-01T07:16:58Z","primary_team_name":"highcorporation","primary_team_display_name":"Xtreme Gamers ®","logo":"http://static-cdn.jtvnw.net/jtv_user_pictures/xedoc-profile_image-382090dbfd835270-300x300.png","banner":null,"video_banner":"http://static-cdn.jtvnw.net/jtv_user_pictures/xedoc-channel_offline_image-a989fd994abe6bb0-640x360.png","background":null,"profile_banner":null,"profile_banner_background_color":null,"url":"http://www.twitch.tv/xedoc","views":241873,"followers":1193,"show_chat":true,"show_videos":true}
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
            //System.Diagnostics.StackTrace t = new System.Diagnostics.StackTrace();
            //foreach (var f in t.GetFrames())
            //    Debug.Print("Twitch SetDescription: {0}: {1} {2}", f.GetFileLineNumber().ToString(), f.GetMethod().ToString(),f.GetMethod().GetParameters().ToString());

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

                //Twitch games: [{"name":"War Thunder","id":66366,"giantbombId":38992,"images":{"icon":"http://static.giantbomb.com/uploads/square_avatar/13/138444/2295942-w_warthunder_keyart_small.jpg","tiny":"http://static.giantbomb.com/uploads/square_mini/13/138444/2295942-w_warthunder_keyart_small.jpg","small":"http://static.giantbomb.com/uploads/scale_small/13/138444/2295942-w_warthunder_keyart_small.jpg","super":"http://static.giantbomb.com/uploads/scale_large/13/138444/2295942-w_warthunder_keyart_small.jpg","thumb":"http://static.giantbomb.com/uploads/scale_avatar/13/138444/2295942-w_warthunder_keyart_small.jpg","medium":"http://static.giantbomb.com/uploads/scale_medium/13/138444/2295942-w_warthunder_keyart_small.jpg","screen":"http://static.giantbomb.com/uploads/screen_medium/13/138444/2295942-w_warthunder_keyart_small.jpg"},"popularity":11,"boxArt":{"template":"http://static-cdn.jtvnw.net/ttv-boxart/War%20Thunder-{width}x{height}.jpg","small":"http://static-cdn.jtvnw.net/ttv-boxart/War%20Thunder-52x72.jpg","medium":"http://static-cdn.jtvnw.net/ttv-boxart/War%20Thunder-136x190.jpg","large":"http://static-cdn.jtvnw.net/ttv-boxart/War%20Thunder-272x380.jpg"},"logoArt":{"template":"http://static-cdn.jtvnw.net/ttv-logoart/War%20Thunder-{width}x{height}.jpg","small":"http://static-cdn.jtvnw.net/ttv-logoart/War%20Thunder-60x36.jpg","medium":"http://static-cdn.jtvnw.net/ttv-logoart/War%20Thunder-120x72.jpg","large":"http://static-cdn.jtvnw.net/ttv-logoart/War%20Thunder-240x144.jpg"}},{"name":"War of The Immortals","id":33218,"giantbombId":37035,"images":{"icon":"http://static.giantbomb.com/uploads/square_avatar/8/87749/2080224-war_of_the_immortals_logo.jpg","tiny":"http://static.giantbomb.com/uploads/square_mini/8/87749/2080224-war_of_the_immortals_logo.jpg","small":"http://static.giantbomb.com/uploads/scale_small/8/87749/2080224-war_of_the_immortals_logo.jpg","super":"http://static.giantbomb.com/uploads/scale_large/8/87749/2080224-war_of_the_immortals_logo.jpg","thumb":"http://static.giantbomb.com/uploads/scale_avatar/8/87749/2080224-war_of_the_immortals_logo.jpg","medium":"http://static.giantbomb.com/uploads/scale_medium/8/87749/2080224-war_of_the_immortals_logo.jpg","screen":"http://static.giantbomb.com/uploads/screen_medium/8/87749/2080224-war_of_the_immortals_logo.jpg"},"popularity":1,"boxArt":{"template":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20The%20Immortals-{width}x{height}.jpg","small":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20The%20Immortals-52x72.jpg","medium":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20The%20Immortals-136x190.jpg","large":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20The%20Immortals-272x380.jpg"},"logoArt":{"template":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20The%20Immortals-{width}x{height}.jpg","small":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20The%20Immortals-60x36.jpg","medium":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20The%20Immortals-120x72.jpg","large":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20The%20Immortals-240x144.jpg"}},{"name":"War of the Vikings","id":313414,"giantbombId":43504,"images":{"icon":"http://static.giantbomb.com/uploads/square_avatar/2/22486/2528199-war_of_the_vikings_plaza_banner.jpg","tiny":"http://static.giantbomb.com/uploads/square_mini/2/22486/2528199-war_of_the_vikings_plaza_banner.jpg","small":"http://static.giantbomb.com/uploads/scale_small/2/22486/2528199-war_of_the_vikings_plaza_banner.jpg","super":"http://static.giantbomb.com/uploads/scale_large/2/22486/2528199-war_of_the_vikings_plaza_banner.jpg","thumb":"http://static.giantbomb.com/uploads/scale_avatar/2/22486/2528199-war_of_the_vikings_plaza_banner.jpg","medium":"http://static.giantbomb.com/uploads/scale_medium/2/22486/2528199-war_of_the_vikings_plaza_banner.jpg","screen":"http://static.giantbomb.com/uploads/screen_medium/2/22486/2528199-war_of_the_vikings_plaza_banner.jpg"},"popularity":1,"boxArt":{"template":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20the%20Vikings-{width}x{height}.jpg","small":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20the%20Vikings-52x72.jpg","medium":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20the%20Vikings-136x190.jpg","large":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20the%20Vikings-272x380.jpg"},"logoArt":{"template":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20the%20Vikings-{width}x{height}.jpg","small":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20the%20Vikings-60x36.jpg","medium":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20the%20Vikings-120x72.jpg","large":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20the%20Vikings-240x144.jpg"}},{"name":"War of the Roses","id":32448,"giantbombId":36170,"images":{"icon":"http://static.giantbomb.com/uploads/square_avatar/8/87790/2323889-box_wotr.png","tiny":"http://static.giantbomb.com/uploads/square_mini/8/87790/2323889-box_wotr.png","small":"http://static.giantbomb.com/uploads/scale_small/8/87790/2323889-box_wotr.png","super":"http://static.giantbomb.com/uploads/scale_large/8/87790/2323889-box_wotr.png","thumb":"http://static.giantbomb.com/uploads/scale_avatar/8/87790/2323889-box_wotr.png","medium":"http://static.giantbomb.com/uploads/scale_medium/8/87790/2323889-box_wotr.png","screen":"http://static.giantbomb.com/uploads/screen_medium/8/87790/2323889-box_wotr.png"},"popularity":1,"boxArt":{"template":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20the%20Roses-{width}x{height}.jpg","small":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20the%20Roses-52x72.jpg","medium":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20the%20Roses-136x190.jpg","large":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20the%20Roses-272x380.jpg"},"logoArt":{"template":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20the%20Roses-{width}x{height}.jpg","small":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20the%20Roses-60x36.jpg","medium":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20the%20Roses-120x72.jpg","large":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20the%20Roses-240x144.jpg"}},{"name":"WarRock","id":20179,"giantbombId":22218,"images":{"icon":"http://static.giantbomb.com/uploads/square_avatar/0/217/326247-warrock_cover.jpg","tiny":"http://static.giantbomb.com/uploads/square_mini/0/217/326247-warrock_cover.jpg","small":"http://static.giantbomb.com/uploads/scale_small/0/217/326247-warrock_cover.jpg","super":"http://static.giantbomb.com/uploads/scale_large/0/217/326247-warrock_cover.jpg","thumb":"http://static.giantbomb.com/uploads/scale_avatar/0/217/326247-warrock_cover.jpg","medium":"http://static.giantbomb.com/uploads/scale_medium/0/217/326247-warrock_cover.jpg","screen":"http://static.giantbomb.com/uploads/screen_medium/0/217/326247-warrock_cover.jpg"},"popularity":1,"boxArt":{"template":"http://static-cdn.jtvnw.net/ttv-boxart/WarRock-{width}x{height}.jpg","small":"http://static-cdn.jtvnw.net/ttv-boxart/WarRock-52x72.jpg","medium":"http://static-cdn.jtvnw.net/ttv-boxart/WarRock-136x190.jpg","large":"http://static-cdn.jtvnw.net/ttv-boxart/WarRock-272x380.jpg"},"logoArt":{"template":"http://static-cdn.jtvnw.net/ttv-logoart/WarRock-{width}x{height}.jpg","small":"http://static-cdn.jtvnw.net/ttv-logoart/WarRock-60x36.jpg","medium":"http://static-cdn.jtvnw.net/ttv-logoart/WarRock-120x72.jpg","large":"http://static-cdn.jtvnw.net/ttv-logoart/WarRock-240x144.jpg"}},{"name":"War of the Dead","id":30448,"giantbombId":33862,"images":{"icon":"http://static.giantbomb.com/uploads/square_avatar/9/97089/1684417-wardeadcover.png","tiny":"http://static.giantbomb.com/uploads/square_mini/9/97089/1684417-wardeadcover.png","small":"http://static.giantbomb.com/uploads/scale_small/9/97089/1684417-wardeadcover.png","super":"http://static.giantbomb.com/uploads/scale_large/9/97089/1684417-wardeadcover.png","thumb":"http://static.giantbomb.com/uploads/scale_avatar/9/97089/1684417-wardeadcover.png","medium":"http://static.giantbomb.com/uploads/scale_medium/9/97089/1684417-wardeadcover.png","screen":"http://static.giantbomb.com/uploads/screen_medium/9/97089/1684417-wardeadcover.png"},"popularity":1,"boxArt":{"template":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20the%20Dead-{width}x{height}.jpg","small":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20the%20Dead-52x72.jpg","medium":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20the%20Dead-136x190.jpg","large":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20the%20Dead-272x380.jpg"},"logoArt":{"template":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20the%20Dead-{width}x{height}.jpg","small":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20the%20Dead-60x36.jpg","medium":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20the%20Dead-120x72.jpg","large":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20the%20Dead-240x144.jpg"}},{"name":"War Inc. Battlezone","id":32222,"giantbombId":35923,"images":{"icon":"http://static.giantbomb.com/uploads/square_avatar/0/4344/1842275-warinc_wallpaper1_1920x1080.jpg","tiny":"http://static.giantbomb.com/uploads/square_mini/0/4344/1842275-warinc_wallpaper1_1920x1080.jpg","small":"http://static.giantbomb.com/uploads/scale_small/0/4344/1842275-warinc_wallpaper1_1920x1080.jpg","super":"http://static.giantbomb.com/uploads/scale_large/0/4344/1842275-warinc_wallpaper1_1920x1080.jpg","thumb":"http://static.giantbomb.com/uploads/scale_avatar/0/4344/1842275-warinc_wallpaper1_1920x1080.jpg","medium":"http://static.giantbomb.com/uploads/scale_medium/0/4344/1842275-warinc_wallpaper1_1920x1080.jpg","screen":"http://static.giantbomb.com/uploads/screen_medium/0/4344/1842275-warinc_wallpaper1_1920x1080.jpg"},"popularity":1,"boxArt":{"template":"http://static-cdn.jtvnw.net/ttv-boxart/War%20Inc.%20Battlezone-{width}x{height}.jpg","small":"http://static-cdn.jtvnw.net/ttv-boxart/War%20Inc.%20Battlezone-52x72.jpg","medium":"http://static-cdn.jtvnw.net/ttv-boxart/War%20Inc.%20Battlezone-136x190.jpg","large":"http://static-cdn.jtvnw.net/ttv-boxart/War%20Inc.%20Battlezone-272x380.jpg"},"logoArt":{"template":"http://static-cdn.jtvnw.net/ttv-logoart/War%20Inc.%20Battlezone-{width}x{height}.jpg","small":"http://static-cdn.jtvnw.net/ttv-logoart/War%20Inc.%20Battlezone-60x36.jpg","medium":"http://static-cdn.jtvnw.net/ttv-logoart/War%20Inc.%20Battlezone-120x72.jpg","large":"http://static-cdn.jtvnw.net/ttv-logoart/War%20Inc.%20Battlezone-240x144.jpg"}},{"name":"War Zone","id":3662,"giantbombId":4016,"images":{},"popularity":null,"boxArt":{"template":"http://static-cdn.jtvnw.net/ttv-boxart/War%20Zone-{width}x{height}.jpg","small":"http://static-cdn.jtvnw.net/ttv-boxart/War%20Zone-52x72.jpg","medium":"http://static-cdn.jtvnw.net/ttv-boxart/War%20Zone-136x190.jpg","large":"http://static-cdn.jtvnw.net/ttv-boxart/War%20Zone-272x380.jpg"},"logoArt":{"template":"http://static-cdn.jtvnw.net/ttv-logoart/War%20Zone-{width}x{height}.jpg","small":"http://static-cdn.jtvnw.net/ttv-logoart/War%20Zone-60x36.jpg","medium":"http://static-cdn.jtvnw.net/ttv-logoart/War%20Zone-120x72.jpg","large":"http://static-cdn.jtvnw.net/ttv-logoart/War%20Zone-240x144.jpg"}},{"name":"War Chess","id":3201,"giantbombId":3492,"images":{},"popularity":null,"boxArt":{"template":"http://static-cdn.jtvnw.net/ttv-boxart/War%20Chess-{width}x{height}.jpg","small":"http://static-cdn.jtvnw.net/ttv-boxart/War%20Chess-52x72.jpg","medium":"http://static-cdn.jtvnw.net/ttv-boxart/War%20Chess-136x190.jpg","large":"http://static-cdn.jtvnw.net/ttv-boxart/War%20Chess-272x380.jpg"},"logoArt":{"template":"http://static-cdn.jtvnw.net/ttv-logoart/War%20Chess-{width}x{height}.jpg","small":"http://static-cdn.jtvnw.net/ttv-logoart/War%20Chess-60x36.jpg","medium":"http://static-cdn.jtvnw.net/ttv-logoart/War%20Chess-120x72.jpg","large":"http://static-cdn.jtvnw.net/ttv-logoart/War%20Chess-240x144.jpg"}},{"name":"War Machine","id":13037,"giantbombId":14205,"images":{},"popularity":null,"boxArt":{"template":"http://static-cdn.jtvnw.net/ttv-boxart/War%20Machine-{width}x{height}.jpg","small":"http://static-cdn.jtvnw.net/ttv-boxart/War%20Machine-52x72.jpg","medium":"http://static-cdn.jtvnw.net/ttv-boxart/War%20Machine-136x190.jpg","large":"http://static-cdn.jtvnw.net/ttv-boxart/War%20Machine-272x380.jpg"},"logoArt":{"template":"http://static-cdn.jtvnw.net/ttv-logoart/War%20Machine-{width}x{height}.jpg","small":"http://static-cdn.jtvnw.net/ttv-logoart/War%20Machine-60x36.jpg","medium":"http://static-cdn.jtvnw.net/ttv-logoart/War%20Machine-120x72.jpg","large":"http://static-cdn.jtvnw.net/ttv-logoart/War%20Machine-240x144.jpg"}},{"name":"War of the Dead Part 2","id":30447,"giantbombId":33863,"images":{"icon":"http://static.giantbomb.com/uploads/square_avatar/9/97089/1682802-warcover2.png","tiny":"http://static.giantbomb.com/uploads/square_mini/9/97089/1682802-warcover2.png","small":"http://static.giantbomb.com/uploads/scale_small/9/97089/1682802-warcover2.png","super":"http://static.giantbomb.com/uploads/scale_large/9/97089/1682802-warcover2.png","thumb":"http://static.giantbomb.com/uploads/scale_avatar/9/97089/1682802-warcover2.png","medium":"http://static.giantbomb.com/uploads/scale_medium/9/97089/1682802-warcover2.png","screen":"http://static.giantbomb.com/uploads/screen_medium/9/97089/1682802-warcover2.png"},"popularity":null,"boxArt":{"template":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20the%20Dead%20Part%202-{width}x{height}.jpg","small":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20the%20Dead%20Part%202-52x72.jpg","medium":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20the%20Dead%20Part%202-136x190.jpg","large":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20the%20Dead%20Part%202-272x380.jpg"},"logoArt":{"template":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20the%20Dead%20Part%202-{width}x{height}.jpg","small":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20the%20Dead%20Part%202-60x36.jpg","medium":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20the%20Dead%20Part%202-120x72.jpg","large":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20the%20Dead%20Part%202-240x144.jpg"}},{"name":"War of Genesis III","id":21116,"giantbombId":23280,"images":{"icon":"http://static.giantbomb.com/uploads/square_avatar/0/5128/554470-logo.jpg","tiny":"http://static.giantbomb.com/uploads/square_mini/0/5128/554470-logo.jpg","small":"http://static.giantbomb.com/uploads/scale_small/0/5128/554470-logo.jpg","super":"http://static.giantbomb.com/uploads/scale_large/0/5128/554470-logo.jpg","thumb":"http://static.giantbomb.com/uploads/scale_avatar/0/5128/554470-logo.jpg","medium":"http://static.giantbomb.com/uploads/scale_medium/0/5128/554470-logo.jpg","screen":"http://static.giantbomb.com/uploads/screen_medium/0/5128/554470-logo.jpg"},"popularity":null,"boxArt":{"template":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20Genesis%20III-{width}x{height}.jpg","small":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20Genesis%20III-52x72.jpg","medium":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20Genesis%20III-136x190.jpg","large":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20Genesis%20III-272x380.jpg"},"logoArt":{"template":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20Genesis%20III-{width}x{height}.jpg","small":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20Genesis%20III-60x36.jpg","medium":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20Genesis%20III-120x72.jpg","large":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20Genesis%20III-240x144.jpg"}},{"name":"War World: Tactical Combat","id":15867,"giantbombId":17302,"images":{"icon":"http://static.giantbomb.com/uploads/square_avatar/8/87790/1858027-box_wwtc.png","tiny":"http://static.giantbomb.com/uploads/square_mini/8/87790/1858027-box_wwtc.png","small":"http://static.giantbomb.com/uploads/scale_small/8/87790/1858027-box_wwtc.png","super":"http://static.giantbomb.com/uploads/scale_large/8/87790/1858027-box_wwtc.png","thumb":"http://static.giantbomb.com/uploads/scale_avatar/8/87790/1858027-box_wwtc.png","medium":"http://static.giantbomb.com/uploads/scale_medium/8/87790/1858027-box_wwtc.png","screen":"http://static.giantbomb.com/uploads/screen_medium/8/87790/1858027-box_wwtc.png"},"popularity":null,"boxArt":{"template":"http://static-cdn.jtvnw.net/ttv-boxart/War%20World%3A%20Tactical%20Combat-{width}x{height}.jpg","small":"http://static-cdn.jtvnw.net/ttv-boxart/War%20World%3A%20Tactical%20Combat-52x72.jpg","medium":"http://static-cdn.jtvnw.net/ttv-boxart/War%20World%3A%20Tactical%20Combat-136x190.jpg","large":"http://static-cdn.jtvnw.net/ttv-boxart/War%20World%3A%20Tactical%20Combat-272x380.jpg"},"logoArt":{"template":"http://static-cdn.jtvnw.net/ttv-logoart/War%20World%3A%20Tactical%20Combat-{width}x{height}.jpg","small":"http://static-cdn.jtvnw.net/ttv-logoart/War%20World%3A%20Tactical%20Combat-60x36.jpg","medium":"http://static-cdn.jtvnw.net/ttv-logoart/War%20World%3A%20Tactical%20Combat-120x72.jpg","large":"http://static-cdn.jtvnw.net/ttv-logoart/War%20World%3A%20Tactical%20Combat-240x144.jpg"}},{"name":"War of Eclipse","id":99264,"giantbombId":40987,"images":{"icon":"http://static.giantbomb.com/uploads/square_avatar/2/24833/2402501-war_of_eclipse.png","tiny":"http://static.giantbomb.com/uploads/square_mini/2/24833/2402501-war_of_eclipse.png","small":"http://static.giantbomb.com/uploads/scale_small/2/24833/2402501-war_of_eclipse.png","super":"http://static.giantbomb.com/uploads/scale_large/2/24833/2402501-war_of_eclipse.png","thumb":"http://static.giantbomb.com/uploads/scale_avatar/2/24833/2402501-war_of_eclipse.png","medium":"http://static.giantbomb.com/uploads/scale_medium/2/24833/2402501-war_of_eclipse.png","screen":"http://static.giantbomb.com/uploads/screen_medium/2/24833/2402501-war_of_eclipse.png"},"popularity":null,"boxArt":{"template":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20Eclipse-{width}x{height}.jpg","small":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20Eclipse-52x72.jpg","medium":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20Eclipse-136x190.jpg","large":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20Eclipse-272x380.jpg"},"logoArt":{"template":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20Eclipse-{width}x{height}.jpg","small":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20Eclipse-60x36.jpg","medium":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20Eclipse-120x72.jpg","large":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20Eclipse-240x144.jpg"}},{"name":"War of Nerves!","id":1004,"giantbombId":1100,"images":{"icon":"http://static.giantbomb.com/uploads/square_avatar/0/5278/288486-war_of_nerves.jpg","tiny":"http://static.giantbomb.com/uploads/square_mini/0/5278/288486-war_of_nerves.jpg","small":"http://static.giantbomb.com/uploads/scale_small/0/5278/288486-war_of_nerves.jpg","super":"http://static.giantbomb.com/uploads/scale_large/0/5278/288486-war_of_nerves.jpg","thumb":"http://static.giantbomb.com/uploads/scale_avatar/0/5278/288486-war_of_nerves.jpg","medium":"http://static.giantbomb.com/uploads/scale_medium/0/5278/288486-war_of_nerves.jpg","screen":"http://static.giantbomb.com/uploads/screen_medium/0/5278/288486-war_of_nerves.jpg"},"popularity":null,"boxArt":{"template":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20Nerves!-{width}x{height}.jpg","small":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20Nerves!-52x72.jpg","medium":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20Nerves!-136x190.jpg","large":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20Nerves!-272x380.jpg"},"logoArt":{"template":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20Nerves!-{width}x{height}.jpg","small":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20Nerves!-60x36.jpg","medium":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20Nerves!-120x72.jpg","large":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20Nerves!-240x144.jpg"}},{"name":"War of Sonria","id":93310,"giantbombId":40857,"images":{"icon":"http://static.giantbomb.com/uploads/square_avatar/14/149021/2388377-warofsonria_featuredimage.jpg","tiny":"http://static.giantbomb.com/uploads/square_mini/14/149021/2388377-warofsonria_featuredimage.jpg","small":"http://static.giantbomb.com/uploads/scale_small/14/149021/2388377-warofsonria_featuredimage.jpg","super":"http://static.giantbomb.com/uploads/scale_large/14/149021/2388377-warofsonria_featuredimage.jpg","thumb":"http://static.giantbomb.com/uploads/scale_avatar/14/149021/2388377-warofsonria_featuredimage.jpg","medium":"http://static.giantbomb.com/uploads/scale_medium/14/149021/2388377-warofsonria_featuredimage.jpg","screen":"http://static.giantbomb.com/uploads/screen_medium/14/149021/2388377-warofsonria_featuredimage.jpg"},"popularity":null,"boxArt":{"template":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20Sonria-{width}x{height}.jpg","small":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20Sonria-52x72.jpg","medium":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20Sonria-136x190.jpg","large":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20Sonria-272x380.jpg"},"logoArt":{"template":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20Sonria-{width}x{height}.jpg","small":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20Sonria-60x36.jpg","medium":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20Sonria-120x72.jpg","large":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20Sonria-240x144.jpg"}},{"name":"War of the Lance","id":16844,"giantbombId":18379,"images":{"icon":"http://static.giantbomb.com/uploads/square_avatar/1/15693/614756-916888_40937_front_1_.jpg","tiny":"http://static.giantbomb.com/uploads/square_mini/1/15693/614756-916888_40937_front_1_.jpg","small":"http://static.giantbomb.com/uploads/scale_small/1/15693/614756-916888_40937_front_1_.jpg","super":"http://static.giantbomb.com/uploads/scale_large/1/15693/614756-916888_40937_front_1_.jpg","thumb":"http://static.giantbomb.com/uploads/scale_avatar/1/15693/614756-916888_40937_front_1_.jpg","medium":"http://static.giantbomb.com/uploads/scale_medium/1/15693/614756-916888_40937_front_1_.jpg","screen":"http://static.giantbomb.com/uploads/screen_medium/1/15693/614756-916888_40937_front_1_.jpg"},"popularity":null,"boxArt":{"template":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20the%20Lance-{width}x{height}.jpg","small":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20the%20Lance-52x72.jpg","medium":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20the%20Lance-136x190.jpg","large":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20the%20Lance-272x380.jpg"},"logoArt":{"template":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20the%20Lance-{width}x{height}.jpg","small":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20the%20Lance-60x36.jpg","medium":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20the%20Lance-120x72.jpg","large":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20the%20Lance-240x144.jpg"}},{"name":"War Front: Turning Point","id":18466,"giantbombId":20195,"images":{"icon":"http://static.giantbomb.com/uploads/square_avatar/0/1379/1293229-war_front___turning_point_coverart.png","tiny":"http://static.giantbomb.com/uploads/square_mini/0/1379/1293229-war_front___turning_point_coverart.png","small":"http://static.giantbomb.com/uploads/scale_small/0/1379/1293229-war_front___turning_point_coverart.png","super":"http://static.giantbomb.com/uploads/scale_large/0/1379/1293229-war_front___turning_point_coverart.png","thumb":"http://static.giantbomb.com/uploads/scale_avatar/0/1379/1293229-war_front___turning_point_coverart.png","medium":"http://static.giantbomb.com/uploads/scale_medium/0/1379/1293229-war_front___turning_point_coverart.png","screen":"http://static.giantbomb.com/uploads/screen_medium/0/1379/1293229-war_front___turning_point_coverart.png"},"popularity":null,"boxArt":{"template":"http://static-cdn.jtvnw.net/ttv-boxart/War%20Front%3A%20Turning%20Point-{width}x{height}.jpg","small":"http://static-cdn.jtvnw.net/ttv-boxart/War%20Front%3A%20Turning%20Point-52x72.jpg","medium":"http://static-cdn.jtvnw.net/ttv-boxart/War%20Front%3A%20Turning%20Point-136x190.jpg","large":"http://static-cdn.jtvnw.net/ttv-boxart/War%20Front%3A%20Turning%20Point-272x380.jpg"},"logoArt":{"template":"http://static-cdn.jtvnw.net/ttv-logoart/War%20Front%3A%20Turning%20Point-{width}x{height}.jpg","small":"http://static-cdn.jtvnw.net/ttv-logoart/War%20Front%3A%20Turning%20Point-60x36.jpg","medium":"http://static-cdn.jtvnw.net/ttv-logoart/War%20Front%3A%20Turning%20Point-120x72.jpg","large":"http://static-cdn.jtvnw.net/ttv-logoart/War%20Front%3A%20Turning%20Point-240x144.jpg"}},{"name":"War of Angels","id":29310,"giantbombId":32565,"images":{"icon":"http://static.giantbomb.com/uploads/square_avatar/16/160705/2239727-warofangelsboxart.jpg","tiny":"http://static.giantbomb.com/uploads/square_mini/16/160705/2239727-warofangelsboxart.jpg","small":"http://static.giantbomb.com/uploads/scale_small/16/160705/2239727-warofangelsboxart.jpg","super":"http://static.giantbomb.com/uploads/scale_large/16/160705/2239727-warofangelsboxart.jpg","thumb":"http://static.giantbomb.com/uploads/scale_avatar/16/160705/2239727-warofangelsboxart.jpg","medium":"http://static.giantbomb.com/uploads/scale_medium/16/160705/2239727-warofangelsboxart.jpg","screen":"http://static.giantbomb.com/uploads/screen_medium/16/160705/2239727-warofangelsboxart.jpg"},"popularity":null,"boxArt":{"template":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20Angels-{width}x{height}.jpg","small":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20Angels-52x72.jpg","medium":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20Angels-136x190.jpg","large":"http://static-cdn.jtvnw.net/ttv-boxart/War%20of%20Angels-272x380.jpg"},"logoArt":{"template":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20Angels-{width}x{height}.jpg","small":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20Angels-60x36.jpg","medium":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20Angels-120x72.jpg","large":"http://static-cdn.jtvnw.net/ttv-logoart/War%20of%20Angels-240x144.jpg"}},{"name":"War Inc.","id":9126,"giantbombId":9955,"images":{},"popularity":null,"boxArt":{"template":"http://static-cdn.jtvnw.net/ttv-boxart/War%20Inc.-{width}x{height}.jpg","small":"http://static-cdn.jtvnw.net/ttv-boxart/War%20Inc.-52x72.jpg","medium":"http://static-cdn.jtvnw.net/ttv-boxart/War%20Inc.-136x190.jpg","large":"http://static-cdn.jtvnw.net/ttv-boxart/War%20Inc.-272x380.jpg"},"logoArt":{"template":"http://static-cdn.jtvnw.net/ttv-logoart/War%20Inc.-{width}x{height}.jpg","small":"http://static-cdn.jtvnw.net/ttv-logoart/War%20Inc.-60x36.jpg","medium":"http://static-cdn.jtvnw.net/ttv-logoart/War%20Inc.-120x72.jpg","large":"http://static-cdn.jtvnw.net/ttv-logoart/War%20Inc.-240x144.jpg"}}]


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
