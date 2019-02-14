using System.Collections.Generic;
using System.Threading.Tasks;

namespace Erosio
{
    public interface IPropagator
    {

        IDictionary<WaterDrop, Vector> Propagate(double[,] map, IDictionary<WaterDrop, Vector> drops);

        Task<IDictionary<WaterDrop, Vector>> PropagateAsync(double[,] map, IDictionary<WaterDrop, Vector> drops);

    }
}