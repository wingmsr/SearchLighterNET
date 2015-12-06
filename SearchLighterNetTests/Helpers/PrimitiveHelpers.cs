using System;
using NUnit.Framework;

namespace SearchLighterNetTests.Helpers
{
    public static class PrimitiveHelpers
    {
        public static void ShouldEqual(this double d, double other)
        {
            Assert.AreEqual(d, other);
        }

        public static void ShouldEqual(this int i, int other)
        {
            Assert.AreEqual(i, other);
        }

        public static void ShouldEqual(this long i, long other)
        {
            Assert.AreEqual(i, other);
        }

        public static void ShouldNotEqual(this long i, long other)
        {
            Assert.AreNotEqual(i, other);
        }

        public static void ShouldBeGreaterThan(this double d, double other)
        {
            var result = (d > other);
            result.ShouldEqual(true);
        }

        public static void ShouldBeGreaterThan(this int i, int other)
        {
            var result = (i > other);
            result.ShouldEqual(true);
        }

        public static void ShouldBeLessThanOne(this double d)
        {
            var result = (d < 1);
            result.ShouldEqual(true);
        }

        public static void ShouldBeLessThanOne(this int i)
        {
            var result = (i < 1);
            result.ShouldEqual(true);
        }

        public static void ShouldNotBeZero(this double d)
        {
            var result = Math.Abs(d) > 0.00000000000001;
            result.ShouldEqual(true);
        }

        public static void ShouldBeZero(this int i)
        {
            var result = (i == 0);
            result.ShouldEqual(true);
        }

        public static void ShouldNotBeZero(this int i)
        {
            var result = (i != 0);
            result.ShouldEqual(true);
        }

        public static void ShouldBeGreaterThanZero(this double d)
        {
            var result = d > 0;
            result.ShouldEqual(true);
        }

        public static void ShouldBeGreaterThanZero(this int i)
        {
            var result = i > 0;
            result.ShouldEqual(true);
        }
    }
}