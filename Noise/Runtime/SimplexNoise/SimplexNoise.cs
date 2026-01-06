using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using antoinegleisberg.Types;

using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;
using Random = System.Random;


namespace antoinegleisberg.Noise.Simplex
{
    internal class SimplexNoise
    {
        private static readonly int _permutationTableSize = 256;
        private byte[] _permutationTable;

        public SimplexNoise(int seed = 0)
        {
            if (seed == 0)
            {
                seed = new Random().Next();
            }

            _permutationTable = new byte[_permutationTableSize * 2];
            var random = new Random(seed);
            // Note: this is technically incorrect, as random bytes does not give an actual permutation
            // Alternative is to create an array of integers from 0 to 255, shuffle it, and then convert it to bytes
            random.NextBytes(_permutationTable);
            // We want the 2nd half of the permutation table to be a copy of the first half
            // This allows for faster lookup during computation
            for (int i = 0; i < _permutationTableSize; i++)
            {
                _permutationTable[i + _permutationTableSize] = _permutationTable[i];
            }
        }

        /// <summary>
        /// Generates a 1D Simplex noise value approximately between -1 and 1 based on the input x coordinate.
        /// This method is free of any Heap allocations.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public float Generate1D(float x)
        {
            // i0, i1 successive integers such that:
            // i0 <= x < i1
            int i0 = FastFloor(x);
            int i1 = i0 + 1;
            // x0 is the vector i0 -> x
            float x0 = x - i0;
            // x1 is the vector i1 -> x
            float x1 = x - i1;

            float n0 = SmoothingFunction1D(x0) * RandomGrad1D(_permutationTable[i0 & 0xff]) * x0;
            float n1 = SmoothingFunction1D(x1) * RandomGrad1D(_permutationTable[i1 & 0xff]) * x1;

            // The maximum value of this noise is 8 * (1 - (1/2)^2)^4 = 81 / 32 = 2.53125
            // We thus scale it between -1 and 1:
            return (n0 + n1) * 32 / 81;
        }

        /// <summary>
        /// Generates a 2D Simplex noise value approximately between -1 and 1 based on the input x and y coordinates.
        /// Returns the same result as GenerateNd(new List<float> { x, y }) but is computationally more efficient for 2D noise generation.
        /// This method is free of any Heap allocations.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public float Generate2D(float x, float y)
        {
            float F = 0.5f * (MathF.Sqrt(3) - 1);
            float G = (3 - MathF.Sqrt(3)) / 6;

            // Skew the input coordinates
            float skewingFactor = (x + y) * F;
            float skewedX = x + skewingFactor;
            float skewedY = y + skewingFactor;

            // Calculate the containing cell's origin coordinates
            int i = FastFloor(skewedX);
            int j = FastFloor(skewedY);

            // Calculate the input point's local coordinates in the simplex by unskewing the skewed offset coordinates
            // This corresponds to the first corner of the 2D simplex
            float unskewingFactor = (i + j) * G;
            float localX = x - (i - unskewingFactor);
            float localY = y - (j - unskewingFactor);

            // Apply a modulo operation on the cell base coords to make sure they are in the permutation table's range
            i = Mod(i, _permutationTableSize);
            j = Mod(j, _permutationTableSize);

            // Calculate the order in which we should traverse the simplex to add contributions of the correct corners
            int i1, j1;
            if (localX > localY) { i1 = 1; j1 = 0; }
            else { i1 = 0; j1 = 1; }

            // Calculate the coordinates of the other two corners of the 2D simplex (i.e. a triangle)
            float x1 = localX - i1 + G;
            float y1 = localY - j1 + G;
            float x2 = localX - 1 + 2 * G;
            float y2 = localY - 1 + 2 * G;

            // Calculate the contributions of the three corners
            float n0, n1, n2;

            float smoothingFactor0 = 0.5f - localX * localX - localY * localY;
            if (smoothingFactor0 < 0) n0 = 0f;
            else
            {
                float t0 = smoothingFactor0 * smoothingFactor0;
                Vector2 randomGradient = RandomGradient2D(_permutationTable[i + _permutationTable[j]]);
                float dotProduct0 = localX * randomGradient.X + localY * randomGradient.Y;
                n0 = t0 * t0 * dotProduct0;
            }

            float smoothingFactor1 = 0.5f - x1 * x1 - y1 * y1;
            if (smoothingFactor1 < 0) n1 = 0f;
            else
            {
                float t1 = smoothingFactor1 * smoothingFactor1;
                Vector2 randomGradient = RandomGradient2D(_permutationTable[i + i1 + _permutationTable[j + j1]]);
                float dotProduct1 = x1 * randomGradient.X + y1 * randomGradient.Y;
                n1 = t1 * t1 * dotProduct1;
            }

            float smoothingFactor2 = 0.5f - x2 * x2 - y2 * y2;
            if (smoothingFactor2 < 0) n2 = 0f;
            else
            {
                float t2 = smoothingFactor2 * smoothingFactor2;
                Vector2 randomGradient = RandomGradient2D(_permutationTable[i + 1 + _permutationTable[j + 1]]);
                float dotProduct2 = x2 * randomGradient.X + y2 * randomGradient.Y;
                n2 = t2 * t2 * dotProduct2;
            }

            float maxNoiseValue = MathF.Pow((0.5f - 0.5f / 9), 4) * MathF.Sqrt(0.5f / 9) * MathF.Sqrt(5);  // => only approximately correct?
            // float maxNoiseValue = 0.884343445f / 40f; ==> better? TODO

            return (n0 + n1 + n2) / maxNoiseValue;
        }

        /// <summary>
        /// Generates a 3D Simplex noise value approximately between -1 and 1 based on the input x, y, and z coordinates.
        /// Returns the same result as GenerateNd(new List<float> { x, y, z }) but is computationally more efficient for 3D noise generation.
        /// This method is free of any Heap allocations.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public float Generate3D(float x, float y, float z)
        {
            float F = 1f / 3f;
            float G = 1f / 6f;

            // Skew the input coordinates
            float skewingFactor = (x + y + z) * F;
            float skewedX = x + skewingFactor;
            float skewedY = y + skewingFactor;
            float skewedZ = z + skewingFactor;

            // Calculate the containing cell's origin coordinates
            int i = FastFloor(skewedX);
            int j = FastFloor(skewedY);
            int k = FastFloor(skewedZ);

            // Calculate the input point's local coordinates in the simplex by unskewing the skewed offset coordinates
            float unskewingFactor = (i + j + k) * G;
            float x0 = x - (i - unskewingFactor);  // called localX in calculateNd
            float y0 = y - (j - unskewingFactor);  // called localY in calculateNd
            float z0 = z - (k - unskewingFactor);  // called localZ in calculateNd

            // Apply a modulo operation on the cell base coords to make sure they are in the permutation table's range
            i = Mod(i, _permutationTableSize);
            j = Mod(j, _permutationTableSize);
            k = Mod(k, _permutationTableSize);

            // Calculate the order in which we should traverse the simplex to add contributions of the correct corners
            int i1, j1, k1;
            int i2, j2, k2;
            if (x0 >= y0)
            {
                if (y0 >= z0) { i1 = 1; j1 = 0; k1 = 0; i2 = 1; j2 = 1; k2 = 0; } // X Y Z order
                else if (x0 >= z0) { i1 = 1; j1 = 0; k1 = 0; i2 = 1; j2 = 0; k2 = 1; } // X Z Y order
                else { i1 = 0; j1 = 0; k1 = 1; i2 = 1; j2 = 0; k2 = 1; } // Z X Y order
            }
            else
            {
                if (y0 < z0) { i1 = 0; j1 = 0; k1 = 1; i2 = 0; j2 = 1; k2 = 1; } // Z Y X order
                else if (x0 < z0) { i1 = 0; j1 = 1; k1 = 0; i2 = 0; j2 = 1; k2 = 1; } // Y Z X order
                else { i1 = 0; j1 = 1; k1 = 0; i2 = 1; j2 = 1; k2 = 0; } // Y X Z order
            }

            // Calculate the coordinates of the other four corners of the 3D simplex (i.e. a tetrahedron)
            float x1 = x0 - i1 + G;
            float y1 = y0 - j1 + G;
            float z1 = z0 - k1 + G;
            float x2 = x0 - i2 + 2 * G;
            float y2 = y0 - j2 + 2 * G;
            float z2 = z0 - k2 + 2 * G;
            float x3 = x0 - 1 + 3 * G;
            float y3 = y0 - 1 + 3 * G;
            float z3 = z0 - 1 + 3 * G;

            // Calculate the contributions of the four corners
            float n0, n1, n2, n3;

            float smoothingFactor0 = 0.6f - x0 * x0 - y0 * y0 - z0 * z0;
            if (smoothingFactor0 < 0) n0 = 0;
            else
            {
                float t0 = smoothingFactor0 * smoothingFactor0;
                Vector3 randomGradient = RandomGradient3D(_permutationTable[i + _permutationTable[j + _permutationTable[k]]]);
                float dotProduct0 = x0 * randomGradient.X + y0 * randomGradient.Y + z0 * randomGradient.Z;
                n0 = t0 * t0 * dotProduct0;
            }

            float smoothingFactor1 = 0.6f - x1 * x1 - y1 * y1 - z1 * z1;
            if (smoothingFactor1 < 0) n1 = 0;
            else
            {
                float t1 = smoothingFactor1 * smoothingFactor1;
                Vector3 randomGradient = RandomGradient3D(_permutationTable[i + i1 + _permutationTable[j + j1 + _permutationTable[k + k1]]]);
                float dotProduct1 = x1 * randomGradient.X + y1 * randomGradient.Y + z1 * randomGradient.Z;
                n1 = t1 * t1 * dotProduct1;
            }

            float smoothingFactor2 = 0.6f - x2 * x2 - y2 * y2 - z2 * z2;
            if (smoothingFactor2 < 0) n2 = 0;
            else
            {
                float t2 = smoothingFactor2 * smoothingFactor2;
                Vector3 randomGradient = RandomGradient3D(_permutationTable[i + i2 + _permutationTable[j + j2 + _permutationTable[k + k2]]]);
                float dotProduct2 = x2 * randomGradient.X + y2 * randomGradient.Y + z2 * randomGradient.Z;
                n2 = t2 * t2 * dotProduct2;
            }

            float smoothingFactor3 = 0.6f - x3 * x3 - y3 * y3 - z3 * z3;
            if (smoothingFactor3 < 0) n3 = 0;
            else
            {
                float t3 = smoothingFactor3 * smoothingFactor3;
                Vector3 randomGradient = RandomGradient3D(_permutationTable[i + 1 + _permutationTable[j + 1 + _permutationTable[k + 1]]]);
                float dotProduct3 = x3 * randomGradient.X + y3 * randomGradient.Y + z3 * randomGradient.Z;
                n3 = t3 * t3 * dotProduct3;
            }

            float scalingFactor = CalculateScalingFactor(3);

            return scalingFactor * (n0 + n1 + n2 + n3);
        }

        /// <summary>
        /// Returns a noise value between approximately -1 and 1
        /// Can return values slightly larger than 1 or lower than -1
        /// </summary>
        /// <param name="X"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public float CalculateNd(List<float> X)
        {
            Debug.LogWarning("Using CalculateNd may allocate a lot of memory on the heap. Prefer to use Generate1D, Generate2D, or Generate3D for 1D, 2D, or 3D noise generation respectively.");

            // N is the number of dimensions
            int N = X.Count;

            if (N == 1)
            {
                // 1D Simplex noise is a special case: handle it separately
                return Generate1D(X[0]);
            }

            if (N >= _permutationTableSize)
            {
                throw new ArgumentException("Cannot generate noise on a dimension higher than the internal hash table size");
            }

            float F = (MathF.Sqrt(N + 1) - 1) / N;
            float G = (1 - 1 / MathF.Sqrt(N + 1)) / N;

            // Skew the input coordinates
            float skewingFactor = X.Sum() * F;
            List<float> skewedCoords = X.Select(xi => xi + skewingFactor).ToList();

            // Calculate the containing cell's origin coordinates
            List<int> cellBaseCoordinates = skewedCoords.Select(xi => FastFloor(xi)).ToList();

            // Calculate the input point's local coordinates in the simplex by unskewing the skewed offset coordinates
            float unskewingFactor = cellBaseCoordinates.Sum() * G;
            List<float> inputPointLocalCoordinates = new List<float>();
            for (int i = 0; i < N; i++)
            {
                inputPointLocalCoordinates.Add(X[i] - (cellBaseCoordinates[i] - unskewingFactor));
            }

            // Apply a modulo operation on the cell base coords to make sure they are in the permutation table's range
            cellBaseCoordinates = cellBaseCoordinates.Select(xi => Mod(xi, _permutationTableSize)).ToList();

            // Calculate the order in which we should traverse the simplex to add contributions of the correct corners
            List<int> decreasingMagnitudeLocalCoordinatesIndices = GetIndicesInDecreasingOrder(inputPointLocalCoordinates);

            // Initialise the simplexe's base corner coordinates
            VectorNd xArray = new VectorNd(inputPointLocalCoordinates.ToArray());
            VectorNd GVector = VectorNd.Constant(N, G);

            float totalContribution = 0.0f;
            int lastChangedDimension = -1;
            HashSet<int> changedDimensions = new HashSet<int>();
            for (int i = 0; i < N + 1; i++)
            {
                // Calculate contribution
                int gradientHash = GetGradientHash(N, changedDimensions, cellBaseCoordinates);
                VectorNd gradient = RandomGradientND(N, gradientHash);
                float dotProduct = VectorNd.DotProduct(xArray, gradient);
                float smoothing = SmoothingFunctionND(N, xArray);
                float ni = smoothing * dotProduct;
                totalContribution += ni;

                if (i < N)
                {
                    // Change xArray to represent the next vertex when walking along the simplex
                    lastChangedDimension = decreasingMagnitudeLocalCoordinatesIndices[i];
                    changedDimensions.Add(lastChangedDimension);
                    xArray[lastChangedDimension] -= 1; // = xArray[lastChangedDimension] - 1;
                    xArray = xArray + GVector;
                }
            }

            float scaledTotalNoise = CalculateScalingFactor(N) * totalContribution;

            return scaledTotalNoise;
        }

        private static float CalculateScalingFactor(int dimension)
        {
            if (dimension == 1)
            {
                // Dimension 1 is a special case, treat it separately
                return 0.395f;
            }
            // For some reason I don't completely understand, the formula below is only approximately correct, 
            // So we use the exact value for low dimension (the most common ones)
            // if (dimension == 2) return 40 / 0.884343445f;
            // if (dimension == 3) return 32.0f;

            // This r2 NEEDS to correlate to the one in the smoothing function. 
            // We do it kinda manually here, ideally we create a class variable for that
            float r2 = (dimension == 2) ? 0.5f : 0.6f;

            // The formula to compute the maximum value a noise can take for a given dimension N is:
            // (r2 - r2 / 9)^4 * sqrt(r2 / 9) * gradientNorm
            float gradientNorm = (dimension == 2) ? MathF.Sqrt(5) : MathF.Sqrt(dimension - 1);
            float maxNoiseValue = MathF.Pow((r2 - r2 / 9), 4) * MathF.Sqrt(r2 / 9) * gradientNorm;
            return 1.0f / maxNoiseValue;
        }

        

        #region RandomradientGeneration
        private static float RandomGrad1D(int hash)
        {
            // Drop all the bits of the hash except the last 4
            int h = hash & 0b1111;
            // Randomize the gradient to be in {1..8}
            float grad = 1.0f + (h & 0b111);
            // Randomize the sign of the gradient
            if ((h & 0b1000) != 0) grad = -grad;
            return grad;
        }
        
        // Gradients evenly distributed around the circle of radius sqrt(5)
        private static readonly Vector2[] _2D_gradients =
        {
            new Vector2(1, 2),
            new Vector2(2, 1),
            new Vector2(-1, 2),
            new Vector2(-2, 1),
            new Vector2(1, -2),
            new Vector2(2, -1),
            new Vector2(-1, -2),
            new Vector2(-2, -1)

        };
        private static Vector2 RandomGradient2D(int hash)
        {
            int h = hash & 0b111;
            return _2D_gradients[h];
        }

        private static Vector3 RandomGradient3D(int hash)
        {
            int x = (hash & 0b001) == 0b001 ? -1 : 1;
            int y = (hash & 0b010) == 0b010 ? -1 : 1;
            int z = (hash & 0b100) == 0b100 ? -1 : 1;
            Vector3 gradient = new Vector3(x, y, z);

            int zeroIndex = Mod(hash / (1 << 3), 3);
            if (zeroIndex == 0)
            {
                gradient.X = 0;
            }
            else if (zeroIndex == 1)
            {
                gradient.Y = 0;
            }
            else
            {
                gradient.Z = 0;
            }

            return gradient;
        }

        private int GetGradientHash(int dimension, HashSet<int> changedDimensions, List<int> cellBaseCoordinates)
        {
            int currentHash = 0;
            for (int i = dimension - 1; i >= 0; i--)
            {
                int offset = changedDimensions.Contains(i) ? 1 : 0;
                currentHash = _permutationTable[cellBaseCoordinates[i] + offset + currentHash];
            }
            return currentHash;
        }

        private static VectorNd RandomGradientND(int dimension, int hash)
        {
            if (dimension == 1)
            {
                float grad = RandomGrad1D(hash);
                return new VectorNd(grad);
            }

            if (dimension == 2)
            {
                Vector2 grad = RandomGradient2D(hash);
                return new VectorNd(grad.X, grad.Y);
            }

            // We select the midpoints of the edges of a hypercube of dimension N as possible gradients
            // We need one zeroIndex, which we choose as the modulo of the hash, shifted by 'dimension' bits to the right, modulo the dimension
            // We shift the hash 'dimension' bits to the right because these bits are used for the selection of the gradient in other dimensions,
            // so we want to preserve entropy
            // We take the modulo 'dimension' because is ensures the zeroIndex has a valid index, with (almost ?) perfectly uniform probability for all dimensions
            int zeroIndex = Mod(hash / (1 << dimension), dimension);
            VectorNd gradient = new VectorNd(dimension);
            int hashSelector = 1;
            for (int i = 0; i < dimension; i++)
            {
                // Sets gradient[i] to 1 or -1 based on the value of the i-th bit of the hash in binary representation
                gradient[i] = ((hash & hashSelector) == hashSelector) ? -1 : 1;
                hashSelector <<= 1;
            }
            gradient[zeroIndex] = 0;
            return gradient;
        }
        #endregion

        #region SmoothingFunctions
        private static float SmoothingFunction1D(float t)
        {
            // Smoothing function f(t) = (1 - t^2)^4
            t *= t;
            t = 1.0f - t;
            t *= t;
            t *= t;
            return t;
        }
        
        private static float SmoothingFunctionND(int dimension, VectorNd X)
        {
            // Computes (r^2 - ||X||^2)^4
            float r2 = 0.6f;
            if (dimension == 1)
            {
                r2 = 1.0f;
            }
            if (dimension == 2)
            {
                r2 = 0.5f;
            }

            float t = r2;
            for (int i = 0; i < dimension; i++)
            {
                t -= X[i] * X[i];
            }
            if (t < 0.0f)
            {
                return 0.0f;
            }

            t *= t;
            t *= t;
            return t;
        }
        #endregion

        #region UtilityFunctions
        private static List<int> GetIndicesInDecreasingOrder(List<float> input)
        {
            return input
                .Select((value, index) => new { Value = value, Index = index })
                .OrderByDescending(item => item.Value)
                .Select(item => item.Index)
                .ToList();
        }
        
        private static int Mod(int x, int mod)
        {
            int result = x % mod;
            return (result < 0) ? result + mod : result;
        }

        private static int FastFloor(float x)
        {
            return x > 0 ? (int)x : (((int)x) - 1);
        }
        #endregion
    }
}
