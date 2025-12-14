using openmeteo_sdk;
using System.Reflection;
using System.Linq;
using System;

namespace OpenMeteo.Helpers
{
    internal static class PropertyMappingHelper
    {
        public static void MapVariableToProperty<TTarget>(TTarget target, string propertyName, VariableWithValues variable)
        {
            var property = typeof(TTarget).GetProperty(propertyName,
           BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (property == null) return;

            switch (property.PropertyType)
            {
                case Type t when t == typeof(float?[]):
                    MapNullableFloatArray(target, property, variable);
                    break;
                case Type t when t == typeof(int?[]):
                    MapNullableIntArray(target, property, variable);
                    break;
                case Type t when t == typeof(float[]):
                    MapFloatArray(target, property, variable);  
                    break;
                case Type t when t == typeof(int[]):
                    MapIntArray(target, property, variable);
                    break;
                case Type t when t == typeof(float):
                    MapFloat(target, property, variable);
                    break;
                case Type t when t == typeof(int):
                    MapInt(target, property, variable);
                    break;
                case Type t when t == typeof(float?):
                    MapNullableFloat(target, property, variable);
                    break;
                case Type t when t == typeof(int?):
                    MapNullableInt(target, property, variable);
                    break;
            }
        }

        private static void MapNullableFloatArray<TTarget>(TTarget target, PropertyInfo property, VariableWithValues variable)
        {
            var values = variable.GetValuesArray();
            if (values != null)
            {
                property.SetValue(target, values.ToNullableFloatArray());
            }
        }

        private static void MapNullableIntArray<TTarget>(TTarget target, PropertyInfo property, VariableWithValues variable)
        {
            var int64Values = variable.GetValuesInt64Array();
            if (int64Values != null && int64Values.Length > 0)
            {
                property.SetValue(target, int64Values.ToNullableIntArray());
            }
            else
            {
                var floatValues = variable.GetValuesArray();
                if (floatValues != null && floatValues.Length > 0)
                {
                    var intArray = floatValues.Select(f => float.IsNaN(f) ? (int?)null : (int?)Math.Round(f)).ToArray();
                    property.SetValue(target, intArray);
                }
            }
        }

        private static void MapFloatArray<TTarget>(TTarget target, PropertyInfo property, VariableWithValues variable)
        {
            var values = variable.GetValuesArray();
            if (values != null)
            {
                property.SetValue(target, values);
            }
        }

        private static void MapIntArray<TTarget>(TTarget target, PropertyInfo property, VariableWithValues variable)
        {
            var int64Values = variable.GetValuesInt64Array();
            if (int64Values != null && int64Values.Length > 0)
            {
                property.SetValue(target, int64Values.Select(x => (int)x).ToArray());
            }
            else
            {
                var floatValues = variable.GetValuesArray();
                if (floatValues != null && floatValues.Length > 0)
                {
                    var intArray = floatValues.Select(f => (int)Math.Round(f)).ToArray();
                    property.SetValue(target, intArray);
                }
            }
        }

        private static void MapFloat<TTarget>(TTarget target, PropertyInfo property, VariableWithValues variable)
        {
            property.SetValue(target, variable.Values(0));
        }

        private static void MapInt<TTarget>(TTarget target, PropertyInfo property, VariableWithValues variable)
        {
            property.SetValue(target, (int)variable.Values(0));
        }

        private static void MapNullableFloat<TTarget>(TTarget target, PropertyInfo property, VariableWithValues variable)
        {
            var value = variable.Value;
            property.SetValue(target, float.IsNaN(value) ? (float?)null : (float?)value);
        }

        private static void MapNullableInt<TTarget>(TTarget target, PropertyInfo property, VariableWithValues variable)
        {
            var value = variable.Value;
            property.SetValue(target, float.IsNaN(value) ? (int?)null : (int?)Math.Round(value));
        }
    }
}
