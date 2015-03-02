using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Common.Extensions
{
  public static partial class ArrayExtensions
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="array"></param>
    /// <param name="value"></param>
    public static void Fill<T>(this T[] array, T value)
    {
      for (var i = 0; i < array.Length; ++i)
      {
        array[i] = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static IEnumerable<T> Extract<T>(this T[] array, int index, int count)
    {
      var retval = new T[count];
      Array.Copy(array, index, retval, 0, retval.Length);
      return retval;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <returns></returns>
    public static IEnumerable<T> ToEnumerable<T>(this T[,] array)
    {
      var len0 = array.GetLength(0);
      var len1 = array.GetLength(1);
      int i, j;

      for (i = 0; i < len0; ++i)
      {
        for (j = 0; j < len1; ++j)
        {
          yield return array[i, j];
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <returns></returns>
    public static IEnumerable<T[]> Permute<T>(this T[] array)
    {
      int i;
      int j;
      T tmp;

      // initialize a
      var a = new T[array.Length];

      // copy the passed in array to a
      array.CopyTo(a, 0);

      // constant index ceiling (a[N] length)
      var N = a.Length;

      // constant index ceiling (a[N] length)
      var ax = N - 1;

      // target array and index control array
      var p = new int[N + 1];

      // p[N] > 0 controls iteration and the index boundary for i
      for (i = 0; i < N + 1; i++)
      {
        p[i] = i;
      }

      // setup first swap points to be ax-1 and ax respectively (i & j)
      i = 1;

      // return a
      yield return a;

      // while there is more
      while (i < N)
      {
        // decrease index "weight" for i by one
        p[i]--;

        // IF i is odd then j = ax - p[i] otherwise j = ax
        j = ax - i % 2 * p[i];

        // adjust i to permute tail (i < j)
        i = ax - i;

        //     if (a[j]!=a[i])
        {
          // swap(a[i], a[j])
          tmp = a[j];
          a[j] = a[i];
          a[i] = tmp;
        }

        // reset index i to 1 (assumed)
        i = 1;

        // while (p[i] == 0)
        while (p[i] == 0)
        {
          // reset p[i] zero value
          p[i] = i;

          // set new index value for i (increase by one)
          i++;
        }

        yield return a;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int BinarySearch<T>(this T[] array, T value)
    {
      return Array.BinarySearch(array, value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int BinarySearch<T>(this T[] array, T value, IComparer<T> comparer)
    {
      return Array.BinarySearch(array, value, comparer);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="index"></param>
    /// <param name="length"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int BinarySearch<T>(this T[] array, int index, int length, T value)
    {
      return Array.BinarySearch(array, index, length, value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="index"></param>
    /// <param name="length"></param>
    /// <param name="value"></param>
    /// <param name="comparer"></param>
    /// <returns></returns>
    public static int BinarySearch<T>(this T[] array, int index, int length, T value, IComparer<T> comparer)
    {
      return Array.BinarySearch(array, index, length, value, comparer);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="index"></param>
    /// <param name="length"></param>
    public static void Clear<T>(this T[] array, int index, int length)
    {
      Array.Clear(array, index, length);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    /// <param name="array"></param>
    /// <param name="converter"></param>
    /// <returns></returns>
    public static TOutput[] ConvertAll<TInput, TOutput>(this TInput[] array, Converter<TInput, TOutput> converter)
    {
      return Array.ConvertAll(array, converter);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="match"></param>
    /// <returns></returns>
    public static bool Exists<T>(this T[] array, Predicate<T> match)
    {
      return Array.Exists(array, match);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="match"></param>
    /// <returns></returns>
    public static T Find<T>(this T[] array, Predicate<T> match)
    {
      return Array.Find(array, match);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="match"></param>
    /// <returns></returns>
    public static T[] FindAll<T>(this T[] array, Predicate<T> match)
    {
      return Array.FindAll(array, match);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="match"></param>
    /// <returns></returns>
    public static int FindIndex<T>(this T[] array, Predicate<T> match)
    {
      return Array.FindIndex(array, match);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="startIndex"></param>
    /// <param name="match"></param>
    /// <returns></returns>
    public static int FindIndex<T>(this T[] array, int startIndex, Predicate<T> match)
    {
      return Array.FindIndex(array, startIndex, match);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="startIndex"></param>
    /// <param name="count"></param>
    /// <param name="match"></param>
    /// <returns></returns>
    public static int FindIndex<T>(this T[] array, int startIndex, int count, Predicate<T> match)
    {
      return Array.FindIndex(array, startIndex, count, match);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="match"></param>
    /// <returns></returns>
    public static T FindLast<T>(this T[] array, Predicate<T> match)
    {
      return Array.FindLast(array, match);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="match"></param>
    /// <returns></returns>
    public static int FindLastIndex<T>(this T[] array, Predicate<T> match)
    {
      return Array.FindLastIndex(array, match);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="startIndex"></param>
    /// <param name="match"></param>
    /// <returns></returns>
    public static int FindLastIndex<T>(this T[] array, int startIndex, Predicate<T> match)
    {
      return Array.FindLastIndex(array, startIndex, match);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="startIndex"></param>
    /// <param name="count"></param>
    /// <param name="match"></param>
    /// <returns></returns>
    public static int FindLastIndex<T>(this T[] array, int startIndex, int count, Predicate<T> match)
    {
      return Array.FindLastIndex(array, startIndex, count, match);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="action"></param>
    public static void ForEach<T>(this T[] array, Action<T> action)
    {
      Array.ForEach(array, action);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int IndexOf<T>(this T[] array, T value)
    {
      return Array.IndexOf(array, value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="value"></param>
    /// <param name="startIndex"></param>
    /// <returns></returns>
    public static int IndexOf<T>(this T[] array, T value, int startIndex)
    {
      return Array.IndexOf(array, value, startIndex);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="value"></param>
    /// <param name="startIndex"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static int IndexOf<T>(this T[] array, T value, int startIndex, int count)
    {
      return Array.IndexOf(array, value, startIndex, count);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int LastIndexOf<T>(this T[] array, T value)
    {
      return Array.LastIndexOf(array, value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="value"></param>
    /// <param name="startIndex"></param>
    /// <returns></returns>
    public static int LastIndexOf<T>(this T[] array, T value, int startIndex)
    {
      return Array.LastIndexOf(array, value, startIndex);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="value"></param>
    /// <param name="startIndex"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static int LastIndexOf<T>(this T[] array, T value, int startIndex, int count)
    {
      return Array.LastIndexOf(array, value, startIndex, count);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    public static void Sort<T>(this T[] array)
    {
      Array.Sort(array);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="comparison"></param>
    public static void Sort<T>(this T[] array, Comparison<T> comparison)
    {
      Array.Sort(array, comparison);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="comparer"></param>
    public static void Sort<T>(this T[] array, IComparer<T> comparer)
    {
      Array.Sort(array, comparer);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="index"></param>
    /// <param name="length"></param>
    public static void Sort<T>(this T[] array, int index, int length)
    {
      Array.Sort(array, index, length);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="index"></param>
    /// <param name="length"></param>
    /// <param name="comparer"></param>
    public static void Sort<T>(this T[] array, int index, int length, IComparer<T> comparer)
    {
      Array.Sort(array, index, length, comparer);
    }
  }
}