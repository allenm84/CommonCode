using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace System.Common.Extensions
{
  public static partial class NumericExtensions
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="baseValue"></param>
    /// <returns></returns>
    public static string ToBase(this int value, int baseValue)
    {
      if (baseValue > 36) { throw new ArgumentException("Can only convert bases up to 36"); }

      // build the table of values
      var table = new char[baseValue];
      var seed = '0';
      for (var i = 0; i < table.Length; ++i)
      {
        if (i == 10) { seed = Convert.ToChar('A' - 10); }
        table[i] = Convert.ToChar(seed + i);
      }

      // build the actual value
      var retval = new List<char>();
      while (value != 0)
      {
        retval.Insert(0, table[value % baseValue]);
        value /= baseValue;
      }

      // return
      return new string(retval.ToArray());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rnd"></param>
    /// <returns></returns>
    public static BigInteger NextBigInteger(this Random rnd)
    {
      var size = rnd.Next(2056) + 1;
      var buffer = new byte[size];
      rnd.NextBytes(buffer);
      return new BigInteger(buffer);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rnd"></param>
    /// <param name="maxValue"></param>
    /// <returns></returns>
    public static BigInteger NextBigInteger(this Random rnd, BigInteger maxValue)
    {
      var size = rnd.Next(2056) + 1;
      var buffer = new byte[size];
      rnd.NextBytes(buffer);
      return new BigInteger(buffer) % (maxValue - 1);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rnd"></param>
    /// <param name="minValue"></param>
    /// <param name="maxValue"></param>
    /// <returns></returns>
    public static BigInteger NextBigInteger(this Random rnd, BigInteger minValue, BigInteger maxValue)
    {
      var size = rnd.Next(2056) + 1;
      var buffer = new byte[size];
      rnd.NextBytes(buffer);
      return new BigInteger(buffer) % (maxValue - minValue + 1) + minValue;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="exp"></param>
    /// <param name="mod"></param>
    /// <returns></returns>
    public static BigInteger ExpMod(this BigInteger value, BigInteger exp, BigInteger mod)
    {
      BigInteger ullResult = 1;
      var ullValue = value;

      while (exp > 0)
      {
        if (exp % 2 != 0)
        {
          // odd
          ullResult *= ullValue;
          ullResult %= mod;
        }

        ullValue *= ullValue;
        ullValue %= mod;
        exp /= 2;
      }

      return (ullResult);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static BigInteger Multiply(this IEnumerable<BigInteger> source)
    {
      BigInteger retval = 1;
      foreach (var d in source)
      {
        retval *= d;
      }
      return retval;
    }

    /// <summary>
    /// Computes the sum of a sequence of <see cref="T:System.Numerics.BigInteger" /> values.
    /// </summary>
    /// <param name="source">A sequence of <see cref="T:System.Numerics.BigInteger" /> values to calculate the sum of.</param>
    /// <returns>The sum of the values in the sequence.</returns>
    public static BigInteger? Sum(this IEnumerable<BigInteger?> source)
    {
      BigInteger sum = 0;
      foreach (var current in source)
      {
        sum += current.GetValueOrDefault(0);
      }
      return sum;
    }

    /// <summary>
    /// Computes the sum of a sequence of <see cref="T:System.Numerics.BigInteger" /> values.
    /// </summary>
    /// <param name="source">A sequence of <see cref="T:System.Numerics.BigInteger" /> values to calculate the sum of.</param>
    /// <returns>The sum of the values in the sequence.</returns>
    public static BigInteger Sum(this IEnumerable<BigInteger> source)
    {
      BigInteger sum = 0;
      foreach (var current in source)
      {
        sum += current;
      }
      return sum;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public static bool IsPrime(this BigInteger n)
    {
      if (n < 2) { return false; }
      if (n % 2 == 0) { return (n == 2); }
      if (n % 3 == 0) { return (n == 3); }
      for (var p = 5;;)
      {
        var q = n / p;
        if (q < p) { break; }
        if (q * p == n) { return false; }
        p += 2;
        q = n / p;
        if (q < p) { break; }
        if (q * p == n) { return false; }
        p += 4;
      }
      return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="prime"></param>
    /// <returns></returns>
    public static bool IsMillerRabinPrime(this BigInteger prime)
    {
      // randomWitness = witness that the "prime" is NOT composite
      // 1 < randomWitness < prime-1
      long totalWitness = 15;
      BigInteger[] randomWitness = {2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 51001, 351011};
      var primeMinusOne = prime - 1;
      BigInteger oddMultiplier;
      long bitLength;
      long i, j;
      BigInteger result;
      // find oddMultiplier, defined as "prime - 1 = (2^B) * M"
      // get bitLength (B) and find the oddMultiplier (M)

      // init value multiplier
      oddMultiplier = primeMinusOne;

      bitLength = 0; // reset
      while (oddMultiplier % 2 == 0)
      {
        oddMultiplier /= 2;
        bitLength++;
      }
      for (i = 0; i < totalWitness; i++)
      {
        if (randomWitness[i] == prime)
        {
          return true;
        }

        // init value of result = (randomWitness ^ oddMultiplier) mod prime
        result = ExpMod(randomWitness[i], oddMultiplier, prime);

        // is y = 1 ?
        if (result == 1)
        {
          continue; // maybe prime
        }

        // is y = prime-1 ?
        if (result == primeMinusOne)
        {
          continue; // maybe prime
        }

        // loop to get AT LEAST one "result = primeMinusOne"
        for (j = 1; j <= bitLength; j++)
        {
          // square of result
          result = ExpMod(result, 2, prime);

          // is result = primeMinusOne ?
          if (result == primeMinusOne)
          {
            break; // maybe prime
          }
        }

        if (result != primeMinusOne)
        {
          return false; // COMPOSITE
        }
      }

      // it may be pseudoprime/prime
      return true;
    }
  }
}