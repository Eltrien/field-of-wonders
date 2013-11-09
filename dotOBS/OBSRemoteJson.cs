using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Net;
using System.Web;
using System.IO;

namespace dotOBS
{
    #region "OBSRemote json classes"

    [DataContract]
    public class StreamStatus
    {
        [DataMember(Name = "fps")]
        public uint fps = 0;
        [DataMember(Name = "preview-only")]
        public bool previewOnly = false;
        [DataMember(Name = "streaming")]
        public bool streaming = false;
        [DataMember(Name = "bytes-per-sec")]
        public uint bitrate = 0;
        [DataMember(Name = "num-total-frames")]
        public uint framesTotal = 0;
        [DataMember(Name = "num-dropped-frames")]
        public uint framesDropped = 0;
        [DataMember(Name = "strain",IsRequired = false)]
        public float strain = 0.0f;
    }
    #endregion

    [DataContract]
    public class SceneStatus
    {
        [DataMember(Name = "status")]
        public string status;
        [DataMember(Name = "message-id")]
        public string messageid;
        [DataMember(Name = "current-scene")]
        public string currentScene;
        [DataMember(Name = "scenes")]
        public List<Scene> scenes
        {
            get;
            set;
        }
    }
    [DataContract]
    public class Scene
    {
        [DataMember(Name = "name")]
        public string name;
        [DataMember(Name = "sources")]
        public List<Source> sources
        {
            get;
            set;
        }

    }

    [DataContract]
    public class Source
    {
        [DataMember(Name = "y")]
        public float y;
        [DataMember(Name = "cx")]
        public float cx;
        [DataMember(Name = "render")]
        public bool render;
        [DataMember(Name = "name")]
        public string name;
        [DataMember(Name = "X")]
        public float x;
        [DataMember(Name = "cy")]
        public float cy;
    }

    [DataContract]
    public class SwitchScenes
    {
        [DataMember(Name = "update-type")]
        public string updateType;
        [DataMember(Name = "scene-name")]
        public string sceneName;
    }

    [DataContract]
    public class SourceChange
    {
        [DataMember(Name = "update-type")]
        public string updateType;
        [DataMember(Name = "source")]
        public Source source;
        [DataMember(Name = "source-name")]
        public string sourceName;

    }

}

