using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Erosio;
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
            var drops = new Dictionary<WaterDrop, Point> {
                { new WaterDrop(initialMass), new Point(0, 0) }
            };

            var newDrops = manager.Absorb(_map, drops);

            Assert.True(1.0 > newDrops.First().Key.Mass);
        }

    }
}