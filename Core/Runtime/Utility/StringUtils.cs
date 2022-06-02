﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Rehawk.Kite
{
    ///Some common string utilities
    public static class StringUtils
    {
        public const string SPACE = " ";
        public const string ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public static readonly char[] CHAR_SPACE_ARRAY = { ' ' };
        private static readonly Dictionary<string, string> splitCaseCache = new Dictionary<string, string>(StringComparer.Ordinal);

        ///Convert camelCase to words.
        public static string SplitCamelCase(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }

            string result;
            if (splitCaseCache.TryGetValue(s, out result))
            {
                return result;
            }

            result = s;
            int underscoreIndex = result.IndexOf('_');
            if (underscoreIndex <= 1)
            {
                result = result.Substring(underscoreIndex + 1);
            }

            result = Regex.Replace(result, "(?<=[a-z])([A-Z])", " $1").CapitalizeFirst().Trim();
            return splitCaseCache[s] = result;
        }

        ///Capitalize first letter
        public static string CapitalizeFirst(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }

            return s.First().ToString().ToUpper() + s.Substring(1);
        }

        ///Caps the length of a string to max length and adds "..." if more.
        public static string CapLength(this string s, int max)
        {
            if (string.IsNullOrEmpty(s) || s.Length <= max || max <= 3)
            {
                return s;
            }

            string result = s.Substring(0, Mathf.Min(s.Length, max) - 3);
            result += "...";
            return result;
        }

        public static string Wrap(this string s, int wrapAt)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                return "";
            }

            s = s.Replace("  ", " ");
            string[] words = s.Split(' ');
            StringBuilder sb = new StringBuilder();
            StringBuilder currString = new StringBuilder();

            foreach (string word in words)
            {
                if (currString.Length + word.Length + 1 < wrapAt) // The + 1 accounts for spaces
                {
                    sb.AppendFormat(" {0}", word);
                    currString.AppendFormat(" {0}", word);
                }
                else
                {
                    currString.Clear();
                    sb.AppendFormat("{0}{1}", Environment.NewLine, word);
                    currString.AppendFormat(" {0}", word);
                }
            }

            return sb.ToString().TrimStart().TrimEnd();
        }

        ///Gets only the capitals of the string trimmed.
        public static string GetCapitals(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            string result = "";
            foreach (char c in s)
            {
                if (char.IsUpper(c))
                {
                    result += c.ToString();
                }
            }

            result = result.Trim();
            return result;
        }

        ///Formats input to error
        public static string FormatError(this string input)
        {
            return string.Format("<color=#ff6457>* {0} *</color>", input);
        }

        ///Returns the alphabet letter based on it's index.
        public static string GetAlphabetLetter(int index)
        {
            if (index < 0)
            {
                return null;
            }

            if (index >= ALPHABET.Length)
            {
                return index.ToString();
            }

            return ALPHABET[index].ToString();
        }

        ///Get the string result within from to
        public static string GetStringWithinOuter(this string input, char from, char to)
        {
            int start = input.IndexOf(from) + 1;
            int end = input.LastIndexOf(to);
            if (start < 0 || end < start)
            {
                return null;
            }

            return input.Substring(start, end - start);
        }

        ///Get the string result within from to
        public static string GetStringWithinInner(this string input, char from, char to)
        {
            int end = input.IndexOf(to);
            int start = int.MinValue;
            for (int i = 0; i < input.Length; i++)
            {
                if (i > end)
                {
                    break;
                }

                if (input[i] == from)
                {
                    start = i;
                }
            }

            start += 1;
            if (start < 0 || end < start)
            {
                return null;
            }

            return input.Substring(start, end - start);
        }

        ///Replace text within start and end chars based on provided processor
        public static string ReplaceWithin(this string text, char startChar, char endChar, Func<string, string> Process)
        {
            string s = text;
            int i = 0;
            while ((i = s.IndexOf(startChar, i)) != -1)
            {
                int end = s.Substring(i + 1).IndexOf(endChar);
                string input = s.Substring(i + 1, end); //what's in the chars
                string output = s.Substring(i, end + 2); //what should be replaced (includes chars)
                string result = Process(input);
                s = s.Replace(output, result);
                i++;
            }

            return s;
        }

        /// Returns a simplistic matching score (0-1) vs leaf + optional category.
        /// Lower is better so can be used without invert in OrderBy.
        public static float ScoreSearchMatch(string input, string leafName, string categoryName = "")
        {
            if (input == null || leafName == null)
            {
                return float.PositiveInfinity;
            }

            if (categoryName == null)
            {
                categoryName = string.Empty;
            }

            input = input.ToUpper();
            string[] inputWords = input.Replace('.', ' ').Split(CHAR_SPACE_ARRAY, StringSplitOptions.RemoveEmptyEntries);
            if (inputWords.Length == 0)
            {
                return 1;
            }

            leafName = leafName.ToUpper();
            string firstLeafWord = leafName.Split(CHAR_SPACE_ARRAY, StringSplitOptions.RemoveEmptyEntries)[0];
            leafName = leafName.Replace(" ", string.Empty);

            if (input.LastOrDefault() == '.')
            {
                leafName = categoryName.ToUpper().Replace(" ", string.Empty);
            }

            //remember lower is better
            float score = 1f;

            if (categoryName.Contains(inputWords[0]))
            {
                score *= 0.9f;
            }

            if (firstLeafWord == inputWords[inputWords.Length - 1])
            {
                score *= 0.5f;
            }

            if (leafName.StartsWith(inputWords[0]))
            {
                score *= 0.5f;
            }

            if (leafName.StartsWith(inputWords[inputWords.Length - 1]))
            {
                score *= 0.5f;
            }

            return score;
        }

        ///Returns whether or not the input is valid for a search match vs the leaf + optional category.
        public static bool SearchMatch(string input, string leafName, string categoryName = "")
        {
            if (input == null || leafName == null)
            {
                return false;
            }

            if (categoryName == null)
            {
                categoryName = string.Empty;
            }

            if (input.Length <= 1)
            {
                return input == leafName;
            }

            //ignore case
            input = input.ToUpper();
            leafName = leafName.ToUpper().Replace(" ", string.Empty);
            categoryName = categoryName.ToUpper().Replace(" ", string.Empty);
            string fullPath = categoryName + "/" + leafName;

            //treat dot as spaces and split to words
            string[] words = input.Replace('.', ' ').Split(CHAR_SPACE_ARRAY, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length == 0)
            {
                return false;
            }

            //last input char check
            if (input.LastOrDefault() == '.')
            {
                return categoryName.Contains(words[0]);
            }

            //check match for sequential occurency
            string leftover = fullPath;
            for (int i = 0; i < words.Length; i++)
            {
                string word = words[i];

                if (!leftover.Contains(word))
                {
                    return false;
                }

                leftover = leftover.Substring(leftover.IndexOf(word) + word.Length);
            }

            //last word should also be contained in leaf name regardless
            string lastWord = words[words.Length - 1];
            return leafName.Contains(lastWord);
        }

        ///A more complete ToString version
        public static string ToStringAdvanced(this object o)
        {
            if (o == null || o.Equals(null))
            {
                return "NULL";
            }

            if (o is string)
            {
                return string.Format("\"{0}\"", (string)o);
            }

            if (o is Object)
            {
                return (o as Object).name;
            }

            Type t = o.GetType();

            if (t.IsSubclassOf(typeof(Enum)))
            {
                if (t.HasAttribute<FlagsAttribute>(true))
                {
                    string value = string.Empty;
                    int cnt = 0;
                    Array list = Enum.GetValues(t);
                    foreach (object e in list)
                    {
                        if ((Convert.ToInt32(e) & Convert.ToInt32(o)) == Convert.ToInt32(e))
                        {
                            cnt++;
                            if (value == string.Empty)
                            {
                                value = e.ToString();
                            }
                            else
                            {
                                value += ", " + e;
                            }
                        }
                    }

                    if (cnt == 0)
                    {
                        return "Nothing";
                    }

                    if (cnt == list.Length)
                    {
                        return "Everything";
                    }

                    return value;
                }
            }

            return o.ToString();
        }

        public static string RemoveTags(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                input = Regex.Replace(input, "<[^>]*>", string.Empty);
                input = Regex.Replace(input, "{[^}]*}", string.Empty);
            }
            
            return input;
        }
        
        public static string RemoveSpecificTags(string input, string[] tags)
        {
            if (!string.IsNullOrEmpty(input))
            {
                for (int i = 0; i < tags.Length; i++)
                {
                    input = Regex.Replace(input, "<" + tags[i] + "[^>]*>", string.Empty);
                    input = Regex.Replace(input, "</" + tags[i] + "[^>]*>", string.Empty);
                    input = Regex.Replace(input, "{" + tags[i] + "[^}]*}", string.Empty);
                }
            }
            
            return input;
        }
    }
}