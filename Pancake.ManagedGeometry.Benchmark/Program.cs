using BenchmarkDotNet.Running;
using System;

namespace Pancake.ManagedGeometry.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<UnionTest>();
        }
    }
}
