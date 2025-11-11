using System;

namespace OpenMeteo.Weather.ResponseModel
{
    public class Minutely15
    {
        public DateTimeOffset[]? Time { get; set; }
        public float?[]? Temperature2m { get; set; }
        public int?[]? RelativeHumidity2m { get; set; }
        public float?[]? DewPoint2m { get; set; }
        public float?[]? ApparentTemperature { get; set; }
        public float?[]? Precipitation { get; set; }
        public float?[]? Rain { get; set; }
        public float?[]? Snowfall { get; set; }
        public float?[]? SnowfallHeight { get; set; }
        public float?[]? FreezingLevelHeight { get; set; }
        public int?[]? WeatherCode { get; set; }
        public float?[]? WindSpeed10m { get; set; }
        public float?[]? WindSpeed80m { get; set; }
        public int?[]? WindDirection10m { get; set; }
        public int?[]? WindDirection80m { get; set; }
        public float?[]? WindGusts10m { get; set; }
        public float?[]? Visibility { get; set; }
        public float?[]? Cape { get; set; }
        public float?[]? LightningPotential { get; set; }
        public float?[]? ShortwaveRadiation { get; set; }
        public float?[]? DirectRadiation { get; set; }
        public float?[]? DiffuseRadiation { get; set; }
        public float?[]? DirectNormalIrradiance { get; set; }
        public float?[]? TerrestrialRadiation { get; set; }
        public float?[]? ShortwaveRadiationInstant { get; set; }
        public float?[]? DirectRadiationInstant { get; set; }
        public float?[]? DiffuseRadiationInstant { get; set; }
        public float?[]? DirectNormalIrradianceInstant { get; set; }
        public float?[]? TerrestrialRadiationInstant { get; set; }
    }
}
