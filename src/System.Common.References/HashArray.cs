using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Common.References
{
  /// <summary>
  /// A container that stores items sorted in ascending hash-code order, and
  /// can be indexed sequentially. Unlike the Hashtable class, it can store
  /// multiple items with the same hash code.
  /// </summary>
  public class HashArray<T> : IEnumerable<T>, ICollection<T>
  {
    /// <summary>
    /// Holds the values in the array.
    /// </summary>
    private T[] _values;

    /// <summary>
    /// Holds the hash codes of the values.
    /// </summary>
    private int[] _hashCodes;

    /// <summary>
    /// The number of items in the array.
    /// </summary>
    private int _count;

    /// <summary>
    /// Used to avoid boxing and unboxing integers for returning a count from IndexOfAllInternal.
    /// </summary>
    private int _outCount;

    /// <summary>Gets the number of items in the array.</summary>
    public int Count { get { return _count; } }

    /// <summary>
    /// Gets or sets the item at the specified zero-based index. The index must
    /// be nonnegative and less than Count. When replacing an existing item,
    /// the new value must have the same hash code or an exception will be thrown.
    /// </summary>
    /// <param name="index">The zero-based index into the array.</param>
    public T this[int index]
    {
      get
      {
        return _values[index];
      }
      set
      {
        int hashCode = value.GetHashCode();
        if (_hashCodes[index] != hashCode)
          throw new ArgumentException(@"Attempted to replace a hash array item with a value having a different hash code");
        _values[index] = value;
      }
    }

    /// <summary>Initializes a new instance of the array.</summary>
    public HashArray()
      : this(16)
    {
    }

    /// <summary>Initializes a new instance of the array.</summary>
    /// <param name="capacity">The initial capacity of the arrray.</param>
    public HashArray(int capacity)
    {
      if (capacity < 1)
        capacity = 1;
      _values = new T[capacity];
      _hashCodes = new int[capacity];
    }

    /// <summary>
    /// Creates a new copy of the array, making a shallow copy
    /// of each element.
    /// </summary>
    /// <param name="other">The CnxHashArray to copy.</param>
    public HashArray(HashArray<T> other)
    {
      _count = other._count;
      _values = new T[other._values.Length];
      _hashCodes = new int[other._hashCodes.Length];
      Array.Copy(other._values, _values, _count);
      Array.Copy(other._hashCodes, _hashCodes, _count);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(T item)
    {
      return IndexOf(item) > -1;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="array"></param>
    /// <param name="arrayIndex"></param>
    public void CopyTo(T[] array, int arrayIndex)
    {
      Array.Copy(_values, 0, array, arrayIndex, _count);
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsReadOnly
    {
      get { return false; }
    }

    /// <summary>
    /// Removes the given item from the array if it exists. Has no effect if the item is not in the array.
    /// </summary>
    /// <param name="item">The item to remove.</param>
    bool ICollection<T>.Remove(T item)
    {
      int before = Count;
      Remove(item);
      return before != Count;
    }

    /// <summary>
    /// Removes all items from the array.
    /// </summary>
    public void Clear()
    {
      Array.Clear(_values, 0, _count); // Necessary in order to release references
      _count = 0;
    }

    /// <summary>
    /// Adds the given item to the array. Items will be inserted in ascending
    /// hash code order, and after any existing items with the same hash code.
    /// </summary>
    /// <param name="value">The value to add.</param>
    public void Add(T value)
    {
      int hashCode = value.GetHashCode();
      int index;
      if (_count == 0 || hashCode >= _hashCodes[_count - 1])
      {
        // Optimize for the common case where objects are added in ascending HashCode order.
        index = _count;
      }
      else
      {
        // Not added in ascending HashCode order so must find the correct insertion point.
        index = this.IndexOfAllInternal(hashCode);

        if (index < 0)
        {
          // No existing item for this hash code so insert one.
          // BinarySearch returns the 1's compliment of the index at which to insert
          // in order to maintain the sort order.
          index = ~index;
        }
        else
        {
          // Insert after the existing item(s)
          index += _outCount;
        }
      }

      InsertAt(value, hashCode, index);
    }

    /// <summary>
    /// Stores the given item in the array, replacing any existing item(s) with
    /// the same hash code. Items will be stored in ascending hash code order.
    /// </summary>
    /// <param name="value">The value to store.</param>
    public void StoreItem(T value)
    {
      int hashCode = value.GetHashCode();
      int index = this.IndexOfAllInternal(hashCode);
      if (index < 0)
      {
        // No existing item for this hash code so insert one.
        // BinarySearch returns the 1's compliment of the index at which to insert
        // in order to maintain the sort order.
        InsertAt(value, hashCode, ~index);
      }
      else
      {
        // Replace the first existing item and then remove the rest
        _values[index] = value;
        _hashCodes[index] = hashCode;
        if (_outCount > 1)
        {
          int removeIndex = index + 1;
          int removeCount = _outCount - 1;

          RemoveRange(removeIndex, removeCount);
        }
      }
    }

    /// <summary>
    /// Removes the given item from the array if it exists. Has no effect if the item is not in the array.
    /// </summary>
    /// <param name="value">The item to remove.</param>
    public void Remove(T value)
    {
      int index = this.IndexOf(value);
      if (index >= 0)
      {
        RemoveRange(index, 1);
      }
    }

    /// <summary>
    /// Removes the item at the given index.
    /// </summary>
    public void RemoveAt(int index)
    {
      RemoveRange(index, 1);
    }

    /// <summary>
    /// Removes all items with the given hash code.
    /// </summary>
    /// <returns>The count of items removed.</returns>
    public int RemoveAll(int hashCode)
    {
      int index = this.IndexOfAllInternal(hashCode);
      if (_outCount > 0)
      {
        RemoveRange(index, _outCount);
      }
      return _outCount;
    }

    /// <summary>
    /// Gets the first item with the given hash code.
    /// </summary>
    /// <param name="hashCode">The hash code for which to search.</param>
    /// <returns>The item with the given hash code, or null if no matching item is found.</returns>
    public T GetItem(int hashCode)
    {
      int index = IndexOf(hashCode);
      if (index < 0)
        return default(T);
      else
        return _values[index];
    }

    /// <summary>
    /// Searches for an item with the given hash code.
    /// </summary>
    /// <returns>The index of the first item with the given hash code, of one is found.
    ///
    /// -or-
    ///
    /// A negative number, which is the bitwise complement of the index at which an item
    /// with the given hash code would be inserted to maintain the sort order.
    /// </returns>
    /// <param name="hashCode">The hash code for which to search.</param>
    public int IndexOf(int hashCode)
    {
      // Design note: This function is highly optimized because it can get called thousands of
      // times during reception of each In-Sight result set. Optimization reduced its execution
      // time from 36 microseconds to 1 microsecond on VisionView 700 with 1000 items in the array.

      unsafe
      {
        fixed (int* hashCodes = _hashCodes)
        {
          // Perform a binary search for the requested hash code.
          int index = 0;
          int endIndex = _count - 1;
          while (index <= endIndex)
          {
            int location = index + ((endIndex - index) >> 1);
            int hash = hashCodes[location];
            if (hash == hashCode)
            {
              // This may not be the first item with the given hash code so back
              // up as long as the hash code matches.
              while (location > 0 && hashCodes[location - 1] == hashCode)
                location--;
              return location;
            }
            if (hash < hashCode)
            {
              index = location + 1;
            }
            else
            {
              endIndex = location - 1;
            }
          }
          return ~index;
        }
      }
    }

    /// <summary>
    /// Searches for a specific item.
    /// </summary>
    /// <returns>The index of the given item, or -1 if not found.
    /// </returns>
    /// <param name="value">The item for which to search.</param>
    public int IndexOf(T value)
    {
      // Locate the index of the first item that matches value's hash code
      int hashCode = value.GetHashCode();
      int index = IndexOf(hashCode);

      // If there's at least one item with a matching hash
      if (index >= 0)
      {
        // Get the first item
        object obj = _values[index];
        while (true)
        {
          // If we have a match, return its index
          if (obj.Equals(value))
            return index;

          // Advance to the next item and quit if beyond the end of the array
          index++;
          if (index >= Count)
            break;

          // Quit if the hash code doesn't match
          if (_hashCodes[index] != hashCode)
            break;

          // Get the next value and keep checking
          obj = _values[index];
        }
      }

      return -1; // No matching item found
    }

    /// <summary>
    /// Searches for items with the given hash code.
    /// </summary>
    /// <returns>The index of the first item with the given hash code, of one is found.
    ///
    /// -or-
    ///
    /// A negative number, which is the bitwise complement of the index at which an item
    /// with the given hash code should be inserted to maintain the sort order.
    /// </returns>
    /// <param name="hashCode">The hash code for which to search.</param>
    /// <param name="count">A reference in which to return the number of items with the given hash code.</param>
    public int IndexOfAll(int hashCode, out int count)
    {
      int index = IndexOfAllInternal(hashCode);
      count = _outCount;
      return index;
    }

    /// <summary>
    /// Internal implementation of IndexOfAll, avoids boxing/unboxing of the count parameter via _outCount.
    /// </summary>
    /// <param name="hashCode">The hash code for which to search.</param>
    private int IndexOfAllInternal(int hashCode)
    {
      // Find the first matching item
      int startIndex = IndexOf(hashCode);

      if (startIndex < 0)
        _outCount = 0; // no match
      else
      {
        // Check subsequent items until we run off the end or find one with a hash
        // code that doesn't match
        int nextIndex = startIndex + 1;
        while (nextIndex < _count && _hashCodes[nextIndex] == hashCode)
          nextIndex++;

        // Return the count of matching items
        _outCount = nextIndex - startIndex;
      }
      return startIndex;
    }

    /// <summary>
    /// Inserts a new value at a specific index.
    /// </summary>
    /// <param name="value">The value to insert.</param>
    /// <param name="hashCode">The value's hash code.</param>
    /// <param name="index">The index at which to insert the value, so that the
    /// hash code sort order is maintained.</param>
    private void InsertAt(T value, int hashCode, int index)
    {
      // Ensure capacity to store one more item
      int current = _values.Length;
      if (_count >= current)
      {
        T[] newValues = new T[current * 2];
        int[] newHashcodes = new int[current * 2];
        Array.Copy(_values, newValues, _count);
        Array.Copy(_hashCodes, newHashcodes, _count);
        _values = newValues;
        _hashCodes = newHashcodes;
      }

      if (index < _count)
      {
        Array.Copy(_values, index, _values, index + 1, _count - index);
        Array.Copy(_hashCodes, index, _hashCodes, index + 1, _count - index);
      }
      _values[index] = value;
      _hashCodes[index] = hashCode;
      _count++;
    }

    /// <summary>
    /// Removes a range of items.
    /// </summary>
    /// <param name="removeIndex">The index at which to start removing.</param>
    /// <param name="removeCount">The number of items to remove.</param>
    private void RemoveRange(int removeIndex, int removeCount)
    {
      _count -= removeCount;
      if (removeIndex < _count)
      {
        Array.Copy(_values, removeIndex + removeCount, _values, removeIndex, _count - removeIndex);
        Array.Copy(_hashCodes, removeIndex + removeCount, _hashCodes, removeIndex, _count - removeIndex);
      }
      Array.Clear(_values, _count, removeCount);
    }

    /// <summary>
    /// Returns an IEnumerator that can iterate through the array.
    /// </summary>
    public IEnumerator<T> GetEnumerator()
    {
      return new HashArrayEnumerator(_values, _count);
    }

    /// <summary>
    /// Returns an IEnumerator that can iterate through the array.
    /// </summary>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return new HashArrayEnumerator(_values, _count);
    }

    #region HashArrayEnumerator

    /// <summary>
    /// Enumerates over the values.
    /// </summary>
    internal class HashArrayEnumerator : IEnumerator<T>, ICloneable
    {
      T[] _array;
      int _count;
      int _index;

      public HashArrayEnumerator(T[] array, int count)
      {
        _array = array;
        _count = count;
        _index = -1;
      }

      #region IEnumerator<T> Members

      T IEnumerator<T>.Current
      {
        get
        {
          if (_index < 0)
          {
            throw new InvalidOperationException();
          }
          if (_index >= _count)
          {
            throw new InvalidOperationException();
          }
          return _array[_index];
        }
      }

      #endregion

      #region IDisposable Members

      public void Dispose()
      {
        if (_array != null)
        {
          Array.Clear(_array, 0, _array.Length);
          _array = null;
        }
      }

      #endregion

      #region IEnumerator Members

      object System.Collections.IEnumerator.Current
      {
        get
        {
          if (_index < 0)
          {
            throw new InvalidOperationException();
          }
          if (_index >= _count)
          {
            throw new InvalidOperationException();
          }
          return _array[_index];
        }
      }

      bool System.Collections.IEnumerator.MoveNext()
      {
        if (_index >= _count)
          return false;
        _index++;
        return (_index < _count);
      }

      void System.Collections.IEnumerator.Reset()
      {
        _index = -1;
      }

      #endregion

      #region ICloneable Members

      object ICloneable.Clone()
      {
        return MemberwiseClone();
      }

      #endregion
    }

    #endregion
  }
}