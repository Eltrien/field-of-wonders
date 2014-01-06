using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ubiquitous
{
    public class ChannelList
    {
        public ChannelList()
        {
            channels = new List<Channel>();
        }
        public List<Channel> channels
        {
            get;
            set;
        }
    }
    public class Channel
    {
        public Channel(global::Ubiquitous.MainForm.EndPoint endpoint)
        {
            Title = endpoint.ToString();
            EndPoint = endpoint;
        }
        public String Title
        {
            get;
            set;
        }
        public global::Ubiquitous.MainForm.EndPoint EndPoint
        {
            get;
            set;
        }
    }
}
