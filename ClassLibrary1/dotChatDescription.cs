using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml.Serialization;

namespace dotInterfaces
{
    public interface IChatDescription 
    {
        [XmlAttribute]
        string Game
        {
            get;
            set;
        }
        [XmlAttribute]
        string ShortDescription
        {
            get;
            set;
        }
        [XmlAttribute]
        string LongDescription
        {
            get;
            set;
        }
        [XmlAttribute]
        string GameId
        {
            get;
            set;
        }

        [XmlIgnore]
        List<KeyValuePair<String, String>> GameList
        {
            get;
            set;
        }
        void GetDescription();
        void SetDescription();
    }
}
