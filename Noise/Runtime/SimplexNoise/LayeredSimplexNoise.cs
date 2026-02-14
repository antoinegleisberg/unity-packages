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

        // ToDo: need to optimise this (by burst compiling and/or generating an array at once?)
        public float Generate1D(float x)
        {
            float totalNoise = 0f;
            foreach (NoiseLayer layer in _layers)
            {
                totalNoise += layer.Amplitude * layer.NoiseFunction.Generate1D(layer.Frequency * x);
            }
            return totalNoise;
        }

        // ToDo: need to optimise this (by burst compiling and/or generating an array at once?)
        public float Generate2D(float x, float y)
        {
            float totalNoise = 0f;
            foreach (NoiseLayer layer in _layers)
            {
                totalNoise += layer.Amplitude * layer.NoiseFunction.Generate2D(layer.Frequency * x, layer.Frequency * y);
            }
            return totalNoise;
        }

        // ToDo: need to optimise this (by burst compiling and/or generating an array at once?)
        public float Generate3D(float x, float y, float z)
        {
            float totalNoise = 0f;
            foreach (NoiseLayer layer in _layers)
            {
                totalNoise += layer.Amplitude * layer.NoiseFunction.Generate3D(layer.Frequency * x, layer.Frequency * y, layer.Frequency * z);
            }
            return totalNoise;
        }
    }
}
