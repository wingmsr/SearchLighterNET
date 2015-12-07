using NUnit.Framework;
using SearchLighterNetTests.Helpers;
using SearchLighterNET;

namespace SearchLighterNetTests.Tests.Internals
{
    [TestFixture]
    public class DefaultLineBreakTests
    {
        [TestCase("<br />a<br />b<br />c<br />d<br /><br />", "<br />a<br />b<br />c<br />d<br /><br />")]
        public void CanPreserveNormalLineBreaks(string initial, string expected)
        {
            var s = new SearchLighter().GetDisplayString(initial, "");
            s.ShouldEqualCaseSensitive(expected);
        }

        [TestCase("\na\nb\nc\nd\n\n", "<br />a<br />b<br />c<br />d<br /><br />")]
        [TestCase("\r\na\r\nb\r\nc\r\nd\r\n\r\n", "<br />a<br />b<br />c<br />d<br /><br />")]
        public void CanConvertNewLineChars(string initial, string expected)
        {
            var s = new SearchLighter().GetDisplayString(initial, "");
            s.ShouldEqualCaseSensitive(expected);
        }

        [TestCase("<br/>a<BR />b<BR/>c<br/>d<br/><br />", "<br />a<br />b<br />c<br />d<br /><br />")]
        public void CanNormalizeImperfectLineBreaks(string initial, string expected)
        {
            var s = new SearchLighter().GetDisplayString(initial, "");
            s.ShouldEqualCaseSensitive(expected);
        }
    }
}