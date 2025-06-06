using antoinegleisberg.Types;
using System;
using System.Collections.Generic;
using System.Linq;

using Vector2 = System.Numerics.Vector2;

namespace antoinegleisberg.Noise.Simplex
{
    internal class SimplexNoise
    {
        private int _permutationTableSize;
        private byte[] _permutationTable;

        public SimplexNoise(int seed = 0, int permutationTableSize = 256)
        {
            if (seed == 0)
            {
                seed = new Random().Next();
            }
            _permutationTableSize = permutationTableSize;
            _permutationTable = new byte[_permutationTableSize * 2];
            var random = new Random(seed);
            random.NextBytes(_permutationTable);
        }

        /// <summary>
        /// Can return values slightly larger than 1 or lower than 0
        /// </summary>
        /// <param name="X"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public float CalculateNd(List<float> X)
        {
            // N is the number of dimensions
            int N = X.Count;

            if (N == 1)
            {
                // 1D Simplex noise is a special case: handle it separately
                return Calculate1D(X[0]);
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

        private float Calculate1D(float x)
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
