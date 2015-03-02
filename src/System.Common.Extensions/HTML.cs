using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Common.Extensions
{
  public static partial class HTMLExtensions
  {
    private static Lazy<Dictionary<char, string>> htmlMapping = new Lazy<Dictionary<char, string>>(() => new Dictionary<char, string>
    {
      {' ', "%20"},
      {'<', "%3C"},
      {'>', "%3E"},
      {'#', "%23"},
      {'%', "%25"},
      {'{', "%7B"},
      {'}', "%7D"},
      {'|', "%7C"},
      {'\\', "%5C"},
      {'^', "%5E"},
      {'~', "%7E"},
      {'[', "%5B"},
      {']', "%5D"},
      {'`', "%60"},
      {';', "%3B"},
      {'/', "%2F"},
      {'?', "%3F"},
      {':', "%3A"},
      {'@', "%40"},
      {'=', "%3D"},
      {'&', "%26"},
      {'$', "%24"},
    }, true);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string ToHtml(this string text)
    {
      var chars = new StringBuilder(text);
      foreach (var kvp in htmlMapping.Value)
      {
        var key = kvp.Key.ToString();
        chars.Replace(key, kvp.Value);
      }
      return chars.ToString();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="html"></param>
    /// <returns></returns>
    public static string FromHtml(this string html)
    {
      var chars = new StringBuilder(html);
      foreach (var kvp in htmlMapping.Value)
      {
        var key = kvp.Key.ToString();
        chars.Replace(kvp.Value, key);
      }
      return chars.ToString();
    }
  }
}