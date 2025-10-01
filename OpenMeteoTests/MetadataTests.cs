using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo;
using System.Threading.Tasks;

namespace OpenMeteoTests;

[TestClass]
public class MetadataTests
{
    [DataTestMethod]
    [DataRow(WeatherModelOptionsParameter.ecmwf_ifs025)]
    [DataRow(WeatherModelOptionsParameter.ecmwf_aifs025_single)]
    [DataRow(WeatherModelOptionsParameter.icon_global)]
    [DataRow(WeatherModelOptionsParameter.icon_eu)]
    [DataRow(WeatherModelOptionsParameter.icon_d2)]
    [DataRow(WeatherModelOptionsParameter.meteofrance_arpege_world)]
    [DataRow(WeatherModelOptionsParameter.meteofrance_arpege_europe)]
    [DataRow(WeatherModelOptionsParameter.meteofrance_arome_france)]
    [DataRow(WeatherModelOptionsParameter.ukmo_uk_deterministic_2km)]
    [DataRow(WeatherModelOptionsParameter.ukmo_global_deterministic_10km)]
    [DataRow(WeatherModelOptionsParameter.gfs_global)]
    [DataRow(WeatherModelOptionsParameter.gfs_graphcast025)]
    [DataRow(WeatherModelOptionsParameter.gfs_hrrr)]
    [DataRow(WeatherModelOptionsParameter.ncep_nbm_conus)]
    [DataRow(WeatherModelOptionsParameter.gem_global)]
    [DataRow(WeatherModelOptionsParameter.gem_hrdps_continental)]
    [DataRow(WeatherModelOptionsParameter.gem_regional)]
    [DataRow(WeatherModelOptionsParameter.jma_gsm)]
    [DataRow(WeatherModelOptionsParameter.jma_msm)]
    [DataRow(WeatherModelOptionsParameter.meteofrance_arome_france_hd)]
    [DataRow(WeatherModelOptionsParameter.metno_nordic)]
    [DataRow(WeatherModelOptionsParameter.bom_access_global)]
    [DataRow(WeatherModelOptionsParameter.italia_meteo_arpae_icon_2i)]
    [DataRow(WeatherModelOptionsParameter.meteoswiss_icon_ch1)]
    [DataRow(WeatherModelOptionsParameter.meteoswiss_icon_ch2)]
    public async Task Metadata_Async_Test(WeatherModelOptionsParameter model)
    {
        OpenMeteoClient client = new();
        var res = await client.QueryWeatherForecastMetadata(model);

        Assert.IsNotNull(res);
    }
}
