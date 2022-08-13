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
    public class Interval1dAlgo
    {
        readonly Interval1dSet _baseSet = new();
        public Interval1dAlgo()
        {
            _baseSet.UnionWith((1, 3));
            _baseSet.UnionWith((5, 7));
            _baseSet.UnionWith((9, 11));
        }

        [Benchmark]
        public int Inplace()
        {
            var set = _baseSet.Clone();
            set.SubtractByInplace((2, 10));
            return set.Count;
        }

        [Benchmark]
        public int Copied()
        {
            var set = _baseSet.Clone();
            set.SubtractByCopied((2, 10));
            return set.Count;
        }
    }
}
