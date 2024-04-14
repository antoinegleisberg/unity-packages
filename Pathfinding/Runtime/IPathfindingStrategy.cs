using System;
using System.Collections.Generic;
using UnityEngine;

namespace antoinegleisberg.Pathfinding
{
    internal interface IPathfindingStrategy<TNode> where TNode : class
    {
        public List<TNode> FindPath(TNode start, TNode end);

        public List<TNode> FindPath(List<TNode> startNodes, List<TNode> endNodes);
    }
}
