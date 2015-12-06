using System;
using Earthfires.Foundation.Libraries.Tools.TestingTools.Helpers;
using NUnit.Framework;
using SearchLighterNET;

namespace SearchLighterNetTests.Tests.Internals
{
    [TestFixture]
    public class SearchTermParsingTests
    {
        [TestCase(" SIX O'CLOCK, AND ALL'S WELL. TICK-TOCK... TICK-TOCK... GOES THE TICKING CLOCK! ", " SIX O'CLOCK, AND ALL'S WELL. TICK-TOCK... TICK-TOCK... GOES THE TICKING CLOCK! |TICKING|CLOCK|GOES|TICK|TOCK|WELL|ALL|AND|SIX|THE|O|S")]
        [TestCase("xxxxxxxxg xxxxxxxxa xxxxxxxxc xxxxxxxxf xxxxxxxxd xxxxxxxxb xxxxxxxxe xxxxxxxxa", "XXXXXXXXG XXXXXXXXA XXXXXXXXC XXXXXXXXF XXXXXXXXD XXXXXXXXB XXXXXXXXE XXXXXXXXA|XXXXXXXXA|XXXXXXXXB|XXXXXXXXC|XXXXXXXXD|XXXXXXXXE|XXXXXXXXF|XXXXXXXXG")]
        [TestCase("", "")]
        [TestCase("a", "A")]
        [TestCase("a a a a a a", "A A A A A A|A")]
        [TestCase("jack & jill went-up the@hill.com", "JACK & JILL WENT-UP THE@HILL.COM|THE@HILL|JACK|JILL|WENT|COM|UP|&")]
        [TestCase(
            "a big car went by the store as it was for official use only in the state where it was registered and so of course the driver adhered to the rules",
            "A BIG CAR WENT BY THE STORE AS IT WAS FOR OFFICIAL USE ONLY IN THE STATE WHERE IT WAS REGISTERED AND SO OF COURSE THE DRIVER ADHERED TO THE RULES|REGISTERED|OFFICIAL|ADHERED|COURSE|DRIVER|RULES|STATE|STORE|WHERE|ONLY|WENT|AND|BIG|CAR|FOR|THE|USE|WAS|AS|BY|IN|IT|OF|SO|TO|A")]
        [TestCase(
            "A Elbereth Gilthoniel, silivren penna mÌriel, o menel aglar elenath",
            "A ELBERETH GILTHONIEL, SILIVREN PENNA MÌRIEL, O MENEL AGLAR ELENATH|GILTHONIEL|ELBERETH|SILIVREN|ELENATH|MÌRIEL|AGLAR|MENEL|PENNA|A|O")]
        public void CanGetSortedSearchTermsCoercedToUppercase(string search, string expectedCsvTerms)
        {
            var expected = expectedCsvTerms.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            SearchLighter.HighlighterResetToDefaults();
            SearchLighter.HighlighterClearSkipWords();
            SearchLighter.HighlighterSetExactMatchMinLength(1);
            SearchLighter.HighlighterSetWordMinLength(1);
            var t = SearchLighter.SearchLighterUtils.getSortedSearchTerms(search);
            expected.Length.ShouldEqual(t.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                expected[i].ShouldEqualCaseSensitive(t[i]);
            }
        }

        [TestCase(
            "a big car went by the store as it was for official use only in the state where it was registered and so of course the driver adhered to the rules",
            "A BIG CAR WENT BY THE STORE AS IT WAS FOR OFFICIAL USE ONLY IN THE STATE WHERE IT WAS REGISTERED AND SO OF COURSE THE DRIVER ADHERED TO THE RULES|REGISTERED|OFFICIAL|ADHERED|COURSE|DRIVER|RULES|STATE|STORE|WHERE|ONLY|WENT|BIG|CAR|USE"
            )]
        public void CanOmitDefaultSkipWords(string search, string expectedCsvTerms)
        {
            var expected = expectedCsvTerms.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            SearchLighter.HighlighterResetToDefaults();
            SearchLighter.HighlighterSetExactMatchMinLength(1);
            SearchLighter.HighlighterSetWordMinLength(1);
            var t = SearchLighter.SearchLighterUtils.getSortedSearchTerms(search);
            expected.Length.ShouldEqual(t.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                expected[i].ShouldEqualCaseSensitive(t[i]);
            }
        }

        [TestCase(
    "a big car went by the store as it was for official use only in the state where it was registered and so of course the driver adhered to the rules",
    "A BIG CAR WENT BY THE STORE AS IT WAS FOR OFFICIAL USE ONLY IN THE STATE WHERE IT WAS REGISTERED AND SO OF COURSE THE DRIVER ADHERED TO THE RULES|REGISTERED|OFFICIAL|ADHERED|COURSE|DRIVER|RULES|STATE|STORE|WHERE|ONLY|WENT|BIG|CAR|USE"
    )]
        public void CanOmitDefaultSkipWordsWhenAddNothingToSkipWords(string search, string expectedCsvTerms)
        {
            var expected = expectedCsvTerms.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            SearchLighter.HighlighterResetToDefaults();
            SearchLighter.HighlighterSetExactMatchMinLength(1);
            SearchLighter.HighlighterSetWordMinLength(1);
            SearchLighter.HighlighterAddSkipWords();
            var t = SearchLighter.SearchLighterUtils.getSortedSearchTerms(search);
            expected.Length.ShouldEqual(t.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                expected[i].ShouldEqualCaseSensitive(t[i]);
            }
        }

        [TestCase(
            "a big car went by the store as it was for official use only in the state where it was registered and so of course the driver adhered to the rules",
            "A BIG CAR WENT BY THE STORE AS IT WAS FOR OFFICIAL USE ONLY IN THE STATE WHERE IT WAS REGISTERED AND SO OF COURSE THE DRIVER ADHERED TO THE RULES|REGISTERED|OFFICIAL|ADHERED|COURSE|DRIVER|RULES|STATE|STORE|WHERE|ONLY|WENT|BIG|CAR|USE"
            )]
        public void CanOmitDefaultSkipWordsWhenAddEmptyStringToSkipWords(string search, string expectedCsvTerms)
        {
            var expected = expectedCsvTerms.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            SearchLighter.HighlighterResetToDefaults();
            SearchLighter.HighlighterSetExactMatchMinLength(1);
            SearchLighter.HighlighterSetWordMinLength(1);
            SearchLighter.HighlighterAddSkipWords("");
            var t = SearchLighter.SearchLighterUtils.getSortedSearchTerms(search);
            expected.Length.ShouldEqual(t.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                expected[i].ShouldEqualCaseSensitive(t[i]);
            }
        }

        [TestCase(
            "a big car went by the store as it was for official use only in the state where it was registered and so of course the driver adhered to the rules",
            "A BIG CAR WENT BY THE STORE AS IT WAS FOR OFFICIAL USE ONLY IN THE STATE WHERE IT WAS REGISTERED AND SO OF COURSE THE DRIVER ADHERED TO THE RULES|OFFICIAL|COURSE|RULES|STORE|ONLY|AND|BIG|FOR|THE|USE|WAS|AS|BY|IN|IT|OF|SO|TO|A"
            )]
        public void CanOmitCustomSkipWords(string search, string expectedCsvTerms)
        {
            var expected = expectedCsvTerms.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            SearchLighter.HighlighterResetToDefaults();
            SearchLighter.HighlighterClearSkipWords();
            SearchLighter.HighlighterSetExactMatchMinLength(1);
            SearchLighter.HighlighterSetWordMinLength(1);
            SearchLighter.HighlighterAddSkipWords("REGISTERED", "ADHERED", "DRIVER", "STATE", "WHERE", "WENT", "CAR");

            var t = SearchLighter.SearchLighterUtils.getSortedSearchTerms(search);
            expected.Length.ShouldEqual(t.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                expected[i].ShouldEqualCaseSensitive(t[i]);
            }
        }

        [TestCase(
    "a big car went by the store as it was for official use only in the state where it was registered and so of course the driver adhered to the rules",
    "A BIG CAR WENT BY THE STORE AS IT WAS FOR OFFICIAL USE ONLY IN THE STATE WHERE IT WAS REGISTERED AND SO OF COURSE THE DRIVER ADHERED TO THE RULES|OFFICIAL|COURSE|RULES|STORE|ONLY|BIG|USE"
    )]
        public void CanOmitDefaultSkipPlusCustomSkipWords(string search, string expectedCsvTerms)
        {
            var expected = expectedCsvTerms.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            SearchLighter.HighlighterResetToDefaults();
            SearchLighter.HighlighterSetExactMatchMinLength(1);
            SearchLighter.HighlighterSetWordMinLength(1);
            SearchLighter.HighlighterAddSkipWords("REGISTERED", "ADHERED", "DRIVER", "STATE", "WHERE", "WENT", "CAR");
            var t = SearchLighter.SearchLighterUtils.getSortedSearchTerms(search);
            expected.Length.ShouldEqual(t.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                expected[i].ShouldEqualCaseSensitive(t[i]);
            }
        }

        [TestCase(7, "", "")]
        [TestCase(3, 
            "A Elbereth Gilthoniel, silivren penna mÌriel, o menel aglar elenath",
            "A ELBERETH GILTHONIEL, SILIVREN PENNA MÌRIEL, O MENEL AGLAR ELENATH|GILTHONIEL|ELBERETH|SILIVREN|ELENATH|MÌRIEL|AGLAR|MENEL|PENNA")]
        [TestCase(6,
            "A Elbereth Gilthoniel, silivren penna mÌriel, o menel aglar elenath",
            "A ELBERETH GILTHONIEL, SILIVREN PENNA MÌRIEL, O MENEL AGLAR ELENATH|GILTHONIEL|ELBERETH|SILIVREN|ELENATH|MÌRIEL")]
        [TestCase(1, "a ccc bb eeeee dddd ggggggg ffffff hhhhhhhh", "A CCC BB EEEEE DDDD GGGGGGG FFFFFF HHHHHHHH|HHHHHHHH|GGGGGGG|FFFFFF|EEEEE|DDDD|CCC|BB|A")]
        [TestCase(2, "a ccc bb eeeee dddd ggggggg ffffff hhhhhhhh", "A CCC BB EEEEE DDDD GGGGGGG FFFFFF HHHHHHHH|HHHHHHHH|GGGGGGG|FFFFFF|EEEEE|DDDD|CCC|BB")]
        [TestCase(3, "a ccc bb eeeee dddd ggggggg ffffff hhhhhhhh", "A CCC BB EEEEE DDDD GGGGGGG FFFFFF HHHHHHHH|HHHHHHHH|GGGGGGG|FFFFFF|EEEEE|DDDD|CCC")]
        [TestCase(4, "a ccc bb eeeee dddd ggggggg ffffff hhhhhhhh", "A CCC BB EEEEE DDDD GGGGGGG FFFFFF HHHHHHHH|HHHHHHHH|GGGGGGG|FFFFFF|EEEEE|DDDD")]
        [TestCase(5, "a ccc bb eeeee dddd ggggggg ffffff hhhhhhhh", "A CCC BB EEEEE DDDD GGGGGGG FFFFFF HHHHHHHH|HHHHHHHH|GGGGGGG|FFFFFF|EEEEE")]
        [TestCase(6, "a ccc bb eeeee dddd ggggggg ffffff hhhhhhhh", "A CCC BB EEEEE DDDD GGGGGGG FFFFFF HHHHHHHH|HHHHHHHH|GGGGGGG|FFFFFF")]
        [TestCase(7, "a ccc bb eeeee dddd ggggggg ffffff hhhhhhhh", "A CCC BB EEEEE DDDD GGGGGGG FFFFFF HHHHHHHH|HHHHHHHH|GGGGGGG")]
        [TestCase(8, "a ccc bb eeeee dddd ggggggg ffffff hhhhhhhh", "A CCC BB EEEEE DDDD GGGGGGG FFFFFF HHHHHHHH|HHHHHHHH")]
        public void CanSkipWordsLessThanMinWordLength(int minWordLength, string search, string expectedCsvTerms)
        {
            var expected = expectedCsvTerms.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            SearchLighter.HighlighterResetToDefaults();
            SearchLighter.HighlighterClearSkipWords();
            SearchLighter.HighlighterSetExactMatchMinLength(minWordLength);
            SearchLighter.HighlighterSetWordMinLength(minWordLength);
            var t = SearchLighter.SearchLighterUtils.getSortedSearchTerms(search);
            expected.Length.ShouldEqual(t.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                expected[i].ShouldEqualCaseSensitive(t[i]);
            }
        }

        [TestCase(5, "qwer tyui asdfgh zxcvbnm by the side of the permutation in an effort to minimize confusion",
            "QWER TYUI ASDFGH ZXCVBNM BY THE SIDE OF THE PERMUTATION IN AN EFFORT TO MINIMIZE CONFUSION|MINIMIZE|ZXCVBNM|ASDFGH|EFFORT")]
        public void CanSkipWordsLessThanMinWordLengthAndSkipWords(int minWordLength, string search, string expectedCsvTerms)
        {
            var expected = expectedCsvTerms.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            SearchLighter.HighlighterResetToDefaults();
            SearchLighter.HighlighterSetExactMatchMinLength(minWordLength);
            SearchLighter.HighlighterSetWordMinLength(minWordLength);
            SearchLighter.HighlighterAddSkipWords("permutation", "confusion");
            var t = SearchLighter.SearchLighterUtils.getSortedSearchTerms(search);
            expected.Length.ShouldEqual(t.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                expected[i].ShouldEqualCaseSensitive(t[i]);
            }
        }

        [TestCase(15, 5, "a ccc bb eeeee", "")]
        [TestCase(14, 5, "a ccc bb eeeee", "A CCC BB EEEEE|EEEEE")]
        public void CanSkipExactMatchLessThanMinExactMatchLength(int minExactLength, int minWordLength, string search, string expectedCsvTerms)
        {
            var expected = expectedCsvTerms.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            SearchLighter.HighlighterResetToDefaults();
            SearchLighter.HighlighterClearSkipWords();
            SearchLighter.HighlighterSetExactMatchMinLength(minExactLength);
            SearchLighter.HighlighterSetWordMinLength(minWordLength);
            var t = SearchLighter.SearchLighterUtils.getSortedSearchTerms(search);
            expected.Length.ShouldEqual(t.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                expected[i].ShouldEqualCaseSensitive(t[i]);
            }
        }

        [TestCase(15, 5, "a ccc bb eeeee", "")]
        [TestCase(14, 5, "a ccc bb eeeee", "A CCC BB EEEEE")]
        public void CanSkipExactMatchLessThanMinExactMatchLengthAndSkipWords(int minExactLength, int minWordLength, string search, string expectedCsvTerms)
        {
            var expected = expectedCsvTerms.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            SearchLighter.HighlighterResetToDefaults();
            SearchLighter.HighlighterSetExactMatchMinLength(minExactLength);
            SearchLighter.HighlighterSetWordMinLength(minWordLength);
            SearchLighter.HighlighterAddSkipWords("eeeee");
            var t = SearchLighter.SearchLighterUtils.getSortedSearchTerms(search);
            expected.Length.ShouldEqual(t.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                expected[i].ShouldEqualCaseSensitive(t[i]);
            }
        }

        [TestCase("<br />", "")]
        [TestCase("<BR/>", "")]
        [TestCase("\n", "")]
        [TestCase("\r\n", "")]
        [TestCase("<br />a<BR/>\r\n", "A")]
        [TestCase("\na \r\na a <BR/>a a a", "A A A A A A|A")]
        [TestCase("<br />jack & jill \nwent-up<BR /> the@hill.com", "JACK & JILL WENT-UP THE@HILL.COM|THE@HILL|JACK|JILL|WENT|COM|UP|&")]
        public void CanGetSortedSearchTermsCoercedToUppercaseWithLineBreaksOmitted(string search, string expectedCsvTerms)
        {
            var expected = expectedCsvTerms.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            SearchLighter.HighlighterResetToDefaults();
            SearchLighter.HighlighterClearSkipWords();
            SearchLighter.HighlighterSetExactMatchMinLength(1);
            SearchLighter.HighlighterSetWordMinLength(1);
            var t = SearchLighter.SearchLighterUtils.getSortedSearchTerms(search);
            expected.Length.ShouldEqual(t.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                expected[i].ShouldEqualCaseSensitive(t[i]);
            }
        }

        [TestCase(">jack >&< <jill> went-up the@hill.com>", "&GT;JACK &GT;&&LT; &LT;JILL&GT; WENT-UP THE@HILL.COM&GT;|THE@HILL|JACK|JILL|WENT|COM|UP|&")]
        public void CanGetSortedSearchTermsCoercedToUppercaseWithSanitizedAngleBrackets(string search, string expectedCsvTerms)
        {
            var expected = expectedCsvTerms.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            SearchLighter.HighlighterResetToDefaults();
            SearchLighter.HighlighterClearSkipWords();
            SearchLighter.HighlighterSetExactMatchMinLength(1);
            SearchLighter.HighlighterSetWordMinLength(1);
            var t = SearchLighter.SearchLighterUtils.getSortedSearchTerms(search);
            expected.Length.ShouldEqual(t.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                expected[i].ShouldEqualCaseSensitive(t[i]);
            }
        }
    }
}