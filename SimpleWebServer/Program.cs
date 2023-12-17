using System.Net;
using System.Text;

namespace SimpleWebServer
{
    class Program
    {
        static void Main(string[] args)
        {
            WebServer ws = new WebServer(SendResponse, "http://+:8888/");
            ws.AddHandler(SendRootResponse, "/");
            ws.AddHandler(SendMartinResponse, "/martin");
            ws.Run();
            Console.WriteLine("A simple webserver. Press a key to quit.");
            Console.ReadKey();
            ws.Stop();
        }

        public static string SendResponse(HttpListenerRequest request)
        {
            string[] paths = Convert.ToString(request.Url.LocalPath).Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            return string.Format("<HTML><BODY>My web page.<br>{1}<br>{0}</BODY></HTML>", DateTime.Now, string.Join(" - ", paths));
        }

        public static string SendRootResponse(HttpListenerRequest request)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<HTML><BODY>");

            sb.Append("<h1>Menu</h1>");
            sb.Append(@"<a href=""/martin"">Martin</a>");

            sb.Append("</BODY></HTML>");

            return sb.ToString();
        }

        public static string SendMartinResponse(HttpListenerRequest request)
        {
            return string.Format("<HTML><BODY>Martins web page response.<br>{0}</BODY></HTML>", DateTime.Now);
        }
    }
}
