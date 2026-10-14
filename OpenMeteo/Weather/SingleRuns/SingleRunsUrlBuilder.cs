using OpenMeteo.Weather.Forecast;
using OpenMeteo.Weather.SingleRuns.Options;
using System;
using System.Globalization;

namespace OpenMeteo.Weather.SingleRuns
{
    /// <summary>
    /// Builds URLs for the Single Runs API.
    /// </summary>
    public class SingleRunsUrlBuilder : WeatherForecastUrlBuilder
    {
        protected override string DefaultSubdomain => "single-runs-api";

        public SingleRunsUrlBuilder() : base()
        {
        }

        public SingleRunsUrlBuilder(Uri customBaseUri) : base(customBaseUri)
        {
        }

        public SingleRunsUrlBuilder(string apiKey) : base(apiKey)
        {
        }

        public SingleRunsUrlBuilder(Uri customBaseUri, string apiKey) : base(customBaseUri, apiKey)
        {
        }

        public SingleRunsUrlBuilder WithOptions(SingleRunsOptions options)
        {
            if (options.Run == default)
                throw new ArgumentException("Run must be specified.", nameof(options.Run));
            if (options.Run.Kind != DateTimeKind.Utc)
                throw new ArgumentException("Run must be expressed in UTC.", nameof(options.Run));
            if (options.Run.Second != 0 || options.Run.Millisecond != 0 || options.Run.Ticks % TimeSpan.TicksPerMillisecond != 0)
                throw new ArgumentException("Run must not include seconds or fractional seconds.", nameof(options.Run));

            base.WithOptions(options);
            _urlBuilder.AddParameter("run", options.Run.ToString("yyyy-MM-dd'T'HH:mm", CultureInfo.InvariantCulture));
            return this;
        }
    }
}