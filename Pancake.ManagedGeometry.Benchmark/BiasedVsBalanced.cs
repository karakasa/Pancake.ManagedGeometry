using BenchmarkDotNet.Attributes;
using Pancake.ManagedGeometry.HigherLevel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pancake.ManagedGeometry.Benchmark
{
    [MemoryDiagnoser]
    public class BiasedVsBalanced
    {
        private class DummyCurve : Curve
        {
            public override Coord Start => throw new NotImplementedException();

            public override Coord End => throw new NotImplementedException();

            public override BoundingBox2d GetBoundingBox2d() => _b2d;

            private readonly BoundingBox2d _b2d;
            public DummyCurve(BoundingBox2d bbox)
            {
                _b2d = bbox;
            }
        }

        private const int COUNT = 1000;

        private IEnumerable<Region2d> CreateRegions()
        {
            for (var i = 0; i < COUNT; i++)
            {
                var bbox = new BoundingBox2d((i*2,i*2), (i*2+1,i*2+1));
                yield return Region2d.CreateFromOneCurve(new DummyCurve(bbox));
            }
        }

        private RegionGroup2d grpA = new();
        private RegionGroup2d grpB = new();

        [GlobalSetup]
        public void Setup()
        {
            var regions = CreateRegions().ToArray();

            const int FirstPart = 100;

            grpA.Regions.AddRange(regions.Take(FirstPart));
            grpB.Regions.AddRange(regions.Skip(FirstPart));
        }


        [Benchmark(Baseline =true)]
        public bool Balanced()
        {
            return RegionGroup2dBoolean.IsXYOverlapped(grpA, grpB);
        }
        [Benchmark]
        public bool FewerAsExpected()
        {
            return RegionGroup2dBoolean.IsXYOverlappedBiased(grpB, grpA);
        }
        [Benchmark]
        public bool FewerNotAsExpected()
        {
            return RegionGroup2dBoolean.IsXYOverlappedBiased(grpA, grpB);
        }
    }
}
