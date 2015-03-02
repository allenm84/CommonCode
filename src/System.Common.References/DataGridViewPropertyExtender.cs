using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace System.Common.References
{
  public class DataGridViewPropertyExtender : INotifyPropertyChanged
  {
    private DataGridView mGrid;

    public event PropertyChangedEventHandler PropertyChanged = (x, y) => { };

    /// <summary>
    /// Gets the current row count. This object can be used as the datasource when databinding so that
    /// the object binding to this will be updated when the count changes.
    /// </summary>
    public int Count { get { return mGrid.Rows.Count; } }

    public DataGridViewPropertyExtender(DataGridView grid)
    {
      mGrid = grid;
      mGrid.RowsAdded += new DataGridViewRowsAddedEventHandler(mGrid_RowsAdded);
      mGrid.RowsRemoved += new DataGridViewRowsRemovedEventHandler(mGrid_RowsRemoved);
    }

    private void mGrid_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
    {
      PropertyChanged(this, new PropertyChangedEventArgs("Count"));
    }

    private void mGrid_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
    {
      PropertyChanged(this, new PropertyChangedEventArgs("Count"));
    }
  }
}
