using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Erosio
{
    public class AbsorptionManager : IAbsorptionManager
    {

        public readonly static Func<double, double> DefaultAbsorbtion = oldVal => oldVal - 0.015; //TODO: Magic constant...

        private readonly Func<double, double> _absorbtionFunc;

        public AbsorptionManager(Func<double, double> absorbtionFunc = null)
        {
            _absorbtionFunc = absorbtionFunc ?? DefaultAbsorbtion;
        }

        public IDictionary<WaterDrop, Point> Absorb(double[,] map, IDictionary<WaterDrop, Point> drops)
        {
            var result = new Dictionary<WaterDrop, Point>();

            foreach (var drop in drops)
            {
                var newMass = _absorbtionFunc(drop.Key.Mass);
                result.Add(new WaterDrop(newMass), drop.Value);
            }

            return result;
        }

        public Task<IDictionary<WaterDrop, Point>> AbsorbAsync(double[,] map, IDictionary<WaterDrop, Point> drops, CancellationToken ct = default(CancellationToken))
        {
            ct.ThrowIfCancellationRequested();
            
            var result = Absorb(map, drops);
            return Task.FromResult(result);
        }

    }
}