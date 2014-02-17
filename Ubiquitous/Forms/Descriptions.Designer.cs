namespace Ubiquitous.Forms
{
    partial class Descriptions
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
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonUpdateWeb = new System.Windows.Forms.Button();
            this.textBoxProfile = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.webAutocompleteGoha = new Ubiquitous.Controls.WebAutocomplete();
            this.label11 = new System.Windows.Forms.Label();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBoxCybergame = new System.Windows.Forms.GroupBox();
            this.webAutocompleteCybergame = new Ubiquitous.Controls.WebAutocomplete();
            this.label7 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBoxGoodgame = new System.Windows.Forms.GroupBox();
            this.textWebSourceGGGame = new Ubiquitous.Controls.WebAutocomplete();
            this.label8 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBoxSc2tv = new System.Windows.Forms.GroupBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textWebSourceSc2tvGame = new Ubiquitous.Controls.WebAutocomplete();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBoxTwitch = new System.Windows.Forms.GroupBox();
            this.textWebSourceTwitchGame = new Ubiquitous.Controls.WebAutocomplete();
            this.label3 = new System.Windows.Forms.Label();
            this.textTwitchDescription = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBoxCybergame.SuspendLayout();
            this.groupBoxGoodgame.SuspendLayout();
            this.groupBoxSc2tv.SuspendLayout();
            this.groupBoxTwitch.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Profile:";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.groupBoxCybergame);
            this.panel1.Controls.Add(this.groupBoxGoodgame);
            this.panel1.Controls.Add(this.groupBoxSc2tv);
            this.panel1.Controls.Add(this.groupBoxTwitch);
            this.panel1.Location = new System.Drawing.Point(0, 36);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(420, 510);
            this.panel1.TabIndex = 0;
            this.panel1.TabStop = true;
            // 
            // buttonUpdateWeb
            // 
            this.buttonUpdateWeb.Location = new System.Drawing.Point(236, 6);
            this.buttonUpdateWeb.Name = "buttonUpdateWeb";
            this.buttonUpdateWeb.Size = new System.Drawing.Size(108, 26);
            this.buttonUpdateWeb.TabIndex = 5;
            this.buttonUpdateWeb.Text = "Update web pages";
            this.buttonUpdateWeb.UseVisualStyleBackColor = true;
            this.buttonUpdateWeb.Click += new System.EventHandler(this.buttonUpdateWeb_Click);
            // 
            // textBoxProfile
            // 
            this.textBoxProfile.Location = new System.Drawing.Point(56, 9);
            this.textBoxProfile.Name = "textBoxProfile";
            this.textBoxProfile.Size = new System.Drawing.Size(174, 20);
            this.textBoxProfile.TabIndex = 6;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.webAutocompleteGoha);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.textBox5);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.DataBindings.Add(new System.Windows.Forms.Binding("Visible", global::Ubiquitous.Properties.Settings.Default, "gohaEnabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 500);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(403, 140);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Goha";
            this.groupBox1.Visible = global::Ubiquitous.Properties.Settings.Default.gohaEnabled;
            // 
            // webAutocompleteGoha
            // 
            this.webAutocompleteGoha.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.webAutocompleteGoha.Autocompletedata = null;
            this.webAutocompleteGoha.AutoSize = true;
            this.webAutocompleteGoha.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.webAutocompleteGoha.CurrentText = global::Ubiquitous.Properties.Settings.Default.goha_Game;
            this.webAutocompleteGoha.DataBindings.Add(new System.Windows.Forms.Binding("CurrentText", global::Ubiquitous.Properties.Settings.Default, "goha_Game", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.webAutocompleteGoha.Location = new System.Drawing.Point(81, 106);
            this.webAutocompleteGoha.Margin = new System.Windows.Forms.Padding(0);
            this.webAutocompleteGoha.Name = "webAutocompleteGoha";
            this.webAutocompleteGoha.Size = new System.Drawing.Size(212, 23);
            this.webAutocompleteGoha.TabIndex = 3;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(43, 112);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(38, 13);
            this.label11.TabIndex = 2;
            this.label11.Text = "Game:";
            // 
            // textBox5
            // 
            this.textBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox5.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Ubiquitous.Properties.Settings.Default, "goha_ShortDescription", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox5.Location = new System.Drawing.Point(84, 23);
            this.textBox5.Multiline = true;
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(304, 80);
            this.textBox5.TabIndex = 1;
            this.textBox5.Text = global::Ubiquitous.Properties.Settings.Default.goha_ShortDescription;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(15, 23);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(63, 13);
            this.label12.TabIndex = 0;
            this.label12.Text = "Description:";
            // 
            // groupBoxCybergame
            // 
            this.groupBoxCybergame.Controls.Add(this.webAutocompleteCybergame);
            this.groupBoxCybergame.Controls.Add(this.label7);
            this.groupBoxCybergame.Controls.Add(this.textBox3);
            this.groupBoxCybergame.Controls.Add(this.label10);
            this.groupBoxCybergame.DataBindings.Add(new System.Windows.Forms.Binding("Visible", global::Ubiquitous.Properties.Settings.Default, "cyberEnabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.groupBoxCybergame.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxCybergame.Location = new System.Drawing.Point(0, 360);
            this.groupBoxCybergame.Name = "groupBoxCybergame";
            this.groupBoxCybergame.Size = new System.Drawing.Size(403, 140);
            this.groupBoxCybergame.TabIndex = 4;
            this.groupBoxCybergame.TabStop = false;
            this.groupBoxCybergame.Text = "Cybergame";
            this.groupBoxCybergame.Visible = global::Ubiquitous.Properties.Settings.Default.cyberEnabled;
            // 
            // webAutocompleteCybergame
            // 
            this.webAutocompleteCybergame.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.webAutocompleteCybergame.Autocompletedata = null;
            this.webAutocompleteCybergame.AutoSize = true;
            this.webAutocompleteCybergame.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.webAutocompleteCybergame.CurrentText = global::Ubiquitous.Properties.Settings.Default.cybergame_Game;
            this.webAutocompleteCybergame.DataBindings.Add(new System.Windows.Forms.Binding("CurrentText", global::Ubiquitous.Properties.Settings.Default, "cybergame_Game", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.webAutocompleteCybergame.Location = new System.Drawing.Point(81, 106);
            this.webAutocompleteCybergame.Margin = new System.Windows.Forms.Padding(0);
            this.webAutocompleteCybergame.Name = "webAutocompleteCybergame";
            this.webAutocompleteCybergame.Size = new System.Drawing.Size(212, 23);
            this.webAutocompleteCybergame.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(43, 112);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(38, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Game:";
            // 
            // textBox3
            // 
            this.textBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox3.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Ubiquitous.Properties.Settings.Default, "cybergame_ShortDescription", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox3.Location = new System.Drawing.Point(84, 20);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(304, 80);
            this.textBox3.TabIndex = 1;
            this.textBox3.Text = global::Ubiquitous.Properties.Settings.Default.cybergame_ShortDescription;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(15, 23);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(63, 13);
            this.label10.TabIndex = 0;
            this.label10.Text = "Description:";
            // 
            // groupBoxGoodgame
            // 
            this.groupBoxGoodgame.Controls.Add(this.textWebSourceGGGame);
            this.groupBoxGoodgame.Controls.Add(this.label8);
            this.groupBoxGoodgame.Controls.Add(this.textBox4);
            this.groupBoxGoodgame.Controls.Add(this.label9);
            this.groupBoxGoodgame.DataBindings.Add(new System.Windows.Forms.Binding("Visible", global::Ubiquitous.Properties.Settings.Default, "goodgameEnabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.groupBoxGoodgame.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxGoodgame.Location = new System.Drawing.Point(0, 265);
            this.groupBoxGoodgame.Name = "groupBoxGoodgame";
            this.groupBoxGoodgame.Size = new System.Drawing.Size(403, 95);
            this.groupBoxGoodgame.TabIndex = 3;
            this.groupBoxGoodgame.TabStop = false;
            this.groupBoxGoodgame.Text = "Goodgame";
            this.groupBoxGoodgame.Visible = global::Ubiquitous.Properties.Settings.Default.goodgameEnabled;
            // 
            // textWebSourceGGGame
            // 
            this.textWebSourceGGGame.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textWebSourceGGGame.Autocompletedata = null;
            this.textWebSourceGGGame.AutoSize = true;
            this.textWebSourceGGGame.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.textWebSourceGGGame.CurrentText = global::Ubiquitous.Properties.Settings.Default.goodgame_Game;
            this.textWebSourceGGGame.DataBindings.Add(new System.Windows.Forms.Binding("CurrentText", global::Ubiquitous.Properties.Settings.Default, "goodgame_Game", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textWebSourceGGGame.Location = new System.Drawing.Point(81, 55);
            this.textWebSourceGGGame.Margin = new System.Windows.Forms.Padding(0);
            this.textWebSourceGGGame.Name = "textWebSourceGGGame";
            this.textWebSourceGGGame.Size = new System.Drawing.Size(212, 23);
            this.textWebSourceGGGame.TabIndex = 3;
            this.textWebSourceGGGame.OnTyping += new System.EventHandler<Ubiquitous.EventArgsString>(this.textWebSourceGGGame_OnTyping);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(43, 61);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(38, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "Game:";
            // 
            // textBox4
            // 
            this.textBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox4.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Ubiquitous.Properties.Settings.Default, "goodgame_ShortDescription", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox4.Location = new System.Drawing.Point(84, 23);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(304, 20);
            this.textBox4.TabIndex = 1;
            this.textBox4.Text = global::Ubiquitous.Properties.Settings.Default.goodgame_ShortDescription;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(48, 23);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(30, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "Title:";
            // 
            // groupBoxSc2tv
            // 
            this.groupBoxSc2tv.Controls.Add(this.textBox2);
            this.groupBoxSc2tv.Controls.Add(this.label6);
            this.groupBoxSc2tv.Controls.Add(this.textWebSourceSc2tvGame);
            this.groupBoxSc2tv.Controls.Add(this.label4);
            this.groupBoxSc2tv.Controls.Add(this.textBox1);
            this.groupBoxSc2tv.Controls.Add(this.label5);
            this.groupBoxSc2tv.DataBindings.Add(new System.Windows.Forms.Binding("Visible", global::Ubiquitous.Properties.Settings.Default, "sc2tvEnabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.groupBoxSc2tv.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxSc2tv.Location = new System.Drawing.Point(0, 82);
            this.groupBoxSc2tv.Name = "groupBoxSc2tv";
            this.groupBoxSc2tv.Size = new System.Drawing.Size(403, 183);
            this.groupBoxSc2tv.TabIndex = 2;
            this.groupBoxSc2tv.TabStop = false;
            this.groupBoxSc2tv.Text = "Sc2tv";
            this.groupBoxSc2tv.Visible = global::Ubiquitous.Properties.Settings.Default.sc2tvEnabled;
            // 
            // textBox2
            // 
            this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox2.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Ubiquitous.Properties.Settings.Default, "sc2tv_LongDescription", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox2.Location = new System.Drawing.Point(84, 49);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox2.Size = new System.Drawing.Size(304, 97);
            this.textBox2.TabIndex = 5;
            this.textBox2.Text = global::Ubiquitous.Properties.Settings.Default.sc2tv_LongDescription;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 52);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "Description:";
            // 
            // textWebSourceSc2tvGame
            // 
            this.textWebSourceSc2tvGame.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textWebSourceSc2tvGame.Autocompletedata = null;
            this.textWebSourceSc2tvGame.AutoSize = true;
            this.textWebSourceSc2tvGame.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.textWebSourceSc2tvGame.CurrentText = global::Ubiquitous.Properties.Settings.Default.sc2tv_Game;
            this.textWebSourceSc2tvGame.DataBindings.Add(new System.Windows.Forms.Binding("CurrentText", global::Ubiquitous.Properties.Settings.Default, "sc2tv_Game", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textWebSourceSc2tvGame.Location = new System.Drawing.Point(81, 149);
            this.textWebSourceSc2tvGame.Margin = new System.Windows.Forms.Padding(0);
            this.textWebSourceSc2tvGame.Name = "textWebSourceSc2tvGame";
            this.textWebSourceSc2tvGame.Size = new System.Drawing.Size(212, 23);
            this.textWebSourceSc2tvGame.TabIndex = 3;
            this.textWebSourceSc2tvGame.OnTyping += new System.EventHandler<Ubiquitous.EventArgsString>(this.textWebSourceSc2tvGame_OnTyping);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(43, 155);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Game:";
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Ubiquitous.Properties.Settings.Default, "sc2tv_ShortDescription", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textBox1.Location = new System.Drawing.Point(84, 23);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(304, 20);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = global::Ubiquitous.Properties.Settings.Default.sc2tv_ShortDescription;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(48, 23);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(30, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Title:";
            // 
            // groupBoxTwitch
            // 
            this.groupBoxTwitch.Controls.Add(this.textWebSourceTwitchGame);
            this.groupBoxTwitch.Controls.Add(this.label3);
            this.groupBoxTwitch.Controls.Add(this.textTwitchDescription);
            this.groupBoxTwitch.Controls.Add(this.label2);
            this.groupBoxTwitch.DataBindings.Add(new System.Windows.Forms.Binding("Visible", global::Ubiquitous.Properties.Settings.Default, "twitchEnabled", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.groupBoxTwitch.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxTwitch.Location = new System.Drawing.Point(0, 0);
            this.groupBoxTwitch.Name = "groupBoxTwitch";
            this.groupBoxTwitch.Size = new System.Drawing.Size(403, 82);
            this.groupBoxTwitch.TabIndex = 1;
            this.groupBoxTwitch.TabStop = false;
            this.groupBoxTwitch.Text = "Twitch.tv";
            this.groupBoxTwitch.Visible = global::Ubiquitous.Properties.Settings.Default.twitchEnabled;
            // 
            // textWebSourceTwitchGame
            // 
            this.textWebSourceTwitchGame.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textWebSourceTwitchGame.Autocompletedata = null;
            this.textWebSourceTwitchGame.AutoSize = true;
            this.textWebSourceTwitchGame.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.textWebSourceTwitchGame.CurrentText = global::Ubiquitous.Properties.Settings.Default.twitch_Game;
            this.textWebSourceTwitchGame.DataBindings.Add(new System.Windows.Forms.Binding("CurrentText", global::Ubiquitous.Properties.Settings.Default, "twitch_Game", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textWebSourceTwitchGame.Location = new System.Drawing.Point(81, 49);
            this.textWebSourceTwitchGame.Margin = new System.Windows.Forms.Padding(0);
            this.textWebSourceTwitchGame.Name = "textWebSourceTwitchGame";
            this.textWebSourceTwitchGame.Size = new System.Drawing.Size(212, 23);
            this.textWebSourceTwitchGame.TabIndex = 3;
            this.textWebSourceTwitchGame.OnTyping += new System.EventHandler<Ubiquitous.EventArgsString>(this.textWebSourceTwitchGame_OnTyping);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(40, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Game:";
            // 
            // textTwitchDescription
            // 
            this.textTwitchDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textTwitchDescription.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Ubiquitous.Properties.Settings.Default, "twitch_ShortDescription", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.textTwitchDescription.Location = new System.Drawing.Point(84, 23);
            this.textTwitchDescription.Name = "textTwitchDescription";
            this.textTwitchDescription.Size = new System.Drawing.Size(304, 20);
            this.textTwitchDescription.TabIndex = 1;
            this.textTwitchDescription.Text = global::Ubiquitous.Properties.Settings.Default.twitch_ShortDescription;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(48, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Title:";
            // 
            // Descriptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(419, 545);
            this.Controls.Add(this.textBoxProfile);
            this.Controls.Add(this.buttonUpdateWeb);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Name = "Descriptions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Profile properties";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Descriptions_FormClosing);
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBoxCybergame.ResumeLayout(false);
            this.groupBoxCybergame.PerformLayout();
            this.groupBoxGoodgame.ResumeLayout(false);
            this.groupBoxGoodgame.PerformLayout();
            this.groupBoxSc2tv.ResumeLayout(false);
            this.groupBoxSc2tv.PerformLayout();
            this.groupBoxTwitch.ResumeLayout(false);
            this.groupBoxTwitch.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonUpdateWeb;
        private System.Windows.Forms.GroupBox groupBoxSc2tv;
        private Controls.WebAutocomplete textWebSourceSc2tvGame;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBoxTwitch;
        private Controls.WebAutocomplete textWebSourceTwitchGame;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textTwitchDescription;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBoxGoodgame;
        private Controls.WebAutocomplete textWebSourceGGGame;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBoxCybergame;
        private Controls.WebAutocomplete webAutocompleteCybergame;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox groupBox1;
        private Controls.WebAutocomplete webAutocompleteGoha;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBoxProfile;
    }
}