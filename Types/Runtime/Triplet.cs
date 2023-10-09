
namespace antoinegleisberg.Types
{
    [System.Serializable]
    public class Triplet<T1, T2, T3>
    {
        public T1 First;
        public T2 Second;
        public T3 Third;

        public Triplet(T1 first, T2 second, T3 third)
        {
            First = first;
            Second = second;
            Third = third;
        }

        public override string ToString()
        {
            return $"Triplet({First}, {Second}, {Third})";
        }
    }
}
