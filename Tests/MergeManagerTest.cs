using Erosio;
using System.Collections.Generic;
using VectorAndPoint.ValTypes;
using Xunit;

namespace Tests
{
    public class MergeManagerTest
    {

        private double[,] _map = new double[,] {
                { 0.5, 0.5 },
                { 0.5, 0.5 },
            };


        [Fact]
        public void MergeTest()
        {
            var mergeManager = new MergeManager();
            var _0_0 = new PointInt(0, 0);
            var _0_1 = new PointInt(0, 1);
            var _1_1 = new PointInt(1, 1);
            var m = 0.1;

            var drops = new Dictionary<WaterDrop, PointInt> {
                { new WaterDrop(m), _0_0 },
                { new WaterDrop(m), _0_0 },
                { new WaterDrop(m, new Vector(0, 1)), _0_1 },
                { new WaterDrop(m, new Vector(0, -1)), _0_1 },
                { new WaterDrop(m, new Vector(0, 1)), _1_1 },
                { new WaterDrop(m, new Vector(1, 0)), _1_1 }
            };

            var newDrops = mergeManager.Merge(_map, drops);

            Assert.Equal(3, newDrops.Count);
        }

    }
}