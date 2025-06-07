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
            
            public float Generate(List<float> coordinates, List<float> offset)
            {
                float frequency = Frequency;
                List<float> offsetScaledCoords =
                    coordinates
                    .Zip(offset, (coord, off) => frequency * (coord + off))
                    .ToList();

                return Amplitude * NoiseFunction.CalculateNd(offsetScaledCoords);
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

        public float Generate(List<float> coordinates, List<float> offsets = null)
        {
            if (offsets == null)
            {
                offsets = Enumerable.Repeat(0f, coordinates.Count).ToList();
            }
            
            if (offsets.Count != coordinates.Count)
            {
                throw new ArgumentException("Coords and offsets need to have the same dimension");
            }

            return _layers
                .Select((layer) => layer.Generate(coordinates, offsets))
                .Sum();
        }
    }
}
