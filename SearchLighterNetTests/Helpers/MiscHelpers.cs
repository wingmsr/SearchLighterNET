namespace SearchLighterNetTests.Helpers
{
    public static class MiscHelpers
    {
        public static void ThisTestCanBeIgnoredBecauseItDoesNotApply(this object o)
        {
            1.ShouldEqual(1);
        }
    }
}