using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;


namespace antoinegleisberg.Noise.Simplex.Burst
{
    [BurstCompile]
    internal struct SimplexNoise1DParallelJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<byte> PermutationTable;
        [ReadOnly] public NativeArray<float> Input;
        [WriteOnly] public NativeArray<float> Output;

        // The maximum value of this noise is 8 * (1 - (1/2)^2)^4 = 81 / 32 = 2.53125
        // We thus multiply by the inverse of that to get a noise in the range [-1, 1]
        private const float _scalingFactor = 32f / 81f;

        public void Execute(int index)
        {
            float x = Input[index];

            // i0, i1 successive integers such that:
            // i0 <= x < i1
            int i0 = (int)math.floor(x);
            int i1 = i0 + 1;

            // x0 is the vector i0 -> x
            float x0 = x - i0;

            // x1 is the vector i1 -> x
            float x1 = x - i1;

            float n0 = SmoothingFunction1D(x0) * RandomGrad1D(PermutationTable[i0 & 0xff]) * x0;
            float n1 = SmoothingFunction1D(x1) * RandomGrad1D(PermutationTable[i1 & 0xff]) * x1;

            Output[index] = (n0 + n1) * _scalingFactor;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float SmoothingFunction1D(float t)
        {
            t *= t;
            t = 1f - t;
            t *= t;
            return t * t;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float RandomGrad1D(int hash)
        {
            int h = hash & 0b1111;
            float grad = 1f + (h & 0b111);
            return (h & 0b1000) != 0 ? -grad : grad;
        }
    }
}
