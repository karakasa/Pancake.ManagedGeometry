using BenchmarkDotNet.Attributes;
using Pancake.ManagedGeometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossLibBenchmark.BasicGeometry
{
    [MemoryDiagnoser]
    public class ClosestPtOnPlane
    {
        const int POINT_COUNT = 100_000;
        private static Coord[] CreateGroup(Random random)
        {
            return Enumerable.Repeat(0, POINT_COUNT).Select(_ => (Coord)(
            random.NextDouble(),
            random.NextDouble(),
            random.NextDouble()
            )).ToArray();
        }

        private Coord[] samplePt1;
        private GShark.Geometry.Point3[] samplePt2;
        private Elements.Geometry.Vector3[] samplePt3;

        private Plane sampleLine1;
        private GShark.Geometry.Plane sampleLine2;
        private Elements.Geometry.Plane sampleLine3;

        [GlobalSetup]
        public void Setup()
        {
            samplePt1 = CreateGroup(new Random(42));
            samplePt2 = samplePt1.Select(pt => new GShark.Geometry.Point3(pt.X, pt.Y, pt.Z)).ToArray();
            samplePt3 = samplePt1.Select(pt => new Elements.Geometry.Vector3(pt.X, pt.Y, pt.Z)).ToArray();

            sampleLine1 = Plane.CreateFromComponents((0, 0, 0), (1, 1, 1), (-1, 1, -1));
            sampleLine2 = new GShark.Geometry.Plane(
                new GShark.Geometry.Point3(0, 0, 0),
                new GShark.Geometry.Vector3(1, 1, 1),
                new GShark.Geometry.Vector3(-1, 1, -1)
                );
            sampleLine3 = new Elements.Geometry.Plane(
                new Elements.Geometry.Vector3(0, 0, 0),
                new Elements.Geometry.Vector3[]
                {
                    new Elements.Geometry.Vector3(0, 0, 0),
                    new Elements.Geometry.Vector3(1,1,1),
                    new Elements.Geometry.Vector3(-1,1,-1)
                }
                );
        }

        [Benchmark(Baseline=true)]
        public double Pancake()
        {
            var sum = 0.0;

            foreach(var it in samplePt1)
                unchecked
                {
                    sum += (it - sampleLine1.Projected(it)).Length;
                }

            return sum;
        }
        [Benchmark]
        public double GShark()
        {
            var sum = 0.0;

            foreach (var it in samplePt2)
                unchecked
                {
                    sampleLine2.ClosestPoint(it, out var d);
                    sum += d;
                }

            return sum;
        }
        [Benchmark]
        public double Elements()
        {
            var sum = 0.0;

            foreach (var it in samplePt3)
                unchecked
                {
                    sum += (sampleLine3.ClosestPoint(it) - it).Length();
                }

            return sum;
        }
    }
}

