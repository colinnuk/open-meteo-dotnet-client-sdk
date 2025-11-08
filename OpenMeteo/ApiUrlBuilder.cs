using System;

namespace OpenMeteo
{
    /// <summary>
    /// Abstract base class for API-specific URL builders.
    /// Centralizes constructor logic for handling various combinations of customBaseUri and apiKey.
    /// </summary>
    public abstract class ApiUrlBuilder
    {
        protected readonly UrlBuilder _urlBuilder;
        protected abstract string DefaultSubdomain { get; }
        protected abstract string ApiPath { get; }

        /// <summary>
        /// Default constructor - uses default subdomain and API path
        /// </summary>
        protected ApiUrlBuilder()
        {
            _urlBuilder = new UrlBuilder()
                .WithSubdomain(DefaultSubdomain)
                .WithPath(ApiPath);
        }

        /// <summary>
        /// Constructor with custom base URI
        /// </summary>
        protected ApiUrlBuilder(Uri customBaseUri)
        {
            _urlBuilder = new UrlBuilder()
                .WithBaseUri(customBaseUri)
                .WithPath(ApiPath);
        }

        /// <summary>
        /// Constructor with API key - uses customer subdomain
        /// </summary>
        protected ApiUrlBuilder(string apiKey)
        {
            _urlBuilder = new UrlBuilder()
                .WithApiKey(apiKey)
                .WithSubdomain($"customer-{DefaultSubdomain}")
                .WithPath(ApiPath);
        }

        /// <summary>
        /// Constructor with both custom base URI and API key
        /// </summary>
        protected ApiUrlBuilder(Uri customBaseUri, string apiKey)
        {
            _urlBuilder = new UrlBuilder()
                .WithBaseUri(customBaseUri)
                .WithPath(ApiPath)
                .WithApiKey(apiKey);
        }

        /// <summary>
        /// Builds the final URL string
        /// </summary>
        public string Build()
        {
            return _urlBuilder.Build();
        }
    }
}
