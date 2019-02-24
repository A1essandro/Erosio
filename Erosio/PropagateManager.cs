using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Erosio
{
    public class PropagateManager : IPropagateManager
    {

        public readonly static Func<Point, IEnumerable<Point>> DefaultNeighborsGetter = (Point pos) =>
        {
            return new List<Point>
            {
                new Point(pos.X, pos.Y - 1),
                new Point(pos.X, pos.Y + 1),
                new Point(pos.X - 1, pos.Y),
                new Point(pos.X + 1, pos.Y)
            };
        };

        private readonly Func<Point, IEnumerable<Point>> _neighborsGetter;

        public PropagateManager(Func<Point, IEnumerable<Point>> neighborsGetter = null)
        {
            _neighborsGetter = neighborsGetter ?? DefaultNeighborsGetter;
        }

        public IDictionary<WaterDrop, Point> Propagate(double[,] map, IDictionary<WaterDrop, Point> drops)
        {
            return _propagateAll(map, drops);
        }

        private IDictionary<WaterDrop, Point> _propagateAll(double[,] map, IDictionary<WaterDrop, Point> drops)
        {
            var newDrops = new Dictionary<WaterDrop, Point>();

            foreach (var drop in drops)
            {
                foreach (var newDrop in _propagateDrop(map, drop))
                {
                    newDrops.Add(newDrop.Key, newDrop.Value);
                }
            }

            return newDrops;
        }

        public Task<IDictionary<WaterDrop, Point>> PropagateAsync(
            double[,] map, IDictionary<WaterDrop, Point> drops, CancellationToken ct = default(CancellationToken))
        {
            ct.ThrowIfCancellationRequested();

            return Task.FromResult(_propagateAll(map, drops));
        }

        #region private methods

        private IEnumerable<KeyValuePair<WaterDrop, Point>> _propagateDrop(
            double[,] map, KeyValuePair<WaterDrop, Point> drop, CancellationToken ct = default(CancellationToken))
        {
            ct.ThrowIfCancellationRequested();

            var moveRanks = _getMoveRanks(map, drop);
            var rankSum = moveRanks.Sum(x => x.Value);
            var moveFactors = _getMoveFactors(moveRanks, rankSum);
            foreach (var targetCell in moveFactors)
            {
                var watermassFactor = targetCell.Value;
                var speed = CalculateSpeed(map, drop, targetCell);
                yield return new KeyValuePair<WaterDrop, Point>(new WaterDrop(drop.Key.Mass * watermassFactor, speed), targetCell.Key);
            }
        }

        private static Vector CalculateSpeed(double[,] map, KeyValuePair<WaterDrop, Point> drop, KeyValuePair<Point, double> targetCell)
        {
            var scalarSpeed = GetHeight(map, drop.Value) - GetHeight(map, targetCell.Key);
            var unitVectorSpeed = new Vector(drop.Value, targetCell.Key);
            var speed = scalarSpeed * unitVectorSpeed;
            return speed + drop.Key.Speed;
        }

        private static Dictionary<Point, double> _getMoveFactors(Dictionary<Point, double> moveRanks, double rankSum)
        {
            Dictionary<Point, double> moveFactors;
            if (rankSum > 0)
                moveFactors = moveRanks.ToDictionary(x => x.Key, x => x.Value / rankSum);
            else
                moveFactors = moveRanks.ToDictionary(x => x.Key, x => 1.0 / moveRanks.Count);
            return moveFactors;
        }

        private Dictionary<Point, double> _getMoveRanks(double[,] map, KeyValuePair<WaterDrop, Point> drop)
        {
            var currentDropPosition = drop.Value;
            var dropObj = drop.Key;
            var moveRanks = new Dictionary<Point, double>();
            foreach (var targetCell in _neighborsGetter(currentDropPosition))
            {
                if (!IsInMap(map, targetCell))
                    continue;
                var heightDiff = GetHeight(map, currentDropPosition) - GetHeight(map, targetCell);
                if (heightDiff < 0)
                    continue;
                if (dropObj.Speed.Length > 0)
                {
                    var angle = Vector.GetAngle(dropObj.Speed, new Vector(currentDropPosition, targetCell));
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

        private static bool IsInMap(double[,] map, Point v) => v.X >= 0 && v.Y >= 0 && v.X < map.GetLength(0) && v.Y < map.GetLength(1);

        private static double GetHeight(double[,] map, Point v) => map[v.X, v.Y];

        #endregion

    }
}