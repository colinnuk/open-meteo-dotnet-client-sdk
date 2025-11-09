using openmeteo_sdk;
using System.Reflection;
using System.Linq;

namespace OpenMeteo.Helpers
{
    internal static class PropertyMappingHelper
    {
        public static void MapVariableToProperty<TTarget>(TTarget target, string propertyName, VariableWithValues variable)
        {
            var property = typeof(TTarget).GetProperty(propertyName, 
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (property == null) return;

            var propertyType = property.PropertyType;

            if (propertyType == typeof(float?[]))
            {
                var values = variable.GetValuesArray();
                if (values != null)
                    property.SetValue(target, values.ToNullableFloatArray());
            }
            else if (propertyType == typeof(int?[]))
            {
                var values = variable.GetValuesInt64Array();
                if (values != null)
                    property.SetValue(target, values.ToNullableIntArray());
            }
            else if (propertyType == typeof(float[]))
            {
                var values = variable.GetValuesArray();
                if (values != null)
                    property.SetValue(target, values);
            }
            else if (propertyType == typeof(int[]))
            {
                var values = variable.GetValuesInt64Array();
                if (values != null)
                    property.SetValue(target, values.Select(x => (int)x).ToArray());
            }
            else if (propertyType == typeof(float))
            {
                property.SetValue(target, variable.Values(0));
            }
            else if (propertyType == typeof(int))
            {
                property.SetValue(target, (int)variable.Values(0));
            }
        }

        private static string ConvertParameterNameToPropertyName(string parameterName)
        {
            if (string.IsNullOrEmpty(parameterName)) return parameterName;
            return char.ToUpper(parameterName[0]) + parameterName.Substring(1);
        }
    }
}
