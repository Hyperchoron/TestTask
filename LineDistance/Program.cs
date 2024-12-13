using System;

namespace LineDistance
{
    readonly struct Vec2d
    {
        public float X => _x;

        public float Y => _y;

        public Vec2d(float x, float y)
        {
            _x = x;
            _y = y;
        }

        #region Vector functions
        public float Magnitude()
        {
            return (float)Math.Sqrt(_x * _x + _y * _y);
        }
        #endregion

        #region Neccessary operator
        public static Vec2d operator -(Vec2d a, Vec2d b)
        {
            return new Vec2d(a.X - b.X, a.Y - b.Y);
        }
        #endregion

        private readonly float _x, _y;

        public override string ToString()
        {
            return $"{{{_x}, {_y}}}";
        }
    }

    readonly struct Line
    {
        public Vec2d P1 => _p1;

        public Vec2d P2 => _p2;

        public Line(Vec2d p1, Vec2d p2)
        {
            _p1 = p1;
            _p2 = p2;
        }

        public float Distance(Vec2d point)
        {
            var dir = P2 - P1;

            return Math.Abs(
                dir.Y * point.X -
                dir.X * point.Y +
                _p2.X * _p1.Y - _p2.Y * _p1.X
                ) / dir.Magnitude();
        }

        private readonly Vec2d _p1, _p2;

        public override string ToString()
        {
            return $"(p1={_p1}, p2={_p2})";
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var p1 = new Vec2d(5, 0);
            var p2 = new Vec2d(5, 10);

            var testP = new Vec2d(10, 5);

            var line = new Line(p1, p2);

            Console.WriteLine($"Distance from {testP} to {line} = {line.Distance(testP)}");
        }
    }
}