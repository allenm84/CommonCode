using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.Common.Extensions
{
  public static partial class ReflectionExtensions
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsNullable(this Type type)
    {
      return type.IsGenericType
        && (type.GetGenericTypeDefinition() == typeof(Nullable<>));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="interfaceType"></param>
    /// <returns></returns>
    public static bool ImplementsInterface(this Type type, Type interfaceType)
    {
      if (!interfaceType.IsInterface)
      {
        throw new ArgumentException("interfaceType is not an interface");
      }
      return type.GetInterfaces().Contains(interfaceType);
    }

    /// <summary>
    /// Determines if a property has an attribute.
    /// </summary>
    /// <param name="property">The property to check.</param>
    /// <param name="attributeType">The type of the attribute</param>
    /// <returns></returns>
    public static bool HasAttribute(this PropertyInfo property, Type attributeType)
    {
      var attributes = property.GetCustomAttributes(attributeType, false);
      return attributes != null && attributes.Length > 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="attributeType"></param>
    /// <returns></returns>
    public static bool HasAttribute(this Type type, Type attributeType)
    {
      var attributes = type.GetCustomAttributes(attributeType, false);
      return attributes != null && attributes.Length > 0;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="properties"></param>
    /// <returns></returns>
    public static IEnumerable<string> PropertyList(this object obj, params string[] properties)
    {
      var type = obj.GetType();
      return properties.Select(prop =>
      {
        var info = type.GetProperty(prop);
        return string.Format("{0}: {1}", prop, info.GetValue(obj, null));
      });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string ExpandObject(this object obj)
    {
      var sb = new StringBuilder();
      PrintProperties(obj, 0, sb);
      return sb.ToString();
    }

    private static void PrintProperties(object obj, int indent, StringBuilder sb)
    {
      if (obj == null) { return; }
      var indentString = new string(' ', indent);
      var objType = obj.GetType();
      var properties = objType.GetProperties();
      foreach (var property in properties)
      {
        var propValue = property.GetValue(obj, null);
        if (property.PropertyType.Assembly == objType.Assembly)
        {
          sb.AppendFormat("{0}{1}:", indentString, property.Name);
          sb.AppendLine();

          if (property.PropertyType.Equals(objType)) { continue; }
          PrintProperties(propValue, indent + 2, sb);
        }
        else
        {
          sb.AppendFormat("{0}{1}: {2}", indentString, property.Name, propValue);
          sb.AppendLine();
        }
      }
    }
  }
}