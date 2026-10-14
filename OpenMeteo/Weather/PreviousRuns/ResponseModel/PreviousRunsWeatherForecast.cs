namespace OpenMeteo.Weather.PreviousRuns.ResponseModel
{
    /// <summary>
    /// Previous model runs API response.
    /// </summary>
    public class PreviousRunsWeatherForecast
    {
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public float Elevation { get; set; }
        public float GenerationTime { get; set; }
        public int UtcOffset { get; set; }
        public string? Timezone { get; set; }
        public string? TimezoneAbbreviation { get; set; }
        public PreviousRunsHourly? Hourly { get; internal set; }
    }
}
