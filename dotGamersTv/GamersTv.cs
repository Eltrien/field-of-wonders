using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dotWebPollChat;
using dotWebClient;
using System.Diagnostics;
using dotUtilities;
using dot.Json;
using dot.Json.Linq;
using System.Web;
using System.Net;

namespace dotGamersTv
{
    public class GamersTVMessage : EventArgs
    {
        public string Text
        {
            get;
            set;
        }
        public string User
        {
            get;
            set;
        }      
    }
    public class GamersTv : WebPollChat
    {
        public event EventHandler<GamersTVMessage> OnMessageReceived;
        public event EventHandler<EventArgs> OnLogin;

        private const String chatBaseUrl = "http://gamerstv.ru/modules/ajax/chat.php?action=update&time={0}&room_id={1}&room_type=streams&user_id={2}";
        private const String reChatParams = @"^.*?room_id=(\d+).*?user_id=(\d+).*$";
        private CookieAwareWebClient loginWC;
        private long RoomId, UserId;
        private object lockLoginWC = new object();
        public GamersTv()
        {
            loginWC = new CookieAwareWebClient();
            LastDate = "undefined";
            ChatPollPeriod = 5000;
        }
        public String ChannelUrl
        {
            get;
            set;
        }
        public String LastDate
        {
            get;
            set;
        }
        public void ReadChatIds()
        {
            String content;
            if (String.IsNullOrEmpty(ChannelUrl))
            {
                Debug.Print("GamersTv: empty channel url");
                return;
            }
            

            lock (lockLoginWC)
            {
                try
                {
                    content = loginWC.DownloadString(ChannelUrl);
                    long.TryParse(Re.GetSubString(content, reChatParams, 1), out RoomId);
                    long.TryParse(Re.GetSubString(content, reChatParams, 2), out UserId);
                }
                catch (Exception e ){
                    Debug.Print(String.Format("GamersTV: read chat ids exception {0} {1}", ChannelUrl, e.Message));
                }

            }
        }
        public override void OnChatReceive(string content)
        {
            if (content == "null" || String.IsNullOrEmpty(content))
                return;
            try
            {
                JObject jsonMsg = JObject.Parse(content);
                var textobj = jsonMsg["text"];
                var date = (string)jsonMsg["date"];

                if (!String.IsNullOrEmpty(date))
                    LastDate = date;
                
                if (textobj == null)
                    return;

                foreach (var msg in textobj)
                {
                    var from = (string)msg["name"];
                    var text = (string)msg["text"];
                    var toId = (string)msg["to_id"];
                
                    if (!String.IsNullOrEmpty( from) && 
                        !String.IsNullOrEmpty(text) && OnMessageReceived != null)
                            OnMessageReceived(this, new GamersTVMessage() { User = from, Text = text});
                }
            }
            catch (Exception e)
            {
                Debug.Print(String.Format("GamersTV: message receive error {0} {1}"), content, e.Message);
            }
        }
        public override string ChatURL
        {
            get
            {
                return String.Format(chatBaseUrl, HttpUtility.UrlEncode(LastDate), RoomId, UserId);
            }
            set
            {
                base.ChatURL = value;
            }
        }
        public override void Start()
        {
            ReadChatIds();
            ChatURL = String.Format(chatBaseUrl, TimeUtils.UnixTimestamp(), RoomId, UserId );
            Cookies = loginWC.Cookies;
            Headers.Add("Accept", "application/json, text/javascript, */*; q=0.01");
            Headers.Add("X-Requested-With", "XMLHttpRequest");
            base.Start();
            if (OnLogin != null)
                OnLogin(this, EventArgs.Empty);
        }
        public override void Stop()
        {
            base.Stop();
        }
        public override void Login(string user, string password)
        {
            
        }

    }
}
