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
namespace dotWebClient
{

    public enum ContentType
    {
        UrlEncoded,
        Multipart
    }
    public class CookieAwareWebClient : WebClient
    {
        private readonly CookieContainer m_container = new CookieContainer();
        private WebExceptionStatus _lastWebError;
        private Dictionary<ContentType, string> contentTypes;
        private const string userAgent = "Mozilla/5.0 (Windows NT 6.0; WOW64; rv:14.0) Gecko/20100101 Firefox/14.0.1";
        public bool stillReading = false;
        public CookieAwareWebClient()
        {
            ServicePointManager.DefaultConnectionLimit = 5;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.UseNagleAlgorithm = false;
            contentTypes = new Dictionary<ContentType, string>();
            contentTypes.Add(ContentType.UrlEncoded, "application/x-www-form-urlencoded");
            contentTypes.Add(ContentType.Multipart, "multipart/form-data");
            
        }
        public string LastWebError
        {
            get { return _lastWebError.ToString(); }
        }
        public bool KeepAlive
        {
            get;
            set;

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
        public string PostUrlEncoded( string url, string param )
        {
            byte[] requestData = Encoding.UTF8.GetBytes(param);
            string result = string.Empty;
            try
            {
                HttpWebRequest request = (HttpWebRequest)GetWebRequest(new Uri(url));
                request.Method = "POST";
                request.UserAgent = userAgent;
                request.ContentType = "application/x-www-form-urlencoded";
                //request.CookieContainer = m_container;
                request.ContentLength = requestData.Length;
                request.KeepAlive = true;
                request.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);

                using (Stream stream = request.GetRequestStream())
                    stream.Write(requestData, 0, requestData.Length);

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                      //foreach (Cookie c in response.Cookies)
                      //Debug.Print(c.Name + " = " + c.Value);

                    var reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.GetEncoding("utf-8"));
                    result = reader.ReadToEnd();
                    //Debug.Print(result);
                }

            }
            catch
            {
                Debug.Print(String.Format("WebClient exception in PostUrlEncoded() url: {0}",url));

            }
            return result;
        }
        public void PostMultipart(string url, string sData, string boundary)
        {
            byte[] data = Encoding.GetBytes(sData);
            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "POST";
                request.ContentType = "multipart/form-data; boundary=" + boundary;
                request.UserAgent = userAgent;
                request.CookieContainer = m_container;
                request.ContentLength = data.Length;
                //request.Referer = url;
                request.KeepAlive = true;
                //request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                //request.Headers["Origin"] = "http://sc2tv.ru";
                //request.Headers["Accept-Charset"] = "windows-1251,utf-8;q=0.7,*;q=0.3";
                //request.Headers["Accept-Encoding"] = "gzip,deflate,sdch";


                

                // You could add authentication here as well if needed:
                // request.PreAuthenticate = true;
                // request.AuthenticationLevel = System.Net.Security.AuthenticationLevel.MutualAuthRequested;
                // request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.Default.GetBytes("username" + ":" + "password")));

                // Send the form data to the request.
                using (var requestStream = request.GetRequestStream())
                {
                    requestStream.Write(data, 0, data.Length);
                    requestStream.Close();
                }
            }
            catch (WebException e)
            {
                Debug.Print("Exception while downloading from url: {0}", url);
                _lastWebError = e.Status;
            }
        }

        public System.IO.Stream downloadURL(string url)
        {
            try
            {
                return this.OpenRead(url);
            }
            catch (WebException e)
            {
                Debug.Print("Exception while downloading from url: {0}", url);
                _lastWebError = e.Status;
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
