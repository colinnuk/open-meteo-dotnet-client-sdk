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
        private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
        private readonly Uri? _customBaseUri;
        private readonly string? _apiKey;

        /// <summary>
        /// If set to true, exceptions from the OpenMeteo API will be rethrown. Default is false.
        /// </summary>
        /// <param name="rethrowExceptions"></param>
        public bool RethrowExceptions { get; set; } = false;

        /// <summary>
        /// Creates a new <seealso cref="OpenMeteoClient"/> object to connect to the public OpenMeteo API
        /// </summary>
        public OpenMeteoClient()
        {
            httpController = new HttpController();
        }

        /// <summary>
        /// Creates a new <seealso cref="OpenMeteoClient"/> object with an API key
        /// </summary>
        /// <param name="apiKey">The API key to use the customer OpenMeteo URLs such as https://customer-api.open-meteo.com</param>
        public OpenMeteoClient(string apiKey)
        {
            httpController = new HttpController();
            _apiKey = apiKey;
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
        }

        /// <summary>
        /// Creates a new <seealso cref="OpenMeteoClient"/> object with a custom base URL to connect to your own instance of OpenMeteo API
        /// </summary>
        /// <param name="customBaseUri">Custom base Uri for the OpenMeteo API</param>
        public OpenMeteoClient(Uri customBaseUri)
        {
            httpController = new HttpController();
            _customBaseUri = customBaseUri;
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
        /// Gets air quality data for a given location with individual options
        /// </summary>
        /// <param name="options">options for air quality request</param>
        /// <returns><see cref="AirQuality"/> if successfull or <see cref="null"/> if failed</returns>
        public async Task<AirQuality?> QueryAirQualityAsync(AirQualityOptions options)
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
                var options = new WeatherForecastOptions();
                options.Models.Add(weatherModel);
                var url = UrlBuilderFactory.Create<WeatherForecastUrlBuilder>(_customBaseUri, _apiKey)
                    .WithOptions(options)
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
            apiModel.update_interval_seconds);

        private async Task<AirQuality?> GetAirQualityAsync(AirQualityOptions options)
        {
            try
            {
                var url = UrlBuilderFactory.Create<AirQualityUrlBuilder>(_customBaseUri, _apiKey)
                    .WithOptions(options)
                    .Build();
                HttpResponseMessage response = await httpController.Client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                AirQuality? airQuality = await JsonSerializer.DeserializeAsync<AirQuality>(await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions);
                return airQuality;
            }
            catch (HttpRequestException)
            {
                if (RethrowExceptions)
                    throw;
                return null;
            }
        }

        private async Task<WeatherForecast?> GetWeatherForecastAsync(WeatherForecastOptions options)
        {
            try
            {
                var url = UrlBuilderFactory.Create<WeatherForecastUrlBuilder>(_customBaseUri, _apiKey)
                    .WithOptions(options)
                    .Build();
                HttpResponseMessage response = await httpController.Client.GetAsync(url);
                if(response.IsSuccessStatusCode)
                {
                    WeatherForecast? weatherForecast = await JsonSerializer.DeserializeAsync<WeatherForecast>(await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions);
                    return weatherForecast;
                }

                ErrorResponse? error = null;
                if((int)response.StatusCode >=400 && (int)response.StatusCode <500)
                {
                    try
                    {
                        error = await JsonSerializer.DeserializeAsync<ErrorResponse>(await response.Content.ReadAsStreamAsync(), _jsonSerializerOptions);
                    }
                    catch (Exception) 
                    {
                        // Empty catch block to ignore deserialization errors for error response
                    }
                } 
                
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

