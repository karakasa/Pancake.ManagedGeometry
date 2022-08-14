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
        public static IEnumerable<T> DistinctByComparer<T>(
            this IEnumerable<T> src,
            IComparer<T> comparer
            )
        {
            var hasElement = false;
            T last = default;

            foreach (var it in src.OrderBy(s => s, comparer))
            {
                if (!hasElement)
                {
                    last = it;
                    yield return it;
                    hasElement = true;
                    continue;
                }

                if (comparer.Compare(last, it) == 0)
                    continue;

                last = it;
                yield return it;
            }
        }

        public static IEnumerable<T> DistinctByComparerInline<T, TComparer>(
            this IEnumerable<T> src,
            TComparer comparer
            )
            where TComparer : IComparer<T>
        {
            var hasElement = false;
            T last = default;

            foreach (var it in src.OrderBy(s => s, comparer))
            {
                if (!hasElement)
                {
                    last = it;
                    yield return it;
                    hasElement = true;
                    continue;
                }

                if (comparer.Compare(last, it) == 0)
                    continue;

                last = it;
                yield return it;
            }
        }
    }
}
