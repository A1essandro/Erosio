using Erosio.Internal.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VectorAndPoint.ValTypes;

namespace Erosio
{
    public class PropagateManager : IPropagateManager
    {

        public readonly static Func<PointInt, IEnumerable<PointInt>> DefaultNeighborsGetter = (PointInt pos) =>
        {
            return new List<PointInt>
            {
                new PointInt(pos.X, pos.Y - 1),
                new PointInt(pos.X, pos.Y + 1),
                new PointInt(pos.X - 1, pos.Y),
                new PointInt(pos.X + 1, pos.Y)
            };
        };

        private readonly Func<PointInt, IEnumerable<PointInt>> _neighborsGetter;

        public PropagateManager(Func<PointInt, IEnumerable<PointInt>> neighborsGetter = null)
        {
            _neighborsGetter = neighborsGetter ?? DefaultNeighborsGetter;
        }

        public IDictionary<WaterDrop, PointInt> Propagate(double[,] map, IDictionary<WaterDrop, PointInt> drops)
        {
            return _propagateAll(map, drops);
        }

        private IDictionary<WaterDrop, PointInt> _propagateAll(double[,] map, IDictionary<WaterDrop, PointInt> drops)
        {
            var newDrops = new Dictionary<WaterDrop, PointInt>();

            foreach (var drop in drops)
            {
                foreach (var newDrop in _propagateDrop(map, drop))
                {
                    newDrops.Add(newDrop.Key, newDrop.Value);
                }
            }

            return newDrops;
        }

        public Task<IDictionary<WaterDrop, PointInt>> PropagateAsync(
            double[,] map, IDictionary<WaterDrop, PointInt> drops, CancellationToken ct = default(CancellationToken))
        {
            ct.ThrowIfCancellationRequested();

            return Task.FromResult(_propagateAll(map, drops));
        }

        #region private methods

        private IEnumerable<KeyValuePair<WaterDrop, PointInt>> _propagateDrop(
            double[,] map, KeyValuePair<WaterDrop, PointInt> drop, CancellationToken ct = default(CancellationToken))
        {
            ct.ThrowIfCancellationRequested();

            var moveRanks = _getMoveRanks(map, drop);
            var rankSum = moveRanks.Sum(x => x.Value);
            var moveFactors = _getMoveFactors(moveRanks, rankSum);
            foreach (var targetCell in moveFactors)
            {
                var watermassFactor = targetCell.Value;
                var speed = CalculateSpeed(map, drop, targetCell);
                yield return new KeyValuePair<WaterDrop, PointInt>(new WaterDrop(drop.Key.Mass * watermassFactor, speed), targetCell.Key);
            }
        }

        private static Vector CalculateSpeed(double[,] map, KeyValuePair<WaterDrop, PointInt> drop, KeyValuePair<PointInt, double> targetCell)
        {
            var scalarSpeed = GetHeight(map, drop.Value) - GetHeight(map, targetCell.Key);
            var unitVectorSpeed = VectorExtension.CreateFromTwoPoints(drop.Value, targetCell.Key);
            var speed = scalarSpeed * unitVectorSpeed;
            return speed + drop.Key.Speed;
        }

        private static Dictionary<PointInt, double> _getMoveFactors(Dictionary<PointInt, double> moveRanks, double rankSum)
        {
            Dictionary<PointInt, double> moveFactors;
            if (rankSum > 0)
                moveFactors = moveRanks.ToDictionary(x => x.Key, x => x.Value / rankSum);
            else
                moveFactors = moveRanks.ToDictionary(x => x.Key, x => 1.0 / moveRanks.Count);
            return moveFactors;
        }

        private Dictionary<PointInt, double> _getMoveRanks(double[,] map, KeyValuePair<WaterDrop, PointInt> drop)
        {
            var currentDropPosition = drop.Value;
            var dropObj = drop.Key;
            var moveRanks = new Dictionary<PointInt, double>();
            foreach (var targetCell in _neighborsGetter(currentDropPosition))
            {
                if (!IsInMap(map, targetCell))
                    continue;
                var heightDiff = GetHeight(map, currentDropPosition) - GetHeight(map, targetCell);
                if (heightDiff < 0)
                    continue;
                if (dropObj.Speed.Length > 0)
                {
                    var targetSpeed = VectorExtension.CreateFromTwoPoints(currentDropPosition, targetCell);
                    var angle = dropObj.Speed.GetAngleWith(targetSpeed);
                    var factor = 0.5 * Math.Abs(angle);
                    moveRanks[targetCell] = heightDiff / factor;
                }
                else
                {
                    var factor = Math.PI / 3; //TODO
                    moveRanks[targetCell] = heightDiff / factor;
                }
            }

            return moveRanks;
        }

        private static bool IsInMap(double[,] map, PointInt v) => v.X >= 0 && v.Y >= 0 && v.X < map.GetLength(0) && v.Y < map.GetLength(1);

        private static double GetHeight(double[,] map, PointInt v) => map[v.X, v.Y];

        #endregion

    }
}