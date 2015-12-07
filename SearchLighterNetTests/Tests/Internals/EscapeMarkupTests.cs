using NUnit.Framework;
using SearchLighterNetTests.Helpers;
using SearchLighterNET;

namespace SearchLighterNetTests.Tests.Internals
{
    [TestFixture]
    public class EscapeMarkupTests
    {
        [TestCase(
            "<br<br>><br/>a<br><BR /><br><br<br<br>>><br>b<BR/>c<br/>d<br/><br><br /><br><<br>br<br>>",
            "&lt;br&lt;br&gt;&gt;<br />a&lt;br&gt;<br />&lt;br&gt;&lt;br&lt;br&lt;br&gt;&gt;&gt;&lt;br&gt;b<br />c<br />d<br />&lt;br&gt;<br />&lt;br&gt;&lt;&lt;br&gt;br&lt;br&gt;&gt;")]
        [TestCase("a<<<BR/>BR/>>b", "a&lt;&lt;<br />BR/&gt;&gt;b")]
        public void CanNormalizeLineBreaksAndSanitizeMalformedLineBreak(string initial, string expected)
        {
            var s = new SearchLighter().GetDisplayString(initial, "");
            s.ShouldEqualCaseSensitive(expected);
        }

        [TestCase("jack <br /> jill went <br>up <BR />the hill", "jack <br /> jill went &lt;br&gt;up <br />the hill",
            TestName = "sanitize preserves exact escape-markup (br default) match only")]
        [TestCase("j<a<c><br>>k<br /> <>><<<br />>j>i>l<>l><><br/><><><br><br>", "j&lt;a&lt;c&gt;&lt;br&gt;&gt;k<br /> &lt;&gt;&gt;&lt;&lt;<br />&gt;j&gt;i&gt;l&lt;&gt;l&gt;&lt;&gt;<br />&lt;&gt;&lt;&gt;&lt;br&gt;&lt;br&gt;",
            TestName = "sanitize kills any malformed escape-markup (br default)")]
        public void CanSanitizeButHonorEscapeMarkup(string initial, string expected)
        {
            var s = new SearchLighter().GetDisplayString(initial, "");
            expected.ShouldEqualCaseSensitive(s);
        }
    }
}