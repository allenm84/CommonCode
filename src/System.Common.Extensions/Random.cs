using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Common.Extensions
{
  public static partial class RandomExtensions
  {
    /// <summary>
    /// 
    /// </summary>
    private static readonly string[] _NextSentenceWords =
    {
      "the", "in", "a", "an", "for", "what",
      "here", "because", "we", "are", "you",
      "won't", "will", "that", "they", "hey",
      "there", "refuse", "to", "sheep", "out",
      "up", "down", "jump", "and", "I", "didn't",
      "wouldn't", "count", "can't", "shouldn't",
      "well", "it", "is", "day", "think", "thank",
      "funny", "was", "really", "come", "on", "off",
      "fall", "type", "drink", "gone", "hole", "rid",
      "ride", "home", "reply", "alright", "then",
      "than", "rather", "believe", "sight", "about",
      "fix", "where", "snap", "right", "wrong"
    };

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rnd"></param>
    /// <returns></returns>
    public static ulong NextUInt64(this Random rnd)
    {
      var buffer = new byte[sizeof(ulong)];
      rnd.NextBytes(buffer);
      return BitConverter.ToUInt64(buffer, 0);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rnd"></param>
    /// <param name="maxValue"></param>
    /// <returns></returns>
    public static ulong NextUInt64(this Random rnd, ulong maxValue)
    {
      var buffer = new byte[sizeof(ulong)];
      rnd.NextBytes(buffer);
      return BitConverter.ToUInt64(buffer, 0) % (maxValue - 1);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rnd"></param>
    /// <param name="minValue"></param>
    /// <param name="maxValue"></param>
    /// <returns></returns>
    public static ulong NextUInt64(this Random rnd, ulong minValue, ulong maxValue)
    {
      var buffer = new byte[sizeof(ulong)];
      rnd.NextBytes(buffer);
      return BitConverter.ToUInt64(buffer, 0) % (maxValue - minValue + 1) + minValue;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="random"></param>
    /// <returns></returns>
    public static DateTime NextDate(this Random random)
    {
      var days = (DateTime.MaxValue - DateTime.MinValue).Days;
      return DateTime.MinValue.AddDays(random.Next(days));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="random"></param>
    /// <returns></returns>
    public static decimal NextDecimal(this Random random)
    {
      Func<int> nextInt32 = () =>
      {
        var firstBits = random.Next(0, 1 << 4) << 28;
        var lastBits = random.Next(0, 1 << 28);
        return firstBits | lastBits;
      };

      var scale = (byte)random.Next(29);
      var sign = random.Next(2) == 1;
      return new decimal(nextInt32(), nextInt32(), nextInt32(), sign, scale);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="random"></param>
    /// <returns></returns>
    public static float NextSingle(this Random random)
    {
      return (float)random.NextDouble();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="getWeight"></param>
    /// <param name="random"></param>
    /// <returns></returns>
    public static T NextWeightedElement<T>(this IEnumerable<T> list, Func<T, int> getWeight, Random random)
    {
      return random.NextWeightedElement(list, getWeight);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="random"></param>
    /// <param name="list"></param>
    /// <param name="getWeight"></param>
    /// <returns></returns>
    public static T NextWeightedElement<T>(this Random random, IEnumerable<T> list, Func<T, int> getWeight)
    {
      // retrieve the weighted values
      var weightedValues =
        (from v in list
          select new {Value = v, Weight = getWeight(v)}).ToArray();

      // if the array is zero, then return the default T
      if (weightedValues.Length == 0)
      {
        return default(T);
      }

      // retrieve the total weight
      var runningtotal = 0;

      // retrieve the total weight
      var weightSum = weightedValues.Sum(a => a.Weight);

      // retrieve a random number
      var randValue = random.Next(weightSum);

      // create a variable to determine if a weighted value was found
      var weightedFound = false;

      // get a weighted value
      var retval = default(T);
      foreach (var value in weightedValues)
      {
        runningtotal += value.Weight;
        if (runningtotal > randValue)
        {
          weightedFound = true;
          retval = value.Value;
          break;
        }
      }

      // if we didn't find a weighted value
      if (!weightedFound)
      {
        retval = random.NextElement(list);
      }

      // return the value
      return retval;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="random"></param>
    /// <param name="list"></param>
    /// <returns></returns>
    public static T NextElement<T>(this Random random, IEnumerable<T> list)
    {
      var index = random.Next(list.Count());
      return list.ElementAt(index);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="random"></param>
    /// <param name="array"></param>
    /// <returns></returns>
    public static T NextElement<T>(this Random random, T[] array)
    {
      var index = random.Next(array.Length);
      return array[index];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="random"></param>
    /// <param name="list"></param>
    /// <returns></returns>
    public static T NextElement<T>(this Random random, IList<T> list)
    {
      var index = random.Next(list.Count);
      return list[index];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="random"></param>
    /// <returns></returns>
    public static string NextName(this Random random)
    {
      var alphabet = "abcdefghijklmnopqrstuvwyxzeeeiouea";
      Func<char> randomLetter = () => alphabet[random.Next(alphabet.Length)];
      Func<int, string> makeName =
        length => new string(Enumerable.Range(0, length)
          .Select(x => x == 0 ? char.ToUpper(randomLetter()) : randomLetter())
          .ToArray());
      return string.Format("{0}", makeName(random.Next(5) + 5));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="random"></param>
    /// <returns></returns>
    public static char NextLetter(this Random random)
    {
      var c = Convert.ToChar(random.Next('A', 'Z' + 1));
      return random.Next(0, 7) == 1 ? char.ToLower(c) : c;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="random"></param>
    /// <param name="words"></param>
    /// <returns></returns>
    public static string NextSentence(this Random random, string[] words = null)
    {
      var source = words ?? _NextSentenceWords;
      var arr = new string[random.Next(4, 11)];

      var indices = Enumerable.Range(0, source.Length).ToList();
      var w = 0;

      for (var i = 0; i < arr.Length; ++i)
      {
        w = random.Next(indices.Count);
        arr[i] = source[indices[w]];
        indices.RemoveAt(w);
      }

      var sb = new StringBuilder(arr[0]);
      sb[0] = char.ToUpper(sb[0]);

      arr[0] = sb.ToString();
      return string.Join(" ", arr);
    }
  }
}