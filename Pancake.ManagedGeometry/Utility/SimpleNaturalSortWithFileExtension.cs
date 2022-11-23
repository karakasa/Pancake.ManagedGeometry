using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Pancake.ManagedGeometry.Utility
{
    public sealed class SimpleNaturalSortWithExtension : IComparer<string>, IComparer
    {
        public static readonly SimpleNaturalSortWithExtension Instance = new();

        public struct Struct : IComparer<string>
        {
            public int Compare(string x, string y) => CompareStatic(x, y);
        }
        public int Compare(object x, object y) => Compare(x?.ToString(), y?.ToString());
        public int Compare(string x, string y) => CompareStatic(x, y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CompareStatic(string x, string y)
        {
            var lx = LastIndexOfOrdinal(x);
            var ly = LastIndexOfOrdinal(y);

            if (lx > 0 && ly > 0)
            {
                var result = default(SimpleNaturalSort.Struct).Compare(x, y, 0, 0, lx, ly);
                if (result != 0)
                    return result;

                return default(SimpleNaturalSort.Struct).Compare(x, y, lx + 1, ly + 1, x.Length, y.Length);
            }

            if (lx < 0 && ly < 0)
            {
                return default(SimpleNaturalSort.Struct).Compare(x, y);
            }

            if (lx > 0 && ly < 0)
            {
                var result = default(SimpleNaturalSort.Struct).Compare(x, y, 0, 0, lx, y.Length);
                if (result != 0)
                    return result;

                return 1;
            }

            {
                var result = default(SimpleNaturalSort.Struct).Compare(x, y, 0, 0, x.Length, ly);
                if (result != 0)
                    return result;

                return -1;
            }
        }
        private static int LastIndexOfOrdinal(string x)
        {
            for (var i = x.Length - 1; i >= 0; i--)
            {
                if (x[i] == '.') return i;
            }

            return -1;
        }
    }
}
