
using System.Collections.Generic;

namespace antoinegleisberg.Types
{
    [System.Serializable]
    public struct Pair<T1, T2>
    {
        public T1 First;
        public T2 Second;

        public Pair(T1 first, T2 second)
        {
            First = first;
            Second = second;
        }

        public override string ToString()
        {
            return $"Pair({First}, {Second})";
        }
    }
}
