using System;
using System.Diagnostics;
using VectorAndPoint.ValTypes;

namespace Erosio
{

    [DebuggerDisplay("M:{Mass}, V({Speed.X},{Speed.Y})")]
    public class WaterDrop : IEquatable<WaterDrop>
    {

        public double Mass { get; set; }

        public Vector Speed { get; set; }

        public double MudMass { get; set; }

        public WaterDrop(double mass)
            : this(mass, new Vector(0, 0))
        {
        }

        public WaterDrop(double mass, Vector speed)
        {
            Mass = mass;
            Speed = speed;
            MudMass = 0;
        }

        public bool Equals(WaterDrop other)
        {
            return Mass == other.Mass
                && MudMass == other.MudMass
                && Speed.GetHashCode() == other.Speed.GetHashCode();
        }

        public static WaterDrop operator +(WaterDrop water1, WaterDrop water2)
        {
            var result = new WaterDrop(water1.Mass + water2.Mass)
            {
                Speed = water1.Speed + water2.Speed,
                MudMass = water1.MudMass + water2.MudMass
            };
            return result;
        }

    }
}
