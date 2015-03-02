using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace System.Common.Extensions
{
  public static partial class XmlLinqExtensions
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="attributeName"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static bool AttributesMatch(this XElement a, XElement b, string attributeName, bool ignoreCase = false)
    {
      var attrA = a.Attribute(attributeName);
      if (attrA == null) { return false; }

      var attrB = b.Attribute(attributeName);
      if (attrB == null) { return false; }

      return string.Equals(attrA.Value ?? "", attrB.Value ?? "",
        ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="elementName"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public static bool ElementsMatch(this XElement a, XElement b, string elementName, bool ignoreCase = false)
    {
      var elemA = a.Element(elementName);
      if (elemA == null) { return false; }

      var elemB = b.Element(elementName);
      if (elemB == null) { return false; }

      return string.Equals(elemA.Value ?? "", elemB.Value ?? "",
        ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);
    }
  }
}