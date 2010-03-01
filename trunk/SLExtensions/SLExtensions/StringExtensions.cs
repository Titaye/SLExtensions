using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace SLExtensions
{
    public static class StringExtensions
    {
        public static string[] Split(this string input, char[] splitChar, int max)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (input.Length == 0)
                return new string[0];

            List<int> charIdx = new List<int>(max);
            for (int i = 0; i < input.Length; i++)
            {
                if (splitChar.Contains(input[i]))
                    charIdx.Add(i);

                if (charIdx.Count == max)
                    break;
            }

            int lastIdx = 0;
            string[] result = new string[charIdx.Count + 1];
            for (int i = 0; i < charIdx.Count; i++)
            {
                var currentIdx = charIdx[i];
                result[i] = input.Substring(lastIdx,  currentIdx - lastIdx);
                lastIdx = currentIdx + 1;
            }
            result[result.Length - 1] = input.Substring(lastIdx, input.Length - lastIdx);

            return result;
        }
    }
}
