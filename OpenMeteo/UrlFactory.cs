using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;

namespace OpenMeteo;
public class UrlFactory
{
    private readonly string _weatherApiUrl = "api.open-meteo.com/v1/forecast";
    private readonly string _geocodeApiUrl = "geocoding-api.open-meteo.com/v1/search";
    private readonly string _airQualityApiUrl = "air-quality-api.open-meteo.com/v1/air-quality";
    private readonly string _elevationApiUrl = "api.open-meteo.com/v1/elevation";
    private readonly string _metadataFileFragment = "/static/meta.json";
    private readonly string _customerApiUrlFragment = "customer-";

    private readonly string _apiKey = string.Empty;
    private readonly Uri? _customBaseUri = null;

    public UrlFactory() 
    {
    }

    public UrlFactory(string apiKey)
    {
        _apiKey = apiKey;
    }

    public UrlFactory(Uri customBaseUri)
    {
        _customBaseUri = customBaseUri;
    }

    public UrlFactory(string apiKey, Uri customBaseUri)
    {
        _apiKey = apiKey;
        _customBaseUri = customBaseUri;
    }

    public string GetUrlWithOptions(WeatherForecastOptions options)
    {
        var parameters = new Dictionary<string, string>
        {
            [nameof(options.Latitude).ToLower()] = options.Latitude.ToString(CultureInfo.InvariantCulture),
            [nameof(options.Longitude).ToLower()] = options.Longitude.ToString(CultureInfo.InvariantCulture),
            [nameof(options.Temperature_Unit).ToLower()] = options.Temperature_Unit.ToString(),
            [nameof(options.Windspeed_Unit).ToLower()] = options.Windspeed_Unit.ToString(),
            [nameof(options.Precipitation_Unit).ToLower()] = options.Precipitation_Unit.ToString(),
            [nameof(options.Timezone).ToLower()] = options.Timezone,
            [nameof(options.Timeformat).ToLower()] = options.Timeformat.ToString(),
            [nameof(options.Past_Days).ToLower()] = options.Past_Days.ToString(),
            [nameof(options.Start_date).ToLower()] = options.Start_date,
            [nameof(options.End_date).ToLower()] = options.End_date,
            [nameof(options.Cell_Selection).ToLower()] = options.Cell_Selection.ToString()
        };
        var collections = new Dictionary<string, IEnumerable<string>>();
        if (options.Hourly.Count >0)
            collections[nameof(options.Hourly).ToLower()] = options.Hourly.Parameter.Select(x => x.ToString());
        if (options.Daily.Count >0)
            collections[nameof(options.Daily).ToLower()] = options.Daily.Parameter.Select(x => x.ToString());
        if (options.Models.Count >0)
            collections[nameof(options.Models).ToLower()] = options.Models.Parameter.Select(x => x.ToString());
        if (options.Current.Count >0)
            collections[nameof(options.Current).ToLower()] = options.Current.Parameter.Select(x => x.ToString());
        if (options.Minutely_15.Count >0)
            collections[nameof(options.Minutely_15).ToLower()] = options.Minutely_15.Parameter.Select(x => x.ToString());

        UriBuilder uri = new(GetBaseUrl(_weatherApiUrl))
        {
            Query = BuildQueryString(parameters, collections)
        };
        SetApiKeyIfNeeded(uri);
        return uri.ToString();
    }

    /// <summary>
    /// Combines a given url with an options object to create a url for GET requests
    /// </summary>
    /// <returns>url+queryString</returns>
    public string GetUrlWithOptions(GeocodingOptions options)
    {
        var parameters = new Dictionary<string, string>
        {
            [nameof(options.Name).ToLower()] = options.Name,
            [nameof(options.Count).ToLower()] = options.Count > 0 ? options.Count.ToString() : 1.ToString(),
            [nameof(options.Format).ToLower()] = options.Format,
            [nameof(options.Language).ToLower()] = options.Language
        };
        UriBuilder uri = new(GetBaseUrl(_geocodeApiUrl))
        {
            Query = BuildQueryString(parameters)
        };
        SetApiKeyIfNeeded(uri);
        return uri.ToString();
    }

    /// <summary>
    /// Combines a given url with an options object to create a url for GET requests
    /// </summary>
    /// <returns>url+queryString</returns>
    public string GetUrlWithOptions(AirQualityOptions options)
    {
        var parameters = new Dictionary<string, string>
        {
            [nameof(options.Latitude).ToLower()] = options.Latitude.ToString(CultureInfo.InvariantCulture),
            [nameof(options.Longitude).ToLower()] = options.Longitude.ToString(CultureInfo.InvariantCulture),
            [nameof(options.Domains).ToLower()] = options.Domains,
            [nameof(options.Timeformat).ToLower()] = options.Timeformat,
            [nameof(options.Timezone).ToLower()] = options.Timezone
        };
        var collections = new Dictionary<string, IEnumerable<string>>();
        if (options.Hourly.Count >0)
            collections[nameof(options.Hourly).ToLower()] = options.Hourly.Parameter.Select(x => x.ToString());

        UriBuilder uri = new(GetBaseUrl(_airQualityApiUrl))
        {
            Query = BuildQueryString(parameters, collections)
        };
        SetApiKeyIfNeeded(uri);
        return uri.ToString();
    }

    public string GetUrlWithOptions(ElevationOptions options)
    {
        var parameters = new Dictionary<string, string>
        {
            [nameof(options.Latitude).ToLower()] = options.Latitude.ToString(CultureInfo.InvariantCulture),
            [nameof(options.Longitude).ToLower()] = options.Longitude.ToString(CultureInfo.InvariantCulture)
        };
        UriBuilder uri = new(GetBaseUrl(_elevationApiUrl))
        {
            Query = BuildQueryString(parameters)
        };
        SetApiKeyIfNeeded(uri);
        return uri.ToString();
    }

    public string GetWeatherForecastMetadataUrl(WeatherModelOptionsParameter weatherModel)
    {
        var metadataBaseUrl = $"api.open-meteo.com/data/{MetadataNameHelper.GetPrefixForWeatherModel(weatherModel)}{_metadataFileFragment}";
        return GetBaseUrl(metadataBaseUrl);
    }

    private void SetApiKeyIfNeeded(UriBuilder uri)
    {
        if (!string.IsNullOrEmpty(_apiKey))
            uri.Query += $"&apikey={_apiKey}";
    }

    private string GetBaseUrl(string url)
    {
        if (_customBaseUri != null)
        {
            return _customBaseUri.ToString();
        }
        var prependCustomerIfHasApiKey = string.IsNullOrEmpty(_apiKey) ? string.Empty : _customerApiUrlFragment;
        return $"https://{prependCustomerIfHasApiKey}{url}";
    }

    private static string BuildQueryString(Dictionary<string, string> parameters)
    {
        return BuildQueryString(parameters, []);
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
