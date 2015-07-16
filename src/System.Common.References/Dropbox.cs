using System;
using System.IO;
using System.Text;

namespace System.Common.References
{
  public static class Dropbox
  {
    private static Lazy<string> location = new Lazy<string>(() =>
    {
      var folders = new[]
      {
        Environment.SpecialFolder.ApplicationData,
        Environment.SpecialFolder.CommonApplicationData,
        Environment.SpecialFolder.LocalApplicationData,
      };

      foreach (var folder in folders)
      {
        string appData = Environment.GetFolderPath(folder);
        string hostdb = Path.Combine(appData, @"Dropbox\host.db");
        if (!File.Exists(hostdb)) continue;
        string[] lines = File.ReadAllLines(hostdb);
        return Encoding.ASCII.GetString(Convert.FromBase64String(lines[1]));
      }

      throw new FileNotFoundException("host.db not found");
    }, true);

    public static string Location { get { return location.Value; } }

    static Dropbox() { }
  }
}
