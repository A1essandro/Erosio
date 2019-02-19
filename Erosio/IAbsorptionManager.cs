using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace Erosio
{
    public interface IAbsorptionManager
    {
         
        IDictionary<WaterDrop, Point> Absorb(double[,] map, IDictionary<WaterDrop, Point> drops);

        Task<IDictionary<WaterDrop, Point>> AbsorbAsync(double[,] map, IDictionary<WaterDrop, Point> drops, CancellationToken ct = default(CancellationToken));

    }
}