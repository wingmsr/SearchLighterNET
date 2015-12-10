using System;
using NUnit.Framework;
using SearchLighterNetTests.Helpers;
using SearchLighterNET;

// ReSharper disable EmptyGeneralCatchClause

namespace SearchLighterNetTests.Tests
{
    [TestFixture]
    public class PublicExpectationTests
    {
        [Test]
        public void GoldenPath()
        {
            /* Note: The HTML-friendly default "escape-markup" includes the most common variants of new-line characters and line-break HTML.
             * Other markup is partially HTML-encoded by default, i.e. "angle-brackets" are HTML-encoded to "sanitize" the output.
             * See the Unit-Test suite for expected behavior of edge-cases.
             */
            string find = "jumped OVER";
            string text = "The quick brown fox\r\njumped over<BR/>and over the <lazy>dog</lazy>.";
            string highlighted = new SearchLighter().GetDisplayString(text, find);

            string expectedOutput =
                "The quick brown fox<br /><span class=\"hlt1\">jumped over</span><br />and <span class=\"hlt2\">over</span> the &lt;lazy&gt;dog&lt;/lazy&gt;.";
            Assert.AreEqual(string.Compare(highlighted, expectedOutput, StringComparison.CurrentCulture), 0);
        }

        [TestCase("?@[\\]the quick brown FOX_`{|", "BROWN fox", "?@[\\]the quick STARTbrown FOXSTOP_`{|")]
        public void CanMakeAsciiCaseInsensitiveHighlight(string initial, string find, string expected)
        {
            var sl = new SearchLighter();
            sl.SetExactMatchOpenMarkup("START");
            sl.SetExactMatchCloseMarkup("STOP");
            var result = sl.GetDisplayString(initial, find);

            expected.ShouldEqualCaseSensitive(result);
        }

        [TestCase("?@[\\]the quick brown FOX_`{|", "BROWN fox", "?@[\\]the quick 1brown FOX2_`{|")]
        public void CanMakeAsciiCaseInsensitiveHighlightWithSingleCharTag(string initial, string find, string expected)
        {
            var sl = new SearchLighter();
            sl.SetExactMatchOpenMarkup("1");
            sl.SetExactMatchCloseMarkup("2");
            var result = sl.GetDisplayString(initial, find);

            expected.ShouldEqualCaseSensitive(result);
        }

        [TestCase("this has\n a good line N line break.", "a GOOD line", "this has<br /> 11a good line111 N 2222line22222 break.")]
        [TestCase("this has\r\n a good line RN line break.", "a GOOD line", "this has<br /> 11a good line111 RN 2222line22222 break.")]
        [TestCase("hello there", "here", "hello t11here111")]
        [TestCase("hello there", "here you go", "hello t2222here22222")]
        [TestCase("hello <span>there</span>", "here you go", "hello &lt;span&gt;t2222here22222&lt;/span&gt;")]
        [TestCase("hello <<span>there</span>", "here you go", "hello &lt;&lt;span&gt;t2222here22222&lt;/span&gt;")]
        [TestCase("hello\nthere", "here", "hello<br />t11here111")]
        [TestCase("hello\n<br />there", "here", "hello<br /><br />t11here111")]
        [TestCase("", "", "", TestName = "empty empty")]
        [TestCase(".", "", ".", TestName = "dot empty")]
        [TestCase("", ".", "", TestName = "empty dot")]
        [TestCase(".", ".", "11.111", TestName = "dot dot - search should respect exact match for punctuation/word boundaries")]
        [TestCase("... ...", "...", "11...111 11...111")]
        [TestCase("xxxxxx", "xxxx", "11xxxx1112222xx22222")]
        [TestCase("xxxxxxxx", "xxxx", "11xxxx11111xxxx111")]
        [TestCase(".....", "...", "11...1112222..22222", TestName = "word-boundary characters are excluded from 3 char threshold due to the fact that they always yield a 'word' boundary result (1)")]
        [TestCase("-.,-.,", "-.,", "11-.,11111-.,111", TestName = "word-boundary characters are excluded from 3 char threshold due to the fact that they always yield a 'word' boundary result (2)")]
        [TestCase("......", "...", "11...11111...111", TestName = "tandem repeat dots; do not orphan tail highlight closing tag")]
        public void CanGetHighlightedStringBasic(string look, string find, string expected)
        {
            var sl = new SearchLighter();
            sl.HighlighterSetExactMatchMinLength(1);
            sl.HighlighterSetWordMinLength(2);
            sl.SetExactMatchOpenMarkup("11");
            sl.SetExactMatchCloseMarkup("111");
            sl.SetPartialMatchOpenMarkup("2222");
            sl.SetPartialMatchCloseMarkup("22222");

            var t = sl.GetDisplayString(look, find);
            expected.ShouldEqualCaseSensitive(t);
        }

        [TestCase("bc def bc def bc def", "c def bc", "b1c def bc11 2def22 2bc22 2def22", TestName = "overlapping exact matches takesfirst only and the rest fall back to word-partial matches")]
        public void SecondaryDoesNotPreemptExactMatch(string look, string find, string expected)
        {
            var sl = new SearchLighter();
            sl.HighlighterSetExactMatchMinLength(1);
            sl.HighlighterSetWordMinLength(1);
            sl.HighlighterClearSkipWords();
            sl.SetExactMatchOpenMarkup("1");
            sl.SetExactMatchCloseMarkup("11");
            sl.SetPartialMatchOpenMarkup("2");
            sl.SetPartialMatchCloseMarkup("22");

            var t = sl.GetDisplayString(look, find);
            expected.ShouldEqualCaseSensitive(t);
        }

        [TestCase("aba baba aba baba", "baba x aba", "2aba22 2baba22 2aba22 2baba22")]
        [TestCase("abababaabababa", "baba x aba", "2aba222baba222aba222baba22")]
        public void SecondaryPreemptsLongerSecondaryMatchToPreservePerformance(string look, string find, string expected)
        {
            var sl = new SearchLighter();
            sl.HighlighterResetToDefaults();
            sl.HighlighterSetExactMatchMinLength(1);
            sl.HighlighterSetWordMinLength(1);
            sl.HighlighterClearSkipWords();
            sl.SetExactMatchOpenMarkup("1");
            sl.SetExactMatchCloseMarkup("11");
            sl.SetPartialMatchOpenMarkup("2");
            sl.SetPartialMatchCloseMarkup("22");

            var t = sl.GetDisplayString(look, find);
            expected.ShouldEqualCaseSensitive(t);
        }

        [TestCase(1, 1, "a ab abc abcd abcde abcdef", "b", "a a1b11 a1b11c a1b11cd a1b11cde a1b11cdef")]
        [TestCase(2, 2, "a ab abc abcd abcde abcdef", "b", "a ab abc abcd abcde abcdef")]
        [TestCase(1, 2, "a ab abc abcd abcde abcdef", "b", "a a1b11 a1b11c a1b11cd a1b11cde a1b11cdef")]

        [TestCase(1, 1, "a ab abc abcd abcde abcdef", "bc", "a a2b22 a1bc11 a1bc11d a1bc11de a1bc11def")]
        [TestCase(2, 2, "a ab abc abcd abcde abcdef", "bc", "a ab a1bc11 a1bc11d a1bc11de a1bc11def")]
        [TestCase(3, 3, "a ab abc abcd abcde abcdef", "bc", "a ab abc abcd abcde abcdef")]
        [TestCase(1, 2, "a ab abc abcd abcde abcdef", "bc", "a a2b22 a1bc11 a1bc11d a1bc11de a1bc11def")]
        [TestCase(1, 3, "a ab abc abcd abcde abcdef", "bc", "a a2b22 a1bc11 a1bc11d a1bc11de a1bc11def")]

        [TestCase(1, 1, "a ab abc abcd abcde abcdef", "bcd", "a a2b22 a2bc22 a1bcd11 a1bcd11e a1bcd11ef")]
        [TestCase(2, 2, "a ab abc abcd abcde abcdef", "bcd", "a ab a2bc22 a1bcd11 a1bcd11e a1bcd11ef")]
        [TestCase(3, 3, "a ab abc abcd abcde abcdef", "bcd", "a ab abc a1bcd11 a1bcd11e a1bcd11ef")]
        [TestCase(4, 4, "a ab abc abcd abcde abcdef", "bcd", "a ab abc abcd abcde abcdef")]
        [TestCase(5, 5, "a ab abc abcd abcde abcdef", "bcd", "a ab abc abcd abcde abcdef")]
        [TestCase(1, 2, "a ab abc abcd abcde abcdef", "bcd", "a a2b22 a2bc22 a1bcd11 a1bcd11e a1bcd11ef")]
        [TestCase(1, 5, "a ab abc abcd abcde abcdef", "bcd", "a a2b22 a2bc22 a1bcd11 a1bcd11e a1bcd11ef")]

        [TestCase(1, 1, "a ab abc abcd abcde abcdef", "bcde", "a a2b22 a2bc22 a2bcd22 a1bcde11 a1bcde11f")]
        [TestCase(4, 4, "a ab abc abcd abcde abcdef", "bcde", "a ab abc abcd a1bcde11 a1bcde11f")]
        [TestCase(5, 5, "a ab abc abcd abcde abcdef", "bcde", "a ab abc abcd abcde abcdef")]

        [TestCase(1, 1, "a ab abc abcd abcde abcdef", "a a", "1a a11b 2a22bc 2a22bcd 2a22bcde 2a22bcdef")]
        [TestCase(2, 2, "a ab abc abcd abcde abcdef", "a a", "1a a11b abc abcd abcde abcdef")]
        [TestCase(3, 3, "a ab abc abcd abcde abcdef", "a a", "1a a11b abc abcd abcde abcdef")]
        [TestCase(4, 4, "a ab abc abcd abcde abcdef", "a a", "a ab abc abcd abcde abcdef")]

        [TestCase(1, 1, "a a ab abc abcd abcde abcdef", "a ab", "2a22 1a ab11 2ab22c 2ab22cd 2ab22cde 2ab22cdef")]
        [TestCase(2, 2, "a a ab abc abcd abcde abcdef", "a ab", "a 1a ab11 2ab22c 2ab22cd 2ab22cde 2ab22cdef")]
        [TestCase(3, 3, "a a ab abc abcd abcde abcdef", "a ab", "a 1a ab11 abc abcd abcde abcdef")]

        [TestCase(1, 1, "abc a", "bc a", "2a221bc a11", TestName = "'bc a' edge case - mini")]
        [TestCase(1, 1, "a ab abc abcd abcde abcdef", "bc a", "2a22 2a222b22 2a221bc a112bc22d 2a222bc22de 2a222bc22def", TestName = "'bc a' edge case")]

        [TestCase(1, 1, "a ab abc abcd abcde abcdef", "bc abcd a", "2a22 2ab22 2a221bc abcd a112bc22de 2abcd22ef", TestName = "'bc abcd a' edge case")]
        [TestCase(2, 2, "a ab abc abcd abcde abcdef", "bc abcd a", "a 2ab22 a1bc abcd a112bc22de 2abcd22ef")]
        [TestCase(9, 9, "a ab abc abcd abcde abcdef", "bc abcd a", "a ab a1bc abcd a11bcde abcdef")]
        [TestCase(10, 10, "a ab abc abcd abcde abcdef", "bc abcd a", "a ab abc abcd abcde abcdef")]
        [TestCase(1, 2, "a ab abc abcd abcde abcdef", "bc abcd a", "2a22 2ab22 a1bc abcd a112bc22de 2abcd22ef")]
        [TestCase(1, 3, "a ab abc abcd abcde abcdef", "bc abcd a", "2a22 2ab22 a1bc abcd a11bcde 2abcd22ef")]
        [TestCase(1, 5, "a ab abc abcd abcde abcdef", "bc abcd a", "a ab a1bc abcd a11bcde abcdef")]

        [TestCase(2, 2, "a ab abc abcd abcde abcdef", "bc abcd ab", "a 2ab22 a1bc abcd ab11cde 2abcd22ef" , TestName = "IMPORTANT - secondary does not preempt exact match and alpha precedence of ab over bc")]
        [TestCase(3, 3, "a ab abc abcd abcde abcdef", "bc abcd ab", "a ab a1bc abcd ab11cde 2abcd22ef")]
        [TestCase(10, 10, "a ab abc abcd abcde abcdef", "bc abcd ab", "a ab a1bc abcd ab11cde abcdef")]
        [TestCase(11, 11, "a ab abc abcd abcde abcdef", "bc abcd ab", "a ab abc abcd abcde abcdef")]
        [TestCase(1, 3, "a ab abc abcd abcde abcdef", "bc abcd ab", "2a22 2ab22 a1bc abcd ab11cde 2abcd22ef")]
        [TestCase(1, 5, "a ab abc abcd abcde abcdef", "bc abcd ab", "a ab a1bc abcd ab11cde abcdef")]
        public void CanGetHighlightedStringWithMinimumCharThresholds(int minExactMatchLength, int minWordMatchLength, string look, string find, string expected)
        {
            var sl = new SearchLighter();
            sl.HighlighterResetToDefaults();
            sl.HighlighterSetExactMatchMinLength(minExactMatchLength);
            sl.HighlighterSetWordMinLength(minWordMatchLength);
            sl.HighlighterClearSkipWords();
            sl.SetExactMatchOpenMarkup("1");
            sl.SetExactMatchCloseMarkup("11");
            sl.SetPartialMatchOpenMarkup("2");
            sl.SetPartialMatchCloseMarkup("22");

            var t = sl.GetDisplayString(look, find);
            expected.ShouldEqualCaseSensitive(t);
        }

        [TestCase(1, 1, "this has<br> thy faulty line break", "THY GOOD LINE", "this has&lt;br&gt; 2thy22 faulty 2line22 break")]
        [TestCase(1, 1, "this has<br> a faulty line break", "A GOOD LINE", "this h2a22s&lt;br&gt; 2a22 f2a22ulty 2line22 bre2a22k")]
        public void CanGetHighlightedStringHandlesEmbeddedLineBreaks(int minExactMatchLength, int minWordMatchLength, string look, string find, string expected)
        {
            var sl = new SearchLighter();
            sl.HighlighterResetToDefaults();
            sl.HighlighterSetExactMatchMinLength(minExactMatchLength);
            sl.HighlighterSetWordMinLength(minWordMatchLength);
            sl.HighlighterClearSkipWords();
            sl.SetExactMatchOpenMarkup("1");
            sl.SetExactMatchCloseMarkup("11");
            sl.SetPartialMatchOpenMarkup("2");
            sl.SetPartialMatchCloseMarkup("22");

            var t = sl.GetDisplayString(look, find);
            expected.ShouldEqualCaseSensitive(t);
        }

        [TestCase("this has<br> a faulty line break.", "this has&lt;br&gt; a faulty line break.", TestName = "faulty line break")]
        [TestCase("this has<BR /> a good line BR line break.", "this has<br /> a good line BR line break.", TestName = "good <br /> line break")]
        [TestCase("this has\n a good line N line break.", "this has<br /> a good line N line break.", TestName = "good \\n line break")]
        [TestCase("this has\r\n a good line RN line break.", "this has<br /> a good line RN line break.", TestName = "good \\r\\n line break")]
        [TestCase("this has\r\n\n a good <br />\r\n\n<br />line RN line break.", "this has<br /><br /> a good <br /><br /><br /><br />line RN line break.", TestName = "multiple consecutive line-breaks")]
        [TestCase("the quick BROWN@fox jumped OVER the lazy-dog!", "the quick BROWN@fox jumped OVER the lazy-dog!")]
        [TestCase("the quick BROWN@fox <xxx>jumped</xxx> OVER the lazy-dog!", "the quick BROWN@fox &lt;xxx&gt;jumped&lt;/xxx&gt; OVER the lazy-dog!")]
        [TestCase("the quick BROWN@fox <xxx>\r\njumped</xxx> OVER the lazy-dog!", "the quick BROWN@fox &lt;xxx&gt;<br />jumped&lt;/xxx&gt; OVER the lazy-dog!")]
        [TestCase("the quick BROWN@fox <xxx>\r\njumped</xxx> OVER the lazy-dog!\n", "the quick BROWN@fox &lt;xxx&gt;<br />jumped&lt;/xxx&gt; OVER the lazy-dog!<br />")]
        [TestCase("the quick BROWN@fox <xxx>\r\njumped</xxx> OVER the lazy-dog!\n\n", "the quick BROWN@fox &lt;xxx&gt;<br />jumped&lt;/xxx&gt; OVER the lazy-dog!<br /><br />")]
        public void CanGetNormalString(string initial, string expected)
        {
            var t = new SearchLighter().GetDisplayString(initial, "");
            expected.ShouldEqualCaseSensitive(t);
        }

        [TestCase("Never predetermined.", "determine", "Never pre1determine11d.")]
        [TestCase("Anticipated green.", "anticipate", "1Anticipate11d green.")]
        [TestCase("Anticipated green.", "I never anticipate red.", "2Anticipate22d green.")]
        public void CanHighlightSearchWordSubstrings(string initial, string find, string expected)
        {
            var sl = new SearchLighter();
            sl.SetExactMatchOpenMarkup("1");
            sl.SetExactMatchCloseMarkup("11");
            sl.SetPartialMatchOpenMarkup("2");
            sl.SetPartialMatchCloseMarkup("22");
            var result = sl.GetDisplayString(initial, find);

            expected.ShouldEqualCaseSensitive(result);
        }

        [TestCase("Anticipate", "anticipated", "2Anticipate22")]
        [TestCase("Anticipate green.", "anticipated", "2Anticipate22 green.")]
        [TestCase("Anticipate green.", "I never anticipated red.", "2Anticipate22 green.")]
        public void CanHighlightInitialSubstringsOfSearchWords(string initial, string find, string expected)
        {
            var sl = new SearchLighter();
            sl.SetExactMatchOpenMarkup("1");
            sl.SetExactMatchCloseMarkup("11");
            sl.SetPartialMatchOpenMarkup("2");
            sl.SetPartialMatchCloseMarkup("22");
            var result = sl.GetDisplayString(initial, find);

            expected.ShouldEqualCaseSensitive(result);
        }

        [TestCase("determine", "Never predetermined.", "determi2ne22")]
        [TestCase("determine this.", "Never predetermined.", "determi2ne22 this.")]
        public void CanNotHighlightMiddlingSubstringsOfSearchWordsDueToNoLookAhead(string initial, string find, string expected)
        {
            var sl = new SearchLighter();
            sl.SetExactMatchOpenMarkup("1");
            sl.SetExactMatchCloseMarkup("11");
            sl.SetPartialMatchOpenMarkup("2");
            sl.SetPartialMatchCloseMarkup("22");
            var result = sl.GetDisplayString(initial, find);

            expected.ShouldEqualCaseSensitive(result);
        }

        //[TestCase("Anticipated green.", "I am anticipating green.", "2Anticipat22ed green.", Ignore = true, IgnoreReason = "Under review.")]
        public void CanHighlightSignificantButInexactSubstringsOfSearchWords(string initial, string find, string expected)
        {
            //E.g. 'anticipat' in 'anticipated' when search for 'anticipating'.
            var sl = new SearchLighter();
            sl.SetPartialMatchOpenMarkup("2");
            sl.SetPartialMatchCloseMarkup("22");
            var result = sl.GetDisplayString(initial, find);

            expected.ShouldEqualCaseSensitive(result);
        }
    }
}
