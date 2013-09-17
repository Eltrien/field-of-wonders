using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using dotTwitchTV;
using dotUtilities;

namespace dotWebServer
{
    public class ChatLine
    {
        public string image;
        public string text;
        public string from;
        public string to;
        public string time;
        public string fromToSeparator;
        public string chatId;
        public bool highlight;

        public string ParseTemplate(string content)
        {
            //<!--ICON--><!--TEXT--><!--FROM--><!--FROM_TO_SEPARATOR--><!--TO--><!--CHAT_ID-->
            content = content.Replace("<!--ICON-->", image);
            if( chatId == "TwitchTV" )
                content = content.Replace("<!--TEXT-->", ReplaceSmiles(text));
            else
                content = content.Replace("<!--TEXT-->", text);

            content = content.Replace("<!--FROM-->", from);
            content = content.Replace("<!--FROM_TO_SEPARATOR-->", fromToSeparator);
            content = content.Replace("<!--TO-->", to);
            content = content.Replace("<!--CHAT_ID-->", chatId);
            content = content.Replace("<!--HIGHLIGHT-->", highlight ? " personalmsg" : "");
            return content;

        }

        private string ReplaceSmiles(string content)
        {
            var regex = new Regex(@"[\s\t\r\n]");
            var words = regex.Split(content).Where(x => !string.IsNullOrEmpty(x));
            foreach (var w in words)
            {
                Bitmap smile = TwitchSmile.Smile(w);
                if( smile != null )
                {
                    content = content.Replace(w, Base64Image.ToBase64ImageTag(smile, ImageFormat.Png));
                }
            }
            return content;
        }
    }

    public class WebChat : HttpServer
    {
        const string webFolder = @"web\";
        const int lineNumber = 500;
        public WebChat(int port)
            : base(port)
        {
            Messages = new List<ChatLine>();
            ThreadPool.QueueUserWorkItem(f => this.listen());
        }
        public void AddMessage( string image, string text, string from, string to, string time, string fromToSeparator, string chatId, bool highlight = false)
        {
            if (Messages.Count >= lineNumber)
            {
                Messages.RemoveAt(Messages.Count - 1);
            }
            Messages.Insert(0, 
                new ChatLine { 
                    image = image,
                    text = text,
                    from = from, 
                    to = to, 
                    time = time, 
                    fromToSeparator = fromToSeparator, 
                    chatId = chatId,
                    highlight = highlight}
            );
        }
        private List<ChatLine> Messages
        {
            get;
            set;
        }
        public readonly HashSet<KeyValuePair<string,string>> ContentTypes = new HashSet<KeyValuePair<string,string>> {
                new KeyValuePair<string,string>(".png", "image/png"),
                new KeyValuePair<string,string>(".gif", "image/gif"),
                new KeyValuePair<string,string>(".jpg", "image/jpeg"),
                new KeyValuePair<string,string>(".jpeg", "image/jpeg"),
                new KeyValuePair<string,string>(".bmp", "image/bmp"),
                new KeyValuePair<string,string>(".js", "application/javascript"),
                new KeyValuePair<string,string>(".html", "text/html"),
                new KeyValuePair<string,string>(".htm", "text/html"),
                new KeyValuePair<string,string>(".css", "text/css")
        
        };
        public override void handleGETRequest(HttpProcessor p)
        {
            var msgLoopRe = "^(.*)<!--.*MESSAGELOOP_BEGIN[^>]*-->(.*)<!--.*MESSAGELOOP_END[^>]*-->(.*)$";

            var requestUrl = new string( p.http_url.TakeWhile(c => c != '?').ToArray() );

            if (requestUrl.Equals("/"))
                requestUrl = "index.htm";

            var contentType = ContentTypes.Where(kvp => requestUrl.ToLower().Contains(kvp.Key)).Select( item => item.Value ).FirstOrDefault();

            if (!String.IsNullOrEmpty(contentType))
            {
                var path = webFolder + requestUrl.Replace("/", @"\");
                p.outputStream.AutoFlush = true;
                p.writeSuccess(contentType);
                if (contentType == "text/html")
                {
                    var content = File.ReadAllText(path);
                    if (content.Contains("MESSAGELOOP_BEGIN"))
                    {
                        var header = Re.GetSubString(content, msgLoopRe, 1);
                        var footer = Re.GetSubString(content, msgLoopRe, 3);
                        var loopblock = Re.GetSubString(content, msgLoopRe, 2);
                        var loopcontent = "";
                        foreach (var line in Messages)
                        {
                            loopcontent += line.ParseTemplate(loopblock);
                        }
                        p.outputStream.Write(header + loopcontent + footer);
                    }
                    else
                    {
                        using (Stream fs = File.Open(path, FileMode.Open))
                        {
                            fs.CopyTo(p.outputStream.BaseStream);
                        }                        
                    }
                }
                else
                {
                    using (Stream fs = File.Open(path, FileMode.Open))
                    {
                        fs.CopyTo(p.outputStream.BaseStream);
                    }
                }
            }
            Debug.Print ("request: {0}", p.http_url);
        }
        public override void handlePOSTRequest(HttpProcessor p, StreamReader inputData)
        {
            Debug.Print("POST request: {0}", p.http_url);
            //string data = inputData.ReadToEnd();

            //p.writeSuccess();
            //p.outputStream.WriteLine("<html></html>");
            //p.outputStream.WriteLine("<a href=/test>return</a><p>");
            //p.outputStream.WriteLine("postbody: <pre>{0}</pre>", data);
        }

    }

    public static class Base64Image
    {
        public static string ToBase64String(this Bitmap bmp, ImageFormat imageFormat)
        {
            string base64String = string.Empty;


            MemoryStream memoryStream = new MemoryStream();
            bmp.Save(memoryStream, imageFormat);


            memoryStream.Position = 0;
            byte[] byteBuffer = memoryStream.ToArray();


            memoryStream.Close();


            base64String = Convert.ToBase64String(byteBuffer);
            byteBuffer = null;


            return base64String;
        }
        public static string ToBase64ImageTag(this Bitmap bmp, ImageFormat imageFormat)
        {
            string imgTag = string.Empty;
            string base64String = string.Empty;


            base64String = ToBase64String(bmp, imageFormat);


            imgTag = "<img src=\"data:image/" + imageFormat.ToString() + ";base64,";
            imgTag += base64String + "\" ";
            imgTag += "width=\"" + bmp.Width.ToString() + "\" ";
            imgTag += "height=\"" + bmp.Height.ToString() + "\" />";


            return imgTag;
        }
    }
}
