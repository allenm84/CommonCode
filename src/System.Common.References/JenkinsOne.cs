using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Common.References
{
  public static class JenkinsOne
  {
    private static unsafe void Hash(byte* d, int len, ref uint h)
    {
      for (int i = 0; i < len; i++)
      {
        h += d[i];
        h += (h << 10);
        h ^= (h >> 6);
      }
    }

    public unsafe static void Hash(ref uint h, string s)
    {
      fixed (char* c = s)
      {
        byte* b = (byte*)(void*)c;
        Hash(b, s.Length * 2, ref h);
      }
    }

    public unsafe static void Hash(ref uint h, int data)
    {
      byte* d = (byte*)(void*)&data;
      Hash(d, sizeof(int), ref h);
    }

    public unsafe static void Hash(ref uint h, long data)
    {
      byte* d = (byte*)(void*)&data;
      Hash(d, sizeof(long), ref h);
    }

    public unsafe static int Avalanche(uint h)
    {
      h += (h << 3);
      h ^= (h >> 11);
      h += (h << 15);
      return *((int*)(void*)&h);
    }

    public static int ComputeHash(IEnumerable<string> strings)
    {
      uint h = 0;
      foreach (var item in strings)
        Hash(ref h, item);
      return Avalanche(h);
    }
  }
}
