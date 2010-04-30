namespace SLExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class StringBuilderExtensions
    {
        #region Methods

        public static int FindClosingTag(this StringBuilder sb, string tag)
        {
            return sb.FindClosingTag(tag, 0);
        }

        public static int FindClosingTag(this StringBuilder sb, string tag, int idx)
        {
            int cnt = 0;
            bool inMatchTag = false;
            int currentTagStart = -1;
            for (int i = idx; i < sb.Length; i++)
            {
                if (sb[i] == '<')
                {
                    bool tagMatch = false;
                    if (i + 1 >= sb.Length)
                        return -1;

                    if (sb[i + 1] == '/')
                    {
                        //en tag
                        tagMatch = IsTagMatch(sb, tag, i + 1);
                        if (tagMatch)
                        {
                            cnt--;
                            if (cnt == 0)
                            {
                                return sb.IndexOf(">", i) + 1;
                            }
                        }
                    }
                    else
                    {
                        //start tag
                        tagMatch = IsTagMatch(sb, tag, i);
                        if (tagMatch)
                        {
                            cnt++;
                            inMatchTag = true;
                            currentTagStart = i;
                        }
                    }
                }

                if ( sb[i] == '>')
                {
                    if (i != 0 && inMatchTag && sb[i - 1] == '/')
                    {
                        cnt--;
                        if (cnt == 0)
                            return i + 1;
                    }
                    inMatchTag = false;
                }
            }

            return -1;
        }

        public static int IndexOf(this StringBuilder sb, string txt)
        {
            return sb.IndexOf(txt, 0);
        }

        public static int IndexOf(this StringBuilder sb, string txt, int idx)
        {
            return sb.IndexOf(txt, idx, true);
        }

        public static int IndexOf(this StringBuilder sb, string txt, int idx, bool caseSensitive)
        {
            if (string.IsNullOrEmpty(txt))
                return -1;

            char[] chars;
            if (caseSensitive)
                chars = txt.ToCharArray();
            else
                chars = txt.ToLower().ToCharArray();

            for (int i = idx; i < sb.Length - chars.Length + 1; i++)
            {
                // Check from the current char in string builder if the following chars are matching the reference char array
                for (int j = 0; j < chars.Length; j++)
                {
                    char compc = chars[j];
                    // Todo: put a buffer for caching case convertion
                    char sbc = caseSensitive ? sb[j + i] : Char.ToLower(sb[j + i]);

                    if (compc != sbc)
                    {
                        // one of the following chars are not matching
                        break;
                    }

                    if (j == chars.Length - 1)
                    {
                        // all the chars are matching return sb index
                        return i;
                    }
                }
            }

            return -1;
        }

        public static int LastIndexOf(this StringBuilder sb, string txt)
        {
            return sb.LastIndexOf(txt, sb.Length - 1);
        }

        public static int LastIndexOf(this StringBuilder sb, string txt, int idx)
        {
            return sb.LastIndexOf(txt, idx, true);
        }

        public static int LastIndexOf(this StringBuilder sb, string txt, int idx, bool caseSensitive)
        {
            if (string.IsNullOrEmpty(txt))
                return -1;

            char[] chars;
            if (caseSensitive)
                chars = txt.ToCharArray();
            else
                chars = txt.ToLower().ToCharArray();

            for (int i = Math.Min(sb.Length - 1, idx); i >= chars.Length - 1; i--)
            {
                // Check from the current char in string builder if the following chars are matching the reference char array
                for (int j = 0; j < chars.Length; j++)
                {
                    char compc = chars[chars.Length - 1 - j];
                    // Todo: put a buffer for caching case convertion
                    char sbc = caseSensitive ? sb[i - j] : Char.ToLower(sb[i - j]);

                    if (compc != sbc)
                    {
                        // one of the following chars are not matching
                        break;
                    }

                    if (j == chars.Length - 1)
                    {
                        // all the chars are matching return sb index
                        return i - chars.Length + 1;
                    }
                }
            }

            return -1;
        }

        private static bool IsTagMatch(StringBuilder sb, string tag, int i)
        {
            bool tagMatch = true;
            for (int j = 0; j < tag.Length && j + 1 + i < sb.Length; j++)
            {
                if (sb[i + 1 + j] != tag[j])
                {
                    tagMatch = false;
                    break;
                }
            }
            return tagMatch;
        }

        #endregion Methods
    }
}