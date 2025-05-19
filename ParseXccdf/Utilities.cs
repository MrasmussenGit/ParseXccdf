using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ParseXccdf
{
    internal class Utilities
    {
        public static bool WildcardMatch(string input, string pattern)
        {
            string regexPattern = "^" + Regex.Escape(pattern)
                .Replace(@"\*", ".*")
                .Replace(@"\?", ".") + "$";

            return Regex.IsMatch(input, regexPattern);
        }

        public static void CopyProperties<T>(T source, T target)
        {
            foreach (PropertyInfo prop in typeof(T).GetProperties())
            {
                if (prop.CanRead && prop.CanWrite)
                {
                    prop.SetValue(target, prop.GetValue(source));
                }
            }
        }

    }
}
