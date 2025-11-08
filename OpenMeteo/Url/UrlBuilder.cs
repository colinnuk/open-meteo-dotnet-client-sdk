using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenMeteo.Url
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

        public UrlBuilder AddParameter(string key, string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return this;
            _parameters[key.ToLower()] = value;
            return this;
        }

        public UrlBuilder AddCollection(string key, IEnumerable<string> values)
        {
            if (!values.Any()) return this;
            _collections[key.ToLower()] = values;
            return this;
        }

        public string Build()
        {
            var builder = new UriBuilder(_baseUri)
            {
                Host = BuildFullHostName(_baseUri.Host),
                Path = _path,
                Query = BuildQueryString()
            };
            return builder.Uri.ToString();
        }

        private string BuildFullHostName(string hostname)
        {
            if (string.IsNullOrEmpty(_subdomain))
            {
                return hostname;
            }
            return $"{_subdomain}.{hostname}";            
        }

        private string BuildQueryString()
        {
            var queryParts = new List<string>();
            foreach (var kvp in _parameters)
            {
                if (!string.IsNullOrEmpty(kvp.Value))
                    queryParts.Add($"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}");
            }
            foreach (var kvp in _collections)
            {
                var joined = string.Join(",", kvp.Value);
                if (!string.IsNullOrEmpty(joined))
                    // Encode key, but NOT the comma in values
                    queryParts.Add($"{Uri.EscapeDataString(kvp.Key)}={string.Join(",", kvp.Value.Select(Uri.EscapeDataString))}");
            }
            if (!string.IsNullOrEmpty(_apiKey))
            {
                queryParts.Add($"apikey={_apiKey}");
            }
            return string.Join("&", queryParts);
        }
    }
}
