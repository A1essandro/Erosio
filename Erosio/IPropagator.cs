using System.Collections.Generic;

namespace Erosio
{
    public interface IPropagator
    {

        IDictionary<WaterDrop, Vector> Propagate(double[,] map, IDictionary<WaterDrop, Vector> drops);

    }
}