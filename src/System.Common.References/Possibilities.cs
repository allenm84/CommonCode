using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Common.References
{
  public static class Possibilities
  {
    class State<T>
    {
      public int index;
      public T[] array;

      public T Value { get { return array[index]; } }
      public bool Valid { get { return (-1 < index && index < array.Length); } }
    }

    /// <summary>
    /// Calculate all possible combinations from multiple arrays.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="arrays"></param>
    /// <returns></returns>
    public static IEnumerable<T[]> Calculate<T>(IList<T[]> arrays)
    {
      var states = arrays
        .Select(a => new State<T> { array = a, index = 0 })
        .ToArray();

      bool keepGoing = true;
      int currentStateIndex = 0;

      while (keepGoing)
      {
        // return the current state
        yield return states.Select(s => s.Value).ToArray();

        // create a variable for the current state
        currentStateIndex = states.Length - 1;

        // retrieve the current state
        var current = states[currentStateIndex];

        // increment the current state
        current.index++;

        // if we passed the length of the array, then go up
        while (!current.Valid)
        {
          // reset this state
          current.index = 0;

          // move up to the parent state
          --currentStateIndex;

          // set the current state
          if (currentStateIndex > -1)
          {
            current = states[currentStateIndex];
            current.index++;
          }
          else
          {
            // we moved as far up as possible
            keepGoing = false;
            break;
          }
        }
      }
    }
  }
}
