using System.Collections.Generic;

namespace antoinegleisberg.Noise.Simplex
{
    public interface INoise
    {
        public float Generate(List<float> coordinates);

        public float Generate2D(float x, float y) => Generate(new List<float>() { x, y });
    }
}
