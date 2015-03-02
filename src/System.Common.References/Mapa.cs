using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace System.Common.References
{
  public static class Mapa
  {
    public static string GetAppPath(string appName)
    {
      string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
      string appPath = Path.Combine(appData, "MAPA Inc", appName);
      if (!Directory.Exists(appPath)) Directory.CreateDirectory(appPath);
      return appPath;
    }
  }
}
