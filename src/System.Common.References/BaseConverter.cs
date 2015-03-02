using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Common.References
{
  /// <summary>
  /// Copyright Andrew Jonkers 2006-
  /// convert a positive integer in base:from  to another base (allowable bases from 2 to 36)
  /// the number can be any number of digits long
  /// input and output in string format 
  /// (e.g. digits in base 2="0-1", base 16="0-F", base 20="0-J" etc
  ///</summary>
  public static class BaseConverter
  {
    /// <summary>
    /// Convert number in string representation from base:from to base:to. 
    /// Return result as a string
    /// </summary>
    /// <param name="from">The base to convert from.</param>
    /// <param name="to">The base to conver to.</param>
    /// <param name="s">The object that when cast to a string contains the proper</param>
    public static string Convert(int from, int to, string s)
    {
      // return error if input is empty
      if (string.IsNullOrEmpty(s))
      {
        throw new ArgumentException("Error: Nothing in Input String");
      }

      // only allow uppercase input characters in string
      s = s.ToUpper();

      // only do base 2 to base 36 (digit represented by characters 0-Z)"
      if (from < 2 || from > 36 || to < 2 || to > 36)
      {
        throw new ArgumentException("Base requested outside range");
      }

      // adjust the string
      s = AdjustString(from, s);

      // convert string to an array of integer digits representing number in base:from
      int il = s.Length;
      int[] fs = new int[il];
      int k = 0;
      for (int i = s.Length - 1; i >= 0; i--)
      {
        if (s[i] >= '0' && s[i] <= '9')
        {
          fs[k++] = (int)(s[i] - '0');
        }
        else
        {
          if (s[i] >= 'A' && s[i] <= 'Z')
          {
            fs[k++] = 10 + (int)(s[i] - 'A');
          }
          else
          {
            // only allow 0-9 A-Z characters
            throw new ArgumentException("Error: Input string must only contain any of 0-9 or A-Z");
          }
        }
      }

      // check the input for digits that exceed the allowable for base:from
      foreach (int i in fs)
      {
        if (i >= from)
        {
          throw new ArgumentException("Error: Not a valid number for this input base");
        }
      }

      // find how many digits the output needs
      int ol = il * (from / to + 1);

      // assign accumulation array
      int[] ts = new int[ol + 10];

      // assign the result array
      int[] cums = new int[ol + 10];

      // initialize array with number 1 
      ts[0] = 1;

      // for each input digit
      for (int i = 0; i < il; i++)
      {
        // add the input digit
        for (int j = 0; j < ol; j++)
        {
          // times (base:to from^i) to the output cumulator
          cums[j] += ts[j] * fs[i];
          int temp = cums[j];
          int rem = 0;
          int ip = j;

          // fix up any remainders in base:to
          do
          {
            rem = temp / to;
            cums[ip] = temp - rem * to; ip++;
            cums[ip] += rem;
            temp = cums[ip];
          }
          while (temp >= to);
        }

        // calculate the next power from^i) in base:to format
        for (int j = 0; j < ol; j++)
        {
          ts[j] = ts[j] * from;
        }

        //check for any remainders
        for (int j = 0; j < ol; j++)
        {
          int temp = ts[j];
          int rem = 0;
          int ip = j;

          // fix up any remainders
          do
          {
            rem = temp / to;
            ts[ip] = temp - rem * to; ip++;
            ts[ip] += rem;
            temp = ts[ip];
          }
          while (temp >= to);
        }
      }

      // convert the output to string format (digits 0,to-1 converted to 0-Z characters) 
      string sout = string.Empty;

      //leading zero flag
      bool first = false;
      for (int i = ol; i >= 0; i--)
      {
        if (cums[i] != 0) { first = true; }
        if (!first) { continue; }
        if (cums[i] < 10) { sout += (char)(cums[i] + '0'); }
        else { sout += (char)(cums[i] + 'A' - 10); }
      }
      if (string.IsNullOrEmpty(sout)) { return "0"; } //input was zero, return 0
      //return the converted string
      return sout;
    }

    private static string AdjustString(int from, string s)
    {
      StringBuilder sb = new StringBuilder(s);
      for (int i = 0; i < sb.Length; ++i)
      {
        char c = sb[i];
        int val = int.MinValue;
        if (char.IsDigit(c))
        {
          val = (int)(c - '0');
        }
        else if (char.IsLetter(c))
        {
          val = 10 + (int)(c - 'A');
        }
        else
        {
          throw new Exception("Error: Input string must only contain any of 0-9 or A-Z");
        }

        // split the number up
        if (val >= from)
        {
          int diff = val - from;
          sb[i] = '1';
          int ins = i + 1;
          if (ins < sb.Length)
          {
            sb.Insert(ins, diff);
          }
          else
          {
            sb.Append(diff);
          }
        }
      }

      return sb.ToString();
    }
  }
}
