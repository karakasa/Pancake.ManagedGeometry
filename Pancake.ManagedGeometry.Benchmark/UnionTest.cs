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
        Interval1dSet set;
        public UnionTest()
        {
            set = new Interval1dSet();
            set.UnionWith((1, 2));
            set.UnionWith((3, 5));
            set.UnionWith((7, 9));
        }
        [Benchmark]
        public int NewMethod()
        {
            var set2 = set.Clone();
            set2.UnionWith((1.5, 7.5));
            return set2.Count;
        }
        [Benchmark]
        public int OldMethod()
        {
            var set2 = set.Clone();
            // set2.UnionWithOld((1.5, 7.5));
            return set2.Count;
        }
    }
}
