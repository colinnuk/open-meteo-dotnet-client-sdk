using System;
using System.Globalization;

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

    public string SanitiseUrl(string url)
    {
        return string.IsNullOrEmpty(_apiKey) ? url : url.Replace(_apiKey, "APIKEY");
    }

    public string GetUrlWithOptions(WeatherForecastOptions options)
    {
        UriBuilder uri = new(GetBaseUrl(_weatherApiUrl));

        // Add the properties
        // Begin with Latitude and Longitude since they're required
        uri.Query = "latitude=" + options.Latitude.ToString(CultureInfo.InvariantCulture);
        uri.Query += "&longitude=" + options.Longitude.ToString(CultureInfo.InvariantCulture);

        uri.Query += "&temperature_unit=" + options.Temperature_Unit.ToString();
        uri.Query += "&windspeed_unit=" + options.Windspeed_Unit.ToString();
        uri.Query += "&precipitation_unit=" + options.Precipitation_Unit.ToString();
        if (options.Timezone != string.Empty)
            uri.Query += "&timezone=" + options.Timezone;

        uri.Query += "&timeformat=" + options.Timeformat.ToString();

        uri.Query += "&past_days=" + options.Past_Days;

        if (options.Start_date != string.Empty)
            uri.Query += "&start_date=" + options.Start_date;
        if (options.End_date != string.Empty)
            uri.Query += "&end_date=" + options.End_date;

        // Now we iterate through hourly and daily

        // Hourly
        if (options.Hourly.Count > 0)
        {
            bool firstHourlyElement = true;
            uri.Query += "&hourly=";

            foreach (var option in options.Hourly)
            {
                if (firstHourlyElement)
                {
                    uri.Query += option.ToString();
                    firstHourlyElement = false;
                }
                else
                {
                    uri.Query += "," + option.ToString();
                }
            }
        }

        // Daily
        if (options.Daily.Count > 0)
        {
            bool firstDailyElement = true;
            uri.Query += "&daily=";
            foreach (var option in options.Daily)
            {
                if (firstDailyElement)
                {
                    uri.Query += option.ToString();
                    firstDailyElement = false;
                }
                else
                {
                    uri.Query += "," + option.ToString();
                }
            }
        }

        // 0.2.0 Weather models
        // cell_selection
        uri.Query += "&cell_selection=" + options.Cell_Selection;

        // Models
        if (options.Models.Count > 0)
        {
            bool firstModelsElement = true;
            uri.Query += "&models=";
            foreach (var option in options.Models)
            {
                if (firstModelsElement)
                {
                    uri.Query += option.ToString();
                    firstModelsElement = false;
                }
                else
                {
                    uri.Query += "," + option.ToString();
                }
            }
        }

        // new current parameter
        if (options.Current.Count > 0)
        {
            bool firstCurrentElement = true;
            uri.Query += "&current=";
            foreach (var option in options.Current)
            {
                if (firstCurrentElement)
                {
                    uri.Query += option.ToString();
                    firstCurrentElement = false;
                }
                else
                {
                    uri.Query += "," + option.ToString();
                }
            }
        }

        // new minutely_15 parameter
        if (options.Minutely15.Count > 0)
        {
            bool firstMinutelyElement = true;
            uri.Query += "&minutely_15=";
            foreach (var option in options.Minutely15)
            {
                if (firstMinutelyElement)
                {
                    uri.Query += option.ToString();
                    firstMinutelyElement = false;
                }
                else
                {
                    uri.Query += "," + option.ToString();
                }
            }
        }

        SetApiKeyIfNeeded(uri);
        return uri.ToString();
    }

    /// <summary>
    /// Combines a given url with an options object to create a url for GET requests
    /// </summary>
    /// <returns>url+queryString</returns>
    public string GetUrlWithOptions(GeocodingOptions options)
    {
        UriBuilder uri = new(GetBaseUrl(_geocodeApiUrl));

        // Now we check every property and set the value, if neccessary
        uri.Query = "name=" + options.Name;

        if (options.Count > 0)
            uri.Query += "&count=" + options.Count;

        if (options.Format != string.Empty)
            uri.Query += "&format=" + options.Format;

        if (options.Language != string.Empty)
            uri.Query += "&language=" + options.Language;

        SetApiKeyIfNeeded(uri);
        return uri.ToString();
    }

    /// <summary>
    /// Combines a given url with an options object to create a url for GET requests
    /// </summary>
    /// <returns>url+queryString</returns>
    public string GetUrlWithOptions(AirQualityOptions options)
    {
        UriBuilder uri = new(GetBaseUrl(_airQualityApiUrl));

        // Now we check every property and set the value, if neccessary
        uri.Query += "latitude=" + options.Latitude.ToString(CultureInfo.InvariantCulture);
        uri.Query += "&longitude=" + options.Longitude.ToString(CultureInfo.InvariantCulture);

        if (options.Domains != string.Empty)
            uri.Query += "&domains=" + options.Domains;

        if (options.Timeformat != string.Empty)
            uri.Query += "&timeformat=" + options.Timeformat;

        if (options.Timezone != string.Empty)
            uri.Query += "&timezone=" + options.Timezone;

        // Finally add hourly array
        if (options.Hourly.Count >= 0)
        {
            bool firstHourlyElement = true;
            uri.Query += "&hourly=";

            foreach (var option in options.Hourly)
            {
                if (firstHourlyElement)
                {
                    uri.Query += option.ToString();
                    firstHourlyElement = false;
                }
                else
                {
                    uri.Query += "," + option.ToString();
                }
            }
        }

        SetApiKeyIfNeeded(uri);
        return uri.ToString();
    }

    public string GetUrlWithOptions(ElevationOptions options)
    {
        UriBuilder uri = new(GetBaseUrl(_elevationApiUrl))
        {
            Query = $"latitude={options.Latitude.ToString(CultureInfo.InvariantCulture)}&longitude={options.Longitude.ToString(CultureInfo.InvariantCulture)}"
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
}
