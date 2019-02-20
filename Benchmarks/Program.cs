using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Erosio;

namespace Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<AsyncVsSync>();
            Console.ReadKey();
        }
    }

    [MemoryDiagnoser]
    public class AsyncVsSync
    {

        private static Dictionary<string, double[,]> Maps = new Dictionary<string, double[,]> {
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
        };

        [Benchmark]
        public void Step()
        {
            var propagator = new PropagateManager(PropagateManager.DefaultNeighborsGetter);
            var merger = new MergeManager();
            var absorber = new AbsorptionManager(AbsorptionManager.DefaultAbsorbtion);
            var context = new WaterContext(Maps["plato"], propagator, merger, absorber);
            for (var i = 0; i < 10; i++)
                context.Step();
        }

        [Benchmark]
        public async Task StepAsync()
        {
            var propagator = new PropagateManager(PropagateManager.DefaultNeighborsGetter);
            var merger = new MergeManager();
            var absorber = new AbsorptionManager(AbsorptionManager.DefaultAbsorbtion);
            var context = new WaterContext(Maps["plato"], propagator, merger, absorber);
            for (var i = 0; i < 10; i++)
                await context.StepAsync();
        }
    }
}
