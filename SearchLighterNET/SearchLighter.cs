﻿using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable ForCanBeConvertedToForeach
//see when porting: http://www.i-programmer.info/programming/javascript/5328-javascript-data-structures-the-linked-list.html
namespace SearchLighterNET
{
    public class SearchLighter
    {
        internal static class SearchLighterUtils
        {
            /*
               Char    Dec     Oct     Hex |
               ---------------------
               A       65      0101    0x41 |
               ...
               Z       90      0132    0x5a |
               ...
               a       97      0141    0x61
               ...
               z       122     0172    0x7a
            */

            internal static bool _charEqualsCaseInsensitive(char a, char b)
            {
                if (a > 96 && a < 123)
                {
                    return a == b || ((char)(a - 32) == b);
                }
                
                if (b > 96 && b < 123)
                {
                    return a == b || ((char)(b - 32) == a);
                }

                return a == b;
            }

            internal static string _convertStringToAsciiUppercase(string html)
            {
                if (string.IsNullOrEmpty(html))
                {
                    return "";
                }

                var result = new char[html.Length];
                for (int i = 0; i < html.Length; i++)
                {
                    char c = html[i];
                    if (c > 96 && c < 123)
                    {
                        result[i] = (char)(c - 32);
                    }
                    else
                    {
                        result[i] = c;
                    }
                }
                return new string(result);
            }

            internal static void _escapeOrSanitize(SearchLighter sl, ref LinkedListNode<CharData> c, LinkedList<CharData> ll)
            {
                if (!_processCurrentChar(sl, ref c, ll, sl._escapeMarkupMap))
                {
                    _processCurrentChar(sl, ref c, ll, sl._sanitizationMap);
                }                
            }

            internal static bool _processCurrentChar(SearchLighter sl, ref LinkedListNode<CharData> c, LinkedList<CharData> ll, string[][] map)
            {
                if (!sl._shouldSanitizeAndEscape)
                    return false;

                for (int i = 0; i < map.Length; i++)
                {
                    if (_processCurrentCharForMapItem(ref c, ll, map[i][0], map[i][1]))
                    {
                        return true;
                    }
                }
                return false;
            }

            private static bool _processCurrentCharForMapItem(ref LinkedListNode<CharData> c, LinkedList<CharData> ll,
                string init, string final)
            {
                int i = 0;
                var cur = c;
                while (cur != null && i < init.Length)
                {
                    if (!_charEqualsCaseInsensitive(cur.Value.Char, init[i]) || (i < init.Length - 1 && cur.Next == null))
                    {
                        return false;
                    }
                    i++;
                    cur = cur.Next;
                }

                cur = c;
                if (init.Length == final.Length)
                {
                    for (int j = 0; j < final.Length; j++)
                    {
                        var temp = cur;
                        cur = new LinkedListNode<CharData>(new CharData() {Char = final[j]});
                        ll.AddAfter(temp, cur);
                        ll.Remove(temp);
                        c = cur;
                        cur = cur.Next;
                    }
                }
                else if (init.Length > final.Length)
                {
                    for (int j = 0; j < final.Length; j++)
                    {
                        var temp = cur;
                        cur = new LinkedListNode<CharData>(new CharData() { Char = final[j] });
                        ll.AddAfter(temp, cur);
                        ll.Remove(temp);
                        c = cur;
                        cur = cur.Next;
                    }

                    //remove extra
                    for (int j = final.Length; j < init.Length; j++)
                    {
                        var temp = c.Next;
                        ll.Remove(temp);
                    }
                    
                }
                else
                {
                    for (int j = 0; j < init.Length; j++)
                    {
                        var temp = cur;
                        cur = new LinkedListNode<CharData>(new CharData() { Char = final[j] });
                        ll.AddAfter(temp, cur);
                        ll.Remove(temp);
                        c = cur;
                        cur = cur.Next;
                    }

                    //add remaining
                    cur = c;
                    for (int j = init.Length; j < final.Length; j++)
                    {
                        var temp = cur;
                        cur = new LinkedListNode<CharData>(new CharData() { Char = final[j] });
                        ll.AddAfter(temp, cur);
                    }
                    c = cur;
                }
                
                return true;
            }

            internal static void _replaceWithLineBreak(SearchLighter sl, ref LinkedListNode<CharData> c, LinkedList<CharData> ll)
            {
                var n = c.Next;
                var cur = new LinkedListNode<CharData>(new CharData() { Char = sl._br[0] });
                ll.AddBefore(c, cur);
                ll.Remove(c);
                c = cur;
                for (int i = 1; i < sl._br.Length; i++)
                {
                    cur = new LinkedListNode<CharData>(new CharData() { Char = sl._br[i] });
                    ll.AddAfter(c, cur);
                    c = cur;
                }
            }

            internal static string[] _getDistinctUppercaseStrings(string[] skipWords)
            {
                if (skipWords == null || skipWords.Length == 0)
                {
                    return new string[0];
                }

                var temp = new HashSet<string>();
                int i;
                for (i = 0; i < skipWords.Length; i++)
                {
                    var s = _convertStringToAsciiUppercase(skipWords[i]);
                    if (s.Length > 0)
                        temp.Add(s);
                }

                skipWords = new string[temp.Count];
                i = -1;
                foreach (var w in temp)
                {
                    skipWords[++i] = w;
                }
                Array.Sort(skipWords);
                return skipWords;
            }

            private static bool _shouldHighlightCurrentMapItem(string[] searchMap, int i, ref LinkedListNode<CharData> endAfter, int minLength, out bool partial)
            {
                for (int j = 1; j < searchMap[i].Length; j++)
                {
                    if (endAfter.Next == null)
                    {
                        partial = i < (searchMap[i].Length - 1);
                        return j >= minLength;
                    }
                    if (searchMap[i][j] != endAfter.Next.Value.Char)
                    {
                        if (endAfter.Next.Value.Char == ' ')//not using full word-boundary characters as a pragmatic optimization
                        {
                            partial = true;
                            return j >= minLength;
                        }
                        partial = false;
                        return false;
                    }
                    endAfter = endAfter.Next;
                }
                partial = false;
                return true;
            }

            private static bool _shouldHighlightAnyMapItemAtCurrentChar(SearchLighter sl, string[] searchMap, LinkedList<CharData> ll, ref LinkedListNode<CharData> c, int i, int minLength)
            {
                if (searchMap[i][0] != c.Value.Char)
                    return false;

                if (searchMap[i].Length == 1)
                {
                    return _applyHighlight(sl, i == 0, ll, ref c, ref c, searchMap[i], searchMap[0]);
                }

                var endAfter = c;
                bool partial;
                if (SearchLighterUtils._shouldHighlightCurrentMapItem(searchMap, i, ref endAfter, minLength, out partial))
                {
                    return _applyHighlight(sl, !partial && i == 0, ll, ref c, ref endAfter, searchMap[i], searchMap[0]);
                }
                else
                {
                    return false;
                }
            }

            internal static void _matchAndHighlight(SearchLighter sl, string[] searchMap, LinkedList<CharData> ll, ref LinkedListNode<CharData> c, int minLength)
            {
                for (int i = 0; i < searchMap.Length; i++)
                {
                    if (SearchLighterUtils._shouldHighlightAnyMapItemAtCurrentChar(sl, searchMap, ll, ref c, i, minLength))
                    {
                        return;
                    }
                }
            }

            private static bool _exactMatchDetectedInReach(string exactMatch, int reach, LinkedList<CharData> ll,
                LinkedListNode<CharData> c)
            {
                while (reach > 0 && c.Next != null)
                {
                    var cur = c.Next;
                    bool match = true;
                    for (int i = 0; i < exactMatch.Length; i++)
                    {
                        if (cur == null || exactMatch[i] != cur.Value.Char)
                        {
                            match = false;
                            break;
                        }
                        cur = cur.Next;
                    }
                    if (match)
                    {
                        return true;
                    }
                    reach--;
                    c = c.Next;
                }
                return false;
            }

            private static bool _applyHighlight(SearchLighter sl, bool exact, LinkedList<CharData> ll, ref LinkedListNode<CharData> c, ref LinkedListNode<CharData> endAfter, string currentMatch, string exactMatch)
            {
                LinkedListNode<CharData> resume = null;
                if (exact)
                {
                    for (int i = 0; i < sl._o1.Length; i++)
                    {
                        ll.AddBefore(c, new LinkedListNode<CharData>(new CharData() { Char = sl._o1[i] }));
                    }

                    resume = new LinkedListNode<CharData>(new CharData() { Char = sl._c1[sl._c1.Length - 1] });
                    ll.AddAfter(endAfter, resume);
                    for (int i = sl._c1.Length - 2; i > -1; i--)
                    {
                        ll.AddAfter(endAfter, new LinkedListNode<CharData>(new CharData() { Char = sl._c1[i] }));
                    }
                    c = resume;
                    return true;
                }
                else
                {
                    if (SearchLighterUtils._exactMatchDetectedInReach(exactMatch, currentMatch.Length-1, ll, c))
                    {
                        return false;
                    }
                    for (int i = 0; i < sl._o2.Length; i++)
                    {
                        ll.AddBefore(c, new LinkedListNode<CharData>(new CharData() { Char = sl._o2[i] }));
                    }

                    resume = new LinkedListNode<CharData>(new CharData() { Char = sl._c2[sl._c2.Length - 1] });
                    ll.AddAfter(endAfter, resume);
                    for (int i = sl._c2.Length - 2; i > -1; i--)
                    {
                        ll.AddAfter(endAfter, new LinkedListNode<CharData>(new CharData() { Char = sl._c2[i] }));
                    }
                    c = resume;
                    return true;
                }
            }

            internal static string convertLinkedListToString(LinkedList<CharData> llHtml)
            {
                char[] arr = new char[llHtml.Count];
                var c = llHtml.First;
                int i = -1;
                while (c != null)
                {
                    arr[++i] = c.Value.LowerCase ? ((char)(c.Value.Char + 32)) : c.Value.Char;
                    c = c.Next;
                }
                return new string(arr);
            }

            internal static LinkedList<CharData> convertStringToLinkedList(string html)
            {
                var result = new LinkedList<CharData>();
                if (html == null || html.Length == 0)
                {
                    return result;
                }

                for (int i = 0; i < html.Length; i++)
                {
                    char c = html[i];
                    bool lc = false;
                    if (c > 96 && c < 123)
                    {
                        lc = true;
                        c = (char)(c - 32);
                    }
                    result.AddLast(new CharData()
                    {
                        Char = c,
                        LowerCase = lc
                    });
                }
                return result;
            }

            internal static string[] getSortedSearchTerms(SearchLighter sl, string search)
            {
                if (search == null)
                    return new string[0];

                if (search.Length < sl._minHighlightExactMatchLength)
                    return new string[0];

                search = SearchLighterUtils._convertStringToAsciiUppercase(search);

                search = _removeEscapeMarkupFromRawSearch(sl, search);

                if (search.Length < sl._minHighlightExactMatchLength)
                    return new string[0];

                //todo: make javascript portable
                var split =
                    search.Split(sl._wordBoundaryCharacters, StringSplitOptions.RemoveEmptyEntries)
                        .Distinct()
                        .Where(x => x.Length >= sl._minHighlightWordLength)
                        .Except(sl._skipWords)
                        .OrderByDescending(x => x.Length)
                        .ThenBy(x => x).ToArray();

                string[] result;

                if (split.Length == 0)
                {
                    result = new string[] { search };
                }
                else if (String.Compare(split[0], search, StringComparison.CurrentCulture) == 0)
                {
                    result = new string[split.Length];
                    for (int i = 0; i < split.Length; i++)
                    {
                        result[i] = split[i];
                    }
                }
                else
                {
                    result = new string[split.Length + 1];
                    for (int i = 0; i < split.Length; i++)
                    {
                        result[0] = search;
                        result[i + 1] = split[i];
                    }
                }

                return _sanitizeEscapedSearches(sl, result);
            }

            private static string _removeEscapeMarkupFromRawSearch(SearchLighter sl, string search)
            {
                //todo: optimize and make JS portable
                for (int i = 0; i < sl._escapeMarkupMap.Length; i++)
                {
                    search = search
                        .Replace(sl._escapeMarkupMap[i][0], "")
                        .Replace(sl._escapeMarkupMap[i][0].ToUpper(), "");
                }
                return search;
            }

            private static string[] _sanitizeEscapedSearches(SearchLighter sl, string[] search)
            {
                //todo: optimize and make JS portable
                for (int i = 0; i < search.Length; i++)
                {
                    for (int j = 0; j < sl._sanitizationMap.Length; j++)
                    {
                        search[i] = search[i]
                            .Replace(sl._sanitizationMap[j][0].ToUpper(), sl._sanitizationMap[j][1].ToUpper())
                            .Replace(sl._sanitizationMap[j][0], sl._sanitizationMap[j][1].ToUpper());
                    }
                }

                return search;
            }
        }

        private char[] _wordBoundaryCharacters = SearchLighterConfigurationTools.DefaultWordBoundaryCharacters();
        private string _o1 = SearchLighterConfigurationTools._defaultO1();
        private string _o2 = SearchLighterConfigurationTools._defaultO2();
        private string _c1 = SearchLighterConfigurationTools._defaultC1();
        private string _c2 = SearchLighterConfigurationTools._defaultC2();
        private char[] _br = SearchLighterConfigurationTools._defaultBr();
        private bool _shouldSanitizeAndEscape = true;
        private string[] _skipWords = SearchLighterConfigurationTools._defaultskipWords();
        private int _minHighlightWordLength = SearchLighterConfigurationTools._defaultMinHighlightWordLength();
        private int _minHighlightExactMatchLength = SearchLighterConfigurationTools._defaultMinHighlightExactMatchLength();
        private int _minHighlightSubstringMatchLength = SearchLighterConfigurationTools._defaultMinHighlightSubstringMatchLength();
        private string[][] _escapeMarkupMap = SearchLighterConfigurationTools.DefaultLineBreakEscapeMarkupMap();
        private string[][] _sanitizationMap = SearchLighterConfigurationTools.DefaultAngleBracketSanitizationMap();

        public void SetShouldSanitizeAndEscape(bool sanitizeAndEscape)
        {
            _shouldSanitizeAndEscape = sanitizeAndEscape;
        }

        public void SetExactMatchOpenMarkup(string s)
        {
            _o1 = s ?? "";
        }

        public void SetExactMatchCloseMarkup(string s)
        {
            _c1 = s ?? "";
        }

        public void SetPartialMatchOpenMarkup(string s)
        {
            _o2 = s ?? "";
        }

        public void SetPartialMatchCloseMarkup(string s)
        {
            _c2 = s ?? "";
        }

        public void SetWordBoundaryCharacters(char[] chars)
        {
            _wordBoundaryCharacters = chars ?? new char[0];
            Array.Sort(_wordBoundaryCharacters);
        }

        public void SetEscapeMarkupMap(string[][] d)
        {
            _escapeMarkupMap = d ?? new string[0][];
        }

        public void SetSanitizationMap(string[][] d)
        {
            _sanitizationMap = d ?? new string[0][];
        }

        public void HighlighterClearSkipWords()
        {
            _skipWords = new string[0];
        }

        public void HighlighterResetSkipWords()
        {
            _skipWords = SearchLighterConfigurationTools._defaultskipWords();
        }

        public void HighlighterResetToDefaults()
        {
            _shouldSanitizeAndEscape = true;
            _wordBoundaryCharacters = SearchLighterConfigurationTools.DefaultWordBoundaryCharacters();
            SetSanitizationMap(SearchLighterConfigurationTools.DefaultAngleBracketSanitizationMap());
            SetEscapeMarkupMap(SearchLighterConfigurationTools.DefaultLineBreakEscapeMarkupMap());
            _skipWords = SearchLighterConfigurationTools._defaultskipWords();
            _c1 = SearchLighterConfigurationTools._defaultC1();
            _br = SearchLighterConfigurationTools._defaultBr();
            _c2 = SearchLighterConfigurationTools._defaultC2();
            _minHighlightExactMatchLength = SearchLighterConfigurationTools._defaultMinHighlightExactMatchLength();
            _minHighlightWordLength = SearchLighterConfigurationTools._defaultMinHighlightWordLength();
            _minHighlightSubstringMatchLength = SearchLighterConfigurationTools._defaultMinHighlightSubstringMatchLength();
            _o1 = SearchLighterConfigurationTools._defaultO1();
            _o2 = SearchLighterConfigurationTools._defaultO2();
        }

        public void HighlighterAddSkipWords(params string[] skipWords)
        {
            skipWords = SearchLighterUtils._getDistinctUppercaseStrings(skipWords);

            var newSkipWords = new string[skipWords.Length + _skipWords.Length];
            for (int i = 0; i < _skipWords.Length; i++)
            {
                newSkipWords[i] = _skipWords[i];
            }

            int j = -1;
            for (int i = _skipWords.Length; i < newSkipWords.Length; i++)
            {
                newSkipWords[i] = skipWords[++j];
            }

            _skipWords = newSkipWords;
            Array.Sort(_skipWords);
        }

        public void HighlighterSetWordMinLength(int length)
        {
            _minHighlightWordLength = length;
        }

        public void HighlighterSetExactMatchMinLength(int length)
        {
            _minHighlightExactMatchLength = length;
        }

        public void HighlighterSetSubstringMatchMinLength(int length)
        {
            _minHighlightSubstringMatchLength = length;
        }

        public string GetDisplayString(string html, string find = "")
        {
            //get sanitized case-insensitive length:alpha sorted distinct search map from find
            var searchMap = SearchLighterUtils.getSortedSearchTerms(this, find);
            bool shouldMatchAndHighlight = searchMap.Length > 0;

            var ll = SearchLighterUtils.convertStringToLinkedList(html);


            //todo: OPTIMIZE & ELIMINATE DOUBLE-ITERATION
            //to preserve O(~N) for this, co-migrate 1 & 2, with head-start by configured amount, default 50 chars
            // (1) ESCAPE & SANITIZATION
            var c = ll.First;
            while (c != null)
            {
                var ch = c.Value.Char;
                SearchLighterUtils._escapeOrSanitize(this, ref c, ll);
                c = c.Next;
            }

            // (2) HIGHLIGHTING
            if (shouldMatchAndHighlight)
            {
                c = ll.First;
                while (c != null)
                {
                    SearchLighterUtils._matchAndHighlight(this, searchMap, ll, ref c, _minHighlightSubstringMatchLength);
                    c = c.Next;
                }
            }
            return SearchLighterUtils.convertLinkedListToString(ll);
        }
    }
}