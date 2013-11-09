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
    public static class TwitchSmiles
    {

        public static TwitchSmile[] Smiles = {
                new TwitchSmile( "4Head",Properties.Resources._4Head ),
                new TwitchSmile( ">(", Properties.Resources.anger),
                new TwitchSmile( "ArsonNoSexy", Properties.Resources.ArsonNoSexy),
                new TwitchSmile( "AsianGlow", Properties.Resources.AsianGlow),
                new TwitchSmile( "BCWarrior", Properties.Resources.BCWarror),
                new TwitchSmile( "BibleThump", Properties.Resources.BibleThump),
                new TwitchSmile( "BionicBunion", Properties.Resources.BionicBunion),
                new TwitchSmile( "BlargNaut", Properties.Resources.BlargNaut),
                new TwitchSmile( "BloodTrail", Properties.Resources.BloodTrail),
                new TwitchSmile( "BORT", Properties.Resources.BORT),
                new TwitchSmile( "BrainSlug", Properties.Resources.BrainSlug),
                new TwitchSmile( "BrokeBack", Properties.Resources.BrokeBack),
                new TwitchSmile( "B)", Properties.Resources.cool),
                new TwitchSmile( "CougarHunt", Properties.Resources.CougarHunt),
                new TwitchSmile( "DansGame", Properties.Resources.DansGame),
                new TwitchSmile( "DatSheffy", Properties.Resources.DatSheffy),
                new TwitchSmile( "DBstyle", Properties.Resources.DBStyle),
                new TwitchSmile( "EagleEye",Properties.Resources.EagleEye),
                new TwitchSmile( "EvilFetus",Properties.Resources.EvilFetus),
                new TwitchSmile( "FailFish",Properties.Resources.FailFish),
                new TwitchSmile( "FPSMarksman",Properties.Resources.FPSMarksman),
                new TwitchSmile( "FrankerZ", Properties.Resources.FrankerZ),
                new TwitchSmile( "FreakinStinkin", Properties.Resources.FreakinStinkin),
                new TwitchSmile( ":(", Properties.Resources.frown),
                new TwitchSmile( "FUNgineer",Properties.Resources.FUNgineer),
                new TwitchSmile( "FuzzyOtterOO",Properties.Resources.FuzzyOtterOO),
                new TwitchSmile( "GingerPower",Properties.Resources.GingerPower),
                new TwitchSmile( "HassanChop",Properties.Resources.HassanChop),
                new TwitchSmile( "<3",Properties.Resources.heart),
                new TwitchSmile( "HotPokket",Properties.Resources.HotPokket),
                new TwitchSmile( "ItsBoshyTime",Properties.Resources.ItsBoshyTime),
                new TwitchSmile( "Jebaited",Properties.Resources.Jebaited),
                new TwitchSmile( "JKanStyle",Properties.Resources.JKanStyle),
                new TwitchSmile( "JonCarnage",Properties.Resources.JonCarnage),
                new TwitchSmile( "Kappa",Properties.Resources.Kappa),
                new TwitchSmile( "KevinTurtle", Properties.Resources.KevinTurtle),
                new TwitchSmile( "Kreygasm", Properties.Resources.Kreygasm),
                new TwitchSmile( "MrDestructoid", Properties.Resources.MrDestructoid),
                new TwitchSmile( "MVGame", Properties.Resources.MVGame),
                new TwitchSmile( "NinjaTroll", Properties.Resources.NinjaTroll),
                new TwitchSmile( "NoNoSpot",Properties.Resources.NoNoSpot),
                new TwitchSmile( "OMGScoots",Properties.Resources.OMGScoots),
                new TwitchSmile( "OneHand",Properties.Resources.OneHand),
                new TwitchSmile( "OpieOP",Properties.Resources.OpieOP),
                new TwitchSmile( "OptimizePrime",Properties.Resources.OptimizePrime),
                new TwitchSmile( "PazPazowitz", Properties.Resources.PazPazowitz),
                new TwitchSmile( "PicoMause", Properties.Resources.PicoMause),
                new TwitchSmile( "PJSalt", Properties.Resources.PJSalt),
                new TwitchSmile( "PMSTwin",Properties.Resources.PMSTwin),
                new TwitchSmile( "PogChamp", Properties.Resources.PogChamp),
                new TwitchSmile( "Poooound", Properties.Resources.Poooound),
                new TwitchSmile( "PunchTrees", Properties.Resources.PunchTrees),
                new TwitchSmile( "R)", Properties.Resources.R),
                new TwitchSmile( "RedCoat", Properties.Resources.RedCoat),
                new TwitchSmile( "ResidentSleeper", Properties.Resources.ResidentSleeper),
                new TwitchSmile( "RuleFive", Properties.Resources.RuleFive),
                new TwitchSmile( "ShazBotstix", Properties.Resources.ShazBotstix),
                new TwitchSmile( ":\\", Properties.Resources.slant),
                new TwitchSmile( ":/", Properties.Resources.slant),
                new TwitchSmile( ":)", Properties.Resources.smile),
                new TwitchSmile( "SMOrc", Properties.Resources.SMOrc),
                new TwitchSmile( "SMSkull", Properties.Resources.SMSkull),
                new TwitchSmile( "SoBayed", Properties.Resources.SoBayed),
                new TwitchSmile( "SoonerLater", Properties.Resources.SoonerLater),
                new TwitchSmile( "SSSsss", Properties.Resources.SSSsss),
                new TwitchSmile( "StoneLightning", Properties.Resources.StoneLightning),
                new TwitchSmile( "StrawBeary", Properties.Resources.StrawBeary),
                new TwitchSmile( "SuperVinlin", Properties.Resources.SuperVinlin),
                new TwitchSmile( "SwiftRage", Properties.Resources.SwiftRage),
                new TwitchSmile( "TehFunrun", Properties.Resources.TehFunrun),
                new TwitchSmile( "TheRinger", Properties.Resources.TheRinger),
                new TwitchSmile( "TheTarFu", Properties.Resources.TheTarFu),
                new TwitchSmile( "TinyFace", Properties.Resources.TinyFace),
                new TwitchSmile( "TooSpicy", Properties.Resources.TooSpicy),
                new TwitchSmile( "TriHard", Properties.Resources.TriHard),
                new TwitchSmile( "UleetBackup", Properties.Resources.UleetBackup),
                new TwitchSmile( "UnSane", Properties.Resources.UnSane),
                new TwitchSmile( ":D", Properties.Resources.vhappy),
                new TwitchSmile( "Volcania", Properties.Resources.Volcania),
                new TwitchSmile( ";)", Properties.Resources.wink),
                new TwitchSmile( "WinWaker", Properties.Resources.WinWaker),
                new TwitchSmile( "o_o", Properties.Resources.eyes),
                new TwitchSmile( ":z", Properties.Resources.bored),
                new TwitchSmile( ":o", Properties.Resources.surprised),
                new TwitchSmile( ":p", Properties.Resources.tongue),
                new TwitchSmile( ";p", Properties.Resources.tonguewink),
                new TwitchSmile( ":peka:", Properties.Resources.peka),
                new TwitchSmile( "peka", Properties.Resources.peka),
                new TwitchSmile( "пека", Properties.Resources.peka),
                new TwitchSmile( ":fp:", Properties.Resources.facepalm),
                new TwitchSmile( "facepalm", Properties.Resources.facepalm),
                new TwitchSmile( "фейспалм", Properties.Resources.facepalm)
               };
    }

    public class TwitchSmile
    {
        public Bitmap Smile
        {
            get;
            set;
        }
        public String Code
        {
            get;
            set;
        }
        public TwitchSmile(String code, Bitmap image)
        {
            Code = code;
            Smile = image;
        }

    }
}
