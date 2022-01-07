using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Pancake.ManagedGeometry.Algo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pancake.ManagedGeometry.Benchmark
{
    [SimpleJob(RuntimeMoniker.Net472, baseline: true)]
    [SimpleJob(RuntimeMoniker.HostProcess)]
    [MemoryDiagnoser]
    public class ShortestPathOfTravel
    {
        PathOfTravel solver = null;

        const int N = 10000;
        private Coord2d[] pts;

        [GlobalSetup]
        public void Setup()
        {
            var ply = new Polygon(new Coord2d[] {
            (0,0),
            (2,0),
            (2,1),
            (1,1),
            (1,2),
            (0,2)
            });

            var hole1 = new Polygon(new Coord2d[] {
            (1.25, 0.25),
            (1.75, 0.25),
            (1.75, 0.75),
            (1.25, 0.75)
            });

            var hole2 = new Polygon(new Coord2d[] {
            (0.25, 1.25),
            (0.75, 1.25),
            (0.75, 1.75),
            (0.25, 1.75)
            });

            solver = new PathOfTravel(ply, new[] { hole1, hole2 });

            var rand = new Random(42);
            pts = Enumerable.Range(0, N)
                .Select(_ => (Coord2d)(rand.NextDouble() * 2, rand.NextDouble() * 2))
                .Where(solver.IsValidPoint)
                .ToArray();
        }

        [Benchmark]
        public double Test()
        {
            var sum = 0.0;

            for (var i = 0; i < pts.Length - 1; i += 2)
            {
                solver.Solve(pts[i], pts[i+1]);

                sum += solver.MinimalDistance;
            }

            return sum;
        }
    }
}
