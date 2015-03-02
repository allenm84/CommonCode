using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace System.Common.References
{
  public static class FileSystem
  {
    public static IEnumerable<string> EnumerateAllLines(string path)
    {
      return EnumerateAllLines(path, Encoding.UTF8);
    }

    public static IEnumerable<string> EnumerateAllLines(string path, Encoding encoding)
    {
      if (path == null)
      {
        throw new ArgumentNullException("path");
      }
      if (encoding == null)
      {
        throw new ArgumentNullException("encoding");
      }
      if (path.Length == 0)
      {
        throw new ArgumentException("path cannot be empty");
      }

      using (StreamReader streamReader = new StreamReader(path, encoding))
      {
        string item;
        while ((item = streamReader.ReadLine()) != null)
        {
          yield return item;
        }
      }
    }
  }
}
