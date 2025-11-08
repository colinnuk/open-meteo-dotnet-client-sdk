using System.Linq;

namespace OpenMeteo.Helpers;
internal static class ArrayExtensions
{
    public static int[] ToIntArray(this long[] longArray)
    {
        return longArray.Select(l => (int)l).ToArray();
    }

    public static int?[] ToNullableIntArray(this int[] intArray)
    {
        return intArray.Select(i => (int?)i).ToArray();
    }

    public static float?[] ToNullableFloatArray(this float[] floatArray)
    {
        return floatArray.Select(f => (float?)f).ToArray();
    }
}
