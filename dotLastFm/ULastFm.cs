using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Threading;
using dotLastfm.Services;
using System.Diagnostics;

namespace dotLastfm
{
    public class ULastFm
    {
        public event EventHandler<EventArgs> OnLogin;
        public event EventHandler<EventArgs> OnLoginFailed;
        public event EventHandler<LastFmArgs> OnTrackChange;

        private Timer pollTimer;
        private const string API_KEY = "601555201ca3988d08079bc5a7a23a59";
        private const string API_SECRET = "6c1fc90676d9845bd473750fc6c7503c";
        private const int POLL_INTERVAL = 2000;
        private Session _session;
        private User _lfmUser;
        private object pollLock = new object();
        private Track _currentTrack;
        
        public ULastFm()
        {
            pollTimer = new Timer(new TimerCallback(pollTimer_Tick), null, Timeout.Infinite, Timeout.Infinite);

        }
        private void pollTimer_Tick(object o)
        {
            lock (pollLock)
            {
                pollTimer.Change(Timeout.Infinite, Timeout.Infinite);
                Poll();
                pollTimer.Change(POLL_INTERVAL, Timeout.Infinite);
            }
            
        }
        public void Poll()
        {
            if (!_session.Authenticated)
                return;

            if (_session != null && _session.Authenticated && _lfmUser != null)
            {

                var track = _lfmUser.GetNowPlaying();
                if (track != null)
                {
                    
                    if (_currentTrack == null || (_currentTrack.GetID() != track.GetID()))
                    {
                        var album = track.GetAlbum();
                        var artist = track.Artist;

                        if (OnTrackChange != null)
                            OnTrackChange(this, new LastFmArgs() { Album = (album == null ? "" : album.Title), Artist = (artist == null ? "" : artist.Name), ArtistImage = null, Title = track.Title });
                    }
                    _currentTrack = track;
                }

            }

        }
        public bool Authenticate(string user, string password)
        {
            try
            {
                _session = new Session(API_KEY, API_SECRET);

                string md5password = Utilities.MD5(password);

                _session.Authenticate(user, md5password);
                if (_session.Authenticated)
                {
                    if (OnLogin != null)
                        OnLogin(this, EventArgs.Empty);
                    _lfmUser = new User(user, _session);

                    pollTimer.Change(0, Timeout.Infinite);
                    return true;
                }
                else
                {
                    if (OnLoginFailed != null)
                        OnLoginFailed(this, EventArgs.Empty);
                    return false;
                }
            }
            catch {
                Debug.Print("Last.FM authentication error");
                return false;
            }
        }
        
         
    }

    public class LastFmArgs : EventArgs
    {
        public String Artist
        {
            get;
            set;
        }
        public String Title
        {
            get;
            set;
        }
        public String Album
        {
            get;
            set;
        }
        public Bitmap ArtistImage
        {
            get;
            set;
        }
    }
}
