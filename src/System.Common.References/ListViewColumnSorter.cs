using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace System.Common.References
{
  /// <summary>
  /// 
  /// </summary>
  public interface IListViewColumnSorter : IComparer
  {
    /// <summary>
    /// 
    /// </summary>
    int SortColumn { get; set; }

    /// <summary>
    /// 
    /// </summary>
    SortOrder Order { get; set; }
  }

  /// <summary>
  /// 
  /// </summary>
  public sealed class ListViewColumnSorter : IListViewColumnSorter
  {
    /// <summary>
    /// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
    /// </summary>
    public int SortColumn { get; set; }

    /// <summary>
    /// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
    /// </summary>
    public SortOrder Order { get; set; }

    /// <summary>
    /// Class constructor.
    /// </summary>
    public ListViewColumnSorter()
    {
      // Initialize the column to '0'
      SortColumn = 0;

      // Initialize the sort order to 'none'
      Order = SortOrder.None;
    }

    /// <summary>
    /// This method is inherited from the IComparer interface.  It compares the two objects passed using a case insensitive comparison.
    /// </summary>
    /// <param name="x">First object to be compared</param>
    /// <param name="y">Second object to be compared</param>
    /// <returns>The result of the comparison. "0" if equal, negative if 'x' is less than 'y' and positive if 'x' is greater than 'y'</returns>
    public int Compare(object x, object y)
    {
      ListViewItem listviewX = (ListViewItem)x;
      ListViewItem listviewY = (ListViewItem)y;

      // get the text
      string textX = listviewX.SubItems[SortColumn].Text;
      string textY = listviewY.SubItems[SortColumn].Text;

      // compare the two items
      int compareResult = compareResult = string.Compare(textX, textY, true);

      // Calculate correct return value based on object comparison
      if (Order == SortOrder.Ascending)
      {
        // Ascending sort is selected, return normal result of compare operation
        return compareResult;
      }
      else if (Order == SortOrder.Descending)
      {
        // Descending sort is selected, return negative result of compare operation
        return (-compareResult);
      }
      else
      {
        return 0;
      }
    }
  }
}
