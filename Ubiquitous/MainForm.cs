using WMPLib;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Deployment;
using System.Reflection;
using System.ComponentModel;
using System.Globalization;
using System.Diagnostics;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using dotSteam;
using dotIRC;
using dotGoodgame;
using dotSC2TV;
using dotSkype;
using dotTwitchTV;
using dotXSplit;
using dotBattlelog;
using System.Text.RegularExpressions;
using dotGohaTV;
using dotEmpireTV;
using dotCybergame;
using System.Configuration;
using dotOBS;
using dotUtilities;

namespace Ubiquitous
{
    public partial class MainForm : Form
    {
        delegate void SetTransparencyCB(Color color);
        delegate void SetVisibilityCB(Control control, bool state);
        delegate void SetTopMostCB(Form control, bool topmost);
        delegate void SetComboValueCB(ComboBox combo, object value);
        delegate void SetTooltipCB(ToolTip tooltip, Control control, string value);
        delegate void SetCheckedToolTipCB(ToolStripMenuItem item, bool state);
        #region Private classes and enums
        private class LogStreamWriter : System.IO.TextWriter
        {
            Log _output = null;
            String[] _maskPasswords;
            public LogStreamWriter(Log _log, String[] maskPasswords)
            {
                _output = _log;
                _maskPasswords = maskPasswords;
            }
            public override void WriteLine(string value)
            {
                if (_maskPasswords != null && !String.IsNullOrEmpty(value))
                {
                    foreach (var pass in _maskPasswords)
                    {
                        if( !String.IsNullOrEmpty(pass ))
                        {
                            value = value.Replace(pass, new String('*', pass.Length));
                        }
                    }
                }

                base.WriteLine(value);
                _output.WriteLine(value.ToString());
            }
            public override Encoding Encoding
            {
                get { return System.Text.Encoding.UTF8; }
            }
        }
        private enum EndPoint
        {
            Sc2Tv,
            TwitchTV,
            Steam,
            SteamAdmin,
            Skype,
            Console,
            SkypeGroup,
            Bot,
            Goodgame,
            Battlelog,
            Gohatv,
            Empiretv,
            Cybergame,
            All
        }
        private class ChatUser
        {
            public ChatUser(string fullName, string nickName, EndPoint endPoint)
            {
                FullName = fullName;
                NickName = nickName;
                EndPoint = endPoint;
            }
            public String FullName
            {
                get;
                set;
            }
            public String NickName
            {
                get;
                set;
            }
            public EndPoint EndPoint
            {
                get;
                set;
            }
        }
        private class ChatAlias
        {
            public ChatAlias(string alias, EndPoint endpoint, ChatIcon icon = ChatIcon.Default)
            {
                Alias = alias;
                Endpoint = endpoint;
                Icon = icon;
            }
            public ChatIcon Icon
            {
                get;
                set;
            }
            public EndPoint Endpoint
            {
                get;
                set;
            }
            public string Alias
            {
                get;
                set;
            }

        }
        private class AdminCommand
        {
            private enum CommandType
            {
                BoolCmd,
                PartnerCmd,
                EmptyParam,
                ReplyCmd,
            }
            private string partnerHandle;
            private string _re;
            private bool _flag;
            private Func<string, Result> _action;
            private Func<bool, Result> _action2;
            private Func<Result> _action3;
            private Func<string, Message, Result> _action4;
            private CommandType _type;
            private Message _message;
            private string _switchto;

            public AdminCommand(string re, Func<Result> action)
            {
                _re = re;
                _action3 = action;
                _type = CommandType.EmptyParam;
            }
            public AdminCommand(string re, Func<string, Result> action)
            {
                _re = re;
                _action = action;
                _type = CommandType.PartnerCmd;
            }
            public AdminCommand(string re, Func<string, Message, Result> action)
            {
                _re = re;
                _action4 = action;                
                _type = CommandType.ReplyCmd;
                _message = new Message("", EndPoint.SteamAdmin);
                _switchto = "";
            }
            public AdminCommand(string re, Func<bool, Result> action, bool flag)
            {
                _re = re;
                _action2 = action;
                _flag = flag;
                _type = CommandType.BoolCmd;
            }
            public Result Execute()
            {
                Result result = Result.Failed;
                switch (_type)
                {
                    case CommandType.BoolCmd:
                        result = _action2(_flag);
                        break;
                    case CommandType.PartnerCmd:
                        if( partnerHandle != null )
                            result = _action(partnerHandle);
                        break;
                    case CommandType.ReplyCmd:
                        if (_message != null )
                        {
                            result = _action4( _switchto, _message );
                        }
                        break;
                    case CommandType.EmptyParam:
                        result = _action3();
                        break;
                }
                return result;
            }

            public bool isCommand(string command)
            {
                if (Regex.IsMatch(command, _re,RegexOptions.IgnoreCase))
                {
                    Match reCommand = Regex.Match(command, _re, RegexOptions.IgnoreCase);
                    switch (_type)
                    {
                        case CommandType.BoolCmd:
                            break;
                        case CommandType.PartnerCmd:
                            if (reCommand.Groups.Count > 0)
                                partnerHandle = reCommand.Groups[1].Value;
                            break;
                        case CommandType.ReplyCmd:
                            if (reCommand.Groups.Count >= 3)
                            {
                                _switchto = reCommand.Groups[1].Value;
                                _message = new Message(reCommand.Groups[2].Value, EndPoint.SteamAdmin);
                            }
                            break;
                        case CommandType.EmptyParam:
                            break;
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        private class Message
        {
            private string _text, _from, _to;
            private EndPoint _fromEndpoint, _toEndpoint;

            public Message(string text, EndPoint fromendpoint)
            {
                _text = text;
                _fromEndpoint = fromendpoint;
                _toEndpoint = EndPoint.Console;
                _from = null;
            }
            public Message(string text, EndPoint fromendpoint, EndPoint toendpoint)
            {
                _text = text;
                _fromEndpoint = fromendpoint;
                _toEndpoint = toendpoint;
                _from = null;
            }

            public Message(string text, string fromName, EndPoint fromEndPoint)
            {
                _text = text;
                _from = fromName;
                _fromEndpoint = fromEndPoint;
                _toEndpoint = EndPoint.Console;
            }
            public string Text
            {
                get { return _text; }
                set { _text = value; }
            }
            public string FromName
            {
                get { return _from; }
                set { _from = value; }
            }
            public EndPoint FromEndPoint
            {
                get { return _fromEndpoint; }
                set { _fromEndpoint = value; }
            }
            public string ToName
            {
                get { return _to; }
                set { _to = value; }
            }
            public EndPoint ToEndPoint
            {
                get { return _toEndpoint; }
                set { _toEndpoint = value; }
            }
            public ChatIcon Icon
            {
                get
                {
                    switch (_fromEndpoint)
                    {
                        case EndPoint.Sc2Tv:
                            return ChatIcon.Sc2Tv;
                        case EndPoint.TwitchTV:
                            return ChatIcon.TwitchTv;
                        case EndPoint.Skype:
                            return ChatIcon.Skype;
                        case EndPoint.SkypeGroup:
                            return ChatIcon.Skype;
                        case EndPoint.Steam:
                            return ChatIcon.Steam;
                        case EndPoint.SteamAdmin:
                            return ChatIcon.Admin;
                        case EndPoint.Console:
                            return ChatIcon.Admin;
                        case EndPoint.Bot:
                            return ChatIcon.Admin;
                        case EndPoint.Goodgame:
                            return ChatIcon.Goodgame;
                        case EndPoint.Battlelog:
                            return ChatIcon.Battlelog;
                        case EndPoint.Empiretv:
                            return ChatIcon.Empire;
                        case EndPoint.Gohatv:
                            return ChatIcon.Goha;
                        case EndPoint.Cybergame:
                            return ChatIcon.Cybergame;
                        default:
                            return ChatIcon.Default;
                    }
                }
            }

        }
        #endregion 

        #region Private properties
        private string formTitle;
        private Point cursorPosBeforeMouseDown;
        private bool isLMBDown = false;
        private Properties.Settings settings;
        private const string twitchIRCDomain = "jtvirc.com";
        private const string gohaIRCDomain = "i.gohanet.ru";
        private Log log;
        private SteamAPISession.User steamAdmin;
        private List<SteamAPISession.Update> updateList;
        private SteamAPISession steamBot;
        private SteamAPISession.LoginStatus status;
        private StatusImage checkMark;
        private StatusImage streamStatus;
        private Sc2Chat sc2tv;
        private IrcClient twitchIrc;
        private IrcClient gohaIrc;
        private SkypeChat skype;
        private List<AdminCommand> adminCommands;
        private List<ChatAlias> chatAliases;
        private Message lastMessageSent;
        private List<Message> lastMessagePerEndpoint;
        private BindingSource channelsSC2;
        private BindingSource channelsGG;
        private uint sc2ChannelId = 0;
        private bool streamIsOnline = false;
        private BGWorker gohaBW, gohaStreamBW, steamBW, sc2BW, twitchBW, skypeBW, twitchTV, goodgameBW, battlelogBW,
                        empireBW, cyberBW, obsremoteBW;
        private EmpireTV empireTV;
        private GohaTV gohaTVstream;
        private Twitch twitchChannel;
        private EndPoint currentChat;
        private Goodgame ggChat;
        private OBSRemote obsRemote;
        private XSplit xsplit;
        private StatusServer statusServer;
        private Battlelog battlelog;
        private Cybergame cybergame;
        private List<ChatUser> chatUsers;
        private bool isDisconnecting = false;
        private ToolTip viewersTooltip;
        private FontDialog fontDialog;
        #endregion 

        #region Form events and methods
        public MainForm()
        {

            //UnprotectConfig();
            settings = Properties.Settings.Default;

            InitializeComponent();

            RefreshChatProperties();
            log = new Log(textMessages);
            var maskPasswords = new String[]{
                settings.battlelogPassword,
                settings.cyberPassword,
                settings.empirePassword,
                settings.GohaPassword,
                settings.goodgamePassword,
                settings.Sc2tvPassword,
                settings.SteamBotPassword,
                settings.TwitchPassword
            };
            if (settings.globalDebug)
            {
               Debug.Listeners.Add(new TextWriterTraceListener(new LogStreamWriter(log, maskPasswords)));
            }

            setTopMost();
            chatUsers = new List<ChatUser>();

            currentChat = EndPoint.TwitchTV;
            lastMessageSent = new Message("", EndPoint.Console);
            adminCommands = new List<AdminCommand>();
            chatAliases = new List<ChatAlias>();
            lastMessagePerEndpoint = new List<Message>();
            adminCommands.Add(new AdminCommand(@"^/r\s*([^\s]*)\s*(.*)", ReplyCommand));
            adminCommands.Add(new AdminCommand(@"^/stream$", StartStopStreamsCommand));
            adminCommands.Add(new AdminCommand(@"^/gohaconfirm\s*([^\s]*)\s*(.*)", GohaConfirmCommand));
            adminCommands.Add(new AdminCommand(@"^/gohasetpass\s*([^\s]*)\s*(.*)", GohaUpdatePassword));

            chatAliases.Add(new ChatAlias(settings.twitchChatAlias, EndPoint.TwitchTV, ChatIcon.TwitchTv));
            chatAliases.Add(new ChatAlias(settings.sc2tvChatAlias, EndPoint.Sc2Tv, ChatIcon.Sc2Tv));
            chatAliases.Add(new ChatAlias(settings.steamChatAlias, EndPoint.Steam, ChatIcon.Steam));
            chatAliases.Add(new ChatAlias(settings.skypeChatAlias, EndPoint.Skype, ChatIcon.Skype));
            chatAliases.Add(new ChatAlias(settings.battlelogChatAlias, EndPoint.Battlelog, ChatIcon.Battlelog));
            chatAliases.Add(new ChatAlias(settings.gohaChatAlias, EndPoint.Gohatv, ChatIcon.Goha));
            chatAliases.Add(new ChatAlias(settings.empireAlias, EndPoint.Empiretv, ChatIcon.Empire));
            chatAliases.Add(new ChatAlias(settings.goodgameChatAlias, EndPoint.Goodgame, ChatIcon.Goodgame));
            chatAliases.Add(new ChatAlias(settings.cyberAlias, EndPoint.Cybergame, ChatIcon.Cybergame));
            chatAliases.Add(new ChatAlias("@all", EndPoint.All, ChatIcon.Default));


            uint.TryParse(settings.Sc2tvId, out sc2ChannelId);
            Debug.Print(String.Format("Sc2tv Channel ID: {0}",sc2ChannelId));

            sc2tv = new Sc2Chat(settings.sc2LoadHistory);
            sc2tv.Logon += OnSc2TvLogin;
            sc2tv.ChannelList += OnSc2TvChannelList;
            sc2tv.MessageReceived += OnSc2TvMessageReceived;
            sc2tv.channelList = new Channels();           

            gohaIrc = new IrcClient();
            gohaIrc.Connected += OnGohaConnect;
            gohaIrc.Registered += OnGohaRegister;
            gohaIrc.Disconnected += OnGohaDisconnect;


            checkMark = new StatusImage(Properties.Resources.checkMarkGreen, Properties.Resources.checkMarkRed);
            streamStatus = new StatusImage(Properties.Resources.streamOnline, Properties.Resources.streamOffline);


            statusServer = new StatusServer();
            battlelog = new Battlelog();

            steamBW = new BGWorker(ConnectSteamBot, null);
            sc2BW = new BGWorker(ConnectSc2tv, null);
            twitchBW = new BGWorker(ConnectTwitchIRC, null);
            gohaBW = new BGWorker(ConnectGohaIRC, null);
            twitchTV = new BGWorker(ConnectTwitchChannel, null);
            skypeBW = new BGWorker(ConnectSkype, null);
            cyberBW = new BGWorker(ConnectCybergame, null);
            obsremoteBW = new BGWorker(ConnectOBSRemote, null);
 
            goodgameBW = new BGWorker(ConnectGoodgame, null);
            battlelogBW = new BGWorker(ConnectBattlelog, null);

            if (settings.enableXSplitStats)
            {
                xsplit = new XSplit();
                xsplit.OnFrameDrops += OnXSplitFrameDrops;
                xsplit.OnStatusRefresh += OnXSplitStatusRefresh;
            }
            if (settings.enableStatusServer)
            {
                statusServer.Start();
            }

            gohaTVstream = new GohaTV();
            gohaStreamBW = new BGWorker(ConnectGohaStream, null);

            empireTV = new EmpireTV();
            empireBW = new BGWorker(ConnectEmpireTV, null);

            settings.PropertyChanged += new PropertyChangedEventHandler(settings_PropertyChanged);
            settings.SettingsSaving += new SettingsSavingEventHandler(settings_SettingsSaving);

            fontDialog = new FontDialog();

            //@Debug.Print("Config is here:" + ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath);
            #region Set tooltips
            ToolTip fullScreenDblClk = new ToolTip();

            fullScreenDblClk.AutoPopDelay = 2000;
            fullScreenDblClk.InitialDelay = 100;
            fullScreenDblClk.ReshowDelay = 100;
            fullScreenDblClk.ShowAlways = false;

            // Set up the ToolTip text for the Button and Checkbox.
            fullScreenDblClk.SetToolTip(textMessages, "Double click to switch Full screen mode");

            viewersTooltip = new ToolTip();

            viewersTooltip.AutoPopDelay = 2000;
            viewersTooltip.InitialDelay = 0;
            viewersTooltip.ReshowDelay = 0;
            viewersTooltip.ShowAlways = false;

            viewersTooltip.SetToolTip(labelViewers, String.Format("Twitch.tv: {0}, Cybergame.tv: {0}", 0, 0));
            // Set up the ToolTip text for the Button and Checkbox.
            fullScreenDblClk.SetToolTip(textMessages, "Double click to switch Full screen mode");

            var tooltip = new ToolTip();
            tooltip.AutoPopDelay = 2000;
            tooltip.InitialDelay = 0;
            tooltip.ReshowDelay = 0;
            tooltip.ShowAlways = false;
            tooltip.SetToolTip(buttonStreamStartStop, "Click to start/stop streaming in OBS");
            
            #endregion


        }
        private void pictureSteamBot_Click(object sender, EventArgs e)
        {
            settings.steamEnabled = !settings.steamEnabled;
            settings.Save();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {

            RefreshChatProperties();

            formTitle = String.Format("{0} {1}", this.Text, GetRunningVersion());
            this.Text = formTitle;
            contextMenuChat.Items.Clear();
            foreach (ChatAlias chatAlias in chatAliases)
            {
                contextMenuChat.Items.Add(String.Format("{0} ({1})",chatAlias.Endpoint.ToString(), chatAlias.Alias),log.GetChatBitmap(chatAlias.Icon));
            }
            if (settings.isFullscreenMode)
            {
                switchFullScreenMode();
                Size = settings.globalCompactSize;
            }
            else
            {
                Size = settings.globalFullSize;
            }
            SwitchBorder();
            Size = new System.Drawing.Size(settings.mainformWidth, settings.mainformHeight);


            SetTopMost(this, settings.globalOnTop);
            StartPosition = settings.mainformStartPos;
            Location = settings.mainFormPosition;

        }
        private Version GetRunningVersion()
        {
                if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
                {
                    return System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion;
                }
                else
                {
                    return Assembly.GetExecutingAssembly().GetName().Version;
                }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Advertising
            if (settings.twitchEnabled && twitchIrc.IsRegistered)
            {
                SendMessageToTwitchIRC(new Message("/commercial", EndPoint.SteamAdmin, EndPoint.TwitchTV));
                SendMessage(new Message("TwitchTv: advertising started!", EndPoint.TwitchTV, EndPoint.SteamAdmin));
            }           

            if (settings.cyberEnabled && cybergame.isLoggedIn)
            {
                cybergame.StartAdvertising();
                SendMessage(new Message("Cybergame: advertising started!", EndPoint.Cybergame, EndPoint.SteamAdmin));
            }

        }

        void settings_SettingsSaving(object sender, CancelEventArgs e)
        {

        }
        void RefreshChatProperties()
        {
            String[] refreshProperties = {
                "globalChatFont",
                "globalToolBoxBack",
                "globalChatEnableTimestamps",
                "globalChatTextColor",
                "globalTimestampForeground"};

            foreach (var p in refreshProperties)
                SetChatProperties(p);


        }
        void SetChatProperties(string propertyName)
        {
            switch (propertyName)
            {
                case "globalChatFont":
                    {
                        textMessages.Font= settings.globalChatFont;
                    }
                    break;
                case "globalToolBoxBack":
                    {
                        textMessages.BackColor = settings.globalToolBoxBack;
                        panelTools.BackColor = settings.globalToolBoxBack;
                    }
                    break;
                case "globalChatTextColor":
                    {
                        textMessages.TextColor = settings.globalChatTextColor;
                    }
                    break;
                case "globalTimestampForeground":
                    {
                        textMessages.TimeColor = settings.globalTimestampForeground;
                    }
                    break;
                case "globalChatEnableTimestamps":
                    {
                        textMessages.TimeStamp = settings.globalChatEnableTimestamps;
                    }
                    break;
                default:
                    {
                        textMessages.TextColor = settings.globalChatTextColor;
                    }
                    break;
            }
        }
        void settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SetChatProperties(e.PropertyName);
        }


        private void buttonFullscreen_Click(object sender, EventArgs e)
        {
            switchFullScreenMode();

        }

        private void buttonSettings_Click_1(object sender, EventArgs e)
        {
            var lastOnTopState = this.TopMost;  
            SetTopMost(this, false);
            SettingsDialog settingsForm = new SettingsDialog();
            settingsForm.ShowDialog();
            ProtectConfig();
            SetTopMost(this, lastOnTopState);

        }
        private void comboSc2Channels_DropDown(object sender, EventArgs e)
        {
            if( settings.sc2tvEnabled )
                sc2tv.updateStreamList();
        }

        private void comboGGChannels_DropDown(object sender, EventArgs e)
        {
            if( settings.goodgameEnabled )
                ggChat.updateChannelList();
        }

        private void textCommand_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                var m = new Message(textCommand.Text, EndPoint.SteamAdmin, currentChat);
                SendMessage(m);
                textCommand.Text = "";
                e.Handled = true;
            }
        }
        private void comboGGChannels_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (settings.goodgameEnabled)
            {
                var selItem = (Goodgame.GGChannel)comboGGChannels.SelectedItem;
                SendMessage(new Message(String.Format("Switching Goodgame channel to: {0}", selItem.Title), EndPoint.Console, EndPoint.SteamAdmin));
                ggChat.ChatId = selItem.Id.ToString();
            }
        }
        private void textMessages_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            textMessages.LinkClick(e.LinkText);
        }
        private bool SwitchToChat(string alias)
        {
            if (!String.IsNullOrEmpty(alias))
            {
                var chatAlias = chatAliases.Where(ca => ca.Alias.Trim().ToLower() == alias.Trim().ToLower()).FirstOrDefault();
                if (chatAlias == null)
                {
                    var knownAliases = "";
                    chatAliases.ForEach(ca => knownAliases += ca.Alias += " ");
                    knownAliases = knownAliases.Trim();
                    SendMessage(new Message(
                        String.Format("\"{0}\" is unknown chat alias. Use one of: {1}", alias, knownAliases),
                        EndPoint.Bot, EndPoint.SteamAdmin)
                    );
                    return false;
                }
                else
                {
                    currentChat = chatAlias.Endpoint;
                    try
                    {
                        pictureCurrentChat.Image = log.GetChatBitmap(chatAlias.Icon);
                    }
                    catch { }

                    if (settings.steamCurrentChatNotify && settings.steamEnabled)
                    {
                        var msg = new Message(String.Format("Switching to {0}...", currentChat.ToString()), EndPoint.Bot, EndPoint.SteamAdmin);
                        if (!isFlood(msg))
                            SendMessage(msg);
                    }
                    else
                    {
                        var msg = new Message(String.Format("Switching to {0}...", currentChat.ToString()), chatAlias.Endpoint, EndPoint.Console);                        
                        if (!isFlood(msg))
                            SendMessage(msg);
                    }
                    return true;
                }
            }
            return false;
        }
        private Result StartStopStreamsCommand()
        {
            StartStopOBSStream();
            return Result.Successful;
        }
        private Result GohaConfirmCommand(string confirmCode)
        {
            SendConfirmCodeToGohaIRC(confirmCode);
            return Result.Successful;
        }
        private Result GohaUpdatePassword()
        {
            SendUpdatePassToGohaIRC();
            return Result.Successful;
        }
        private Result ReplyCommand( string switchto, Message message)
        {

            //TODO Switch chat using given chat alias/user name
            if (!SwitchToChat(switchto))
                return Result.Failed;

            if (currentChat != lastMessageSent.FromEndPoint)
            {
                var chatAlias = chatAliases.Where(ca => ca.Endpoint == lastMessageSent.FromEndPoint).FirstOrDefault();
                if (chatAlias == null)
                {
                    SendMessage(new Message(
                        String.Format("I can't replay to a message from ({0})!", lastMessageSent.FromEndPoint.ToString()),
                        EndPoint.Bot, EndPoint.SteamAdmin)
                    );
                }
                else
                {
                    currentChat = lastMessageSent.FromEndPoint;
                }
            }

            message.FromEndPoint = EndPoint.SteamAdmin;
            message.ToEndPoint = currentChat;
            if( message.Text.Length > 0 )
                SendMessage(message);

            return Result.Successful;
        }
        private bool isFlood( Message message)
        {
            try
            {
                if (lastMessagePerEndpoint.FirstOrDefault(m => (m.Text == message.Text && m.ToEndPoint == m.ToEndPoint)) != null)
                    return true;
                else
                    lastMessagePerEndpoint.RemoveAll(m => m.ToEndPoint == message.ToEndPoint);

                lastMessagePerEndpoint.Add(message);
            }
            catch {
                Debug.Print("Exception in isFlood()");
            }
            return false;

        }
        private void SendMessage(Message message)
        {
            if( message == null )
                return;            

            message.Text = message.Text.Trim();           

            if( message.FromEndPoint != EndPoint.Console && 
                message.FromEndPoint != EndPoint.SteamAdmin &&
                message.FromEndPoint != EndPoint.Bot)
                lastMessageSent = message;

            if (message.Text.Length <= 0)
                return;

            // Execute command or write it to console
            if (message.FromEndPoint == EndPoint.Console ||
                message.FromEndPoint == EndPoint.SteamAdmin)
            {
                if (ParseAdminCommand(message.Text) == Result.Successful)
                {
                    log.WriteLine(message.Text, ChatIcon.Admin);
                    return;
                }
                if (!isFlood(message))
                    log.WriteLine(message.Text, ChatIcon.Admin);

                if (message.ToEndPoint == EndPoint.Console)
                    return;

                message.ToEndPoint = currentChat;
            }


            // Send message to specified chat(s)
            switch (message.ToEndPoint)
            {
                case EndPoint.All:
                    {
                        SendMessageToEmpireTV(message);
                        SendMessageToGohaIRC(message);
                        SendMessageToTwitchIRC(message);
                        SendMessageToSc2Tv(message);
                        SendMessageToCybergame(message);
                    }
                    break;
                case EndPoint.Sc2Tv:
                    SendMessageToSc2Tv(message);
                    break;
                case EndPoint.Skype:
                    SendMessageToSkype(message);
                    break;
                case EndPoint.SkypeGroup:
                    SendMessageToSkype(message);
                    break;
                case EndPoint.SteamAdmin:
                    SendMessageToSteamAdmin(message);
                    break;
                case EndPoint.TwitchTV:
                    SendMessageToTwitchIRC(message);
                    break;
                case EndPoint.Gohatv:
                    SendMessageToGohaIRC(message);
                    break;
                case EndPoint.Empiretv:
                    SendMessageToEmpireTV(message);
                    break;
                case EndPoint.Cybergame:
                    SendMessageToCybergame(message);
                    break;
                case EndPoint.Console:
                    log.WriteLine(message.Text);
                    break;
                default:
                    log.WriteLine("Can't send a message. Chat is readonly!");
                    break;
            }
            if (!isFlood(message))
            {
                log.WriteLine(message.Text, message.Icon);
                if (message.Icon == ChatIcon.Sc2Tv)
                {
                    if (message.Text.Contains(":s:"))
                    {
                        String m = message.Text;
                        for (int i = 0; i < m.Length; i++)
                        {
                            int smilePos = -1;
                            if (m.Substring(i).IndexOf(":s:") == 0)
                            {
                                foreach (Smile smile in sc2tv.smiles)
                                {
                                    smilePos = m.Substring(i).IndexOf(":s" + smile.Code);
                                    if (smilePos == 0)
                                    {                                                                
                                        log.ReplaceSmileCode(":s" + smile.Code, smile.bmp );                                        
                                        break;
                                    }
                                }
                            }
                        }

                        textMessages.ScrollToEnd();
                    }
                }
            }
        }
        private void SendMessageToSc2Tv(Message message)
        {
            if( sc2tv.LoggedIn && settings.sc2tvEnabled )
                sc2tv.sendMessage(message.Text);            
        }
        private void SendMessageToSkype(Message message)
        {
            //TODO implement sending to Skype. Add currentDestination to Skype class
        }
        private void SendMessageToSteamAdmin(Message message)
        {
            if (steamAdmin == null || steamBot == null)
                return;

            if (steamBot.loginStatus == SteamAPISession.LoginStatus.LoginSuccessful)
            {
                if (settings.skypeSkipGroupMessages && message.FromEndPoint == EndPoint.SkypeGroup)
                    return;
                if (steamAdmin.status != SteamAPISession.UserStatus.Online)
                    return;

                steamBot.SendMessage(steamAdmin, message.Text);
            }
        }
        private void SendMessageToTwitchIRC(Message message)
        {
            if (!settings.twitchEnabled || twitchIrc == null)
                return;
            
            if( twitchIrc.IsRegistered &&
                (message.FromEndPoint == EndPoint.Console || message.FromEndPoint == EndPoint.SteamAdmin))
            {
                var channelName = "#" + settings.TwitchUser;
                var twitchChannel = twitchIrc.Channels.SingleOrDefault(c => c.Name == channelName);
                twitchIrc.LocalUser.SendMessage(twitchChannel, message.Text);
            }

        }
        private void SendMessageToGohaIRC(Message message)
        {
            if (settings.gohaEnabled &&
                gohaIrc.IsRegistered &&
                (message.FromEndPoint == EndPoint.Console || message.FromEndPoint == EndPoint.SteamAdmin))
            {
                var channelName = "#" + settings.GohaIRCChannel;
                var gohaChannel = gohaIrc.Channels.SingleOrDefault(c => c.Name == channelName);
                gohaIrc.LocalUser.SendMessage(gohaChannel, message.Text);
            }

        }
        private void SendMessageToCybergame(Message message)
        {
            if (settings.cyberEnabled &&
                cybergame.isLoggedIn &&
                (message.FromEndPoint == EndPoint.Console || message.FromEndPoint == EndPoint.SteamAdmin))
            {
                cybergame.SendMessage(message.Text);
            }

        }
        private void SendRegisterInfoToGohaIRC(string email)
        {
            if (settings.gohaEnabled &&
                !string.IsNullOrEmpty(settings.GohaPassword) &&
                !string.IsNullOrEmpty(settings.GohaUser))
            {
                gohaIrc.LocalUser.SendMessage("NickServ", String.Format("REGISTER {0} {1}",settings.GohaPassword, email));
            }
        }
        private void SendConfirmCodeToGohaIRC(string confirmCode)
        {
            if (settings.gohaEnabled &&
                !string.IsNullOrEmpty(settings.GohaPassword) &&
                !string.IsNullOrEmpty(settings.GohaUser))
            {
                gohaIrc.LocalUser.SendMessage("NickServ", String.Format("VERIFY REGISTER {0} {1}",settings.GohaUser,confirmCode));
            }
        }
        private void SendUpdatePassToGohaIRC()
        {
            if (settings.gohaEnabled &&
                !string.IsNullOrEmpty(settings.GohaPassword) &&
                !string.IsNullOrEmpty(settings.GohaUser))
            {
                gohaIrc.LocalUser.SendMessage("NickServ", String.Format("SET PASSWORD {0}", settings.GohaPassword));
            }
        }

        private void SendMessageToEmpireTV(Message message)
        {
            if (settings.empireEnabled && empireTV.LoggedIn &&
                (message.FromEndPoint == EndPoint.Console || message.FromEndPoint == EndPoint.SteamAdmin))
            {
                empireTV.SendMessage(message.Text);
            }
        }


        private void pictureCurrentChat_Click(object sender, EventArgs e)
        {
            pictureCurrentChat.ContextMenuStrip.Show();
        }
        private void SwitchPlayersOn(bool switchGoha, bool switchSc2tv)
        {
            if (switchGoha && gohaTVstream != null)
            {
                if (gohaTVstream.LoggedIn && gohaTVstream.StreamStatus == "off")
                {
                    gohaTVstream.SwitchStream();
                    if (gohaTVstream.StreamStatus == "off")
                    {
                        streamStatus.SetOff(pictureGohaStream);
                        MessageBox.Show("Goha Live Stream Player wasn't switched on! Do it manually!");
                    }
                    else
                    {
                        SendMessage(new Message(String.Format("Goha: Live Stream Player switched on!"), EndPoint.Gohatv, EndPoint.SteamAdmin));
                        streamStatus.SetOn(pictureGohaStream);
                    }
                }
            }

            if (switchSc2tv && sc2tv != null)
            {
                sc2tv.LoadStreamSettings();
                if (sc2tv.LoggedIn && !sc2tv.isLive())
                {
                    sc2tv.setLiveStatus(true);
                    Thread.Sleep(2000);
                    sc2tv.LoadStreamSettings();
                    if (!sc2tv.isLive())
                    {
                        streamStatus.SetOff(pictureSc2tvStream);
                        MessageBox.Show("Sc2tv Live Stream Player wasn't switched on! Do it manually!");                        
                    }
                    else
                    {
                        SendMessage(new Message(String.Format("Sc2Tv: Live Stream Player switched on!"), EndPoint.Sc2Tv, EndPoint.SteamAdmin));
                        streamStatus.SetOn(pictureSc2tvStream);
                    }
                }
            }

        }
        private void SwitchPlayersOff( bool switchGoha, bool switchSc2tv)
        {
            
            if( switchGoha && gohaTVstream != null )
            {
                if (gohaTVstream.LoggedIn && gohaTVstream.StreamStatus == "on")
                {
                    gohaTVstream.SwitchStream();
                    if (gohaTVstream.StreamStatus == "on")
                    {
                        MessageBox.Show("Goha Live Stream Player wasn't switched off! Do it manually!");
                    }
                    else
                    {
                        SendMessage(new Message(String.Format("Goha: Live Stream Player switched off!"), EndPoint.Gohatv, EndPoint.SteamAdmin));
                        streamStatus.SetOff(pictureGohaStream);
                    }
                }
            }

            if (switchSc2tv && sc2tv != null)
            {
                sc2tv.LoadStreamSettings();
                if( sc2tv.LoggedIn && sc2tv.isLive() )
                {
                    sc2tv.setLiveStatus(false);
                    Thread.Sleep(2000);
                    sc2tv.LoadStreamSettings();
                    if (sc2tv.isLive())
                    {
                        MessageBox.Show("Sc2tv Live Stream Player wasn't switched off! Do it manually!");
                    }
                    else
                    {
                        SendMessage(new Message(String.Format("Sc2Tv: Live Stream Player switched off!"), EndPoint.Sc2Tv, EndPoint.SteamAdmin));
                        streamStatus.SetOff(pictureSc2tvStream);
                    }
                }
            }

        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            settings.mainformWidth = Size.Width;
            settings.mainformHeight = Size.Height;
            settings.Save();
            this.Visible = false;
            isDisconnecting = true;
            e.Cancel = true;
            SendMessage(new Message(String.Format("Leaving chats..."), EndPoint.Steam, EndPoint.SteamAdmin));
            timerEverySecond.Enabled = false;
            try
            {

                SwitchPlayersOff( true, true );

                var b1 = new BGWorker( StopTwitchIRC, null );
                var b2 = new BGWorker( StopGohaIRC, null );
                var b3 = new BGWorker( StopSteamBot, null );
                var b4 = new BGWorker(StopSc2Chat, null);
                var b5 = new BGWorker(StopGoodgame, null);
             
                // FlourineFx causing crash on exit if NetConnection object was connected to a server. 
                // So I using this dirty workaround until I'll find something better.
                //MessageBox.Show("test");
                Thread.Sleep(5000);
                Process.GetCurrentProcess().Kill();
            }
            catch
            {
               
            }
        }
        private void StopTwitchIRC()
        {
            if (twitchIrc != null)
            {
                if (twitchIrc.IsRegistered)
                {

                    twitchIrc.Quit();
                    SendMessage(new Message(String.Format("TwitchTV: disconnected!"), EndPoint.TwitchTV, EndPoint.SteamAdmin));
                    checkMark.SetOff(pictureTwitch);
                }
            }
        }
        private void StopGohaIRC()
        {
            if (gohaIrc != null)
            {
                if (gohaIrc.IsRegistered)
                {
                    gohaIrc.Disconnect(); //.Quit();
                    SendMessage(new Message(String.Format("GohaTV: disconnected."), EndPoint.Gohatv, EndPoint.SteamAdmin));
                    checkMark.SetOff(pictureGoha);
                }
            }
        }
        private void StopGoodgame()
        {
            if (ggChat == null || !settings.goodgameEnabled)
                return;

            ggChat.Disconnect();
            SendMessage(new Message(String.Format("Goodgame: disconnected."), EndPoint.Goodgame, EndPoint.SteamAdmin));
            checkMark.SetOff(pictureGoodgame);
        }
        private void StopSteamBot()
        {
            if (!settings.steamEnabled)
                return;

            bWorkerSteamPoll.CancelAsync();
            while (bWorkerSteamPoll.CancellationPending) Thread.Sleep(10);
            SendMessage(new Message(String.Format("Steam: disconnected."), EndPoint.Steam, EndPoint.SteamAdmin));
            checkMark.SetOff(pictureSteamBot);
        }
        private void StopSc2Chat()
        {
            if (settings.sc2tvEnabled)
            {
                bWorkerSc2TvPoll.CancelAsync();
                while (bWorkerSc2TvPoll.CancellationPending) Thread.Sleep(10);
                SendMessage(new Message(String.Format("Sc2tv: disconnected."), EndPoint.Sc2Tv, EndPoint.SteamAdmin));
                checkMark.SetOff(pictureSc2tv);

            }  
        }

        private void RunWithTimeout(ParameterizedThreadStart start, int timeoutSec)
        {
            Thread t = new Thread(start);
            t.Start();
            if (!t.Join(TimeSpan.FromSeconds(timeoutSec)))
            {
                try
                {
                    t.Abort();
                }
                catch
                {

                }
            }
        }
        private void textMessages_SizeChanged(object sender, EventArgs e)
        {
            textMessages.ScrollToEnd();
        }
        private void ShowSettings()
        {
            SettingsDialog settingsForm = new SettingsDialog();
            settingsForm.ShowDialog();
        }
        private Result ParseAdminCommand(string command)
        {

            var cmd = adminCommands.Where(ac => ac.isCommand(command)).FirstOrDefault();
            if (cmd != null)
            {
                cmd.Execute();
                return Result.Successful;
            }
            else
                return Result.Failed;
        }
        private void pictureGohaStream_Click(object sender, EventArgs e)
        {
            gohaTVstream.SwitchStream();
        }
        private void pictureSc2tvStream_Click(object sender, EventArgs e)
        {
            if (!sc2tv.LoggedIn)
                return;

            sc2tv.LoadStreamSettings();

            var prevLiveStatus = sc2tv.isLive();
            
            if (prevLiveStatus)
                sc2tv.setLiveStatus(false);
            else
                sc2tv.setLiveStatus(true);

            if (prevLiveStatus != sc2tv.isLive())
            {
                if (prevLiveStatus)
                {
                    SendMessage(new Message(String.Format("Sc2Tv: Stream switched off!"), EndPoint.Sc2Tv, EndPoint.SteamAdmin));
                    streamStatus.SetOff(pictureSc2tvStream);
                }
                else
                {
                    streamStatus.SetOn(pictureSc2tvStream);
                    SendMessage(new Message(String.Format("Sc2Tv: Stream switched on!"), EndPoint.Sc2Tv, EndPoint.SteamAdmin));
                }
            }
            else
            {
                SendMessage(new Message(String.Format("Sc2Tv: Stream wasn't switched! Please try again!"), EndPoint.Sc2Tv, EndPoint.SteamAdmin));
            }

        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            setTopMost();
        }
        private void setTopMost()
        {
            if (checkBoxOnTop.Checked)
            {
                SetTopMost(this, true);
            }
            else
            {
                SetTopMost( this, false);
            }
        }
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            SwitchBorder();
        }
        private void SwitchBorder()
        {
            if (checkBoxBorder.Checked)
            {
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            }
            else
            {
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            }
        }
        private void switchFullScreenMode()
        {
            if (panelMessages.Dock == DockStyle.Fill)
            {
                Size = settings.globalFullSize;
                settings.isFullscreenMode = false;
                panelMessages.Dock = DockStyle.None;
                panelMessages.Anchor = (AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom);
                panelMessages.Height = textCommand.Top;
                panelMessages.Width = groupBox1.Left - 5;
                textMessages.ScrollToEnd();
            }
            else
            {
                Size = settings.globalCompactSize;
                settings.isFullscreenMode = true;
                panelMessages.Dock = DockStyle.Fill;
                textMessages.ScrollToEnd();
            }

        }
        private void textMessages_DoubleClick(object sender, EventArgs e)
        {
            switchFullScreenMode();
        }

        private void hideTools()
        {
            if (checkBoxOnTop.Visible)
            {
                SetVisibility(panelTools, false);
                if (TransparencyKey != textMessages.BackColor)
                    SetTransparency(textMessages.BackColor);
            }

        }
        private void showTools()
        {
            if (!checkBoxOnTop.Visible)
            {
                SetVisibility(panelTools, true);
            }
            if (!trackBarTransparency.Visible)
                SetVisibility(trackBarTransparency, true);

            SetTransparency(Color.Empty);
        }
        private void SetTooltip(ToolTip tooltip, Control control, string value)
        {
            if (control.InvokeRequired)
            {
                SetTooltipCB d = new SetTooltipCB(SetTooltip);
                control.Parent.Invoke(d, new object[] { tooltip, control, value });
            }
            else
            {
                tooltip.SetToolTip(control, value);
            }
        }
        private void SetCheckedToolTip(ToolStripMenuItem item, bool state)
        {
            if (item == null)
                return;
            if (item.GetCurrentParent() == null)
                return;
            if (item.GetCurrentParent().InvokeRequired)
            {
                SetCheckedToolTipCB d = new SetCheckedToolTipCB(SetCheckedToolTip);
                item.GetCurrentParent().Invoke(d, new object[] { item, state });
            }
            else
            {
                item.Checked = state;
            }
        }
        private void SetTransparency(Color color)
        {
            if (this.InvokeRequired)
            {
                SetTransparencyCB d = new SetTransparencyCB(SetTransparency);
                this.Invoke(d, new object[] { color });
            }
            else
            {
                if (color == Color.Empty)
                {
                    if (this.Opacity != 1)
                    {
                        this.AllowTransparency = false;
                        this.Opacity = 1;
                    }
                }
                else
                {
                    if (this.Opacity != settings.globalTransparency / 100.0f)
                    {
                        this.AllowTransparency = true;
                        this.Opacity = settings.globalTransparency / 100.0f;
                    }
                }
            }
        }
        private void SetVisibility(Control control, bool visibility)
        {
            if (control.InvokeRequired)
            {
                SetVisibilityCB d = new SetVisibilityCB(SetVisibility);
                control.Parent.Invoke(d, new object[] { control, visibility });
            }
            else
            {
                control.Visible = visibility;
            }
        }
        private void SetTopMost(Form form, bool topmost)
        {
            if (form.InvokeRequired)
            {
                SetTopMostCB cb = new SetTopMostCB(SetTopMost);
                form.Invoke(cb, new object[] { form, topmost });
            }
            else
            {
                form.TopMost = topmost;
            }
        }
        private void SetComboValue(ComboBox combo, object value)
        {
            if (combo.InvokeRequired)
            {
                SetComboValueCB cb = new SetComboValueCB(SetComboValue);
                combo.Parent.Invoke(cb, new object[] {combo, value });
            }
            else
            {
                combo.SelectedValue = value;
            }
        }
        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {

        }
        private void MainForm_MouseEnter(object sender, EventArgs e)
        {

        }
        private void MainForm_Enter(object sender, EventArgs e)
        {
        }
        private void MainForm_Activated(object sender, EventArgs e)
        {

        }
        private void MainForm_Deactivate(object sender, EventArgs e)
        {

        }
        private void textMessages_Click(object sender, EventArgs e)
        {

        }
        private void label7_Click(object sender, EventArgs e)
        {

        }
        private void textMessages_MouseDown(object sender, MouseEventArgs e)
        {
            cursorPosBeforeMouseDown = Cursor.Position;
            isLMBDown = true;
        }
        private void textMessages_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = textMessages.PointToClient(Cursor.Position);
            if (!(p.X <= textMessages.ClientRectangle.Right &&
                p.X >= textMessages.ClientRectangle.Left &&
                p.Y <= textMessages.ClientRectangle.Bottom &&
                p.Y >= textMessages.ClientRectangle.Top))
            {
                isLMBDown = false;
            }
            if (isLMBDown)
            {
                this.Left += Cursor.Position.X - cursorPosBeforeMouseDown.X;
                this.Top += Cursor.Position.Y - cursorPosBeforeMouseDown.Y;
                cursorPosBeforeMouseDown = Cursor.Position;
            }
            showTools();
        }
        private void textMessages_MouseUp(object sender, MouseEventArgs e)
        {
            isLMBDown = false;
        }
        private void timerEverySecond_Tick(object sender, EventArgs e)
        {
            try
            {
                if (settings.globalOnTop != TopMost)
                    TopMost = settings.globalOnTop;

                UInt32 twitchViewers = 0, cybergameViewers = 0;
                if (twitchChannel != null)
                    UInt32.TryParse(twitchChannel.Viewers, out twitchViewers);

                if (cybergame != null)
                    UInt32.TryParse(cybergame.Viewers, out cybergameViewers);

                labelViewers.Text = String.Format("{0}", cybergameViewers + twitchViewers);
                SetTooltip(viewersTooltip, labelViewers, String.Format("Twitch.tv: {0}, Cybergame.tv: {1}", twitchViewers, cybergameViewers));
                if (trackBarTransparency.ClientRectangle.Contains(trackBarTransparency.PointToClient(Cursor.Position)))
                    return;

                if ((!ClientRectangle.Contains(PointToClient(Cursor.Position))))
                {
                    hideTools();
                }
                else
                {
                    showTools();
                }

                if (settings.obsRemoteEnable && !checkBoxBorder.Checked)
                {
                    if (obsRemote != null)
                    {
                        var stats = String.Format(
                            " FPS: {0} RATE: {1}K DROPS: {2}",
                            obsRemote.Status.fps,
                            obsRemote.Status.bitrate / 1024 * 8,
                            obsRemote.Status.framesDropped);
                        this.Text = formTitle + stats;
                    }
                }
            }
            catch { }

        }
        private void trackBarTransparency_MouseMove(object sender, MouseEventArgs e)
        {
            SetTransparency(textMessages.BackColor);
        }
        private void buttonFullscreen_Click_1(object sender, EventArgs e)
        {
            switchFullScreenMode();
        }           
        #endregion

        #region Cybergame methods and events
        private void ConnectCybergame()
        {
            if (!settings.cyberEnabled || 
                String.IsNullOrEmpty(settings.cyberUser) ||
                String.IsNullOrEmpty(settings.cyberPassword))
                return;
            cybergame = new Cybergame(settings.cyberUser, settings.cyberPassword);
            cybergame.Live += new EventHandler<EventArgs>(cybergame_Live);
            cybergame.Offline += new EventHandler<EventArgs>(cybergame_Offline);
            cybergame.OnMessage += new EventHandler<MessageReceivedEventArgs>(cybergame_OnMessage);
            cybergame.OnLogin += new EventHandler<EventArgs>(cybergame_OnLogin);

            if (!cybergame.Login())
            {
                SendMessage(new Message("Cybergame: login failed!", EndPoint.Cybergame, EndPoint.SteamAdmin));
            }
        }

        void cybergame_OnLogin(object sender, EventArgs e)
        {
            checkMark.SetOn(pictureCybergame);
            SendMessage(new Message("Cybergame: logged in!", EndPoint.Cybergame, EndPoint.SteamAdmin));
        }
        void cybergame_OnMessage(object sender, MessageReceivedEventArgs e)
        {
            SendMessage(new Message( String.Format("{0} ({1})",e.Message.message,e.Message.alias), EndPoint.Cybergame, EndPoint.SteamAdmin ));
        }
        void cybergame_Offline(object sender, EventArgs e)
        {
            streamStatus.SetOff(pictureCybergameStream);
            //throw new NotImplementedException();
        }
        void cybergame_Live(object sender, EventArgs e)
        {
            streamStatus.SetOn(pictureCybergameStream);
            //throw new NotImplementedException();
        }


        #endregion

        #region EmpireTV methods and events
        private void ConnectEmpireTV()
        {
            if (!settings.empireEnabled || String.IsNullOrEmpty(settings.empireUser) || String.IsNullOrEmpty(settings.empirePassword))
                return;

            empireTV.OnLogin += OnEmpireLogin;
            empireTV.OnNewMessage += OnEmpireMessage;

            empireTV.Login(settings.empireUser, settings.empirePassword);
        }
        private void OnEmpireLogin(object sender, EventArgs e)
        {
            checkMark.SetOn(pictureEmpire);
            empireTV.LoadHistory = settings.empireLoadHistory;
            empireTV.Enabled = true;
        }
        private void OnEmpireMessage(object sender, MessageArgs e)
        {
            SendMessage(new Message(String.Format("{0} ({1}{2})", e.Message.text, e.Message.nick, settings.empireAlias), EndPoint.Empiretv, EndPoint.SteamAdmin));
        }
        #endregion

        #region GohaTV Stream methods and events
        private void ConnectGohaStream()
        {
            if (!settings.gohaEnabled || String.IsNullOrEmpty(settings.GohaUser) || String.IsNullOrEmpty(settings.GohaPassword))
                return;

            gohaTVstream.OnLogin += OnGohaStreamLogin;
            gohaTVstream.OnLive += OnGohaStreamLive;
            gohaTVstream.OnOffline += OnGohaStreamOffline;

            gohaTVstream.Login(settings.GohaUser, settings.GohaPassword);
        }
        private void OnGohaStreamLogin(object sender, EventArgs e)
        {
            checkMark.SetOn(pictureGohaWeb);
            if (settings.gohaStreamControlOnStartExit && gohaTVstream.StreamStatus == "off")
                gohaTVstream.SwitchStream();
        }
        private void OnGohaStreamLive(object sender, EventArgs e)
        {
            streamStatus.SetOn(pictureGohaStream);
        }
        private void OnGohaStreamOffline(object sender, EventArgs e)
        {
            streamStatus.SetOff(pictureGohaStream);            
        }
        #endregion

        #region Twitch channel methods and events
        private void ConnectTwitchChannel()
        {

            if (String.IsNullOrEmpty(settings.TwitchUser) ||
                !settings.twitchEnabled)
                return;
          
            twitchChannel = new Twitch(settings.TwitchUser);
            twitchChannel.Live += OnGoLive;
            twitchChannel.Offline += OnGoOffline;
            twitchChannel.Start();
            adminCommands.Add(new AdminCommand(@"^/viewers\s*$", TwitchViewers));
            adminCommands.Add(new AdminCommand(@"^/bitrate\s*$", TwitchBitrate));
        }
        private Result TwitchViewers()
        {
            var m = new Message(String.Format("Twitch viewers: {0}", twitchChannel.Viewers), EndPoint.TwitchTV, EndPoint.SteamAdmin);
            SendMessage(m);
            return Result.Successful;
        }
        private Result TwitchBitrate()
        {
            var bitrate = (int)double.Parse(twitchChannel.Bitrate, NumberStyles.Float, CultureInfo.InvariantCulture);
            var m = new Message(String.Format("Twitch bitrate: {0}Kbit", bitrate), EndPoint.TwitchTV, EndPoint.SteamAdmin);
            SendMessage(m);
            return Result.Successful;
        }

        private void OnGoLive(object sender, EventArgs e)
        {
            if (!streamIsOnline)
            {
                streamIsOnline = false;
                try
                {
                    WindowsMediaPlayer a = new WMPLib.WindowsMediaPlayer();
                
                    a.URL = "online.mp3";
                    a.controls.play();
                    SendMessage(new Message(String.Format("Twitch: STREAM ONLINE!"), EndPoint.TwitchTV, EndPoint.SteamAdmin));
                    while (a.playState == WMPPlayState.wmppsPlaying)
                        Thread.Sleep(10);
                }
                catch{
                    Debug.Print("Exception in OnGoLive()");
                }                
            }

           

            if (settings.gohaStreamControl)
            {                
                if (gohaTVstream.LoggedIn)
                {
                    if (gohaTVstream.StreamStatus == "off")
                    {
                        if (!streamIsOnline)
                            SendMessage(new Message(String.Format("Goha: Stream switched on!"), EndPoint.TwitchTV, EndPoint.SteamAdmin));

                        gohaTVstream.SwitchStream();
                    }
                }
            }
            if (settings.sc2StreamAutoSwitch)
            {
                if (sc2tv.LoggedIn)
                {
                    sc2tv.LoadStreamSettings();
                    if (!sc2tv.isLive() )
                    {
                        sc2tv.setLiveStatus(true);
                        if (sc2tv.isLive())
                        {
                            SendMessage(new Message(String.Format("Sc2Tv: Stream switched on (Twitch stream went online)!"), EndPoint.Sc2Tv, EndPoint.SteamAdmin));
                            streamStatus.SetOn(pictureSc2tvStream);
                        }
                        else
                        {
                            streamStatus.SetOff(pictureSc2tvStream);
                        }
                    }
                }
            }
            streamStatus.SetOn(pictureStream);

            streamIsOnline = true;
        }
        private void OnGoOffline(object sender, EventArgs e)
        {
            if (streamIsOnline)
            {

                try
                {
                    WindowsMediaPlayer a = new WMPLib.WindowsMediaPlayer();
                    a.URL = "offline.mp3";
                    a.controls.play();
                    SendMessage(new Message(String.Format("Twitch: STREAM OFFLINE!"), EndPoint.TwitchTV, EndPoint.SteamAdmin));
                    while (a.playState == WMPPlayState.wmppsPlaying)
                        Thread.Sleep(10);
                }
                catch {
                    Debug.Print("Exception in OnGoOffline()");
                }
            }

           
            if (settings.gohaStreamControl)
            {
                if (gohaTVstream.LoggedIn)
                {
                    if (gohaTVstream.StreamStatus == "on")
                    {
                        if (streamIsOnline)
                            SendMessage(new Message(String.Format("Goha: Stream switched off!"), EndPoint.TwitchTV, EndPoint.SteamAdmin));
                        gohaTVstream.SwitchStream();
                    }
                }
            }
            if (settings.sc2StreamAutoSwitch)
            {
                if (sc2tv.LoggedIn)
                {
                    sc2tv.LoadStreamSettings();
                    if (sc2tv.isLive())
                    {
                        sc2tv.setLiveStatus(false);
                        if (!sc2tv.isLive())
                        {
                            SendMessage(new Message(String.Format("Sc2Tv: Stream switched off (Twitch stream went offline)!"), EndPoint.Sc2Tv, EndPoint.SteamAdmin));
                            streamStatus.SetOff(pictureSc2tvStream);
                        }
                        else
                        {
                            throw new Exception("Sc2tv stream wasn't switched! Do it manually!");
                        }
                    }
                }
            }
            streamStatus.SetOff(pictureStream);
            streamIsOnline = false;
            
        }
        #endregion

        #region Twitch IRC methods and events
        private void ConnectTwitchIRC()
        {
            //twitchIrc.FloodPreventer = new IrcStandardFloodPreventer(4, 1000);
            if (settings.TwitchUser.Length <= 0 ||
                !settings.twitchEnabled)
                return;

            try
            {
                if (twitchIrc != null && twitchIrc.IsConnected)
                {
                    twitchIrc.Quit("Bye!");
                    twitchIrc = null;
                }
            }
            catch
            {
                Debug.Print("Exception in ConnectTwitchIRC()");
            }

            twitchIrc = new IrcClient();
            twitchIrc.Connected += OnTwitchConnect;
            twitchIrc.Registered += OnTwitchRegister;
            twitchIrc.Disconnected += OnTwitchDisconnect;
            twitchIrc.Error += new EventHandler<IrcErrorEventArgs>(twitchIrc_Error);
            twitchIrc.RawMessageReceived += new EventHandler<IrcRawMessageEventArgs>(twitchIrc_RawMessageReceived);
            using (var connectedEvent = new ManualResetEventSlim(false))
            {
                twitchIrc.Connected += (sender2, e2) => connectedEvent.Set();
                twitchIrc.Connect(settings.TwitchUser + "." + twitchIRCDomain, false, new IrcUserRegistrationInfo()
                {
                    NickName = settings.TwitchUser,
                    UserName = settings.TwitchUser,
                    RealName = "Twitch bot of " + settings.TwitchUser,
                    Password = settings.TwitchPassword
                });               
                if (!connectedEvent.Wait(10000))
                {
                    SendMessage(new Message("Twitch: connection timeout!", EndPoint.TwitchTV, EndPoint.SteamAdmin));
                    return;
                }
                
                }
            }

        void twitchIrc_Error(object sender, IrcErrorEventArgs e)
        {
            SendMessage( new Message( String.Format( "Twitch IRC error: {0}", e.Error.Message), EndPoint.TwitchTV, EndPoint.SteamAdmin));
        }

        void twitchIrc_RawMessageReceived(object sender, IrcRawMessageEventArgs e)
        {

            if (e.RawContent.Contains("Login failed"))
            {
                SendMessage(new Message("Twitch login failed! Check settings!", EndPoint.TwitchTV, EndPoint.SteamAdmin));
                isDisconnecting = true;
            }
            else if (settings.twitchDebugMessages)
            {
                SendMessage(new Message(e.RawContent, EndPoint.TwitchTV, EndPoint.SteamAdmin));
            }
        }

        private void OnTwitchDisconnect(object sender, EventArgs e)
        {
            if (!settings.twitchEnabled)
                return;
            
            SendMessage(new Message("Twitch bot disconnecting from the IRC", EndPoint.TwitchTV, EndPoint.SteamAdmin));
            if (!isDisconnecting)
            {
                twitchBW.Stop();
                twitchBW = new BGWorker(ConnectTwitchIRC, null);
            }
            else
            {
                twitchIrc.Quit();
            }
        }
        private void OnTwitchConnect(object sender, EventArgs e)
        {
        }
        private void OnTwitchChannelList(object sender, IrcChannelListReceivedEventArgs e)
        {

        }
        private void OnTwitchChannelJoinLocal(object sender, IrcChannelEventArgs e)
        {
            e.Channel.MessageReceived += OnTwitchMessageReceived;
            e.Channel.UserJoined += OnTwitchChannelJoin;
            e.Channel.UserLeft += OnTwitchChannelLeft;
            SendMessage(new Message(String.Format("Twitch IRC: logged in!"), EndPoint.TwitchTV, EndPoint.SteamAdmin));
            checkMark.SetOn(pictureTwitch);
        }
        private void OnTwitchChannelLeftLocal(object sender, IrcChannelEventArgs e)
        {
            SendMessage(new Message(String.Format("Twitch: bot left!"), EndPoint.TwitchTV,
                EndPoint.SteamAdmin));
        }
        private void OnTwitchMessageReceivedLocal(object sender, IrcMessageEventArgs e)
        {
            if (e.Text.Contains("HISTORYEND") || 
                e.Text.Contains("USERCOLOR") ||
                e.Text.Contains("EMOTESET")) 
                return;

            SendMessage(new Message(String.Format("{1} ({0}{2})", e.Source, e.Text, "@twitch.tv"), EndPoint.TwitchTV, EndPoint.SteamAdmin));
        }
        private void OnTwitchNoticeReceivedLocal(object sender, IrcMessageEventArgs e)
        {
            SendMessage(new Message(String.Format("{1} ({0}{2})", e.Source, e.Text, "@twitch.tv"), EndPoint.TwitchTV, EndPoint.SteamAdmin));
        }
        private void OnTwitchChannelJoin(object sender, IrcChannelUserEventArgs e)
        {
            if( settings.twitchLeaveJoinMessages )
                SendMessage(new Message(String.Format("{0} joined " + settings.twitchChatAlias, e.ChannelUser.User.NickName), EndPoint.TwitchTV,EndPoint.SteamAdmin));
        }
        private void OnTwitchChannelLeft(object sender, IrcChannelUserEventArgs e)
        {
            if( settings.twitchLeaveJoinMessages )
                SendMessage(new Message(String.Format("{1}{0} left ", settings.twitchChatAlias, e.ChannelUser.User.NickName), EndPoint.TwitchTV, EndPoint.SteamAdmin));
        }
        private void OnTwitchMessageReceived(object sender, IrcMessageEventArgs e)
        {
            var m = new Message(String.Format("{1} ({0}{2})", e.Source, e.Text, "@twitch.tv"), EndPoint.TwitchTV, EndPoint.SteamAdmin);
            
            SendMessage(m);
        }
        private void OnTwitchNoticeReceived(object sender, IrcMessageEventArgs e)
        {
            var m = new Message(String.Format("{1} ({0}{2})", e.Source, e.Text, "@twitch.tv"), EndPoint.TwitchTV, EndPoint.SteamAdmin);
            SendMessage(m);
        }
        private void OnTwitchRegister(object sender, EventArgs e)
        {
            twitchIrc.Channels.Join("#" + settings.TwitchUser);
            twitchIrc.LocalUser.NoticeReceived += OnTwitchNoticeReceivedLocal;
            twitchIrc.LocalUser.MessageReceived += OnTwitchMessageReceivedLocal;
            twitchIrc.LocalUser.JoinedChannel += OnTwitchChannelJoinLocal;
            twitchIrc.LocalUser.LeftChannel += OnTwitchChannelLeftLocal;
        }
        #endregion

        #region Sc2Tv methods and events
        private void ConnectSc2tv()
        {
            if (!settings.sc2tvEnabled)
                return;

            if (sc2ChannelId != 0)
            {
                sc2tv.ChannelId = sc2ChannelId;
            }


            if (String.IsNullOrEmpty(settings.Sc2tvUser) || String.IsNullOrEmpty(settings.Sc2tvPassword))
                return;

            sc2tv.Login(settings.Sc2tvUser, settings.Sc2tvPassword);


        }
        private void bWorkerSc2TvPoll_DoWork(object sender, DoWorkEventArgs e)
        {
            if ((bWorkerSc2TvPoll.CancellationPending == true))
            {
                e.Cancel = true;
                return;
            }
            
            UpdateSc2TvMessages();
        }
        private void bWorkerSc2TvPoll_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
           bWorkerSc2TvPoll.RunWorkerAsync();
        }

        private void OnSc2TvLogin(object sender, Sc2Chat.Sc2Event e)
        {
            if (sc2tv.LoggedIn)
            {
                SendMessage(new Message(String.Format("Sc2tv: logged in!"), EndPoint.Sc2Tv, EndPoint.SteamAdmin));
                checkMark.SetOn(pictureSc2tv);
                sc2tv.updateStreamList();
                sc2tv.updateSmiles();
                UInt32.TryParse(sc2tv.GetStreamID(), out sc2ChannelId);
                settings.Sc2tvId = sc2ChannelId.ToString();

                if (sc2ChannelId != 0 )
                {
                    if (!sc2tv.updateChat(sc2ChannelId))
                        SendMessageToSc2Tv(new Message("Revive!", EndPoint.SteamAdmin, EndPoint.Sc2Tv));
                    
                    sc2tv.LoadStreamSettings();
                    if (sc2tv.ChannelIsLive)
                        streamStatus.SetOn(pictureSc2tvStream);
                    else
                        streamStatus.SetOff(pictureSc2tvStream);
                    var currentVal = sc2tv.channelList.getById(sc2ChannelId);
                    if(currentVal != null)
                        SetComboValue(comboSc2Channels, currentVal);
                }
                bWorkerSc2TvPoll.RunWorkerAsync();           
            }
            else
            {
                SendMessage(new Message(String.Format("Sc2tv: login failed!"), EndPoint.Sc2Tv, EndPoint.SteamAdmin));
            }
        }
        private void OnSc2TvChannelList(object sender, Sc2Chat.Sc2Event e)
        {
            if (channelsSC2 == null)
            {
                channelsSC2 = new BindingSource();
                channelsSC2.DataSource = sc2tv.channelList.channels;
            }
            comboSc2Channels.SetDataSource(null);
            comboSc2Channels.SetDataSource(channelsSC2, "Title", "Id");


        }
        private void OnSc2TvMessageReceived(object sender, Sc2Chat.Sc2MessageEvent e)
        {
            //if (e.message.name.ToLower() == settings.Sc2tvUser.ToLower())
               // return;

            var message = sc2tv.sanitizeMessage(e.message.message,settings.sc2tvSanitizeSmiles);
            if (message.Trim().Length <= 0)
                return;
            
            var to = e.message.to;
            
            if( to == settings.Sc2tvUser && 
                settings.sc2tvPersonalizedOnly )
            {
                SendMessage(new Message(String.Format("{0} ({1}{2})", message, e.message.name, settings.sc2tvChatAlias), EndPoint.Sc2Tv, EndPoint.SteamAdmin));
            }
            else
            {
                SendMessage(new Message(String.Format("{0} ({1}{2})", message, e.message.name, to == null?"":"->" + to), EndPoint.Sc2Tv, EndPoint.SteamAdmin));
            }
        }

        private void comboSc2Channels_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (comboSc2Channels.Items.Count <= 0)
                return;
            try
            {
                var channel = (dotSC2TV.Channel)comboSc2Channels.SelectedValue;
                SendMessage(new Message(String.Format("Switching sc2tv channel to: {0}", channel.Title), EndPoint.Console, EndPoint.SteamAdmin));
                sc2ChannelId = channel.Id;
            }
            catch { }
        }
        private void UpdateSc2TvMessages()
        {
            if (!sc2tv.LoggedIn)
            {
                Debug.Print("Sc2tv: Not logged in");
                return;
            }

            if (!sc2tv.updateChat(sc2ChannelId))
            {
                SendMessage(new Message(String.Format(@"Sc2tv channel #{0} is unavailable", sc2ChannelId ), EndPoint.Sc2Tv, EndPoint.SteamAdmin));
                SendMessageToSc2Tv(new Message("Revive!", EndPoint.SteamAdmin, EndPoint.Sc2Tv));

            }
            Debug.Print("Sc2tv: Chat updated");
            Thread.Sleep(5000);            
        }
        #endregion
        
        #region Steam bot methods and events
        private void backgroundWorkerSteamPoll_DoWork(object sender, DoWorkEventArgs e)
        {
            if ((bWorkerSteamPoll.CancellationPending == true))
            {
                e.Cancel = true;
                return;
            }
            if (steamBot == null || !settings.steamEnabled)
                return;


            if (steamBot.loginStatus == SteamAPISession.LoginStatus.LoginSuccessful)
                updateList = steamBot.Poll();
        }
        private void backgroundWorkerPoll_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bWorkerSteamPoll.RunWorkerAsync();
        }
        private void ConnectSteamBot()
        {
            
            string user = settings.SteamBot;
            var steamEnabled = settings.steamEnabled;
            if (String.IsNullOrEmpty(user) || !steamEnabled)
                return;

            steamBot = new SteamAPISession();
            steamBot.Logon += OnSteamLogin;
            steamBot.NewMessage += OnNewSteamMessage;
            steamBot.FriendStateChange += OnSteamFriendStatusChange;
            steamBot.Typing += OnSteamTyping;

            checkMark.SetOff(pictureSteamBot);

            string password = settings.SteamBotPassword;

            // Try to authenticate with token first
            status = steamBot.Authenticate(settings.SteamBotAccessToken);

            // If token failed, try user and password
            if (status != SteamAPISession.LoginStatus.LoginSuccessful)
            {
                settings.SteamBotAccessToken = "";
                status = steamBot.Authenticate(user, password);
                // Ask for SteamGuard code if required
                if (status == SteamAPISession.LoginStatus.SteamGuard)
                {
                    string code = InputBox.Show("Enter code:");
                    status = steamBot.Authenticate(user, password, code);
                }
            }

            if (status == SteamAPISession.LoginStatus.LoginSuccessful)
            {
                SendMessage(new Message(String.Format("Steam: logged in!"), EndPoint.Steam, EndPoint.SteamAdmin));
                settings.SteamBotAccessToken = steamBot.accessToken;
            }
            else
            {
                SendMessage(new Message(String.Format("Steam: login failed"), EndPoint.Steam, EndPoint.SteamAdmin));
            }
            settings.Save();
        }
        private void OnSteamTyping(object sender, SteamAPISession.SteamEvent e)
        {
            SendMessage( new Message(String.Format("Replying to {0}", currentChat.ToString() ), EndPoint.Steam, EndPoint.SteamAdmin));
        }
        private void OnSteamLogin(object sender, SteamAPISession.SteamEvent e)
        {
            checkMark.SetOn(pictureSteamBot);

            //Get Steam Admin ID
            if (String.IsNullOrEmpty(settings.SteamAdminId))
            {
                List<SteamAPISession.Friend> friends = steamBot.GetFriends();
                foreach (SteamAPISession.Friend f in friends)
                {
                    SteamAPISession.User user = steamBot.GetUserInfo(f.steamid);
                    if (user.nickname == settings.SteamAdmin)
                    {
                        steamAdmin = user;
                        settings.SteamAdminId = steamAdmin.steamid;
                        settings.Save();
                        break;
                    }
                }
            }
            else
            {
                steamAdmin = steamBot.GetUserInfo(settings.SteamAdminId);
            }


            if (steamAdmin != null)
            {
                SteamAPISession.User ui = steamBot.GetUserInfo(steamAdmin.steamid);
                if (ui.status != SteamAPISession.UserStatus.Offline)
                {
                    checkMark.SetOn(pictureSteamAdmin);
                    steamAdmin.status = SteamAPISession.UserStatus.Online;
                }

            }
            else
                SendMessage(new Message(String.Format("Can't find {0} in your friends! Check settings or add that account into friend list for bot!", 
                    settings.SteamAdmin), EndPoint.Steam, EndPoint.SteamAdmin));

            if( !bWorkerSteamPoll.IsBusy )
                bWorkerSteamPoll.RunWorkerAsync();

        }
        private void OnNewSteamMessage(object sender, SteamAPISession.SteamEvent e)
        {
            // Message or command from admin. Route it to chat or execute specified action
            if (e.update.origin == steamAdmin.steamid)
            {
                SendMessage( new Message(String.Format("{0}", e.update.message), EndPoint.SteamAdmin, currentChat) );
            }
        }
        private void OnSteamFriendStatusChange(object sender, SteamAPISession.SteamEvent e)
        {
            if (e.update.origin == steamAdmin.steamid)
            {
                if (e.update.status == SteamAPISession.UserStatus.Offline)
                {
                    checkMark.SetOff(pictureSteamAdmin);
                    steamAdmin.status = SteamAPISession.UserStatus.Offline;
                }
                else
                {
                    checkMark.SetOn(pictureSteamAdmin);
                    steamAdmin.status = SteamAPISession.UserStatus.Online;
                }
            }
        }
        #endregion

        #region Skype methods and events
        public void ConnectSkype()
        {
            var skypeEnabled = settings.skypeEnabled;
            if (!skypeEnabled)
                return;

            skype = new SkypeChat();
            if (skype == null)
                return;

            adminCommands.Add( new AdminCommand(@"^/hangup\s*(.*)$", skype.Hangup));
            adminCommands.Add( new AdminCommand(@"^/call\s*(.*)$", skype.Call));
            adminCommands.Add( new AdminCommand(@"^/answer\s*(.*)$", skype.Answer));
            adminCommands.Add( new AdminCommand(@"^/mute$",skype.SetMute,true));
            adminCommands.Add( new AdminCommand(@"^/unmute$",skype.SetMute,false));
            adminCommands.Add( new AdminCommand(@"^/speakoff$",skype.SetSpeakers,false));
            adminCommands.Add( new AdminCommand(@"^/speakon$",skype.SetSpeakers,true));

            skype.Connect += OnConnectSkype;
            try
            {
                if (!skype.Start())
                {
                    SendMessage(new Message(skype.LastError, EndPoint.Skype, EndPoint.SteamAdmin));
                }
            }
            catch {
                SendMessage(new Message("Skype: attach to process failed!", EndPoint.Skype, EndPoint.SteamAdmin));
            }
        }
        private void OnGroupMessageReceived(object sender, ChatMessageEventArgs e)
        {
            if( !settings.skypeSkipGroupMessages )
                SendMessage(new Message(String.Format("{2} ({1}@{0})", e.GroupName, e.From, e.Text), EndPoint.SkypeGroup, EndPoint.SteamAdmin));
        }
        public void OnMessageReceived(object sender, ChatMessageEventArgs e)
        {
            SendMessage(new Message(String.Format("{0} ({1}{2})", e.Text, e.From,settings.skypeChatAlias), EndPoint.Skype, EndPoint.SteamAdmin));
        }
        public void OnIncomingCall(object sender, CallEventArgs e)
        {
            SendMessage(new Message(String.Format("{0} calling you on Skype. Type /answer to respond.", e.from), EndPoint.Skype, EndPoint.SteamAdmin));
        }
        public void OnConnectSkype( object sender, EventArgs e)
        {
            checkMark.SetOn(pictureSkype);
            skype.MessageReceived += OnMessageReceived;
            skype.GroupMessageReceived += OnGroupMessageReceived;
            skype.IncomingCall += OnIncomingCall;
        }
        #endregion

        #region Goodgame methods and events
        public void ConnectGoodgame()
        {
            if (!settings.goodgameEnabled)
                return;

            ggChat = new Goodgame(settings.goodgameUser, settings.goodgamePassword, settings.goodgameLoadHistory);

            ggChat.OnMessageReceived += OnGGMessageReceived;
            ggChat.OnConnect += OnGGConnect;
            ggChat.OnDisconnect += OnGGDisconnect;
            ggChat.OnChannelListReceived += OnGGChannelListReceived;
            ggChat.OnError += OnGGError;
            ggChat.Connect();
        }
        public void OnGGDisconnect(object sender, EventArgs e)
        {
            goodgameBW.Stop();
            goodgameBW = new BGWorker(ConnectGoodgame, null);
            checkMark.SetOff(pictureGoodgame);
        }
        public void OnGGConnect(object sender, EventArgs e)
        {
            checkMark.SetOn(pictureGoodgame);
        }
        public void OnGGChannelListReceived(object sender, EventArgs e)
        {
            if (channelsGG == null)
            {
                channelsGG = new BindingSource();
                channelsGG.DataSource = ggChat.Channels;
                if( ggChat.Channels.Count > 0 )
                    ggChat.ChatId = ggChat.Channels[0].Id.ToString();
            }
            comboGGChannels.SetDataSource(null);
            comboGGChannels.SetDataSource(channelsGG, "TitleAndViewers", "Id");
        }
        public void OnGGMessageReceived(object sender, Goodgame.GGMessageEventArgs e)
        {
            SendMessage(new Message(String.Format("{0} ({1}{2})", e.Message.Text, e.Message.Sender.Name, settings.goodgameChatAlias), EndPoint.Goodgame, EndPoint.SteamAdmin));
        }
        private void OnGGError(object sender, Goodgame.TextEventArgs e)
        {
            SendMessage(new Message(String.Format("Goodgame error: {0}", e.Text), EndPoint.Goodgame, EndPoint.SteamAdmin));
        }
        private void comboGGChannels_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        #endregion

        #region XSplit methods and events
        public void OnXSplitFrameDrops(object sender, EventArgs e)
        {
            if (!settings.enableXSplitStats)
                return;

            XSplit xapp = null;
            uint framesDropped = 0;
            try
            {
                xapp = (XSplit)sender;
                framesDropped = xapp.FrameDrops;
            }
            catch { }
            SendMessage(new Message(
                String.Format("Frame drops detected! {0} frame(s) dropped so far", framesDropped),
                EndPoint.Bot, EndPoint.SteamAdmin)
            );
        }
        public void OnXSplitStatusRefresh(object sender, EventArgs e)
        {
            if( statusServer != null && settings.enableStatusServer )
                statusServer.Broadcast(xsplit.GetJson());
        }
        #endregion

        #region Battlelog methods and events

        public void ConnectBattlelog()
        {
            if( settings.battlelogEnabled && 
                !String.IsNullOrEmpty(settings.battlelogEmail) &&
                !String.IsNullOrEmpty(settings.battlelogPassword))
            {
                battlelog.OnMessageReceive += OnBattlelogMessage;
                battlelog.OnConnect += OnBattlelogConnect;
                battlelog.OnUnknownJson += OnBattlelogJson;
                battlelog.Start(settings.battlelogEmail,settings.battlelogPassword);
                
            }

        }
        public void OnBattlelogConnect(object sender, EventArgs e)
        {
            checkMark.SetOn(pictureBattlelog);
            SendMessage(new Message(String.Format("Connected to the Battlelog!"), EndPoint.Battlelog, EndPoint.SteamAdmin));          
        }
        public void OnBattlelogJson(object sender, StringEventArgs e)
        {
            if( String.IsNullOrEmpty(e.Message))
                return;
            SendMessage(new Message(String.Format("Unknown JSON from the Battlelog: {0}", e.Message), EndPoint.Battlelog, EndPoint.SteamAdmin));
        }
        public void OnBattlelogMessage(object sender, BattleChatMessageArgs e)
        {
            if (settings.battlelogEnabled)
            {
                if( e.message.fromUsername != settings.battlelogNick )
                    SendMessage(new Message(String.Format("{0} ({1}{2})", e.message.message, e.message.fromUsername, settings.battlelogChatAlias), EndPoint.Battlelog, EndPoint.SteamAdmin));                    
            }
        }

        #endregion

        #region Goha.tv methods and events
        private void ConnectGohaIRC()
        {
            //gohaIrc.FloodPreventer = new IrcStandardFloodPreventer(4, 1000);
            if (settings.GohaUser.Length <= 0 ||
                !settings.gohaEnabled)
                return;

            using (var connectedEvent = new ManualResetEventSlim(false))
            {
                gohaIrc.Connected += (sender2, e2) => connectedEvent.Set();
                gohaIrc.RawMessageReceived += new EventHandler<IrcRawMessageEventArgs>(gohaIrc_RawMessageReceived);
                gohaIrc.Error += new EventHandler<IrcErrorEventArgs>(gohaIrc_Error);
                gohaIrc.Connect(gohaIRCDomain, false, new IrcUserRegistrationInfo()
                {
                    NickName = settings.GohaUser,
                    UserName = settings.GohaUser,
                    RealName = "Goha bot of " + settings.GohaUser,
                    //Password = settings.GohaPassword
                });

                if (!connectedEvent.Wait(10000))
                {
                    SendMessage(new Message("Goha: connection timeout!", EndPoint.Gohatv, EndPoint.SteamAdmin));
                    return;
                }

            }
        }

        void gohaIrc_Error(object sender, IrcErrorEventArgs e)
        {            
            SendMessage(new Message(String.Format("Goha IRC error: {0}",e.Error.Message), EndPoint.Gohatv, EndPoint.SteamAdmin));            
        }

        void gohaIrc_RawMessageReceived(object sender, IrcRawMessageEventArgs e)
        {
            if (settings.gohaDebugMessages)
            {
                SendMessage(new Message(e.RawContent, EndPoint.Gohatv, EndPoint.SteamAdmin));
            }


            if (e.Message.Source == null)
                return;

            if (e.Message.Source.Name.ToLower() == "nickserv")
            {
                if (e.RawContent.Contains("Invalid password for"))
                {
                    SendMessage(new Message("Goha login failed! Check settings!", EndPoint.Gohatv, EndPoint.SteamAdmin));

                }
                else if (e.RawContent.Contains("You are now identified"))
                {
                    SendMessage(new Message(String.Format("Goha IRC: logged in!"), EndPoint.Gohatv, EndPoint.SteamAdmin));
                    checkMark.SetOn(pictureGoha);

                }
                else if (e.RawContent.Contains("is not a registered nickname"))
                {
                    var email = InputBox.Show("Enter your email to receive Goha confirmation code:");
                    if (string.IsNullOrEmpty(email))
                        return;
                    SendRegisterInfoToGohaIRC(email);
                }
                else if (e.RawContent.Contains("An email containing nickname activation instructions"))
                {
                    var confirmCode = InputBox.Show("Check your mail and enter Goha confirmation code:");
                    if (string.IsNullOrEmpty(confirmCode))
                        return;

                    SendConfirmCodeToGohaIRC(confirmCode);
                }
                else if (e.RawContent.Contains("Verification failed. Invalid key for"))
                {
                    var confirmCode = InputBox.Show("Wrong confirmation code. Try again:");
                    if (string.IsNullOrEmpty(confirmCode))
                        return;

                    SendConfirmCodeToGohaIRC(confirmCode);
                }
                else if (e.RawContent.Contains("has now been verified."))
                {
                    SendMessage(new Message(String.Format("Goha IRC: email verified!"), EndPoint.Gohatv, EndPoint.SteamAdmin));
                }
            }
            
        }
        private void OnGohaDisconnect(object sender, EventArgs e)
        {
            if (!settings.gohaEnabled)
                return;

            gohaIrc.Quit();
        }
        private void OnGohaConnect(object sender, EventArgs e)
        {
            //SendMessage( new Message(String.Format("Goha: joining to the channel"),EndPoint.Gohatv, EndPoint.SteamAdmin));
        }
        private void OnGohaChannelList(object sender, IrcChannelListReceivedEventArgs e)
        {

        }
        private void OnGohaChannelJoinLocal(object sender, IrcChannelEventArgs e)
        {
            e.Channel.MessageReceived += OnGohaMessageReceived;
            e.Channel.UserJoined += OnGohaChannelJoin;
            e.Channel.UserLeft += OnGohaChannelLeft;

            //gohaIrc.SendRawMessage("NICK " + settings.GohaUser);
            gohaIrc.LocalUser.SendMessage("NickServ", String.Format("IDENTIFY {0}", settings.GohaPassword));
  
        }
        private void OnGohaChannelLeftLocal(object sender, IrcChannelEventArgs e)
        {
            //SendMessage(new Message(String.Format("Goha: logged in!"), EndPoint.Gohatv,EndPoint.SteamAdmin));
        }
        private void OnGohaMessageReceivedLocal(object sender, IrcMessageEventArgs e)
        {      
            SendMessage(new Message(String.Format("{1} ({0}{2})", e.Source, e.Text, "@goha.tv"), EndPoint.Gohatv, EndPoint.SteamAdmin));
        }
        private void OnGohaNoticeReceivedLocal(object sender, IrcMessageEventArgs e)
        {
            
            //SendMessage(new Message(String.Format("{1} ({0}{2})", e.Source, e.Text, "@goha.tv"), EndPoint.Gohatv, EndPoint.SteamAdmin));
        }
        private void OnGohaChannelJoin(object sender, IrcChannelUserEventArgs e)
        {

            if (settings.gohaLeaveJoinMessages)
                SendMessage(new Message(String.Format("{1}{0} joined ",settings.gohaChatAlias, e.ChannelUser.User.NickName), EndPoint.Gohatv, EndPoint.SteamAdmin));

        }
        private void OnGohaChannelLeft(object sender, IrcChannelUserEventArgs e)
        {
            var nickName = e.ChannelUser.User.NickName;
            var chatAlias = settings.gohaChatAlias;
            var endPoint = EndPoint.Gohatv;

            if (!chatUsers.Exists(u => (u.NickName == nickName && u.EndPoint == endPoint)))
                chatUsers.Add(new ChatUser(null, nickName, endPoint));

            if (settings.gohaLeaveJoinMessages)
                SendMessage(new Message(String.Format("{1}{0} left ", chatAlias, nickName), endPoint, EndPoint.SteamAdmin));
        }
        private void OnGohaMessageReceived(object sender, IrcMessageEventArgs e)
        {
            var m = new Message(String.Format("{1} ({0}{2})", e.Source, e.Text, "@goha.tv"), EndPoint.Gohatv, EndPoint.SteamAdmin);

            SendMessage(m);
        }
        private void OnGohaNoticeReceived(object sender, IrcMessageEventArgs e)
        {
            var m = new Message(String.Format("{1} ({0})", e.Source, e.Text), EndPoint.Gohatv, EndPoint.SteamAdmin);
            SendMessage(m);
        }
        private void OnGohaRegister(object sender, EventArgs e)
        {
            gohaIrc.Channels.Join("#" + settings.GohaIRCChannel);

            gohaIrc.LocalUser.NoticeReceived += OnGohaNoticeReceivedLocal;
            gohaIrc.LocalUser.MessageReceived += OnGohaMessageReceivedLocal;
            gohaIrc.LocalUser.JoinedChannel += OnGohaChannelJoinLocal;
            gohaIrc.LocalUser.LeftChannel += OnGohaChannelLeftLocal;
        }

        private void UnprotectConfig()
        {
            try
            {
                // Open the configuration file and retrieve 
                // the connectionStrings section.
                Configuration config = ConfigurationManager.
                    OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);

                ConfigurationSectionGroup group = config.GetSectionGroup("userSettings") as ConfigurationSectionGroup;
                foreach (ConfigurationSection section in group.Sections)
                {
                    if (section.SectionInformation.IsProtected)
                    {
                        // Encrypt the section.
                        section.SectionInformation.UnprotectSection();
                    }
                }
                // Save the current configuration.
                config.Save();

            }
            catch
            {
            }
        }
        static void ProtectConfig()
        {
            try
            {
                // Open the configuration file and retrieve 
                // the connectionStrings section.
                    Configuration config = ConfigurationManager.
                        OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);

                    ConfigurationSectionGroup group = config.GetSectionGroup("userSettings") as ConfigurationSectionGroup;
                    foreach (ConfigurationSection section in group.Sections)
                    {
                        if (!section.SectionInformation.IsProtected)
                        {
                            // Encrypt the section.
                            section.SectionInformation.ProtectSection(
                                "DataProtectionConfigurationProvider");
                        }
                    }
                    // Save the current configuration.
                    config.Save();

            }
            catch
            {
            }
        }
        #endregion

        #region OBS Remote methods and events
        public void ConnectOBSRemote()
        {
            obsRemote = new OBSRemote();
            obsRemote.OnLive += new EventHandler<EventArgs>(obsRemote_OnLive);
            obsRemote.OnOffline += new EventHandler<EventArgs>(obsRemote_OnOffline);
            obsRemote.OnError += new EventHandler<EventArgs>(obsRemote_OnError);
            obsRemote.OnSceneList += new EventHandler<OBSSceneStatusEventArgs>(obsRemote_OnSceneList);
            obsRemote.OnSceneSet += new EventHandler<OBSMessageEventArgs>(obsRemote_OnSceneSet);
            obsRemote.OnSourceChange += new EventHandler<OBSSourceEventArgs>(obsRemote_OnSourceChange);
            obsRemote.Connect(settings.obsHost);
        }

        void obsRemote_OnSourceChange(object sender, OBSSourceEventArgs e)
        {
            foreach (ToolStripMenuItem item in contextSceneSwitch.Items)
            {
                if (item.Checked)
                {
                    foreach (ToolStripMenuItem subitem in item.DropDownItems)
                    {
                        if (subitem.Text == e.Source.name)
                        {
                            SetCheckedToolTip(subitem, e.Source.render);
                        }
                    }
                }
            }
            
        }

        void obsRemote_OnSceneSet(object sender, OBSMessageEventArgs e)
        {
            var sceneName = e.Message;
            if (String.IsNullOrEmpty(sceneName))
                return;

            foreach (ToolStripMenuItem item in contextSceneSwitch.Items)
            {
                if (item.Text == sceneName)
                {
                    SetCheckedToolTip(item, true);
                }
                else
                {
                    SetCheckedToolTip(item, false);
                }
            }
        }

        void obsRemote_OnSceneList(object sender, OBSSceneStatusEventArgs e)
        {
            contextSceneSwitch.Items.Clear();
            if (e.Status.scenes.Count <= 0)
            {
                contextSceneSwitch.Items.Add("No scenes");
                return;
            }

            foreach (Scene scene in e.Status.scenes)
            {
                ToolStripMenuItem item = (ToolStripMenuItem)contextSceneSwitch.Items.Add(scene.name);
                if (e.Status.currentScene == scene.name)
                    item.Checked = true;

                foreach (Source source in scene.sources)
                {
                    ToolStripMenuItem subitem = (ToolStripMenuItem)item.DropDownItems.Add(source.name,null, contextSceneSwitch_onClick);                    
                    if (source.render)
                        subitem.Checked = true;
                }
            }
        }


        void contextSceneSwitch_onClick(object sender, EventArgs e)
        {
            ToolStripMenuItem clickedItem = (ToolStripMenuItem)sender;
            obsRemote.SetSourceRenderer(clickedItem.Text, !clickedItem.Checked);
        }
        private void contextSceneSwitch_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if( !settings.obsRemoteEnable )
                return;

            var menuItem = (ToolStripMenuItem)e.ClickedItem;
            obsRemote.SetCurrentScene(menuItem.Text);
        }

        void obsRemote_OnError(object sender, EventArgs e)
        {
            Thread.Sleep(3000);
            ConnectOBSRemote();
        }

        void obsRemote_OnOffline(object sender, EventArgs e)
        {
            buttonStreamStartStop.Image = Properties.Resources.play;
            SwitchPlayersOff(true,true);
        }


        void obsRemote_OnLive(object sender, EventArgs e)
        {
            buttonStreamStartStop.Image = Properties.Resources.stop;
            SwitchPlayersOn(true,true);
        }

        #endregion

        private void twitchToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void contextMenuChat_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            SwitchToChat(chatAliases[contextMenuChat.Items.IndexOf(e.ClickedItem)].Alias);
        }

        private void pictureCurrentChat_Click_1(object sender, EventArgs e)
        {
            contextMenuChat.Show(Cursor.Position);
        }

        private void panelTools_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void buttonStreamStartStop_Click(object sender, EventArgs e)
        {

            StartStopOBSStream();
        }
        private void StartStopOBSStream()
        {
            if (settings.obsRemoteEnable)
            {
                if (obsRemote.Opened)
                {
                    obsRemote.StartStopStream();
                }
                else
                {
                    SendMessage(new Message("No connection to OBS plugin!", EndPoint.Bot, EndPoint.SteamAdmin));
                }
            }
            else
            {
                SendMessage(new Message("OBS control is not enabled. Check your settings!", EndPoint.Bot, EndPoint.SteamAdmin));
            }
        }
        private void contextSceneSwitch_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            //e.Cancel = !btnClose;
            //btnClose = true;
        }

        private void textMessages_TextChanged(object sender, EventArgs e)
        {

        }
        private void Chat2Image()
        {
            Debug.Print("Screen capture");
            Control2Image.RtbToBitmap(textMessages, @"c:\test.jpeg");
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {

        }

        private void MainForm_ResizeEnd(object sender, EventArgs e)
        {
            if (settings.isFullscreenMode)
            {
                settings.globalCompactSize = Size;
            }
            else
            {
                settings.globalFullSize = Size;
            }
        }

    }

}
