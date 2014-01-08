using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ubiquitous
{
    public partial class ChatStatus : UserControl
    {
        delegate void SetImageCallback();
        private bool ison = false;

        public ChatStatus()
        {
            InitializeComponent();
        }

        public String Label
        {
            get { return label.Text; }
            set { label.Text = value; }
        }
        public bool On
        {
            get 
            {
                return ison;
            }
            set
            {
                if( ison == value )
                    return;
                ison = value;
                switch (ison)
                {
                    case true:
                        SetOn();
                        break;
                    case false:
                        SetOff();
                        break;
                }
            }
        }

        private void SetOn()
        {
            if (picture.InvokeRequired)
            {
                SetImageCallback d = new SetImageCallback(SetOn);
                picture.Parent.Invoke(d);
            }
            else
            {
                picture.Image = Properties.Resources.checkMarkGreen;
            }
        }
        private void SetOff()
        {
            if (picture.InvokeRequired)
            {
                SetImageCallback d = new SetImageCallback(SetOff);
                picture.Parent.Invoke(d);
            }
            else
            {
                picture.Image = Properties.Resources.checkMarkRed;
            }
        }
        private void ChatStatus_VisibleChanged(object sender, EventArgs e)
        {
           // if (Visible) Dock = DockStyle.Top;
           // else Dock = DockStyle.None;
        }

        private void picture_Click(object sender, EventArgs e)
        {
            this.OnClick(e);
        }

        private void label_Click(object sender, EventArgs e)
        {
            this.OnClick(e);
        }
    }
}
