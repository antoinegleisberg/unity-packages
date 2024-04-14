using antoinegleisberg.Types;
using System;
using System.Collections.Generic;

namespace antoinegleisberg.Pathfinding
{
    internal class Dijkstra<TNode> : IPathfindingStrategy<TNode> where TNode : class
    {
        private Func<TNode, List<Pair<TNode, int>>> _neighbours;

        public Dijkstra(Func<TNode, List<Pair<TNode, int>>> neighbours)
        {
            _neighbours = neighbours;
        }

        public List<TNode> FindPath(TNode start, TNode end)
        {
            throw new NotImplementedException();
        }

        public List<TNode> FindPath(List<TNode> startNodes, List<TNode> endNodes)
        {
            throw new NotImplementedException();
        }
    }
}
