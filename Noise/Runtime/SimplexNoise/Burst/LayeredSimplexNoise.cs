using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;


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
            NativeArray<float> inputArray = new NativeArray<float>(input, Allocator.TempJob);
            NativeArray<float> outputArray = new NativeArray<float>(input.Length, Allocator.TempJob);

            NativeArray<float> amplitudes = new NativeArray<float>(_layers.Count, Allocator.TempJob);
            for (int i = 0; i < _layers.Count; i++)
            {
                amplitudes[i] = _layers[i].Amplitude;
            }

            NativeArray<NativeArray<float>> inputs = new NativeArray<NativeArray<float>>(_layers.Count, Allocator.TempJob);
            NativeArray<NativeArray<float>> outputs = new NativeArray<NativeArray<float>>(_layers.Count, Allocator.TempJob);
            NativeArray<JobHandle> handles = new NativeArray<JobHandle>(_layers.Count, Allocator.TempJob);


            for (int i = 0; i < _layers.Count; i++)
            {
                NativeArray<float> layerScaledInput = new NativeArray<float>(input.Length, Allocator.TempJob);
                NativeArray<float> layerOutput = new NativeArray<float>(input.Length, Allocator.TempJob);

                // Scale input
                Scale1DInputParallelJob scale1DInputParallelJob = new Scale1DInputParallelJob()
                {
                    Input = inputArray,
                    Frequency = _layers[i].Frequency,
                    Output = layerScaledInput
                };

                // Generate noise
                JobHandle handle = CachedSimplexNoiseGenerator.Instance.Schedule1D(
                    _layers[i].Seed,
                    layerScaledInput,
                    layerOutput,
                    scale1DInputParallelJob.Schedule(inputArray.Length, 128));

                inputs[i] = (layerScaledInput);
                outputs[i] = (layerOutput);
                handles[i] = (handle);
            }

            // Combine outputs
            JobHandle combinedHandle = JobHandle.CombineDependencies(handles);

            Sum1DNoiseParallelJob sum1DNoiseParallelJob = new Sum1DNoiseParallelJob()
            {
                Inputs = inputs,
                Amplitudes = amplitudes,
                Output = outputArray
            };
            sum1DNoiseParallelJob.Schedule(inputArray.Length, 128, combinedHandle).Complete();

            float[] output = outputArray.ToArray();

            inputArray.Dispose();
            outputArray.Dispose();
            amplitudes.Dispose();
            foreach (var arr in inputs)
            {
                arr.Dispose();
            }
            foreach (var arr in outputs)
            {
                arr.Dispose();
            }
            inputs.Dispose();
            outputs.Dispose();

            return output;
        }
    }

    [BurstCompile]
    internal struct Scale1DInputParallelJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float> Input;
        [ReadOnly] public float Frequency;
        [WriteOnly] public NativeArray<float> Output;
        public void Execute(int index)
        {
            Output[index] = Input[index] * Frequency;
        }
    }

    [BurstCompile]
    internal struct Sum1DNoiseParallelJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<NativeArray<float>> Inputs;
        [ReadOnly] public NativeArray<float> Amplitudes;
        [WriteOnly] public NativeArray<float> Output;
        public void Execute(int index)
        {
            float sum = 0f;
            for (int i = 0; i < Inputs.Length; i++)
            {
                sum += Inputs[i][index] * Amplitudes[i];
            }
            Output[index] = sum;
        }
    }
}
