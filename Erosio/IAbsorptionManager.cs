using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VectorAndPoint.ValTypes;

namespace Erosio
{
    public interface IAbsorptionManager
    {
         
        IDictionary<WaterDrop, PointInt> Absorb(double[,] map, IDictionary<WaterDrop, PointInt> drops);

        Task<IDictionary<WaterDrop, PointInt>> AbsorbAsync(double[,] map, IDictionary<WaterDrop, PointInt> drops, CancellationToken ct = default(CancellationToken));

    }
}