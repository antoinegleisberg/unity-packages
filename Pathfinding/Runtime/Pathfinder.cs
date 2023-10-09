

using antoinegleisberg.Types;
using System;
using System.Collections.Generic;

namespace antoinegleisberg.Pathfinding
{
    public class Pathfinder
    {
        private IPathfindingStrategy _strategy;

        private Pathfinder(IPathfindingStrategy strategy)
        {
            _strategy = strategy;
        }

        public static Pathfinder GetAStarPathfinder(Func<Node, Node, int> heuristicDistance, Func<Node, List<Pair<Node, int>>> neighbours)
        {
            AStar aStar = new AStar(heuristicDistance, neighbours);

            Pathfinder pathfinder = new Pathfinder(aStar);

            return pathfinder;
        }

        public List<Node> FindPath(Node start, Node end)
        {
            return _strategy.FindPath(start, end);
        }
    }
}
