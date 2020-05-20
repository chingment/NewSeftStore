using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class StringExtensions
    {
        public static string NullToEmpty(this string s)
        {
            if (s == null)
            {
                return "";
            }
            else
            {
                return s.ToString().Trim();
            }
        }

        public static string Trim2(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                return s.ToString().Trim();
            }

            return s;
        }
    }
}
