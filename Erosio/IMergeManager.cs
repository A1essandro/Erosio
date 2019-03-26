using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VectorAndPoint.ValTypes;

namespace Erosio
{
    public interface IMergeManager
    {

        IDictionary<WaterDrop, PointInt> Merge(double[,] map, IDictionary<WaterDrop, PointInt> drops);

        Task<IDictionary<WaterDrop, PointInt>> MergeAsync(double[,] map, IDictionary<WaterDrop, PointInt> drops, CancellationToken ct = default(CancellationToken));

    }
}