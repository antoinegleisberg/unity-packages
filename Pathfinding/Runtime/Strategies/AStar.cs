using antoinegleisberg.Types;
using System;
using System.Collections.Generic;

namespace antoinegleisberg.Pathfinding
{
    internal class AStar<TNode> : IPathfindingStrategy<TNode> where TNode : class
    {
        private Dictionary<TNode, int> _gScore; // cost to reach that TNode
        private Dictionary<TNode, int> _fScore; // g + h
        private Dictionary<TNode, int> _hScore; // heuristic distance to end
        private Dictionary<TNode, TNode> _parents;
        
        private Func<TNode, TNode, int> _heuristicDistance;
        private Func<TNode, List<Pair<TNode, int>>> _neighboursDistances; // For each neighbour, the distance to it

        public AStar(Func<TNode, TNode, int> heuristicDistance, Func<TNode, List<Pair<TNode, int>>> neighbours)
        {
            _heuristicDistance = heuristicDistance;
            _neighboursDistances = neighbours;

            _gScore = new Dictionary<TNode, int>();
            _fScore = new Dictionary<TNode, int>();
            _hScore = new Dictionary<TNode, int>();
            _parents = new Dictionary<TNode, TNode>();
        }

        public List<TNode> FindPath(TNode start, TNode end)
        {
            List<TNode> openList = new List<TNode>();
            HashSet<TNode> closedSet = new HashSet<TNode>();

            _gScore.Clear();
            _fScore.Clear();
            _hScore.Clear();
            _parents.Clear();

            _gScore.Add(start, 0);
            _hScore.Add(start, _heuristicDistance(start, end));
            _fScore.Add(start, _gScore[start] + _hScore[start]);
            _parents.Add(start, null);

            openList.Add(start);

            while (openList.Count > 0)
            {
                TNode currentNode = GetLowestCostNode(openList);

                if (currentNode == end)
                {
                    return CalculatePath(end);
                }

                openList.Remove(currentNode);
                closedSet.Add(currentNode);

                foreach (Pair<TNode, int> neighbourDistancePair in _neighboursDistances(currentNode))
                {
                    TNode neighbour = neighbourDistancePair.First;
                    int distance = neighbourDistancePair.Second;

                    if (closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    if (!_parents.ContainsKey(neighbour))
                    {
                        _parents[neighbour] = currentNode;
                        _gScore[neighbour] = int.MaxValue;
                        _hScore[neighbour] = _heuristicDistance(neighbour, end);
                        _fScore[neighbour] = int.MaxValue;
                    }

                    UpdateCosts(currentNode, neighbour, distance, end);

                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                    }
                }
            }
            
            // No path was found, the two nodes are not connected
            return null;
        }

        private void UpdateCosts(TNode currentNode, TNode neighbour, int distance, TNode end)
        {
            int potentialNewGCost = _gScore[currentNode] + distance;

            if (potentialNewGCost < _gScore[neighbour])
            {
                _parents[neighbour] = currentNode;
                _gScore[neighbour] = potentialNewGCost;
                _fScore[neighbour] = _gScore[neighbour] + _hScore[neighbour];
            }
        }
        

        private List<TNode> CalculatePath(TNode end)
        {
            List<TNode> path = new List<TNode>();
            TNode currentNode = end;
            while (_parents[currentNode] != null)
            {
                path.Insert(0, currentNode);
                currentNode = _parents[currentNode];
            }
            path.Insert(0, currentNode);
            return path;
        }

        private TNode GetLowestCostNode(List<TNode> openList)
        {
            TNode lowestNode = openList[0];
            foreach (TNode TNode in openList)
            {
                if (_fScore[TNode] < _fScore[lowestNode])
                {
                    lowestNode = TNode;
                }

                else if (_fScore[TNode] == _fScore[lowestNode])
                {
                    if (_hScore[TNode] < _hScore[lowestNode])
                    {
                        lowestNode = TNode;
                    }
                }
            }
            return lowestNode;
        }
    }
}
