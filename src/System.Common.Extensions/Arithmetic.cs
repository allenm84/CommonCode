using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Common.Extensions
{
  public static partial class ArithmeticExtensions
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public static IEnumerable<long> CollatzChain(this long n)
    {
      var CollatzChainTable = new Func<long, long>[2];
      CollatzChainTable[0] = (x => (3L * x) + 1L);
      CollatzChainTable[1] = (x => x / 2L);

      var i = n;
      while (i != 1)
      {
        var even = (i % 2) == 0;
        yield return i;
        i = CollatzChainTable[Convert.ToInt32(even)](i);
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public static bool IsPalindrome(this int n)
    {
      var num = n;
      var rev = 0;

      while (n > 0)
      {
        var dig = n % 10;
        rev = rev * 10 + dig;
        n = n / 10;
      }

      return (num == rev);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="d"></param>
    /// <returns></returns>
    public static bool IsInteger(this double d)
    {
      return (d % 1) == 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public static IEnumerable<uint> Digits(this uint n)
    {
      while (n > 0u)
      {
        yield return n % 10u;
        n /= 10;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public static int[] GetDigits(this int n)
    {
      var digits = new List<int>();
      while (n > 0)
      {
        digits.Insert(0, n % 10);
        n /= 10;
      }
      return digits.ToArray();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public static int[] GetDigits(this long n)
    {
      var digits = new List<int>();
      while (n > 0)
      {
        digits.Insert(0, (int)(n % 10));
        n /= 10;
      }
      return digits.ToArray();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public static bool IsPanDigital(this int n)
    {
      var digits = new HashSet<int>(new[] {1, 2, 3, 4, 5, 6, 7, 8, 9});
      while (n > 0)
      {
        var d = (n % 10);
        if (!digits.Remove(d)) { return false; }
        n /= 10;
      }
      return digits.Count == 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public static IEnumerable<int> Factors(this int x)
    {
      for (var factor = 1; factor * factor <= x; factor++)
      {
        if (x % factor == 0)
        {
          yield return factor;
          if (factor * factor != x)
          {
            yield return x / factor;
          }
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public static bool IsPowerOfTwo(this ulong x)
    {
      return (x != 0) && ((x & (x - 1)) == 0);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public static bool IsPowerOfTwo(this uint x)
    {
      return (x != 0) && ((x & (x - 1)) == 0);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public static bool IsPowerOfTwo(this ushort x)
    {
      return (x != 0) && ((x & (x - 1)) == 0);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public static IEnumerable<int[]> GetPartitions(this int n)
    {
      /*
      def accelAsc(n):
          a = [0 for i in range(n + 1)]
          k = 1
          a[0] = 0
          y = n - 1
          while k != 0:
              x = a[k - 1] + 1
              k -= 1
              while 2*x <= y:
                  a[k] = x
                  y -= x
                  k += 1
              l = k + 1
              while x <= y:
                  a[k] = x
                  a[l] = y
                  yield a[:k + 2]
                  x += 1
                  y -= 1
              a[k] = x + y
              y = x + y - 1
              yield a[:k + 1]
     */

      var a = Enumerable.Repeat(0, n + 1).ToArray();
      var k = 1;
      a[0] = 0;
      var y = n - 1;
      while (k != 0)
      {
        var x = a[k - 1] + 1;
        --k;
        while ((2 * x) <= y)
        {
          a[k] = x;
          y -= x;
          ++k;
        }
        var l = k + 1;
        while (x <= y)
        {
          a[k] = x;
          a[l] = y;
          yield return a.Take(k + 2).ToArray();
          x += 1;
          y -= 1;
        }
        a[k] = x + y;
        y = x + y - 1;
        yield return a.Take(k + 1).ToArray();
      }
    }
  }
}