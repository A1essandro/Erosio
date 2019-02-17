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
        private readonly ConcurrentDictionary<WaterDrop, Vector> _drops = new ConcurrentDictionary<WaterDrop, Vector>();
        private readonly IPropagateManager _propagator;
        private readonly IMergeManager _mergeManager;

        public IDictionary<WaterDrop, Vector> Drops => _drops;


        public WaterContext(
            double[,] heightmap,
            IPropagateManager propagator,
            IMergeManager mergeManager)
        {
            _heightmap = heightmap;
            _propagator = propagator;
            _mergeManager = mergeManager;
        }

        public void AddDrop(WaterDrop drop, (int, int) p) => AddDrop(drop, new Vector(p.Item1, p.Item2));

        public void AddDrop(WaterDrop drop, Vector position) => _drops.TryAdd(drop, position);

        public void Step(Func<double, double> absobtion = null)
        {
            PropagateWater();
            Merge();
            Absorb(absobtion ?? DefaultAbsorbtion);
        }

        public async Task StepAsync(Func<double, double> absobtion = null, CancellationToken ct = default(CancellationToken))
        {
            await PropagateWaterAsync(ct).ConfigureAwait(false);
            await MergeAsync(ct);
            Absorb(absobtion ?? DefaultAbsorbtion);
        }

        #region propagation

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

        #endregion

        /// <summary>
        /// Merge all drops are in the same cells
        /// </summary>
        public void Merge()
        {
            var newDrops = _mergeManager.Merge(_heightmap, _drops);
            _peplaceDrops(newDrops);
        }

        /// <summary>
        /// Merge all drops are in the same cells
        /// </summary>
        public async Task MergeAsync(CancellationToken ct = default(CancellationToken))
        {
            var newDrops = await _mergeManager.MergeAsync(_heightmap, _drops, ct).ConfigureAwait(false);
            _peplaceDrops(newDrops);
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
                _drops.TryAdd(new WaterDrop(newMass), drop.Value);
            }
        }

        private void _peplaceDrops(IDictionary<WaterDrop, Vector> newDrops)
        {
            _drops.Clear();
            foreach (var drop in newDrops)
            {
                _drops.TryAdd(drop.Key, drop.Value);
            }
        }

    }
}