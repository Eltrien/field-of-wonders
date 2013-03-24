using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Net;
using System.Web;
using System.IO;

namespace dotHashd
{
    #region "Hashd.tv json classes"

    [DataContract]
    public class Channel
    {
        private string _name, _nameSeo, _title, _descriptionLong, _descriptionShort;
        [DataMember(Name = "id")]
        public string id;
        [DataMember(Name = "name")]
        public string name
        {
            get { return _name; }
            set { _name = HttpUtility.HtmlDecode(value); }
        }
        [DataMember(Name = "name_seo")]
        public string nameSeo
        {
            get { return _nameSeo; }
            set { _nameSeo = HttpUtility.HtmlDecode(value); }
        }
        [DataMember(Name = "title")]
        public string title
        {
            get { return _title; }
            set { _title = HttpUtility.HtmlDecode(value); }
        }

        [DataMember(Name = "live")]
        public bool live;
        [DataMember(Name = "current_viewers")]
        public uint currentViewers;
        [DataMember(Name = "unique_viewers")]
        public uint uniqueViewers;
        [DataMember(Name = "total_views")]
        public string totalViews;
        [DataMember(Name = "game_id")]
        public string gameId;
        [DataMember(Name = "description_short")]
        public string descriptionShort
        {
            get { return _descriptionShort; }
            set { _descriptionShort = HttpUtility.HtmlDecode(value); }
        }

        [DataMember(Name = "description_long")]
        public string descriptionLong
        {
            get { return _descriptionLong; }
            set { _descriptionLong = HttpUtility.HtmlDecode(value); }
        }

        [DataMember(Name = "current_video_width")]
        public uint videoWidth;
        [DataMember(Name = "current_video_height")]
        public uint videoHeight;
        [DataMember(Name = "thumbnails")]
        public List<object> thumbnails
        {
            get;
            set;
        }
        [DataMember(Name = "teaser")]
        public List<object> teaser
        {
            get;
            set;
        }
    }

    #endregion
}
