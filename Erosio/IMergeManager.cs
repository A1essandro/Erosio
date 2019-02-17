using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace Erosio
{
    public interface IMergeManager
    {

        IDictionary<WaterDrop, Point> Merge(double[,] map, IDictionary<WaterDrop, Point> drops);

        Task<IDictionary<WaterDrop, Point>> MergeAsync(double[,] map, IDictionary<WaterDrop, Point> drops, CancellationToken ct = default(CancellationToken));

    }
}