using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;


namespace antoinegleisberg.Noise.Simplex.Burst
{
    internal class CachedSimplexNoiseGenerator : IDisposable
    {
        private static CachedSimplexNoiseGenerator _generator;

        private Dictionary<int, SimplexNoiseGenerator> _cache = new Dictionary<int, SimplexNoiseGenerator>();

        private CachedSimplexNoiseGenerator(){}

        internal static CachedSimplexNoiseGenerator Instance
        {
            get
            {
                if (_generator == null)
                {
                    _generator = new CachedSimplexNoiseGenerator();
                }
                return _generator;
            }
        }

        public void Dispose()
        {
            foreach (var generator in _cache.Values)
            {
                generator.Dispose();
            }
            _cache.Clear();
        }

        private SimplexNoiseGenerator GetNoiseGenerator(int seed)
        {
            if (!_cache.TryGetValue(seed, out SimplexNoiseGenerator generator))
            {
                generator = new SimplexNoiseGenerator(seed);
                _cache.Add(seed, generator);
            }
            return generator;
        }

        internal JobHandle Schedule1D(int seed, NativeArray<float> input, NativeArray<float> output, JobHandle dependency = default)
        {
            return GetNoiseGenerator(seed).Schedule1D(input, output, dependency);
        }

        internal JobHandle Schedule2D(int seed, NativeArray<float2> input, NativeArray<float> output, JobHandle dependency = default)
        {
            return GetNoiseGenerator(seed).Schedule2D(input, output, dependency);
        }

        internal JobHandle Schedule3D(int seed, NativeArray<float3> input, NativeArray<float> output, JobHandle dependency = default)
        {
            return GetNoiseGenerator(seed).Schedule3D(input, output, dependency);
        }
    }
}
