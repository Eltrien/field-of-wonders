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
using System.Drawing;

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
        private object statsLock = new object();
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
            DownloadStats(currentChannelName);
        }
        private void DownloadStats( string channel )
        {
            lock (statsLock)
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

                        }

                        if (!isAlive() && prevChannelStats != currentChannelStats)
                            OnOffline(EventArgs.Empty);
                        else if (isAlive() && prevChannelStats != currentChannelStats)
                            OnLive(EventArgs.Empty);
                    }
                }
                catch
                {
                    Debug.Print("Twitch: Exception in DownloadStats");
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

    public class TwitchSmile
    {
        public static Bitmap Smile(String code)
        {
            switch (code)
            {
                case "4Head":
                    return Properties.Resources._4Head;
                case ">(":
                    return Properties.Resources.anger;
                case "ArsonNoSexy":
                    return Properties.Resources.ArsonNoSexy;
                case "AsianGlow":
                    return Properties.Resources.AsianGlow;
                case "BCWarrior":
                    return Properties.Resources.BCWarror;
                case "BibleThump":
                    return Properties.Resources.BibleThump;
                case "BionicBunion":
                    return Properties.Resources.BionicBunion;
                case "BlargNaut":
                    return Properties.Resources.BlargNaut;
                case "BloodTrail":
                    return Properties.Resources.BloodTrail;
                case "BORT":
                    return Properties.Resources.BORT;
                case "BrainSlug":
                    return Properties.Resources.BrainSlug;
                case "BrokeBack":
                    return Properties.Resources.BrokeBack;
                case "B)":
                    return Properties.Resources.cool;
                case "CougarHunt":
                    return Properties.Resources.CougarHunt;
                case "DansGame":
                    return Properties.Resources.DansGame;
                case "DatSheffy":
                    return Properties.Resources.DatSheffy;
                case "DBstyle":
                    return Properties.Resources.DBStyle;
                case "EagleEye":
                    return Properties.Resources.EagleEye;
                case "EvilFetus":
                    return Properties.Resources.EvilFetus;
                case "FailFish":
                    return Properties.Resources.FailFish;
                case "FPSMarksman":
                    return Properties.Resources.FPSMarksman;
                case "FrankerZ":
                    return Properties.Resources.FrankerZ;
                case "FreakinStinkin":
                    return Properties.Resources.FreakinStinkin;
                case ":(":
                    return Properties.Resources.frown;
                case "FUNgineer":
                    return Properties.Resources.FUNgineer;
                case "FuzzyOtterOO":
                    return Properties.Resources.FuzzyOtterOO;
                case "GingerPower":
                    return Properties.Resources.GingerPower;
                case "HassanChop":
                    return Properties.Resources.HassanChop;
                case "<3":
                    return Properties.Resources.heart;
                case "HotPokket":
                    return Properties.Resources.HotPokket;
                case "ItsBoshyTime":
                    return Properties.Resources.ItsBoshyTime;
                case "Jebaited":
                    return Properties.Resources.Jebaited;
                case "JKanStyle":
                    return Properties.Resources.JKanStyle;
                case "JonCarnage":
                    return Properties.Resources.JonCarnage;
                case "Kappa":
                    return Properties.Resources.Kappa;
                case "KevinTurtle":
                    return Properties.Resources.KevinTurtle;
                case "Kreygasm":
                    return Properties.Resources.Kreygasm;
                case "MrDestructoid":
                    return Properties.Resources.MrDestructoid;
                case "MVGame":
                    return Properties.Resources.MVGame;
                case "NinjaTroll":
                    return Properties.Resources.NinjaTroll;
                case "NoNoSpot":
                    return Properties.Resources.NoNoSpot;
                case "OMGScoots":
                    return Properties.Resources.OMGScoots;
                case "OneHand":
                    return Properties.Resources.OneHand;
                case "OpieOP":
                    return Properties.Resources.OpieOP;
                case "OptimizePrime":
                    return Properties.Resources.OptimizePrime;
                case "PazPazowitz":
                    return Properties.Resources.PazPazowitz;
                case "PicoMause":
                    return Properties.Resources.PicoMause;
                case "PJSalt":
                    return Properties.Resources.PJSalt;
                case "PMSTwin":
                    return Properties.Resources.PMSTwin;
                case "PogChamp":
                    return Properties.Resources.PogChamp;
                case "Poooound":
                    return Properties.Resources.Poooound;
                case "PunchTrees":
                    return Properties.Resources.PunchTrees;
                case "R)":
                    return Properties.Resources.R;
                case "RedCoat":
                    return Properties.Resources.RedCoat;
                case "ResidentSleeper":
                    return Properties.Resources.ResidentSleeper;
                case "RuleFive":
                    return Properties.Resources.RuleFive;
                case "ShazBotstix":
                    return Properties.Resources.ShazBotstix;
                case ":\\":
                    return Properties.Resources.slant;
                case ":/":
                    return Properties.Resources.slant;
                case ":)":
                    return Properties.Resources.smile;
                case "SMOrc":
                    return Properties.Resources.SMOrc;
                case "SMSkull":
                    return Properties.Resources.SMSkull;
                case "SoBayed":
                    return Properties.Resources.SoBayed;
                case "SoonerLater":
                    return Properties.Resources.SoonerLater;
                case "SSSsss":
                    return Properties.Resources.SSSsss;
                case "StoneLightning":
                    return Properties.Resources.StoneLightning;
                case "StrawBeary":
                    return Properties.Resources.StrawBeary;
                case "SuperVinlin":
                    return Properties.Resources.SuperVinlin;
                case "SwiftRage":
                    return Properties.Resources.SwiftRage;
                case "TehFunrun":
                    return Properties.Resources.TehFunrun;
                case "TheRinger":
                    return Properties.Resources.TheRinger;
                case "TheTarFu":
                    return Properties.Resources.TheTarFu;
                case "TinyFace":
                    return Properties.Resources.TinyFace;
                case "TooSpicy":
                    return Properties.Resources.TooSpicy;
                case "TriHard":
                    return Properties.Resources.TriHard;
                case "UleetBackup":
                    return Properties.Resources.UleetBackup;
                case "UnSane":
                    return Properties.Resources.UnSane;
                case ":D":
                    return Properties.Resources.vhappy;
                case "Volcania":
                    return Properties.Resources.Volcania;
                case ";)":
                    return Properties.Resources.wink;
                case "WinWaker":
                    return Properties.Resources.WinWaker;
            }

            switch (code.ToLower())
            {
                case "o_o":
                    return Properties.Resources.eyes;
                case ":z":
                    return Properties.Resources.bored;
                case ":o":
                    return Properties.Resources.surprised;
                case ":p":
                    return Properties.Resources.tongue;
                case ";p":
                    return Properties.Resources.tonguewink;
            }
            return null;
        }
    }
}
