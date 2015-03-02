using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace System.Common.References
{
  /// <summary>
  /// 
  /// </summary>
  public static class Duid
  {
    private const string Format = "{0:yyyyyMMddHHmmssfffffff}{1:0000000000000000000}{2:0000000000000000000}";

    private static string LastID = "";
    private static object syncRoot = new object();

    /// <summary>
    /// 
    /// </summary>
    public static string Next
    {
      get
      {
        string id = string.Empty;
        lock (syncRoot)
        {
          long salt = 0;
          long postFix = Stopwatch.GetTimestamp();
          DateTime time = DateTime.Now;
          do
          {
            id = string.Format(Format, time, postFix, salt++);
          }
          while (string.Equals(LastID, id));
          LastID = id;
        }
        return id;
      }
    }
  }
}