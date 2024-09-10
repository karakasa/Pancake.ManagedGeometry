using BenchmarkDotNet.Attributes;
using GShark.Core;
using Pancake.ManagedGeometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossLibBenchmark.BasicGeometry
{
    [MemoryDiagnoser]
    public class Colinear2d
    {
        const int POINT_COUNT = 100_000;

        public Coord[] _ptGrp1;
        public Coord[] _ptGrp2;
        public Coord[] _ptGrp3;

        public GShark.Geometry.Point3[] _ptGrp1gs;
        public GShark.Geometry.Point3[] _ptGrp2gs;
        public GShark.Geometry.Point3[] _ptGrp3gs;


        public Elements.Geometry.Vector3[] _ptGrp1he;
        public Elements.Geometry.Vector3[] _ptGrp2he;
        public Elements.Geometry.Vector3[] _ptGrp3he;

        private static Coord[] CreateGroup(Random random)
        {
            return Enumerable.Repeat(0, POINT_COUNT).Select(_ => (Coord)(
            random.NextDouble(),
            random.NextDouble(),
            random.NextDouble()
            )).ToArray();
        }

        [GlobalSetup]
        public void Setup()
        {
            var random = new Random(42);

            _ptGrp1 = CreateGroup(random);
            _ptGrp2 = CreateGroup(random);
            _ptGrp3 = CreateGroup(random);

            _ptGrp1gs = _ptGrp1.Select(pt => new GShark.Geometry.Point3(pt.X, pt.Y, pt.Z)).ToArray();
            _ptGrp2gs = _ptGrp2.Select(pt => new GShark.Geometry.Point3(pt.X, pt.Y, pt.Z)).ToArray();
            _ptGrp3gs = _ptGrp3.Select(pt => new GShark.Geometry.Point3(pt.X, pt.Y, pt.Z)).ToArray();

            _ptGrp1he = _ptGrp1.Select(pt => new Elements.Geometry.Vector3(pt.X, pt.Y, pt.Z)).ToArray();
            _ptGrp2he = _ptGrp2.Select(pt => new Elements.Geometry.Vector3(pt.X, pt.Y, pt.Z)).ToArray();
            _ptGrp3he = _ptGrp3.Select(pt => new Elements.Geometry.Vector3(pt.X, pt.Y, pt.Z)).ToArray();
        }
        [Benchmark(Baseline =true)]
        public bool PancakeGeometry()
        {
            var result = false;
            for (var i = 0; i < POINT_COUNT; i++)
            {
                var pt1 = _ptGrp1[i];
                var pt2 = _ptGrp2[i];
                var pt3 = _ptGrp3[i];

                var sameLine = Coord.IsColinear(pt1, pt2, pt3);
                result ^= sameLine;
            }
            return result;
        }
        [Benchmark]
        public bool GShark()
        {
            var result = false;
            for (var i = 0; i < POINT_COUNT; i++)
            {
                var pt1 = _ptGrp1gs[i];
                var pt2 = _ptGrp2gs[i];
                var pt3 = _ptGrp3gs[i];

                var sameLine = Trigonometry.ArePointsCollinear(pt1, pt2, pt3, 1e-9);
                result ^= sameLine;
            }
            return result;
        }
        [Benchmark]
        public bool HyparElement()
        {
            var result = false;
            for (var i = 0; i < POINT_COUNT; i++)
            {
                var pt1 = _ptGrp1he[i];
                var pt2 = _ptGrp2he[i];
                var pt3 = _ptGrp3he[i];

                var sameLine = Elements.Geometry.Vector3.AreCollinearByDistance(pt1, pt2, pt3);
                result ^= sameLine;
            }
            return result;
        }
    }
}
