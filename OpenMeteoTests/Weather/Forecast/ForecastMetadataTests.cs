using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo;
using OpenMeteo.Weather.Forecast.Options;
using System.Threading.Tasks;

namespace OpenMeteoTests.Weather.Forecast;

[TestClass]
[TestCategory(TestCategoryConstants.Integration)]
public class ForecastMetadataTests
{
    [DataTestMethod]
    [DataRow(WeatherModelOptionsParameter.ecmwf_ifs)]
    [DataRow(WeatherModelOptionsParameter.icon_global)]
    [DataRow(WeatherModelOptionsParameter.meteofrance_arpege_world)]
    [DataRow(WeatherModelOptionsParameter.ukmo_global_deterministic_10km)]
    [DataRow(WeatherModelOptionsParameter.gfs_global)]
    [DataRow(WeatherModelOptionsParameter.gem_global)]
    [DataRow(WeatherModelOptionsParameter.jma_gsm)]
    [DataRow(WeatherModelOptionsParameter.metno_nordic)]
    [DataRow(WeatherModelOptionsParameter.bom_access_global)]
    [DataRow(WeatherModelOptionsParameter.italia_meteo_arpae_icon_2i)]
    [DataRow(WeatherModelOptionsParameter.meteoswiss_icon_ch1)]
    [DataRow(WeatherModelOptionsParameter.ncep_aigfs025)]
    [DataRow(WeatherModelOptionsParameter.ncep_hgefs025_ensemble_mean)]
    public async Task Metadata_Async_Test(WeatherModelOptionsParameter model)
    {
        OpenMeteoClient client = new();
        var res = await client.QueryWeatherForecastMetadata(model);

        Assert.IsNotNull(res);
    }
}
