using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo.Weather.Forecast;

namespace OpenMeteoTests.Weather.Forecast
{
    [TestClass]
    public class WeatherCodeHelperTests
    {
        [DataTestMethod]
        [DataRow(0, "Clear sky")]
        [DataRow(1, "Mainly clear")]
        [DataRow(2, "Partly cloudy")]
        [DataRow(3, "Overcast")]
        [DataRow(51, "Light drizzle")]
        [DataRow(53, "Moderate drizzle")]
        [DataRow(96, "Thunderstorm with light hail")]
        [DataRow(99, "Thunderstorm with heavy hail")]
        [DataRow(100, "Invalid weathercode")]
        public void Weather_Codes_To_String_Tests(int weatherCode, string expectedString)
        {
            string weatherCodeString = WeatherCodeHelper.WeathercodeToString(weatherCode);
            Assert.IsInstanceOfType(weatherCodeString, typeof(string));
            Assert.AreEqual(expectedString, weatherCodeString);
        }
    }
}