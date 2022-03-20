using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pancake.ManagedGeometry.Utility
{
    internal static class LanguageExtensions
    {
        public static void Swap<T>(ref T a, ref T b)
        {
            T c = a;
            a = b;
            b = c;
        }
        public static List<List<T>> ToNestedLists<T>(this IEnumerable<IEnumerable<T>> src)
        {
            return src.Select(x => x.ToList()).ToList();
        }
    }
}
