using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
namespace Ubiquitous
{
    public class LanguageList
    {
        public String CurrentLanguage
        {
            get;
            set;
        }
    }

    public class Languages
    {

        [XmlElement]
        public List<String> Name
        {
            get;
            set;
        }
    }
}
