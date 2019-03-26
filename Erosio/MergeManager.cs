using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VectorAndPoint.ValTypes;

namespace Erosio
{
    public class MergeManager : IMergeManager
    {
        public IDictionary<WaterDrop, PointInt> Merge(double[,] map, IDictionary<WaterDrop, PointInt> drops)
        {
            var newDrops = new Dictionary<WaterDrop, PointInt>();
            var groups = drops.GroupBy(x => x.Value).Where(x => x.Count() > 1).ToArray();
            foreach (var group in groups)
            {
                var unmerged = group.Select(x => x.Key).ToArray();
                var merged = unmerged.Aggregate((total, next) => total + next);
                newDrops.Add(merged, group.Key);
            }

            return newDrops;
        }

        public Task<IDictionary<WaterDrop, PointInt>> MergeAsync(
            double[,] map, IDictionary<WaterDrop, PointInt> drops, CancellationToken ct = default(CancellationToken))
        {
            ct.ThrowIfCancellationRequested();

            var result = Merge(map, drops);
            return Task.FromResult(result);
        }
    }
}