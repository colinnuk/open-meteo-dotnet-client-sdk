using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using OpenMeteo.AirQuality;
using OpenMeteo.Elevation;
using OpenMeteo.Geocoding;
using OpenMeteo.Url;
using OpenMeteo.Weather.Forecast;
using OpenMeteo.Weather.Forecast.Metadata;
using OpenMeteo.Weather.Forecast.Options;
using OpenMeteo.Weather.Forecast.ResponseModel;
using OpenMeteo.Weather.Ensemble;
using OpenMeteo.Weather.Ensemble.Metadata;
using OpenMeteo.Weather.Ensemble.Options;
using OpenMeteo.Weather.Ensemble.ResponseModel;
using OpenMeteo.Weather.Metadata;

namespace OpenMeteo
{
    /// <summary>
    /// Handles GET Requests and performs API Calls.
    /// </summary>
    public class OpenMeteoClient
    {
        private readonly HttpController httpController;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
        private readonly Uri? _customBaseUri;
        private readonly string? _apiKey;
        private readonly WeatherForecastResponseParser _weatherForecastParser;
        private readonly WeatherEnsembleResponseParser _weatherEnsembleParser;

        /// <summary>
        /// If set to true, exceptions from the OpenMeteo API will be rethrown. Default is false.
        /// </summary>
        public bool RethrowExceptions { get; set; } = false;

        /// <summary>
        /// If set to true, calls to the OpenMeteo API will use FlatBuffers format. Default is false.
        /// </summary>
        public bool UseFlatbuffers { get; set; } = false;

        /// <summary>
        /// Creates a new <seealso cref="OpenMeteoClient"/> object to connect to the public OpenMeteo API
        /// </summary>
        public OpenMeteoClient()
        {
            httpController = new HttpController();
            _weatherForecastParser = new WeatherForecastResponseParser(_jsonSerializerOptions);
            _weatherEnsembleParser = new WeatherEnsembleResponseParser(_jsonSerializerOptions);
        }

        /// <summary>
        /// Creates a new <seealso cref="OpenMeteoClient"/> object to connect to the public OpenMeteo API using a custom <see cref="HttpClient"/>
        /// </summary>
        /// <param name="httpClient">A pre-configured <see cref="HttpClient"/> instance</param>
        public OpenMeteoClient(HttpClient httpClient)
        {
            httpController = new HttpController(httpClient);
            _weatherForecastParser = new WeatherForecastResponseParser(_jsonSerializerOptions);
            _weatherEnsembleParser = new WeatherEnsembleResponseParser(_jsonSerializerOptions);
        }

        /// <summary>
        /// Creates a new <seealso cref="OpenMeteoClient"/> object with an API key
        /// </summary>
        /// <param name="apiKey">The API key to use the customer OpenMeteo URLs such as https://customer-api.open-meteo.com</param>
        public OpenMeteoClient(string apiKey)
        {
            httpController = new HttpController();
            _apiKey = apiKey;
            _weatherForecastParser = new WeatherForecastResponseParser(_jsonSerializerOptions);
            _weatherEnsembleParser = new WeatherEnsembleResponseParser(_jsonSerializerOptions);
        }

        /// <summary>
        /// Creates a new <seealso cref="OpenMeteoClient"/> object with an API key using a custom <see cref="HttpClient"/>
        /// </summary>
        /// <param name="apiKey">The API key to use the customer OpenMeteo URLs such as https://customer-api.open-meteo.com</param>
        /// <param name="httpClient">A pre-configured <see cref="HttpClient"/> instance</param>
        public OpenMeteoClient(string apiKey, HttpClient httpClient)
        {
            httpController = new HttpController(httpClient);
            _apiKey = apiKey;
            _weatherForecastParser = new WeatherForecastResponseParser(_jsonSerializerOptions);
            _weatherEnsembleParser = new WeatherEnsembleResponseParser(_jsonSerializerOptions);
        }

        /// <summary>
        /// Creates a new <seealso cref="OpenMeteoClient"/> object with an API key and a custom base URL to connect to your own instance of OpenMeteo API
        /// </summary>
        /// <param name="apiKey">API key</param>
        /// <param name="customBaseUri">Custom base Uri for the OpenMeteo API</param>
        public OpenMeteoClient(string apiKey, Uri customBaseUri)
        {
            httpController = new HttpController();
            _apiKey = apiKey;
            _customBaseUri = customBaseUri;
            _weatherForecastParser = new WeatherForecastResponseParser(_jsonSerializerOptions);
            _weatherEnsembleParser = new WeatherEnsembleResponseParser(_jsonSerializerOptions);
        }

        /// <summary>
        /// Creates a new <seealso cref="OpenMeteoClient"/> object with an API key and a custom base URL to connect to your own instance of OpenMeteo API using a custom <see cref="HttpClient"/>
        /// </summary>
        /// <param name="apiKey">API key</param>
        /// <param name="customBaseUri">Custom base Uri for the OpenMeteo API</param>
        /// <param name="httpClient">A pre-configured <see cref="HttpClient"/> instance</param>
        public OpenMeteoClient(string apiKey, Uri customBaseUri, HttpClient httpClient)
        {
            httpController = new HttpController(httpClient);
            _apiKey = apiKey;
            _customBaseUri = customBaseUri;
            _weatherForecastParser = new WeatherForecastResponseParser(_jsonSerializerOptions);
            _weatherEnsembleParser = new WeatherEnsembleResponseParser(_jsonSerializerOptions);
        }

        /// <summary>
        /// Creates a new <seealso cref="OpenMeteoClient"/> object with a custom base URL to connect to your own instance of OpenMeteo API
        /// </summary>
        /// <param name="customBaseUri">Custom base Uri for the OpenMeteo API</param>
        public OpenMeteoClient(Uri customBaseUri)
        {
            httpController = new HttpController();
            _customBaseUri = customBaseUri;
            _weatherForecastParser = new WeatherForecastResponseParser(_jsonSerializerOptions);
            _weatherEnsembleParser = new WeatherEnsembleResponseParser(_jsonSerializerOptions);
        }

        /// <summary>
        /// Creates a new <seealso cref="OpenMeteoClient"/> object with a custom base URL to connect to your own instance of OpenMeteo API using a custom <see cref="HttpClient"/>
        /// </summary>
        /// <param name="customBaseUri">Custom base Uri for the OpenMeteo API</param>
        /// <param name="httpClient">A pre-configured <see cref="HttpClient"/> instance</param>
        public OpenMeteoClient(Uri customBaseUri, HttpClient httpClient)
        {
            httpController = new HttpController(httpClient);
            _customBaseUri = customBaseUri;
            _weatherForecastParser = new WeatherForecastResponseParser(_jsonSerializerOptions);
            _weatherEnsembleParser = new WeatherEnsembleResponseParser(_jsonSerializerOptions);
        }

        /// <summary>
        /// Performs two GET-Requests (first geocoding api for latitude,longitude, then weather forecast)
        /// </summary>
        /// <param name="location">Name of city</param>
        /// <returns>If successful returns an awaitable Task containing WeatherForecast or NULL if request failed</returns>
        public async Task<WeatherForecast?> QueryWeatherApiAsync(string location)
        {
            GeocodingOptions geocodingOptions = new(location);
            return await QueryWeatherApiAsync(geocodingOptions);
        }

        /// <summary>
        /// Performs two GET-Requests (first geocoding api for latitude,longitude, then weather forecast)
        /// </summary>
        /// <param name="options">Geocoding options</param>
        /// <returns>If successful awaitable <see cref="Task"/> or NULL</returns>
        public async Task<WeatherForecast?> QueryWeatherApiAsync(GeocodingOptions options)
        {
            GeocodingApiResponse? response = await GetLocationDataAsync(options);
            if (response == null || response?.Locations == null)
                return null;

            if (response.Locations.Length == 0)
                return null;

            WeatherForecastOptions weatherForecastOptions = new()
            {
                Latitude = response.Locations[0].Latitude,
                Longitude = response.Locations[0].Longitude,
                Current = CurrentOptions.All
            };

            return await GetWeatherForecastAsync(weatherForecastOptions);
        }

        /// <summary>
        /// Performs one GET-Request
        /// </summary>
        /// <param name="options"></param>
        /// <returns>Awaitable Task containing WeatherForecast or NULL</returns>
        public async Task<WeatherForecast?> QueryWeatherApiAsync(WeatherForecastOptions options)
        {
            return await GetWeatherForecastAsync(options);
        }

        /// <summary>
        /// Performs one GET-Request to get weather information
        /// </summary>
        /// <param name="latitude">City latitude</param>
        /// <param name="longitude">City longitude</param>
        /// <returns>Awaitable Task containing WeatherForecast or NULL</returns>
        public async Task<WeatherForecast?> QueryWeatherApiAsync(float latitude, float longitude)
        {
            WeatherForecastOptions options = new()
            {
                Latitude = latitude,
                Longitude = longitude,

            };
            return await QueryWeatherApiAsync(options);
        }

        /// <summary>
        /// Gets Weather Forecast for a given location with individual options
        /// </summary>
        /// <param name="location"></param>
        /// <param name="options"></param>
        /// <returns><see cref="WeatherForecast"/> for the FIRST found result for <paramref name="location"/></returns>
        public async Task<WeatherForecast?> QueryWeatherApiAsync(string location, WeatherForecastOptions options)
        {
            GeocodingApiResponse? geocodingApiResponse = await GetLocationDataAsync(location);
            if (geocodingApiResponse == null || geocodingApiResponse?.Locations == null)
                return null;

            options.Longitude = geocodingApiResponse.Locations[0].Longitude;
            options.Latitude = geocodingApiResponse.Locations[0].Latitude;

            return await GetWeatherForecastAsync(options);
        }

        /// <summary>
        /// Gets ensemble weather forecast data for a given location with individual options
        /// </summary>
        /// <param name="options">Ensemble options for the request</param>
        /// <returns><see cref="WeatherEnsemble"/> if successful or <see cref="null"/> if failed</returns>
        public async Task<WeatherEnsemble?> QueryEnsembleApiAsync(WeatherEnsembleOptions options)
        {
            return await GetWeatherEnsembleAsync(options);
        }

        /// <summary>
        /// Gets ensemble weather forecast data for a given latitude and longitude
        /// </summary>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Longitude</param>
        /// <returns>Awaitable Task containing WeatherEnsemble or NULL</returns>
        public async Task<WeatherEnsemble?> QueryEnsembleApiAsync(float latitude, float longitude)
        {
            WeatherEnsembleOptions options = new()
            {
                Latitude = latitude,
                Longitude = longitude
            };
            return await QueryEnsembleApiAsync(options);
        }

        /// <summary>
        /// Gets ensemble weather forecast for a given location with individual options
        /// </summary>
        /// <param name="location">Name of location or city</param>
        /// <param name="options">Ensemble options for the request</param>
        /// <returns><see cref="WeatherEnsemble"/> for the FIRST found result for <paramref name="location"/></returns>
        public async Task<WeatherEnsemble?> QueryEnsembleApiAsync(string location, WeatherEnsembleOptions options)
        {
            GeocodingApiResponse? geocodingApiResponse = await GetLocationDataAsync(location);
            if (geocodingApiResponse == null || geocodingApiResponse?.Locations == null)
                return null;

            options.Longitude = geocodingApiResponse.Locations[0].Longitude;
            options.Latitude = geocodingApiResponse.Locations[0].Latitude;

            return await GetWeatherEnsembleAsync(options);
        }

        /// <summary>
        /// Gets air quality data for a given location with individual options
        /// </summary>
        /// <param name="options">options for air quality request</param>
        /// <returns><see cref="AirQualityResponse"/> if successfull or <see cref="null"/> if failed</returns>
        public async Task<AirQualityResponse?> QueryAirQualityAsync(AirQualityOptions options)
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

        /// <summary>
        /// Performs one GET-Request to Open-Meteo Geocoding API 
        /// </summary>
        /// <param name="options">GeocodingOptions object to search with</param>
        /// <returns></returns>
        public async Task<GeocodingApiResponse?> GetLocationDataAsync(GeocodingOptions options)
        {
            return await GetGeocodingDataAsync(options);
        }

        /// <summary>
        /// Performs one GET-Request to Open-Meteo Elevation API 
        /// </summary>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Longitude</param>
        /// <returns></returns>
        public async Task<ElevationApiResponse?> QueryElevationAsync(float latitude, float longitude)
        {
            ElevationOptions elevationOptions = new(latitude, longitude);
            return await GetElevationAsync(elevationOptions);
        }

        public async Task<MetadataModel> QueryWeatherForecastMetadata(WeatherModelOptionsParameter weatherModel)
        {
            try
            {
                var url = UrlBuilderFactory.Create<WeatherForecastMetadataUrlBuilder>(_customBaseUri, _apiKey)
                    .WithModel(weatherModel)
                    .Build();

                HttpResponseMessage response = await httpController.Client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                MetadataApiModel? meta = await JsonSerializer.DeserializeAsync<MetadataApiModel>(await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions);
                return ConvertMetadataModel(meta ?? throw new OpenMeteoClientException("No metadata found", response.StatusCode));
            }
            catch (HttpRequestException)
            {
                throw;
            }
        }

        public async Task<MetadataModel> QueryWeatherEnsembleMetadata(EnsembleModelOptionsParameter weatherModel)
        {
            try
            {
                var url = UrlBuilderFactory.Create<WeatherEnsembleMetadataUrlBuilder>(_customBaseUri, _apiKey)
                    .WithModel(weatherModel)
                    .Build();

                HttpResponseMessage response = await httpController.Client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                MetadataApiModel? meta = await JsonSerializer.DeserializeAsync<MetadataApiModel>(await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions);
                return ConvertMetadataModel(meta ?? throw new OpenMeteoClientException("No metadata found", response.StatusCode));
            }
            catch (HttpRequestException)
            {
                throw;
            }
        }

        private static MetadataModel ConvertMetadataModel(MetadataApiModel apiModel) => new(
            DateTimeOffset.FromUnixTimeSeconds(apiModel.data_end_time).UtcDateTime,
            DateTimeOffset.FromUnixTimeSeconds(apiModel.last_run_availability_time).UtcDateTime,
            DateTimeOffset.FromUnixTimeSeconds(apiModel.last_run_initialisation_time).UtcDateTime,
            DateTimeOffset.FromUnixTimeSeconds(apiModel.last_run_modification_time).UtcDateTime,
            apiModel.temporal_resolution_seconds,
            apiModel.update_interval_seconds,
            apiModel.crs_wkt);

        private async Task<AirQualityResponse?> GetAirQualityAsync(AirQualityOptions options)
        {
            try
            {
                var url = UrlBuilderFactory.Create<AirQualityUrlBuilder>(_customBaseUri, _apiKey)
                    .WithOptions(options)
                    .Build();
                HttpResponseMessage response = await httpController.Client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                AirQualityResponse? airQuality = await JsonSerializer.DeserializeAsync<AirQualityResponse>(await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions);
                return airQuality;
            }
            catch (HttpRequestException)
            {
                if (RethrowExceptions)
                    throw;
                return null;
            }
        }

        private async Task<ErrorResponse?> ParseErrorResponseAsync(HttpResponseMessage response)
        {
            if ((int)response.StatusCode >= 400 && (int)response.StatusCode < 500)
            {
                try
                {
                    return await JsonSerializer.DeserializeAsync<ErrorResponse>(await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions);
                }
                catch (Exception)
                {
                    // Empty catch block to ignore deserialization errors for error response
                }
            }
            return null;
        }

        private async Task<WeatherForecast?> GetWeatherForecastAsync(WeatherForecastOptions options)
        {
            try
            {
                var url = UrlBuilderFactory.Create<WeatherForecastUrlBuilder>(_customBaseUri, _apiKey)
                    .WithOptions(options)
                    .WithFlatbuffers(UseFlatbuffers)
                    .Build();
                HttpResponseMessage response = await httpController.Client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return UseFlatbuffers
                        ? await _weatherForecastParser.ConvertFlatBuffersAsync(response, options)
                        : await _weatherForecastParser.DeserializeJsonAsync(response, options);
                }

                ErrorResponse? error = await ParseErrorResponseAsync(response);
                throw new OpenMeteoClientException(error?.Reason ?? "Exception in OpenMeteoClient", response.StatusCode);
            }
            catch (Exception)
            {
                if (RethrowExceptions)
                    throw;
                return null;
            }
        }

        private async Task<WeatherEnsemble?> GetWeatherEnsembleAsync(WeatherEnsembleOptions options)
        {
            try
            {
                var url = UrlBuilderFactory.Create<WeatherEnsembleUrlBuilder>(_customBaseUri, _apiKey)
                    .WithOptions(options)
                    .WithFlatbuffers(UseFlatbuffers)
                    .Build();
                HttpResponseMessage response = await httpController.Client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return UseFlatbuffers
                        ? await _weatherEnsembleParser.ConvertFlatBuffersAsync(response, options)
                        : await _weatherEnsembleParser.DeserializeJsonAsync(response, options);
                }

                ErrorResponse? error = await ParseErrorResponseAsync(response);
                throw new OpenMeteoClientException(error?.Reason ?? "Exception in OpenMeteoClient", response.StatusCode);
            }
            catch (Exception)
            {
                if (RethrowExceptions)
                    throw;
                return null;
            }
        }

        private async Task<GeocodingApiResponse?> GetGeocodingDataAsync(GeocodingOptions options)
        {
            try
            {
                var url = UrlBuilderFactory.Create<GeocodingUrlBuilder>(_customBaseUri, _apiKey)
                    .WithOptions(options)
                    .Build();
                HttpResponseMessage response = await httpController.Client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                GeocodingApiResponse? geocodingData = await JsonSerializer.DeserializeAsync<GeocodingApiResponse>(await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions);

                return geocodingData;
            }
            catch (HttpRequestException)
            {
                if (RethrowExceptions)
                    throw;
                return null;
            }
        }

        private async Task<ElevationApiResponse?> GetElevationAsync(ElevationOptions options)
        {
            try
            {
                var url = UrlBuilderFactory.Create<ElevationUrlBuilder>(_customBaseUri, _apiKey)
                    .WithOptions(options)
                    .Build();
                HttpResponseMessage response = await httpController.Client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                ElevationApiResponse? elevationData = await JsonSerializer.DeserializeAsync<ElevationApiResponse>(await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions);

                return elevationData;
            }
            catch (HttpRequestException)
            {
                if (RethrowExceptions)
                    throw;
                return null;
            }
        }
    }
}
