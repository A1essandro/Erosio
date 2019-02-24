using System;
using System.Diagnostics;
using System.Drawing;

namespace Erosio
{

    [DebuggerDisplay("({X}, {Y})")]
    public struct Vector : IEquatable<Vector>
    {

        public Vector(double x, double y)
        {
            X = x;
            Y = y;
            _length = Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));
        }

        public Vector(Point a, Point b)
        {
            X = b.X - a.X;
            Y = b.Y - a.Y;
            _length = Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));
        }

        private double _length;

        public double X { get; private set; }

        public double Y { get; private set; }

        public static Vector operator +(Vector v1, Vector v2) => new Vector(v1.X + v2.X, v1.Y + v2.Y);

        public double Length => _length;

        public override int GetHashCode() => ((17 + X.GetHashCode()) * 11) ^ (13 + Y.GetHashCode()) * 7;

        public override bool Equals(object obj) => GetHashCode() == obj?.GetHashCode();

        public bool Equals(Vector other) => GetHashCode() == other.GetHashCode();

        public static bool IsCollinear(Vector v1, Vector v2) => v1.X / v1.Y == v2.X / v2.Y;

        public static double GetAngle(Vector v1, Vector v2)
        {
            var scalar = (v1.X * v2.X + v1.Y * v2.Y) / v1.Length * v2.Length;

            return Math.Acos(scalar);
        }

        public static Vector operator *(Vector v, double s) => new Vector(v.X * s, v.Y * s);

        public static Vector operator *(double s, Vector v) => v * s;

        public static bool operator ==(Vector drop1, Vector drop2) => drop1.Equals(drop2);

        public static bool operator !=(Vector drop1, Vector drop2) => !drop1.Equals(drop2);

    }
}