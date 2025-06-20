using System.Collections.Generic;
using UnityEngine;

namespace antoinegleisberg.Noise.Simplex
{
    public interface INoise
    {
        public float Generate(List<float> coordinates);

        public float Generate1D(float x);

        public float Generate2D(float x, float y);

        public float Generate3D(float x, float y, float z);

        public float Generate2D(Vector2 coordinates) => Generate2D(coordinates.x, coordinates.y);

        public float Generate3D(Vector3 coordinates) => Generate3D(coordinates.x, coordinates.y, coordinates.z);
    }
}
