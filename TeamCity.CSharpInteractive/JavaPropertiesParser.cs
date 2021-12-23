// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    public class JavaPropertiesParser : IJavaPropertiesParser
    {
        private static readonly Regex IgnorableLinePattern = new("^[ \t\f]*([#!].*)?$", RegexOptions.Compiled);
        private static readonly Regex ContinuingLinePattern = new(@"(^|.*[^\\])(\\\\)*\\$", RegexOptions.Compiled);

        public IReadOnlyDictionary<string, string> Parse(IEnumerable<string> lines)
        {
            var filteredLines =
                from line in lines
                where !IsIgnorable(line)
                select TrimLeadingWhiteSpaces(line);

            var composedLines = ComposeLines(filteredLines);
            var pairs = ParseLines(composedLines);
            return new Dictionary<string, string>(pairs, StringComparer.InvariantCultureIgnoreCase);
        }

        private static IEnumerable<string> ComposeLines(IEnumerable<string> lines)
        {
            using var linesEnumerator = lines.GetEnumerator();
            while (linesEnumerator.MoveNext())
            {
                var line = linesEnumerator.Current;
                if (!IsContinuing(line))
                {
                    yield return line;
                }
                else
                {
                    var sb = new StringBuilder(RemoveLastChar(line));
                    while (linesEnumerator.MoveNext())
                    {
                        if (IsContinuing(line))
                        {
                            sb.Append(RemoveLastChar(line));
                        }
                        else
                        {
                            sb.Append(line);
                            yield return sb.ToString();
                        }
                    }
                }
            }
        }

        private static IEnumerable<KeyValuePair<string, string>> ParseLines(IEnumerable<string> lines)
        {
            foreach (var line in lines)
            {
                var index = 0;
                var key = ParseKey(line, ref index);
                if (string.IsNullOrWhiteSpace(key))
                {
                    continue;
                }

                var value = ParseValue(line, index);
                yield return new KeyValuePair<string, string>(key, value);
            }
        }
        
        private static string ParseKey(string line, ref int index)
        {
            var builder = new StringBuilder();
            var len = line.Length;
            for (var i = 0; i < len; ++i)
            {
                var ch = line[i];
                if (IsSeparator(ch))
                {
                    ProcessSeparator(line, out index, i);
                    break;
                }

                if (IsWhiteSpace(ch))
                {
                    ProcessWhiteSpace(line, out index, i);
                    break;
                }

                if (ch == '\\')
                {
                    ProcessBackSlash(line, ref i, builder);
                }
                else
                {
                    builder.Append(ch);
                }

                if (i + 1 != len)
                {
                    continue;
                }
                
                index = len;
                break;
            }

            return builder.ToString();
        }


        private static void ProcessSeparator(string line, out int index, int i) => 
            index = SkipWhiteSpaces(line, i + 1);

        private static void ProcessWhiteSpace(string line, out int index, int i)
        {
            index = SkipWhiteSpaces(line, i + 1);
            if (index < line.Length && IsSeparator(line[index]))
            {
                index = SkipWhiteSpaces(line, index + 1);
            }
        }
        
        private static void ProcessBackSlash(string line, ref int index, StringBuilder builder)
        {
            var c = line[++index];
            switch (c)
            {
                case 'f': 
                    builder.Append('\f');
                    return;

                case 'n': 
                    builder.Append('\n');
                    return;

                case 'r':
                    builder.Append('\r');
                    return;

                case 't':
                    builder.Append('\t');
                    return;

                case 'u': 
                    break;

                default: 
                    builder.Append(c);
                    return;
            }

            if (!TryParseHex(line, index + 1, out var value))
            {
                throw new InvalidOperationException("Cannot read hex value.");
            }

            builder.Append((char)value);
            index += 4;
        }

        private static int SkipWhiteSpaces(string str, int index)
        {
            var len = str.Length;
            for ( ; index < len; ++index)
            {
                if (IsWhiteSpace(str[index]) == false)
                {
                    break;
                }
            }

            return index;
        }
        
        private static bool TryParseHex(string str, int start, out int value)
        {
            if (str.Length > start + 3)
            {
                var int0 = TryConvertHexToInt(str[start]);
                var int1 = TryConvertHexToInt(str[start + 1]);
                var int2 = TryConvertHexToInt(str[start + 2]);
                var int3 = TryConvertHexToInt(str[start + 3]);
                if (int0.HasValue && int1.HasValue && int2.HasValue && int3.HasValue)
                {
                    value = (int0.Value << 12) | (int1.Value << 8) | (int2.Value << 4) | int3.Value;
                    return true;
                }
            }

            value = default;
            return false;
        }

        private static string ParseValue(string line, int index)
        {
            var sb = new StringBuilder();
            for (var i = index; i < line.Length; ++i)
            {
                var ch = line[i];
                if (ch != '\\')
                {
                    sb.Append(ch);
                    continue;
                }

                ProcessBackSlash(line, ref i, sb);
            }

            return sb.ToString();
        }

        private static int? TryConvertHexToInt(char ch) =>
            ch switch
            {
                >= '0' and <= '9' => ch - '0',
                >= 'A' and <= 'F' => ch - 'A' + 10,
                >= 'a' and <= 'f' => ch - 'a' + 10,
                _ => null
            };
        
        private static string TrimLeadingWhiteSpaces(string str) => str.TrimStart(' ', '\t', '\f');
        
        private static string RemoveLastChar(string str) => str[..^1];

        private static bool IsWhiteSpace(char c) => char.IsWhiteSpace(c) || c is '\t' or '\f';

        private static bool IsSeparator(char c) => c is '=' or ':';
        
        private static bool IsIgnorable(string line) => IgnorableLinePattern.IsMatch(line);

        private static bool IsContinuing(string line) => ContinuingLinePattern.IsMatch(line);
    }
}