

using antoinegleisberg.Types;
using System;
using System.Collections.Generic;

namespace antoinegleisberg.Pathfinding
{
    public class Pathfinder<TNode> where TNode : class
    {
        private IPathfindingStrategy<TNode> _strategy;

        private Pathfinder(IPathfindingStrategy<TNode> strategy)
        {
            _strategy = strategy;
        }

        public static Pathfinder<TNode> GetAStarPathfinder(Func<TNode, TNode, int> heuristicDistance, Func<TNode, IEnumerable<ValueTuple<TNode, int>>> neighbours)
        {
            AStar<TNode> aStar = new AStar<TNode>(heuristicDistance, neighbours);

            Pathfinder<TNode> pathfinder = new Pathfinder<TNode>(aStar);

            return pathfinder;
        }

        public List<TNode> FindPath(TNode start, TNode end)
        {
            return _strategy.FindPath(start, end);
        }

        public List<TNode> FindPath(List<TNode> startNodes, List<TNode> endNodes)
        {
            return _strategy.FindPath(startNodes, endNodes);
        }

        public List<TNode> FindPath(TNode start, List<TNode> endNodes)
        {
            return _strategy.FindPath(new List<TNode> { start }, endNodes);
        }

        public List<TNode> FindPath(List<TNode> startNodes, TNode end)
        {
            return _strategy.FindPath(startNodes, new List<TNode> { end });
        }
    }
}
