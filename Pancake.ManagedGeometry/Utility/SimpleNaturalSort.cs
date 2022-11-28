using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;

namespace Pancake.ManagedGeometry.Utility
{
    public static class NaturalSortExtensions
    {
        public static IOrderedEnumerable<TSrc> OrderByNatural<TSrc>(this IEnumerable<TSrc> src, Func<TSrc, string> keySelector)
        {
            return src.OrderBy(keySelector, SimpleNaturalSort.Instance);
        }
        public static IOrderedEnumerable<TSrc> OrderByNaturalDescending<TSrc>(this IEnumerable<TSrc> src, Func<TSrc, string> keySelector)
        {
            return src.OrderByDescending(keySelector, SimpleNaturalSort.Instance);
        }
        public static IOrderedEnumerable<TSrc> ThenByNatural<TSrc>(this IOrderedEnumerable<TSrc> src, Func<TSrc, string> keySelector)
        {
            return src.ThenBy(keySelector, SimpleNaturalSort.Instance);
        }
        public static IOrderedEnumerable<TSrc> ThenByNaturalDescending<TSrc>(this IOrderedEnumerable<TSrc> src, Func<TSrc, string> keySelector)
        {
            return src.ThenByDescending(keySelector, SimpleNaturalSort.Instance);
        }
    }
    public sealed class SimpleNaturalSort : IComparer<string>, IComparer
    {
        public static readonly SimpleNaturalSort Instance = new();
        public int Compare(string x, string y) => CompareStatic(x, y);

        public struct Struct : IComparer<string>
        {
            public int Compare(string x, string y) => CompareStatic(x, y);
            public int Compare(string x, string y, int startIndexX, int startIndexY, int endIndexX, int endIndexY)
            => CompareStatic(x, y, startIndexX, startIndexY, endIndexX, endIndexY);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CompareStatic(string x, string y)
            => CompareStatic(x, y, 0, 0, x.Length, y.Length);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CompareStatic(string x, string y, 
            int startIndexX, int startIndexY, int endIndexX, int endIndexY)
        {
            var atEndX = x is null;
            var atEndY = y is null;

            if (atEndX && atEndY) return 0;
            if (atEndX && !atEndY) return -1;
            if (!atEndX && atEndY) return 1;

            var indX = startIndexX;
            var indY = startIndexY;

            var lenX = endIndexX;
            var lenY = endIndexY;

            for (; ; )
            {
                atEndX = indX == lenX;
                atEndY = indY == lenY;

                if (atEndX && atEndY) return 0;
                if (atEndX && !atEndY) return -1;
                if (!atEndX && atEndY) return 1;

                var cx = x[indX];
                var cy = y[indY];

                atEndX = IsDigit(cx);
                atEndY = IsDigit(cy);

                if (!atEndX && !atEndY)
                {
                    if (cx < cy)
                        return -1;

                    if (cx > cy)
                        return 1;

                    ++indX;
                    ++indY;
                    continue;
                }

                if (atEndX && !atEndY)
                    return 1;

                if (!atEndX && atEndY)
                    return -1;

                var firstNonDigitCharX = LocateFirstNonDigitChar(x, indX, lenX, out var numX, out atEndX);
                var firstNonDigitCharY = LocateFirstNonDigitChar(y, indY, lenY, out var numY, out atEndY);

                if (!atEndX && !atEndY)
                {
                    // No overflow

                    if (numX < numY)
                        return -1;

                    if (numX > numY)
                        return 1;
                }
                else
                {
                    // X and/or Y are overflowed

                    var result = CompareOverflowedString(x, y, indX, indY, firstNonDigitCharX, firstNonDigitCharY);
                    if (result != 0)
                        return result;
                }

                indX = firstNonDigitCharX;
                indY = firstNonDigitCharY;
            }
        }

        private static int CompareOverflowedString(
            string x, string y,
            int startX, int startY,
            int endX, int endY)
        {
            for (; ; )
            {
                if (startX == endX || x[startX] != '0')
                    break;

                ++startX;
            }

            for (; ; )
            {
                if (startY == endY || y[startY] != '0')
                    break;

                ++startY;
            }

            var lenX = endX - startX;
            var lenY = endY - startY;

            if (lenX < lenY) return -1;
            if (lenX > lenY) return 1;

            for (; startX < endX; ++startX, ++startY)
            {
                var cx = x[startX];
                var cy = y[startY];

                if (cx < cy) return -1;
                if (cx > cy) return 1;
            }

            return 0;
        }
        private static int LocateFirstNonDigitChar(string str, int startIndex, int length, out ulong? number, out bool overflow)
        {
            number = (ulong)(str[startIndex] - '0');
            var i = startIndex + 1;
            var numberLength = 1;

            for (; i < length; i++)
            {
                var c = str[i];

                if (!IsDigit(c))
                    break;

                number = number * 10 + c - '0';
                ++numberLength;
            }

            overflow = numberLength >= 20;

            return i;
        }
        private static bool IsDigit(char c)
            => c >= '0' && c <= '9';
        public int Compare(object x, object y)
            => Compare(x?.ToString(), y?.ToString());
    }
}
