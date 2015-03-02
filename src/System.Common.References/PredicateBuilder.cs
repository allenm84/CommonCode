using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace System.Common.References
{
  /// <summary>
  /// 
  /// </summary>
  public static class PredicateBuilder
  {
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Expression<Func<T, bool>> True<T>() { return f => true; }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Expression<Func<T, bool>> False<T>() { return f => false; }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="expr1"></param>
    /// <param name="expr2"></param>
    /// <returns></returns>
    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1,
      Expression<Func<T, bool>> expr2)
    {
      var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
      return Expression.Lambda<Func<T, bool>>
        (Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="expr1"></param>
    /// <param name="expr2"></param>
    /// <returns></returns>
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1,
      Expression<Func<T, bool>> expr2)
    {
      var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
      return Expression.Lambda<Func<T, bool>>
        (Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
    }
  }
}
