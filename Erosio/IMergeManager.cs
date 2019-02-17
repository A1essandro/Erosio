using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Erosio
{
    public interface IMergeManager
    {

        IDictionary<WaterDrop, Vector> Merge(double[,] map, IDictionary<WaterDrop, Vector> drops);

        Task<IDictionary<WaterDrop, Vector>> MergeAsync(double[,] map, IDictionary<WaterDrop, Vector> drops, CancellationToken ct = default(CancellationToken));

    }
}