using Erosio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VectorAndPoint.ValTypes;
using Xunit;

namespace Tests
{
    public class PropagateManagerTest
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

        [Theory]
        [InlineData("plato", 0, 2, 3)]
        [InlineData("plato", 1, 2, 4)]
        [InlineData("plato", 2, 2, 4)]
        [InlineData("pit", 0, 2, 3)]
        [InlineData("pit", 1, 2, 1)]
        [InlineData("pit", 2, 2, 0)]
        public void PropagateTest(string type, int x, int y, int expectedCount)
        {
            var propagator = new PropagateManager(PropagateManager.DefaultNeighborsGetter);
            var drops = new Dictionary<WaterDrop, PointInt> { { new WaterDrop(0.1), new PointInt(x, y) } };

            var newDrops = propagator.Propagate(_maps[type], drops);

            Assert.Equal(expectedCount, newDrops.Count);
        }

        [Theory]
        [InlineData("plato", 0, 2, 3)]
        [InlineData("plato", 1, 2, 4)]
        [InlineData("plato", 2, 2, 4)]
        [InlineData("pit", 0, 2, 3)]
        [InlineData("pit", 1, 2, 1)]
        [InlineData("pit", 2, 2, 0)]
        public async Task PropagateTestAsync(string type, int x, int y, int expectedCount)
        {
            var propagator = new PropagateManager(PropagateManager.DefaultNeighborsGetter);
            var drops = new Dictionary<WaterDrop, PointInt> { { new WaterDrop(0.1), new PointInt(x, y) } };

            var newDrops = await propagator.PropagateAsync(_maps[type], drops);

            Assert.Equal(expectedCount, newDrops.Count);
        }

        [Fact]
        public void PropagateToPitSpeedTest()
        {
            var propagator = new PropagateManager(PropagateManager.DefaultNeighborsGetter);
            var drops = new Dictionary<WaterDrop, PointInt> { { new WaterDrop(0.1), new PointInt(1, 2) } };

            var newDrop = propagator.Propagate(_maps["pit"], drops).Single().Key;

            Assert.True(newDrop.Speed.Length > 0);
            Assert.True(Vector.IsCollinear(newDrop.Speed, new Vector(1, 0)));
        }

        [Fact]
        public async Task PropagateAsyncCancelTest()
        {
            var propagator = new PropagateManager(PropagateManager.DefaultNeighborsGetter);
            var drops = new Dictionary<WaterDrop, PointInt> { { new WaterDrop(0.1), new PointInt(2, 2) } };

            var cts = new CancellationTokenSource();
            cts.Cancel();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => propagator.PropagateAsync(_maps["plato"], drops, cts.Token));
        }

    }
}