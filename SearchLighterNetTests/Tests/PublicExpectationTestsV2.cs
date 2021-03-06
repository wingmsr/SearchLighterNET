﻿using NUnit.Framework;
using SearchLighterNetTests.Helpers;
using SearchLighterNET;

// ReSharper disable EmptyGeneralCatchClause

namespace SearchLighterNetTests.Tests
{
    [TestFixture]
    public class PublicExpectationTestsV2
    {
        [TestCase("why hello there sir", "hello there", "why 1hello there11 sir")]
        [TestCase("why HELLO there ma'am", "hello there", "why 1HELLO there11 ma'am")]
        [TestCase("why hello there sir", "HELLO there", "why 1hello there11 sir")]
        public void Vanilla(string initial, string find, string expected)
        {
            var sl = getSearchLighterWithTestSettings();
            var result = sl.GetDisplayString(initial, find);
            expected.ShouldEqualCaseSensitive(result);
        }

        [TestCase("why hell° there •a'am", "", "why hell^ there $a'am")]
        [TestCase("why HELL° there •ir", "", "why HELL^ there $ir")]
        public void CanSanitizeTestSanitizationChars(string initial, string find, string expected)
        {
            var sl = getSearchLighterWithTestSettings();
            var result = sl.GetDisplayString(initial, find);
            expected.ShouldEqualCaseSensitive(result);
        }

        [TestCase("why hell^ there •a'am", "hell° there", "why 1hell^ there11 $a'am")]
        [TestCase("why HELL^ there •ir", "hell° there", "why 1HELL^ there11 $ir")]
        [TestCase("why hell^ there •a'am", "HELL° there", "why 1hell^ there11 $a'am")]
        public void SearchSanitizationAllowsForExactMatching(string initial, string find, string expected)
        {
            var sl = getSearchLighterWithTestSettings();
            var result = sl.GetDisplayString(initial, find);
            expected.ShouldEqualCaseSensitive(result);
        }

        [TestCase("why hell° there •ir", "hell^ there", "why 1hell^ there11 $ir")]
        [TestCase("why HELL° there •a'am", "hell^ there", "why 1HELL^ there11 $a'am")]
        [TestCase("why hell° there •ir", "HELL^ there", "why 1hell^ there11 $ir")]
        public void TargetSanitizationAllowsForMatching(string initial, string find, string expected)
        {
            var sl = getSearchLighterWithTestSettings();
            var result = sl.GetDisplayString(initial, find);
            expected.ShouldEqualCaseSensitive(result);
        }

        [TestCase("why hell° there •a'am", "hell° there", "why 1hell^ there11 $a'am")]
        [TestCase("why HELL° there •ir", "hell° there", "why 1HELL^ there11 $ir")]
        [TestCase("why hell° there •a'am", "HELL° there", "why 1hell^ there11 $a'am")]
        public void SearchAndTargetSanitizationAllowsForMatching(string initial, string find, string expected)
        {
            var sl = getSearchLighterWithTestSettings();
            var result = sl.GetDisplayString(initial, find);
            expected.ShouldEqualCaseSensitive(result);
        }

        [TestCase("abc abc abc", "abc abc", "1abc abc11 2abc22")]
        [TestCase("abc abc abc abc", "abc abc", "1abc abc11 1abc abc11")]
        public void OverlappingExact(string initial, string find, string expected)
        {
            var sl = getSearchLighterWithTestSettings();
            var result = sl.GetDisplayString(initial, find);
            expected.ShouldEqualCaseSensitive(result);
        }

        [TestCase("me find me", "find me", "2me22 1find me11")]
        [TestCase("me find me °•", "find me °•", "2me22 1find me ^$11")]
        [TestCase("me find me °•", "find•br /° me °•", "2me22 1find me ^$11")]
        [TestCase("me find•br /° me °•", "find me °•", "2me22 2find22•_° 2me22 2^$22")]
        public void PartialLeadingIntoExact(string initial, string find, string expected)
        {
            var sl = getSearchLighterWithTestSettings();
            var result = sl.GetDisplayString(initial, find);
            expected.ShouldEqualCaseSensitive(result);
        }

        [TestCase("why hell°•br /° there •ir", "hell° there", "why 2hell^22•_° 2there22 $ir")]
        public void EscapeMarkupInTargetCase_EscapeSequenceDisruptsExactMatch(string initial, string find, string expected)
        {
            /*** note: sacrificing exact-match accuracy for better performance ***/
            var sl = getSearchLighterWithTestSettings();
            var result = sl.GetDisplayString(initial, find);
            expected.ShouldEqualCaseSensitive(result);
        }

        [TestCase("why hell° there •a'am", "hell°•br /° there", "why 1hell^ there11 $a'am")]
        public void EscapeMarkupInSearchButNotTarget_EscapeSequenceEliminatedFromSearchAllowsExactMatch(string initial, string find, string expected)
        {
            /*** note: scrubbing line-breaks from search terms ***/
            var sl = getSearchLighterWithTestSettings();
            var result = sl.GetDisplayString(initial, find);
            expected.ShouldEqualCaseSensitive(result);
        }

        [TestCase("why hell°•br /° there •ir", "hell°•br /° there", "why 2hell^22•_° 2there22 $ir")]
        public void EscapeMarkupInBothSearchAndTarget_EscapeSequenceEliminatedFromSearchPreventsExactMatch(string initial, string find, string expected)
        {
            var sl = getSearchLighterWithTestSettings();
            var result = sl.GetDisplayString(initial, find);
            expected.ShouldEqualCaseSensitive(result);
        }

        [TestCase("xyz abcdef qabcdef xyzabcdef xyzabcdef xyzq xyz abcdef", "abcdef xyz", "2xyz22 2abcdef22 q1abcdef xyz111abcdef xyz111abcdef xyz11q 2xyz22 2abcdef22")]
        public void ExactWithinPartials(string initial, string find, string expected)
        {
            var sl = getSearchLighterWithTestSettings();
            var result = sl.GetDisplayString(initial, find);
            expected.ShouldEqualCaseSensitive(result);
        }

        private static SearchLighter getSearchLighterWithTestSettings()
        {
            SearchLighter sl = new SearchLighter();

            sl.SetEscapeMarkupMap(SearchLighterConfigurationTools.TestLineBreakEscapeMarkupMap());
            sl.SetSanitizationMap(SearchLighterConfigurationTools.TestSanitizationMap());
            sl.HighlighterSetWordMinLength(2);
            sl.HighlighterSetExactMatchMinLength(2);
            sl.HighlighterClearSkipWords();
            sl.SetExactMatchOpenMarkup("1");
            sl.SetExactMatchCloseMarkup("11");
            sl.SetPartialMatchOpenMarkup("2");
            sl.SetPartialMatchCloseMarkup("22");

            return sl;
        }
    }
}
