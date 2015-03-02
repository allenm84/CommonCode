using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Common.References
{
  /// <summary>
  /// 
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class FuncComparer<T> : IComparer<T>
  {
    private Func<T, T, int> _compareFn;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fn"></param>
    public FuncComparer(Func<T, T, int> fn)
    {
      _compareFn = fn;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public int Compare(T x, T y)
    {
      return _compareFn(x, y);
    }
  }

  /// <summary>
  /// 
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class FuncEqualityComparer<T> : IEqualityComparer<T>
  {
    private Func<T, T, bool> _equalsFn;
    private Func<T, int> _getHashCodefn;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="equalsFn"></param>
    /// <param name="getHashCodefn"></param>
    public FuncEqualityComparer(Func<T, T, bool> equalsFn, Func<T, int> getHashCodefn)
    {
      _equalsFn = equalsFn;
      _getHashCodefn = getHashCodefn;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool Equals(T x, T y)
    {
      return _equalsFn(x, y);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public int GetHashCode(T obj)
    {
      return _getHashCodefn(obj);
    }
  }
}