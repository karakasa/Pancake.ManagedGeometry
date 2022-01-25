using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Pancake.ManagedGeometry.HigherLevel;

namespace Pancake.ManagedGeometry.Algo
{
    [DebuggerDisplay("{Start} -> {End}")]
    public struct CurveRepresentation
    {
        public Coord Start;
        public Coord End;

        public CurveRepresentation(Coord s, Coord e)
        {
            Start = s;
            End = e;
        }
    }
    [DebuggerDisplay("{Index}, {IsReversed}")]
    public struct SortedCurveRepresentation
    {
        public int Index;
        public bool IsReversed;

        public SortedCurveRepresentation(int i, bool reversed)
        {
            Index = i;
            IsReversed = reversed;
        }
    }
    public static class CurveOperation
    {
        public static IList<SortedCurveRepresentation> MergeCurveEndRepresentations(CurveRepresentation[] curves)
        {
            var result = new List<SortedCurveRepresentation>(curves.Length);
            result.Add(new(0, false));
            var lastEnd = curves[0].End;

            var remainingCrvs = curves.Skip(1).Select((crv, i) => new { Curve = crv, Index = i + 1 }).ToList();
            var fndCurve = false;

            while (remainingCrvs.Count > 0)
            {
                fndCurve = false;
                var i = 0;

                for (; i < remainingCrvs.Count; i++)
                {
                    var curCrv = remainingCrvs[i];
                    if (curCrv.Curve.Start.IdenticalTo(lastEnd))
                    {
                        lastEnd = curCrv.Curve.End;
                        fndCurve = true;
                        result.Add(new(curCrv.Index, false));
                        break;
                    }
                    else if (curCrv.Curve.End.IdenticalTo(lastEnd))
                    {
                        lastEnd = curCrv.Curve.Start;
                        fndCurve = true;
                        result.Add(new(curCrv.Index, true));
                        break;
                    }
                }

                if (!fndCurve)
                {
                    throw new InvalidOperationException("Curves don't form a loop.");
                }
                else
                {
                    remainingCrvs.RemoveAt(i);
                }
            }

            return result;
        }

        public static IEnumerable<List<SortedCurveRepresentation>> MergeCurves(Curve[] curves)
        {
            return MergeCurves(curves.Select(s => new CurveRepresentation(s.Start, s.End)).ToArray());
        }
        public static IEnumerable<List<SortedCurveRepresentation>> MergeCurves(CurveRepresentation[] curves)
        {
            var remainingCrvs = curves.Select((crv, i) => new { Curve = crv, Index = i}).ToList();
            var fndCurve = false;

            while(remainingCrvs.Count > 0)
            {
                var firstCrv = remainingCrvs[0];
                var crvCollection = new List<SortedCurveRepresentation>();
                var lastEnd = firstCrv.Curve.End;

                crvCollection.Add(new(firstCrv.Index, false));
                remainingCrvs.RemoveAt(0);

                for (; ; )
                {
                    fndCurve = false;

                    var i = 0;
                    for (; i < remainingCrvs.Count; i++)
                    {
                        var curCrv = remainingCrvs[i];
                        if (curCrv.Curve.Start.IdenticalTo(lastEnd))
                        {
                            lastEnd = curCrv.Curve.End;
                            fndCurve = true;
                            crvCollection.Add(new(curCrv.Index, false));
                            break;
                        }
                        else if (curCrv.Curve.End.IdenticalTo(lastEnd))
                        {
                            lastEnd = curCrv.Curve.Start;
                            fndCurve = true;
                            crvCollection.Add(new(curCrv.Index, true));
                            break;
                        }
                    }

                    if (!fndCurve)
                    {
                        // Nothing to add
                        break;
                    }
                    else
                    {
                        remainingCrvs.RemoveAt(i);
                    }
                }

                yield return crvCollection;
            }
        }
    }
}
