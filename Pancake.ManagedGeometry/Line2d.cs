using Pancake.ManagedGeometry.Utility;
using System;
using System.Diagnostics;

namespace Pancake.ManagedGeometry
{
    public enum LineRelation : int
    {
        Collinear,
        Parallel,
        Intersected,
        NotIntersected
    }
    [DebuggerDisplay("{From} -> {To}")]
    public partial struct Line2d
    {
        public Coord2d From;
        public Coord2d Direction;

        public Coord2d To
        {
            get => From + Direction;
            set => Direction = value - From;
        }

        public double Length
        {
            get => Direction.Length;
            set => Direction = Direction / Direction.Length * value;
        }

        public void Reverse() => Direction = -Direction;

        public void Translate(Coord2d vec) => From += vec;

        public void ReverseTranslate(Coord2d vec) => From -= vec;

        public Coord2d PointAt(double k) => From + k * Direction;

        public Line2d(Coord2d start, Coord2d end)
        {
            From = start;
            Direction = end - start;
        }
        public bool IsOnLine(Coord2d pt)
        {
            return NearestPointInfinite(pt).BetweenRange(0, 1);
        }
        public void GetEquation(out double a, out double b, out double c)
        {
            a = Direction.Y;
            b = -Direction.X;
            c = From.Y * Direction.X - From.X * Direction.Y;
        }
        
        public double NearestPointInfinite(Coord2d outsidePt)
        {
            var l = Direction.SquareLength;
            if (l.CloseToZero()) return 0;

            return (outsidePt - From) * Direction / l;
        }
        /// <summary>
        /// 获得外部点与直线的最近点
        /// </summary>
        /// <param name="outsidePt"></param>
        /// <returns>参数</returns>
        public double NearestPoint(Coord2d outsidePt)
        {
            var l = Direction.SquareLength;

            if (l.CloseToZero()) return 0;

            var t = MathUtils.Clip((outsidePt - From) * Direction / l, 0, 1);

            return t;
        }
        public double DistanceToPoint(Coord2d outsidePt)
        {
            return (PointAt(NearestPoint(outsidePt)) - outsidePt).Length;
        }

        public double DistanceToPoint(Coord2d outsidePt, out Coord2d closestPt)
        {
            closestPt = PointAt(NearestPoint(outsidePt));
            return (closestPt - outsidePt).Length;
        }

        public double SquareDistanceToPoint(Coord2d outsidePt)
        {
            return (PointAt(NearestPoint(outsidePt)) - outsidePt).SquareLength;
        }
        public double DistanceToPointInfinite(Coord2d ptr)
        {
            GetEquation(out var a, out var b, out var c);
            return Math.Abs(a * ptr.X + b * ptr.Y + c) / Math.Sqrt(a * a + b * b);
        }

        public double DistanceToOriginInfinite()
        {
            return Math.Abs(From.Y * Direction.X - From.X * Direction.Y) / Math.Sqrt(Direction.Y * Direction.Y + Direction.X * Direction.X);
        }

        public bool PassOriginInfinite(double tolerance)
        {
            return Math.Abs(From.Y * Direction.X - From.X * Direction.Y) < tolerance;
        }

        public bool ContainsOriginPoint(out double k)
        {
            if (Math.Abs(Direction.X) > MathUtils.ZeroTolerance)
            {
                k = -From.X / Direction.X;
                return -MathUtils.ZeroTolerance < k && k < 1 + MathUtils.ZeroTolerance;
            }

            if (Math.Abs(Direction.Y) > MathUtils.ZeroTolerance)
            {
                k = -From.Y / Direction.Y;
                return -MathUtils.ZeroTolerance < k && k < 1 + MathUtils.ZeroTolerance;
            }

            k = 0.0;
            return From.SquareLength < MathUtils.ZeroTolerance;
        }

        public double NearestPtToAnotherLine(Line2d another, out Coord2d thisPt, out Coord2d anotherPt)
        {
            var relation = IntersectWith(another, out var t, out _);
            if (relation == LineRelation.Intersected)
            {
                thisPt = anotherPt = PointAt(t);
                return 0;
            }
            else if (relation == LineRelation.Collinear)
            {
                {
                    var pt = From;

                    if (another.IsOnLine(pt))
                    {
                        thisPt = anotherPt = pt;
                        return 0;
                    }

                    pt = To;

                    if (another.IsOnLine(pt))
                    {
                        thisPt = anotherPt = pt;
                        return 0;
                    }
                }

                {
                    var pt = another.From;

                    if (IsOnLine(pt))
                    {
                        thisPt = anotherPt = pt;
                        return 0;
                    }

                    pt = another.To;

                    if (IsOnLine(pt))
                    {
                        thisPt = anotherPt = pt;
                        return 0;
                    }
                }
            }
            else if (relation == LineRelation.Parallel)
            {
                var dist = another.DistanceToPointInfinite(From);

                {
                    var pt = From;
                    var u = another.NearestPointInfinite(pt);

                    if (u.BetweenRange(0, 1))
                    {
                        thisPt = pt;
                        anotherPt = another.PointAt(u);
                        return dist;
                    }

                    pt = To;
                    u = another.NearestPointInfinite(pt);

                    if (u.BetweenRange(0, 1))
                    {
                        thisPt = pt;
                        anotherPt = another.PointAt(u);
                        return dist;
                    }
                }

                {
                    var pt = another.From;
                    var u = NearestPointInfinite(pt);

                    if (u.BetweenRange(0, 1))
                    {
                        thisPt = PointAt(u);
                        anotherPt = pt;
                        return dist;
                    }

                    pt = another.To;
                    u = NearestPointInfinite(pt);

                    if (u.BetweenRange(0, 1))
                    {
                        thisPt = PointAt(u);
                        anotherPt = pt;
                        return dist;
                    }
                }
            }

            thisPt = anotherPt = default;
            double minDist = double.MaxValue;

            {
                var pt = From;
                var dist = another.DistanceToPoint(pt, out var lastPt);
                if (dist < minDist)
                {
                    minDist = dist;
                    anotherPt = lastPt;
                    thisPt = pt;
                }

                pt = To;
                dist = another.DistanceToPoint(pt, out lastPt);
                if (dist < minDist)
                {
                    minDist = dist;
                    anotherPt = lastPt;
                    thisPt = pt;
                }
            }

            {
                var pt = another.From;
                var dist = DistanceToPoint(pt, out var lastPt);
                if (dist < minDist)
                {
                    minDist = dist;
                    anotherPt = pt;
                    thisPt = lastPt;
                }

                pt = another.To;
                dist = DistanceToPoint(pt, out lastPt);
                if (dist < minDist)
                {
                    minDist = dist;
                    anotherPt = pt;
                    thisPt = lastPt;
                }
            }

            return minDist;
        }
        public LineRelation IntersectWith(Line2d another,
            out double paramOnThisLine, out double paramOnAnotherLine)
        {
            // https://stackoverflow.com/questions/563198/how-do-you-detect-where-two-line-segments-intersect
            var p1 = Coord2d.CrossProductLength(this.Direction, another.Direction);
            var p2 = another.From - this.From;

            if (p1.CloseToZero())
            {
                var judge = Coord2d.CrossProductLength(p2, this.Direction);
                paramOnThisLine = paramOnAnotherLine = 0;
                return judge.CloseToZero() ? LineRelation.Collinear : LineRelation.Parallel;
            }

            var t = Coord2d.CrossProductLength(p2, another.Direction) / p1;
            var u = Coord2d.CrossProductLength(p2, this.Direction) / p1;

            paramOnThisLine = t;
            paramOnAnotherLine = u;

            return (t.BetweenRange(0, 1) && u.BetweenRange(0, 1)) ? LineRelation.Intersected : LineRelation.NotIntersected;
        }

        public LineRelation DoesIntersectWith(Line2d another)
        {
            var p1 = Coord2d.CrossProductLength(this.Direction, another.Direction);
            var p2 = another.From - this.From;

            if (p1.CloseToZero())
            {
                var judge = Coord2d.CrossProductLength(p2, this.Direction);
                return judge.CloseToZero() ? LineRelation.Collinear : LineRelation.Parallel;
            }

            var t = Coord2d.CrossProductLength(p2, another.Direction) / p1;
            var u = Coord2d.CrossProductLength(p2, this.Direction) / p1;

            return t.BetweenRange(0, 1) && u.BetweenRange(0, 1) ? LineRelation.Intersected : LineRelation.NotIntersected;
        }
    }
}
