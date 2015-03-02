using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace System.Common.Extensions
{
  public static partial class DataContractExtensions
  {
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="target"></param>
    /// <param name="source"></param>
    public static void CopyFrom<T>(this T target, T source)
    {
      var type = typeof(T);
      var properties = type.GetProperties();
      foreach (var property in properties)
      {
        if (property.HasAttribute(typeof(DataMemberAttribute)))
        {
          var value = property.GetValue(source, null);
          property.SetValue(target, value, null);
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="original"></param>
    /// <returns></returns>
    public static List<T> Duplicate<T>(this List<T> original) where T : class
    {
      var retval = new List<T>();

      var type = typeof(T);
      if (type.HasAttribute(typeof(DataContractAttribute)))
      {
        var dcs = new DataContractSerializer(typeof(List<T>));
        using (var stream = new MemoryStream())
        {
          dcs.WriteObject(stream, original);
          stream.Seek(0, SeekOrigin.Begin);
          retval.AddRange(dcs.ReadObject(stream) as List<T>);
        }
      }

      return retval;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="original"></param>
    /// <returns></returns>
    public static T Duplicate<T>(this T original) where T : class
    {
      var retval = default(T);
      try { retval = Activator.CreateInstance<T>(); }
      catch
      {
        retval = default(T);
      }

      var type = typeof(T);
      if (type.HasAttribute(typeof(DataContractAttribute)))
      {
        var dcs = new DataContractSerializer(type);
        using (var stream = new MemoryStream())
        {
          dcs.WriteObject(stream, original);
          stream.Seek(0, SeekOrigin.Begin);
          retval = dcs.ReadObject(stream) as T;
        }
      }

      return retval;
    }
  }
}