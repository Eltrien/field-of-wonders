using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace dotUtilities
{
    public static class Utils
    {
        public static T CloneObject<T>(T obj)
        {
            using (var memStream = new MemoryStream())
            {
                var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formatter.Serialize(memStream, obj);
                memStream.Position = 0;
                return (T)formatter.Deserialize(memStream);
            }
        }


    }
}
