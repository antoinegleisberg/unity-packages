using System;
using System.Collections.Generic;
using System.Linq;

namespace antoinegleisberg.Noise.Simplex
{
    public class LayeredSimplexNoise : INoise
    {
        private struct NoiseLayer
        {
            public float Amplitude;
            public float Frequency;
            public SimplexNoise NoiseFunction;
            
            public float Generate(List<float> coordinates)
            {
                float frequency = Frequency;
                List<float> scaledCoords =
                    coordinates
                    .Select(coord => frequency * coord)
                    .ToList();

                return Amplitude * NoiseFunction.CalculateNd(scaledCoords);
            }
        }

        private List<NoiseLayer> _layers;

        public LayeredSimplexNoise()
        {
            _layers = new List<NoiseLayer>();
        }

        public void AddLayer(float amplitude, float frequency, int seed)
        {
            _layers.Add(new NoiseLayer() { Amplitude = amplitude, Frequency = frequency, NoiseFunction = new SimplexNoise(seed) });
        }

        public float Generate(List<float> coordinates)
        {
            return _layers
                .Select((layer) => layer.Generate(coordinates))
                .Sum();
        }
    }
}
