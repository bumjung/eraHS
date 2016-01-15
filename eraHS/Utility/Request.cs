using System;
using System.IO;
using System.Net;


namespace eraHS.Utility
{
    public static class Request
    {
        private static string _baseAddress = "http://localhost:4455/api/game/";

        // TODO: add content to body, instead of url
        public static void Post(string jsonContent)
        {
            string url = _baseAddress + jsonContent;

            var http = (HttpWebRequest)WebRequest.Create(new Uri(url));
            http.Accept = "application/json";
            http.ContentType = "application/json";
            http.Method = "POST";

            var response = http.GetResponse();

            var stream = response.GetResponseStream();
            var sr = new StreamReader(stream);
            var content = sr.ReadToEnd();
        }
        
    }
}
