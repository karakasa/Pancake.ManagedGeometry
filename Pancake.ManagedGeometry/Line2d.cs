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
        NotIntersected,
        Undetermined
    }
    /// <summary>
    /// 代表一个二维线段。线段是没有方向的。
    /// </summary>
    [DebuggerDisplay("{From} -> {To}")]
    public partial struct Line2d
    {
        public const double ParamAtStart = 0;
        public const double ParamAtEnd = 1;

        public Coord2d From;
        public Coord2d Direction;

        public Coord2d To
        { readonly get => From + Direction;
            set => Direction = value - From;
        }

        public double Length
        { readonly get => Direction.Length;
            set => Direction = Direction / Direction.Length * value;
        }

        public void Reverse() => Direction = -Direction;

        public void Translate(Coord2d vec) => From += vec;

        public void ReverseTranslate(Coord2d vec) => From -= vec;

        public readonly Coord2d PointAt(double k) => From + k * Direction;

        public Line2d(Coord2d start, Coord2d end)
        {
            From = start;
            Direction = end - start;
        }
        public readonly bool IsOnLine(Coord2d pt)
        {
            return NearestPointInfinite(pt).BetweenRange(0, 1);
        }
        public readonly bool IsOnLine(Coord2d pt, double tolerance)
        {
            return NearestPointInfinite(pt).BetweenRange(0, 1, tolerance);
        }
        public readonly void GetEquation(out double a, out double b, out double c)
        {
            a = Direction.Y;
            b = -Direction.X;
            c = From.Y * Direction.X - From.X * Direction.Y;
        }
        
        public readonly double NearestPointInfinite(Coord2d outsidePt)
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
        public readonly double NearestPoint(Coord2d outsidePt)
        {
            var l = Direction.SquareLength;

            if (l.CloseToZero()) return 0;

            var t = MathUtils.Clip((outsidePt - From) * Direction / l, 0, 1);

            return t;
        }
        public readonly double DistanceToPoint(Coord2d outsidePt)
        {
            return (PointAt(NearestPoint(outsidePt)) - outsidePt).Length;
        }

        public readonly double DistanceToPoint(Coord2d outsidePt, out Coord2d closestPt)
        {
            closestPt = PointAt(NearestPoint(outsidePt));
            return (closestPt - outsidePt).Length;
        }

        public readonly double SquareDistanceToPoint(Coord2d outsidePt)
        {
            return (PointAt(NearestPoint(outsidePt)) - outsidePt).SquareLength;
        }
        public readonly double DistanceToPointInfinite(Coord2d ptr)
        {
            GetEquation(out var a, out var b, out var c);
            return Math.Abs(a * ptr.X + b * ptr.Y + c) / Math.Sqrt(a * a + b * b);
        }

        public readonly double DistanceToOriginInfinite()
        {
            return Math.Abs(From.Y * Direction.X - From.X * Direction.Y) / Math.Sqrt(Direction.Y * Direction.Y + Direction.X * Direction.X);
        }

        public readonly bool PassOriginInfinite(double tolerance)
        {
            return Math.Abs(From.Y * Direction.X - From.X * Direction.Y) < tolerance;
        }

        public readonly bool ContainsOriginPoint(out double k)
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

        public readonly double NearestPtToAnotherLine(Line2d another, out Coord2d thisPt, out Coord2d anotherPt)
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
        public readonly LineRelation IntersectWith(Line2d another,
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

        public readonly LineRelation DoesIntersectWith(Line2d another)
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

        public readonly LineRelation IsParallelOrColinear(Line2d another)
        {
            var p1 = Coord2d.CrossProductLength(this.Direction, another.Direction);

            if (p1.CloseToZero())
            {
                var p2 = another.From - this.From;
                var judge = Coord2d.CrossProductLength(p2, this.Direction);
                return judge.CloseToZero() ? LineRelation.Collinear : LineRelation.Parallel;
            }

            return LineRelation.Undetermined;
        }
        public readonly LineRelation IsParallelOrColinear(Line2d another, double tolerance)
        {
            var p1 = Coord2d.CrossProductLength(this.Direction.ChopZero(tolerance), another.Direction.ChopZero(tolerance));

            if (p1.CloseToZero(tolerance))
            {
                var p2 = another.From - this.From;
                var judge = Coord2d.CrossProductLength(p2.ChopZero(tolerance), this.Direction.ChopZero(tolerance));
                return judge.CloseToZero(tolerance) ? LineRelation.Collinear : LineRelation.Parallel;
            }

            return LineRelation.Undetermined;
        }

        public readonly bool AlmostEqualTo(Line2d another)
        {
            var end = To;
            var anotherEnd = another.To;

            if (From.AlmostEqualTo(another.From) && end.AlmostEqualTo(anotherEnd)) return true;
            if (From.AlmostEqualTo(anotherEnd) && end.AlmostEqualTo(another.From)) return true;

            return false;
        }
        public readonly bool AlmostEqualTo(Line2d another, double tolerance)
        {
            var end = To;
            var anotherEnd = another.To;

            if (From.AlmostEqualTo(another.From, tolerance) && end.AlmostEqualTo(anotherEnd, tolerance)) return true;
            if (From.AlmostEqualTo(anotherEnd, tolerance) && end.AlmostEqualTo(another.From, tolerance)) return true;

            return false;
        }

        public static implicit operator Line2d((Coord2d, Coord2d) d) => new(d.Item1, d.Item2);
        public readonly bool IsValid() =>
            From.IsValid && Direction.IsValid && !Direction.SquareLength.CloseToZero();

        public readonly bool IsValid(double tolerance) =>
            From.IsValid && Direction.IsValid && !Direction.SquareLength.CloseToZero(tolerance);
        public readonly bool DoesOverlapWith(Line2d another)
            => DoesOverlap(this, another, MathUtils.ZeroTolerance);
        public static bool DoesOverlap(Line2d a, Line2d b, double t)
        {
            if (!a.IsValid() || !b.IsValid()) return false;

            var relation = a.IsParallelOrColinear(b, t);
            if (relation != LineRelation.Collinear) return false;

            if (a.IsOnLine(b.From, t)
                || a.IsOnLine(b.To, t)
                || b.IsOnLine(a.From, t)
                || b.IsOnLine(a.To, t))
            {
                return true;
            }

            return false;
        }

        internal readonly bool CrossXAxis(double yValue, double minX, double maxX)
        {
            if (Direction.Y.CloseToZero())
            {
                if (!From.Y.CloseTo(yValue))
                {
                    return false;
                }

                var x1 = From.X;
                var x2 = x1 + Direction.X;

                return !((x1 < minX - MathUtils.ZeroTolerance && x2 < minX - MathUtils.ZeroTolerance)
                    || (x1 > maxX + MathUtils.ZeroTolerance && x2 > maxX + MathUtils.ZeroTolerance));
            }
            else
            {
                var xValue = From.X + Direction.X * (yValue - From.Y) / Direction.Y;
                return xValue.BetweenRange(minX, maxX);
            }
        }

        internal readonly bool CrossYAxis(double xValue, double minY, double maxY)
        {
            if (Direction.X.CloseToZero())
            {
                if (!From.X.CloseTo(xValue))
                {
                    return false;
                }

                var y1 = From.Y;
                var y2 = y1 + Direction.Y;

                return !((y1 < minY - MathUtils.ZeroTolerance && y2 < minY - MathUtils.ZeroTolerance)
                    || (y1 > maxY + MathUtils.ZeroTolerance && y2 > maxY + MathUtils.ZeroTolerance));
            }
            else
            {
                var yValue = From.Y + Direction.Y * (xValue - From.X) / Direction.X;
                return yValue.BetweenRange(minY, maxY);
            }
        }
    }
}
