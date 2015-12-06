using System;
using NUnit.Framework;

namespace SearchLighterNetTests.Helpers
{
    public static class StringHelpers
    {
        public static void ShouldEqualCaseSensitive(this string s, string other)
        {
            var compare = (string.Compare(s, other, StringComparison.CurrentCulture) == 0);
            Assert.AreEqual(compare, true);
        }

        public static void ShouldEqualIgnoreCase(this string s, string other)
        {
            var compare = (string.Compare(s, other, StringComparison.CurrentCultureIgnoreCase) == 0);
            Assert.AreEqual(compare, true);
        }
    }
}