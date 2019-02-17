using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Erosio;
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
            var drops = new Dictionary<WaterDrop, Vector> { { new WaterDrop(0.1), new Vector(x, y) } };

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
            var drops = new Dictionary<WaterDrop, Vector> { { new WaterDrop(0.1), new Vector(x, y) } };

            var newDrops = await propagator.PropagateAsync(_maps[type], drops);

            Assert.Equal(expectedCount, newDrops.Count);
        }

        [Fact]
        public async Task PropagateAsyncCancelTest()
        {
            var propagator = new PropagateManager(PropagateManager.DefaultNeighborsGetter);
            var drops = new Dictionary<WaterDrop, Vector> { { new WaterDrop(0.1), new Vector(2, 2) } };

            var cts = new CancellationTokenSource();
            cts.Cancel();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => propagator.PropagateAsync(_maps["plato"], drops, cts.Token));
        }

    }
}