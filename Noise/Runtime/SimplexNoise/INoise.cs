using System.Collections.Generic;

namespace antoinegleisberg.Noise.Simplex
{
    public interface INoise
    {
        public float Generate(List<float> coordinates, List<float> offsets = null);

        public float Generate2D(float x, float y, float offsetX = 0f, float offsetY = 0f) => Generate(new List<float>() { x, y }, new List<float>() { offsetX, offsetY });
    }
}
