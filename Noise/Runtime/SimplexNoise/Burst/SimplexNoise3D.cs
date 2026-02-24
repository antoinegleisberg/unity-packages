using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;


namespace antoinegleisberg.Noise.Simplex.Burst
{
    [BurstCompile]
    internal struct SimplexNoise3DParallelJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<byte> PermutationTable;
        [ReadOnly] public NativeArray<float3> Input;
        [WriteOnly] public NativeArray<float> Output;

        private const int _permutationTableSize = 256;
        private const float F = 1f / 3f;
        private const float G = 1f / 6f;

        private const float _scalingFactor = 32f;  // To get noise in the range [-1, 1]

        public void Execute(int index)
        {
            float3 vec = Input[index];

            // Skew the input coordinates
            float skewingFactor = (vec.x + vec.y + vec.z) * F;
            float3 skewedCoords = vec + skewingFactor;

            // Calculate the containing cell's origin coordinates
            int3 ijk = (int3)math.floor(skewedCoords);

            // Calculate the input point's local coordinates in the simplex by unskewing the skewed offset coordinates
            float unskewingFactor = (ijk.x + ijk.y + ijk.z) * G;
            float3 d0 = vec - ((float3)ijk - unskewingFactor);

            // Calculate the order in which we should traverse the simplex to add contributions of the correct corners
            int3 i1, i2;
            if (d0.x >= d0.y)
            {
                if (d0.y >= d0.z) { i1 = new int3(1, 0, 0); i2 = new int3(1, 1, 0); } // X Y Z order
                else if (d0.x >= d0.z) { i1 = new int3(1, 0, 0); i2 = new int3(1, 0, 1); } // X Z Y order
                else { i1 = new int3(0, 0, 1); i2 = new int3(1, 0, 1); } // Z X Y order
            }
            else
            {
                if (d0.y < d0.z) { i1 = new int3(0, 0, 1); i2 = new int3(0, 1, 1); } // Z Y X order
                else if (d0.x < d0.z) { i1 = new int3(0, 1, 0); i2 = new int3(0, 1, 1); } // Y Z X order
                else { i1 = new int3(0, 1, 0); i2 = new int3(1, 1, 0); } // Y X Z order
            }

            // Calculate the coordinates of the other four corners of the 3D simplex (i.e. a tetrahedron)
            float3 d1 = d0 - (float3)i1 + G;
            float3 d2 = d0 - (float3)i2 + 2.0f * G;
            float3 d3 = d0 - 1.0f + 3.0f * G;

            // Apply a modulo operation on the cell base coords to make sure they are in the permutation table's range
            int ii = ijk.x & (_permutationTableSize - 1);
            int jj = ijk.y & (_permutationTableSize - 1);
            int kk = ijk.z & (_permutationTableSize - 1);

            // Calculate the contributions of the four corners
            float n0 = GetContribution(d0, PermutationTable[ii + PermutationTable[jj + PermutationTable[kk]]]);
            float n1 = GetContribution(d1, PermutationTable[ii + i1.x + PermutationTable[jj + i1.y + PermutationTable[kk + i1.z]]]);
            float n2 = GetContribution(d2, PermutationTable[ii + i2.x + PermutationTable[jj + i2.y + PermutationTable[kk + i2.z]]]);
            float n3 = GetContribution(d3, PermutationTable[ii + 1 + PermutationTable[jj + 1 + PermutationTable[kk + 1]]]);

            Output[index] = (n0 + n1 + n2 + n3) * _scalingFactor;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float GetContribution(float3 d, int hash)
        {
            // The 3D kernel radius is usually 0.6
            float t = 0.6f - math.lengthsq(d);
            // if (t < 0) return 0;
            t = math.max(0, t);  // Avoid branching for performance

            t *= t;
            return t * t * math.dot(GetGradient3D(hash), d);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float3 GetGradient3D(int hash)
        {
            // Standard 12 gradients for 3D Simplex
            int h = hash & 15;

            // Note: For Burst, a simple switch is often clearer and faster
            switch (h)
            {
                case 0: return new float3(1, 1, 0);
                case 1: return new float3(-1, 1, 0);
                case 2: return new float3(1, -1, 0);
                case 3: return new float3(-1, -1, 0);
                case 4: return new float3(1, 0, 1);
                case 5: return new float3(-1, 0, 1);
                case 6: return new float3(1, 0, -1);
                case 7: return new float3(-1, 0, -1);
                case 8: return new float3(0, 1, 1);
                case 9: return new float3(0, -1, 1);
                case 10: return new float3(0, 1, -1);
                case 11: return new float3(0, -1, -1);
                // Duplicate cases to fill the 4-bit hash (h < 16)
                case 12: return new float3(1, 1, 0);
                case 13: return new float3(-1, 1, 0);
                case 14: return new float3(0, -1, 1);
                case 15: return new float3(0, -1, -1);
                default: return new float3(0, 0, 0);
            }
        }
    }
}
