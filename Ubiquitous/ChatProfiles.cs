using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml.Serialization;
using dotInterfaces;

namespace Ubiquitous
{
    

    public class ChatProfiles
    {
        [XmlElement]
        public List<ChatProfile> Profiles
        {
            get;
            set;
        }
    }
    public class ChatProfile
    {
        [XmlAttribute]
        public string ChatName
        {
            get;
            set;
        }
        [XmlAttribute]
        public string Language
        {
            get;
            set;
        }
        [XmlAttribute]
        public string Topic
        {
            get;
            set;
        }
        [XmlAttribute]
        public string Game
        {
            get;
            set;
        }
        [XmlAttribute]
        public string ShortDescription
        {
            get;
            set;
        }
        [XmlAttribute]
        public string LongDescription
        {
            get;
            set;
        }
    }


}
