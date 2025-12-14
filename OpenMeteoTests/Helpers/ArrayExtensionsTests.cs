using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo.Helpers;

namespace OpenMeteoTests.Helpers
{
    [TestClass]
    public class ArrayExtensionsTests
    {
        [TestMethod]
        public void ToIntArray_ReturnsEmptyForNullInput()
        {
            long[] input = null!;
            var result = input.ToIntArray();
            Assert.AreEqual(0, result.Length);
        }

        [TestMethod]
        public void ToIntArray_ConvertsLongsToInts()
        {
            long[] input = new long[] { 1L, 2L, 3L };
            var result = input.ToIntArray();
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, result);
        }

        [TestMethod]
        public void ToNullableIntArray_FromIntArray_ReturnsEmptyForNullInput()
        {
            int[] input = null!;
            var result = input.ToNullableIntArray();
            Assert.AreEqual(0, result.Length);
        }

        [TestMethod]
        public void ToNullableIntArray_FromIntArray_ConvertsToNullable()
        {
            int[] input = new int[] { 1, 2, 3 };
            var result = input.ToNullableIntArray();
            CollectionAssert.AreEqual(new int?[] { 1, 2, 3 }, result);
        }

        [TestMethod]
        public void ToNullableFloatArray_ReturnsEmptyForNullInput()
        {
            float[] input = null!;
            var result = input.ToNullableFloatArray();
            Assert.AreEqual(0, result.Length);
        }

        [TestMethod]
        public void ToNullableFloatArray_ConvertsToNullableAndHandlesNaN()
        {
            float[] input = new float[] { 1.0f, float.NaN, 2.5f };
            var result = input.ToNullableFloatArray();
            CollectionAssert.AreEqual(new float?[] { 1.0f, null, 2.5f }, result);
        }

        [TestMethod]
        public void ToNullableIntArray_FromLongArray_ReturnsEmptyForNullInput()
        {
            long[] input = null!;
            var result = input.ToNullableIntArray();
            Assert.AreEqual(0, result.Length);
        }

        [TestMethod]
        public void ToNullableIntArray_FromLongArray_ConvertsToNullable()
        {
            long[] input = new long[] { 1L, 2L, 3L };
            var result = input.ToNullableIntArray();
            CollectionAssert.AreEqual(new int?[] { 1, 2, 3 }, result);
        }
    }
}
