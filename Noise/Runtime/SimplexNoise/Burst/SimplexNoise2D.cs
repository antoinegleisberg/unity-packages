using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;


namespace antoinegleisberg.Noise.Simplex.Burst
{
    [BurstCompile]
    internal struct SimplexNoise2DParallelJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<byte> PermutationTable;
        [ReadOnly] public NativeArray<float2> Input;
        [WriteOnly] public NativeArray<float> Output;

        private const int _permutationTableSize = 256;
        private const float F = 0.36602540378f;  // 0.5f * (MathF.Sqrt(3) - 1);
        private const float G = 0.2113248654f;   // (3 - MathF.Sqrt(3)) / 6;

        // float maxNoiseValue = MathF.Pow((0.5f - 0.5f / 9), 4) * MathF.Sqrt(0.5f / 9) * MathF.Sqrt(5) = 0.02056452474f;  // => only approximately correct?
        // float maxNoiseValue = 0.884343445f / 40f; ==> better?
        // Verify this scaling factor, end noise should be between [-1, 1]
        private const float _scalingFactor = 40f / 0.884343445f;

        public void Execute(int index)
        {
            float2 vec = Input[index];

            // Skew the input coordinates
            float skewingFactor = (vec.x + vec.y) * F;
            float2 skewedCoords = vec + skewingFactor;

            // Calculate the containing cell's origin coordinates
            int2 ij = (int2)math.floor(skewedCoords);

            // Calculate the input point's local coordinates in the simplex by unskewing the skewed offset coordinates
            // This corresponds to the first corner of the 2D simplex
            float unskewingFactor = (ij.x + ij.y) * G;
            float2 d0 = vec - ((float2)ij - unskewingFactor);

            // Calculate the order in which we should traverse the simplex to add contributions of the correct corners
            int2 i1 = (d0.x > d0.y) ? new int2(1, 0) : new int2(0, 1);

            // Calculate the coordinates of the other two corners of the 2D simplex (i.e. a triangle)
            float2 d1 = d0 - i1 + G;
            float2 d2 = d0 - 1 + 2 * G;

            // Apply a modulo operation on the cell base coords to make sure they are in the permutation table's range
            // For powers of 2, we can shortcut the modulo with a bitwise AND
            int ii = ij.x & (_permutationTableSize - 1);
            int jj = ij.y & (_permutationTableSize - 1);

            // Calculate the contributions of the three corners
            float n0 = GetContribution(d0, PermutationTable[ii + PermutationTable[jj]]);
            float n1 = GetContribution(d1, PermutationTable[ii + i1.x + PermutationTable[jj + i1.y]]);
            float n2 = GetContribution(d2, PermutationTable[ii + 1 + PermutationTable[jj + 1]]);

            Output[index] = (n0 + n1 + n2) * _scalingFactor;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float GetContribution(float2 d, int hash)
        {
            float t = 0.5f - math.lengthsq(d);
            // if (t < 0) return 0;
            t = math.max(0, t);  // Avoid branching for performance

            t *= t;
            return t * t * math.dot(RandomGradient2D(hash), d);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float2 RandomGradient2D(int hash)
        {
            // Burst turns this into a very fast jump table
            switch (hash & 0b111)
            {
                case 0: return new float2(1, 2);
                case 1: return new float2(2, 1);
                case 2: return new float2(-1, 2);
                case 3: return new float2(-2, 1);
                case 4: return new float2(1, -2);
                case 5: return new float2(2, -1);
                case 6: return new float2(-1, -2);
                case 7: return new float2(-2, -1);
                default: return new float2(0, 0); // Should never happen
            }
        }
    }
}
