using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;

namespace Ubiquitous
{
    public partial class ColorPicker : UserControl
    {
        private Form parentForm;
        public ColorPicker()
        {
            InitializeComponent();
        }

        private void buttonColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.AllowFullOpen = true;
            
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {                
                SelectedColor = colorDialog.Color;
            }
        }
        [UserScopedSetting()]
        public Color SelectedColor
        {
            get { return buttonColor.BackColor; }
            set
            {
                buttonColor.BackColor = value;
            }
        }

        public String Caption
        {
            set { label33.Text = value; }
            get { return label33.Text; }
        }
    }
}
