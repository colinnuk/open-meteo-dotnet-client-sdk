using System;
using System.Net.Http;

namespace OpenMeteo
{
    /// <summary>
    /// This class automatically sets the header 'Content-Type" and performs all of the api calls
    /// </summary>
    internal class HttpController
    {
        public HttpClient Client { get { return _httpClient; } }
        private readonly HttpClient _httpClient;

        public HttpController()
        {
            var socketHttpHandler = new SocketsHttpHandler()
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(4),
            };
            _httpClient = new HttpClient(socketHttpHandler);

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json")
                );
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("om-dotnet");

        }
    }
}
