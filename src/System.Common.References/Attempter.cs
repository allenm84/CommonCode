using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace System.Common.References
{
  /// <summary>
  /// 
  /// </summary>
  public static class Attempter
  {
    private static object InternalAttempt(Delegate delg, Action timeoutCallback, int attemptCount, int attemptTimeout, out bool succeeded, params object[] args)
    {
      object retval = null;
      succeeded = false;

      int attempts = 0;
      while (!succeeded && ((attempts++) < attemptCount))
      {
        try
        {
          retval = delg.DynamicInvoke(args);
          succeeded = true;
        }
        catch
        {
          succeeded = false;
          for (int i = 0; i < attemptTimeout; ++i)
          {
            if (timeoutCallback != null)
              timeoutCallback();
            Thread.Sleep(1);
          }
        }
      }
      return retval;
    }

    #region Functions

    /// <summary>
    /// Attempts to invoke a delegate. If the delegate causes an exception, this will attempt to
    /// invoke it again after a timeout period has passed.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="func">The function to invoke.</param>
    /// <param name="timeoutCallback">The action to call every during the timeout. This can be null.</param>
    /// <param name="attemptCount">The number of times to attempt invoking the function.</param>
    /// <param name="attemptTimeout">The time (in milliseconds) between each attempt.</param>
    /// <param name="result">The result of the first successful attempt.</param>
    /// <returns>True if any attempt succedded; otherwise false.</returns>
    public static bool Attempt<TResult>(Func<TResult> func, Action timeoutCallback, int attemptCount, int attemptTimeout, out TResult result)
    {
      result = default(TResult);
      bool succeeded;
      object retval = InternalAttempt(func, timeoutCallback, attemptCount, attemptTimeout, out succeeded);
      if (retval is TResult) { result = (TResult)retval; }
      return succeeded;
    }

    /// <summary>
    /// Attempts to invoke a delegate. If the delegate causes an exception, this will attempt to
    /// invoke it again after a timeout period has passed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="func">The function to invoke.</param>
    /// <param name="arg">The argument.</param>
    /// <param name="timeoutCallback">The action to call every during the timeout. This can be null.</param>
    /// <param name="attemptCount">The number of times to attempt invoking the function.</param>
    /// <param name="attemptTimeout">The time (in milliseconds) between each attempt.</param>
    /// <param name="result">The result of the first successful attempt.</param>
    /// <returns>True if any attempt succedded; otherwise false.</returns>
    public static bool Attempt<T, TResult>(Func<T, TResult> func, T arg, Action timeoutCallback, int attemptCount, int attemptTimeout, out TResult result)
    {
      result = default(TResult);
      bool succeeded;
      object retval = InternalAttempt(func, timeoutCallback, attemptCount, attemptTimeout, out succeeded, 
        arg);
      if (retval is TResult) { result = (TResult)retval; }
      return succeeded;
    }

    /// <summary>
    /// Attempts to invoke a delegate. If the delegate causes an exception, this will attempt to
    /// invoke it again after a timeout period has passed.
    /// </summary>
    /// <typeparam name="T0"></typeparam>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="func">The function to invoke.</param>
    /// <param name="arg0">The first argument.</param>
    /// <param name="arg1">The second argument.</param>
    /// <param name="timeoutCallback">The action to call every during the timeout. This can be null.</param>
    /// <param name="attemptCount">The number of times to attempt invoking the function.</param>
    /// <param name="attemptTimeout">The time (in milliseconds) between each attempt.</param>
    /// <param name="result">The result of the first successful attempt.</param>
    /// <returns>True if any attempt succedded; otherwise false.</returns>
    public static bool Attempt<T0, T1, TResult>(Func<T0, T1, TResult> func,
      T0 arg0, T1 arg1,
      Action timeoutCallback, int attemptCount, int attemptTimeout, out TResult result)
    {
      result = default(TResult);
      bool succeeded;
      object retval = InternalAttempt(func, timeoutCallback, attemptCount, attemptTimeout, out succeeded,
        arg0, arg1);
      if (retval is TResult) { result = (TResult)retval; }
      return succeeded;
    }

    /// <summary>
    /// Attempts to invoke a delegate. If the delegate causes an exception, this will attempt to
    /// invoke it again after a timeout period has passed.
    /// </summary>
    /// <typeparam name="T0"></typeparam>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="func">The function to invoke.</param>
    /// <param name="arg0">The first argument.</param>
    /// <param name="arg1">The second argument.</param>
    /// <param name="arg2">The third argument.</param>
    /// <param name="timeoutCallback">The action to call every during the timeout. This can be null.</param>
    /// <param name="attemptCount">The number of times to attempt invoking the function.</param>
    /// <param name="attemptTimeout">The time (in milliseconds) between each attempt.</param>
    /// <param name="result">The result of the first successful attempt.</param>
    /// <returns>True if any attempt succedded; otherwise false.</returns>
    public static bool Attempt<T0, T1, T2, TResult>(Func<T0, T1, T2, TResult> func,
      T0 arg0, T1 arg1, T2 arg2,
      Action timeoutCallback, int attemptCount, int attemptTimeout, out TResult result)
    {
      result = default(TResult);
      bool succeeded;
      object retval = InternalAttempt(func, timeoutCallback, attemptCount, attemptTimeout, out succeeded,
        arg0, arg1, arg2);
      if (retval is TResult) { result = (TResult)retval; }
      return succeeded;
    }

    /// <summary>
    /// Attempts to invoke a delegate. If the delegate causes an exception, this will attempt to
    /// invoke it again after a timeout period has passed.
    /// </summary>
    /// <typeparam name="T0"></typeparam>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="func">The function to invoke.</param>
    /// <param name="arg0">The first argument.</param>
    /// <param name="arg1">The second argument.</param>
    /// <param name="arg2">The third argument.</param>
    /// <param name="arg3">The fourth argument</param>
    /// <param name="timeoutCallback">The action to call every during the timeout. This can be null.</param>
    /// <param name="attemptCount">The number of times to attempt invoking the function.</param>
    /// <param name="attemptTimeout">The time (in milliseconds) between each attempt.</param>
    /// <param name="result">The result of the first successful attempt.</param>
    /// <returns>True if any attempt succedded; otherwise false.</returns>
    public static bool Attempt<T0, T1, T2, T3, TResult>(Func<T0, T1, T2, T3, TResult> func,
      T0 arg0, T1 arg1, T2 arg2, T3 arg3,
      Action timeoutCallback, int attemptCount, int attemptTimeout, out TResult result)
    {
      result = default(TResult);
      bool succeeded;
      object retval = InternalAttempt(func, timeoutCallback, attemptCount, attemptTimeout, out succeeded,
        arg0, arg1, arg2, arg3);
      if (retval is TResult) { result = (TResult)retval; }
      return succeeded;
    }

    #endregion

    #region Actions

    /// <summary>
    /// Attempts to invoke a delegate. If the delegate causes an exception, this will attempt to
    /// invoke it again after a timeout period has passed.
    /// </summary>
    /// <param name="action"></param>
    /// <param name="timeoutCallback">The action to call every during the timeout. This can be null.</param>
    /// <param name="attemptCount">The number of times to attempt invoking the function.</param>
    /// <param name="attemptTimeout">The time (in milliseconds) between each attempt.</param>
    /// <returns>True if any attempt succedded; otherwise false.</returns>
    public static bool Attempt(Action action, Action timeoutCallback, int attemptCount, int attemptTimeout)
    {
      bool succeeded;
      InternalAttempt(action, timeoutCallback, attemptCount, attemptTimeout, out succeeded);
      return succeeded;
    }

    /// <summary>
    /// Attempts to invoke a delegate. If the delegate causes an exception, this will attempt to
    /// invoke it again after a timeout period has passed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="action">The function to invoke.</param>
    /// <param name="arg">The argument.</param>
    /// <param name="timeoutCallback">The action to call every during the timeout. This can be null.</param>
    /// <param name="attemptCount">The number of times to attempt invoking the function.</param>
    /// <param name="attemptTimeout">The time (in milliseconds) between each attempt.</param>
    /// <returns>True if any attempt succedded; otherwise false.</returns>
    public static bool Attempt<T>(Action<T> action, T arg, Action timeoutCallback, int attemptCount, int attemptTimeout)
    {
      bool succeeded;
      InternalAttempt(action, timeoutCallback, attemptCount, attemptTimeout, out succeeded,
        arg);
      return succeeded;
    }

    /// <summary>
    /// Attempts to invoke a delegate. If the delegate causes an exception, this will attempt to
    /// invoke it again after a timeout period has passed.
    /// </summary>
    /// <typeparam name="T0"></typeparam>
    /// <typeparam name="T1"></typeparam>
    /// <param name="action">The function to invoke.</param>
    /// <param name="arg0">The first argument.</param>
    /// <param name="arg1">The second argument.</param>
    /// <param name="timeoutCallback">The action to call every during the timeout. This can be null.</param>
    /// <param name="attemptCount">The number of times to attempt invoking the function.</param>
    /// <param name="attemptTimeout">The time (in milliseconds) between each attempt.</param>
    /// <returns>True if any attempt succedded; otherwise false.</returns>
    public static bool Attempt<T0, T1>(Action<T0, T1> action,
      T0 arg0, T1 arg1,
      Action timeoutCallback, int attemptCount, int attemptTimeout)
    {
      bool succeeded;
      InternalAttempt(action, timeoutCallback, attemptCount, attemptTimeout, out succeeded,
        arg0, arg1);
      return succeeded;
    }

    /// <summary>
    /// Attempts to invoke a delegate. If the delegate causes an exception, this will attempt to
    /// invoke it again after a timeout period has passed.
    /// </summary>
    /// <typeparam name="T0"></typeparam>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="action">The function to invoke.</param>
    /// <param name="arg0">The first argument.</param>
    /// <param name="arg1">The second argument.</param>
    /// <param name="arg2">The third argument.</param>
    /// <param name="timeoutCallback">The action to call every during the timeout. This can be null.</param>
    /// <param name="attemptCount">The number of times to attempt invoking the function.</param>
    /// <param name="attemptTimeout">The time (in milliseconds) between each attempt.</param>
    /// <returns>True if any attempt succedded; otherwise false.</returns>
    public static bool Attempt<T0, T1, T2>(Action<T0, T1, T2> action,
      T0 arg0, T1 arg1, T2 arg2,
      Action timeoutCallback, int attemptCount, int attemptTimeout)
    {
      bool succeeded;
      InternalAttempt(action, timeoutCallback, attemptCount, attemptTimeout, out succeeded,
        arg0, arg1, arg2);
      return succeeded;
    }

    /// <summary>
    /// Attempts to invoke a delegate. If the delegate causes an exception, this will attempt to
    /// invoke it again after a timeout period has passed.
    /// </summary>
    /// <typeparam name="T0"></typeparam>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <param name="action">The function to invoke.</param>
    /// <param name="arg0">The first argument.</param>
    /// <param name="arg1">The second argument.</param>
    /// <param name="arg2">The third argument.</param>
    /// <param name="arg3">The fourth argument</param>
    /// <param name="timeoutCallback">The action to call every during the timeout. This can be null.</param>
    /// <param name="attemptCount">The number of times to attempt invoking the function.</param>
    /// <param name="attemptTimeout">The time (in milliseconds) between each attempt.</param>
    /// <returns>True if any attempt succedded; otherwise false.</returns>
    public static bool Attempt<T0, T1, T2, T3>(Action<T0, T1, T2, T3> action,
      T0 arg0, T1 arg1, T2 arg2, T3 arg3,
      Action timeoutCallback, int attemptCount, int attemptTimeout)
    {
      bool succeeded;
      InternalAttempt(action, timeoutCallback, attemptCount, attemptTimeout, out succeeded,
        arg0, arg1, arg2, arg3);
      return succeeded;
    }

    #endregion
  }
}
