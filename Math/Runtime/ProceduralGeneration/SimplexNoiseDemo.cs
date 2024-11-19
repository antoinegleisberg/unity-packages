using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace antoinegleisberg.Math.ProceduralGeneration
{
    public class SimplexNoiseDemo : MonoBehaviour
    {
        [SerializeField] private int _seed = 1;
        [SerializeField] private int _octaves = 4;
        [Range(1, 1000)]
        [SerializeField] private float _frequency = 0.1f;
        [SerializeField] private float _amplitude = 1.0f;
        [Range(0, 1)]
        [SerializeField] private float _persistence = 0.5f;

        [SerializeField] private float _scale = Mathf.Exp(1);

        [SerializeField] private int debugSize = 100;
        
        [SerializeField] private bool draw2D;
        [SerializeField] private Vector2 offset;
        private float[,] values2D;

        [SerializeField] private float timeScale;

        private IEnumerator Start()
        {
            values2D = new float[debugSize, debugSize];
            while (true)
            {
                Generate2D(timeScale * Time.time);
                yield return null;
            }
        }

        private void Generate2D(float time)
        {
            LayeredSimplexNoise layeredSimplexNoise = new LayeredSimplexNoise(_seed);

            if (draw2D)
            {
                for (int x = 0; x < debugSize; x++)
                {
                    for (int y = 0; y < debugSize; y++)
                    {
                        // float z = simplexNoise.CalculateNd(new List<float>() { 0.2f * x * _scale, 0.2f * y * _scale });
                        float z = layeredSimplexNoise.Generate(
                            new List<float>() { 0.2f * x * _scale, 0.2f * y * _scale, time },
                            _octaves, _amplitude, _persistence, _frequency, new List<float>() { offset.x, offset.y, 0 });
                        values2D[x, y] = z;
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (draw2D && values2D != null)
            {
                for (int x = 0; x < debugSize; x++)
                {
                    for (int y = 0; y < debugSize; y++)
                    {
                        float z = values2D[x, y];
                        z = (1 + z) / 2;
                        Gizmos.color = new Color(z, z, z);
                        Gizmos.DrawSphere(new Vector3(0.2f * x, 0.2f * y, 0), 0.15f);
                    }
                }
            }
        }
    }
}
