using System;

namespace antoinegleisberg.Types
{
    public class VectorNd
    {
        private float[] _components;

        public VectorNd(int dimension)
        {
            _components = new float[dimension];
        }

        public VectorNd(params float[] components)
        {
            _components = components;
        }

        public static VectorNd Constant(int dimension, float value)
        {
            VectorNd vector = new VectorNd(dimension);
            for (int i = 0; i < dimension; i++)
            {
                vector[i] = value;
            }
            return vector;
        }

        public int Dimension => _components.Length;

        public float this[int index] {
            get => _components[index];
            set => _components[index] = value;
        }

        public static VectorNd operator +(VectorNd v1, VectorNd v2)
        {
            if (v1.Dimension != v2.Dimension)
            {
                throw new ArgumentException("Vectors must have the same dimension to be added together");
            }

            VectorNd result = new VectorNd(v1.Dimension);
            for (int i = 0; i < v1.Dimension; i++)
            {
                result[i] = v1[i] + v2[i];
            }

            return result;
        }

        public static VectorNd operator -(VectorNd v1, VectorNd v2)
        {
            if (v1.Dimension != v2.Dimension)
            {
                throw new ArgumentException("Vectors must have the same dimension to be subtracted from each other");
            }

            VectorNd result = new VectorNd(v1.Dimension);
            for (int i = 0; i < v1.Dimension; i++)
            {
                result[i] = v1[i] - v2[i];
            }

            return result;
        }

        public static VectorNd operator *(float scalar, VectorNd v)
        {
            VectorNd result = new VectorNd(v.Dimension);
            for (int i = 0; i < v.Dimension; i++)
            {
                result[i] = scalar * v[i];
            }

            return result;
        }

        public static VectorNd operator *(VectorNd v, float scalar)
        {
            return scalar * v;
        }
        
        public static float DotProduct(VectorNd v1, VectorNd v2)
        {
            if (v1.Dimension != v2.Dimension)
                throw new InvalidOperationException("Vectors must have the same dimension.");

            float result = 0.0f;
            for (int i = 0; i < v1.Dimension; i++)
            {
                result += v1[i] * v2[i];
            }
            return result;
        }
    }
}
