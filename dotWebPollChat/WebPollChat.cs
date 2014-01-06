using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using dotWebClient;
using System.Diagnostics;
using System.Net;
using System.Web;

namespace dotWebPollChat
{
    public enum RequestMethod
    {
        GET,
        POST,
        LOWPOST
    }
    public class WebPollChat
    {
        private Timer chatPollTimer;
        private String user, password;
        private object lockChatDownload;
        private CookieAwareWebClient chatWC;
        public WebPollChat()
        {
            chatPollTimer = new Timer( timerPollChatTick, null, Timeout.Infinite, Timeout.Infinite );
            lockChatDownload = new object();
            Headers = new WebHeaderCollection();
            ChatPollPeriod = 10000;
            Method = RequestMethod.GET;
            chatWC = new CookieAwareWebClient();
        }
        public long ChatPollPeriod
        {
            get;
            set;
        }

        public virtual String ChatURL
        {
            get;
            set;
        }
        public CookieContainer Cookies
        {
            get { return chatWC.Cookies; }
            set { chatWC.Cookies = value; }
        }
        public virtual void Login(String user, String password)
        {
        }
        public RequestMethod Method
        {
            get;
            set;
        }
        public String ChatParams
        {
            get;
            set;
        }
        public WebHeaderCollection Headers
        {
            get;
            set;
        }
        public virtual void OnChatReceive(String content)
        {
            
        }

        private void timerPollChatTick( object state )
        {
            if (String.IsNullOrEmpty(ChatURL) || 
                !ChatURL.StartsWith("http"))
                return;
            lock (lockChatDownload)
            {
                String result = string.Empty;
                chatWC.Headers = Headers;
                try
                {
                        switch (Method)
                        {
                            case RequestMethod.GET:
                                result = chatWC.DownloadString( ChatURL);
                                break;
                            case RequestMethod.POST:
                                result = chatWC.UploadString(ChatURL, ChatParams);
                                break;
                            case RequestMethod.LOWPOST:
                                result = chatWC.postFormDataLowLevel(ChatURL, ChatParams);
                                break;

                        }
                        OnChatReceive(result);

                }
                catch (Exception e)
                {
                    Debug.Print(String.Format("WebPollChat download error: {0} {1}", ChatURL, e.Message));
                }
            }
        }
        public virtual void Start()
        {
            chatPollTimer.Change(0, ChatPollPeriod);
        }

        public virtual void Stop()
        {
            chatPollTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

    }
}
