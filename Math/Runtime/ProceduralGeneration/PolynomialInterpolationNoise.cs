using System;
using UnityEngine;


namespace antoinegleisberg.HOA.Core
{
    public static class PolynomialInterpolationNoise
    {
        public static float RandomNoise1D(float t)
        {
            return CreateInterpolationPolynomial(t)(t % 1);
        }

        private static float PseudoRandomFunction(float t)
        {
            return (Mathf.Sin(t) * 1e4f) % 1;
        }

        private static Func<float, float> CreateInterpolationPolynomial(float t)
        {
            int[] X = { 0, 1, 2, 3 };
            float[] Y = { PseudoRandomFunction(t), PseudoRandomFunction(t + 1), PseudoRandomFunction(t + 2), PseudoRandomFunction(t + 3) };

            return CreateInterpolationPolynomial(X, Y);
        }

        private static Func<float, float> CreateInterpolationPolynomial(int[] X, float[] Y)
        {
            // Find the polynomial of degree 3 that interpolated the 4 points in X and Y:
            float a3 = (((Y[3] - Y[0]) / (X[3] - X[0]) - (Y[1] - Y[0]) / (X[1] - X[0])) / (X[3] - X[1])
                - ((Y[2] - Y[0]) / (X[2] - X[0]) - (Y[1] - Y[0]) / (X[1] - X[0])) / (X[2] - X[1]))
                / (X[3] - X[2]);

            float a2 = ((Y[2] - Y[0]) / (X[2] - X[0]) - (Y[1] - Y[0]) / (X[1] - X[0])) / (X[2] - X[1])
                - a3 * (X[0] + X[1] + X[2]);

            float a1 = ((Y[1] - Y[0]) / (X[1] - X[0]))
                - a3 * (X[0] * X[0] + X[1] * X[0] + X[1] * X[1])
                - a2 * (X[0] + X[1]);

            float a0 = Y[0]
                - a3 * Mathf.Pow(X[0], 3)
                - a2 * Mathf.Pow(X[0], 2)
                - a1 * Mathf.Pow(X[0], 1);

            Debug.Log($"Created polynomial {a0} {a1} {a2} {a3}");

            float polynomial(float x)
            {
                return a0 + (x * (a1 + x * (a2 + x * (a3))));
            }

            return polynomial;
        }
    }
}
