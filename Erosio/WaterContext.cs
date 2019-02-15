using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Erosio
{
    public class WaterContext
    {

        public readonly static Func<double, double> DefaultAbsorbtion = oldVal => oldVal - 0.015; //TODO: Magic constant...

        private readonly double[,] _heightmap;
        private readonly IDictionary<WaterDrop, Vector> _drops = new Dictionary<WaterDrop, Vector>();
        private readonly IPropagator _propagator;

        public IDictionary<WaterDrop, Vector> Drops => _drops;


        public WaterContext(double[,] heightmap, IPropagator propagator)
        {
            _heightmap = heightmap;
            _propagator = propagator;
        }

        public void AddDrop(WaterDrop drop, (int, int) p) => AddDrop(drop, new Vector(p.Item1, p.Item2));

        public void AddDrop(WaterDrop drop, Vector position) => _drops.Add(drop, position);

        public void Step(Func<double, double> absobtion = null)
        {
            PropagateWater();
            Merge();
            Absorb(absobtion ?? DefaultAbsorbtion);
        }

        public async Task StepAsync(Func<double, double> absobtion = null, CancellationToken ct = default(CancellationToken))
        {
            await PropagateWaterAsync(ct).ConfigureAwait(false);
            Merge();
            Absorb(absobtion ?? DefaultAbsorbtion);
        }

        /// <summary>
        /// Propagation drops on the map
        /// </summary>
        public void PropagateWater()
        {
            var newDrops = _propagator.Propagate(_heightmap, _drops);
            _peplaceDrops(newDrops);
        }

        /// <summary>
        /// Propagation drops on the map
        /// </summary>
        public async Task PropagateWaterAsync(CancellationToken ct = default(CancellationToken))
        {
            ct.ThrowIfCancellationRequested();

            var newDrops = await _propagator.PropagateAsync(_heightmap, _drops, ct).ConfigureAwait(false);
            _peplaceDrops(newDrops);
        }

        /// <summary>
        /// Merge all drops are in the same cells
        /// </summary>
        public void Merge()
        {
            var groups = _drops.GroupBy(x => x.Value).Where(x => x.Count() > 1).ToArray();
            foreach (var group in groups)
            {
                var unmerged = group.Select(x => x.Key).ToArray();
                var merged = unmerged.Aggregate((total, next) => total + next);
                foreach (var key in unmerged)
                    _drops.Remove(key);
                _drops.Add(merged, group.Key);
            }
        }

        /// <summary>
        /// Step of water absorption into the soil 
        /// </summary>
        /// <param name="absobtion"></param>
        public void Absorb(Func<double, double> absobtion)
        {
            var drops = _drops.ToArray();
            _drops.Clear();
            foreach (var drop in drops)
            {
                var newMass = DefaultAbsorbtion(drop.Key.Mass);
                _drops.Add(new WaterDrop(newMass), drop.Value);
            }
        }

        private void _peplaceDrops(IDictionary<WaterDrop, Vector> newDrops)
        {
            _drops.Clear();
            foreach (var drop in newDrops)
            {
                _drops.Add(drop.Key, drop.Value);
            }
        }

    }
}