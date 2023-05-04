using Pancake.ManagedGeometry.Utility;
using System.Diagnostics;

namespace Pancake.ManagedGeometry
{
    [DebuggerDisplay("{From} -> {To}")]
    public struct Line3d
    {
        public Coord From;
        public Coord Direction;
        public Coord To => From + Direction;

        public double Length => (To - From).Length;
        public Line3d(Coord start, Coord end)
        {
            From = start;
            Direction = end - start;
        }

        public Line3d(Coord start, Coord vec, double Length)
        {
            From = start;
            Direction = vec / vec.Length * Length;
        }

        public Line2d GetXYProjection()
        {
            return new Line2d(new Coord2d(From.X, From.Y), new Coord2d(To.X, To.Y));
        }
        public static implicit operator Line3d((Coord, Coord) d) => new(d.Item1, d.Item2);
        /// <summary>
        /// 获得外部点与直线的最近点
        /// </summary>
        /// <param name="outsidePt"></param>
        /// <returns>参数</returns>
        public double NearestPoint(Coord outsidePt)
        {
            var l = Direction.SquareLength;

            if (l.CloseToZero()) return 0;

            var t = MathUtils.Clip((outsidePt - From) * Direction / l, 0, 1);

            return t;
        }

        public Coord PointAt(double t) => From + t * Direction;
    }
}
