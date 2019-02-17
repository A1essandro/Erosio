using System;
using System.Diagnostics;

namespace Erosio
{

    [DebuggerDisplay("({X}, {Y})")]
    public struct Vector : IEquatable<Vector>
    {

        public Vector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; private set; }

        public int Y { get; private set; }

        public static Vector operator +(Vector v1, Vector v2) => new Vector(v1.X + v2.X, v1.Y + v2.Y);

        public double GetLength() => Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));

        public override int GetHashCode() => ((17 + X) * 11) ^ (13 + Y) * 7;

        public override bool Equals(object obj) => GetHashCode() == obj?.GetHashCode();

        public bool Equals(Vector other) => GetHashCode() == other.GetHashCode();

        public static bool operator ==(Vector drop1, Vector drop2) => drop1.Equals(drop2);

        public static bool operator !=(Vector drop1, Vector drop2) => !drop1.Equals(drop2);

    }
}