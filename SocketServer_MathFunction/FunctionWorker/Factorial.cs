using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace FunctionWorker
{
    static class Factorial
    {
        public static BigInteger Calculate(int n)
        {
            if (n < 10) return SimpleFactorial(n);

            int bits = n - Convert.ToString(n, 2).Sum(x => x - '0');

            return OddFactorial(n) * ((BigInteger)2 ^ bits);
        }

        static int SimpleFactorial(int n)
        {
            if (n == 0) return 1;

            return n * SimpleFactorial(n - 1);
        }

        static BigInteger OddFactorial(int n)
        {
            if (n < 2) return 1;

            return (OddFactorial(n / 2) ^ 2) * PrimeSwing(n);
        }

        static BigInteger PrimeSwing(int n)
        {
            int count = 0;
            List<BigInteger> factorList = new List<BigInteger>();
            BigInteger[] primes = Primes(3, n);

            foreach(var prime in primes)
            {
                BigInteger q = n, p = 1;

                do
                {
                    q = q / prime;
                    if ((q & 1) == 1) p *= prime;
                } while (q != 0);

                if (p > 1) { factorList.Add(p); count++; }
            }

            return Product(factorList, 0, count - 1);
        }

        static BigInteger Product(List<BigInteger> list, int n, int m)
        {
            if (n > m) return 1;
            if (n == m) return list[n];

            int k = (n + m) / 2;

            return Product(list, n, k) * Product(list, k + 1, m);
        }

        static BigInteger[] Primes(int first, int last)
        {
            bool[] primes = new bool[last - 1];
            for (int i = 2; i <= last; i++)
                primes[i - 2] = true;

            for (int i = 2; i <= last; i++)
                if (primes[i - 2])
                    for (int p = 2; p * i <= last; p++)
                        primes[p * i - 2] = false;

            List<BigInteger> res = new List<BigInteger>();
            for (int i = first; i <= last; i++)
                if (primes[i - 2]) res.Add(i);

            return res.ToArray();
        }
    }
}
