using BenchmarkDotNet.Running;
using System;

namespace CrossLibBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            //BenchmarkRunner.Run<BasicGeometry.Colinear2d>();
            BenchmarkRunner.Run<Algo.NaturalSort>();
            Console.ReadLine();
        }
    }
}
