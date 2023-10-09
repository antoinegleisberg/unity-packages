using antoinegleisberg.Types;
using System;
using System.Collections.Generic;

namespace antoinegleisberg.Pathfinding
{
    internal class Dijkstra : IPathfindingStrategy
    {
        private Func<Node, List<Pair<Node, int>>> _neighbours;

        public Dijkstra(Func<Node, List<Pair<Node, int>>> neighbours)
        {
            _neighbours = neighbours;
        }

        public List<Node> FindPath(Node start, Node end)
        {
            throw new NotImplementedException();
        }
    }
}
