using System;
using System.Collections.Generic;

namespace Pancake.ManagedGeometry
{
    public static class ManagedShapeExtensions
    {
        public static Coord2d ToVector(this Direction dir)
        {
            return dir switch
            {
                Direction.Right => (1, 0),
                Direction.RightBottom => (1, -1),
                Direction.Bottom => (0, -1),
                Direction.LeftBottom => (-1, -1),
                Direction.Left => (-1, 0),
                Direction.LeftTop => (-1, 1),
                Direction.Top => (0, 1),
                Direction.RightTop => (1, 1),
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        public static string ToArrow(this Direction dir)
        {
            return dir switch
            {
                Direction.Right => "→",
                Direction.RightBottom => "↘",
                Direction.Bottom => "↓",
                Direction.LeftBottom => "↙",
                Direction.Left => "←",
                Direction.LeftTop => "↖",
                Direction.Top => "↑",
                Direction.RightTop => "↗",
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        public static IEnumerable<Direction> AllDirections()
        {
            for (var i = Direction.Right; i <= Direction.LeftBottom; i++)
                yield return i;
        }
    }

    public enum Direction : int
    {
        Right,
        Left,
        Top,
        Bottom,
        RightTop,
        LeftTop,
        RightBottom,
        LeftBottom
    }
}
