using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocket4Net;
using System.Diagnostics;
using dotUtilities;
using System.Threading;
namespace dotOBS
{


    public class OBSRemote
    {
        public event EventHandler<OBSMessageEventArgs> OnMessage;

        WebSocket socket;
        string _host;
        UInt32 currentMessageId = 1;
        public void Connect(string host)
        {
            _host = host;
            socket = new WebSocket(
                String.Format("ws://{0}:4444", _host),
                "obsapi",
                null,
                null,
                null,
                @"http://client.obsremote.com",
                WebSocketVersion.DraftHybi10
                );

            socket.Opened += new EventHandler(socket_Opened);
            socket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(socket_MessageReceived);
            socket.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(socket_Error);
            socket.Closed += new EventHandler(socket_Closed);
            socket.DataReceived += new EventHandler<WebSocket4Net.DataReceivedEventArgs>(socket_DataReceived);
            Status = new StreamStatus();
            socket.Open();
            
        }
        public bool Opened
        {
            get { return socket.State == WebSocketState.Open; }
        }
        public StreamStatus Status
        {
            get;
            set;
        }
        void socket_DataReceived(object sender, WebSocket4Net.DataReceivedEventArgs e)
        {
        }
        


        void socket_Closed(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        void socket_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            Thread.Sleep(1000);
            Connect(_host);
            //throw new NotImplementedException();
        }

        void socket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if( string.IsNullOrEmpty(e.Message ))
                return;

            if( e.Message.Contains("StreamStatus") )
            {
                var streamStatus = JsonGenerics.ParseJson<StreamStatus>.ReadObject(e.Message);
                if (streamStatus != null)
                    Status = streamStatus;
            }
            if( OnMessage != null )
                OnMessage(this, new OBSMessageEventArgs(e.Message));
            //throw new NotImplementedException();
        }

        void socket_Opened(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            RequestVersion();
            RequestSceneList();
            RequestStreamingStatus();
            RequestVolumes();
        }

        public void RequestVersion()
        {
            var msg = "{\"request-type\":\"GetVersion\",\"message-id\":\"" + MessageId + "\"}";
            socket.Send(msg);
        }
        public void RequestStreamingStatus()
        {
            var msg = "{\"request-type\":\"GetStreamingStatus\",\"message-id\":\"" + MessageId + "\"}";
            socket.Send(msg);
        }
        public void RequestSceneList()
        {
            var msg = "{\"request-type\":\"GetSceneList\",\"message-id\":\"" + MessageId + "\"}";
            socket.Send(msg);
        }
        public void RequestVolumes()
        {
            var msg = "{\"request-type\":\"GetVolumes\",\"message-id\":\"" + MessageId + "\"}";
            socket.Send(msg);
        }
        private string MessageId
        {
            get
            {
                currentMessageId++; return currentMessageId.ToString();
            }            
        }
        public void StartStopStream()
        {
            var msg = "{\"request-type\":\"StartStopStreaming\",\"message-id\":\"" + MessageId + "\"}";
            socket.Send(msg);
        }

    }
    public class OBSMessageEventArgs : EventArgs
    {
        public OBSMessageEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; private set; }
    }
}
