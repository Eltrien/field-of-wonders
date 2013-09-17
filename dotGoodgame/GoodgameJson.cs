using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace dotGoodgame
{
    [DataContract]
    public class DataList
    {
        [DataMember(Name = "a")]
        public List<String> list 
        {
            get;set;
        }
    }
    [DataContract]
    public class Data
    {
        [DataMember(Name = "type")]
        public string type;
        [DataMember(Name = "data")]
        public object data;
    }

    //a["{\"type\":\"channel_counters\",\"data\":{\"channel_id\":5403,\"clients_in_channel\":\"615\",\"users_in_channel\":196}}"]
    [DataContract]
    public class Counters
    {
        [DataMember(Name = "channel_id")]
        public int channelId;
        [DataMember(Name = "clients_in_channel")]
        public int viewersCount;
        [DataMember(Name = "users_in_channel")]
        public int usersCount;        
    }
}
