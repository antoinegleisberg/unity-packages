

using antoinegleisberg.Types;
using System;
using System.Collections.Generic;

namespace antoinegleisberg.Pathfinding
{
    public class AStar : IPathfindingStrategy
    {
        private Dictionary<Node, int> _gScore; // cost to reach that node
        private Dictionary<Node, int> _fScore; // g + h
        private Dictionary<Node, int> _hScore; // heuristic distance to end
        private Dictionary<Node, Node> _parents;
        
        private Func<Node, Node, int> _heuristicDistance;
        private Func<Node, List<Pair<Node, int>>> _neighboursDistances; // For each neighbour, the distance to it

        public AStar(Func<Node, Node, int> heuristicDistance, Func<Node, List<Pair<Node, int>>> neighbours)
        {
            _heuristicDistance = heuristicDistance;
            _neighboursDistances = neighbours;
        }

        public List<Node> FindPath(Node start, Node end)
        {
            List<Node> openList = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();

            _gScore = new Dictionary<Node, int>();
            _fScore = new Dictionary<Node, int>();
            _hScore = new Dictionary<Node, int>();
            _parents = new Dictionary<Node, Node>();

            _gScore.Add(start, 0);
            _hScore.Add(start, _heuristicDistance(start, end));
            _fScore.Add(start, _gScore[start] + _hScore[start]);
            _parents.Add(start, null);

            openList.Add(start);

            while (openList.Count > 0)
            {
                Node currentNode = GetLowestCostNode(openList);

                if (currentNode == end)
                {
                    return CalculatePath(end);
                }

                openList.Remove(currentNode);
                closedSet.Add(currentNode);

                foreach (Pair<Node, int> neighbourDistancePair in _neighboursDistances(currentNode))
                {
                    Node neighbour = neighbourDistancePair.First;
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
            return null;
        }

        private void UpdateCosts(Node currentNode, Node neighbour, int distance, Node end)
        {
            int potentialNewGCost = _gScore[currentNode] + distance;

            if (potentialNewGCost < _gScore[neighbour])
            {
                _parents[neighbour] = currentNode;
                _gScore[neighbour] = potentialNewGCost;
                _fScore[neighbour] = _gScore[neighbour] + _hScore[neighbour];
            }
        }
        

        private List<Node> CalculatePath(Node end)
        {
            List<Node> path = new List<Node>();
            Node currentNode = end;
            while (_parents[currentNode] != null)
            {
                path.Insert(0, currentNode);
                currentNode = _parents[currentNode];
            }
            path.Insert(0, currentNode);
            return path;
        }

        private Node GetLowestCostNode(List<Node> openList)
        {
            Node lowestNode = openList[0];
            foreach (Node node in openList)
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
    }
}
