using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dotWebClient;
using System.Threading;
using System.Diagnostics;
using dotUtilities;

namespace dotHashd
{
    public class Hashd
    {
        #region Constants
        private const string userAgent = "Mozilla/5.0 (Windows NT 6.0; WOW64; rv:14.0) Gecko/20100101 Firefox/14.0.1";
        private const string channelInfoUrl = "http://api.hashd.tv/v1/stream/{0}";
        private const int pollIntervalStats = 20000;

        #endregion
        #region Private properties
        private string lastTimestamp;
        private string chatUpdateNonce;
        private string chatNewMessageNonce;
        private Timer statsDownloader;
        private CookieAwareWebClient statsWC;
        private bool prevOnlineState = false;
        private Channel currentChannelStats;
        private string _user;
        private string _password;
        private object loginLock = new object();
        private object statsLock = new object();
        #endregion
        #region Events
        public event EventHandler<EventArgs> Live;
        public event EventHandler<EventArgs> Offline;
        public event EventHandler<EventArgs> OnLogin;
        private void DefaultEvent(EventHandler<EventArgs> evnt, EventArgs e)
        {
            EventHandler<EventArgs> handler = evnt;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        private void OnLive(EventArgs e)
        {
            DefaultEvent(Live, e);
        }
        private void OnOffline(EventArgs e)
        {
            DefaultEvent(Offline, e);
        }
        #endregion

        #region Public methods
        public Hashd(string user, string password)
        {
            _user = user;
            _password = password;
            statsWC = new CookieAwareWebClient();
            statsWC.Headers["User-Agent"] = userAgent;
            statsDownloader = new Timer(new TimerCallback(statsDownloader_Tick), null, Timeout.Infinite, Timeout.Infinite);

        }

        private void statsDownloader_Tick(object o)
        {
            DownloadStats(_user);
        }
        public bool isLoggedIn
        {
            get;
            set;
        }
        public void Start()
        {
            statsDownloader.Change(0, pollIntervalStats);
        }
        public void Stop()
        {
            statsDownloader.Change(Timeout.Infinite, Timeout.Infinite);
        }
        public bool Login()
        {
            lock (loginLock)
            {
                isLoggedIn = false;
                if (String.IsNullOrEmpty(_user) || String.IsNullOrEmpty(_password))
                    return false;

                if (OnLogin != null)
                    OnLogin(this, EventArgs.Empty);

                isLoggedIn = true;

                return true;
            }
        }

        private void DownloadStats(string channel)
        {
            if (String.IsNullOrEmpty(_user))
                return;
            lock (statsLock)
            {
                var prevChannelStats = currentChannelStats;
                try
                {
                    statsWC.Headers["Cache-Control"] = "no-cache";
                    var url = String.Format(channelInfoUrl, channel);
                    using (var stream = statsWC.downloadURL(url))
                    {
                        if (statsWC.LastWebError == "ProtocolError")
                        {
                            return;
                        }

                        if (stream == null)
                        {
                            Debug.Print("Cybergame: Can't download channel info of {0} result is null. Url: {1}", channel, channelInfoUrl);
                        }
                        else
                        {
                            currentChannelStats = JsonGenerics.ParseJson<Channel>.ReadObject(stream);
                        }

                        if (!Alive && prevChannelStats != currentChannelStats)
                            OnOffline(EventArgs.Empty);
                        else if (Alive && prevChannelStats != currentChannelStats)
                             OnLive(EventArgs.Empty);

                    }
                }
                catch
                {
                    Debug.Print("Hashd: Exception in Downloadstats");
                }
            }
        }
        private bool Alive
        {
            get
            {
                if (currentChannelStats == null)
                    return false;

                return currentChannelStats.live;
            }
        }
        #endregion
        #region Public properties
        public string Viewers
        {
            get { return Alive ? String.Format("{0}",currentChannelStats.currentViewers) : "0"; }
            set { }
        }
        #endregion

    }

}
