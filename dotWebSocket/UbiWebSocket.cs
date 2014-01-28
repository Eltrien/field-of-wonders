using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocket4Net;
using System.Net;
using System.Diagnostics;

namespace dotWebSocket
{

    public class UbiWebSocket
    {
        private WebSocket socket;
        public List<KeyValuePair<string, string>> Cookies
        {
            get;
            set;
        }
        public String Domain
        {
            get;
            set;
        }
        public String Path
        {
            get;
            set;
        }
        public String Port
        {
            get;
            set;
        }
        public void Connect()
        {
            String url;

            if (String.IsNullOrEmpty(Port) ||
                String.IsNullOrEmpty(Domain))
                return;

            url = String.Format("ws://{0}:{1}{2}", Domain, Port, Path);

            try
            {

                socket = new WebSocket(
                    url,
                    "",
                    Cookies,
                    null,
                    null,
                    "http://" + Domain,
                    WebSocketVersion.DraftHybi10
                    );
                socket.Opened += new EventHandler(socket_Opened);
                socket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(socket_MessageReceived);
                socket.Closed += new EventHandler(socket_Closed);
                socket.Open();
            }
            catch (Exception e)
            {
                Debug.Print(String.Format("Websocket connection failed. {0} {1}", url, e.Message));
            }
        }

        void socket_Closed(object sender, EventArgs e)
        {
            OnDisconnect();   
        }
        public virtual void OnDisconnect()
        {
        }
        public virtual void OnConnect()
        {
        }
        public int PingInterval
        {
            set
            {
                if (socket != null)
                {
                    if (value <= 0)
                    {
                        socket.AutoSendPingInterval = 0;
                        socket.EnableAutoSendPing = false;
                    }
                    else
                    {
                        socket.AutoSendPingInterval = value;
                        socket.EnableAutoSendPing = true;
                    }
                }
            }
        }
        public void Send(string message)
        {
            socket.Send(message);
        }
        void socket_Opened(object sender, EventArgs e)
        {
            OnConnect();
        }

        public virtual void OnMessage(String message)
        {

        }

        void socket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            OnMessage(e.Message);
        }
    }
}
