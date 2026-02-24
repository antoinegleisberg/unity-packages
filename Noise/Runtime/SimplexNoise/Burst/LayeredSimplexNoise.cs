using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;


namespace antoinegleisberg.Noise.Simplex.Burst
{

    public class LayeredSimplexNoise : IDisposable
    {
        private struct NoiseLayer
        {
            public float Amplitude;
            public float Frequency;
            public int Seed;
        }

        private List<NoiseLayer> _layers;

        public void AddLayer(float amplitude, float frequency, int seed)
        {
            _layers.Add(new NoiseLayer() { Amplitude = amplitude, Frequency = frequency, Seed = seed });
        }

        public void Dispose()
        {
            CachedSimplexNoiseGenerator.Instance.Dispose();
        }

        public float[] Generate1D(float[] input)
        {
            float[] output = new float[input.Length];

            for (int i = 0; i < _layers.Count; i++)
            {
                NativeArray<float> layerScaledInput = new NativeArray<float>(input.Length, Allocator.TempJob);
                NativeArray<float> layerOutput = new NativeArray<float>(input.Length, Allocator.TempJob);

                for (int j = 0; j < input.Length; j++)
                {
                    layerScaledInput[j] = input[j] * _layers[i].Frequency;
                }

                // Generate noise
                CachedSimplexNoiseGenerator.Instance.Schedule1D(
                    _layers[i].Seed,
                    layerScaledInput,
                    layerOutput).Complete();

                for (int j = 0; j < input.Length; j++)
                {
                    output[j] += layerOutput[j] * _layers[i].Amplitude;
                }

                layerScaledInput.Dispose();
                layerOutput.Dispose();
            }

            return output;
        }

        public float[] Generate2D(float2[] input)
        {
            float[] output = new float[input.Length];

            for (int i = 0; i < _layers.Count; i++)
            {
                NativeArray<float2> layerScaledInput = new NativeArray<float2>(input.Length, Allocator.TempJob);
                NativeArray<float> layerOutput = new NativeArray<float>(input.Length, Allocator.TempJob);

                for (int j = 0; j < input.Length; j++)
                {
                    layerScaledInput[j] = input[j] * _layers[i].Frequency;
                }

                // Generate noise
                CachedSimplexNoiseGenerator.Instance.Schedule2D(
                    _layers[i].Seed,
                    layerScaledInput,
                    layerOutput).Complete();

                for (int j = 0; j < input.Length; j++)
                {
                    output[j] += layerOutput[j] * _layers[i].Amplitude;
                }

                layerScaledInput.Dispose();
                layerOutput.Dispose();
            }

            return output;
        }

        public float[] Generate3D(float3[] input)
        {
            float[] output = new float[input.Length];

            for (int i = 0; i < _layers.Count; i++)
            {
                NativeArray<float3> layerScaledInput = new NativeArray<float3>(input.Length, Allocator.TempJob);
                NativeArray<float> layerOutput = new NativeArray<float>(input.Length, Allocator.TempJob);

                for (int j = 0; j < input.Length; j++)
                {
                    layerScaledInput[j] = input[j] * _layers[i].Frequency;
                }

                // Generate noise
                CachedSimplexNoiseGenerator.Instance.Schedule3D(
                    _layers[i].Seed,
                    layerScaledInput,
                    layerOutput).Complete();

                for (int j = 0; j < input.Length; j++)
                {
                    output[j] += layerOutput[j] * _layers[i].Amplitude;
                }

                layerScaledInput.Dispose();
                layerOutput.Dispose();
            }

            return output;
        }
    }
}
