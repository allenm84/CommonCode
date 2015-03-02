using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Common.Extensions
{
  public static partial class DelegateExtensions
  {
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="f"></param>
    /// <returns></returns>
    public static Func<A, R> Memoize<A, R>(this Func<A, R> f)
    {
      var map = new Dictionary<A, R>();
      return a =>
      {
        R value;
        if (map.TryGetValue(a, out value))
        {
          return value;
        }
        value = f(a);
        map.Add(a, value);
        return value;
      };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="A1"></typeparam>
    /// <typeparam name="A2"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="f"></param>
    /// <returns></returns>
    public static Func<A1, A2, R> Memoize<A1, A2, R>(this Func<A1, A2, R> f)
    {
      var map = new Dictionary<A1, Dictionary<A2, R>>();
      return (a, b) =>
      {
        Dictionary<A2, R> table;
        if (!map.TryGetValue(a, out table))
        {
          table = new Dictionary<A2, R>();
          map[a] = table;
        }

        R value;
        if (table.TryGetValue(b, out value))
        {
          return value;
        }

        value = f(a, b);
        table.Add(b, value);
        return value;
      };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="A1"></typeparam>
    /// <typeparam name="A2"></typeparam>
    /// <typeparam name="A3"></typeparam>
    /// <typeparam name="R"></typeparam>
    /// <param name="f"></param>
    /// <returns></returns>
    public static Func<A1, A2, A3, R> Memoize<A1, A2, A3, R>(this Func<A1, A2, A3, R> f)
    {
      var map = new Dictionary<A1, Dictionary<A2, Dictionary<A3, R>>>();
      return (a, b, c) =>
      {
        Dictionary<A2, Dictionary<A3, R>> table1;
        if (!map.TryGetValue(a, out table1))
        {
          table1 = new Dictionary<A2, Dictionary<A3, R>>();
          map[a] = table1;
        }

        Dictionary<A3, R> table2;
        if (!table1.TryGetValue(b, out table2))
        {
          table2 = new Dictionary<A3, R>();
          table1[b] = table2;
        }

        R value;
        if (table2.TryGetValue(c, out value))
        {
          return value;
        }

        value = f(a, b, c);
        table2.Add(c, value);
        return value;
      };
    }
  }
}