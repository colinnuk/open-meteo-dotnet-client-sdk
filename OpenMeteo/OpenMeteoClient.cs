using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

namespace OpenMeteo
{
    /// <summary>
    /// Handles GET Requests and performs API Calls.
    /// </summary>
    public class OpenMeteoClient
    {
        private readonly HttpController httpController;
        private readonly UrlFactory _urlFactory = new();
        private readonly IOpenMeteoLogger? _logger = default!;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

        /// <summary>
        /// If set to true, exceptions from the OpenMeteo API will be rethrown. Default is false.
        /// </summary>
        /// <param name="rethrowExceptions"></param>
        public bool RethrowExceptions { get; set; } = false;

        /// <summary>
        /// Creates a new <seealso cref="OpenMeteoClient"/> object and sets the neccessary variables (httpController, CultureInfo)
        /// </summary>
        public OpenMeteoClient()
        {
            httpController = new HttpController();
        }

        /// <summary>
        /// Creates a new <seealso cref="OpenMeteoClient"/> object with a logger
        /// </summary>
        /// <param name="logger">An object which implements an interface that can be used for logging from this class</param>
        public OpenMeteoClient(IOpenMeteoLogger logger)
        {
            httpController = new HttpController();
            _logger = logger;
        }

        /// <summary>
        /// Creates a new <seealso cref="OpenMeteoClient"/> object with an API key
        /// </summary>
        /// <param name="apiKey">The API key to use the customer OpenMeteo URLs such as https://customer-api.open-meteo.com</param>
        public OpenMeteoClient(string apiKey)
        {
            httpController = new HttpController();
            _urlFactory = new UrlFactory(apiKey);
        }

        /// <summary>
        /// Creates a new <seealso cref="OpenMeteoClient"/> object with a logger and an API key
        /// </summary>
        /// <param name="logger">An object which implements an interface that can be used for logging from this class</param>
        /// <param name="apiKey">The API key to use the customer OpenMeteo URLs such as https://customer-api.open-meteo.com</param>

        public OpenMeteoClient(IOpenMeteoLogger logger, string apiKey)
        {
            httpController = new HttpController();
            _logger = logger;
            _urlFactory = new UrlFactory(apiKey);

            if (!string.IsNullOrEmpty(apiKey))
                _logger?.Information($"{nameof(OpenMeteoClient)} Initialised with API key starting with: {apiKey[..2]}");
        }

        /// <summary>
        /// Creates a new <seealso cref="OpenMeteoClient"/> object with a custom base URL
        /// </summary>
        /// <param name="customBaseUri">Custom base Uri for the OpenMeteo API</param>
        public OpenMeteoClient(Uri customBaseUri)
        {
            httpController = new HttpController();
            _urlFactory = new UrlFactory(customBaseUri);
        }

        /// <summary>
        /// Creates a new <seealso cref="OpenMeteoClient"/> object with a logger and a custom base URL
        /// </summary>
        /// <param name="logger">Logger instance</param>
        /// <param name="customBaseUri">Custom base Uri for the OpenMeteo API</param>
        public OpenMeteoClient(IOpenMeteoLogger logger, Uri customBaseUri)
        {
            httpController = new HttpController();
            _logger = logger;
            _urlFactory = new UrlFactory(customBaseUri);
        }

        /// <summary>
        /// Creates a new <seealso cref="OpenMeteoClient"/> object with an API key and a custom base URL
        /// </summary>
        /// <param name="apiKey">API key</param>
        /// <param name="customBaseUri">Custom base Uri for the OpenMeteo API</param>
        public OpenMeteoClient(string apiKey, Uri customBaseUri)
        {
            httpController = new HttpController();
            _urlFactory = new UrlFactory(apiKey, customBaseUri);
        }

        /// <summary>
        /// Creates a new <seealso cref="OpenMeteoClient"/> object with a logger, an API key, and a custom base URL
        /// </summary>
        /// <param name="logger">Logger instance</param>
        /// <param name="apiKey">API key</param>
        /// <param name="customBaseUri">Custom base Uri for the OpenMeteo API</param>
        public OpenMeteoClient(IOpenMeteoLogger logger, string apiKey, Uri customBaseUri)
        {
            httpController = new HttpController();
            _logger = logger;
            _urlFactory = new UrlFactory(apiKey, customBaseUri);
            if (!string.IsNullOrEmpty(apiKey))
                _logger?.Information($"{nameof(OpenMeteoClient)} Initialised with API key starting with: {apiKey[..2]}");
        }

        /// <summary>
        /// Performs two GET-Requests (first geocoding api for latitude,longitude, then weather forecast)
        /// </summary>
        /// <param name="location">Name of city</param>
        /// <returns>If successful returns an awaitable Task containing WeatherForecast or NULL if request failed</returns>
        public async Task<WeatherForecast?> QueryAsync(string location)
        {
            GeocodingOptions geocodingOptions = new(location);

            // Get location Information
            GeocodingApiResponse? response = await GetGeocodingDataAsync(geocodingOptions);
            if (response == null || response.Locations == null)
                return null;

            WeatherForecastOptions options = new()
            {
                Latitude = response.Locations[0].Latitude,
                Longitude = response.Locations[0].Longitude,
                Current = CurrentOptions.All // Get all current weather data if nothing else is provided
                
            };

            return await GetWeatherForecastAsync(options);
        }

        /// <summary>
        /// Performs two GET-Requests (first geocoding api for latitude,longitude, then weather forecast)
        /// </summary>
        /// <param name="options">Geocoding options</param>
        /// <returns>If successful awaitable <see cref="Task"/> or NULL</returns>
        public async Task<WeatherForecast?> QueryAsync(GeocodingOptions options)
        {
            // Get City Information
            GeocodingApiResponse? response = await GetLocationDataAsync(options);
            if (response == null || response?.Locations == null)
                return null;

            WeatherForecastOptions weatherForecastOptions = new()
            {
                Latitude = response.Locations[0].Latitude,
                Longitude = response.Locations[0].Longitude,
                Current = CurrentOptions.All // Get all current weather data if nothing else is provided
                
            };

            return await GetWeatherForecastAsync(weatherForecastOptions);
        }

        /// <summary>
        /// Performs one GET-Request
        /// </summary>
        /// <param name="options"></param>
        /// <returns>Awaitable Task containing WeatherForecast or NULL</returns>
        public async Task<WeatherForecast?> QueryAsync(WeatherForecastOptions options)
        {
            return await GetWeatherForecastAsync(options);
        }

        /// <summary>
        /// Performs one GET-Request to get weather information
        /// </summary>
        /// <param name="latitude">City latitude</param>
        /// <param name="longitude">City longitude</param>
        /// <returns>Awaitable Task containing WeatherForecast or NULL</returns>
        public async Task<WeatherForecast?> QueryAsync(float latitude, float longitude)
        {
            WeatherForecastOptions options = new()
            {
                Latitude = latitude,
                Longitude = longitude,
                
            };
            return await QueryAsync(options);
        }

        /// <summary>
        /// Gets Weather Forecast for a given location with individual options
        /// </summary>
        /// <param name="location"></param>
        /// <param name="options"></param>
        /// <returns><see cref="WeatherForecast"/> for the FIRST found result for <paramref name="location"/></returns>
        public async Task<WeatherForecast?> QueryAsync(string location, WeatherForecastOptions options)
        {
            GeocodingApiResponse? geocodingApiResponse = await GetLocationDataAsync(location);
            if (geocodingApiResponse == null || geocodingApiResponse?.Locations == null)
                return null;
            
            options.Longitude = geocodingApiResponse.Locations[0].Longitude;
            options.Latitude = geocodingApiResponse.Locations[0].Latitude;

            return await GetWeatherForecastAsync(options);
        }

        /// <summary>
        /// Gets air quality data for a given location with individual options
        /// </summary>
        /// <param name="options">options for air quality request</param>
        /// <returns><see cref="AirQuality"/> if successfull or <see cref="null"/> if failed</returns>
        public async Task<AirQuality?> QueryAsync(AirQualityOptions options)
        {
            return await GetAirQualityAsync(options);
        }

        /// <summary>
        /// Performs one GET-Request to Open-Meteo Geocoding API 
        /// </summary>
        /// <param name="location">Name of a location or city</param>
        /// <returns></returns>
        public async Task<GeocodingApiResponse?> GetLocationDataAsync(string location)
        {
            GeocodingOptions geocodingOptions = new(location);

            return await GetLocationDataAsync(geocodingOptions);
        }

        public async Task<GeocodingApiResponse?> GetLocationDataAsync(GeocodingOptions options)
        {
            return await GetGeocodingDataAsync(options);
        }

        /// <summary>
        /// Performs one GET-Request to get a (float, float) tuple
        /// </summary>
        /// <param name="location">Name of a city or location</param>
        /// <returns>(latitude, longitude) tuple of first found location or null if no location was found</returns>
        public async Task<(float latitude, float longitude)?> GetLocationLatitudeLongitudeAsync(string location)
        {
            GeocodingApiResponse? response = await GetLocationDataAsync(location);
            if (response == null || response?.Locations == null)
                return null;
            return (response.Locations[0].Latitude, response.Locations[0].Longitude);
        }

        /// <summary>
        /// Performs one GET-Request to Open-Meteo Elevation API 
        /// </summary>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Longitude</param>
        /// <returns></returns>
        public async Task<ElevationApiResponse?> QueryElevationAsync(float latitude, float longitude)
        {
            ElevationOptions elevationOptions = new ElevationOptions(latitude, longitude);

            return await GetElevationAsync(elevationOptions);
        }

        public WeatherForecast? Query(WeatherForecastOptions options)
        {
            return QueryAsync(options).GetAwaiter().GetResult();
        }

        public WeatherForecast? Query(float latitude, float longitude)
        {
            return QueryAsync(latitude, longitude).GetAwaiter().GetResult();
        }

        public WeatherForecast? Query(string location, WeatherForecastOptions options)
        {
            return QueryAsync(location, options).GetAwaiter().GetResult();
        }

        public WeatherForecast? Query(GeocodingOptions options)
        {
            return QueryAsync(options).GetAwaiter().GetResult();
        }

        public WeatherForecast? Query(string location)
        {
            return QueryAsync(location).GetAwaiter().GetResult();
        }

        public AirQuality? Query(AirQualityOptions options)
        {
            return QueryAsync(options).GetAwaiter().GetResult();
        }

        public ElevationApiResponse? QueryElevation(float latitude, float longitude)
        {
            return QueryElevationAsync(latitude, longitude).GetAwaiter().GetResult();
        }

        public async Task<MetadataModel> QueryWeatherForecastMetadata(WeatherModelOptionsParameter weatherModel)
        {
            try
            {
                var url = _urlFactory.GetWeatherForecastMetadataUrl(weatherModel);
                _logger?.Debug($"{nameof(OpenMeteoClient)}.QueryWeatherForecastMetadata(). URL: {_urlFactory.SanitiseUrl(url)}");
                HttpResponseMessage response = await httpController.Client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                MetadataApiModel? meta = await JsonSerializer.DeserializeAsync<MetadataApiModel>(await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions);
                return ConvertMetadataModel(meta ?? throw new OpenMeteoClientException("No metadata found", response.StatusCode));
            }
            catch (HttpRequestException e)
            {
                _logger?.Warning($"{nameof(OpenMeteoClient)}.QueryWeatherForecastMetadata(). Message: {e.Message} StackTrace: {e.StackTrace}");
                throw;
            }
        }

        private static MetadataModel ConvertMetadataModel(MetadataApiModel apiModel) => new(
            DateTimeOffset.FromUnixTimeSeconds(apiModel.data_end_time).UtcDateTime,
            DateTimeOffset.FromUnixTimeSeconds(apiModel.last_run_availability_time).UtcDateTime,
            DateTimeOffset.FromUnixTimeSeconds(apiModel.last_run_initialisation_time).UtcDateTime,
            DateTimeOffset.FromUnixTimeSeconds(apiModel.last_run_modification_time).UtcDateTime,
            apiModel.temporal_resolution_seconds,
            apiModel.update_interval_seconds);

        private async Task<AirQuality?> GetAirQualityAsync(AirQualityOptions options)
        {
            try
            {
                var url = _urlFactory.GetUrlWithOptions(options);
                _logger?.Debug($"{nameof(OpenMeteoClient)}.GetAirQualityAsync(). URL: {_urlFactory.SanitiseUrl(url)}");
                HttpResponseMessage response = await httpController.Client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                AirQuality? airQuality = await JsonSerializer.DeserializeAsync<AirQuality>(await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions);
                return airQuality;
            }
            catch (HttpRequestException e)
            {
                _logger?.Warning($"{nameof(OpenMeteoClient)}.GetAirQualityAsync(). Message: {e.Message} StackTrace: {e.StackTrace}");
                if (RethrowExceptions)
                    throw;
                return null;
            }
        }

        private async Task<WeatherForecast?> GetWeatherForecastAsync(WeatherForecastOptions options)
        {
            try
            {
                var url = _urlFactory.GetUrlWithOptions(options);
                _logger?.Debug($"{nameof(OpenMeteoClient)}.GetWeatherForecastAsync(). URL: {_urlFactory.SanitiseUrl(url)}");
                HttpResponseMessage response = await httpController.Client.GetAsync(url);
                if(response.IsSuccessStatusCode)
                {
                    WeatherForecast? weatherForecast = await JsonSerializer.DeserializeAsync<WeatherForecast>(await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions);
                    return weatherForecast;
                }

                ErrorResponse? error = null;
                if((int)response.StatusCode >= 400 && (int)response.StatusCode < 500)
                {
                    try
                    {
                        error = await JsonSerializer.DeserializeAsync<ErrorResponse>(await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions);
                    }
                    catch (Exception e)
                    {
                        _logger?.Error($"{nameof(OpenMeteoClient)}.GetWeatherForecastAsync(). Unable to deserialise error response. This exception will be thrown. Message: {e.Message} StackTrace: {e.StackTrace}");
                    }
                }                
                
                throw new OpenMeteoClientException(error?.Reason ?? "Exception in OpenMeteoClient", response.StatusCode);
            }
            catch (Exception e)
            {
                _logger?.Warning($"{nameof(OpenMeteoClient)}.GetWeatherForecastAsync(). Message: {e.Message} StackTrace: {e.StackTrace}");
                if (RethrowExceptions)
                    throw;
                return null;
            }

        }

        private async Task<GeocodingApiResponse?> GetGeocodingDataAsync(GeocodingOptions options)
        {
            try
            {

                var url = _urlFactory.GetUrlWithOptions(options);
                _logger?.Debug($"{nameof(OpenMeteoClient)}.GetGeocodingDataAsync(). URL: {_urlFactory.SanitiseUrl(url)}");
                HttpResponseMessage response = await httpController.Client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                GeocodingApiResponse? geocodingData = await JsonSerializer.DeserializeAsync<GeocodingApiResponse>(await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions);

                return geocodingData;
            }
            catch (HttpRequestException e)
            {
                _logger?.Warning($"{nameof(OpenMeteoClient)}.GetGeocodingDataAsync(). Message: {e.Message} StackTrace: {e.StackTrace}");
                if (RethrowExceptions)
                    throw;
                return null;
            }
        }

        private async Task<ElevationApiResponse?> GetElevationAsync(ElevationOptions options)
        {
            try
            {
                var url = _urlFactory.GetUrlWithOptions(options);
                _logger?.Debug($"{nameof(OpenMeteoClient)}.GetElevationAsync(). URL: {_urlFactory.SanitiseUrl(url)}");
                HttpResponseMessage response = await httpController.Client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                ElevationApiResponse? elevationData = await JsonSerializer.DeserializeAsync<ElevationApiResponse>(await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions);

                return elevationData;
            }
            catch (HttpRequestException e)
            {
                _logger?.Warning($"Can't find elevation for latitude {options.Latitude} & longitude {options.Longitude}. Please make sure that they are valid.");
                _logger?.Warning($"Error in {nameof(OpenMeteoClient)}.GetElevationAsync(). Message: {e.Message} StackTrace: {e.StackTrace}");
                if (RethrowExceptions)
                    throw;
                return null;
            }
        }
    }
}

