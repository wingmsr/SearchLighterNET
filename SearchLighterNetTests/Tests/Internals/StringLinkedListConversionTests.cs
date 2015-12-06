using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Earthfires.Foundation.Libraries.Tools.TestingTools.Helpers;
using NUnit.Framework;
using SearchLighterNET;

namespace SearchLighterNetTests.Tests.Internals
{
    [TestFixture]
    public class StringLinkedListConversionPerformanceTests
    {
        [Test]
        public void CanConvertStringToLinkedList()
        {
            var ll = SearchLighter.SearchLighterUtils.convertStringToLinkedList("The quick brown fox jumped over\r\nthe \"lazy\" dog!");
            ll.Count.ShouldEqual(48);
        }

        [Test]
        public void CanConvertLinkedListToString()
        {
            var ll = new LinkedList<CharData>();
            ll.AddLast(new CharData(){Char='T', LowerCase = false });
            ll.AddLast(new CharData(){Char='H', LowerCase = true });
            ll.AddLast(new CharData(){Char='E', LowerCase = true });
            ll.AddLast(new CharData() { Char = ' ', LowerCase = false });
            ll.AddLast(new CharData(){Char='Q', LowerCase = true });
            ll.AddLast(new CharData(){Char='U', LowerCase = true });
            ll.AddLast(new CharData(){Char='I', LowerCase = true });
            ll.AddLast(new CharData(){Char='C', LowerCase = true });
            ll.AddLast(new CharData(){Char='K', LowerCase = true });
            ll.AddLast(new CharData() { Char = ' ', LowerCase = false });
            ll.AddLast(new CharData(){Char='B', LowerCase = true });
            ll.AddLast(new CharData(){Char='R', LowerCase = true });
            ll.AddLast(new CharData(){Char='O', LowerCase = true });
            ll.AddLast(new CharData(){Char='W', LowerCase = true });
            ll.AddLast(new CharData(){Char='N', LowerCase = true });
            ll.AddLast(new CharData() { Char = ' ', LowerCase = false });
            ll.AddLast(new CharData(){Char='F', LowerCase = true });
            ll.AddLast(new CharData(){Char='O', LowerCase = true });
            ll.AddLast(new CharData(){Char='X', LowerCase = true });
            ll.AddLast(new CharData() { Char = ' ', LowerCase = false });
            ll.AddLast(new CharData(){Char='J', LowerCase = true });
            ll.AddLast(new CharData(){Char='U', LowerCase = true });
            ll.AddLast(new CharData(){Char='M', LowerCase = true });
            ll.AddLast(new CharData(){Char='P', LowerCase = true });
            ll.AddLast(new CharData(){Char='E', LowerCase = true });
            ll.AddLast(new CharData(){Char='D', LowerCase = true });
            ll.AddLast(new CharData() { Char = ' ', LowerCase = false });
            ll.AddLast(new CharData(){Char='O', LowerCase = true });
            ll.AddLast(new CharData(){Char='V', LowerCase = true });
            ll.AddLast(new CharData(){Char='E', LowerCase = true });
            ll.AddLast(new CharData(){Char='R', LowerCase = true });
            ll.AddLast(new CharData() { Char = '\r', LowerCase = false });
            ll.AddLast(new CharData() { Char = '\n', LowerCase = false });
            ll.AddLast(new CharData(){Char='T', LowerCase = true });
            ll.AddLast(new CharData(){Char='H', LowerCase = true });
            ll.AddLast(new CharData(){Char='E', LowerCase = true });
            ll.AddLast(new CharData() { Char = ' ', LowerCase = false });
            ll.AddLast(new CharData() { Char = '\"', LowerCase = false });
            ll.AddLast(new CharData(){Char='L', LowerCase = true });
            ll.AddLast(new CharData(){Char='A', LowerCase = true });
            ll.AddLast(new CharData(){Char='Z', LowerCase = true });
            ll.AddLast(new CharData(){Char='Y', LowerCase = true });
            ll.AddLast(new CharData() { Char = '\"', LowerCase = false });
            ll.AddLast(new CharData() { Char = ' ', LowerCase = false });
            ll.AddLast(new CharData(){Char='D', LowerCase = true });
            ll.AddLast(new CharData(){Char='O', LowerCase = true });
            ll.AddLast(new CharData(){Char='G', LowerCase = true });
            ll.AddLast(new CharData() { Char = '!', LowerCase = false });
            var s = SearchLighter.SearchLighterUtils.convertLinkedListToString(ll);
            s.ShouldEqualCaseSensitive("The quick brown fox jumped over\r\nthe \"lazy\" dog!");
        }

        [Test]
        public void SimplePublicPassThroughTest()
        {
            //no line-breaks or sanitization required here
            string test = "THe qUiCk BroWn fox jumped over the \"lazy\" dog!";
            var s = SearchLighter.GetDisplayString(test, "");
            s.ShouldEqualCaseSensitive(test);
        }

        [Test]
        public void MultipleRoundTripPerformanceTest()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            int i = -1;
            for (i = 0; i < 1000000; i++)
            {
                SearchLighter.SearchLighterUtils.convertLinkedListToString(SearchLighter.SearchLighterUtils.convertStringToLinkedList("THe qUiCk BroWn fox jumped over\r\nthe \"lazy\" dog!"));
            }
            stopwatch.Stop();
            Console.WriteLine("time for "+(i)+" small round trips: " + stopwatch.ElapsedMilliseconds + " msec");
            (stopwatch.ElapsedMilliseconds < 3000).ShouldBeTrue();
        }

        [Test]
        public void StringLinkedListWarAndPeacePerformanceTest()
        {
            string dir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

            string file = dir + @"\TestData\war and peace.txt";
            string text = "";

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            using (
                StreamReader sr =
                    new StreamReader(file)
                )
            {
                text = sr.ReadToEnd();
            }

            stopwatch.Stop();
            var loadTime = stopwatch.ElapsedMilliseconds;
            Console.WriteLine("load baseline: " + loadTime + " msec");
            stopwatch.Reset();
            
            stopwatch.Start();
            var ll = SearchLighter.SearchLighterUtils.convertStringToLinkedList(text);
            stopwatch.Stop();
            var timeToLinkedList = stopwatch.ElapsedMilliseconds;
            Console.WriteLine("timeToLinkedList: " + timeToLinkedList + " msec");
            stopwatch.Reset();

            stopwatch.Start();
            var text2 = SearchLighter.SearchLighterUtils.convertLinkedListToString(ll);
            stopwatch.Stop();
            var timeToString = stopwatch.ElapsedMilliseconds;
            Console.WriteLine("timeToString: " + timeToString + " msec");
            stopwatch.Reset();

            (timeToLinkedList < 500).ShouldBeTrue();
            (timeToString < 250).ShouldBeTrue();
            text.Length.ShouldEqual(text2.Length);
            Console.WriteLine("text length: " + text.Length);
        }
    }
}