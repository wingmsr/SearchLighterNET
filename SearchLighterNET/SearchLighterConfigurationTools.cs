namespace SearchLighterNET
{
    public static class SearchLighterConfigurationTools
    {
        public static char[] DefaultWordBoundaryCharacters()
        {
            return new char[]
            {
                '\t',
                '\r',
                '\n',
                ' ',
                '!',
                '"',
                //'#',
                //'$',
                //'%',
                //'&',
                '\'',
                //'(',
                //')',
                //'*',
                //'+',
                ',',
                '-',
                '.',
                '/',
                ':',
                ';',
                '<',
                //'=',
                '>',
                '?',
                //'@',
                //'[',
                '\\',
                //']',
                //'^',
                '_',
                '`',
                //'{',
                '|',
                //'}',
                //'~'
            };
        }

        public static string[][] DefaultAngleBracketSanitizationMap()
        {
            return new string[][]
            {
                new[] {"<", "&lt;"}, new[] {">", "&gt;"}
            };
        }

        public static string[][] DefaultLineBreakEscapeMarkupMap()
        {
            return new string[][]
            {
                new[] {"<br />", "<br />"},
                new[] {"<br/>", "<br />"},
                new[] {"<BR />", "<br />"},
                new[] {"<BR/>", "<br />"},
                new[] {"\r\n", "<br />"},
                new[] {"\n", "<br />"}
            };
        }

        internal static string[][] TestSanitizationMap()
        {
            return new string[][]
            {
                new[] {"•", "$"}, new[] {"°", "^"}
            };
        }

        internal static string[][] TestLineBreakEscapeMarkupMap()
        {
            return new string[][]
            {
                new[] {"•br /°", "•_°"}
            };
        }

        internal static string _defaultO1()
        {
            return "<span class=\"hlt1\">";
        }

        internal static string _defaultO2()
        {
            return "<span class=\"hlt2\">";
        }

        internal static string _defaultC1()
        {
            return "</span>";
        }

        internal static string _defaultC2()
        {
            return "</span>";
        }

        internal static char[] _defaultBr()
        {
            return new char[]
            {
                '<',
                'b',
                'r',
                ' ',
                '/',
                '>'
            };
        }

        internal static string[] _defaultskipWords()
        {
            return new string[]
            {
                "A", "AND", "AS", "BY", "FOR", "HER", "HIS", "IN", "IT", "MY", "OF", "SO", "THAT",
                "THE", "THEM", "THEY", "THIS", "TO", "WAS", "WITH"
            };
        }

        internal static int _defaultMinHighlightWordLength()
        {
            return 3;
        }

        internal static int _defaultMinHighlightExactMatchLength()
        {
            return 2;
        }

        internal static int _defaultMinHighlightSubstringMatchLength()
        {
            return 5;
        }
    }
}