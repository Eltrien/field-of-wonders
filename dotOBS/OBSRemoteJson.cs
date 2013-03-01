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
}

