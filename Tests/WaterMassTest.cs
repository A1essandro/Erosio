using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Erosio;
using Xunit;

namespace Tests.Ravine
{

    public class WaterMassTest
    {
        private double[,] _map = new double[,] {
                { 0.5, 0.5, 0.5, 0.5, 0.5 },
                { 0.5, 0.5, 0.5, 0.5, 0.5 },
                { 0.5, 0.5, 0.5, 0.5, 0.5 },
                { 0.5, 0.5, 0.5, 0.5, 0.5 },
                { 0.5, 0.5, 0.5, 0.5, 0.5 },
            };

        [Fact]
        public void PropagateTest()
        {
            var propagator = new PropagateManager(PropagateManager.DefaultNeighborsGetter);
            var context = new WaterContext(_map, propagator);
            var drop = new WaterDrop(0.1);

            context.AddDrop(drop, (2, 2));
            context.PropagateWater();
            var drops = context.Drops;

            Assert.Equal(4, drops.Count);
        }

        [Fact]
        public async Task PropagateAsyncTest()
        {
            var propagator = new PropagateManager(PropagateManager.DefaultNeighborsGetter);
            var context = new WaterContext(_map, propagator);
            var drop = new WaterDrop(0.1);

            context.AddDrop(drop, (2, 2));
            await context.PropagateWaterAsync();
            var drops = context.Drops;

            Assert.Equal(4, drops.Count);
        }

        [Fact]
        public async Task PropagateAsyncCancelTest()
        {
            var propagator = new PropagateManager(PropagateManager.DefaultNeighborsGetter);
            var context = new WaterContext(_map, propagator);
            var drop = new WaterDrop(0.1);

            context.AddDrop(drop, (2, 2));
            var cts = new CancellationTokenSource();
            cts.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(() => context.PropagateWaterAsync(cts.Token));
        }

        [Fact]
        public void MergeTest()
        {
            var propagator = new PropagateManager(PropagateManager.DefaultNeighborsGetter);
            var context = new WaterContext(_map, propagator);
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