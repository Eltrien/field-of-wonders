namespace Ubiquitous
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.bWorkerSteamPoll = new System.ComponentModel.BackgroundWorker();
            this.contextMenuChat = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.twitchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sc2TvruToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.empiretvToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gohatvToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panelTools = new System.Windows.Forms.Panel();
            this.trackBarTransparency = new System.Windows.Forms.TrackBar();
            this.pictureBoxMoveTools = new System.Windows.Forms.PictureBox();
            this.buttonStreamStartStop = new System.Windows.Forms.Button();
            this.contextSceneSwitch = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.asdfToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBoxBorder = new System.Windows.Forms.CheckBox();
            this.buttonCommercial = new System.Windows.Forms.Button();
            this.checkBoxOnTop = new System.Windows.Forms.CheckBox();
            this.panelMessages = new System.Windows.Forms.Panel();
            this.counterHitBox = new Ubiquitous.CounterPanel();
            this.counterYoutube = new Ubiquitous.CounterPanel();
            this.counterGoodgame = new Ubiquitous.CounterPanel();
            this.counterCybergame = new Ubiquitous.CounterPanel();
            this.counterHash = new Ubiquitous.CounterPanel();
            this.counterTwitch = new Ubiquitous.CounterPanel();
            this.labelViewers = new Ubiquitous.CounterPanel();
            this.textMessages = new SC2TV.RTFControl.ExRichTextBox();
            this.buttonInvisible = new System.Windows.Forms.Button();
            this.comboSc2Channels = new Ubiquitous.ComboBoxWithId();
            this.comboGGChannels = new Ubiquitous.ComboBoxWithId();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.panelRight = new System.Windows.Forms.Panel();
            this.comboBoxProfiles = new Ubiquitous.ComboBoxWithId();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chatStatusHitBox = new Ubiquitous.ChatStatus();
            this.chatStatusOBS = new Ubiquitous.ChatStatus();
            this.chatStatusJetSet = new Ubiquitous.ChatStatus();
            this.chatStatusGamerTv = new Ubiquitous.ChatStatus();
            this.chatStatusTwitch = new Ubiquitous.ChatStatus();
            this.chatStatusSteamBot = new Ubiquitous.ChatStatus();
            this.chatStatusSteamAdmin = new Ubiquitous.ChatStatus();
            this.chatStatusSkype = new Ubiquitous.ChatStatus();
            this.chatStatusSc2tv = new Ubiquitous.ChatStatus();
            this.chatStatusHashd = new Ubiquitous.ChatStatus();
            this.chatStatusGoodgame = new Ubiquitous.ChatStatus();
            this.chatStatusGohaWeb = new Ubiquitous.ChatStatus();
            this.chatStatusGoha = new Ubiquitous.ChatStatus();
            this.chatStatusEmpire = new Ubiquitous.ChatStatus();
            this.chatStatusCybergame = new Ubiquitous.ChatStatus();
            this.chatStatusBattlelog = new Ubiquitous.ChatStatus();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label18 = new System.Windows.Forms.Label();
            this.pictureHashdStream = new System.Windows.Forms.PictureBox();
            this.label16 = new System.Windows.Forms.Label();
            this.pictureCybergameStream = new System.Windows.Forms.PictureBox();
            this.label14 = new System.Windows.Forms.Label();
            this.pictureSc2tvStream = new System.Windows.Forms.PictureBox();
            this.label12 = new System.Windows.Forms.Label();
            this.pictureGohaStream = new System.Windows.Forms.PictureBox();
            this.label11 = new System.Windows.Forms.Label();
            this.pictureStream = new System.Windows.Forms.PictureBox();
            this.buttonSettings = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.buttonFullscreen = new System.Windows.Forms.Button();
            this.imageListChatSize = new System.Windows.Forms.ImageList(this.components);
            this.textCommand = new System.Windows.Forms.TextBox();
            this.pictureCurrentChat = new System.Windows.Forms.PictureBox();
            this.timerEverySecond = new System.Windows.Forms.Timer(this.components);
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuChat.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panelTools.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarTransparency)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMoveTools)).BeginInit();
            this.contextSceneSwitch.SuspendLayout();
            this.panelMessages.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureHashdStream)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureCybergameStream)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureSc2tvStream)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureGohaStream)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureStream)).BeginInit();
            this.panelBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureCurrentChat)).BeginInit();
            this.SuspendLayout();
            // 
            // bWorkerSteamPoll
            // 
            this.bWorkerSteamPoll.WorkerSupportsCancellation = true;
            this.bWorkerSteamPoll.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerSteamPoll_DoWork);
            this.bWorkerSteamPoll.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerPoll_RunWorkerCompleted);
            // 
            // contextMenuChat
            // 
            this.contextMenuChat.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.twitchToolStripMenuItem,
            this.sc2TvruToolStripMenuItem,
            this.empiretvToolStripMenuItem,
            this.gohatvToolStripMenuItem,
            this.allToolStripMenuItem});
            this.contextMenuChat.Name = "contextMenuChat";
            this.contextMenuChat.Size = new System.Drawing.Size(125, 114);
            this.contextMenuChat.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuChat_ItemClicked);
            // 
            // twitchToolStripMenuItem
            // 
            this.twitchToolStripMenuItem.Name = "twitchToolStripMenuItem";
            this.twitchToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.twitchToolStripMenuItem.Text = "Twitch.tv";
            // 
            // sc2TvruToolStripMenuItem
            // 
            this.sc2TvruToolStripMenuItem.Name = "sc2TvruToolStripMenuItem";
            this.sc2TvruToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.sc2TvruToolStripMenuItem.Text = "Sc2tv.ru";
            // 
            // empiretvToolStripMenuItem
            // 
            this.empiretvToolStripMenuItem.Name = "empiretvToolStripMenuItem";
            this.empiretvToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.empiretvToolStripMenuItem.Text = "Empire.tv";
            // 
            // gohatvToolStripMenuItem
            // 
            this.gohatvToolStripMenuItem.Name = "gohatvToolStripMenuItem";
            this.gohatvToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.gohatvToolStripMenuItem.Text = "Goha.tv";
            // 
            // allToolStripMenuItem
            // 
            this.allToolStripMenuItem.Name = "allToolStripMenuItem";
            this.allToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.allToolStripMenuItem.Text = "All";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panel1.Controls.Add(this.panelTools);
            this.panel1.Controls.Add(this.panelMessages);
            this.panel1.Controls.Add(this.buttonInvisible);
            this.panel1.Controls.Add(this.comboSc2Channels);
            this.panel1.Controls.Add(this.comboGGChannels);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.panelRight);
            this.panel1.Controls.Add(this.panelBottom);
            this.panel1.ForeColor = System.Drawing.Color.White;
            this.panel1.Location = new System.Drawing.Point(-3, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(815, 482);
            this.panel1.TabIndex = 0;
            // 
            // panelTools
            // 
            this.panelTools.BackColor = global::Ubiquitous.Properties.Settings.Default.globalToolBoxBack;
            this.panelTools.Controls.Add(this.trackBarTransparency);
            this.panelTools.Controls.Add(this.pictureBoxMoveTools);
            this.panelTools.Controls.Add(this.buttonStreamStartStop);
            this.panelTools.Controls.Add(this.checkBox1);
            this.panelTools.Controls.Add(this.checkBoxBorder);
            this.panelTools.Controls.Add(this.buttonCommercial);
            this.panelTools.Controls.Add(this.checkBoxOnTop);
            this.panelTools.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::Ubiquitous.Properties.Settings.Default, "globalToolBoxBack", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.panelTools.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::Ubiquitous.Properties.Settings.Default, "globalToolsLocation", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.panelTools.Location = global::Ubiquitous.Properties.Settings.Default.globalToolsLocation;
            this.panelTools.Margin = new System.Windows.Forms.Padding(0);
            this.panelTools.Name = "panelTools";
            this.panelTools.Size = new System.Drawing.Size(392, 19);
            this.panelTools.TabIndex = 35;
            // 
            // trackBarTransparency
            // 
            this.trackBarTransparency.AutoSize = false;
            this.trackBarTransparency.BackColor = global::Ubiquitous.Properties.Settings.Default.globalToolBoxBack;
            this.trackBarTransparency.DataBindings.Add(new System.Windows.Forms.Binding("Value", global::Ubiquitous.Properties.Settings.Default, "globalTransparency", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.trackBarTransparency.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::Ubiquitous.Properties.Settings.Default, "globalToolBoxBack", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.trackBarTransparency.Location = new System.Drawing.Point(242, 3);
            this.trackBarTransparency.Maximum = 100;
            this.trackBarTransparency.Minimum = 20;
            this.trackBarTransparency.Name = "trackBarTransparency";
            this.trackBarTransparency.Size = new System.Drawing.Size(104, 17);
            this.trackBarTransparency.TabIndex = 3;
            this.trackBarTransparency.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarTransparency.Value = global::Ubiquitous.Properties.Settings.Default.globalTransparency;
            this.trackBarTransparency.MouseMove += new System.Windows.Forms.MouseEventHandler(this.trackBarTransparency_MouseMove);
            // 
            // pictureBoxMoveTools
            // 
            this.pictureBoxMoveTools.Image = global::Ubiquitous.Properties.Resources.dots;
            this.pictureBoxMoveTools.Location = new System.Drawing.Point(0, 2);
            this.pictureBoxMoveTools.Name = "pictureBoxMoveTools";
            this.pictureBoxMoveTools.Size = new System.Drawing.Size(16, 16);
            this.pictureBoxMoveTools.TabIndex = 6;
            this.pictureBoxMoveTools.TabStop = false;
            this.pictureBoxMoveTools.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxMoveTools_MouseDown);
            this.pictureBoxMoveTools.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBoxMoveTools_MouseMove);
            this.pictureBoxMoveTools.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBoxMoveTools_MouseUp);
            // 
            // buttonStreamStartStop
            // 
            this.buttonStreamStartStop.BackColor = global::Ubiquitous.Properties.Settings.Default.globalToolBoxBack;
            this.buttonStreamStartStop.ContextMenuStrip = this.contextSceneSwitch;
            this.buttonStreamStartStop.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::Ubiquitous.Properties.Settings.Default, "globalToolBoxBack", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.buttonStreamStartStop.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonStreamStartStop.FlatAppearance.BorderSize = 0;
            this.buttonStreamStartStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonStreamStartStop.Font = new System.Drawing.Font("Chiller", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonStreamStartStop.Image = global::Ubiquitous.Properties.Resources.play;
            this.buttonStreamStartStop.Location = new System.Drawing.Point(371, 0);
            this.buttonStreamStartStop.Margin = new System.Windows.Forms.Padding(0);
            this.buttonStreamStartStop.Name = "buttonStreamStartStop";
            this.buttonStreamStartStop.Size = new System.Drawing.Size(18, 18);
            this.buttonStreamStartStop.TabIndex = 5;
            this.buttonStreamStartStop.UseVisualStyleBackColor = false;
            this.buttonStreamStartStop.Click += new System.EventHandler(this.buttonStreamStartStop_Click);
            // 
            // contextSceneSwitch
            // 
            this.contextSceneSwitch.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.asdfToolStripMenuItem});
            this.contextSceneSwitch.Name = "contextSceneSwitch";
            this.contextSceneSwitch.Size = new System.Drawing.Size(129, 26);
            this.contextSceneSwitch.Closing += new System.Windows.Forms.ToolStripDropDownClosingEventHandler(this.contextSceneSwitch_Closing);
            this.contextSceneSwitch.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextSceneSwitch_ItemClicked);
            // 
            // asdfToolStripMenuItem
            // 
            this.asdfToolStripMenuItem.CheckOnClick = true;
            this.asdfToolStripMenuItem.Name = "asdfToolStripMenuItem";
            this.asdfToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.asdfToolStripMenuItem.Text = "No scenes";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.BackColor = global::Ubiquitous.Properties.Settings.Default.globalToolBoxBack;
            this.checkBox1.Checked = global::Ubiquitous.Properties.Settings.Default.globalUseChroma;
            this.checkBox1.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::Ubiquitous.Properties.Settings.Default, "globalToolBoxBack", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox1.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Ubiquitous.Properties.Settings.Default, "globalUseChroma", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBox1.ForeColor = System.Drawing.Color.White;
            this.checkBox1.Location = new System.Drawing.Point(162, 1);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(83, 17);
            this.checkBox1.TabIndex = 2;
            this.checkBox1.Text = "Use chroma";
            this.checkBox1.UseVisualStyleBackColor = false;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            this.checkBox1.Click += new System.EventHandler(this.checkBox1_Click);
            // 
            // checkBoxBorder
            // 
            this.checkBoxBorder.AutoSize = true;
            this.checkBoxBorder.BackColor = global::Ubiquitous.Properties.Settings.Default.globalToolBoxBack;
            this.checkBoxBorder.Checked = global::Ubiquitous.Properties.Settings.Default.globalHideBorder;
            this.checkBoxBorder.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Ubiquitous.Properties.Settings.Default, "globalHideBorder", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxBorder.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::Ubiquitous.Properties.Settings.Default, "globalToolBoxBack", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxBorder.ForeColor = System.Drawing.Color.White;
            this.checkBoxBorder.Location = new System.Drawing.Point(22, 1);
            this.checkBoxBorder.Name = "checkBoxBorder";
            this.checkBoxBorder.Size = new System.Drawing.Size(74, 17);
            this.checkBoxBorder.TabIndex = 2;
            this.checkBoxBorder.Text = "No Border";
            this.checkBoxBorder.UseVisualStyleBackColor = false;
            this.checkBoxBorder.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // buttonCommercial
            // 
            this.buttonCommercial.BackColor = global::Ubiquitous.Properties.Settings.Default.globalToolBoxBack;
            this.buttonCommercial.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::Ubiquitous.Properties.Settings.Default, "globalToolBoxBack", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.buttonCommercial.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonCommercial.FlatAppearance.BorderSize = 0;
            this.buttonCommercial.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonCommercial.Font = new System.Drawing.Font("Chiller", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCommercial.Image = ((System.Drawing.Image)(resources.GetObject("buttonCommercial.Image")));
            this.buttonCommercial.Location = new System.Drawing.Point(345, 0);
            this.buttonCommercial.Margin = new System.Windows.Forms.Padding(0);
            this.buttonCommercial.Name = "buttonCommercial";
            this.buttonCommercial.Size = new System.Drawing.Size(18, 18);
            this.buttonCommercial.TabIndex = 4;
            this.buttonCommercial.UseVisualStyleBackColor = false;
            this.buttonCommercial.Click += new System.EventHandler(this.button1_Click);
            // 
            // checkBoxOnTop
            // 
            this.checkBoxOnTop.AutoSize = true;
            this.checkBoxOnTop.BackColor = global::Ubiquitous.Properties.Settings.Default.globalToolBoxBack;
            this.checkBoxOnTop.Checked = global::Ubiquitous.Properties.Settings.Default.globalOnTop;
            this.checkBoxOnTop.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::Ubiquitous.Properties.Settings.Default, "globalOnTop", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxOnTop.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::Ubiquitous.Properties.Settings.Default, "globalToolBoxBack", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.checkBoxOnTop.Location = new System.Drawing.Point(98, 1);
            this.checkBoxOnTop.Name = "checkBoxOnTop";
            this.checkBoxOnTop.Size = new System.Drawing.Size(62, 17);
            this.checkBoxOnTop.TabIndex = 1;
            this.checkBoxOnTop.Text = "On Top";
            this.checkBoxOnTop.UseVisualStyleBackColor = false;
            // 
            // panelMessages
            // 
            this.panelMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelMessages.BackColor = global::Ubiquitous.Properties.Settings.Default.globalToolBoxBack;
            this.panelMessages.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelMessages.Controls.Add(this.counterHitBox);
            this.panelMessages.Controls.Add(this.counterYoutube);
            this.panelMessages.Controls.Add(this.counterGoodgame);
            this.panelMessages.Controls.Add(this.counterCybergame);
            this.panelMessages.Controls.Add(this.counterHash);
            this.panelMessages.Controls.Add(this.counterTwitch);
            this.panelMessages.Controls.Add(this.labelViewers);
            this.panelMessages.Controls.Add(this.textMessages);
            this.panelMessages.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::Ubiquitous.Properties.Settings.Default, "globalToolBoxBack", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.panelMessages.Location = new System.Drawing.Point(4, 0);
            this.panelMessages.Margin = new System.Windows.Forms.Padding(0);
            this.panelMessages.Name = "panelMessages";
            this.panelMessages.Size = new System.Drawing.Size(700, 456);
            this.panelMessages.TabIndex = 36;
            // 
            // counterHitBox
            // 
            this.counterHitBox.BackColor = global::Ubiquitous.Properties.Settings.Default.globalCounterBackColor;
            this.counterHitBox.Counter = "0";
            this.counterHitBox.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::Ubiquitous.Properties.Settings.Default, "globalCounterBackColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.counterHitBox.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::Ubiquitous.Properties.Settings.Default, "globalCounterTextColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.counterHitBox.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::Ubiquitous.Properties.Settings.Default, "globalCounterFont", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.counterHitBox.DataBindings.Add(new System.Windows.Forms.Binding("MouseTransparent", global::Ubiquitous.Properties.Settings.Default, "globalMouseTransparent", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.counterHitBox.DataBindings.Add(new System.Windows.Forms.Binding("Visible", global::Ubiquitous.Properties.Settings.Default, "globalCounterHitbox", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.counterHitBox.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::Ubiquitous.Properties.Settings.Default, "globalCounterPosHitbox", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.counterHitBox.Font = global::Ubiquitous.Properties.Settings.Default.globalCounterFont;
            this.counterHitBox.ForeColor = global::Ubiquitous.Properties.Settings.Default.globalCounterTextColor;
            this.counterHitBox.Image = global::Ubiquitous.Properties.Resources.hitbox;
            this.counterHitBox.Location = global::Ubiquitous.Properties.Settings.Default.globalCounterPosHitbox;
            this.counterHitBox.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.counterHitBox.MouseTransparent = global::Ubiquitous.Properties.Settings.Default.globalMouseTransparent;
            this.counterHitBox.Name = "counterHitBox";
            this.counterHitBox.Size = new System.Drawing.Size(45, 17);
            this.counterHitBox.TabIndex = 29;
            this.counterHitBox.Visible = global::Ubiquitous.Properties.Settings.Default.globalCounterHitbox;
            // 
            // counterYoutube
            // 
            this.counterYoutube.BackColor = global::Ubiquitous.Properties.Settings.Default.globalCounterBackColor;
            this.counterYoutube.Counter = "0";
            this.counterYoutube.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::Ubiquitous.Properties.Settings.Default, "globalCounterTextColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.counterYoutube.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::Ubiquitous.Properties.Settings.Default, "globalCounterBackColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.counterYoutube.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::Ubiquitous.Properties.Settings.Default, "globalCounterFont", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.counterYoutube.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::Ubiquitous.Properties.Settings.Default, "globalCounterPosYoutube", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.counterYoutube.DataBindings.Add(new System.Windows.Forms.Binding("Visible", global::Ubiquitous.Properties.Settings.Default, "globalCounterYoutube", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.counterYoutube.DataBindings.Add(new System.Windows.Forms.Binding("MouseTransparent", global::Ubiquitous.Properties.Settings.Default, "globalMouseTransparent", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.counterYoutube.Font = global::Ubiquitous.Properties.Settings.Default.globalCounterFont;
            this.counterYoutube.ForeColor = global::Ubiquitous.Properties.Settings.Default.globalCounterTextColor;
            this.counterYoutube.Image = global::Ubiquitous.Properties.Resources.youtube;
            this.counterYoutube.Location = global::Ubiquitous.Properties.Settings.Default.globalCounterPosYoutube;
            this.counterYoutube.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.counterYoutube.MouseTransparent = global::Ubiquitous.Properties.Settings.Default.globalMouseTransparent;
            this.counterYoutube.Name = "counterYoutube";
            this.counterYoutube.Size = new System.Drawing.Size(45, 17);
            this.counterYoutube.TabIndex = 28;
            this.counterYoutube.Visible = global::Ubiquitous.Properties.Settings.Default.globalCounterYoutube;
            // 
            // counterGoodgame
            // 
            this.counterGoodgame.BackColor = global::Ubiquitous.Properties.Settings.Default.globalCounterBackColor;
            this.counterGoodgame.Counter = "0";
            this.counterGoodgame.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::Ubiquitous.Properties.Settings.Default, "globalCounterTextColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.counterGoodgame.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::Ubiquitous.Properties.Settings.Default, "globalCounterBackColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.counterGoodgame.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::Ubiquitous.Properties.Settings.Default, "globalCounterFont", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.counterGoodgame.DataBindings.Add(new System.Windows.Forms.Binding("Visible", global::Ubiquitous.Properties.Settings.Default, "globalCounterGoodgame", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.counterGoodgame.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::Ubiquitous.Properties.Settings.Default, "globalCounterPosGoodgame", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.counterGoodgame.DataBindings.Add(new System.Windows.Forms.Binding("MouseTransparent", global::Ubiquitous.Properties.Settings.Default, "globalMouseTransparent", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.counterGoodgame.Font = global::Ubiquitous.Properties.Settings.Default.globalCounterFont;
            this.counterGoodgame.ForeColor = global::Ubiquitous.Properties.Settings.Default.globalCounterTextColor;
            this.counterGoodgame.Image = global::Ubiquitous.Properties.Resources.goodgameicon;
            this.counterGoodgame.Location = global::Ubiquitous.Properties.Settings.Default.globalCounterPosGoodgame;
            this.counterGoodgame.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.counterGoodgame.MouseTransparent = global::Ubiquitous.Properties.Settings.Default.globalMouseTransparent;
            this.counterGoodgame.Name = "counterGoodgame";
            this.counterGoodgame.Size = new System.Drawing.Size(45, 17);
            this.counterGoodgame.TabIndex = 27;
            this.counterGoodgame.Visible = global::Ubiquitous.Properties.Settings.Default.globalCounterGoodgame;
            // 
            // counterCybergame
            // 
            this.counterCybergame.BackColor = global::Ubiquitous.Properties.Settings.Default.globalCounterBackColor;
            this.counterCybergame.Counter = "0";
            this.counterCybergame.DataBindings.Add(new System.Windows.Forms.Binding("Visible", global::Ubiquitous.Properties.Settings.Default, "globalCounterCybergame", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.counterCybergame.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::Ubiquitous.Properties.Settings.Default, "globalCounterPosCybergame", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.counterCybergame.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::Ubiquitous.Properties.Settings.Default, "globalCounterTextColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.counterCybergame.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::Ubiquitous.Properties.Settings.Default, "globalCounterBackColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.counterCybergame.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::Ubiquitous.Properties.Settings.Default, "globalCounterFont", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.counterCybergame.DataBindings.Add(new System.Windows.Forms.Binding("MouseTransparent", global::Ubiquitous.Properties.Settings.Default, "globalMouseTransparent", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.counterCybergame.Font = global::Ubiquitous.Properties.Settings.Default.globalCounterFont;
            this.counterCybergame.ForeColor = global::Ubiquitous.Properties.Settings.Default.globalCounterTextColor;
            this.counterCybergame.Image = global::Ubiquitous.Properties.Resources.cybergame;
            this.counterCybergame.Location = global::Ubiquitous.Properties.Settings.Default.globalCounterPosCybergame;
            this.counterCybergame.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.counterCybergame.MouseTransparent = global::Ubiquitous.Properties.Settings.Default.globalMouseTransparent;
            this.counterCybergame.Name = "counterCybergame";
            this.counterCybergame.Size = new System.Drawing.Size(45, 17);
            this.counterCybergame.TabIndex = 27;
            this.counterCybergame.Visible = global::Ubiquitous.Properties.Settings.Default.globalCounterCybergame;
            // 
            // counterHash
            // 
            this.counterHash.BackColor = global::Ubiquitous.Properties.Settings.Default.globalCounterBackColor;
            this.counterHash.Counter = "0";
            this.counterHash.DataBindings.Add(new System.Windows.Forms.Binding("Visible", global::Ubiquitous.Properties.Settings.Default, "globalCounterHashd", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.counterHash.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::Ubiquitous.Properties.Settings.Default, "globalCounterPosHashd", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.counterHash.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::Ubiquitous.Properties.Settings.Default, "globalCounterTextColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.counterHash.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::Ubiquitous.Properties.Settings.Default, "globalCounterBackColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.counterHash.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::Ubiquitous.Properties.Settings.Default, "globalCounterFont", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.counterHash.DataBindings.Add(new System.Windows.Forms.Binding("MouseTransparent", global::Ubiquitous.Properties.Settings.Default, "globalMouseTransparent", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.counterHash.Font = global::Ubiquitous.Properties.Settings.Default.globalCounterFont;
            this.counterHash.ForeColor = global::Ubiquitous.Properties.Settings.Default.globalCounterTextColor;
            this.counterHash.Image = global::Ubiquitous.Properties.Resources.hashd;
            this.counterHash.Location = global::Ubiquitous.Properties.Settings.Default.globalCounterPosHashd;
            this.counterHash.Margin = new System.Windows.Forms.Padding(0);
            this.counterHash.MouseTransparent = global::Ubiquitous.Properties.Settings.Default.globalMouseTransparent;
            this.counterHash.Name = "counterHash";
            this.counterHash.Size = new System.Drawing.Size(45, 17);
            this.counterHash.TabIndex = 26;
            this.counterHash.Visible = global::Ubiquitous.Properties.Settings.Default.globalCounterHashd;
            // 
            // counterTwitch
            // 
            this.counterTwitch.BackColor = global::Ubiquitous.Properties.Settings.Default.globalCounterBackColor;
            this.counterTwitch.Counter = "0";
            this.counterTwitch.DataBindings.Add(new System.Windows.Forms.Binding("Visible", global::Ubiquitous.Properties.Settings.Default, "globalCounterTwitch", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.counterTwitch.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::Ubiquitous.Properties.Settings.Default, "globalCounterPosTwitch", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.counterTwitch.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::Ubiquitous.Properties.Settings.Default, "globalCounterTextColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.counterTwitch.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::Ubiquitous.Properties.Settings.Default, "globalCounterBackColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.counterTwitch.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::Ubiquitous.Properties.Settings.Default, "globalCounterFont", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.counterTwitch.DataBindings.Add(new System.Windows.Forms.Binding("MouseTransparent", global::Ubiquitous.Properties.Settings.Default, "globalMouseTransparent", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.counterTwitch.Font = global::Ubiquitous.Properties.Settings.Default.globalCounterFont;
            this.counterTwitch.ForeColor = global::Ubiquitous.Properties.Settings.Default.globalCounterTextColor;
            this.counterTwitch.Image = global::Ubiquitous.Properties.Resources.twitchicon;
            this.counterTwitch.Location = global::Ubiquitous.Properties.Settings.Default.globalCounterPosTwitch;
            this.counterTwitch.Margin = new System.Windows.Forms.Padding(0);
            this.counterTwitch.MouseTransparent = global::Ubiquitous.Properties.Settings.Default.globalMouseTransparent;
            this.counterTwitch.Name = "counterTwitch";
            this.counterTwitch.Size = new System.Drawing.Size(45, 17);
            this.counterTwitch.TabIndex = 25;
            this.counterTwitch.Visible = global::Ubiquitous.Properties.Settings.Default.globalCounterTwitch;
            // 
            // labelViewers
            // 
            this.labelViewers.BackColor = global::Ubiquitous.Properties.Settings.Default.globalCounterBackColor;
            this.labelViewers.Counter = "0";
            this.labelViewers.DataBindings.Add(new System.Windows.Forms.Binding("Visible", global::Ubiquitous.Properties.Settings.Default, "globalCounterTotal", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.labelViewers.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::Ubiquitous.Properties.Settings.Default, "globalCounterPosTotal", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.labelViewers.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::Ubiquitous.Properties.Settings.Default, "globalCounterBackColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.labelViewers.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::Ubiquitous.Properties.Settings.Default, "globalCounterTextColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.labelViewers.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::Ubiquitous.Properties.Settings.Default, "globalCounterFont", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.labelViewers.DataBindings.Add(new System.Windows.Forms.Binding("MouseTransparent", global::Ubiquitous.Properties.Settings.Default, "globalMouseTransparent", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.labelViewers.Font = global::Ubiquitous.Properties.Settings.Default.globalCounterFont;
            this.labelViewers.ForeColor = global::Ubiquitous.Properties.Settings.Default.globalCounterTextColor;
            this.labelViewers.Image = global::Ubiquitous.Properties.Resources.adminicon;
            this.labelViewers.Location = global::Ubiquitous.Properties.Settings.Default.globalCounterPosTotal;
            this.labelViewers.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.labelViewers.MouseTransparent = global::Ubiquitous.Properties.Settings.Default.globalMouseTransparent;
            this.labelViewers.Name = "labelViewers";
            this.labelViewers.Size = new System.Drawing.Size(45, 17);
            this.labelViewers.TabIndex = 24;
            this.labelViewers.Visible = global::Ubiquitous.Properties.Settings.Default.globalCounterTotal;
            // 
            // textMessages
            // 
            this.textMessages.Antialias = true;
            this.textMessages.BackColor = System.Drawing.Color.Black;
            this.textMessages.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textMessages.Caret = false;
            this.textMessages.DataBindings.Add(new System.Windows.Forms.Binding("TimeColor", global::Ubiquitous.Properties.Settings.Default, "globalTimestampForeground", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textMessages.DataBindings.Add(new System.Windows.Forms.Binding("HighlightColor", global::Ubiquitous.Properties.Settings.Default, "globalToolBoxBack", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textMessages.DataBindings.Add(new System.Windows.Forms.Binding("SaveToImageFileName", global::Ubiquitous.Properties.Settings.Default, "globalChatImageFilename", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textMessages.DataBindings.Add(new System.Windows.Forms.Binding("SaveToImage", global::Ubiquitous.Properties.Settings.Default, "globalChat2Image", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textMessages.DataBindings.Add(new System.Windows.Forms.Binding("TextColor", global::Ubiquitous.Properties.Settings.Default, "globalChatTextColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textMessages.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::Ubiquitous.Properties.Settings.Default, "globalChatTextColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textMessages.DataBindings.Add(new System.Windows.Forms.Binding("SlowScroll", global::Ubiquitous.Properties.Settings.Default, "globalSmoothScroll", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textMessages.DataBindings.Add(new System.Windows.Forms.Binding("PersonalMessageBack", global::Ubiquitous.Properties.Settings.Default, "globalPersonalMessageBack", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textMessages.DataBindings.Add(new System.Windows.Forms.Binding("PersonalMessageFont", global::Ubiquitous.Properties.Settings.Default, "globalPersonalMessageFont", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textMessages.DataBindings.Add(new System.Windows.Forms.Binding("PersonalMessageColor", global::Ubiquitous.Properties.Settings.Default, "personalMessageColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textMessages.DataBindings.Add(new System.Windows.Forms.Binding("MouseTransparent", global::Ubiquitous.Properties.Settings.Default, "globalMouseTransparent", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textMessages.DataBindings.Add(new System.Windows.Forms.Binding("TimestampFont", global::Ubiquitous.Properties.Settings.Default, "appearTimestampFont", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textMessages.DataBindings.Add(new System.Windows.Forms.Binding("TimestampColor", global::Ubiquitous.Properties.Settings.Default, "appearTimestampColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textMessages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textMessages.ForeColor = global::Ubiquitous.Properties.Settings.Default.globalChatTextColor;
            this.textMessages.HighlightColor = global::Ubiquitous.Properties.Settings.Default.globalToolBoxBack;
            this.textMessages.Location = new System.Drawing.Point(0, 0);
            this.textMessages.Margin = new System.Windows.Forms.Padding(20);
            this.textMessages.MaxLines = ((uint)(4u));
            this.textMessages.MouseTransparent = global::Ubiquitous.Properties.Settings.Default.globalMouseTransparent;
            this.textMessages.Name = "textMessages";
            this.textMessages.PersonalMessageBack = global::Ubiquitous.Properties.Settings.Default.globalPersonalMessageBack;
            this.textMessages.PersonalMessageColor = global::Ubiquitous.Properties.Settings.Default.personalMessageColor;
            this.textMessages.PersonalMessageFont = global::Ubiquitous.Properties.Settings.Default.globalPersonalMessageFont;
            this.textMessages.RawTextColor = null;
            this.textMessages.ReadOnly = true;
            this.textMessages.RTF = resources.GetString("textMessages.RTF");
            this.textMessages.SaveToImage = global::Ubiquitous.Properties.Settings.Default.globalChat2Image;
            this.textMessages.SaveToImageFileName = global::Ubiquitous.Properties.Settings.Default.globalChatImageFilename;
            this.textMessages.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.textMessages.SelectionAlignment = SC2TV.RTFControl.ExRichTextBox.TextAlign.Left;
            this.textMessages.Size = new System.Drawing.Size(698, 454);
            this.textMessages.SlowScroll = global::Ubiquitous.Properties.Settings.Default.globalSmoothScroll;
            this.textMessages.TabIndex = 23;
            this.textMessages.Text = "";
            this.textMessages.TextColor = global::Ubiquitous.Properties.Settings.Default.globalChatTextColor;
            this.textMessages.TimeColor = global::Ubiquitous.Properties.Settings.Default.globalTimestampForeground;
            this.textMessages.TimeStamp = false;
            this.textMessages.TimestampColor = global::Ubiquitous.Properties.Settings.Default.appearTimestampColor;
            this.textMessages.TimestampFont = global::Ubiquitous.Properties.Settings.Default.appearTimestampFont;
            this.textMessages.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.textMessages_LinkClicked);
            this.textMessages.SelectionChanged += new System.EventHandler(this.textMessages_SelectionChanged);
            this.textMessages.SizeChanged += new System.EventHandler(this.textMessages_SizeChanged);
            this.textMessages.TextChanged += new System.EventHandler(this.textMessages_TextChanged);
            this.textMessages.DoubleClick += new System.EventHandler(this.textMessages_DoubleClick);
            this.textMessages.MouseDown += new System.Windows.Forms.MouseEventHandler(this.textMessages_MouseDown);
            this.textMessages.MouseMove += new System.Windows.Forms.MouseEventHandler(this.textMessages_MouseMove);
            this.textMessages.MouseUp += new System.Windows.Forms.MouseEventHandler(this.textMessages_MouseUp);
            // 
            // buttonInvisible
            // 
            this.buttonInvisible.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonInvisible.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonInvisible.Location = new System.Drawing.Point(669, 622);
            this.buttonInvisible.Name = "buttonInvisible";
            this.buttonInvisible.Size = new System.Drawing.Size(62, 23);
            this.buttonInvisible.TabIndex = 0;
            this.buttonInvisible.Text = "Invisible";
            this.buttonInvisible.UseVisualStyleBackColor = true;
            this.buttonInvisible.Visible = false;
            // 
            // comboSc2Channels
            // 
            this.comboSc2Channels.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboSc2Channels.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.comboSc2Channels.DropDownWidth = 300;
            this.comboSc2Channels.ForeColor = System.Drawing.SystemColors.Window;
            this.comboSc2Channels.FormattingEnabled = true;
            this.comboSc2Channels.Location = new System.Drawing.Point(670, 552);
            this.comboSc2Channels.Name = "comboSc2Channels";
            this.comboSc2Channels.Size = new System.Drawing.Size(86, 21);
            this.comboSc2Channels.TabIndex = 3;
            this.comboSc2Channels.Visible = false;
            this.comboSc2Channels.DropDown += new System.EventHandler(this.comboSc2Channels_DropDown);
            this.comboSc2Channels.SelectionChangeCommitted += new System.EventHandler(this.comboSc2Channels_SelectionChangeCommitted);
            // 
            // comboGGChannels
            // 
            this.comboGGChannels.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboGGChannels.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.comboGGChannels.DropDownWidth = 300;
            this.comboGGChannels.ForeColor = System.Drawing.SystemColors.Window;
            this.comboGGChannels.FormattingEnabled = true;
            this.comboGGChannels.Location = new System.Drawing.Point(669, 595);
            this.comboGGChannels.Name = "comboGGChannels";
            this.comboGGChannels.Size = new System.Drawing.Size(121, 21);
            this.comboGGChannels.TabIndex = 4;
            this.comboGGChannels.Visible = false;
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(669, 579);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(71, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "Goodgame.ru";
            this.label7.Visible = false;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(669, 536);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(42, 13);
            this.label6.TabIndex = 24;
            this.label6.Text = "Sc2Tv:";
            this.label6.Visible = false;
            // 
            // panelRight
            // 
            this.panelRight.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelRight.Controls.Add(this.comboBoxProfiles);
            this.panelRight.Controls.Add(this.groupBox1);
            this.panelRight.Controls.Add(this.groupBox2);
            this.panelRight.Controls.Add(this.buttonSettings);
            this.panelRight.Controls.Add(this.button1);
            this.panelRight.Location = new System.Drawing.Point(703, 0);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(109, 456);
            this.panelRight.TabIndex = 37;
            // 
            // comboBoxProfiles
            // 
            this.comboBoxProfiles.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxProfiles.DropDownWidth = 200;
            this.comboBoxProfiles.FormattingEnabled = true;
            this.comboBoxProfiles.Location = new System.Drawing.Point(4, 36);
            this.comboBoxProfiles.Name = "comboBoxProfiles";
            this.comboBoxProfiles.Size = new System.Drawing.Size(101, 21);
            this.comboBoxProfiles.TabIndex = 26;
            this.comboBoxProfiles.SelectedIndexChanged += new System.EventHandler(this.comboBoxProfiles_SelectedIndexChanged);
            this.comboBoxProfiles.SelectionChangeCommitted += new System.EventHandler(this.comboBoxProfiles_SelectionChangeCommitted);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.AutoSize = true;
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.groupBox1.Controls.Add(this.chatStatusHitBox);
            this.groupBox1.Controls.Add(this.chatStatusOBS);
            this.groupBox1.Controls.Add(this.chatStatusJetSet);
            this.groupBox1.Controls.Add(this.chatStatusGamerTv);
            this.groupBox1.Controls.Add(this.chatStatusTwitch);
            this.groupBox1.Controls.Add(this.chatStatusSteamBot);
            this.groupBox1.Controls.Add(this.chatStatusSteamAdmin);
            this.groupBox1.Controls.Add(this.chatStatusSkype);
            this.groupBox1.Controls.Add(this.chatStatusSc2tv);
            this.groupBox1.Controls.Add(this.chatStatusHashd);
            this.groupBox1.Controls.Add(this.chatStatusGoodgame);
            this.groupBox1.Controls.Add(this.chatStatusGohaWeb);
            this.groupBox1.Controls.Add(this.chatStatusGoha);
            this.groupBox1.Controls.Add(this.chatStatusEmpire);
            this.groupBox1.Controls.Add(this.chatStatusCybergame);
            this.groupBox1.Controls.Add(this.chatStatusBattlelog);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(4, 187);
            this.groupBox1.MinimumSize = new System.Drawing.Size(101, 33);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(101, 259);
            this.groupBox1.TabIndex = 23;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Login status:";
            // 
            // chatStatusHitBox
            // 
            this.chatStatusHitBox.DataBindings.Add(new System.Windows.Forms.Binding("Visible", global::Ubiquitous.Properties.Settings.Default, "hitboxEnable", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chatStatusHitBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.chatStatusHitBox.Label = "HitBox";
            this.chatStatusHitBox.Location = new System.Drawing.Point(3, 241);
            this.chatStatusHitBox.Name = "chatStatusHitBox";
            this.chatStatusHitBox.On = false;
            this.chatStatusHitBox.Size = new System.Drawing.Size(95, 15);
            this.chatStatusHitBox.TabIndex = 15;
            this.chatStatusHitBox.Visible = global::Ubiquitous.Properties.Settings.Default.hitboxEnable;
            // 
            // chatStatusOBS
            // 
            this.chatStatusOBS.DataBindings.Add(new System.Windows.Forms.Binding("Visible", global::Ubiquitous.Properties.Settings.Default, "obsRemoteEnable", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chatStatusOBS.Dock = System.Windows.Forms.DockStyle.Top;
            this.chatStatusOBS.Label = "OBS remote";
            this.chatStatusOBS.Location = new System.Drawing.Point(3, 226);
            this.chatStatusOBS.Name = "chatStatusOBS";
            this.chatStatusOBS.On = false;
            this.chatStatusOBS.Size = new System.Drawing.Size(95, 15);
            this.chatStatusOBS.TabIndex = 14;
            this.chatStatusOBS.Visible = global::Ubiquitous.Properties.Settings.Default.obsRemoteEnable;
            this.chatStatusOBS.Click += new System.EventHandler(this.chatStatusOBS_Click);
            // 
            // chatStatusJetSet
            // 
            this.chatStatusJetSet.DataBindings.Add(new System.Windows.Forms.Binding("Visible", global::Ubiquitous.Properties.Settings.Default, "jetsetEnable", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chatStatusJetSet.Dock = System.Windows.Forms.DockStyle.Top;
            this.chatStatusJetSet.Label = "JetSet";
            this.chatStatusJetSet.Location = new System.Drawing.Point(3, 211);
            this.chatStatusJetSet.Name = "chatStatusJetSet";
            this.chatStatusJetSet.On = false;
            this.chatStatusJetSet.Size = new System.Drawing.Size(95, 15);
            this.chatStatusJetSet.TabIndex = 13;
            this.chatStatusJetSet.Visible = global::Ubiquitous.Properties.Settings.Default.jetsetEnable;
            // 
            // chatStatusGamerTv
            // 
            this.chatStatusGamerTv.DataBindings.Add(new System.Windows.Forms.Binding("Visible", global::Ubiquitous.Properties.Settings.Default, "gmtvEnabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chatStatusGamerTv.Dock = System.Windows.Forms.DockStyle.Top;
            this.chatStatusGamerTv.Label = "GamersTv";
            this.chatStatusGamerTv.Location = new System.Drawing.Point(3, 196);
            this.chatStatusGamerTv.Name = "chatStatusGamerTv";
            this.chatStatusGamerTv.On = false;
            this.chatStatusGamerTv.Size = new System.Drawing.Size(95, 15);
            this.chatStatusGamerTv.TabIndex = 12;
            this.chatStatusGamerTv.Visible = global::Ubiquitous.Properties.Settings.Default.gmtvEnabled;
            // 
            // chatStatusTwitch
            // 
            this.chatStatusTwitch.DataBindings.Add(new System.Windows.Forms.Binding("Visible", global::Ubiquitous.Properties.Settings.Default, "twitchEnabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chatStatusTwitch.Dock = System.Windows.Forms.DockStyle.Top;
            this.chatStatusTwitch.Label = "Twitch";
            this.chatStatusTwitch.Location = new System.Drawing.Point(3, 181);
            this.chatStatusTwitch.Name = "chatStatusTwitch";
            this.chatStatusTwitch.On = false;
            this.chatStatusTwitch.Size = new System.Drawing.Size(95, 15);
            this.chatStatusTwitch.TabIndex = 11;
            this.chatStatusTwitch.Visible = global::Ubiquitous.Properties.Settings.Default.twitchEnabled;
            this.chatStatusTwitch.VisibleChanged += new System.EventHandler(this.chatStatus_VisibleChanged);
            // 
            // chatStatusSteamBot
            // 
            this.chatStatusSteamBot.DataBindings.Add(new System.Windows.Forms.Binding("Visible", global::Ubiquitous.Properties.Settings.Default, "steamEnabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chatStatusSteamBot.Dock = System.Windows.Forms.DockStyle.Top;
            this.chatStatusSteamBot.Label = "Steam bot";
            this.chatStatusSteamBot.Location = new System.Drawing.Point(3, 166);
            this.chatStatusSteamBot.Name = "chatStatusSteamBot";
            this.chatStatusSteamBot.On = false;
            this.chatStatusSteamBot.Size = new System.Drawing.Size(95, 15);
            this.chatStatusSteamBot.TabIndex = 10;
            this.chatStatusSteamBot.Visible = global::Ubiquitous.Properties.Settings.Default.steamEnabled;
            this.chatStatusSteamBot.VisibleChanged += new System.EventHandler(this.chatStatus_VisibleChanged);
            // 
            // chatStatusSteamAdmin
            // 
            this.chatStatusSteamAdmin.DataBindings.Add(new System.Windows.Forms.Binding("Visible", global::Ubiquitous.Properties.Settings.Default, "steamEnabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chatStatusSteamAdmin.Dock = System.Windows.Forms.DockStyle.Top;
            this.chatStatusSteamAdmin.Label = "Steam admin";
            this.chatStatusSteamAdmin.Location = new System.Drawing.Point(3, 151);
            this.chatStatusSteamAdmin.Name = "chatStatusSteamAdmin";
            this.chatStatusSteamAdmin.On = false;
            this.chatStatusSteamAdmin.Size = new System.Drawing.Size(95, 15);
            this.chatStatusSteamAdmin.TabIndex = 9;
            this.chatStatusSteamAdmin.Visible = global::Ubiquitous.Properties.Settings.Default.steamEnabled;
            this.chatStatusSteamAdmin.VisibleChanged += new System.EventHandler(this.chatStatus_VisibleChanged);
            // 
            // chatStatusSkype
            // 
            this.chatStatusSkype.DataBindings.Add(new System.Windows.Forms.Binding("Visible", global::Ubiquitous.Properties.Settings.Default, "skypeEnabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chatStatusSkype.Dock = System.Windows.Forms.DockStyle.Top;
            this.chatStatusSkype.Label = "Skype";
            this.chatStatusSkype.Location = new System.Drawing.Point(3, 136);
            this.chatStatusSkype.Name = "chatStatusSkype";
            this.chatStatusSkype.On = false;
            this.chatStatusSkype.Size = new System.Drawing.Size(95, 15);
            this.chatStatusSkype.TabIndex = 8;
            this.chatStatusSkype.Visible = global::Ubiquitous.Properties.Settings.Default.skypeEnabled;
            this.chatStatusSkype.VisibleChanged += new System.EventHandler(this.chatStatus_VisibleChanged);
            // 
            // chatStatusSc2tv
            // 
            this.chatStatusSc2tv.DataBindings.Add(new System.Windows.Forms.Binding("Visible", global::Ubiquitous.Properties.Settings.Default, "sc2tvEnabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chatStatusSc2tv.Dock = System.Windows.Forms.DockStyle.Top;
            this.chatStatusSc2tv.Label = "Sc2Tv";
            this.chatStatusSc2tv.Location = new System.Drawing.Point(3, 121);
            this.chatStatusSc2tv.Name = "chatStatusSc2tv";
            this.chatStatusSc2tv.On = false;
            this.chatStatusSc2tv.Size = new System.Drawing.Size(95, 15);
            this.chatStatusSc2tv.TabIndex = 7;
            this.chatStatusSc2tv.Visible = global::Ubiquitous.Properties.Settings.Default.sc2tvEnabled;
            this.chatStatusSc2tv.VisibleChanged += new System.EventHandler(this.chatStatus_VisibleChanged);
            // 
            // chatStatusHashd
            // 
            this.chatStatusHashd.DataBindings.Add(new System.Windows.Forms.Binding("Visible", global::Ubiquitous.Properties.Settings.Default, "hashdEnabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chatStatusHashd.Dock = System.Windows.Forms.DockStyle.Top;
            this.chatStatusHashd.Label = "Hashd";
            this.chatStatusHashd.Location = new System.Drawing.Point(3, 106);
            this.chatStatusHashd.Name = "chatStatusHashd";
            this.chatStatusHashd.On = false;
            this.chatStatusHashd.Size = new System.Drawing.Size(95, 15);
            this.chatStatusHashd.TabIndex = 6;
            this.chatStatusHashd.Visible = global::Ubiquitous.Properties.Settings.Default.hashdEnabled;
            this.chatStatusHashd.VisibleChanged += new System.EventHandler(this.chatStatus_VisibleChanged);
            // 
            // chatStatusGoodgame
            // 
            this.chatStatusGoodgame.DataBindings.Add(new System.Windows.Forms.Binding("Visible", global::Ubiquitous.Properties.Settings.Default, "goodgameEnabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chatStatusGoodgame.Dock = System.Windows.Forms.DockStyle.Top;
            this.chatStatusGoodgame.Label = "Goodgame";
            this.chatStatusGoodgame.Location = new System.Drawing.Point(3, 91);
            this.chatStatusGoodgame.Name = "chatStatusGoodgame";
            this.chatStatusGoodgame.On = false;
            this.chatStatusGoodgame.Size = new System.Drawing.Size(95, 15);
            this.chatStatusGoodgame.TabIndex = 5;
            this.chatStatusGoodgame.Visible = global::Ubiquitous.Properties.Settings.Default.goodgameEnabled;
            this.chatStatusGoodgame.VisibleChanged += new System.EventHandler(this.chatStatus_VisibleChanged);
            // 
            // chatStatusGohaWeb
            // 
            this.chatStatusGohaWeb.DataBindings.Add(new System.Windows.Forms.Binding("Visible", global::Ubiquitous.Properties.Settings.Default, "gohaEnabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chatStatusGohaWeb.Dock = System.Windows.Forms.DockStyle.Top;
            this.chatStatusGohaWeb.Label = "Goha Web";
            this.chatStatusGohaWeb.Location = new System.Drawing.Point(3, 76);
            this.chatStatusGohaWeb.Name = "chatStatusGohaWeb";
            this.chatStatusGohaWeb.On = false;
            this.chatStatusGohaWeb.Size = new System.Drawing.Size(95, 15);
            this.chatStatusGohaWeb.TabIndex = 4;
            this.chatStatusGohaWeb.Visible = global::Ubiquitous.Properties.Settings.Default.gohaEnabled;
            this.chatStatusGohaWeb.VisibleChanged += new System.EventHandler(this.chatStatus_VisibleChanged);
            // 
            // chatStatusGoha
            // 
            this.chatStatusGoha.DataBindings.Add(new System.Windows.Forms.Binding("Visible", global::Ubiquitous.Properties.Settings.Default, "gohaEnabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chatStatusGoha.Dock = System.Windows.Forms.DockStyle.Top;
            this.chatStatusGoha.Label = "Goha";
            this.chatStatusGoha.Location = new System.Drawing.Point(3, 61);
            this.chatStatusGoha.Name = "chatStatusGoha";
            this.chatStatusGoha.On = false;
            this.chatStatusGoha.Size = new System.Drawing.Size(95, 15);
            this.chatStatusGoha.TabIndex = 3;
            this.chatStatusGoha.Visible = global::Ubiquitous.Properties.Settings.Default.gohaEnabled;
            this.chatStatusGoha.VisibleChanged += new System.EventHandler(this.chatStatus_VisibleChanged);
            // 
            // chatStatusEmpire
            // 
            this.chatStatusEmpire.DataBindings.Add(new System.Windows.Forms.Binding("Visible", global::Ubiquitous.Properties.Settings.Default, "empireEnabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chatStatusEmpire.Dock = System.Windows.Forms.DockStyle.Top;
            this.chatStatusEmpire.Label = "Empire";
            this.chatStatusEmpire.Location = new System.Drawing.Point(3, 46);
            this.chatStatusEmpire.Name = "chatStatusEmpire";
            this.chatStatusEmpire.On = false;
            this.chatStatusEmpire.Size = new System.Drawing.Size(95, 15);
            this.chatStatusEmpire.TabIndex = 2;
            this.chatStatusEmpire.Visible = global::Ubiquitous.Properties.Settings.Default.empireEnabled;
            this.chatStatusEmpire.VisibleChanged += new System.EventHandler(this.chatStatus_VisibleChanged);
            // 
            // chatStatusCybergame
            // 
            this.chatStatusCybergame.DataBindings.Add(new System.Windows.Forms.Binding("Visible", global::Ubiquitous.Properties.Settings.Default, "cyberEnabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chatStatusCybergame.Dock = System.Windows.Forms.DockStyle.Top;
            this.chatStatusCybergame.Label = "Cybergame";
            this.chatStatusCybergame.Location = new System.Drawing.Point(3, 31);
            this.chatStatusCybergame.Name = "chatStatusCybergame";
            this.chatStatusCybergame.On = false;
            this.chatStatusCybergame.Size = new System.Drawing.Size(95, 15);
            this.chatStatusCybergame.TabIndex = 1;
            this.chatStatusCybergame.Visible = global::Ubiquitous.Properties.Settings.Default.cyberEnabled;
            this.chatStatusCybergame.VisibleChanged += new System.EventHandler(this.chatStatus_VisibleChanged);
            // 
            // chatStatusBattlelog
            // 
            this.chatStatusBattlelog.DataBindings.Add(new System.Windows.Forms.Binding("Visible", global::Ubiquitous.Properties.Settings.Default, "battlelogEnabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.chatStatusBattlelog.Dock = System.Windows.Forms.DockStyle.Top;
            this.chatStatusBattlelog.Label = "Battlelog";
            this.chatStatusBattlelog.Location = new System.Drawing.Point(3, 16);
            this.chatStatusBattlelog.Name = "chatStatusBattlelog";
            this.chatStatusBattlelog.On = false;
            this.chatStatusBattlelog.Size = new System.Drawing.Size(95, 15);
            this.chatStatusBattlelog.TabIndex = 0;
            this.chatStatusBattlelog.Visible = global::Ubiquitous.Properties.Settings.Default.battlelogEnabled;
            this.chatStatusBattlelog.VisibleChanged += new System.EventHandler(this.chatStatus_VisibleChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Controls.Add(this.pictureHashdStream);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.pictureCybergameStream);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.pictureSc2tvStream);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.pictureGohaStream);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.pictureStream);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox2.ForeColor = System.Drawing.Color.White;
            this.groupBox2.Location = new System.Drawing.Point(4, 63);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(102, 118);
            this.groupBox2.TabIndex = 22;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Stream";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label18.Location = new System.Drawing.Point(18, 87);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(50, 13);
            this.label18.TabIndex = 9;
            this.label18.Text = "Hashd.tv";
            // 
            // pictureHashdStream
            // 
            this.pictureHashdStream.Image = ((System.Drawing.Image)(resources.GetObject("pictureHashdStream.Image")));
            this.pictureHashdStream.Location = new System.Drawing.Point(6, 87);
            this.pictureHashdStream.Name = "pictureHashdStream";
            this.pictureHashdStream.Size = new System.Drawing.Size(12, 12);
            this.pictureHashdStream.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureHashdStream.TabIndex = 8;
            this.pictureHashdStream.TabStop = false;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label16.Location = new System.Drawing.Point(18, 70);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(72, 13);
            this.label16.TabIndex = 7;
            this.label16.Text = "Cybergame.tv";
            // 
            // pictureCybergameStream
            // 
            this.pictureCybergameStream.Image = ((System.Drawing.Image)(resources.GetObject("pictureCybergameStream.Image")));
            this.pictureCybergameStream.Location = new System.Drawing.Point(6, 70);
            this.pictureCybergameStream.Name = "pictureCybergameStream";
            this.pictureCybergameStream.Size = new System.Drawing.Size(12, 12);
            this.pictureCybergameStream.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureCybergameStream.TabIndex = 6;
            this.pictureCybergameStream.TabStop = false;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label14.Location = new System.Drawing.Point(18, 53);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(47, 13);
            this.label14.TabIndex = 5;
            this.label14.Text = "Sc2tv.ru";
            // 
            // pictureSc2tvStream
            // 
            this.pictureSc2tvStream.Image = ((System.Drawing.Image)(resources.GetObject("pictureSc2tvStream.Image")));
            this.pictureSc2tvStream.Location = new System.Drawing.Point(6, 53);
            this.pictureSc2tvStream.Name = "pictureSc2tvStream";
            this.pictureSc2tvStream.Size = new System.Drawing.Size(12, 12);
            this.pictureSc2tvStream.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureSc2tvStream.TabIndex = 4;
            this.pictureSc2tvStream.TabStop = false;
            this.pictureSc2tvStream.Click += new System.EventHandler(this.pictureSc2tvStream_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label12.Location = new System.Drawing.Point(18, 36);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(50, 13);
            this.label12.TabIndex = 3;
            this.label12.Text = "Goha.TV";
            // 
            // pictureGohaStream
            // 
            this.pictureGohaStream.Image = ((System.Drawing.Image)(resources.GetObject("pictureGohaStream.Image")));
            this.pictureGohaStream.Location = new System.Drawing.Point(6, 36);
            this.pictureGohaStream.Name = "pictureGohaStream";
            this.pictureGohaStream.Size = new System.Drawing.Size(12, 12);
            this.pictureGohaStream.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureGohaStream.TabIndex = 2;
            this.pictureGohaStream.TabStop = false;
            this.pictureGohaStream.Click += new System.EventHandler(this.pictureGohaStream_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label11.Location = new System.Drawing.Point(18, 19);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(56, 13);
            this.label11.TabIndex = 1;
            this.label11.Text = "Twitch.TV";
            // 
            // pictureStream
            // 
            this.pictureStream.Image = ((System.Drawing.Image)(resources.GetObject("pictureStream.Image")));
            this.pictureStream.Location = new System.Drawing.Point(6, 19);
            this.pictureStream.Name = "pictureStream";
            this.pictureStream.Size = new System.Drawing.Size(12, 12);
            this.pictureStream.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureStream.TabIndex = 0;
            this.pictureStream.TabStop = false;
            // 
            // buttonSettings
            // 
            this.buttonSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSettings.Location = new System.Drawing.Point(5, 3);
            this.buttonSettings.Name = "buttonSettings";
            this.buttonSettings.Size = new System.Drawing.Size(56, 23);
            this.buttonSettings.TabIndex = 1;
            this.buttonSettings.Text = "Settings";
            this.buttonSettings.UseVisualStyleBackColor = false;
            this.buttonSettings.Click += new System.EventHandler(this.buttonSettings_Click_1);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.button1.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", global::Ubiquitous.Properties.Settings.Default, "globalDebug", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.button1.Enabled = global::Ubiquitous.Properties.Settings.Default.globalDebug;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(67, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(38, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Log";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.buttonFullscreen);
            this.panelBottom.Controls.Add(this.textCommand);
            this.panelBottom.Controls.Add(this.pictureCurrentChat);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 457);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(815, 25);
            this.panelBottom.TabIndex = 38;
            // 
            // buttonFullscreen
            // 
            this.buttonFullscreen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonFullscreen.BackColor = System.Drawing.Color.White;
            this.buttonFullscreen.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonFullscreen.ForeColor = System.Drawing.Color.Black;
            this.buttonFullscreen.ImageIndex = 0;
            this.buttonFullscreen.ImageList = this.imageListChatSize;
            this.buttonFullscreen.Location = new System.Drawing.Point(792, 1);
            this.buttonFullscreen.Name = "buttonFullscreen";
            this.buttonFullscreen.Size = new System.Drawing.Size(23, 21);
            this.buttonFullscreen.TabIndex = 37;
            this.buttonFullscreen.UseVisualStyleBackColor = false;
            this.buttonFullscreen.Click += new System.EventHandler(this.buttonFullscreen_Click);
            // 
            // imageListChatSize
            // 
            this.imageListChatSize.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListChatSize.ImageStream")));
            this.imageListChatSize.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListChatSize.Images.SetKeyName(0, "fullscreen.png");
            this.imageListChatSize.Images.SetKeyName(1, "fullscreen_exit.png");
            // 
            // textCommand
            // 
            this.textCommand.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textCommand.BackColor = global::Ubiquitous.Properties.Settings.Default.globalToolBoxBack;
            this.textCommand.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textCommand.DataBindings.Add(new System.Windows.Forms.Binding("ForeColor", global::Ubiquitous.Properties.Settings.Default, "globalChatTextColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textCommand.DataBindings.Add(new System.Windows.Forms.Binding("BackColor", global::Ubiquitous.Properties.Settings.Default, "globalToolBoxBack", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textCommand.ForeColor = global::Ubiquitous.Properties.Settings.Default.globalChatTextColor;
            this.textCommand.Location = new System.Drawing.Point(22, 2);
            this.textCommand.Name = "textCommand";
            this.textCommand.Size = new System.Drawing.Size(770, 20);
            this.textCommand.TabIndex = 36;
            this.textCommand.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textCommand_KeyUp);
            // 
            // pictureCurrentChat
            // 
            this.pictureCurrentChat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pictureCurrentChat.BackColor = System.Drawing.Color.White;
            this.pictureCurrentChat.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureCurrentChat.ContextMenuStrip = this.contextMenuChat;
            this.pictureCurrentChat.Image = global::Ubiquitous.Properties.Resources.twitchicon;
            this.pictureCurrentChat.Location = new System.Drawing.Point(3, 2);
            this.pictureCurrentChat.Name = "pictureCurrentChat";
            this.pictureCurrentChat.Size = new System.Drawing.Size(19, 21);
            this.pictureCurrentChat.TabIndex = 35;
            this.pictureCurrentChat.TabStop = false;
            this.pictureCurrentChat.Click += new System.EventHandler(this.pictureCurrentChat_Click_1);
            // 
            // timerEverySecond
            // 
            this.timerEverySecond.Enabled = true;
            this.timerEverySecond.Interval = 1000;
            this.timerEverySecond.Tick += new System.EventHandler(this.timerEverySecond_Tick);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // MainForm
            // 
            this.AcceptButton = this.buttonInvisible;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(810, 481);
            this.Controls.Add(this.panel1);
            this.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::Ubiquitous.Properties.Settings.Default, "mainFormPosition", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.DataBindings.Add(new System.Windows.Forms.Binding("StartPosition", global::Ubiquitous.Properties.Settings.Default, "mainformStartPos", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.DataBindings.Add(new System.Windows.Forms.Binding("TopMost", global::Ubiquitous.Properties.Settings.Default, "globalOnTop", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = global::Ubiquitous.Properties.Settings.Default.mainFormPosition;
            this.MinimumSize = new System.Drawing.Size(16, 36);
            this.Name = "MainForm";
            this.StartPosition = global::Ubiquitous.Properties.Settings.Default.mainformStartPos;
            this.Text = "Ubiquitous - MultiChat";
            this.TopMost = global::Ubiquitous.Properties.Settings.Default.globalOnTop;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.ResizeEnd += new System.EventHandler(this.MainForm_ResizeEnd);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyUp);
            this.contextMenuChat.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panelTools.ResumeLayout(false);
            this.panelTools.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarTransparency)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMoveTools)).EndInit();
            this.contextSceneSwitch.ResumeLayout(false);
            this.panelMessages.ResumeLayout(false);
            this.panelRight.ResumeLayout(false);
            this.panelRight.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureHashdStream)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureCybergameStream)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureSc2tvStream)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureGohaStream)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureStream)).EndInit();
            this.panelBottom.ResumeLayout(false);
            this.panelBottom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureCurrentChat)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.ComponentModel.BackgroundWorker bWorkerSteamPoll;
        private System.Windows.Forms.ContextMenuStrip contextMenuChat;
        private System.Windows.Forms.ToolStripMenuItem twitchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sc2TvruToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private SC2TV.RTFControl.ExRichTextBox textMessages;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button buttonInvisible;
        private System.Windows.Forms.Label label6;
        private ComboBoxWithId comboSc2Channels;
        private ComboBoxWithId comboGGChannels;
        private System.Windows.Forms.Button buttonSettings;
        private System.Windows.Forms.ImageList imageListChatSize;
        private System.Windows.Forms.CheckBox checkBoxOnTop;
        private System.Windows.Forms.CheckBox checkBoxBorder;
        private System.Windows.Forms.TrackBar trackBarTransparency;
        private System.Windows.Forms.Timer timerEverySecond;
        private System.Windows.Forms.Button buttonCommercial;
        private System.Windows.Forms.ToolStripMenuItem empiretvToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gohatvToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allToolStripMenuItem;
        private System.Windows.Forms.Panel panelTools;
        private System.Windows.Forms.Button buttonStreamStartStop;
        private System.Windows.Forms.ContextMenuStrip contextSceneSwitch;
        private System.Windows.Forms.ToolStripMenuItem asdfToolStripMenuItem;
        private System.Windows.Forms.Panel panelMessages;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.PictureBox pictureBoxMoveTools;
        private CounterPanel labelViewers;
        private CounterPanel counterCybergame;
        private CounterPanel counterHash;
        private CounterPanel counterTwitch;
        private System.Windows.Forms.CheckBox checkBox1;
        private CounterPanel counterGoodgame;
        private CounterPanel counterYoutube;
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.PictureBox pictureHashdStream;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.PictureBox pictureCybergameStream;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.PictureBox pictureSc2tvStream;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.PictureBox pictureGohaStream;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.PictureBox pictureStream;
        private ChatStatus chatStatusBattlelog;
        private ChatStatus chatStatusTwitch;
        private ChatStatus chatStatusSteamBot;
        private ChatStatus chatStatusSteamAdmin;
        private ChatStatus chatStatusSkype;
        private ChatStatus chatStatusSc2tv;
        private ChatStatus chatStatusHashd;
        private ChatStatus chatStatusGoodgame;
        private ChatStatus chatStatusGohaWeb;
        private ChatStatus chatStatusGoha;
        private ChatStatus chatStatusEmpire;
        private ChatStatus chatStatusCybergame;
        private ChatStatus chatStatusGamerTv;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Button buttonFullscreen;
        private System.Windows.Forms.TextBox textCommand;
        private System.Windows.Forms.PictureBox pictureCurrentChat;
        private ChatStatus chatStatusJetSet;
        private ChatStatus chatStatusOBS;
        private ComboBoxWithId comboBoxProfiles;
        private ChatStatus chatStatusHitBox;
        private CounterPanel counterHitBox;
    }
}

