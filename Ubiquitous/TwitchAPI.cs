using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dotWebClient;
using System.Web;
using System.Net;

namespace Ubiquitous
{

    public class TwitchAPI
    {
        private const string auth_url = "https://api.twitch.tv/kraken/oauth2/token";
        private const string client_id = @"mnh6c0d1vx90003yzmux6qiv15erllf";
        private const string client_secret = @"87z9nkokcwxv6lr4pxmfirx9kywb8l6";
        private const string auth_params = @"client_id={0}&client_secret={1}&username={2}&password={3}&scope=chat_login&grant_type=password";
        private CookieAwareWebClient wc;
        private object getTokenLock;
        public TwitchAPI()
        {
            getTokenLock = new object();
            wc = new CookieAwareWebClient();

        }
        public String GetToken(String user, String password)
        {

            try
            {
                wc.ContentType = ContentType.UrlEncoded;

                var result = wc.UploadString(auth_url,
                    String.Format(auth_params, client_id, client_secret, user,password));
                return result;
            }
            catch
            {
                return String.Empty;
            }
        }
    }
}
