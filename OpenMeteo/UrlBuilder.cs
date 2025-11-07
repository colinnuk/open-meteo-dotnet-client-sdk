using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenMeteo
{
    public class UrlBuilder
    {
        private Uri _baseUri = new("https://open-meteo.com");
        private string? _subdomain = null;
        private string _path = string.Empty;
        private string? _apiKey = null;
        private readonly Dictionary<string, string> _parameters = [];
        private readonly Dictionary<string, IEnumerable<string>> _collections = [];

        public UrlBuilder WithBaseUri(Uri baseUri)
        {
            _baseUri = baseUri;
            return this;
        }

        public UrlBuilder WithSubdomain(string? subdomain)
        {
            _subdomain = subdomain;
            return this;
        }

        public UrlBuilder WithPath(string path)
        {
            _path = path.StartsWith('/') ? path : "/" + path;
            return this;
        }

        public UrlBuilder WithApiKey(string apiKey)
        {
            _apiKey = apiKey;
            return this;
        }

        public UrlBuilder AddParameter(string key, string value)
        {
            _parameters[key.ToLower()] = value.ToLower();
            return this;
        }

        public UrlBuilder AddCollection(string key, IEnumerable<string> values)
        {
            _collections[key.ToLower()] = values.Select(v => v.ToLower());
            return this;
        }

        public string Build()
        {
            var builder = new UriBuilder(_baseUri);
            if (!string.IsNullOrEmpty(_subdomain))
            {
                var hostParts = builder.Host.Split('.');
                if (hostParts.Length > 1)
                {
                    builder.Host = _subdomain + "." + string.Join('.', hostParts);
                }
                else
                {
                    builder.Host = _subdomain + "." + builder.Host;
                }
            }

            builder.Path = _path;


            if (!string.IsNullOrEmpty(_apiKey))
            {
                _parameters["apikey"] = _apiKey;
            }

            var query = BuildQueryString(_parameters, _collections);
            builder.Query = query;
            return builder.Uri.ToString();
        }

        private static string BuildQueryString(Dictionary<string, string> parameters, Dictionary<string, IEnumerable<string>> collections)
        {
            var queryParts = new List<string>();
            foreach (var kvp in parameters)
            {
                if (!string.IsNullOrEmpty(kvp.Value))
                    queryParts.Add($"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}");
            }
            foreach (var kvp in collections)
            {
                var joined = string.Join(",", kvp.Value);
                if (!string.IsNullOrEmpty(joined))
                    queryParts.Add($"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(joined)}");
            }
            return string.Join("&", queryParts);
        }
    }
}
