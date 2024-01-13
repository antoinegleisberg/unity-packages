using System.Collections.Generic;


namespace antoinegleisberg.Math
{
    public static class Math
    {
        public static int GCD(List<int> numbers)
        {
            if (numbers.Count == 0) return 0;
            if (numbers.Count == 1) return numbers[0];

            int gcd = GCD(numbers[0], numbers[1]);
            for (int i = 2; i < numbers.Count; i++)
            {
                gcd = GCD(gcd, numbers[i]);
            }
            return gcd;
        }

        public static int LCM(List<int> numbers)
        {
            if (numbers.Count == 0) return 0;
            if (numbers.Count == 1) return numbers[0];

            int lcm = LCM(numbers[0], numbers[1]);
            for (int i = 2; i < numbers.Count; i++)
            {
                lcm = LCM(lcm, numbers[i]);
            }
            return lcm;
        }

        public static int GCD(int a, int b)
        {
            if (a == 0) return b;
            if (b == 0) return a;
            if (a < b) return GCD(b, a);

            int tmp;
            while (b != 0)
            {
                tmp = b;
                b = a % b;
                a = tmp;
            }
            return a;
        }

        public static int LCM(int a, int b)
        {
            return a * b / GCD(a, b);
        }
    }
}
