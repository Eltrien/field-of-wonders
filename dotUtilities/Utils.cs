using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Reflection;

namespace dotUtilities
{
    public static class Utils
    {
        #region Constants
        private const int WM_NCHITTEST = 0x84;
        private const int HTTRANSPARENT = -1;
        #endregion

        public delegate void SetPropCallback(Control ctrl, string propName, object value);
        public delegate void SetPropCallback2(UserControl ctrl, string propName, object value);
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
        public static void SetProperty<TControl, TValue>(this TControl ctrl, string propName, TValue value) where TControl : Control
        {
            if (ctrl.InvokeRequired)
            {
                var d = new SetPropCallback(SetProperty);
                ctrl.Invoke(d, new object[] { ctrl, propName, value });
            }
            else
            {
                Type t = ctrl.GetType();
                t.InvokeMember(propName, BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.Public, null, ctrl, new object[] { value });
            }
        }

        public static void SetProperty2<TControl, TValue>(this TControl ctrl, string propName, TValue value) where TControl : UserControl
        {
            if (ctrl.InvokeRequired)
            {
                var d = new SetPropCallback2(SetProperty2);
                ctrl.Invoke(d, new object[] { ctrl, propName, value });
            }
            else
            {
                Type t = ctrl.GetType();
                t.InvokeMember(propName, BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.Public, null, ctrl, new object[] { value });
            }
        }


    }
}
