using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Erosio
{
    public interface IPropagateManager
    {

        IDictionary<WaterDrop, Vector> Propagate(double[,] map, IDictionary<WaterDrop, Vector> drops);

        Task<IDictionary<WaterDrop, Vector>> PropagateAsync(double[,] map, IDictionary<WaterDrop, Vector> drops, CancellationToken ct = default(CancellationToken));

    }
}