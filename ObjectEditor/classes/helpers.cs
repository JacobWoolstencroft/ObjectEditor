using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Windows.Forms;

namespace ObjectEditor
{
    internal static class helpers
    {
        public static string ToShortString(this decimal val)
        {
            string s = val.ToString();
            if (s.Contains('.'))
                s = s.TrimEnd('0').TrimEnd('.');
            return s;
        }
        public static string ToShortString(this double val)
        {
            string s = val.ToString();
            if (s.Contains('.'))
                s = s.TrimEnd('0').TrimEnd('.');
            return s;
        }
        public static string Collapse(this IEnumerable<string> list, string Separator)
        {
            bool first = true;
            StringBuilder r = new StringBuilder();
            if (list != null)
            {
                foreach (string str in list)
                {
                    if (first)
                        first = false;
                    else
                        r.Append(Separator);
                    r.Append(str);
                }
            }
            return r.ToString();
        }
        public static List<string> Expand(this string str, char separator, bool trim = false, StringSplitOptions SplitOptions = StringSplitOptions.None, bool EmptyListToNull = false)
        {
            char[] separators = new char[] { separator };
            return Expand(str, separators, trim, SplitOptions, EmptyListToNull);
        }
        public static List<string> Expand(this string str, char[] Separators, bool trim = false, StringSplitOptions SplitOptions = StringSplitOptions.None, bool EmptyListToNull = false)
        {
            List<string> r = new List<string>();
            string[] word = str.Split(Separators, SplitOptions);
            string t;
            for (int x = 0; x < word.Length; x++)
            {
                t = word[x];
                if (trim)
                {
                    t = t.Trim();
                    if (t.Length < 1)
                        continue;
                }
                r.Add(t);
            }
            if (r.Count < 1 && EmptyListToNull)
                return null;
            return r;
        }
        public static void ResizeColumns(this ListView lst)
        {
            for (int x = 0; x < lst.Columns.Count; x++)
            {
                lst.Columns[x].Width = -2;
            }
        }
    }
}
