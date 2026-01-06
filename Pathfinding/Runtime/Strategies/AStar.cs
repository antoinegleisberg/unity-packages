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
        
        private Func<TNode, List<TNode>, int> _heuristicDistance;
        private Func<TNode, List<Tuple<TNode, int>>> _neighboursDistances; // For each neighbour, the distance to it

        public AStar(Func<TNode, TNode, int> heuristicDistance, Func<TNode, List<Tuple<TNode, int>>> neighbours)
        {
            _heuristicDistance = ConvertHeuristicDistance(heuristicDistance);
            _neighboursDistances = neighbours;

            _gScore = new Dictionary<TNode, int>();
            _fScore = new Dictionary<TNode, int>();
            _hScore = new Dictionary<TNode, int>();
            _parents = new Dictionary<TNode, TNode>();
        }

        public List<TNode> FindPath(TNode start, TNode end)
        {
            return FindPath(new List<TNode>() { start }, new List<TNode>() { end });
        }

        public List<TNode> FindPath(List<TNode> startNodes, List<TNode> endNodes)
        {
            // ToDo: implement priority queue to optimise this
            List<TNode> openList = new List<TNode>();
            HashSet<TNode> closedSet = new HashSet<TNode>();

            _gScore.Clear();
            _fScore.Clear();
            _hScore.Clear();
            _parents.Clear();

            foreach (TNode startNode in startNodes)
            {
                _gScore.Add(startNode, 0);
                _hScore.Add(startNode, _heuristicDistance(startNode, endNodes));
                _fScore.Add(startNode, _gScore[startNode] + _hScore[startNode]);
                _parents.Add(startNode, null);

                openList.Add(startNode);
            }

            while (openList.Count > 0)
            {
                TNode currentNode = GetLowestCostNode(openList);

                if (endNodes.Contains(currentNode))
                {
                    return CalculatePath(currentNode);
                }

                openList.Remove(currentNode);
                closedSet.Add(currentNode);

                foreach (Tuple<TNode, int> neighbourDistancePair in _neighboursDistances(currentNode))
                {
                    TNode neighbour = neighbourDistancePair.Item1;
                    int distance = neighbourDistancePair.Item2;

                    if (closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    if (!_parents.ContainsKey(neighbour))
                    {
                        _parents[neighbour] = currentNode;
                        _gScore[neighbour] = int.MaxValue;
                        _hScore[neighbour] = _heuristicDistance(neighbour, endNodes);
                        _fScore[neighbour] = int.MaxValue;
                    }

                    UpdateCosts(currentNode, neighbour, distance);

                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                    }
                }
            }
            
            // No path was found, the two nodes are not connected
            return null;
        }

        private void UpdateCosts(TNode currentNode, TNode neighbour, int distance)
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
            foreach (TNode node in openList)
            {
                if (_fScore[node] < _fScore[lowestNode])
                {
                    lowestNode = node;
                }

                else if (_fScore[node] == _fScore[lowestNode])
                {
                    if (_hScore[node] < _hScore[lowestNode])
                    {
                        lowestNode = node;
                    }
                }
            }
            return lowestNode;
        }

        private static Func<TNode, List<TNode>, int> ConvertHeuristicDistance(Func<TNode, TNode, int> heuristicDistance)
        {
            Func<TNode, List<TNode>, int> heuristicFunction = (node, endNodes) =>
            {
                int minDistance = int.MaxValue;
                foreach (TNode endNode in endNodes)
                {
                    int distance = heuristicDistance(node, endNode);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                    }
                }
                return minDistance;
            };

            return heuristicFunction;
        }
    }
}
