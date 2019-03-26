using Erosio;
using System.Collections.Generic;
using System.Linq;
using VectorAndPoint.ValTypes;
using Xunit;

namespace Tests
{
    public class AbsorptionManagerTest
    {

        private double[,] _map = new double[,] {
                { 0.5, 0.5 },
                { 0.5, 0.5 },
            };


        [Fact]
        public void AbsorbTest()
        {
            var initialMass = 1.0;
            var manager = new AbsorptionManager();
            var drops = new Dictionary<WaterDrop, PointInt> {
                { new WaterDrop(initialMass), new PointInt(0, 0) }
            };

            var newDrops = manager.Absorb(_map, drops);

            Assert.True(1.0 > newDrops.First().Key.Mass);
        }

    }
}