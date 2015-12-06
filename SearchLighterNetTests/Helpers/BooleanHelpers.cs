using NUnit.Framework;

namespace SearchLighterNetTests.Helpers
{
    public static class BooleanHelpers
    {
        public static void ShouldEqual(this bool b, bool other)
        {
            Assert.AreEqual(b, other);
        }

        public static void ShouldBeTrue(this bool b)
        {
            Assert.AreEqual(b, true);
        }

        public static void ShouldBeFalse(this bool b)
        {
            Assert.AreEqual(b, false);
        }
    }
}