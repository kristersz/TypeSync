using System.Text;

namespace TypeSync.Common.Utilities
{
    public static class NameCaseConverter
    {
        public static string ToCamelCase(string str)
        {
            if (string.IsNullOrEmpty(str) || !char.IsUpper(str[0]))
            {
                return str;
            }

            char[] chars = str.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                if (i == 1 && !char.IsUpper(chars[i]))
                {
                    break;
                }

                bool hasNext = (i + 1 < chars.Length);
                if (i > 0 && hasNext && !char.IsUpper(chars[i + 1]))
                {
                    break;
                }

                char c;

                c = char.ToLowerInvariant(chars[i]);

                chars[i] = c;
            }

            return new string(chars);
        }

        public static string ToKebabCase(string s)
        {
            return ToSnakeOrKebabCase(s, '-');
        }       

        public static string ToSnakeCase(string s)
        {
            return ToSnakeOrKebabCase(s, '_');
        }

        internal enum CaseState
        {
            Start,
            Lower,
            Upper,
            NewWord
        }

        private static string ToSnakeOrKebabCase(string s, char replaceWith)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }

            StringBuilder sb = new StringBuilder();
            CaseState state = CaseState.Start;

            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == ' ')
                {
                    if (state != CaseState.Start)
                    {
                        state = CaseState.NewWord;
                    }
                }
                else if (char.IsUpper(s[i]))
                {
                    switch (state)
                    {
                        case CaseState.Upper:
                            bool hasNext = (i + 1 < s.Length);
                            if (i > 0 && hasNext)
                            {
                                char nextChar = s[i + 1];
                                if (!char.IsUpper(nextChar) && nextChar != replaceWith)
                                {
                                    sb.Append(replaceWith);
                                }
                            }
                            break;
                        case CaseState.Lower:
                        case CaseState.NewWord:
                            sb.Append(replaceWith);
                            break;
                    }

                    char c;

                    c = char.ToLowerInvariant(s[i]);

                    sb.Append(c);

                    state = CaseState.Upper;
                }
                else if (s[i] == replaceWith)
                {
                    sb.Append(replaceWith);
                    state = CaseState.Start;
                }
                else
                {
                    if (state == CaseState.NewWord)
                    {
                        sb.Append(replaceWith);
                    }

                    sb.Append(s[i]);
                    state = CaseState.Lower;
                }
            }

            return sb.ToString();
        }
    }
}
