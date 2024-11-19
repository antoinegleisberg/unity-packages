using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace antoinegleisberg.Math.ProceduralGeneration
{
    public class LayeredSimplexNoise
    {
        private BaseSimplexNoise _baseNoise;

        public LayeredSimplexNoise(int seed)
        {
            _baseNoise = new BaseSimplexNoise(seed);
        }

        public float Generate(List<float> coordinates, int octaves = 1, float amplitude = 1f, float persistency = 0.6f, float frequencyGain = 0.5f, List<float> offsets = null, float initialFrequency = 1.0f)
        {
            if (offsets == null)
            {
                offsets = new List<float>(coordinates.Count);
                for (int i = 0; i < coordinates.Count; i++)
                {
                    offsets.Add(0);
                }
            }

            if (coordinates.Count != offsets.Count)
            {
                throw new ArgumentException("Coords and offsets need to have the same dimension");
            }

            float totalNoise = 0.0f;
            float currentPersistency = 1.0f;

            List<float> offsetCoordinates = coordinates
                    .Select((value, index) => index)
                    .Select(idx => coordinates[idx] + offsets[idx])
                    .ToList();

            for (int i = 0; i < octaves; i++)
            {
                List<float> scaledCoords = offsetCoordinates.Select(x => x * initialFrequency).ToList();

                totalNoise += currentPersistency * _baseNoise.CalculateNd(scaledCoords);

                initialFrequency *= frequencyGain;
                currentPersistency *= persistency;
            }

            // Rescale the noise to be between [-1, 1] (which it is not due to summation)
            float rescalingFactor = (1 - persistency) / (1 - Mathf.Pow(persistency, octaves));
            totalNoise *= rescalingFactor;

            // We can then still apply amplitude factor
            return amplitude * totalNoise;
        }
    }
}
