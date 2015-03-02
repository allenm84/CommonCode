using System;
using System.IO;
using System.Text;

namespace System.Common.References
{
  /// <summary>
  /// 
  /// </summary>
  public static class Dropbox
  {
    /// <summary>
    /// 
    /// </summary>
    private static Lazy<string> location = new Lazy<string>(() =>
    {
      string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
      string hostdb = Path.Combine(appData, @"Dropbox\host.db");
      string[] lines = File.ReadAllLines(hostdb);
      return Encoding.ASCII.GetString(Convert.FromBase64String(lines[1]));
    }, true);

    /// <summary>
    /// 
    /// </summary>
    public static string Location { get { return location.Value; } }

    /// <summary>
    /// 
    /// </summary>
    static Dropbox() { }
  }
}
