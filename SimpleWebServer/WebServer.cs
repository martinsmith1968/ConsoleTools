using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SimpleWebServer
{
    /// <summary>
    /// WebServer class
    /// </summary>
    /// <remarks>
    /// Ripped from: http://www.codehosting.net/blog/BlogEngine/post/Simple-C-Web-Server.aspx
    /// Added handler methods to simplify request parsing in default handler
    /// </remarks>
    public class WebServer
    {
        private readonly HttpListener _listener = new HttpListener();
        private readonly Func<HttpListenerRequest, string> _defaultHandlerMethod;

        private Dictionary<string, Func<HttpListenerRequest, string>> _handlerMethods = new Dictionary<string, Func<HttpListenerRequest, string>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="WebServer"/> class.
        /// </summary>
        /// <param name="prefixes">The prefixes.</param>
        /// <param name="method">The method.</param>
        public WebServer(Func<HttpListenerRequest, string> method, params string[] prefixes)
        {
            if (!HttpListener.IsSupported)
                throw new NotSupportedException(
                    "Needs Windows XP SP2, Server 2003 or later.");

            // URI prefixes are required, for example
            // "http://localhost:8080/index/".
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");

            // A responder method is required
            if (method == null)
                throw new ArgumentException("method");

            foreach (string s in prefixes)
                _listener.Prefixes.Add(s);

            _defaultHandlerMethod = method;
            _listener.Start();
        }

        /// <summary>
        /// Adds the handler.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="requests">The requests.</param>
        public void AddHandler(Func<HttpListenerRequest, string> method, params string[] requestMatches)
        {
            if (method == null)
                throw new ArgumentNullException("method");
            if (requestMatches == null || requestMatches.Length == 0)
                throw new ArgumentNullException("requests");

            foreach (string requestMatch in requestMatches)
            {
                this._handlerMethods.Add(requestMatch, method);
            }
        }

        /// <summary>
        /// Gets the best matched handler for the request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>A Handler method</returns>
        private Func<HttpListenerRequest, string> GetHandler(HttpListenerRequest request)
        {
            // QAD way of finding best matched handler
            var keys = this._handlerMethods.Keys.ToList();
            keys.Sort();
            keys.Reverse();

            foreach (string requestMatch in keys)
            {
                if (Regex.IsMatch(request.Url.PathAndQuery, requestMatch))
                {
                    return this._handlerMethods[requestMatch];
                }
            }

            return this._defaultHandlerMethod;
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        public void Run()
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                Console.WriteLine("Webserver running...");
                try
                {
                    while (_listener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem((c) =>
                        {
                            var ctx = c as HttpListenerContext;
                            try
                            {
                                Func<HttpListenerRequest, string> method = GetHandler(ctx.Request);
                                string rstr = method(ctx.Request);
                                WriteResponse(ctx.Response, rstr);
                            }
                            catch (Exception ex)
                            {
                                WriteResponse(ctx.Response, string.Format("Handler Exception ({0}): {1}", ex.GetType().Name, ex.Message));
                            }
                            finally
                            {
                                // always close the stream
                                ctx.Response.OutputStream.Close();
                            }
                        }, _listener.GetContext());
                    }
                }
                catch { } // suppress any exceptions
            });
        }

        /// <summary>
        /// Writes the response.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="text">The text.</param>
        private void WriteResponse(HttpListenerResponse response, string text)
        {
            // Deal with nulls
            text = Convert.ToString(text);

            byte[] buf = Encoding.UTF8.GetBytes(text);

            response.ContentLength64 = buf.Length;
            response.OutputStream.Write(buf, 0, buf.Length);
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }
    }
}
