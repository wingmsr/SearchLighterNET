using NUnit.Framework;
using SearchLighterNetTests.Helpers;
using SearchLighterNET;

namespace SearchLighterNetTests.Tests.Internals
{
    [TestFixture]
    public class SanitizationTests
    {
        [TestCase("$@N", "san", "h&!", "Hai", "$@N", "san")]
        [TestCase("$@N", "san", "h&!", "Hai", "h&!", "Hai")]

        [TestCase("$@N", "san", "h&!", "Hai", "h&! anjin $@N", "Hai anjin san")]
        [TestCase("$@N", "san", "h&!", "Hai", "H&! anjin $@n", "Hai anjin san")]

        [TestCase("$@", "san", "&!", "Hai", "&! anjin $@", "Hai anjin san")]
        [TestCase("$@N", "sa", "h&!", "ai", "H&! anjin $@n", "ai anjin sa")]

        [TestCase("$$@@", "san", "&&!!", "Hai", "&&!! anjin $$@@", "Hai anjin san")]
        [TestCase("$$@@N", "sa", "h&&!!", "ai", "H&&!! anjin $$@@n", "ai anjin sa")]
        public void CanSanitizeMultiCharSequenceCaseInsensitively(string sanitizeInit1, string sanitizeFinal1, string sanitizeInit2,
            string sanitizeFinal2, string initial, string expected)
        {
            var sl = new SearchLighter();
            sl.SetSanitizationMap(new string[][]
            {
                new string[] {sanitizeInit1, sanitizeFinal1},
                new string[] {sanitizeInit2, sanitizeFinal2}
            });
            var s = sl.GetDisplayString(initial, "");
            s.ShouldEqualCaseSensitive(expected);
        }

        [TestCase("a<<<BR/>BR/>>b", "a<<<BR/>BR/>>b")]
        public void ShouldNotSanitizeWhenDisabled(string initial, string expected)
        {
            var sl = new SearchLighter();
            sl.SetShouldSanitizeAndEscape(false);
            var s = sl.GetDisplayString(initial, "");
            s.ShouldEqualCaseSensitive(expected);
        }

        [TestCase(
                   "The [quick] BROWN fox \\ jumped over \\ the {lazy} dog!",
                   "The [quick] BROWN fox \\ jumped over \\ the {lazy} dog!",
                   TestName = "sanitize preserves normal text")]
        [TestCase(
                   "The [quick] BROWN fox \\ jumped over \\ the {lazy} dog!<",
                   "The [quick] BROWN fox \\ jumped over \\ the {lazy} dog!&lt;",
                   TestName = "sanitize orphaned trailing '<'")]
        [TestCase(
           "The [quick] BROWN fox \\ jumped over \\ the {lazy} dog!>",
           "The [quick] BROWN fox \\ jumped over \\ the {lazy} dog!&gt;",
           TestName = "sanitize orphaned '>'")]
        [TestCase("jack &xyz; jill", "jack &xyz; jill", TestName = "sanitize preserves arbitrary &xyz;")]
        [TestCase("jack 'and' jill", "jack 'and' jill", TestName = "sanitize preserves 'single quotes'")]
        [TestCase("jack \"and\" jill", "jack \"and\" jill", TestName = "sanitize preserves \"double quotes\"")]
        [TestCase("jack > jill", "jack &gt; jill", TestName = "sanitize kills any orphaned > angle-brackets")]
        [TestCase("jack < jill", "jack &lt; jill", TestName = "sanitize kills any orphaned < angle-brackets and trailing content"
            )]
        [TestCase("jack <tag> jill", "jack &lt;tag&gt; jill", TestName = "sanitize kills any non-br angle-brackets - opening tag")]
        [TestCase("jack </tag> jill", "jack &lt;/tag&gt; jill", TestName = "sanitize kills any non-br angle-brackets - closing tag")
        ]
        [TestCase("jack <tag>jill</tag>", "jack &lt;tag&gt;jill&lt;/tag&gt;",
            TestName = "sanitize kills any non-br angle-brackets - open and closing tag")]
        [TestCase("jack <tag id=\"abc\">jill", "jack &lt;tag id=\"abc\"&gt;jill",
            TestName = "sanitize kills any non-br angle-brackets - complex opening tag")]
        public void CanSanitize(string initial, string expected)
        {
            var result = new SearchLighter().GetDisplayString(initial, "");
            expected.ShouldEqualCaseSensitive(result);
        }
    }
}