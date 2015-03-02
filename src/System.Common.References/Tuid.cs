using System.Diagnostics;

namespace System.Common.References
{
  /// <summary>
  /// 
  /// </summary>
  public static class Tuid
  {
    static long lastTick;
    static long salt = 0;
    static object syncRoot = new object();

    static Tuid()
    {
      lastTick = Stopwatch.GetTimestamp();
    }

    /// <summary>
    /// 
    /// </summary>
    public static string Next
    {
      get
      {
        string id = "";
        lock (syncRoot)
        {
          long tick = Stopwatch.GetTimestamp();
          if (tick == lastTick) { ++salt; }
          else { salt = 0; }

          lastTick = tick;
          id = string.Format("{0:X}{1:X}", tick, salt);
        }
        return id;
      }
    }
  }
}
