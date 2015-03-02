using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Common.References
{
  /// <summary>
  /// 
  /// </summary>
  public struct Index
  {
    /// <summary>
    /// 
    /// </summary>
    public static readonly Index Empty;

    private int col;
    private int row;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="column"></param>
    /// <param name="row"></param>
    public Index(int column, int row)
    {
      this.col = column;
      this.row = row;
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsEmpty
    {
      get { return ((this.col == 0) && (this.row == 0)); }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Column
    {
      get { return this.col; }
      set { this.col = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public int Row
    {
      get { return this.row; }
      set { this.row = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(Index left, Index right)
    {
      return ((left.Column == right.Column) && (left.Row == right.Row));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(Index left, Index right)
    {
      return !(left == right);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
      if (!(obj is Index))
      {
        return false;
      }

      Index point = (Index)obj;
      return ((point.Column == this.Column) && (point.Row == this.Row));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
      return (this.col ^ this.row);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return string.Format("{{Column={0}, Row={1}}}", new object[] { this.col, this.row });
    }

    static Index()
    {
      Empty = new Index();
    }
  }
}
