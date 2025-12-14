using System.Linq;

namespace OpenMeteo.Helpers;
public static class ArrayExtensions
{
    public static int[] ToIntArray(this long[] longArray)
    {
        if (longArray == null) return [];
        return longArray.Select(l => (int)l).ToArray();
    }

    public static int?[] ToNullableIntArray(this int[] intArray)
    {
        if (intArray == null) return [];
        return intArray.Select(i => (int?)i).ToArray();
    }

    public static float?[] ToNullableFloatArray(this float[] floatArray)
    {
        if (floatArray == null) return [];
        return floatArray.Select(f => float.IsNaN(f) ? (float?)null : f).ToArray();
    }

    public static int?[] ToNullableIntArray(this long[] longArray)
    {
        if (longArray == null) return [];
        return longArray.Select(l => (int?)l).ToArray();
    }
}
