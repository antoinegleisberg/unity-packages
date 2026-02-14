using antoinegleisberg.Types;
using System;
using System.Collections.Generic;

using Utils;

namespace antoinegleisberg.Pathfinding
{
    internal class AStar<TNode> : IPathfindingStrategy<TNode> where TNode : class
    {
        private Dictionary<TNode, int> _gScore; // cost to reach that TNode
        private Dictionary<TNode, int> _hScore; // heuristic distance to end
        private Dictionary<TNode, TNode> _parents;
        
        private Func<TNode, List<TNode>, int> _heuristicDistance;
        private Func<TNode, IEnumerable<ValueTuple<TNode, int>>> _neighboursDistances; // For each neighbour, the distance to it

        public AStar(Func<TNode, TNode, int> heuristicDistance, Func<TNode, IEnumerable<ValueTuple<TNode, int>>> neighbours)
        {
            _heuristicDistance = ConvertHeuristicDistance(heuristicDistance);
            _neighboursDistances = neighbours;

            _gScore = new Dictionary<TNode, int>();
            _hScore = new Dictionary<TNode, int>();
            _parents = new Dictionary<TNode, TNode>();
        }

        public List<TNode> FindPath(TNode start, TNode end)
        {
            return FindPath(new List<TNode>() { start }, new List<TNode>() { end });
        }

        public List<TNode> FindPath(List<TNode> startNodes, List<TNode> endNodes)
        {
            PriorityQueue<TNode, PathPriority> openSet = new PriorityQueue<TNode, PathPriority>();  // min-heap, smallest priority first
            HashSet<TNode> closedSet = new HashSet<TNode>();

            _gScore.Clear();
            _hScore.Clear();
            _parents.Clear();

            foreach (TNode startNode in startNodes)
            {
                int g = 0;
                int h = _heuristicDistance(startNode, endNodes);
                int f = g + h;
                _gScore.Add(startNode, g);
                _hScore.Add(startNode, h);
                _parents.Add(startNode, null);

                openSet.Enqueue(startNode, new PathPriority(f, h));
            }

            while (openSet.Count > 0)
            {
                TNode currentNode = openSet.Dequeue();

                if (endNodes.Contains(currentNode))
                {
                    return CalculatePath(currentNode);
                }

                // If the node has already been processed, skip it
                if (!closedSet.Add(currentNode)) continue;

                foreach ((TNode neighbour, int distance) in  _neighboursDistances(currentNode))
                {
                    if (closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int tentativeGScore = _gScore[currentNode] + distance;

                    // If we haven't seen this neighbor OR found a faster way to it
                    if (!_gScore.ContainsKey(neighbour) || tentativeGScore < _gScore[neighbour])
                    {
                        _parents[neighbour] = currentNode;
                        _gScore[neighbour] = tentativeGScore;
                        int h = _heuristicDistance(neighbour, endNodes);
                        _hScore[neighbour] = h;

                        int fScore = tentativeGScore + h;  // fScore is f + g

                        // We may enqueue duplicates here. The while loop checks for duplicates in the closed set.
                        openSet.Enqueue(neighbour, new PathPriority(fScore, h));
                    }
                }
            }
            
            // No path was found, the two nodes are not connected
            return null;
        }
        

        private List<TNode> CalculatePath(TNode end)
        {
            List<TNode> path = new List<TNode>();
            TNode currentNode = end;
            while (currentNode != null)
            {
                path.Add(currentNode);
                if (!_parents.TryGetValue(currentNode, out currentNode))
                {
                    currentNode = null;
                }
            }
            path.Reverse();
            return path;
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
    

        private struct PathPriority : IComparable<PathPriority>
        {
            public int F;
            public int H;

            public PathPriority(int f, int h)
            {
                F = f;
                H = h;
            }

            public int CompareTo(PathPriority other)
            {
                // First, compare F scores (Main priority)
                int result = F.CompareTo(other.F);
                if (result == 0)
                {
                    // If F is equal, compare H scores (Tie-breaker)
                    result = H.CompareTo(other.H);
                }
                return result;
            }
        }
    }
}
