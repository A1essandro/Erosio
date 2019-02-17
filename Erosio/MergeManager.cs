using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Erosio
{
    public class MergeManager : IMergeManager
    {
        public IDictionary<WaterDrop, Vector> Merge(double[,] map, IDictionary<WaterDrop, Vector> drops)
        {
            var newDrops = new Dictionary<WaterDrop, Vector>();
            var groups = drops.GroupBy(x => x.Value).Where(x => x.Count() > 1).ToArray();
            foreach (var group in groups)
            {
                var unmerged = group.Select(x => x.Key).ToArray();
                var merged = unmerged.Aggregate((total, next) => total + next);
                newDrops.Add(merged, group.Key);
            }

            return newDrops;
        }

        public Task<IDictionary<WaterDrop, Vector>> MergeAsync(
            double[,] map, IDictionary<WaterDrop, Vector> drops, CancellationToken ct = default(CancellationToken))
        {
            ct.ThrowIfCancellationRequested();

            return Task.Run(() => Merge(map, drops), ct);
        }
    }
}