using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Drawing;

namespace dotUtilities
{
    	public enum RtfColor {
		    Black, Maroon, Green, Olive, Navy, Purple, Teal, Gray, Silver,
		    Red, Lime, Yellow, Blue, Fuchsia, Aqua, White
	    }
    public static class ColorUtils
    {
        

        public static String GetRtfColor( Color color )
        {
            string mask = @"\red{0}\green{1}\blue{2}";

            return String.Format(mask,color.R,color.G,color.B);
        }
    }
}
