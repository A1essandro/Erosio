using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VectorAndPoint.ValTypes;

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

        public IDictionary<WaterDrop, PointInt> Absorb(double[,] map, IDictionary<WaterDrop, PointInt> drops)
        {
            var result = new Dictionary<WaterDrop, PointInt>();

            foreach (var drop in drops)
            {
                var newMass = _absorbtionFunc(drop.Key.Mass);
                result.Add(new WaterDrop(newMass), drop.Value);
            }

            return result;
        }

        public Task<IDictionary<WaterDrop, PointInt>> AbsorbAsync(double[,] map, IDictionary<WaterDrop, PointInt> drops, CancellationToken ct = default(CancellationToken))
        {
            ct.ThrowIfCancellationRequested();
            
            var result = Absorb(map, drops);
            return Task.FromResult(result);
        }

    }
}