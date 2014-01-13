using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Specialized;

namespace Ubiquitous.Controls
{
    public partial class WebAutocomplete : UserControl
    {
        //private BindingSource bindingSource;
        private AutoCompleteStringCollection autocompleteCol;
        public event EventHandler<EventArgsString> OnTyping;
        public WebAutocomplete()
        {

            InitializeComponent();

        }

        public AutoCompleteStringCollection Autocompletedata
        {
            get
            {
                return autocompleteCol; 
            }
            set
            {
                autocompleteCol = value;
                dotUtilities.Utils.SetProperty<TextBox, AutoCompleteStringCollection>(textBox, "AutoCompleteCustomSource", autocompleteCol);
            }
        }
        public String CurrentText
        {
            get { return textBox.Text; }
            set { 
                dotUtilities.Utils.SetProperty<TextBox, String>(textBox, "Text", value); 
            }
        }
        private void textBoxEntry_TextChanged(object sender, EventArgs e)
        {
            if (OnTyping != null)
                OnTyping(this, new EventArgsString(textBox.Text));
        }
        
    }
}
