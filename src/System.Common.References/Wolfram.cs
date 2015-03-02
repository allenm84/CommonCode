using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Common.References
{
  public static class Wolfram
  {
    /// <summary>
    /// Performs simple interpolation. For example, if we have:
    /// 
    /// Small = 40
    /// Medium = ??
    /// Large = ??
    /// X-Large = 190
    /// 
    /// The min is 40, the max is 190, and there are 4 values. This
    /// will return an array filled with the interpolated values.
    /// </summary>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <param name="valueCount">The number of values.</param>
    /// <returns>An array filled with the interpolated values.</returns>
    public static double[] Interpolate(double min, double max, int valueCount)
    {
      double[] values = new double[valueCount];
      values[0] = min;
      values[valueCount - 1] = max;
      Interpolate(values);
      return values;
    }

    /// <summary>
    /// Performs simple interpolation. The first and last values of the array should have values.
    /// The array is filled with the interpolated values.
    /// </summary>
    /// <param name="values">
    /// The array to fill. The first and last values should be the min 
    /// and max value respectively.
    /// </param>
    public static void Interpolate(double[] values)
    {
      int lastIndex = values.Length - 1;
      double min = values[0];
      double max = values[lastIndex];

      double step = Divide((max - min), lastIndex);
      for (int i = 1; i < lastIndex; ++i)
        values[i] += values[i - 1] + step;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static float Divide(float a, float b)
    {
      return a / b;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static double Divide(double a, double b)
    {
      return a / b;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static decimal Divide(decimal a, decimal b)
    {
      return a / b;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="exponent"></param>
    /// <param name="modulus"></param>
    /// <returns></returns>
    public static long ModPow(long value, long exponent, long modulus)
    {
      long result = 1;
      while (exponent > 0)
      {
        if ((exponent & 1) == 1)
        {
          result = (result * value) % modulus;
        }
        exponent >>= 1;
        value = (value * value) % modulus;
      }
      return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="exponent"></param>
    /// <param name="modulus"></param>
    /// <returns></returns>
    public static int ModPow(int value, int exponent, int modulus)
    {
      int result = 1;
      while (exponent > 0)
      {
        if ((exponent & 1) == 1)
        {
          result = (result * value) % modulus;
        }
        exponent >>= 1;
        value = (value * value) % modulus;
      }
      return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static byte Clamp(byte value, byte min, byte max)
    {
      return Math.Max(min, Math.Min(value, max));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static decimal Clamp(decimal value, decimal min, decimal max)
    {
      return Math.Max(min, Math.Min(value, max));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static double Clamp(double value, double min, double max)
    {
      return Math.Max(min, Math.Min(value, max));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static float Clamp(float value, float min, float max)
    {
      return Math.Max(min, Math.Min(value, max));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static int Clamp(int value, int min, int max)
    {
      return Math.Max(min, Math.Min(value, max));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static long Clamp(long value, long min, long max)
    {
      return Math.Max(min, Math.Min(value, max));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static sbyte Clamp(sbyte value, sbyte min, sbyte max)
    {
      return Math.Max(min, Math.Min(value, max));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static short Clamp(short value, short min, short max)
    {
      return Math.Max(min, Math.Min(value, max));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static uint Clamp(uint value, uint min, uint max)
    {
      return Math.Max(min, Math.Min(value, max));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static ulong Clamp(ulong value, ulong min, ulong max)
    {
      return Math.Max(min, Math.Min(value, max));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static ushort Clamp(ushort value, ushort min, ushort max)
    {
      return Math.Max(min, Math.Min(value, max));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
    {
      if (value.CompareTo(min) < 0) return min;
      if (value.CompareTo(max) > 0) return max;
      return value;
    }


    /// <summary>
    /// Performs linear interpolation between two values.
    /// </summary>
    /// <param name="x">The first value.</param>
    /// <param name="y">The second value.</param>
    /// <param name="mu">Value between 0 and 1 indicating the weight of y.</param>
    /// <param name="clamp">Indicates whether or not mu needs to be clamped.</param>
    /// <returns>Interpolated value.</returns>
    public static float Lerp(float x, float y, float mu, bool clamp = true)
    {
      if (clamp)
        mu = Clamp(mu, 0, 1);
      return (x * (1 - mu) + y * mu);
    }

    /// <summary>
    /// Performs cosine interpolation between two values.
    /// </summary>
    /// <param name="x">The first value.</param>
    /// <param name="y">The second value.</param>
    /// <param name="mu">Value between 0 and 1 indicating the weight of y.</param>
    /// <returns>Interpolated value.</returns>
    public static float Coserp(float x, float y, float mu)
    {
      float mu2;
      mu2 = (float)(1 - Math.Cos(mu * Math.PI)) / 2f;
      return (x * (1 - mu2) + y * mu2);
    }

    /// <summary>
    /// Performs linear interpolation between two values.
    /// </summary>
    /// <param name="x">The first value.</param>
    /// <param name="y">The second value.</param>
    /// <param name="mu">Value between 0 and 1 indicating the weight of y.</param>
    /// <param name="clamp">Indicates whether or not mu needs to be clamped.</param>
    /// <returns>Interpolated value.</returns>
    public static double Lerp(double x, double y, double mu, bool clamp = true)
    {
      if (clamp)
        mu = Clamp(mu, 0, 1);
      return (x * (1 - mu) + y * mu);
    }

    /// <summary>
    /// Performs cosine interpolation between two values.
    /// </summary>
    /// <param name="x">The first value.</param>
    /// <param name="y">The second value.</param>
    /// <param name="mu">Value between 0 and 1 indicating the weight of y.</param>
    /// <returns>Interpolated value.</returns>
    public static double Coserp(double x, double y, double mu)
    {
      double mu2;
      mu2 = (1 - Math.Cos(mu * Math.PI)) / 2;
      return (x * (1 - mu2) + y * mu2);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public static float SmoothStep(float value1, float value2, float amount)
    {
      float num = Clamp(amount, 0f, 1f);
      return Lerp(value1, value2, num * num * (3f - 2f * num));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public static double SmoothStep(double value1, double value2, double amount)
    {
      double num = Clamp(amount, 0d, 1d);
      return Lerp(value1, value2, num * num * (3d - 2d * num));
    }
  }
}
