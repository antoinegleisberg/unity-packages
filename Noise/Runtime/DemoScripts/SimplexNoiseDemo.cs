using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using antoinegleisberg.Noise.Simplex;
using antoinegleisberg.Types;

namespace antoinegleisberg.Noise.Demo
{
    public class SimplexNoiseDemo : MonoBehaviour
    {
        public enum DrawMode
        {
            Z,
            ColoredSpheres,
            Lines
        }

        [SerializeField] private List<Pair<float, float>> _amplitudeFrequencyPairs = new List<Pair<float, float>>() { new Pair<float, float>(1, 0.2f) };
        [SerializeField] private DrawMode _drawMode = DrawMode.Z;
        [SerializeField] private bool _overTime = false;
        [SerializeField] private int _seed = 1;
        [SerializeField] private int _gridSizeX = 25;
        [SerializeField] private int _gridSizeZ = 25;
        [SerializeField] private float _timeScale = 1f;
        [SerializeField] private float _noiseScale = 1f;
        [SerializeField] private float _drawScale = 1f;

        private float[,] values2D;
        
        private IEnumerator Start()
        {
            values2D = new float[_gridSizeX, _gridSizeZ];
            Generate2D(_timeScale * Time.time);
            while (true)
            {
                if (_overTime)
                    Generate2D(_timeScale * Time.time);
                yield return null;
            }
        }

        private void Generate2D(float time)
        {
            LayeredSimplexNoise layeredSimplexNoise = new LayeredSimplexNoise();
            
            int i = 0;
            foreach (Pair<float, float> amplFreq in _amplitudeFrequencyPairs)
            {
                float amplitude = amplFreq.First;
                float frequency = amplFreq.Second;
                layeredSimplexNoise.AddLayer(amplitude, frequency, _seed + ++i);
            }

            for (int x = 0; x < _gridSizeX; x++)
            {
                for (int z = 0; z < _gridSizeZ; z++)
                {
                    float y = layeredSimplexNoise.Generate(new List<float>() { x / _noiseScale, z / _noiseScale }, new List<float>() { time, 0 });
                    values2D[x, z] = y;
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (values2D != null)
            {
                for (int x = 0; x < _gridSizeX; x++)
                {
                    for (int z = 0; z < _gridSizeZ; z++)
                    {
                        float y = values2D[x, z] / 2f + 0.5f;
                        switch (_drawMode)
                        {
                            case DrawMode.Z:
                                Gizmos.DrawSphere(new Vector3(_drawScale * x, y, _drawScale * z), 0.15f);
                                break;
                            case DrawMode.ColoredSpheres:
                                Gizmos.color = new Color(y, y, y);
                                Gizmos.DrawSphere(new Vector3(_drawScale * x, 0, _drawScale * z), 0.15f);
                                break;
                            case DrawMode.Lines:
                                if (x + 1 < _gridSizeX)
                                {
                                    float yx = values2D[x + 1, z];
                                    Gizmos.DrawLine(new Vector3(x, y, z), new Vector3(x + 1, yx, z));
                                }
                                if (z + 1 < _gridSizeZ)
                                {
                                    float yz = values2D[x, z + 1];
                                    Gizmos.DrawLine(new Vector3(x, y, z), new Vector3(x, yz, z + 1));
                                }
                                break;
                        }
                    }
                }
            }
        }
    }
}
