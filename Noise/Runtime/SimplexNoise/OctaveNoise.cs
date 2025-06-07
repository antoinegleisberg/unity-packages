using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace antoinegleisberg.Noise.Simplex
{
    public class OctaveNoise : INoise
    {
        private int _octaves;
        private float _initialAmplitude;
        private float _initialFrequency;
        private float _persistence;
        private float _lacunarity;
        private int _seed;

        public OctaveNoise(int octaves, float initialAmplitude, float initialFrequency, float persistence, float lacunarity, int seed)
        {
            _octaves = octaves;
            _initialAmplitude = initialAmplitude;
            _initialFrequency = initialFrequency;
            _persistence = persistence;
            _lacunarity = lacunarity;
            _seed = seed;
        }
        
        public float Generate(List<float> coordinates, List<float> offsets = null)
        {
            float totalNoise = 0;
            float currentAmplitude = _initialAmplitude;
            float currentFrequency = _initialFrequency;

            List<SimplexNoise> noiseSources = new List<SimplexNoise>();
            for (int seed = _seed; seed < _octaves + _seed; seed++)
            {
                noiseSources.Add(new SimplexNoise(seed));
            }

            if (offsets == null) offsets = Enumerable.Repeat(0f, coordinates.Count).ToList();

            List<float> offsetCoords = coordinates.Zip(offsets, (coord, off) => coord + off).ToList();

            for (int i = 0; i < _octaves; i++)
            {
                List<float> scaledCoords = offsetCoords.Select(coord => coord * currentFrequency).ToList();
                float noise = noiseSources[i].CalculateNd(scaledCoords);
                totalNoise += noise * currentAmplitude;
                currentAmplitude *= _persistence;
                currentFrequency *= _lacunarity;
            }

            return totalNoise;
        }
    }
}
