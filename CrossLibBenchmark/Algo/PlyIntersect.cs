using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using CrossLibBenchmark.Utils;
using Pancake.ManagedGeometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossLibBenchmark.Algo
{
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net60)]
    [SimpleJob(RuntimeMoniker.Net80)]
    public class PlyIntersect
    {
        private Elements.Geometry.Polygon plyElements1;
        private GShark.Geometry.Polygon plyGshark1;
        private Polygon plyPancake1;

        private Elements.Geometry.Polygon plyElements2;
        private GShark.Geometry.Polygon plyGshark2;
        private Polygon plyPancake2;

        public Coord[] GeneratePolygon(int SIDE, double RADIUS, int BASEANGLE = 0)
        {
            return
                Enumerable.Range(0, SIDE)
                .Select(i => (360.0 / SIDE * i + BASEANGLE) / 180.0 * Math.PI)
                .Select(ang => (Coord)(RADIUS * Math.Cos(ang), RADIUS * Math.Sin(ang), 0))
                .ToArray();
        }

        [GlobalSetup]
        public void Setup()
        {
            const int PT_COUNT = 10000;
            Coord[] plyCoords;

            plyCoords = GeneratePolygon(5, 1.0);

            plyElements1 = new Elements.Geometry.Polygon(plyCoords.Select(CoordConversion.ToElement).ToArray());
            plyGshark1 = new GShark.Geometry.Polygon(plyCoords.Select(CoordConversion.ToGShark).ToArray());
            plyPancake1 = new Polygon(plyCoords.Select(r => (Coord2d)(r.X, r.Y)).ToArray());

            plyCoords = GeneratePolygon(6, 1.0, 45);

            plyElements2 = new Elements.Geometry.Polygon(plyCoords.Select(CoordConversion.ToElement).ToArray());
            plyGshark2 = new GShark.Geometry.Polygon(plyCoords.Select(CoordConversion.ToGShark).ToArray());
            plyPancake2 = new Polygon(plyCoords.Select(r => (Coord2d)(r.X, r.Y)).ToArray());
        }
        [Benchmark]
        public bool Pancake()
        {
            return plyPancake1.IntersectWith(plyPancake2);
        }
        [Benchmark]
        public bool Elements()
        {
            return plyElements1.Intersects(plyElements2);
        }
        //[Benchmark]
        public bool GShark2()
        {
            return GShark.Intersection.Intersect.CurveCurve(plyGshark1, plyGshark2).Count > 0;
        }
    }
}
