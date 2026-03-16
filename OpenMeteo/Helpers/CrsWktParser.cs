using System.Globalization;
using System.Text.RegularExpressions;
using OpenMeteo.Weather.Metadata;

namespace OpenMeteo.Helpers;

public static partial class CrsWktParser
{
    [GeneratedRegex(@"BBOX\[(-?[\d.]+),\s*(-?[\d.]+),\s*(-?[\d.]+),\s*(-?[\d.]+)\]", RegexOptions.IgnoreCase)]
    private static partial Regex BboxRegex();

    public static BoundingBox? ParseBoundingBox(string wkt)
    {
        var match = BboxRegex().Match(wkt);
        if (!match.Success)
            return null;

        return new BoundingBox(
            double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture),
            double.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture),
            double.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture),
            double.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture));
    }
}
