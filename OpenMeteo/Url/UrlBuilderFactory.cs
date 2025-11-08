using System;

namespace OpenMeteo.Url
{
    /// <summary>
    /// Factory for creating API-specific URL builders with various configuration options.
    /// Centralizes the instantiation logic for all URL builders that inherit from ApiUrlBuilder.
    /// </summary>
    public static class UrlBuilderFactory
    {
        /// <summary>
        /// Creates an instance of the specified URL builder type with optional custom base URI and API key.
        /// </summary>
        /// <typeparam name="T">The type of URL builder to create (must inherit from ApiUrlBuilder)</typeparam>
        /// <param name="customBaseUri">Optional custom base URI for the API</param>
        /// <param name="apiKey">Optional API key for authenticated requests</param>
        /// <returns>An instance of the specified URL builder type</returns>
        /// <exception cref="InvalidOperationException">Thrown when the builder type cannot be instantiated</exception>
        public static T Create<T>(Uri? customBaseUri = null, string? apiKey = null) where T : ApiUrlBuilder
        {
            // Determine which constructor to use based on the provided parameters
            if (customBaseUri is not null && apiKey is not null)
            {
                return (T?)Activator.CreateInstance(typeof(T), customBaseUri, apiKey)
                    ?? throw new InvalidOperationException($"Failed to create instance of {typeof(T).Name} with customBaseUri and apiKey");
            }
            else if (customBaseUri is not null)
            {
                return (T?)Activator.CreateInstance(typeof(T), customBaseUri)
                    ?? throw new InvalidOperationException($"Failed to create instance of {typeof(T).Name} with customBaseUri");
            }
            else if (apiKey is not null)
            {
                return (T?)Activator.CreateInstance(typeof(T), apiKey)
                    ?? throw new InvalidOperationException($"Failed to create instance of {typeof(T).Name} with apiKey");
            }
            else
            {
                return (T?)Activator.CreateInstance(typeof(T))
                    ?? throw new InvalidOperationException($"Failed to create instance of {typeof(T).Name}");
            }
        }
    }
}
