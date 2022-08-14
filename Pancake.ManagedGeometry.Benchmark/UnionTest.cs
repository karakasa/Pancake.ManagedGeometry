using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Pancake.ManagedGeometry.Algo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pancake.ManagedGeometry.Benchmark
{
    [SimpleJob(RuntimeMoniker.Net48, baseline: true)]
    [SimpleJob(RuntimeMoniker.HostProcess)]
    [MemoryDiagnoser]
    public class UnionTest
    {
        readonly Interval1dSet set;
        public UnionTest()
        {
            set = new Interval1dSet();
            for (var i = 0; i < 100; i++)
                set.UnionWith((1 + 0.01 * i, 1 + 0.01 * i + 0.01));
            for (var i = 0; i < 100; i++)
                set.UnionWith((3 + 0.01 * i, 3 + 0.01 * i + 0.01));
            for (var i = 0; i < 100; i++)
                set.UnionWith((7 + 0.01 * i, 7 + 0.01 * i + 0.01));
        }
        [Benchmark]
        public int Union()
        {
            var set2 = set.Clone();
            set2.UnionWith((1.5, 7.5));
            return set2.Count;
        }
        [Benchmark]
        public int Intersect()
        {
            var set2 = set.Clone();
            set2.IntersectWith((1.5, 7.5));
            return set2.Count;
        }
        [Benchmark]
        public int Subtract()
        {
            var set2 = set.Clone();
            set2.SubtractBy((1.5, 7.5));
            return set2.Count;
        }
    }
}
