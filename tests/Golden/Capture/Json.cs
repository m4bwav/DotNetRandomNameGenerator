#nullable disable

// Hand-written JSON writer for the golden capture (copied from CachingServiceWithAOPSupport's capture): the same text on every runtime, no serializer version in the way.
// Values: null, bool, int, long, string, JsonObject (ordered), and IEnumerable of values.
// Escapes are built from char codes (a backslash is (char)92) so no editor or shell can rewrite them.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace GoldenCapture
{
    public sealed class JsonObject : IEnumerable<KeyValuePair<string, object>>
    {
        private readonly List<KeyValuePair<string, object>> _items = new List<KeyValuePair<string, object>>();

        public void Add(string key, object value)
        {
            _items.Add(new KeyValuePair<string, object>(key, value));
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }
    }

    public static class Json
    {
        private const char Backslash = (char)92;
        private const char Quote = (char)34;

        public static string Write(object value)
        {
            var sb = new StringBuilder();
            WriteValue(sb, value, 0);
            sb.Append((char)10);
            return sb.ToString();
        }

        private static void Indent(StringBuilder sb, int depth)
        {
            sb.Append((char)10);
            sb.Append(' ', depth * 2);
        }

        private static void WriteValue(StringBuilder sb, object value, int depth)
        {
            if (value == null)
            {
                sb.Append("null");
            }
            else if (value is bool b)
            {
                sb.Append(b ? "true" : "false");
            }
            else if (value is int i)
            {
                sb.Append(i.ToString(CultureInfo.InvariantCulture));
            }
            else if (value is long l)
            {
                sb.Append(l.ToString(CultureInfo.InvariantCulture));
            }
            else if (value is string s)
            {
                WriteString(sb, s);
            }
            else if (value is JsonObject o)
            {
                sb.Append('{');
                var first = true;
                foreach (var kv in o)
                {
                    if (!first)
                    {
                        sb.Append(',');
                    }

                    first = false;
                    Indent(sb, depth + 1);
                    WriteString(sb, kv.Key);
                    sb.Append(": ");
                    WriteValue(sb, kv.Value, depth + 1);
                }

                if (!first)
                {
                    Indent(sb, depth);
                }

                sb.Append('}');
            }
            else if (value is IEnumerable e)
            {
                sb.Append('[');
                var first = true;
                foreach (var item in e)
                {
                    if (!first)
                    {
                        sb.Append(',');
                    }

                    first = false;
                    Indent(sb, depth + 1);
                    WriteValue(sb, item, depth + 1);
                }

                if (!first)
                {
                    Indent(sb, depth);
                }

                sb.Append(']');
            }
            else
            {
                throw new ArgumentException("Json.Write cannot write a " + value.GetType().FullName);
            }
        }

        private static void WriteString(StringBuilder sb, string s)
        {
            sb.Append(Quote);
            foreach (var c in s)
            {
                if (c == Quote || c == Backslash)
                {
                    sb.Append(Backslash).Append(c);
                }
                else if (c < (char)32 || c == (char)127)
                {
                    sb.Append(Backslash).Append('u').Append(((int)c).ToString("x4", CultureInfo.InvariantCulture));
                }
                else
                {
                    sb.Append(c);
                }
            }

            sb.Append(Quote);
        }
    }
}
