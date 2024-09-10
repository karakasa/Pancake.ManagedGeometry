using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using CrossLibBenchmark.Utils;
using Pancake.ManagedGeometry;
using Pancake.ManagedGeometry.Algo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossLibBenchmark.Algo
{
    [SimpleJob(RuntimeMoniker.Net60)]
    [SimpleJob(RuntimeMoniker.Net80)]
    [MemoryDiagnoser]
    public class PtInPly
    {
        private Coord[] plyCoords;
        private Elements.Geometry.Polygon plyElements;
        private GShark.Geometry.Polygon plyGshark;
        private SharpMath.Geometry.Polygon plySharpMath;

        private Coord[] samplePts;
        private Elements.Geometry.Vector3[] samplePtsElements;
        private GShark.Geometry.Point3[] samplePtsGShark;
        private SharpMath.Geometry.Point2D[] samplePtsSharpMath;

        private Coord2d[] plyPancake;
        private Coord2d[] samplePtsPancake;

        [GlobalSetup]
        public void Setup()
        {
            const int SIDE = 5;
            const double RADIUS = 1.0;
            const int PT_COUNT = 10000;

            plyCoords = Enumerable.Range(0, SIDE)
                .Select(i => (360.0 / SIDE) / 180.0 * Math.PI * i)
                .Select(ang => (Coord)(RADIUS * Math.Cos(ang), RADIUS * Math.Sin(ang), 0))
                .ToArray();

            plyElements = new Elements.Geometry.Polygon(plyCoords.Select(CoordConversion.ToElement).ToArray());
            plyGshark = new GShark.Geometry.Polygon(plyCoords.Select(CoordConversion.ToGShark).ToArray());

            var rand = new Random(42);
            samplePts = new Coord[PT_COUNT];
            for (var i = 0; i < PT_COUNT; i++)
                samplePts[i] = (rand.NextDouble(), rand.NextDouble(), 0);

            samplePtsElements = samplePts.Select(CoordConversion.ToElement).ToArray();
            samplePtsGShark = samplePts.Select(CoordConversion.ToGShark).ToArray();

            plyPancake = plyCoords.Select(r => (Coord2d)(r.X, r.Y)).ToArray();
            samplePtsPancake = samplePts.Select(r => (Coord2d)(r.X, r.Y)).ToArray();

            samplePtsSharpMath = samplePts.Select(r => new SharpMath.Geometry.Point2D(r.X, r.Y)).ToArray();
            plySharpMath = new SharpMath.Geometry.Polygon(
                plyCoords.Select(r => new SharpMath.Geometry.Point2D(r.X, r.Y)).ToArray()
                );
        }
        [Benchmark]
        public bool SharpMath()
        {
            var sum = false;
            foreach (var it in samplePtsSharpMath)
            {
                var result = plySharpMath.ContainsPoint(it);
                sum ^= result;
            }

            return sum;
        }
        [Benchmark]
        public bool Pancake()
        {
            var sum = false;
            foreach (var it in samplePtsPancake)
            {
                var result = PointInsidePolygon.Contains(plyPancake, it) != PointInsidePolygon.PointContainment.Outside;
                sum ^= result;
            }

            return sum;
        }
        [Benchmark]
        public bool Elements()
        {
            var sum = false;
            foreach (var it in samplePtsElements)
            {
                var result = plyElements.Contains(it, out _);
                sum ^= result;
            }

            return sum;
        }
        //[Benchmark]
        public bool GShark()
        {
            // GShark doesn't have this feature

            var sum = false;
            foreach (var it in samplePtsGShark)
            {
                //var result = PointInsidePolygon.Contains(plyPancake, it) != PointInsidePolygon.PointContainment.Outside;
                //sum ^= result;
            }

            return sum;
        }
    }
}
