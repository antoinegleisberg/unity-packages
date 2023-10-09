

using System;
using System.Collections.Generic;

namespace antoinegleisberg.Pathfinding
{
    internal class Node
    {
        private static List<Guid> _guids;

        public Guid Guid { get; private set; }

        public Node()
        {
            Guid = Guid.NewGuid();
            while (_guids.Contains(Guid))
            {
                Guid = Guid.NewGuid();
            }
            _guids.Add(Guid);
        }

        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }
    }
}
