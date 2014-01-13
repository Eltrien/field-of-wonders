using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ubiquitous
{
    public class EventArgsString : EventArgs
    {
        public EventArgsString(String text)
        {
            Text = text;
        }

        public String Text
        {
            get;
            set;
        }
    }
    public class EventArgsKeyValue : EventArgs
    {
        public EventArgsKeyValue(List<KeyValuePair<String,String>> keyvalue)
        {
            Keyvalue = keyvalue;
        }
        public List<String> Values
        {
            get {
                return Keyvalue.Select(v => v.Value).ToList();
            }
        }
        public List<String> Keys
        {
            get
            {
                return Keyvalue.Select(k => k.Key).ToList();
            }
        }
        public List<KeyValuePair<String,String>> Keyvalue
        {
            get;
            set;
        }
    }
}
