using Pancake.ManagedGeometry.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pancake.ManagedGeometry.Algo
{
    public class LineInsidePolygon
    {
        private List<double> _listOfParameters = new(capacity: 8);

        public bool IsOutside(Polygon ply, Line2d line)
        {
            return JudgeSide(ply, line, PointInsidePolygon.PointContainment.Inside);
        }

        private bool JudgeSide(Polygon ply, Line2d line, PointInsidePolygon.PointContainment disallowed)
        {
            var listOfParameters = _listOfParameters;

            listOfParameters.Clear();

            var cnt = ply.VertexCount;

            var lineFrom = line.From;
            var lineTo = line.To;

            var relationAtLineStart = PointInsidePolygon.Contains(ply, lineFrom);
            var relationAtLineEnd = PointInsidePolygon.Contains(ply, lineTo);

            if (relationAtLineStart == disallowed
                || relationAtLineEnd == disallowed)
                return false;

            // 先检查就是边的特殊情况
            for (var i = 0; i < cnt; i++)
            {
                var plyLine = ply.EdgeAt(i);
                if (plyLine.AlmostEqualTo(line)) return true;
            }

            var middlePt = (lineFrom + lineTo) / 2;
            if (PointInsidePolygon.Contains(ply, middlePt) == disallowed)
                return false;

            var lastIntersectionParam = default(double?);

            // 枚举边和线段的相交情况
            for (var i = 0; i < cnt; i++)
            {
                var plyLine = ply.EdgeAt(i);
                var relation = plyLine.IntersectWith(line, out var plyEdgeParam, out var param);

                if (relation != LineRelation.Intersected) continue;

                if (!(plyEdgeParam - Line2d.ParamAtStart).CloseToZero()
                    && !(plyEdgeParam - Line2d.ParamAtEnd).CloseToZero()
                    && !(param - Line2d.ParamAtStart).CloseToZero()
                    && !(param - Line2d.ParamAtEnd).CloseToZero())
                    return false;

                if (lastIntersectionParam.HasValue)
                {
                    if (!(lastIntersectionParam.Value - param).CloseToZero())
                    {
                        // 如果有第二个不一样的交点，则把交点记录下来
                        if (listOfParameters.Count == 0)
                        {
                            listOfParameters.Add(lastIntersectionParam.Value);
                        }
                        listOfParameters.Add(param);
                    }
                }
                else
                {
                    lastIntersectionParam = param;
                }
            }

            // 至多只有一个相异的交点则在多边形外
            if (listOfParameters.Count == 0)
            {
                return true;
            }

            // 对由交点切分的若干条线段的中点，判断其是否在多边形内
            listOfParameters.Sort();
            var listCnt = listOfParameters.Count - 1;

            for (var i = 0; i < listCnt; i++)
            {
                var pt = line.PointAt((listOfParameters[i] + listOfParameters[i + 1]) / 2);
                var containment = PointInsidePolygon.Contains(ply.InternalVerticeArray, pt);

                if (containment == disallowed) return false;
            }

            return true;
        }

        public bool IsInside(Polygon ply, Line2d line)
        {
            return JudgeSide(ply, line, PointInsidePolygon.PointContainment.Outside);
        }

        /// <summary>
        /// 线段是否在凸多边形内
        /// </summary>
        /// <param name="line"></param>
        /// <param name="ply"></param>
        /// <returns></returns>
        public static bool ContainsConvex(Polygon ply, Line2d line)
        {
            var lineFrom = line.From;
            var lineTo = line.To;

            var relationAtLineStart = PointInsidePolygon.Contains(ply.InternalVerticeArray, lineFrom);
            var relationAtLineEnd = PointInsidePolygon.Contains(ply.InternalVerticeArray, lineTo);

            // 如果线段有一点在多边形外部，那么线段肯定不全在多边形里
            if (relationAtLineStart == PointInsidePolygon.PointContainment.Outside
                || relationAtLineEnd == PointInsidePolygon.PointContainment.Outside)
                return false;

            return true;
        }
    }
}
