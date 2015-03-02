using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace System.Common.References
{
  public static class TypeCreator
  {
    private class TypeCreatorItem : IComparable<TypeCreatorItem>
    {
      public Type type;
      public Func<object> Create;

      public int CompareTo(TypeCreatorItem other)
      {
        return type.FullName.CompareTo(other.type.FullName);
      }
    }

    static List<TypeCreatorItem> factories = new List<TypeCreatorItem>();
    static object syncRoot = new object();

    static TypeCreator()
    {
      add(typeof(string), () => "");
      add(typeof(Version), () => new Version(0, 0, 0, 0));
    }

    private static int indexOf(Type type)
    {
      TypeCreatorItem item = new TypeCreatorItem { type = type };
      return factories.BinarySearch(item);
    }

    private static bool contains(Type type, bool doLock = false)
    {
      if (doLock)
        Monitor.Enter(syncRoot);

      int index = indexOf(type);

      if (doLock)
        Monitor.Exit(syncRoot);

      return index > -1;
    }

    private static void unsafeAdd(Type type, Func<object> creator)
    {
      factories.Add(new TypeCreatorItem { type = type, Create = creator });
      factories.Sort();
    }

    private static void inject(Type type, Func<object> creator, bool doLock = false)
    {
      if (type == null) throw new ArgumentNullException("type");
      if (creator == null) throw new ArgumentNullException("creator");

      if (doLock)
        Monitor.Enter(syncRoot);

      int index = indexOf(type);
      if (index < 0)
      {
        index = factories.Count;
        unsafeAdd(type, null);
      }
      factories[index].Create = creator;

      if (doLock)
        Monitor.Exit(syncRoot);
    }

    private static bool add(Type type, Func<object> creator, bool doLock = false)
    {
      if (type == null) throw new ArgumentNullException("type");
      if (creator == null) throw new ArgumentNullException("creator");

      if (doLock)
        Monitor.Enter(syncRoot);

      bool exists = contains(type);
      if (!exists)
      {
        unsafeAdd(type, creator);
      }

      if (doLock)
        Monitor.Exit(syncRoot);

      return exists;
    }

    private static bool remove(Type type, bool doLock = false)
    {
      if (type == null) throw new ArgumentNullException("type");

      if (doLock)
        Monitor.Enter(syncRoot);

      int index = indexOf(type);
      if (index > -1)
        factories.RemoveAt(index);

      if (doLock)
        Monitor.Exit(syncRoot);

      return index > -1;
    }

    /// <summary>
    /// Calls the creator function for the specified type. If the type isn't found, then
    /// Activator.CreateInstance is used.
    /// </summary>
    /// <param name="type">The type to create.</param>
    /// <returns>The created object.</returns>
    /// <exception cref="ArgumentNullException">If the type or the creator found are null.</exception>
    public static object Create(Type type)
    {
      if (type == null) throw new ArgumentNullException("type");
      lock (syncRoot)
      {
        int index = indexOf(type);
        if (index < 0)
        {
          return Activator.CreateInstance(type);
        }
        else
        {
          return factories[index].Create();
        }
      }
    }

    /// <summary>
    /// Determines if the type is stored.
    /// </summary>
    /// <param name="type">The type to check for.</param>
    /// <returns>true if the type is contained; otherwise false.</returns>
    /// <exception cref="ArgumentNullException">If the type is null.</exception>
    public static bool Contains(Type type)
    {
      if (type == null) throw new ArgumentNullException("type");
      return contains(type, true);
    }

    /// <summary>
    /// Stores the type and it's creator.
    /// </summary>
    /// <param name="type">The type to add.</param>
    /// <param name="creator">The factory used to create the type.</param>
    /// <exception cref="ArgumentNullException">If the type or creator are null.</exception>
    /// <exception cref="Exception">If the type already exists.</exception>
    public static void Add(Type type, Func<object> creator)
    {
      if (add(type, creator, true))
      {
        throw new Exception("The type already exists.");
      }
    }

    /// <summary>
    /// Stores the type and it's creator. If the type exists, it's creator
    /// is overwritten.
    /// </summary>
    /// <param name="type">The type to add.</param>
    /// <param name="creator">The factory used to create the type.</param>
    public static void Inject(Type type, Func<object> creator)
    {
      inject(type, creator, true);
    }

    /// <summary>
    /// Attempts to store the type and it's creator.
    /// </summary>
    /// <param name="type">The type to add.</param>
    /// <param name="creator">The factory used to create the type.</param>
    /// <returns>true if the type was added; otherwise false.</returns>
    public static bool TryAdd(Type type, Func<object> creator)
    {
      return !add(type, creator, true);
    }

    /// <summary>
    /// Removes the type and it's creator.
    /// </summary>
    /// <param name="type">The type to remove.</param>
    /// <returns>true if the type was removed; otherwise false (if the type wasn't stored).</returns>
    public static bool Remove(Type type)
    {
      return remove(type, true);
    }
  }
}