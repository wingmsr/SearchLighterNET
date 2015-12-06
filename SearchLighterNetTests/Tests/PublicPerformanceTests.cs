using System;
using System.Diagnostics;
using System.IO;
using Earthfires.Foundation.Libraries.Tools.TestingTools.Helpers;
using NUnit.Framework;
using SearchLighterNET;

namespace SearchLighterNetTests.Tests
{
    [TestFixture]
    public class PublicPerformanceTests
    {
        [Test]
        public void PerformanceTests()
        {
            string find;
            var text = performanceTestGateway(out find);

            Stopwatch s = new Stopwatch();
            SearchLighter.HighlighterResetToDefaults();
            s.Start();
            
            for (int i = 0; i < 30000; i++)
            {
                var t = SearchLighter.GetDisplayString(text + i, find);
            }

            s.Stop();
            Console.WriteLine("performance test elapsed time (msec): " + s.Elapsed.TotalMilliseconds);
            (s.Elapsed.TotalMilliseconds < 3000).ShouldBeTrue();
        }

        [TestCase("war and peace_10K.txt", 800, "A young officer with a bewildered and pained expression on his face stepped away from the man and looked round inquiringly at the adjutant as he rode by.")]
        [TestCase("war and peace.txt", 5000, "A young officer with a bewildered and pained expression on his face stepped away from the man and looked round inquiringly at the adjutant as he rode by.")]    
        public void PerformanceTestWarAndPeace(string file, int msecLimit, string find)
        {
            string f0;
            var t0 = performanceTestGateway(out f0);

            SearchLighter.HighlighterResetToDefaults();

            string dir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

            string path = dir + @"\TestData\" + file;
            string text = "";
            using (
                StreamReader sr =
                    new StreamReader(path)
                )
            {
                text = sr.ReadToEnd();
            }

            Stopwatch s = new Stopwatch();
            s.Start();
            
            var t = SearchLighter.GetDisplayString(text, find);

            s.Stop();
            Console.WriteLine(file + " performance test elapsed time (msec): " + s.Elapsed.TotalMilliseconds);
            (s.Elapsed.TotalMilliseconds < msecLimit).ShouldBeTrue();
            int ix = t.IndexOf("<span class=\"hlt1\">" + find + "</span>", StringComparison.InvariantCulture);
            ix.ShouldBeGreaterThanZero();
            text.Length.ShouldBeGreaterThanZero();
        }

        private static string performanceTestGateway(out string find)
        {
            var text =
                "the<br> <xxx>quick</xxx> BROWN@fox \r\njumped OVER the<BR/>lazy-dog!\n\n";
            find = "brown@FOX \njumped over";
            var expected =
                "the&lt;br&gt; &lt;xxx&gt;quick&lt;/xxx&gt; <span class=\"hlt2\">BROWN@fox</span> <br /><span class=\"hlt2\">jumped</span> <span class=\"hlt2\">OVER</span> the<br />lazy-dog!<br /><br />";

            SearchLighter.HighlighterResetToDefaults();
            var t0 = SearchLighter.GetDisplayString(text + 1, find);
            (expected + 1).ShouldEqualCaseSensitive(t0);
            return text;
        }
    }
}
