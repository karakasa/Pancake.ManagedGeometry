using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.Windows.Configs;
using Pancake.ManagedGeometry.Algo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pancake.ManagedGeometry.Benchmark
{
    [MemoryDiagnoser]
    [InliningDiagnoser(false, new[] { "Pancake.ManagedGeometry.Benchmark", "Pancake.ManagedGeometry", "Pancake.ManagedGeometry.Algo", "Pancake.ManagedGeometry.Utility" })]
    public class DistincterBenchmark
    {
        private struct IntComparer : IComparer<int>
        {
            public int Compare(int x, int y)
            {
                return x.CompareTo(y);
            }
        }

        private struct IntEqualityComparer : IEqualityComparer<int>
        {
            public bool Equals(int x, int y)
            {
                return x == y;
            }

            public int GetHashCode(int x)
            {
                return x.GetHashCode();
            }
        }

        private static readonly Comparer<int> _comparerBoxed = Comparer<int>.Default;
        private static readonly IntComparer _comparer = new();
        private static readonly IComparer<int> _comparerB = new IntComparer();
        private static readonly IntEqualityComparer _comparer2 = new();

        // [Benchmark]
        public int HashSetMethod()
        {
            var intArray = new int[] { 46, 69, 65, 16, 45, 35, 88, 3, 95, 10, 43, 38, 52, 56, 11, 31, 25, 37, 67, 61, 11, 10, 45, 35, 69 };


            Array.Sort(intArray, _comparer);
            var hs = new HashSet<int>(intArray.Length, _comparer2);

            var sum = 0;

            foreach (var it in intArray)
            {
                if (hs.Add(it))
                    sum += it;
            }

            return sum;
        }
        // [Benchmark]
        public int LINQMethod()
        {
            var intArray = new int[] { 46, 69, 65, 16, 45, 35, 88, 3, 95, 10, 43, 38, 52, 56, 11, 31, 25, 37, 67, 61, 11, 10, 45, 35, 69 };

            return intArray.OrderBy(static s => s).Distinct(_comparer2).Sum();
        }
        [Benchmark]
        public int InplaceMethod()
        {
            var intArray = new int[] { 46, 69, 65, 16, 45, 35, 88, 3, 95, 10, 43, 38, 52, 56, 11, 31, 25, 37, 67, 61, 11, 10, 45, 35, 69 };
            var index = Distincter.SortAndDistinctArrayInplace(intArray, _comparer, _comparerBoxed);

            var sum = 0;

            for (var i = 0; i < index; i++)
                sum += intArray[i];
            return sum;
        }
    }
}
