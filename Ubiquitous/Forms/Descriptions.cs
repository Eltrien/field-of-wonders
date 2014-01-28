using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using dotInterfaces;
using System.Threading;
using Ubiquitous.Controls;

namespace Ubiquitous.Forms
{
    public partial class Descriptions : Form
    {
        private MainForm parentForm;
        private dotTwitchTV.TwitchWeb twitchWeb;
        private dotSC2TV.Sc2Chat sc2tv;
        private dotGoodgame.Goodgame ggChat;
        private dotCybergame.Cybergame cybergame;
        private dotGohaTV.GohaTV gohaweb;
        private Properties.Settings _settings;

        private object lockTwitchCol = new object();
        private AutoCompleteStringCollection twitchCol, sc2tvCol, ggCol,cyberCol;
        public Descriptions(MainForm mainform, object settings)
        {
            InitializeComponent();
            _settings = (Properties.Settings)settings;
            parentForm = mainform;
            twitchWeb = parentForm.twitchWeb;
            sc2tv = parentForm.sc2tv;
            cybergame = parentForm.cybergame;
            ggChat = parentForm.ggChat;
            gohaweb = parentForm.gohaTVstream;

            if (sc2tv != null && sc2tv.LoggedIn)
            {
                sc2tvCol = new AutoCompleteStringCollection();
                sc2tvCol.AddRange(sc2tv.GameList.Select(v => v.Value).ToArray());
                textWebSourceSc2tvGame.Autocompletedata = sc2tvCol;
            }
            if (ggChat != null && ggChat.isLoggedIn)
            {
                ggCol = new AutoCompleteStringCollection();
                textWebSourceGGGame.Autocompletedata = ggCol;
                ggCol.AddRange(ggChat.GameList.Select(v => v.Value).ToArray());
            }
            if (twitchWeb != null)
            {
                twitchCol = new AutoCompleteStringCollection();
                twitchCol.AddRange(new string[]{});
                textWebSourceTwitchGame.Autocompletedata = twitchCol;
                
            }
            if (cybergame != null && cybergame.isLoggedIn)
            {
                cyberCol = new AutoCompleteStringCollection();
                cyberCol.AddRange(cybergame.GameList.ToList().Select(v => v.Value).ToArray());
                webAutocompleteCybergame.Autocompletedata = cyberCol;
            }
            textBoxProfile.Text = _settings.currentProfile;

        }
        private void buttonUpdateWeb_Click(object sender, EventArgs e)
        {
            IChatDescription[] channels = {parentForm.twitchWeb, 
                                              parentForm.sc2tv, 
                                              parentForm.ggChat,
                                              parentForm.cybergame,
                                              parentForm.gohaTVstream
                                          };

            parentForm.CopyChannelDescriptions();

            channels.Where(ch => ch!=null).ToList().ForEach(c => 
                ThreadPool.QueueUserWorkItem( f => c.SetDescription())
            );
        }

        private void textWebSourceTwitchGame_OnTyping(object sender, EventArgsString e)
        {
            if (twitchWeb == null || twitchCol == null )
                return;

            if (!twitchWeb.Game.Any())
                return;

            Debug.Print(e.Text);
            var filter = twitchWeb.GameList.Where( v => v.Value.Equals( e.Text, StringComparison.CurrentCultureIgnoreCase ));
            if (filter.Any())
            {
                textWebSourceTwitchGame.CurrentText = filter.FirstOrDefault().Value;
                return;
            }

            if (e.Text.Length >= 3 &&
                twitchWeb.GameList.Count(v => v.Value.Equals(e.Text, StringComparison.CurrentCultureIgnoreCase)) == 0)
            {
                twitchWeb.SearchGame(e.Text);

                var tmplist = twitchWeb.GameList.ToList();
                twitchCol.Clear();
                twitchCol.AddRange(tmplist.Select(v => v.Value).ToArray());

            }
        }

        private void textWebSourceSc2tvGame_OnTyping(object sender, EventArgsString e)
        {
            if (sc2tv == null)
                return;

            var gameTitle = sc2tv.GameList.Where(v => v.Value.Equals(e.Text, StringComparison.CurrentCultureIgnoreCase)).Select(g => g.Value).FirstOrDefault();
            if (!String.IsNullOrEmpty(gameTitle))
                textWebSourceSc2tvGame.CurrentText = gameTitle;
        }

        private void textWebSourceGGGame_OnTyping(object sender, EventArgsString e)
        {
            if (ggChat == null || ggCol == null)
                return;
            var gameTitle = ggChat.GameList.Where(v => v.Value.Equals(e.Text, StringComparison.CurrentCultureIgnoreCase)).Select(g => g.Value).FirstOrDefault();
            if (!String.IsNullOrEmpty(gameTitle))
                textWebSourceGGGame.CurrentText = gameTitle;
            else
            {
                ggChat.SearchGame(e.Text);
                ggCol.Clear();
                ggCol.AddRange(ggChat.GameList.ToList().Select(v => v.Value).ToArray());
            }



        }

        private void Descriptions_FormClosing(object sender, FormClosingEventArgs e)
        {
            int i = 1;

            String newName = textBoxProfile.Text;
            ChatProfile curProfile = _settings.chatProfiles.Profiles.FirstOrDefault(p => p.Name.Equals(_settings.currentProfile));
            if (curProfile == null)
                return;

            while (_settings.chatProfiles.Profiles.Any(p => !curProfile.Equals(p) && p.Name.Equals(newName, StringComparison.CurrentCultureIgnoreCase) ))
            {
                newName = String.Format("{0} #{1}", textBoxProfile.Text, i++);
            }
            curProfile.Name = newName;
            _settings.currentProfile = newName;
        }

    }
}
