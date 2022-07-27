using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Pancake.ManagedGeometry.Algo
{
    // TODO: 支持洞

    public class PathOfTravel
    {
        private LineInsidePolygon _insideSolver = new();
        [DebuggerDisplay("{Distance}, {InBetweenNode}")]
        private struct FloydDistance
        {
            public double Distance;
            public int InBetweenNode;

            public static readonly FloydDistance SamePoint = new()
            {
                Distance = 0,
                InBetweenNode = -1
            };

            public static readonly FloydDistance NotConnected = new()
            {
                Distance = double.PositiveInfinity,
                InBetweenNode = -1
            };

            public FloydDistance(double distance)
            {
                Distance = distance;
                InBetweenNode = -1;
            }

            public FloydDistance(double distance, int inbetween)
            {
                Distance = distance;
                InBetweenNode = inbetween;
            }
        }
        private readonly Polygon _exterior = null;
        private readonly Polygon[] _holes = null;

        private Coord2d[] _mergedVertices = null;

        private FloydDistance[,] _vertexDistance;
        public PathOfTravel(Polygon shape)
        {
            _exterior = shape;
            _holes = null;

            CalculateVertexMap();
        }

        public PathOfTravel(Polygon shape, IEnumerable<Polygon> holes)
        {
            _exterior = shape;
            _holes = holes.ToArray();

            CalculateVertexMap();
        }

        private void MergeVertices()
        {
            if(_holes == null)
            {
                _mergedVertices = _exterior.InternalVerticeArray;
                return;
            }

            var sum = _exterior.InternalVerticeArray.Length;
            for (var i = 0; i < _holes.Length; i++)
                sum += _holes[i].InternalVerticeArray.Length;

            _mergedVertices = new Coord2d[sum];

            var index = _exterior.InternalVerticeArray.Length;
            Array.Copy(_exterior.InternalVerticeArray, 0, _mergedVertices, 0, index);

            for (var i = 0; i < _holes.Length; i++)
            {
                var len = _holes[i].InternalVerticeArray.Length;
                Array.Copy(_holes[i].InternalVerticeArray, 0, _mergedVertices, index, len);
                index += len;
            }
        }

        private void CalculateVertexMap()
        {
            MergeVertices();

            var _v = _mergedVertices;

            InitFloyd(_v);
            CalculateDirectPath(_v);
            CalculateFloyd(_v);
        }

        private bool IsInside(Line2d line)
        {
            if (!_insideSolver.IsInside(_exterior, line)) return false;
            if (_holes != null)
            {
                foreach(var it in _holes)
                {
                    if (!_insideSolver.IsOutside(it, line)) return false;
                }
            }
            return true;
        }

        private void InitFloyd(Coord2d[] v)
        {
            _vertexDistance = new FloydDistance[v.Length, v.Length];
            for (var i = 0; i < v.Length; i++)
                for (var j = 0; j < v.Length; j++)
                {
                    if (i == j)
                    {
                        _vertexDistance[i, j] = FloydDistance.SamePoint;
                    }
                    else
                    {
                        _vertexDistance[i, j] = FloydDistance.NotConnected;
                    }
                }
        }

        private void CalculateDirectPath(Coord2d[] v)
        {
            for (var i = 0; i < v.Length; i++)
            {
                for (var j = i + 1; j < v.Length; j++)
                {
                    var line = new Line2d(v[i], v[j]);
                    if (IsInside(line))
                    {
                        _vertexDistance[i, j] = _vertexDistance[j, i] = new(line.Length);
                    }
                }
            }
        }

        private void CalculateFloyd(Coord2d[] v)
        {
            for (var k = 0; k < v.Length; k++)
                for (var i = 0; i < v.Length; i++)
                    for (var j = 0; j < v.Length; j++)
                    {
                        var dist = _vertexDistance[i, k].Distance + _vertexDistance[k, j].Distance;
                        if (dist < _vertexDistance[i, j].Distance)
                        {
                            _vertexDistance[i, j] = new(dist, k);
                        }
                    }
        }

        public double MinimalDistance { get; private set; } = double.NaN;

        public bool HasResult => !double.IsNaN(MinimalDistance);

        private int _lastFromVertex = -1;
        private int _lastToVertex = -1;

        private Coord2d _from;
        private Coord2d _to;

        public IEnumerable<Coord2d> GetPath()
        {
            if (!HasResult)
                throw new InvalidOperationException("Result hasn't been generated yet.");

            yield return _from;

            foreach (var it in GetInBetweenVertexIndexes())
                yield return _mergedVertices[it];

            yield return _to;
        }

        private IEnumerable<int> GetInBetweenVertexIndexes()
        {
            if (!HasResult)
                throw new InvalidOperationException();

            if (_lastFromVertex == -1)
                yield break;

            yield return _lastFromVertex;

            foreach (var it in GetShortestPathPair(_lastFromVertex, _lastToVertex))
                yield return it;

            yield return _lastToVertex;
        }

        private IEnumerable<int> GetShortestPathPair(int from, int to)
        {
            if (from == to)
            {
                // yield return from;
                yield break;
            }

            var calc = _vertexDistance[from, to];
            if (calc.InBetweenNode == -1)
            {
                // 直接连
                yield break;
            }
            else
            {
                // 中间还有点
                foreach (var it in GetShortestPathPair(from, calc.InBetweenNode))
                    yield return it;

                yield return calc.InBetweenNode;

                foreach (var it in GetShortestPathPair(calc.InBetweenNode, to))
                    yield return it;
            }
        }

        public bool IsValidPoint(Coord2d pt)
        {
            if (PointInsidePolygon.Contains(_exterior, pt) == PointInsidePolygon.PointContainment.Outside)
                return false;

            if (_holes != null)
            {
                foreach (var it in _holes)
                    if (PointInsidePolygon.Contains(it, pt) == PointInsidePolygon.PointContainment.Inside)
                        return false;
            }

            return true;
        }
        public bool Solve(Coord2d from, Coord2d to)
        {
            _from = from;
            _to = to;

            _lastFromVertex = _lastToVertex = -1;
            MinimalDistance = double.NaN;

            if (!IsValidPoint(from) || !IsValidPoint(to))
                return false;

            var directLine = new Line2d(from, to);
            if (IsInside(directLine))
            {
                MinimalDistance = directLine.Length;
                // 直接相连就行
                return true;
            }

            var v = _mergedVertices;
            var minDist = double.MaxValue;
            var lastFromVertex = -1;
            var lastToVertex = -1;

            for (var fromVertex = 0; fromVertex < v.Length; fromVertex++)
            {
                var fromLine = new Line2d(from, v[fromVertex]);
                var fromLineLength = fromLine.Length;

                if (fromLineLength > minDist)
                    continue;

                if (!IsInside(fromLine))
                    continue;

                for (var toVertex = 0; toVertex < v.Length; toVertex++)
                {
                    var toLine = new Line2d(v[toVertex], to);
                    var toLineLength = toLine.Length;

                    var dist = _vertexDistance[fromVertex, toVertex].Distance + fromLineLength + toLineLength;

                    if (dist > minDist)
                        continue;

                    if (!IsInside(toLine))
                        continue;

                    minDist = dist;
                    lastFromVertex = fromVertex;
                    lastToVertex = toVertex;
                }
            }

            if (lastFromVertex == -1 || lastToVertex == -1)
                return false;

            _lastFromVertex = lastFromVertex;
            _lastToVertex = lastToVertex;
            MinimalDistance = minDist;

            return true;
        }
    }
}
