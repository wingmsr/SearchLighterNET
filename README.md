# SearchLighterNET
A light-weight .NET library for highlighting search text within results. Injects differential highlighting-markup for exact or partial (word) matches. Supports configuration of exact and partial-match markup, sanitization (e.g. angle-brackets) and escape (e.g. new-line) string maps, "stop-words", word-boundary characters, and minimum match-length thresholds.

C# Usage:

(1)	Add a reference to the SearchLighterNET.dll or SearchLighterNET.csproj, and use it in your code:

        using SearchLighterNET;
  
(2)	Get highlighted and sanitized text:

		string find = "jumped OVER";
		string text = "The quick brown fox\r\njumped over<BR/>and over the <lazy>dog</lazy>.";
		string highlighted = SearchLighter.GetDisplayString(text, find);
		string expectedOutput = "The quick brown fox<br /><span class=\"hlt1\">jumped over</span><br />and <span class=\"hlt2\">over</span> the &lt;lazy&gt;dog&lt;/lazy&gt;.";
		Assert.AreEqual(string.Compare(highlighted, expectedOutput, StringComparison.CurrentCulture), 0);
		//Or if you are presenting text without any search filter, simply pass an empty-string for the "find" parameter
		string normal = SearchLighter.GetDisplayString(text, "");
			
(3)	Advanced usage: Configure any non default settings via static-method calls, e.g.:

        /* Note: The HTML-friendly default "escape-markup" includes the most common variants of new-line characters and line-break HTML.
         * Other markup is partially HTML-encoded by default, i.e. "angle-brackets" are HTML-encoded to "sanitize" the output.
         * See the Unit-Test suite for expected behaviour of edge-cases.
         */		 
        SearchLighter.HighlighterResetToDefaults();
        SearchLighter.SetEscapeMarkupMap(SearchLighterConfigurationTools.TestLineBreakEscapeMarkupMap());
        SearchLighter.SetSanitizationMap(SearchLighterConfigurationTools.TestSanitizationMap());
        SearchLighter.HighlighterSetWordMinLength(2);
        SearchLighter.HighlighterSetExactMatchMinLength(2);
        SearchLighter.HighlighterClearSkipWords();//a.k.a. "stop-words"
        SearchLighter.SetExactMatchOpenMarkup("<span class=\"my-exact-match\">");//default is '<span class="hlt1">'
        SearchLighter.SetExactMatchCloseMarkup("</span>");
        SearchLighter.SetPartialMatchOpenMarkup("<span class=\"my-partial-match\">");//default is '<span class="hlt2">'
        SearchLighter.SetPartialMatchCloseMarkup("</span>");
