using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace Erosio
{
    public interface IPropagateManager
    {

        IDictionary<WaterDrop, Point> Propagate(double[,] map, IDictionary<WaterDrop, Point> drops);

        Task<IDictionary<WaterDrop, Point>> PropagateAsync(double[,] map, IDictionary<WaterDrop, Point> drops, CancellationToken ct = default(CancellationToken));

    }
}