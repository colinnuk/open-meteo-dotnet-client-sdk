namespace OpenMeteo.Weather.Ensemble.ResponseModel.Conversion;

internal static class EnsembleConversionConstants
{
    // Time format
    public const string Iso8601TimeFormat = "iso8601";
    
    // Aggregation types
    public const string AggregationNone = "none";
    public const string AggregationMean = "mean";
    public const string AggregationMin = "min";
    public const string AggregationMax = "max";
    public const string AggregationSum = "sum";
    public const string AggregationDominant = "dominant";
    public const string AggregationHours = "hours";
    
    // Unit suffixes
    public const string UnitSuffixMeters = "m";
    public const string UnitSuffixCentimeters = "cm";
    public const string UnitSuffixHectoPascals = "hPa";
    
    // Separators
    public const char UnderscoreSeparator = '_';
    public const string ToSeparator = "to";
    
    // Variable name format
    public const string VariableNameFormat = "variable_{0}";
}
