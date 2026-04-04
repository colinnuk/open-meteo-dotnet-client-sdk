<#
.SYNOPSIS
    Generates C# static classes from the regional GeoJSON coverage boundary files.

.DESCRIPTION
    Each regional weather model has a GeoJSON file in
    OpenMeteo/Weather/Forecast/RegionalGeoJSON/ that describes its spatial
    coverage boundary as a polygon. This script reads those files and emits a
    corresponding C# static class for each one containing the exterior ring
    coordinates as a compile-time array of (Longitude, Latitude) value tuples.

    The generated classes are consumed by RegionalModelPolygons.cs and
    RegionalModelCoverageHelper.cs to perform point-in-polygon checks without
    any file I/O at runtime - the data is compiled directly into the assembly,
    which avoids packaging/path problems when the library is distributed as a
    NuGet package.

.NOTES
    Re-run this script whenever a GeoJSON boundary file is added or updated,
    then commit the regenerated C# files alongside it.

    Only the exterior ring (coordinates[0] of the first feature) is extracted.
    Holes in the polygon are intentionally ignored for coverage purposes.

.EXAMPLE
    .\Scripts\Generate-RegionalCoverageClasses.ps1
#>

$repoRoot    = Split-Path $PSScriptRoot -Parent
$geojsonDir  = Join-Path $repoRoot "OpenMeteo\Weather\Forecast\RegionalGeoJSON"
$inv         = [System.Globalization.CultureInfo]::InvariantCulture

$files = Get-ChildItem "$geojsonDir\*.geojson"
if (-not $files) {
    Write-Error "No .geojson files found in $geojsonDir"
    exit 1
}

foreach ($file in $files) {
    $json   = Get-Content $file.FullName -Raw | ConvertFrom-Json
    $coords = $json.features[0].geometry.coordinates[0]

    # Convert snake_case filename to PascalCase class name, e.g. dwd_icon_d2 -> DwdIconD2
    $className = ($file.BaseName -split '_' |
                  ForEach-Object { $_.Substring(0,1).ToUpper() + $_.Substring(1) }) -join ''

    $coordLines = ($coords | ForEach-Object {
        $lon = [string]::Format($inv, "{0}", $_[0])
        $lat = [string]::Format($inv, "{0}", $_[1])
        "        ($lon, $lat),"
    }) -join "`n"

    $cs = @"
namespace OpenMeteo.Weather.Forecast.RegionalGeoJSON;

internal static class ${className}Coverage
{
    internal static readonly (double Longitude, double Latitude)[] ExteriorRing =
    [
$coordLines
    ];
}
"@

    $outputPath = Join-Path $geojsonDir "${className}Coverage.cs"
    [System.IO.File]::WriteAllText($outputPath, $cs, [System.Text.Encoding]::UTF8)
    Write-Host "Generated: $($file.BaseName) -> ${className}Coverage.cs ($($coords.Count) points)"
}

Write-Host "`nDone. $($files.Count) file(s) processed."
