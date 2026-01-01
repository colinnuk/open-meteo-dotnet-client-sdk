using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo.Weather.Ensemble.ResponseModel.Conversion;

namespace OpenMeteoTests.Weather.Ensemble;

[TestClass]
public class EnsembleMemberParserTests
{
    [TestMethod]
    public void TryParseMember_ValidMemberPattern_ReturnsTrue()
    {
        var result = EnsembleMemberParser.TryParseMember(
            "temperature_2m_member01",
            out string baseName,
            out int memberNumber);

        Assert.IsTrue(result);
        Assert.AreEqual("temperature_2m", baseName);
        Assert.AreEqual(1, memberNumber);
    }

    [TestMethod]
    public void TryParseMember_MultipleDigitMember_ParsesCorrectly()
    {
        var result = EnsembleMemberParser.TryParseMember(
            "precipitation_member20",
            out string baseName,
            out int memberNumber);

        Assert.IsTrue(result);
        Assert.AreEqual("precipitation", baseName);
        Assert.AreEqual(20, memberNumber);
    }

    [TestMethod]
    public void TryParseMember_ComplexVariableName_ParsesCorrectly()
    {
        var result = EnsembleMemberParser.TryParseMember(
            "temperature_2m_max_member05",
            out string baseName,
            out int memberNumber);

        Assert.IsTrue(result);
        Assert.AreEqual("temperature_2m_max", baseName);
        Assert.AreEqual(5, memberNumber);
    }

    [TestMethod]
    public void TryParseMember_PressureLevelVariable_ParsesCorrectly()
    {
        var result = EnsembleMemberParser.TryParseMember(
            "temperature_850hPa_member10",
            out string baseName,
            out int memberNumber);

        Assert.IsTrue(result);
        Assert.AreEqual("temperature_850hPa", baseName);
        Assert.AreEqual(10, memberNumber);
    }

    [TestMethod]
    public void TryParseMember_NoMemberSuffix_ReturnsTrueAsMember0()
    {
        var result = EnsembleMemberParser.TryParseMember(
            "temperature_2m",
            out string baseName,
            out int memberNumber);

        Assert.IsTrue(result, "Base variable should be treated as member 0");
        Assert.AreEqual("temperature_2m", baseName);
        Assert.AreEqual(0, memberNumber, "Base variable should be member 0");
    }

    [TestMethod]
    public void TryParseMember_InvalidMemberFormat_ReturnsTrueAsMember0()
    {
        var result = EnsembleMemberParser.TryParseMember(
            "temperature_2m_memberABC",
            out string baseName,
            out int memberNumber);

        Assert.IsTrue(result, "Invalid member format should be treated as base variable (member 0)");
        Assert.AreEqual("temperature_2m_memberABC", baseName);
        Assert.AreEqual(0, memberNumber);
    }

    [TestMethod]
    public void TryParseMember_EmptyString_ReturnsTrueAsMember0()
    {
        var result = EnsembleMemberParser.TryParseMember(
            "",
            out string baseName,
            out int memberNumber);

        Assert.IsTrue(result, "Empty string should be treated as member 0");
        Assert.AreEqual("", baseName);
        Assert.AreEqual(0, memberNumber);
    }

    [TestMethod]
    [DataRow("temperature_2m_member01", "Temperature2m")]
    [DataRow("windspeed_10m_member05", "Windspeed10m")]
    [DataRow("temperature_2m_max_member10", "Temperature2mMax")]
    [DataRow("cloudcover_mean_member02", "CloudcoverMean")]
    [DataRow("soil_temperature_0_to_10cm_member01", "SoilTemperature0To10cm")]
    [DataRow("et0_fao_evapotranspiration_member03", "Et0FaoEvapotranspiration")]
    public void PropertyNameConversion_VariousFormats_ConvertsCorrectly(string variableName, string expectedPropertyName)
    {
        // This test verifies that the snake_case to PascalCase conversion works
        EnsembleMemberParser.TryParseMember(variableName, out string baseName, out _);
        
        var parts = baseName.Split('_');
        var propertyName = string.Join("", parts.Select(p => 
            p.Length > 0 ? char.ToUpper(p[0]) + p.Substring(1) : p));
        
        Assert.AreEqual(expectedPropertyName, propertyName);
    }
}
