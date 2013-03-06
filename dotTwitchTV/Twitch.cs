using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Net;
using System.Diagnostics;
using System.Threading;
using dotWebClient;
using dotUtilities;
namespace dotTwitchTV
{

    public class Twitch
    {
        #region Constants
        private const string userAgent = "Mozilla/5.0 (Windows NT 6.0; WOW64; rv:14.0) Gecko/20100101 Firefox/14.0.1";
        private const string channelJsonUrl = "http://api.justin.tv/api/stream/list.json?channel={0}&t={1}";
        private const int pollInterval = 20000;
        #endregion

        
        #region Private properties
        private Timer statsDownloader;
        private CookieAwareWebClient statsWC;
        private object downloadLock = new object();
        private Channel currentChannelStats;
        private string currentChannelName;
        #endregion
        #region Events
        public event EventHandler<EventArgs> Live;
        public event EventHandler<EventArgs> Offline;
        private void DefaultEvent(EventHandler<EventArgs> evnt, EventArgs e)
        {
            EventHandler<EventArgs> handler = evnt;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        private void OnLive( EventArgs e)
        {
            DefaultEvent(Live, e);
        }
        private void OnOffline(EventArgs e)
        {
            DefaultEvent(Offline, e);
        }
        #endregion

        #region Public methods
        public Twitch(string channelName)
        {
            currentChannelName = channelName;

            statsWC = new CookieAwareWebClient();
            statsWC.Headers["User-Agent"] = userAgent;
            statsDownloader = new Timer(new TimerCallback(statsDownloader_Tick), null, Timeout.Infinite, Timeout.Infinite);
        }
        public void Start()
        {
            statsDownloader.Change(0, pollInterval);
        }
        public void Stop()
        {
            statsDownloader.Change(Timeout.Infinite, Timeout.Infinite);
        }
        private void statsDownloader_Tick(object o)
        {
            if (String.IsNullOrEmpty(currentChannelName))
            {
                Debug.Print("TwitchTV: channel name is empty. Can't fetch stats");
                return;
            }
            CrawlTwitchChannel(currentChannelName);
        }
        private void CrawlTwitchChannel( string channel )
        {
            lock (downloadLock)
            {
                var prevChannelStats = currentChannelStats;
                try
                {
                    statsWC.Headers["Cache-Control"] = "no-cache";
                    var url = String.Format(channelJsonUrl, channel, TimeUtils.UnixTimestamp());
                    using (var stream = statsWC.downloadURL(url))
                    {                        
                        if (statsWC.LastWebError == "ProtocolError")
                        {
                            return;
                        }

                        if (stream == null)
                        {
                            Debug.Print("TwitchTV: Can't download channel info of {0} result is null. Url: {1}", currentChannelName, channelJsonUrl);
                        }
                        else
                        {
                            currentChannelStats = ParseJson<List<Channel>>.ReadObject(stream).FirstOrDefault();

                            if (prevChannelStats != currentChannelStats && currentChannelStats != null)
                            {
                                OnLive(EventArgs.Empty);
                            }
                        }

                        if (!isAlive() && prevChannelStats != currentChannelStats)
                            OnOffline(EventArgs.Empty);
                    }
                }
                catch
                {
                    Debug.Print("Exception in CrawlTwitchChannel");
                }
            }
        }
        private bool isAlive()
        {
            return currentChannelStats != null;
        }
        #endregion
        #region Public properties
        public string Viewers
        {
            get { return !isAlive() ? "0" : currentChannelStats.viewers; }
            set { }
        }
        public string Bitrate
        {
            get { return !isAlive() ? "0" : currentChannelStats.videoBitrate; }
            set { }
        }
        #endregion

    }
}
