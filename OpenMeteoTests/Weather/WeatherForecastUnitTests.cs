using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo.Weather.Options;
using OpenMeteo.Weather.ResponseModel;

namespace OpenMeteoTests.Weather
{
 [TestClass]
 public class WeatherForecastUnitTests
 {
 [TestMethod]
 public void WeatherForecast_With_All_Options_Test()
 {
 WeatherForecastOptions options = new()
 {
 Hourly = HourlyOptions.All,
 Daily = DailyOptions.All,
 Models = WeatherModelOptions.All,
 Current = CurrentOptions.All,
 Minutely_15 = Minutely15Options.All
 };

 Assert.IsTrue(HourlyOptions.All.Parameter.All(p => options.Hourly.Parameter.Contains(p)));
 Assert.IsTrue(DailyOptions.All.Parameter.All(p => options.Daily.Parameter.Contains(p)));
 Assert.IsTrue(WeatherModelOptions.All.Parameter.All(p => options.Models.Parameter.Contains(p)));
 Assert.IsTrue(CurrentOptions.All.Parameter.All(p => options.Current.Parameter.Contains(p)));
 Assert.IsTrue(Minutely15Options.All.Parameter.All(p => options.Minutely_15.Parameter.Contains(p)));
 }
 }
}
