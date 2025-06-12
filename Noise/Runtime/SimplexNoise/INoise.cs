using System.Collections.Generic;
using UnityEngine;

namespace antoinegleisberg.Noise.Simplex
{
    public interface INoise
    {
        public float Generate(List<float> coordinates);

        public float Generate2D(float x, float y) => Generate(new List<float>() { x, y });
    
        public float Generate3D(Vector3 coordinates) => Generate(new List<float>() { coordinates.x, coordinates.y, coordinates.z });
    }
}
