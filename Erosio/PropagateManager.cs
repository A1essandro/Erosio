using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
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
                _propagateDrop(map, drop, newDrops);
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

        private void _propagateDrop(
            double[,] map, KeyValuePair<WaterDrop, Point> drop, Dictionary<WaterDrop, Point> newDrops, CancellationToken ct = default(CancellationToken))
        {
            ct.ThrowIfCancellationRequested();

            var currentDropPosition = drop.Value;
            var dropObj = drop.Key;
            var moveRanks = _getMoveRanks(map, currentDropPosition, dropObj);
            var rankSum = moveRanks.Sum(x => x.Value);
            var moveFactors = _getMoveFactors(moveRanks, rankSum);
            foreach (var targetCell in moveFactors)
            {
                var watermassFactor = targetCell.Value;
                newDrops.Add(new WaterDrop(dropObj.Mass * watermassFactor), targetCell.Key);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Dictionary<Point, double> _getMoveFactors(Dictionary<Point, double> moveRanks, double rankSum)
        {
            Dictionary<Point, double> moveFactors;
            if (rankSum > 0)
                moveFactors = moveRanks.ToDictionary(x => x.Key, x => x.Value / rankSum);
            else
                moveFactors = moveRanks.ToDictionary(x => x.Key, x => 1.0 / moveRanks.Count);
            return moveFactors;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Dictionary<Point, double> _getMoveRanks(double[,] map, Point currentDropPosition, WaterDrop dropObj)
        {
            var moveRanks = new Dictionary<Point, double>();
            foreach (var targetCell in _neighborsGetter(currentDropPosition))
            {
                if (!IsInMap(map, targetCell))
                    continue;
                var rank = GetHeight(map, currentDropPosition) - GetHeight(map, targetCell);
                if (rank < 0)
                    continue;
                var range = Math.Sqrt(Math.Pow(currentDropPosition.X + dropObj.Speed.X - targetCell.X, 2)
                    + Math.Pow(currentDropPosition.Y + dropObj.Speed.Y - targetCell.Y, 2));
                var factor = range != 0 ? range : 0.00001;
                moveRanks[targetCell] = rank / factor;
            }

            return moveRanks;
        }

        private static bool IsInMap(double[,] map, Point v) => v.X >= 0 && v.Y >= 0 && v.X < map.GetLength(0) && v.Y < map.GetLength(1);

        private double GetHeight(double[,] map, Point v) => map[v.X, v.Y];

        #endregion

    }
}