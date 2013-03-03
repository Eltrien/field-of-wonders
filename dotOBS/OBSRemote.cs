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
        public event EventHandler<EventArgs> OnLive;
        public event EventHandler<EventArgs> OnOffline;
        public event EventHandler<EventArgs> OnError;
        public event EventHandler<OBSSceneStatusEventArgs> OnSceneList;
        public event EventHandler<OBSMessageEventArgs> OnSceneSet;
        public event EventHandler<OBSSourceEventArgs> OnSourceChange;

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
        public void SetCurrentScene(string sceneName)
        {
            var msg = "{\"request-type\":\"SetCurrentScene\",\"scene-name\":\"" + sceneName + "\",\"message-id\":\"" + MessageId + "\"}";
            socket.Send(msg);
        }
        public void SetSourceRenderer(String sourceName, bool enable)
        {
            var msg = "{\"request-type\":\"SetSourceRender\",\"source\":\"" + sourceName + "\",\"render\":" + enable.ToString().ToLower() +",\"message-id\":\"" + MessageId + "\"}";
            socket.Send(msg);
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
            if (OnError != null)
                OnError(this, EventArgs.Empty);
        }

        void socket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if( string.IsNullOrEmpty(e.Message ))
                return;

            if (e.Message.Contains("StreamStatus"))
            {
                var streamStatus = JsonGenerics.ParseJson<StreamStatus>.ReadObject(e.Message);
                if (Status != null)
                {
                    if (Status.streaming != streamStatus.streaming)
                    {
                        if (streamStatus.streaming)
                        {
                            if (OnLive != null)
                                OnLive(this, EventArgs.Empty);
                        }
                        else
                        {
                            if (OnOffline != null)
                                OnOffline(this, EventArgs.Empty);
                        }
                    }
                }
                if (streamStatus != null)
                    Status = streamStatus;

            }
            else if (e.Message.Contains("StreamStopping"))
            {
                if (OnOffline != null)
                    OnOffline(this, EventArgs.Empty);

                Status.fps = 0;
                Status.bitrate = 0;
                Status.framesDropped = 0;
                Status.framesTotal = 0;
                Status.strain = 0;
                Status.streaming = false;

            }
            else if (e.Message.Contains("StreamStarting"))
            {
                if (OnLive != null)
                    OnLive(this, EventArgs.Empty);
            }
            else if (e.Message.Contains("\"current-scene\":"))
            {
                var sceneStatus = JsonGenerics.ParseJson<SceneStatus>.ReadObject(e.Message);
                if (sceneStatus != null)
                {
                    if (OnSceneList != null)
                        OnSceneList(this, new OBSSceneStatusEventArgs(sceneStatus));
                }
            }
            else if (e.Message.Contains("\"SwitchScenes\","))
            {
                var sceneSwitch = JsonGenerics.ParseJson<SwitchScenes>.ReadObject(e.Message);
                if (sceneSwitch == null)
                    return;

                if (OnSceneSet != null)
                {
                    OnSceneSet(this, new OBSMessageEventArgs(sceneSwitch.sceneName));
                }
            }
            else if (e.Message.Contains("\"SourceChanged\","))
            {
                var sourceChange = JsonGenerics.ParseJson<SourceChange>.ReadObject(e.Message);
                if (sourceChange == null)
                    return;

                if (OnSourceChange != null)
                {
                    OnSourceChange(this, new OBSSourceEventArgs(sourceChange.source));
                }
            }
            else
            {
                Debug.Print(e.Message);
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
    public class OBSSceneStatusEventArgs : EventArgs
    {
        public OBSSceneStatusEventArgs(SceneStatus status)
        {
            Status = status;
        }

        public SceneStatus Status { get; private set; }
    }
    public class OBSSourceEventArgs : EventArgs
    {
        public OBSSourceEventArgs (Source source)
        {
            Source = source;
        }

        public Source Source { get; private set; }
    }
}
