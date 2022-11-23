using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Pancake.ManagedGeometry.Utility
{
    public sealed class SimpleNaturalSortWithCultureInfo : IComparer<string>, IComparer
    {
        private readonly CompareInfo _compare;
        private readonly CompareOptions _option;
        public SimpleNaturalSortWithCultureInfo(CultureInfo culture)
            : this(culture, CompareOptions.None)
        {
        }
        public SimpleNaturalSortWithCultureInfo(CultureInfo culture, CompareOptions option)
        {
            _compare = culture.CompareInfo;
            _option = option;
        }
        public int Compare(string x, string y) => Compare(x, y, 0, 0, x.Length, y.Length, _option);
        private int Compare(string x, string y,
            int startIndexX, int startIndexY, int endIndexX, int endIndexY, CompareOptions options)
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
                    var result = _compare.Compare(x, indX, 1, y, indY, 1, options);

                    if (result != 0)
                        return result;

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
                if (startX == endX || GetNumeric(x[startX]) != 0)
                    break;

                ++startX;
            }

            for (; ; )
            {
                if (startY == endY || GetNumeric(y[startY]) != 0)
                    break;

                ++startY;
            }

            var lenX = endX - startX;
            var lenY = endY - startY;

            if (lenX < lenY) return -1;
            if (lenX > lenY) return 1;

            for (; startX < endX; ++startX, ++startY)
            {
                var cx = GetNumeric(x[startX]);
                var cy = GetNumeric(y[startY]);

                if (cx < cy) return -1;
                if (cx > cy) return 1;
            }

            return 0;
        }
        private static int LocateFirstNonDigitChar(string str, int startIndex, int length, out ulong? number, out bool overflow)
        {
            number = (ulong)GetNumeric(str[startIndex]);
            var i = startIndex + 1;
            var numberLength = 1;

            for (; i < length; i++)
            {
                var c = str[i];

                if (!IsDigit(c))
                    break;

                number = number * 10 + (ulong)GetNumeric(c);
                ++numberLength;
            }

            overflow = numberLength >= 20;

            return i;
        }
        private static int GetNumeric(char c) => CharUnicodeInfo.GetDigitValue(c);
        private static bool IsDigit(char c) => char.IsDigit(c);
        public int Compare(object x, object y)
            => Compare(x?.ToString(), y?.ToString());
    }
}
