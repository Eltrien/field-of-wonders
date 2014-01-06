using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
namespace Ubiquitous
{
    public class LogStreamWriter : System.IO.TextWriter
    {
        Log _output = null;
        String[] _maskPasswords;
        public LogStreamWriter(Log _log, String[] maskPasswords)
        {
            _output = _log;
            _maskPasswords = maskPasswords;
        }
        public override void WriteLine(string value)
        {
            if (_maskPasswords != null && !String.IsNullOrEmpty(value))
            {
                foreach (var pass in _maskPasswords)
                {
                    if (!String.IsNullOrEmpty(pass))
                    {
                        value = value.Replace(pass, new String('*', pass.Length));
                    }
                }
            }

            base.WriteLine(value);
            _output.WriteLine(new Ubiquitous.MainForm.UbiMessage( value.ToString()),ChatIcon.Default, false, System.Drawing.Color.White, System.Drawing.Color.Black);
        }
        public override Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }
    }
}
