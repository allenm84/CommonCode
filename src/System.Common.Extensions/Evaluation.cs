using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.XPath;

namespace System.Common.Extensions
{
  public static partial class EvaluationExtensions
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static double Evaluate(this string expression)
    {
      return (double)new XPathDocument(
        new StringReader("<r/>")).CreateNavigator().Evaluate(
          string.Format("number({0})", new Regex(@"([\+\-\*])").
            Replace(expression, " ${1} ").
            Replace("/", " div ").
            Replace("%", " mod ")));
    }
  }
}