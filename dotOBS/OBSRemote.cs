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
        public void SetSourceRendererPart(String sourceName, bool enable)
        {
            if( SceneStatus != null )
            {
                var curSceneName = SceneStatus.currentScene;
                if( !String.IsNullOrEmpty(curSceneName) )
                {
                    Scene curScene = Scenes.FirstOrDefault( scene => scene.name == curSceneName );
                    Source switchSource = curScene.sources.FirstOrDefault(source => source.name.ToLower().StartsWith(sourceName.ToLower()));
                    if( switchSource != null && !String.IsNullOrEmpty(switchSource.name))
                    {
                        SetSourceRenderer(switchSource.name, enable);
                    }
                }
            }
        }
        public SceneStatus SceneStatus
        {
            get;set;
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

        public List<Scene> Scenes
        {
            get;
            set;
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
                    SceneStatus = sceneStatus;
                    if (sceneStatus.scenes.Count > 0)
                        Scenes = sceneStatus.scenes;
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



/* Authentication routines

bool Config::checkChallengeAuth(const char *response, const char *challenge)
{
    size_t challengeLength = strlen(challenge);
    size_t authHashLength = this->authHash.length();
    size_t authPlusChallengeSize = authHashLength + challengeLength;
    char* authPlusChallenge = (char*)malloc(authPlusChallengeSize);

    //concat authHash and challenge string
    memcpy(authPlusChallenge, this->authHash.c_str(), authHashLength);
    memcpy(authPlusChallenge + authHashLength, challenge, challengeLength);

    unsigned char respHash[32];
    unsigned char respHash64[64];
    size_t respHash64Size = 64;

    //hash concatenated authHash and string and base 64 encode
    sha2((unsigned char *)authPlusChallenge, authPlusChallengeSize, respHash, 0);
    base64_encode(respHash64, &respHash64Size, respHash, 32);
    respHash64[respHash64Size] = 0;

    free(authPlusChallenge);

    //compare against response
    return strcmp((char*)respHash64, response) == 0;
}
      public static string CreateSHAHash(string Phrase)
    {
        SHA512Managed HashTool = new SHA512Managed();
        Byte[] PhraseAsByte = System.Text.Encoding.UTF8.GetBytes(string.Concat(Phrase));
        Byte[] EncryptedBytes = HashTool.ComputeHash(PhraseAsByte);
        HashTool.Clear();
        return Convert.ToBase64String(EncryptedBytes);
    }

*/
