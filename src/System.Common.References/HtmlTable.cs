using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Sgml;

namespace System.Common.References
{
  /// <summary>
  /// 
  /// </summary>
  public static class HtmlTable
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="html"></param>
    /// <returns></returns>
    public static IEnumerable<DataTable> GetTables(string html)
    {
      // get a string reader for the HTML
      var reader = new StringReader(html);

      // setup SgmlReader
      var sgmlReader = new SgmlReader();
      sgmlReader.DocType = "HTML";
      sgmlReader.WhitespaceHandling = WhitespaceHandling.None;
      sgmlReader.CaseFolding = CaseFolding.ToLower;
      sgmlReader.InputStream = reader;

      // create document
      var doc = new XmlDocument();
      doc.PreserveWhitespace = true;
      doc.XmlResolver = null;
      doc.Load(sgmlReader);

      // get the table node
      var tables = doc.SelectNodes("//table");
      for (int i = 0; i < tables.Count; ++i)
      {
        // retrieve the table
        var table = tables[i];

        // create a data table
        var dataTable = new DataTable("Table" + i);

        // the first child is the header row
        var headerRow = table.ChildNodes[0];

        // if the header row is <tbody>, get the children of that
        // tbody
        if (string.Equals("tbody", headerRow.Name))
        {
          table = headerRow;
          headerRow = table.ChildNodes[0];
        }

        // go through each of the defined columns
        foreach (XmlNode column in headerRow.ChildNodes)
        {
          dataTable.Columns.Add((column.InnerText ?? string.Empty).Trim());
        }

        // the rest of the children are the actual data
        for (int c = 1; c < table.ChildNodes.Count; ++c)
        {
          XmlNode row = table.ChildNodes[c];
          List<object> data = new List<object>();
          foreach (XmlNode cell in row.ChildNodes)
          {
            data.Add((cell.InnerText ?? string.Empty).Trim());
          }
          dataTable.Rows.Add(data.ToArray());
        }

        // yield return the data table
        yield return dataTable;
      }
    }
  }
}
