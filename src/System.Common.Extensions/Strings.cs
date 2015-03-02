using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Common.Extensions
{
  public static partial class StringExtensions
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static IEnumerable<string> SplitCamelCase(this string text)
    {
      if (string.IsNullOrEmpty(text))
      {
        yield break;
      }

      var chars = text.ToCharArray();
      var sb = new StringBuilder();
      sb.Append(chars[0]);

      var c = '\\';
      for (var i = 1; i < chars.Length; ++i)
      {
        c = chars[i];
        if (char.IsUpper(c))
        {
          yield return sb.ToString();
          sb.Clear();
        }
        sb.Append(c);
      }

      if (sb.Length > 0)
      {
        yield return sb.ToString();
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sb"></param>
    /// <param name="format"></param>
    /// <param name="sequence"></param>
    /// <returns></returns>
    public static StringBuilder ClearAndAppendFormat(this StringBuilder sb, string format, char[] sequence)
    {
      sb.Clear();
      sb.AppendFormat(format, sequence);
      return sb;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool Matches(this string text, string value)
    {
      return string.Equals((text ?? "").Trim(), (value ?? "").Trim(),
        StringComparison.InvariantCultureIgnoreCase);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool CloseMatch(this string a, string b)
    {
      var t1 = (a ?? "").Trim();
      var t2 = (b ?? "").Trim();

      Func<string, string, bool> idx = (x, y) =>
        x.IndexOf(y, StringComparison.InvariantCultureIgnoreCase) > -1;

      return idx(t1, t2) || idx(t2, t1);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="str"></param>
    /// <param name="seperators"></param>
    /// <returns></returns>
    public static string[] SplitBy(this string str, params char[] seperators)
    {
      return str.Split(seperators, StringSplitOptions.RemoveEmptyEntries);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="characters"></param>
    /// <returns></returns>
    public static string AsString(this IEnumerable<char> characters)
    {
      return new string(characters.ToArray());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <param name="newValue"></param>
    /// <param name="oldValues"></param>
    /// <returns></returns>
    public static string ReplaceAll(this string text, string newValue, params string[] oldValues)
    {
      var current = string.Copy(text);
      foreach (var oldValue in oldValues)
      {
        current = current.Replace(oldValue, newValue);
      }
      return current;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string Duplicate(this string text)
    {
      return string.Copy(text ?? "");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="text"></param>
    /// <returns></returns>
    public static T ToEnum<T>(this string text)
    {
      return (T)Enum.Parse(typeof(T), text);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static IEnumerable<string> Words(this string text)
    {
      return from str in text.Split(' ')
        where !string.IsNullOrEmpty(str.Trim())
        select str;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ToCamelCase(this string str)
    {
      var sb = new StringBuilder();
      var words = str.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
      var retval = new string[words.Length];
      for (var i = 0; i < words.Length; ++i)
      {
        sb.Append(words[i].ToLower());
        sb[0] = char.ToUpper(sb[0]);
        retval[i] = sb.ToString();
        sb.Remove(0, sb.Length);
      }
      return string.Join(" ", retval);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static bool IsPalindrome(this string t)
    {
      var l = t.Length / 2;
      var m = t.Length % 2;

      var left = string.Join("", t.Take(l));
      var right = string.Join("", t.Skip(l + m).Reverse());

      return left.Equals(right);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool ContainsIgnoreCase(this string text, string value)
    {
      text = text.ToLower();
      value = value.ToLower();
      return text.Contains(value);
    }
  }
}