using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VectorAndPoint.ValTypes;

namespace Erosio
{
    public class WaterContext
    {

        public readonly static Func<double, double> DefaultAbsorbtion = oldVal => oldVal - 0.015; //TODO: Magic constant...

        private readonly double[,] _heightmap;
        private readonly Dictionary<WaterDrop, PointInt> _drops = new Dictionary<WaterDrop, PointInt>();
        private readonly IPropagateManager _propagator;
        private readonly IMergeManager _mergeManager;
        private readonly IAbsorptionManager _absorptionManager;

        /// <summary>
        /// Copy dictionary of drops in context
        /// </summary>
        /// <param name="x.Key"></param>
        /// <param name="x.Value"></param>
        /// <returns></returns>
        public IDictionary<WaterDrop, PointInt> Drops => _drops.ToDictionary(x => x.Key, x => x.Value);

        public WaterContext(
            double[,] heightmap,
            IPropagateManager propagator,
            IMergeManager mergeManager,
            IAbsorptionManager absorptionManager)
        {
            _heightmap = heightmap;
            _propagator = propagator;
            _mergeManager = mergeManager;
            _absorptionManager = absorptionManager;
        }

        public void AddDrop(WaterDrop drop, (int, int) p) => AddDrop(drop, new PointInt(p.Item1, p.Item2));

        public void AddDrop(WaterDrop drop, PointInt position) => _drops.Add(drop, position);

        public void Step(Func<double, double> absobtion = null)
        {
            PropagateWater();
            Merge();
            Absorb();
        }

        public async Task StepAsync(Func<double, double> absobtion = null, CancellationToken ct = default(CancellationToken))
        {
            await PropagateWaterAsync(ct).ConfigureAwait(false);
            await MergeAsync(ct);
            await AbsorbAsync(ct);
        }

        #region propagation

        /// <summary>
        /// Propagation drops on the map
        /// </summary>
        public void PropagateWater()
        {
            var newDrops = _propagator.Propagate(_heightmap, Drops);
            _peplaceDrops(newDrops);
        }

        /// <summary>
        /// Propagation drops on the map
        /// </summary>
        public async Task PropagateWaterAsync(CancellationToken ct = default(CancellationToken))
        {
            ct.ThrowIfCancellationRequested();

            var newDrops = await _propagator.PropagateAsync(_heightmap, Drops, ct).ConfigureAwait(false);
            _peplaceDrops(newDrops);
        }

        #endregion

        #region  merge

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
            var newDrops = await _mergeManager.MergeAsync(_heightmap, Drops, ct).ConfigureAwait(false);
            _peplaceDrops(newDrops);
        }

        #endregion

        #region absorption

        /// <summary>
        /// Step of water absorption into the soil 
        /// </summary>
        public void Absorb()
        {
            var newDrops = _absorptionManager.Absorb(_heightmap, Drops);
            _peplaceDrops(newDrops);
        }


        /// <summary>
        /// Step of water absorption into the soil 
        /// </summary>
        public async Task AbsorbAsync(CancellationToken ct = default(CancellationToken))
        {
            var newDrops = await _absorptionManager.AbsorbAsync(_heightmap, Drops, ct).ConfigureAwait(false);
            _peplaceDrops(newDrops);
        }

        #endregion

        #region private methods

        private void _peplaceDrops(IDictionary<WaterDrop, PointInt> newDrops)
        {
            _drops.Clear();
            foreach (var drop in newDrops)
            {
                _drops.Add(drop.Key, drop.Value);
            }
        }

        #endregion

    }
}