using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Pancake.ManagedGeometry.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CrossLibBenchmark.Algo
{
    internal static class NaturalSortHelper
    {
        private static Regex _numbers = new Regex(@"\d+", RegexOptions.Compiled);

        public static IOrderedEnumerable<T> OrderByNatural<T>(this IEnumerable<T> items, Func<T, string> selector, StringComparer stringComparer = null)
        {
            int maxDigits = items
                          .SelectMany(i => _numbers.Matches(selector(i)).Cast<Match>().Select(digitChunk => (int?)digitChunk.Value.Length))
                          .Max() ?? 0;

            return items.OrderBy(i => _numbers.Replace(selector(i), match => match.Value.PadLeft(maxDigits, '0')), stringComparer ?? StringComparer.CurrentCulture);
        }
    }
#if NET7_0_OR_GREATER
    internal static partial class NaturalSortHelperNet7
    {
        private static readonly Regex _numbers = MyRegex();

        public static IOrderedEnumerable<T> OrderByNatural7<T>(this IEnumerable<T> items, Func<T, string> selector, StringComparer stringComparer = null)
        {
            var maxDigits = items
                          .SelectMany(i => _numbers.Matches(selector(i)).Cast<Match>().Select(digitChunk => (int?)digitChunk.Value.Length))
                          .Max() ?? 0;

            return items.OrderBy(i => _numbers.Replace(selector(i), match => match.Value.PadLeft(maxDigits, '0')), stringComparer ?? StringComparer.CurrentCulture);
        }

        [GeneratedRegex("\\d+", RegexOptions.Compiled)]
        private static partial Regex MyRegex();
    }

    public sealed partial class WinapiNaturalStringComparer7 : IComparer<string>
    {
        public static readonly WinapiNaturalStringComparer7 Instance = new();
        [SuppressUnmanagedCodeSecurity]
        private static partial class SafeNativeMethods
        {
            [LibraryImport("shlwapi.dll", EntryPoint = "StrCmpLogicalW", StringMarshalling = StringMarshalling.Utf16)]
            public static partial int StrCmpLogicalW(string psz1, string psz2);
        }
        public int Compare(string a, string b)
        {
            return SafeNativeMethods.StrCmpLogicalW(a, b);
        }
    }
#endif
    public sealed class WinapiNaturalStringComparer : IComparer<string>
    {
        public static readonly WinapiNaturalStringComparer Instance = new();
        [SuppressUnmanagedCodeSecurity]
        private static class SafeNativeMethods
        {
            [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
            public static extern int StrCmpLogicalW(string psz1, string psz2);
        }
        public int Compare(string a, string b)
        {
            return SafeNativeMethods.StrCmpLogicalW(a, b);
        }
    }

    internal sealed class JDSort : IComparer<string>
    {
        public static readonly JDSort Instance = new JDSort();
        public int Compare(string x, string y) 
            => CompareNatural(x, y, CultureInfo.InvariantCulture, CompareOptions.Ordinal);
        public static int CompareNatural(string strA, string strB)
        {
            return CompareNatural(strA, strB, CultureInfo.CurrentCulture, CompareOptions.IgnoreCase);
        }

        public static int CompareNatural(string strA, string strB, CultureInfo culture, CompareOptions options)
        {
            CompareInfo cmp = culture.CompareInfo;
            int iA = 0;
            int iB = 0;
            int softResult = 0;
            int softResultWeight = 0;
            while (iA < strA.Length && iB < strB.Length)
            {
                bool isDigitA = Char.IsDigit(strA[iA]);
                bool isDigitB = Char.IsDigit(strB[iB]);
                if (isDigitA != isDigitB)
                {
                    return cmp.Compare(strA, iA, strB, iB, options);
                }
                else if (!isDigitA && !isDigitB)
                {
                    int jA = iA + 1;
                    int jB = iB + 1;
                    while (jA < strA.Length && !Char.IsDigit(strA[jA])) jA++;
                    while (jB < strB.Length && !Char.IsDigit(strB[jB])) jB++;
                    int cmpResult = cmp.Compare(strA, iA, jA - iA, strB, iB, jB - iB, options);
                    if (cmpResult != 0)
                    {
                        // Certain strings may be considered different due to "soft" differences that are
                        // ignored if more significant differences follow, e.g. a hyphen only affects the
                        // comparison if no other differences follow
                        string sectionA = strA.Substring(iA, jA - iA);
                        string sectionB = strB.Substring(iB, jB - iB);
                        if (cmp.Compare(sectionA + "1", sectionB + "2", options) ==
                            cmp.Compare(sectionA + "2", sectionB + "1", options))
                        {
                            return cmp.Compare(strA, iA, strB, iB, options);
                        }
                        else if (softResultWeight < 1)
                        {
                            softResult = cmpResult;
                            softResultWeight = 1;
                        }
                    }
                    iA = jA;
                    iB = jB;
                }
                else
                {
                    char zeroA = (char)(strA[iA] - (int)Char.GetNumericValue(strA[iA]));
                    char zeroB = (char)(strB[iB] - (int)Char.GetNumericValue(strB[iB]));
                    int jA = iA;
                    int jB = iB;
                    while (jA < strA.Length && strA[jA] == zeroA) jA++;
                    while (jB < strB.Length && strB[jB] == zeroB) jB++;
                    int resultIfSameLength = 0;
                    do
                    {
                        isDigitA = jA < strA.Length && Char.IsDigit(strA[jA]);
                        isDigitB = jB < strB.Length && Char.IsDigit(strB[jB]);
                        int numA = isDigitA ? (int)Char.GetNumericValue(strA[jA]) : 0;
                        int numB = isDigitB ? (int)Char.GetNumericValue(strB[jB]) : 0;
                        if (isDigitA && (char)(strA[jA] - numA) != zeroA) isDigitA = false;
                        if (isDigitB && (char)(strB[jB] - numB) != zeroB) isDigitB = false;
                        if (isDigitA && isDigitB)
                        {
                            if (numA != numB && resultIfSameLength == 0)
                            {
                                resultIfSameLength = numA < numB ? -1 : 1;
                            }
                            jA++;
                            jB++;
                        }
                    }
                    while (isDigitA && isDigitB);
                    if (isDigitA != isDigitB)
                    {
                        // One number has more digits than the other (ignoring leading zeros) - the longer
                        // number must be larger
                        return isDigitA ? 1 : -1;
                    }
                    else if (resultIfSameLength != 0)
                    {
                        // Both numbers are the same length (ignoring leading zeros) and at least one of
                        // the digits differed - the first difference determines the result
                        return resultIfSameLength;
                    }
                    int lA = jA - iA;
                    int lB = jB - iB;
                    if (lA != lB)
                    {
                        // Both numbers are equivalent but one has more leading zeros
                        return lA > lB ? -1 : 1;
                    }
                    else if (zeroA != zeroB && softResultWeight < 2)
                    {
                        softResult = cmp.Compare(strA, iA, 1, strB, iB, 1, options);
                        softResultWeight = 2;
                    }
                    iA = jA;
                    iB = jB;
                }
            }
            if (iA < strA.Length || iB < strB.Length)
            {
                return iA < strA.Length ? 1 : -1;
            }
            else if (softResult != 0)
            {
                return softResult;
            }
            return 0;
        }
    }

    [MemoryDiagnoser]
    // [SimpleJob(runtimeMoniker: RuntimeMoniker.Net48, baseline: true)]
    // [SimpleJob(runtimeMoniker: RuntimeMoniker.Net60)]
    [SimpleJob(runtimeMoniker: RuntimeMoniker.HostProcess)]
    public class NaturalSort
    {
        private string[] UnsortedData;

        [GlobalSetup]
        public void PrepareData()
        {
            UnsortedData = new[] { "-abc12.txt", "abc12.txt", "1abc_2.txt", "a0000012.txt", "a0000012c.txt", "a000012.txt", "a000012b.txt", "a012.txt", "a0000102.txt", "abc1_2.txt", "abc12", "abc12b", "abc123", "abccde", "b0000", "b00001.txt", "b0001.txt", "b001.txt", "c0000.txt", "c0000c.txt", "c00001", "c000b", "d0.20.2b", "d0.1000c", "d0.2000y", "d0.20000.2b", "e0000120000012", "e0000012000012" };
        }
        [Benchmark(Baseline = true)]
        public string[] Pancake()
        {
            Array.Sort(UnsortedData, SimpleNaturalSort.Instance);
            return UnsortedData;
        }
        [Benchmark]
        public string[] PancakeWithExtension()
        {
            Array.Sort(UnsortedData, SimpleNaturalSortWithExtension.Instance);
            return UnsortedData;
        }
        [Benchmark]
        public string[] PancakeWithCulture()
        {
            Array.Sort(UnsortedData, new SimpleNaturalSortWithCultureInfo(CultureInfo.InvariantCulture));
            return UnsortedData;
        }
        [Benchmark]
        public string[] RegExLinq()
        {
            return UnsortedData.OrderByNatural(static x => x).ToArray();
        }
#if NET7_0_OR_GREATER
        [Benchmark]
        public string[] RegExLinq7()
        {
            return UnsortedData.OrderByNatural7(static x => x).ToArray();
        }

        [Benchmark]
        public string[] WindowsAPI7()
        {
            Array.Sort(UnsortedData, WinapiNaturalStringComparer7.Instance);
            return UnsortedData;
        }
#endif
        [Benchmark]
        public string[] WindowsAPI()
        {
            Array.Sort(UnsortedData, WinapiNaturalStringComparer.Instance);
            return UnsortedData;
        }
        [Benchmark]
        public string[] StackOverflow()
        {
            Array.Sort(UnsortedData, JDSort.Instance);
            return UnsortedData;
        }
    }

}
