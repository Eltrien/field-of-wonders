using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace dotUtilities
{
    static public class Browser
    {
        private static WebBrowser wb = new WebBrowser();
        [STAThread]
        public static HtmlDocument ParseContent( string content )
        {
            if (wb == null)
            {
                wb = new WebBrowser();
                wb.Navigate("about:blank");
            }
            
            wb.Document.Write( content );
            
            return wb.Document;

        }
    }
}
