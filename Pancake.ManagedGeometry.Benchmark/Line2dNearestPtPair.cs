using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pancake.ManagedGeometry.Benchmark
{
    [SimpleJob(RuntimeMoniker.Net472, baseline: true)]
    [SimpleJob(RuntimeMoniker.HostProcess)]
    [MemoryDiagnoser]
    public class Line2dNearestPtPair
    {
        const int N = 100000;
        const double Rate = 10000.0;
        private Line2d[] Lines = new Line2d[N];

        [GlobalSetup]
        public void Setup()
        {
            var rand = new Random();

            for (var i = 0; i < N; i++)
            {
                Lines[i] = new Line2d(
                    (rand.NextDouble() * Rate, rand.NextDouble() * Rate),
                (rand.NextDouble() * Rate, rand.NextDouble() * Rate));
            }
        }

        [Benchmark]
        public double Accurate()
        {
            var sum = 0.0;

            for (var i = 0; i < Lines.Length - 1; i += 2)
            {
                unchecked
                {
                    sum += Lines[i].NearestPtToAnotherLine(Lines[i + 1], out _, out _);
                }
            }

            return sum;
        }
    }
}
