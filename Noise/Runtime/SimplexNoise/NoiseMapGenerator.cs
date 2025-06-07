using System;
using UnityEngine;

namespace antoinegleisberg.Noise.Simplex
{
    public static class NoiseMapGenerator
    {
        public static float[,] Generate2DMap(INoise noiseGenerator, int mapWidth, int mapHeight, float scale = 1, Vector2 offset = new Vector2())
        {
            if (scale <= 0) throw new ArgumentException("Parameter 'scale' must be greater than 0");

            float[,] noiseMap = new float[mapWidth, mapHeight];

            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    float sampleX = (x + offset.x) / scale;
                    float sampleY = (y + offset.y) / scale;
                    float noise = noiseGenerator.Generate2D(sampleX, sampleY);
                    // Debug.Log($"{x}, {y} => {noise}");
                    noiseMap[x, y] = noise;
                }
            }

            return noiseMap;
        }
    }
}
