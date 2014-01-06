using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Ubiquitous
{
    public class IntTextBox:TextBox
    {
            public int IntValue
            {
                get
                {
                    int ret = 0;
                    int.TryParse(this.Text, out ret);
                    return ret;
                }
                set
                {
                    this.Text = value.ToString();
                }
            }
    }
}
