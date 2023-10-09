

using System;
using System.Collections.Generic;

namespace antoinegleisberg.Pathfinding
{
    public interface IPathfindingStrategy
    {
        public List<Node> FindPath(Node start, Node end);
    }
}
