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
    public partial class SizeBox : UserControl
    {
        public SizeBox()
        {
            InitializeComponent();     
        }
        public Size Dimensions
        {
            get { return new Size(textWidth.IntValue,textHeight.IntValue);}
            set { textWidth.IntValue = value.Width; textHeight.IntValue = value.Height; }
        }
    }
}
