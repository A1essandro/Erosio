using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VectorAndPoint.ValTypes;

namespace Erosio
{
    public interface IPropagateManager
    {

        IDictionary<WaterDrop, PointInt> Propagate(double[,] map, IDictionary<WaterDrop, PointInt> drops);

        Task<IDictionary<WaterDrop, PointInt>> PropagateAsync(double[,] map, IDictionary<WaterDrop, PointInt> drops, CancellationToken ct = default(CancellationToken));

    }
}