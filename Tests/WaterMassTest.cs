using System;
using System.Collections.Generic;
using System.Linq;
using Erosio;
using Xunit;

namespace Tests.Ravine
{

    [Obsolete]
    public class WaterMassTest
    {

        private Dictionary<string, double[,]> _maps = new Dictionary<string, double[,]> {
            {
                "plato",
                new double[,] {
                    { 0.5, 0.5, 0.5, 0.5, 0.5 },
                    { 0.5, 0.5, 0.5, 0.5, 0.5 },
                    { 0.5, 0.5, 0.5, 0.5, 0.5 },
                    { 0.5, 0.5, 0.5, 0.5, 0.5 },
                    { 0.5, 0.5, 0.5, 0.5, 0.5 },
                }
            },
            {
                "pit",
                new double[,] {
                    { 0.5, 0.5, 0.5, 0.5, 0.5 },
                    { 0.5, 0.4, 0.3, 0.4, 0.5 },
                    { 0.5, 0.3, 0.2, 0.3, 0.5 },
                    { 0.5, 0.4, 0.3, 0.4, 0.5 },
                    { 0.5, 0.5, 0.5, 0.5, 0.5 },
                }
            }
        };

        [Fact]
        public void MergeTest()
        {
            var propagator = new PropagateManager(PropagateManager.DefaultNeighborsGetter);
            var context = new WaterContext(_maps["plato"], propagator);
            var drop = new WaterDrop(0.1);

            context.AddDrop(drop, (2, 2));
            context.PropagateWater();
            context.PropagateWater();
            var dropsBefore = context.Drops.ToDictionary(x => x.Key, x => x.Value);
            context.Merge();
            var dropsAfter = context.Drops.ToDictionary(x => x.Key, x => x.Value);

            Assert.Equal(16, dropsBefore.Count);
            Assert.Equal(9, dropsAfter.Count);
        }

    }

}