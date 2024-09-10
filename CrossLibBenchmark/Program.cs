using BenchmarkDotNet.Running;
using System;

namespace CrossLibBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            //BenchmarkRunner.Run<BasicGeometry.Colinear2d>();
            BenchmarkRunner.Run<BasicGeometry.Colinear2d>();
            Console.ReadLine();
        }
    }
}
