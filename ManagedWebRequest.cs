using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace ManagedWebRequest
{
    /// <summary>
    /// Main managed request object containing all methods and parameters.
    /// </summary>
    public class Request
    {
        private string hostname = "";
        private List<string> TextRequest = new List<string>();
        private string method = "";
        private string path = "";
        private List<Tuple<string, string>> Headers = new List<Tuple<string, string>>();
        private List<Tuple<string, string>> Parameters = new List<Tuple<string, string>>();
        private List<Tuple<string, string>> PostParameters = null;
        private string PostContent = "";
        private bool PostData = false;
        private static JsonDocument JSON = null;
        private XDocument XML = null;
        private WebProxy Proxy = null;
        private HttpWebRequest ParsedRequest;

        /// <summary>
        /// Main constuctor method. Contains all methods for performing basic math functions. Creates a Request object. Takes a filname containing a HTTP request, a string contining the hostname
        ///  an optional bool indicating of bad certs should be allowable and an optional bool indicating whether redirects should be automatically followed.
        /// </summary>
        public Request(string filename, string hostname, bool allowBadCerts = true, bool followRedirects = true)
        {
            if (!File.Exists(filename))
            {
                throw new Exception("File does not exist.");
            }

            foreach(string s in File.ReadLines(filename)){
                TextRequest.Add(s);
            }

            ParsedRequest = (HttpWebRequest)WebRequest.Create(hostname);

            if(allowBadCerts == true)
            {
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            }
            
            if(followRedirects == true)
            {
                ParsedRequest.AllowAutoRedirect = true;
            }
            else
            {
                ParsedRequest.AllowAutoRedirect = false;
            }
            
            ParseRequestFile();
        }

        /// <summary>
        /// Gets the hostname of the current httpWebRequest object.
        /// </summary>
        /// <returns>A string containing the hostname</returns>
        public string GetHostname()
        {
            return hostname;
        }

        /// <summary>
        /// Sets the hostname of the current httpWebRequest object
        /// </summary>
        /// <param name="uri">String containing the hostname to be used.</param>
        public void SetHostname(string uri)
        {
            hostname = uri;
        }

        /// <summary>
        /// Gets the TextRequest object. TextRequest contains the current web request to be executed in string format.
        /// </summary>
        /// <returns>Returns a list of strings.</returns>
        public List<string> GetTextRequest()
        {
            return TextRequest;
        }

        /// <summary>
        /// Sets the TextRequest object.
        /// </summary>
        /// <param name="textRequest">A list of strings containing a web request.</param>
        public void SetTextRequest(List<string> textRequest)
        {
            TextRequest = textRequest;
        }

        /// <summary>
        /// Gets the url parameters of the current httpWebRequest object.
        /// </summary>
        /// <returns>Returns a List of tuples containing key/value pairs of the url parameters</returns>
        public List<Tuple<string, string>> GetParameters()
        {
            return Parameters;
        }

        /// <summary>
        /// Sets the url parameters for the current httpWebRequest object.
        /// </summary>
        /// <param name="parameters">Takes a list of tuples containing key value pairs (string, string) for parameters</param>
        public void SetParameters(List<Tuple<string, string>> parameters)
        {
            Parameters = parameters;
        }

        /// <summary>
        /// Gets the headers for the current httpWebRequest object.
        /// </summary>
        /// <returns>Returns a list of tuples containing key value pairs (string, string)</returns>
        public List<Tuple<string, string>> GetHeaders()
        {
            return Headers;
        }

        /// <summary>
        /// Sets the headers for the current httpWebRequest object.
        /// </summary>
        /// <param name="headers">Takes a list of tuples containing key value pairs (string, string)</param>
        public void SetHeaders(List<Tuple<string, string>> headers)
        {
            Headers = headers;
        }

        /// <summary>
        /// Returns a string containing the raw post content.
        /// </summary>
        /// <returns>String</returns>
        public string GetPostContent()
        {
            return PostContent;
        }

        /// <summary>
        /// Sets the post content to be used with the current httpWebRequest object.
        /// </summary>
        /// <param name="postContent">String containing post data</param>
        public void SetPostContent(string postContent)
        {
            PostContent = postContent;
        }

        /// <summary>
        /// Gets the post parameters for the current httpWebRequest object.
        /// </summary>
        /// <returns>Returns a list of tuples containing key value pairs (string, string)</returns>
        public List<Tuple<string, string>> GetPostParameters()
        {
            return PostParameters;
        }

        /// <summary>
        /// Sets the post parameters for the current httpWebRequest object.
        /// </summary>
        /// <param name="postParameters">Takes a list of tuples containing key value pairs (string, string)</param>
        public void SetPostParameters(List<Tuple<string, string>> postParameters)
        {
            PostParameters = postParameters;
        }

        /// <summary>
        /// Gets the JSON data associated with the current httpWebRequest object.
        /// </summary>
        /// <returns>Retruns a JsonDocument if available. Returns null otherwise</returns>
        public JsonDocument GetJSON()
        {
            return JSON;
        }

        /// <summary>
        /// Sets the JSON data to be used with the current httpWebRequest object.
        /// </summary>
        /// <param name="json">JsonDocument</param>
        public void SetJSON(JsonDocument json)
        {
            JSON = json;
        }

        /// <summary>
        /// Gets the XML document associated with the current httpWebRequest object.
        /// </summary>
        /// <returns>Retruns a XDocument if available. Returns null otherwise</returns>
        public XDocument GetXML()
        {
            return XML;
        }

        /// <summary>
        /// Sets the XML document to be used with the current httpWebRequest object.
        /// </summary>
        /// <param name="xml">XDocument</param>
        public void SetXML(XDocument xml)
        {
            XML = xml;
        }

        /// <summary>
        /// Returns JSON data of current httpWebRequest as string.
        /// </summary>
        /// <returns>Returns a string if JSON data is present. Returns an empty string if no JSON data is present.</returns>
        public string GetJSONString()
        {
            using (var stream = new MemoryStream())
            {
                try
                {
                    Utf8JsonWriter writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });
                    JSON.WriteTo(writer);
                    writer.Flush();
                    string json = Encoding.UTF8.GetString(stream.ToArray());
                    return json;
                }
                catch
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Returns XML data of current httpWebRequest as string.
        /// </summary>
        /// <returns>Returns a string if XML data is present. Returns an empty string if no JSON data is present.</returns>
        public string GetXMLString()
        {
            try
            {
                return XML.ToString();
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Gets information about the current proxy settings.
        /// </summary>
        /// <returns>Returns a WebProxy object</returns>
        public WebProxy GetProxy()
        {
            return Proxy;
        }

        /// <summary>
        /// Set proxy settings. Takes a url with port and protocol specified e.g. "http://127.0.0.1:8080"
        /// </summary>
        /// <param name="proxyAddress">String containing protocol, host and port</param>
        public void SetProxy(string proxyAddress)
        {
            try
            {
                Proxy = new WebProxy();
                Proxy.Address = new Uri(proxyAddress);
            }
            catch (Exception)
            {
                //Do something
            }
        }

        /// <summary>
        /// Sets the WebProxy object to be used with the current httpWebRequest
        /// </summary>
        /// <param name="proxy">WebProxy object</param>
        public void SetProxy(WebProxy proxy)
        {
            Proxy = proxy;
        }

        private static bool IsJsonValid(string json)
        {
            try {
                JSON = JsonDocument.Parse(json);
                return true;
            } 
            catch { }

            return false;
        }

        private bool IsValidXml(string xml)
        {
            try
            {
                XML = XDocument.Parse(xml);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public HttpWebResponse ExecuteRequest()
        {
            if(Proxy != null)
            {
                ParsedRequest.Proxy = Proxy;
            }
            HttpWebResponse response = (HttpWebResponse)ParsedRequest.GetResponse();
            return response;
        }

        private void ParseRequestFile()
        {
            var requestType = TextRequest[0].Split(' ');
            method = requestType[0];
            path = requestType[1];
            string[] urlParams = path.Split(new[] { '?' }, 2);
            if(urlParams.Length > 1)
            {
                string[] splitUrlParams = urlParams[1].Split('&');
                foreach (string s in splitUrlParams)
                {
                    string[] paramPair = s.Split("=");
                    Parameters.Add(new Tuple<string, string>(paramPair[0], paramPair[1]));
                }
            }
            
            TextRequest.RemoveAt(0);

            List<string> tempTextRequest = new List<string>();
            foreach(string s in TextRequest)
            {
                tempTextRequest.Add(s);
            }

            bool body = false;
            int counter = 0;
            List<int> RemoveElements = new List<int>();

            foreach(string s in TextRequest)
            {
                if (s.Contains(':') && body == false)
                {
                    string header = s.Trim();
                    var splitHeader = header.Split(new[] { ':' }, 2);
                    Headers.Add(new Tuple<string, string>(splitHeader[0], splitHeader[1]));
                    RemoveElements.Add(counter);
                    counter++;
                }
                else
                {
                    body = true;
                }
            }

            RemoveElements.Sort();
            RemoveElements.Reverse();
            foreach(int i in RemoveElements)
            {
                tempTextRequest.RemoveAt(i);
            }

            tempTextRequest = tempTextRequest.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

            string post = String.Join("\n", tempTextRequest.ToArray());
            post = post.Trim();
            post = post.TrimStart('\n');
            PostContent = post;

            if(post.Length > 0)
            {

                PostData = true;
                IsJsonValid(post);
                IsValidXml(post);
                if (JSON == null && XML == null)
                {
                    int newlines = post.Split('\n').Length - 1;
                    if(newlines == 0)
                    {
                        if (post.Contains("&"))
                        {
                            PostParameters = new List<Tuple<string, string>>();

                            string[] splitPostParams = post.Split('&');
                            foreach (string s in splitPostParams)
                            {
                                if (s.Contains("="))
                                {
                                    string[] paramPair = s.Split("=");
                                    PostParameters.Add(new Tuple<string, string>(paramPair[0], paramPair[1]));
                                }
                            }
                        }
                    }
                }
            }
        }

        private void BuildWebRequest()
        {
            string urlParameters = "";
            foreach(Tuple<string, string> t in Parameters)
            {
                urlParameters += t.Item1 + "=" + t.Item2 + "&";
            }
            urlParameters = urlParameters.TrimEnd('&');

            ParsedRequest = (HttpWebRequest)WebRequest.Create(hostname + urlParameters);
            ParsedRequest.Method = method;
            
            foreach(Tuple<string, string> t in Headers)
            {
                ParsedRequest.Headers.Add(t.Item1, t.Item2);
            }

            if(PostData == true)
            {
                byte[] byteArray = null;

                if (JSON != null)
                {
                    byteArray = Encoding.UTF8.GetBytes(GetJSONString());
                }
                else if (XML != null)
                {
                    byteArray = Encoding.UTF8.GetBytes(GetXMLString());
                }
                else if (PostContent.Length > 1)
                {
                    byteArray = Encoding.UTF8.GetBytes(PostContent);
                }

                Stream dataStream = ParsedRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
            }

            //Need to add code to set client cert.
            //Need to add code to set cookies.
            //Need to add code to output response to file.
        }
    }
}
