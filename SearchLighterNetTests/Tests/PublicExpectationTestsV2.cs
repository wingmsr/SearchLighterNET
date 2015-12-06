using NUnit.Framework;
using SearchLighterNetTests.Helpers;
using SearchLighterNET;

// ReSharper disable EmptyGeneralCatchClause

namespace SearchLighterNetTests.Tests
{
    [TestFixture]
    public class PublicExpectationTestsV2
    {
        [TestCase("why hello there sir", "hello there", "why 1hello there11 sir")]
        [TestCase("why HELLO there sir", "hello there", "why 1HELLO there11 sir")]
        [TestCase("why hello there sir", "HELLO there", "why 1hello there11 sir")]
        public void Vanilla(string initial, string find, string expected)
        {
            setTestSettings();
            var result = SearchLighter.GetDisplayString(initial, find);
            expected.ShouldEqualCaseSensitive(result);
        }

        [TestCase("why hell° there •ir", "", "why hell^ there $ir")]
        [TestCase("why HELL° there •ir", "", "why HELL^ there $ir")]
        public void CanSanitizeTestSanitizationChars(string initial, string find, string expected)
        {
            setTestSettings();
            var result = SearchLighter.GetDisplayString(initial, find);
            expected.ShouldEqualCaseSensitive(result);
        }

        [TestCase("why hell^ there •ir", "hell° there", "why 1hell^ there11 $ir")]
        [TestCase("why HELL^ there •ir", "hell° there", "why 1HELL^ there11 $ir")]
        [TestCase("why hell^ there •ir", "HELL° there", "why 1hell^ there11 $ir")]
        public void SearchSanitizationAllowsForExactMatching(string initial, string find, string expected)
        {
            setTestSettings();
            var result = SearchLighter.GetDisplayString(initial, find);
            expected.ShouldEqualCaseSensitive(result);
        }

        [TestCase("why hell° there •ir", "hell^ there", "why 1hell^ there11 $ir")]
        [TestCase("why HELL° there •ir", "hell^ there", "why 1HELL^ there11 $ir")]
        [TestCase("why hell° there •ir", "HELL^ there", "why 1hell^ there11 $ir")]
        public void TargetSanitizationAllowsForMatching(string initial, string find, string expected)
        {
            setTestSettings();
            var result = SearchLighter.GetDisplayString(initial, find);
            expected.ShouldEqualCaseSensitive(result);
        }

        [TestCase("why hell° there •ir", "hell° there", "why 1hell^ there11 $ir")]
        [TestCase("why HELL° there •ir", "hell° there", "why 1HELL^ there11 $ir")]
        [TestCase("why hell° there •ir", "HELL° there", "why 1hell^ there11 $ir")]
        public void SearchAndTargetSanitizationAllowsForMatching(string initial, string find, string expected)
        {
            setTestSettings();
            var result = SearchLighter.GetDisplayString(initial, find);
            expected.ShouldEqualCaseSensitive(result);
        }

        [TestCase("abc abc abc", "abc abc", "1abc abc11 2abc22")]
        [TestCase("abc abc abc abc", "abc abc", "1abc abc11 1abc abc11")]
        public void OverlappingExact(string initial, string find, string expected)
        {
            setTestSettings();
            var result = SearchLighter.GetDisplayString(initial, find);
            expected.ShouldEqualCaseSensitive(result);
        }

        [TestCase("me find me", "find me", "2me22 1find me11")]
        [TestCase("me find me °•", "find me °•", "2me22 1find me ^$11")]
        [TestCase("me find me °•", "find•br /° me °•", "2me22 1find me ^$11")]
        [TestCase("me find•br /° me °•", "find me °•", "2me22 2find22•_° 2me22 2^$22")]
        public void PartialLeadingIntoExact(string initial, string find, string expected)
        {
            setTestSettings();
            //1/0;//todo: resume here with look-ahead for exact match within range of partial
            var result = SearchLighter.GetDisplayString(initial, find);
            expected.ShouldEqualCaseSensitive(result);
        }

        [TestCase("why hell°•br /° there •ir", "hell° there", "why 2hell^22•_° 2there22 $ir")]
        public void EscapeMarkupInTargetCase_EscapeSequenceDisruptsExactMatch(string initial, string find, string expected)
        {
            /*** note: sacrificing exact-match accuracy for better performance ***/
            setTestSettings();
            var result = SearchLighter.GetDisplayString(initial, find);
            expected.ShouldEqualCaseSensitive(result);
        }

        [TestCase("why hell° there •ir", "hell°•br /° there", "why 1hell^ there11 $ir")]
        public void EscapeMarkupInSearchButNotTarget_EscapeSequenceEliminatedFromSearchAllowsExactMatch(string initial, string find, string expected)
        {
            /*** note: scrubbing line-breaks from search terms ***/
            setTestSettings();
            var result = SearchLighter.GetDisplayString(initial, find);
            expected.ShouldEqualCaseSensitive(result);
        }

        [TestCase("why hell°•br /° there •ir", "hell°•br /° there", "why 2hell^22•_° 2there22 $ir")]
        public void EscapeMarkupInBothSearchAndTarget_EscapeSequenceEliminatedFromSearchPreventsExactMatch(string initial, string find, string expected)
        {
            setTestSettings();
            var result = SearchLighter.GetDisplayString(initial, find);
            expected.ShouldEqualCaseSensitive(result);
        }

        [TestCase("xyz abcdef qabcdef xyzabcdef xyzabcdef xyzq xyz abcdef", "abcdef xyz", "2xyz22 2abcdef22 q1abcdef xyz111abcdef xyz111abcdef xyz11q 2xyz22 2abcdef22")]
        public void ExactWithinPartials(string initial, string find, string expected)
        {
            setTestSettings();
            var result = SearchLighter.GetDisplayString(initial, find);
            expected.ShouldEqualCaseSensitive(result);
        }

        private static void setTestSettings()
        {
            SearchLighter.HighlighterResetToDefaults();
            SearchLighter.SetEscapeMarkupMap(SearchLighterConfigurationTools.TestLineBreakEscapeMarkupMap());
            SearchLighter.SetSanitizationMap(SearchLighterConfigurationTools.TestSanitizationMap());
            SearchLighter.HighlighterSetWordMinLength(2);
            SearchLighter.HighlighterSetExactMatchMinLength(2);
            SearchLighter.HighlighterClearSkipWords();
            SearchLighter.SetExactMatchOpenMarkup("1");
            SearchLighter.SetExactMatchCloseMarkup("11");
            SearchLighter.SetPartialMatchOpenMarkup("2");
            SearchLighter.SetPartialMatchCloseMarkup("22");
        }
    }
}
