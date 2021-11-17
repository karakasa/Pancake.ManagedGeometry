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
    public class Matrix44Inverse
    {
        const int N = 10;
        const double Rate = 10000.0;
        private Matrix44[] Matrixes = new Matrix44[N];

        [GlobalSetup]
        public void Setup()
        {
            var rand = new Random();
            var data = new double[16];

            for (var i = 0; i < N; i++)
            {
                RandomFill(rand, ref data);
                Matrixes[i] = Matrix44.CreateByRowArray(data);
            }
        }

        private static void RandomFill(Random rand, ref double[] array)
        {
            for (var i = 0; i < array.Length; i++) array[i] = rand.NextDouble() * Rate;
        }

        [Benchmark]
        public double InverseInlined()
        {
            var sum = 0;

            for (var i = 0; i < Matrixes.Length; i++)
            {
                sum += Matrixes[i].TryGetInverse(out _) ? 1 : 0;
            }

            return sum;
        }
        [Benchmark]
        public double InverseNotInlined()
        {
            var sum = 0;

            for (var i = 0; i < Matrixes.Length; i++)
            {
                sum += Matrixes[i].TryGetInverse2(out _) ? 1 : 0;
            }

            return sum;
        }
    }
}
