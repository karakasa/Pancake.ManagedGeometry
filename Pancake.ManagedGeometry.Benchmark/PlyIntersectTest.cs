using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnostics.Windows.Configs;
using Pancake.ManagedGeometry.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pancake.ManagedGeometry.Benchmark
{
    [MemoryDiagnoser]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByParams)]
    public class PlyIntersectTest
    {
        private static Polygon ply1;
        private static BoundingBox2d[] bboxes = new BoundingBox2d[5];
        private static Polygon[] bboxPly = new Polygon[5];

        [Params(0, 1, 2, 3, 4)]
        public int Dataset { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            ply1 = PolygonFactory.RegularInscribed(5, 1);

            bboxes[0] = new BoundingBox2d((50, 50), (100, 100));
            bboxes[1] = new BoundingBox2d((0, 0), (10, 10));
            bboxes[2] = new BoundingBox2d((-10, 0.1), (10, 0.2));
            bboxes[3] = new BoundingBox2d((-50, -50), (50, 50));
            bboxes[4] = new BoundingBox2d((-0.1, -0.1), (0.1, 0.1));

            for (var i = 0; i < bboxes.Length; i++)
                bboxPly[i] = bboxes[i].ToPolygon();
        }

        [Benchmark]
        public bool TwoPly()
        {
            return ply1.IntersectWith(bboxPly[Dataset]);
        }
        [Benchmark]
        public bool BboxMethod()
        {
            return ply1.IntersectWith(bboxes[Dataset]);
        }
        [Benchmark]
        public bool BboxMethodWithFastCheck()
        {
            return ply1.IntersectWithFastCheck(bboxes[Dataset]);
        }
        // [Benchmark]
        public bool IPolygon()
        {
            return Polygon.IntersectsWith(ply1, bboxes[Dataset]);
        }
    }
}
