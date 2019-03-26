using VectorAndPoint.ValTypes;

namespace Erosio.Internal.Extensions
{
    internal static class VectorExtension
    {

        public static Vector CreateFromTwoPoints(PointInt a, PointInt b)
        {
            var x = b.X - a.X;
            var y = b.Y - a.Y;
            return new Vector(x, y);
        }

    }
}
