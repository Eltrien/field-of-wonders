using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using dotUtilities;
using System.Windows.Forms;

namespace Ubiquitous
{
    public partial class CounterPanel : UserControl
    {
        private Point _Offset = Point.Empty;
        public CounterPanel()
        {
            InitializeComponent();
        }

        private void labelViewers_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                _Offset = new Point(e.X, e.Y);
                Cursor = Cursors.SizeAll;
            }
        }
        public override string Text
        {
            get
            {
                return labelViewers.Text;
            }
            set
            {
                Utils.SetProperty<Label, String>(labelViewers, "Text", value);
            }
        }
        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value;
                labelViewers.Font = value;
                labelViewers.FlatStyle = FlatStyle.System;
                labelViewers.AutoSize = false;
                labelViewers.Margin = new Padding(0, 0, 0, 0);
                labelViewers.Padding = new Padding(0, 0, 0, 0);

               
            }
        }
        public override Color BackColor
        {

            get { return base.BackColor; }
            set
            {
                base.BackColor = value;
                labelViewers.BackColor = value;
                pictureBox1.BackColor = value;
            }
        }
        public override Color ForeColor
        {
            get
            {
                 return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
                labelViewers.ForeColor = value;                
            }
        }
        public String Counter
        {
            get { return labelViewers.Text; }
            set {
                Utils.SetProperty<Label, String>(labelViewers, "Text", value);
                }
        }
        public Image Image
        {
            get { return pictureBox1.Image; }
            set { pictureBox1.Image = value; }
        }
        private void labelViewers_MouseMove(object sender, MouseEventArgs e)
        {
            if (_Offset != Point.Empty)
            {
                Point newlocation = this.Location;

                var newX = newlocation.X + e.X - _Offset.X;
                var newY = newlocation.Y + e.Y - _Offset.Y;
                var maxX = this.Parent.ClientRectangle.Width;
                var maxY = this.Parent.ClientRectangle.Height;

                if (newX < maxX && newX >= 0)
                    newlocation.X += e.X - _Offset.X;
                else if (newX > maxX)
                    newlocation.X = maxX;
                else if (newX < 0)
                    newlocation.X = 0;

                if (newY >= 0 && newY < maxY)
                    newlocation.Y += e.Y - _Offset.Y;
                else if (newY > maxY)
                    newlocation.Y = maxY;
                else if (newY < 0)
                    newlocation.Y = 0;

                this.Location = newlocation;
            }
        }

        private void labelViewers_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                _Offset = Point.Empty;
                Cursor = Cursors.Default;
            }
        }
    }
}
