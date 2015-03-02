using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace System.Common.References
{
  /// <summary>
  /// 
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  public delegate void GridEXRowDragEventHandler(object sender, GridEXRowDragEventArgs e);

  /// <summary>
  /// 
  /// </summary>
  public class GridEXRowDragEventArgs : EventArgs
  {
    public GridEXRow Row { get; private set; }
    public GridEXRowDragEventArgs(GridEXRow row)
    {
      Row = row;
    }
  }

  /// <summary>
  /// Adds drag and drop to a GridEX by exposing a DraggingRow event.
  /// </summary>
  public class GridEXDragDropHandler : IDisposable
  {
    private Rectangle dragDropRectangle;
    private Point dragDropStartPoint;
    private GridEX mGrid;
    private bool mDisposed;

    /// <summary>
    /// 
    /// </summary>
    public event GridEXRowDragEventHandler DraggingRow = (x, y) => { };

    /// <summary>
    /// 
    /// </summary>
    public GridEX Grid { get { return mGrid; } }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="grid"></param>
    public GridEXDragDropHandler(GridEX grid)
    {
      mGrid = grid;
      mDisposed = false;

      mGrid.MouseDown += new MouseEventHandler(mGrid_MouseDown);
      mGrid.MouseMove += new MouseEventHandler(mGrid_MouseMove);
      mGrid.MouseUp += new MouseEventHandler(mGrid_MouseUp);
    }

    ~GridEXDragDropHandler()
    {
      // Do not re-create Dispose clean-up code here.
      // Calling Dispose(false) is optimal in terms of
      // readability and maintainability.
      Dispose(false);
    }

    /// <summary>
    /// Removes the dragging row event
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
      // This object will be cleaned up by the Dispose method.
      // Therefore, you should call GC.SupressFinalize to
      // take this object off the finalization queue
      // and prevent finalization code for this object
      // from executing a second time.
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      // Check to see if Dispose has already been called.
      if (!mDisposed)
      {
        // If disposing equals true, dispose all managed
        // resources.
        if (disposing)
        {
          // Dispose managed resources.
          if (mGrid != null)
          {
            mGrid.MouseDown -= new MouseEventHandler(mGrid_MouseDown);
            mGrid.MouseMove -= new MouseEventHandler(mGrid_MouseMove);
            mGrid.MouseUp -= new MouseEventHandler(mGrid_MouseUp);
            mGrid = null;
            DraggingRow = null;
          }
        }

        // Note disposing has been done.
        mDisposed = true;
      }
    }

    private void mGrid_MouseUp(object sender, MouseEventArgs e)
    {
      dragDropRectangle = Rectangle.Empty;
      dragDropStartPoint = Point.Empty;
    }

    private void mGrid_MouseMove(object sender, MouseEventArgs e)
    {
      if ((e.Button & System.Windows.Forms.MouseButtons.Left) != 0
        && !dragDropRectangle.IsEmpty
        && !dragDropRectangle.Contains(e.X, e.Y))
      {
        var pos = mGrid.RowPositionFromPoint(dragDropStartPoint.X, dragDropStartPoint.Y);
        var row = mGrid.GetRow(pos);
        DraggingRow(this, new GridEXRowDragEventArgs(row));
      }
    }

    private void mGrid_MouseDown(object sender, MouseEventArgs e)
    {
      if ((e.Button & System.Windows.Forms.MouseButtons.Left) != 0)
      {
        var hitTest = mGrid.HitTest(e.X, e.Y);
        if (hitTest == Janus.Windows.GridEX.GridArea.Cell)
        {
          // this is a row of some kind!
          var dragSize = SystemInformation.DragSize;
          dragDropRectangle = new Rectangle(new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2)), dragSize);
          dragDropStartPoint = new Point(e.X, e.Y);
        }
        else
        {
          dragDropRectangle = Rectangle.Empty;
        }
      }
      else
      {
        dragDropRectangle = Rectangle.Empty;
      }
    }
  }
}