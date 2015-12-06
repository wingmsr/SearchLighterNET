namespace SearchLighterNetTests.Helpers
{
    public static class ObjectHelpers
    {
        public static void ShouldNotBeNull(this object o)
        {
            var result = o != null;
            result.ShouldEqual(true);
        }

        public static void ShouldBeNull(this object o)
        {
            var result = o == null;
            result.ShouldEqual(true);
        }
    }
}