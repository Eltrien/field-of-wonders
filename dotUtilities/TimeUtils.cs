using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dotUtilities
{
    public static class TimeUtils
    {
        
        public static string UnixTimestamp()
        {

            DateTime nx = new DateTime(1970,1,1); 
            TimeSpan ts = DateTime.UtcNow - nx; 

            return  ((int)ts.TotalSeconds ).ToString();

        }
    }
}
