using System;
using NUnit.Framework;

namespace SearchLighterNetTests.Helpers
{
    public static class DateTimeHelpers
    {
        public static void ShouldEqual(this DateTime dt, DateTime other)
        {
            Assert.AreEqual(dt.Ticks, other.Ticks);
        }

        public static void ShouldEqual(this DateTime? dt, DateTime? other)
        {
            Assert.AreEqual((dt ?? DateTime.MinValue).Ticks, (other ?? DateTime.MinValue).Ticks);
        }
    }
}