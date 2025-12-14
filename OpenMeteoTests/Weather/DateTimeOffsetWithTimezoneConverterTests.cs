using System;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo.Weather.ResponseModel;

namespace OpenMeteoTests.Weather
{
    [TestClass]
    public class DateTimeOffsetWithTimezoneConverterTests
    {
        [TestMethod]
        public void Read_ParsesDateWithCorrectOffset_GMT()
        {
            var converter = new DateTimeOffsetWithTimezoneConverter("GMT");
            var json = "\"2024-06-01T12:00\"";
            var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(json));
            reader.Read();
            var result = converter.Read(ref reader, typeof(DateTimeOffset), new JsonSerializerOptions());
            Assert.AreEqual(0, result.Offset.Hours); // GMT
            Assert.AreEqual(12, result.Hour);
        }

        [TestMethod]
        public void Read_ParsesDateWithCorrectOffset_EDT()
        {
            var converter = new DateTimeOffsetWithTimezoneConverter("America/New_York");
            var json = "\"2024-06-01T12:00\"";
            var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(json));
            reader.Read();
            var result = converter.Read(ref reader, typeof(DateTimeOffset), new JsonSerializerOptions());
            Assert.AreEqual(-4, result.Offset.Hours); // EDT
            Assert.AreEqual(12, result.Hour);
        }

        [TestMethod]
        public void Read_ParsesDateWithCorrectOffset_EST()
        {
            var converter = new DateTimeOffsetWithTimezoneConverter("America/New_York");
            var json = "\"2024-01-01T12:00\"";
            var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(json));
            reader.Read();
            var result = converter.Read(ref reader, typeof(DateTimeOffset), new JsonSerializerOptions());
            Assert.AreEqual(-5, result.Offset.Hours); // EST
            Assert.AreEqual(12, result.Hour);
        }

        [TestMethod]
        public void Read_EmptyString_ThrowsJsonException()
        {
            var converter = new DateTimeOffsetWithTimezoneConverter("America/New_York");
            var json = "\"\"";
            var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(json));
            reader.Read();

            // Sadly have to use a full try-catch because MSTest Assert.Throws doesnt work with the reader struct
            try
            {
                converter.Read(ref reader, typeof(DateTimeOffset), new JsonSerializerOptions());
                Assert.Fail("Expected JsonException was not thrown.");
            }
            catch (JsonException)
            {
                // Test passes
            }
        }

        [TestMethod]
        public void Read_InvalidDate_ThrowsJsonException()
        {
            var converter = new DateTimeOffsetWithTimezoneConverter("America/New_York");
            var json = "\"not-a-date\"";
            var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(json));
            reader.Read();

            // Sadly have to use a full try-catch because MSTest Assert.Throws doesnt work with the reader struct
            try
            {
                converter.Read(ref reader, typeof(DateTimeOffset), new JsonSerializerOptions());
                Assert.Fail("Expected JsonException was not thrown.");
            }
            catch (JsonException)
            {
                // Test passes
            }
        }
    }
}
