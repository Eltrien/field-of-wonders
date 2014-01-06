using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Net;
using System.Web;
using System.IO;

namespace dotCybergame
{
    #region "Cybergame json classes"

    [DataContract]
    public class Channel
    {
        [DataMember(Name = "online")]
        public string online;
        [DataMember(Name = "spectators")]
        public string viewers;
        [DataMember(Name = "donates")]
        public object donates;
        [DataMember(Name = "followers")]
        public object followers;
    }

    [DataContract]
    public class Chat
    {
        private List<Message> _messages;
        [DataMember(Name = "quick_chat_messages", IsRequired = false)]
        public List<Message> messages
        {
            get
            {
                if (_messages == null)
                    return null;
                _messages.Sort((m1, m2) => m1.unixtimestamp.CompareTo(m2.unixtimestamp));
                return _messages;
            }
            set
            {
                _messages = value;
            }
        }
        [DataMember(Name = "quick_chat_no_participation")]
        public bool istheresomebody;
        [DataMember(Name = "quick_chat_success")]
        public bool success;
        [DataMember(Name = "quick_chat_update_messages_nonce")]
        public string updateid;
        public UInt32 MaxTimestamp()
        {
            return _messages.Max(m => m.unixtimestamp);
        }
        public System.Collections.IEnumerator GetEnumerator()
        {
            for (int i = 0; i < _messages.Count; i++)
            {
                yield return _messages[i];
            }
        }
    }
    [DataContract]
    public class Message
    {
        private string _room,_alias,_message;
        [DataMember(Name = "id")]
        public int messageid ;
        [DataMember(Name = "room")]
        public string room 
        {
            get{return _room;}
            set { _room = HttpUtility.HtmlDecode(value); }
        }
        [DataMember(Name = "timestamp")]
        public string timestamp;
        [DataMember(Name = "deleted")]
        public bool isdeleted;
        [DataMember(Name = "unix_timestamp")]
        public UInt32 unixtimestamp;
        [DataMember(Name = "alias")]
        public string alias
        {
            get { return _alias; }
            set { _alias = HttpUtility.HtmlDecode(value); }
        }
        [DataMember(Name = "md5email")]
        public string md5email;
        [DataMember(Name = "status")]
        public int status;
        [DataMember(Name = "message")]
        public string message
        {
            get { return _message; }
            set { _message = HttpUtility.HtmlDecode(value); }
        }
        [DataMember(Name = "timestring")]
        public string timestring ;
    }


    #endregion
}
