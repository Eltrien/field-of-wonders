using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Cache;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using System.Collections;
using System.IO;
using System.Net.Sockets;
using dotUtilities;
namespace dotWebClient
{

    public enum ContentType
    {
        UrlEncoded,
        UrlEncodedUTF8,
        Multipart
    }
    public class CookieAwareWebClient : WebClient
    {
        private readonly CookieContainer m_container = new CookieContainer();
        private WebExceptionStatus _lastWebError;
        private string _lastWebErrorDescription;
        private Dictionary<ContentType, string> contentTypes;
        private HttpWebResponse gResponse;

        private const string userAgent = "Mozilla/5.0 (Windows NT 6.0; WOW64; rv:14.0) Gecko/20100101 Firefox/14.0.1";
        public bool stillReading = false;
        public CookieAwareWebClient()
        {
            
            ServicePointManager.DefaultConnectionLimit = 5;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.UseNagleAlgorithm = false;
            contentTypes = new Dictionary<ContentType, string>();
            contentTypes.Add(ContentType.UrlEncodedUTF8, "application/x-www-form-urlencoded; charset=UTF-8");
            contentTypes.Add(ContentType.UrlEncoded, "application/x-www-form-urlencoded");
            contentTypes.Add(ContentType.Multipart, "multipart/form-data");
            
        }
        public string LastWebError
        {
            get { return _lastWebError.ToString(); }
        }
        public string LastWebErrorDescription
        {
            get { return _lastWebErrorDescription; }
        }
        public bool KeepAlive
        {
            get;
            set;

        }
        public List<KeyValuePair<string, string>> CookiesStrings
        {
            get
            {
                List<KeyValuePair<string, string>> _cookies = new List<KeyValuePair<string, string>>();
                Hashtable table = (Hashtable)m_container.GetType().InvokeMember("m_domainTable",
                                             BindingFlags.NonPublic |
                                             BindingFlags.GetField |
                                             BindingFlags.Instance,
                                             null,
                                             m_container,
                                             new object[] { });
                foreach (var key in table.Keys)
                {
                    var url = String.Format("http://{0}/", key.ToString().TrimStart('.'));

                    foreach (Cookie cookie in m_container.GetCookies(new Uri(url)))
                    {
                        _cookies.Add(new KeyValuePair<string, string>(cookie.Name, cookie.Value));
                    }
                }

                return _cookies;
            }
        }
        public CookieContainer Cookies
        {
            get { return m_container; }
            set
            {
                Hashtable table = (Hashtable)value.GetType().InvokeMember("m_domainTable",
                                                             BindingFlags.NonPublic |
                                                             BindingFlags.GetField |
                                                             BindingFlags.Instance,
                                                             null,
                                                             value,
                                                             new object[] { });
                foreach (var key in table.Keys)
                {
                    var url = String.Format("http://{0}/",key.ToString().TrimStart('.'));

                    foreach (Cookie cookie in value.GetCookies(new Uri(url)))
                    {
                        m_container.Add(cookie);
                    }
                }


            }
        }
        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);

            HttpWebRequest webRequest = request as HttpWebRequest;
            if (webRequest != null)
            {
                if (KeepAlive)
                {
                    webRequest.ProtocolVersion = HttpVersion.Version11;
                    webRequest.KeepAlive = true;
                    var sp = webRequest.ServicePoint;
                    var prop = sp.GetType().GetProperty("HttpBehaviour", BindingFlags.Instance | BindingFlags.NonPublic);
                    prop.SetValue(sp, (byte)0, null);
                }
                webRequest.CookieContainer = m_container;
                webRequest.UserAgent = userAgent;
                webRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            }
            return request;
        }
        public void setCookie(string name, string value, string domain)
        {
            if (name == null || value == null)
                return;
            m_container.Capacity += 1;
            m_container.Add(new Cookie(name, value, "/", domain));
        }
        public bool gotCookies(string name, string url)
        {
            if (String.IsNullOrEmpty(name) || string.IsNullOrEmpty(url) )
                return false;

            if (m_container == null || m_container.Count == 0)
                return false;
            
            
            string value = m_container.GetCookies(new Uri(url))[name].Value;
            return value == null ? false : true;
        }
        public string CookieValue(string name, string url)
        {
            return m_container.GetCookies(new Uri(url))[name].Value;
        }
        public string postFormDataLowLevel(string formActionUrl, string postData)
        {
            var uri = new Uri(formActionUrl);
            IPHostEntry hostEntry = Dns.GetHostEntry(uri.Host);
            IPAddress address = hostEntry.AddressList[0];
            IPEndPoint ipEndpoint = new IPEndPoint(address, 80);
            Socket socket = new Socket(ipEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                socket.Connect(ipEndpoint);
                if (socket.Connected)
                {
                    Debug.Print(String.Format("WebClient postFormDataLowLevel: Connected to {0}", ipEndpoint.ToString()));
                }
                else
                {
                    Debug.Print(String.Format("WebClient postFormDataLowLevel: can't connect to {0}", ipEndpoint.ToString()));
                }
            }
            catch (SocketException ex)
            {
                Debug.Print("WebClient postFormDataLowLevel: socket exception {0}", ex.Message);
            }
            var buffer = Encoding.UTF8.GetBytes(postData);

            String requestString = String.Format("POST {0} HTTP/1.1\r\n" +
                "User-Agent: {4}\r\n" +
                "Accept: text/html\r\n" +
                "Host: {1}\r\n" +
                "Cache-Control: no-store, no-cache\r\n" +
                "Pragma: no-cache\r\n" +
                "Content-Length: {2}\r\n" +
                "Connection: Keep-Alive\r\n" +
                "Content-Type: application/x-www-form-urlencoded\r\n\r\n"
                + "{3}", uri.AbsolutePath, uri.Host, Encoding.UTF8.GetBytes(postData).Length, postData, userAgent);

            byte[] request = Encoding.UTF8.GetBytes(requestString);

            Byte[] bytesReceived = new Byte[1024];
            socket.Send(request, request.Length, 0);
            string result = String.Empty;
            int bytes = 0;
            try
            {
                do
                {
                    bytes = socket.Receive(bytesReceived, bytesReceived.Length, 0);
                    result = result + Encoding.ASCII.GetString(bytesReceived, 0, bytes);
                }
                while (bytes > 0);
            }
            catch { 
            
            }
            bool found = true;
            var tmpResult = result;
            while (found)
            {
                var pair = Re.GetSubString( tmpResult, @"Set-Cookie:\s([^;]+)?;", 1);
                if (String.IsNullOrEmpty(pair))
                {
                    found = false;
                    break;
                }
                var re = @"(.*)?=(.*)";
                var name = Re.GetSubString(pair, re, 1);
                var value = Re.GetSubString(pair, re, 2);
                setCookie(name, value, uri.DnsSafeHost);
                tmpResult = tmpResult.Replace( String.Format(@"Set-Cookie: {0}",name),"");
            }

            return result;
        }
        public string PostMultipart(string url, string sData, string boundary)
        {
            _lastWebError = WebExceptionStatus.Success;
            _lastWebErrorDescription = string.Empty;
            byte[] data = Encoding.GetBytes(sData);
            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "POST";
                request.ContentType = "multipart/form-data; boundary=" + boundary;
                request.UserAgent = userAgent;
                request.CookieContainer = m_container;
                request.ContentLength = data.Length;
                request.KeepAlive = true;
                using (var requestStream = request.GetRequestStream())
                {
                    requestStream.Write(data, 0, data.Length);
                    requestStream.Close();
                }
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream resStream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(resStream, Encoding.UTF8);
                        return reader.ReadToEnd();
                    }
                }

            }
            catch (WebException e)
            {
                Debug.Print("Exception while posting to url: {0}, Error: {1}", url, e.Message);
                _lastWebError = e.Status;
                _lastWebErrorDescription = e.Message;
            }
            return string.Empty;
        }

        public System.IO.Stream downloadURL(string url)
        {
            _lastWebError = WebExceptionStatus.Success;
            _lastWebErrorDescription = string.Empty;

            try
            {
                return this.OpenRead(url);
            }
            catch (WebException e)
            {
                Debug.Print("Exception while downloading from url: {0}, Error: {1}", url, e.Message);
                _lastWebError = e.Status;
                _lastWebErrorDescription = e.Message;
                   
            }
            return null;
        }
        public ContentType ContentType
        {
            set { this.Headers[HttpRequestHeader.ContentType] = contentTypes[value]; }
        }
        
    }
    public class PostData
    {

        private List<PostDataParam> m_Params;

        public List<PostDataParam> Params
        {
            get { return m_Params; }
            set { m_Params = value; }
        }

        public PostData()
        {
            m_Params = new List<PostDataParam>();

        }

        public String Boundary
        {
            get;
            set;
        }
        /// <summary>
        /// Returns the parameters array formatted for multi-part/form data
        /// </summary>
        /// <returns></returns>
        public string GetPostData()
        {
            // Get boundary, default is --AaB03x

            Boundary = "----WebKitFormBoundary" + RandomString(16);

            StringBuilder sb = new StringBuilder();
            foreach (PostDataParam p in m_Params)
            {
                sb.AppendLine("--" + Boundary);

                if (p.Type == PostDataParamType.File)
                {
                    sb.AppendLine(string.Format("Content-Disposition: file; name=\"{0}\"; filename=\"{1}\"", p.Name, p.FileName));
                    sb.AppendLine("Content-Type: text/plain");
                    sb.AppendLine();
                    sb.AppendLine(p.Value);
                }
                else
                {
                    sb.AppendLine(string.Format("Content-Disposition: form-data; name=\"{0}\"", p.Name));
                    sb.AppendLine();
                    sb.AppendLine(p.Value);
                }
            }

            sb.AppendLine(Boundary + "--");

            return sb.ToString();
        }
        private string RandomString(int Size)
        {
            string input = "ABCDEFGHJIKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < Size; i++)
            {
                ch = input[random.Next(0, input.Length)];
                builder.Append(ch);
            }
            return builder.ToString();
        }
    }

    public enum PostDataParamType
    {
        Field,
        File
    }

    public class PostDataParam
    {


        public PostDataParam(string name, string value, PostDataParamType type)
        {
            Name = name;
            Value = value;
            Type = type;
        }

        public string Name;
        public string FileName;
        public string Value;
        public PostDataParamType Type;
    }



}
