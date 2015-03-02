using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Common.References
{
  /// <summary>
  /// Class for performing the Boyer–Moore string search algorithm.
  /// </summary>
  public sealed class BoyerMoore
  {
    #region BoyerHashTable

    private class BoyerHashTable
    {
      private BoyerHashItem[][] Items;
      private int Count;

      /// <summary>
      /// Gets an array of keys present in the table.
      /// </summary>
      public char[] Keys
      {
        get
        {
          int index = 0;
          char[] keys = new char[Count];
          for (int i = 0; i < 256; i++)
            if (Items[i] != null)
              for (int j = 0; j < Items[i].Length; j++)
              {
                keys[index] = Items[i][j].Key;
                index++;
              }
          return keys;
        }
      }

      public BoyerHashTable()
      {
        Items = new BoyerHashItem[256][];
      }

      /// <summary>
      /// Gets the value specified by Key.
      /// </summary>
      /// <param name="Key">The key of the value to retrieve.</param>
      /// <returns>Returns the value or null if the Key isn't found.</returns>
      public int[] Get(char Key)
      {
        int HashedKey = Key % 256;
        if (Items[HashedKey] != null)
        {
          // The most likely variant
          if (Items[HashedKey][0].Key == Key)
            return Items[HashedKey][0].Shifts;
          for (int i = 1; i < Items[HashedKey].Length; i++)
            if (Items[HashedKey][i].Key == Key)
              return Items[HashedKey][i].Shifts;
        }
        return null;
      }

      /// <summary>
      /// Adds a key-value pair to the hash table. If the key is already present in the table this 
      /// method does nothing.
      /// </summary>
      /// <param name="Key">The key of the value to add.</param>
      /// <param name="Value">The value itself to add.</param>
      public void Add(char Key, int[] Value)
      {
        if (Get(Key) != null)
          return;
        int HashedKey = Key % 256;
        BoyerHashItem HI = new BoyerHashItem();
        HI.Shifts = new int[Value.Length];
        HI.Key = Key;
        HI.Shifts = Value;
        if (Items[HashedKey] == null)
          Items[HashedKey] = new BoyerHashItem[1] { HI };
        else
        {
          BoyerHashItem[] NewItems = new BoyerHashItem[Items[HashedKey].Length + 1];
          Items[HashedKey].CopyTo(NewItems, 0);
          NewItems[Items[HashedKey].Length] = HI;
          Items[HashedKey] = NewItems;
        }
        Count++;
      }

      #region Boyer Hash Item
      private struct BoyerHashItem
      {
        /// <summary>
        /// Gets or sets the Hash-key
        /// </summary>
        public char Key { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public int[] Shifts { get; set; }
      }
      #endregion
    }

    #endregion

    /// <summary>Shift table for chars present in the pattern.</summary>
    private BoyerHashTable PatternCharShifts;

    /// <summary>Shifts for all other chars.</summary>
    private int[] OtherCharShifts;

    /// <summary>Length of the search pattern.</summary>
    private int PatternLength;

    /// <summary>
    /// Creates a new Boyer-Moore searching object that is not case sensitive.
    /// </summary>
    /// <param name="pattern">The pattern to search for. This is pre-processed.</param>
    public BoyerMoore(string pattern)
      : this(pattern, false)
    {
    }

    /// <summary>
    /// Creates a new Boyer-Moore searching object.
    /// </summary>
    /// <param name="pattern">The pattern to search for. This is pre-processed.</param>
    /// <param name="caseSensitive">Whether or not the search should be case sensitive.</param>
    public BoyerMoore(string pattern, bool caseSensitive)
    {
      if (string.IsNullOrEmpty(pattern))
        throw new ArgumentException("pattern can not be null or empty");

      string Pattern = caseSensitive ? string.Copy(pattern) : pattern.ToLower();
      PatternCharShifts = new BoyerHashTable();

      // Building shift table
      PatternLength = Pattern.Length;
      int MaxShift = PatternLength;

      // Constructing the table where number of columns is equal to PatternLength
      // and number of rows is equal to the number of distinct chars in the pattern
      for (int i = 0; i < PatternLength; i++)
        PatternCharShifts.Add(Pattern[i], new int[PatternLength]);
      OtherCharShifts = new int[PatternLength];

      // Filling the last column of the table with maximum shifts (pattern length)
      foreach (char key in PatternCharShifts.Keys)
        PatternCharShifts.Get(key)[PatternLength - 1] = MaxShift;
      OtherCharShifts[PatternLength - 1] = MaxShift;

      // Calculating other shifts (filling each column from PatternLength - 2 to 0 (from right to left)
      for (int i = PatternLength - 1; i >= 0; i--)
      {
        // Suffix string contains the characters right to the character being processsed
        string Suffix = new String(Pattern.ToCharArray(), i + 1, PatternLength - i - 1);

        // if Pattern begins with Suffix the maximum shift is equal to i + 1
        if (Pattern.StartsWith(Suffix))
          MaxShift = i + 1;

        // Store shift for characters not present in the pattern
        OtherCharShifts[i] = MaxShift;

        // We shorten patter by one char in NewPattern.
        string NewPattern = new string(Pattern.ToCharArray(), 0, Pattern.Length - 1);
        if ((NewPattern.LastIndexOf(Suffix) > 0) || (Suffix.Length == 0))
        {
          foreach (char key in PatternCharShifts.Keys)
          {
            string NewSuffix = key + Suffix;
            // Calculate shifts:
            // Check if there are other occurences of the new suffix in the pattern
            // If several occurences exist, we need the rightmost one
            int NewSuffixPos = NewPattern.LastIndexOf(NewSuffix);
            int shift = NewSuffixPos >= 0 ? i - NewSuffixPos : MaxShift;
            PatternCharShifts.Get(key)[i] = shift;

            // Storing 0 if characters in a row and a columnt are the same
            if (key == Pattern[i])
              PatternCharShifts.Get(key)[i] = 0;
          }
        }
        else
        {
          foreach (char key in PatternCharShifts.Keys)
          {
            // if Suffix doesn't occure in NewPattern we simply use previous shift value
            PatternCharShifts.Get(key)[i] = MaxShift;
            if (key == Pattern[i])
              PatternCharShifts.Get(key)[i] = 0;
          }
        }
      }

      if (!caseSensitive)
      {
        foreach (char key in PatternCharShifts.Keys)
        {
          PatternCharShifts.Add(char.ToUpper(key), PatternCharShifts.Get(key));
        }
      }
    }

    /// <summary>
    /// Searches the input string for the first occurrence of this object's pattern starting at 
    /// the beginning of the string.
    /// </summary>
    /// <param name="input">The string to search in.</param>
    /// <returns>The index of the input if found, or -1 if it is not.</returns>
    public int Search(string input)
    {
      return Search(input, 0);
    }

    /// <summary>
    /// Searches the input string for the first occurrence of this object's pattern.
    /// </summary>
    /// <param name="input">The string to search in.</param>
    /// <param name="startIndex">The zero-based index to start searching.</param>
    /// <returns>The index of the pattern if found, or -1 if it is not.</returns>
    public int Search(string input, int startIndex)
    {
      int curIndex = startIndex;
      while (curIndex <= input.Length - PatternLength)
      {
        for (int i = PatternLength - 1; i >= 0; i--)
        {
          int[] shifts = PatternCharShifts.Get(input[curIndex + i]);
          if (shifts != null)
          {
            // pattern contains char Text[Pos + i]
            int Shift = shifts[i];
            if (Shift != 0)
            {
              curIndex += Shift; // shifting
              break;
            }
            else
              if (i == 0)  // we came to the leftmost pattern character so pattern matches
                return curIndex;  // return matching substring start index
          }
          else
          {
            curIndex += OtherCharShifts[i]; // shifting
            break;
          }
        }
      }

      // Nothing is found
      return -1;
    }

    /// <summary>
    /// Searches an input string for the first occurence of pattern. The search will start at the beginning of input and
    /// will not be case sensitive.
    /// </summary>
    /// <param name="input">The string to search in.</param>
    /// <param name="pattern">The string to search for.</param>
    /// <returns>The index of the pattern if found, or -1 if it is not.</returns>
    public static int Find(string input, string pattern)
    {
      return Find(input, pattern, 0, false);
    }

    /// <summary>
    /// Searches an input string for the first occurence of pattern. The search will not be case sensitive.
    /// </summary>
    /// <param name="input">The string to search in.</param>
    /// <param name="pattern">The string to search for.</param>
    /// <param name="startIndex">The zero-based index to start searching.</param>
    /// <returns>The index of the pattern if found, or -1 if it is not.</returns>
    public static int Find(string input, string pattern, int startIndex)
    {
      return Find(input, pattern, startIndex, false);
    }

    /// <summary>
    /// Searches an input string for the first occurence of pattern. The search will start at the beginning of input.
    /// </summary>
    /// <param name="input">The string to search in.</param>
    /// <param name="pattern">The string to search for.</param>
    /// <param name="caseSensitive">Whether or not the search should be case sensitive.</param>
    /// <returns>The index of the pattern if found, or -1 if it is not.</returns>
    public static int Find(string input, string pattern, bool caseSensitive)
    {
      return Find(input, pattern, 0, caseSensitive);
    }

    /// <summary>
    /// Searches an input string for the first occurence of pattern.
    /// </summary>
    /// <param name="input">The string to search in.</param>
    /// <param name="pattern">The string to search for.</param>
    /// <param name="startIndex">The zero-based index to start searching.</param>
    /// <param name="caseSensitive">Whether or not the search should be case sensitive.</param>
    /// <returns>The index of the pattern if found, or -1 if it is not.</returns>
    public static int Find(string input, string pattern, int startIndex, bool caseSensitive)
    {
      BoyerMoore searcher = new BoyerMoore(pattern, caseSensitive);
      return searcher.Search(pattern, startIndex);
    }

    /// <summary>
    /// Searches an input string for all occurences of pattern. The search will start at the beginning of input and
    /// will not be case sensitive.
    /// </summary>
    /// <param name="input">The string to search in.</param>
    /// <param name="pattern">The string to search for.</param>
    /// <returns>The indices of all occurences of the pattern found, or an empty array if no occurences were found.</returns>
    public static int[] FindAll(string input, string pattern)
    {
      return FindAll(input, pattern, 0, false);
    }

    /// <summary>
    /// Searches an input string for all occurences of pattern. The search will not be case sensitive.
    /// </summary>
    /// <param name="input">The string to search in.</param>
    /// <param name="pattern">The string to search for.</param>
    /// <param name="startIndex">The zero-based index to start searching.</param>
    /// <returns>The indices of all occurences of the pattern found, or an empty array if no occurences were found.</returns>
    public static int[] FindAll(string input, string pattern, int startIndex)
    {
      return FindAll(input, pattern, startIndex, false);
    }

    /// <summary>
    /// Searches an input string for all occurences of pattern. The search will start at the beginning of input.
    /// </summary>
    /// <param name="input">The string to search in.</param>
    /// <param name="pattern">The string to search for.</param>
    /// <param name="caseSensitive">Whether or not the search should be case sensitive.</param>
    /// <returns>The indices of all occurences of the pattern found, or an empty array if no occurences were found.</returns>
    public static int[] FindAll(string input, string pattern, bool caseSensitive)
    {
      return FindAll(input, pattern, 0, caseSensitive);
    }

    /// <summary>
    /// Searches an input string for all occurences of pattern.
    /// </summary>
    /// <param name="input">The string to search in.</param>
    /// <param name="pattern">The string to search for.</param>
    /// <param name="startIndex">The zero-based index to start searching.</param>
    /// <param name="caseSensitive">Whether or not the search should be case sensitive.</param>
    /// <returns>The indices of all occurences of the pattern found, or an empty array if no occurences were found.</returns>
    public static int[] FindAll(string input, string pattern, int startIndex, bool caseSensitive)
    {
      int index = -1;
      int start = startIndex;

      List<int> retval = new List<int>();
      BoyerMoore searcher = new BoyerMoore(pattern, caseSensitive);

      while ((index = searcher.Search(input, start)) >= 0)
      {
        retval.Add(index);
        start = index + pattern.Length;
      }

      return retval.ToArray();
    }
  }
}