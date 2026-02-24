using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

using Random = System.Random;


namespace antoinegleisberg.Noise.Simplex.Burst
{
    internal class SimplexNoiseGenerator : IDisposable
    {
        private static readonly int _permutationTableSize = 256;  // If editing, change also in the job structs
        private NativeArray<byte> _permutationTable;

        internal SimplexNoiseGenerator(int seed)
        {
            if (seed == 0)
            {
                seed = new Random().Next();
            }

            Random random = new Random(seed);
            // Note: this is technically incorrect, as random bytes does not give an actual permutation
            // Alternative is to create an array of integers from 0 to 255, shuffle it, and then convert it to bytes
            // This is usually fine though
            byte[] temp = new byte[_permutationTableSize];
            random.NextBytes(temp);
            
            // We want the 2nd half of the permutation table to be a copy of the first half
            // This allows for faster lookup during computation
            _permutationTable = new NativeArray<byte>(_permutationTableSize * 2, Allocator.Persistent);
            for (int i = 0; i < _permutationTableSize; i++)
            {
                _permutationTable[i] = temp[i];
                _permutationTable[i + _permutationTableSize] = temp[i];
            }
        }

        public void Dispose()
        {
            if (_permutationTable.IsCreated)
            {
                _permutationTable.Dispose();
            }
        }

        internal JobHandle Schedule1D(NativeArray<float> input, NativeArray<float> output, JobHandle dependency = default)
        {
            SimplexNoise1DParallelJob job = new SimplexNoise1DParallelJob()
            {
                Input = input,
                PermutationTable = _permutationTable,
                Output = output
            };
            return job.Schedule(input.Length, 64, dependency);
        }

        internal JobHandle Schedule2D(NativeArray<float2> input, NativeArray<float> output, JobHandle dependency = default)
        {
            SimplexNoise2DParallelJob job = new SimplexNoise2DParallelJob()
            {
                Input = input,
                PermutationTable = _permutationTable,
                Output = output
            };
            return job.Schedule(input.Length, 64, dependency);
        }

        internal JobHandle Schedule3D(NativeArray<float3> input, NativeArray<float> output, JobHandle dependency = default)
        {
            SimplexNoise3DParallelJob job = new SimplexNoise3DParallelJob()
            {
                Input = input,
                PermutationTable = _permutationTable,
                Output = output
            };
            return job.Schedule(input.Length, 64, dependency);
        }
    }
}
