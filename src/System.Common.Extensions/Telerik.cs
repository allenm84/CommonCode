using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace System.Common.Extensions
{
  public enum GridViewColumnDataType
  {
    Text,
    Numeric,
    DropDown,
    CheckBox,
  }

  public static partial class TelerikExtensions
  {
    private static List<GridViewIncrementalSearchData> radGridViewIncrementalSearch = new List<GridViewIncrementalSearchData>();
    private static List<ColumnFormat> columnFormats = new List<ColumnFormat>();
    private static Dictionary<RadGridView, string> groupByYearMonthWeek = new Dictionary<RadGridView, string>();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="list"></param>
    public static void ArrowVisible(this RadDropDownList list, bool visible)
    {
      list.DropDownListElement.ArrowButton.Visibility = visible ?
        ElementVisibility.Visible :
        ElementVisibility.Collapsed;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="listView"></param>
    /// <returns></returns>
    public static int GetVScrollWidth(this RadListView listView)
    {
      return listView.ListViewElement.ViewElement.VScrollBar.Size.Width;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="list"></param>
    public static void AutoFocus(this RadDropDownList list)
    {
      list.GotFocus -= new EventHandler(list_GotFocus);
      list.GotFocus += new EventHandler(list_GotFocus);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void list_GotFocus(object sender, EventArgs e)
    {
      var list = sender as RadDropDownList;
      if (list.DropDownStyle == Telerik.WinControls.RadDropDownStyle.DropDownList &&
        !list.DropDownListElement.ContainsFocus)
      {
        list.DropDownListElement.Focus();
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="listView"></param>
    /// <param name="column"></param>
    /// <param name="format"></param>
    public static void SetColumnFormat(this RadListView listView, string column, string format)
    {
      if (!ColumnFormat.listViews.Contains(listView))
      {
        ColumnFormat.listViews.Add(listView);
      }

      var value = new ColumnFormat {listView = listView, column = column, format = format};
      var index = columnFormats.BinarySearch(value);
      if (index < 0)
      {
        columnFormats.Insert(~index, value);
      }
      else
      {
        columnFormats[index] = value;
      }

      listView.CellFormatting -= new ListViewCellFormattingEventHandler(listView_CellFormatting);
      listView.CellFormatting += new ListViewCellFormattingEventHandler(listView_CellFormatting);
    }

    /// <summary>
    /// 
    /// </summary>
    private static void listView_CellFormatting(object sender, ListViewCellFormattingEventArgs e)
    {
      var cell = e.CellElement as DetailListViewDataCellElement;
      if (cell == null) { return; }

      foreach (var value in columnFormats)
      {
        if (!ReferenceEquals(sender, value.listView.ListViewElement)) { continue; }
        if (e.CellElement.Data.FieldName == value.column)
        {
          e.CellElement.Text = string.Format(value.format, cell.Row[e.CellElement.Data]);
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="columns"></param>
    /// <param name="name"></param>
    /// <param name="index"></param>
    public static void Move(this ListViewColumnCollection columns, string name, int index)
    {
      columns.Move(columns.IndexOf(name), index);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gridView"></param>
    /// <param name="evaluator"></param>
    public static void SetGroupHeaderFormat(this RadGridView gridView, Func<GroupSummaryEvaluationEventArgs, string> evaluator)
    {
      gridView.GroupSummaryEvaluate += (o, e) => e.FormatString = evaluator(e);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gridView"></param>
    /// <param name="defaultValue"></param>
    public static void SetGroupHeaderFormat(this RadGridView gridView, string defaultValue = "(none)")
    {
      gridView.SetGroupHeaderFormat(e => string.IsNullOrEmpty(e.Group.Header) ? defaultValue : "{1}");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="listView"></param>
    public static void AutoFitColumns(this RadListView listView)
    {
      var availableWidth = listView.Width - (listView.ListViewElement.ViewElement.VScrollBar.Size.Width + 4);
      var columns = listView.Columns.Where(c => c.Visible).ToArray();

      foreach (var col in columns)
      {
        col.Width = availableWidth / columns.Length;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="listView"></param>
    public static void AutoSizeColumns(this RadListView listView)
    {
      using (var defer = listView.DeferRefresh())
      {
        var sizes = listView
          .Items
          .Select(i =>
          {
            var cols = new List<int>();
            for (var m = 0; m < i.FieldCount; ++m)
            {
              object value = i[m];
              if (value == null)
              {
                value = "";
              }
              cols.Add(TextRenderer.MeasureText(value.ToString(), listView.Font).Width + 4);
            }
            return cols.ToArray();
          })
          .ToArray();

        for (var c = 0; c < listView.Columns.Count; ++c)
        {
          var col = listView.Columns[c];
          if (!col.Visible) { continue; }

          if (sizes.Length == 0) { col.Width = 1; }
          else
          { col.Width = Math.Max(1, sizes.Max(s => s[c])); }
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="listView"></param>
    /// <returns></returns>
    public static IDisposable DeferRefresh(this RadListView listView)
    {
      return new ListViewDeferUpdate(listView);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="columns"></param>
    /// <param name="name"></param>
    /// <param name="width"></param>
    /// <param name="centerText"></param>
    public static void LockColumnWidth(this ListViewColumnCollection columns, string name, int width)
    {
      var column = columns[name];
      column.MinWidth = width;
      column.Width = width;
      column.MaxWidth = width;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="columns"></param>
    /// <param name="names"></param>
    public static void Hide(this ListViewColumnCollection columns, params string[] names)
    {
      foreach (var column in columns)
      {
        column.Visible = !names.Contains(column.Name);
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="columns"></param>
    /// <param name="names"></param>
    public static void HideAllExcept(this ListViewColumnCollection columns, params string[] names)
    {
      foreach (var column in columns)
      {
        column.Visible = names.Contains(column.Name);
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="list"></param>
    /// <param name="multiSelect"></param>
    /// <param name="allowRemove"></param>
    public static void SetPropertiesToList(this RadListView list, bool multiSelect = false, bool allowRemove = false)
    {
      list.AllowEdit = false;
      list.AllowRemove = allowRemove;
      list.FullRowSelect = true;
      list.MultiSelect = multiSelect;
      list.ShowColumnHeaders = false;
      list.ShowGridLines = false;
      list.ViewType = ListViewType.DetailsView;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="column"></param>
    public static void EnableIncrementalSearch(this RadGridView grid, Func<GridViewRowInfo, string> getText)
    {
      if (radGridViewIncrementalSearch.Any(d => object.ReferenceEquals(d.Grid, grid))) { return; }

      radGridViewIncrementalSearch.Add(new GridViewIncrementalSearchData
      {
        Buffer = new StringBuilder(),
        Grid = grid,
        Start = DateTime.Now,
        GetText = getText,
      });

      grid.KeyPress += new KeyPressEventHandler(grid_KeyPress);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void grid_KeyPress(object sender, KeyPressEventArgs e)
    {
      var data = radGridViewIncrementalSearch.SingleOrDefault(d => object.ReferenceEquals(d.Grid, sender));
      if (data == null) { return; }

      if ((DateTime.Now - data.Start).TotalSeconds >= 2)
      {
        data.Buffer.Clear();
      }

      data.Buffer.Append(e.KeyChar);
      var searchString = data.Buffer.ToString();

      foreach (var row in data.Grid.ChildRows)
      {
        var text = data.GetText(row);
        if (text.StartsWith(searchString, StringComparison.InvariantCultureIgnoreCase))
        {
          data.Grid.ClearSelection();

          row.IsSelected = true;
          data.Grid.TableElement.ScrollToRow(row);

          break;
        }
      }

      data.Start = DateTime.Now;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="strip"></param>
    public static void SetPropertiesToToolStrip(this CommandBarStripElement strip)
    {
      strip.StretchHorizontally = true;
      strip.UpdateVisibility(false, false);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="strip"></param>
    /// <param name="overflowVisible"></param>
    /// <param name="gripVisible"></param>
    public static void UpdateVisibility(this CommandBarStripElement strip, bool overflowVisible, bool gripVisible)
    {
      strip.OverflowButton.Visibility = overflowVisible ? ElementVisibility.Visible : ElementVisibility.Hidden;
      strip.Grip.Visibility = gripVisible ? ElementVisibility.Visible : ElementVisibility.Hidden;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="grid"></param>
    public static void RemoveCellBorder(this RadGridView grid)
    {
      grid.CellFormatting += new CellFormattingEventHandler(grid_CellFormatting_RemoveCellBorder);
    }

    /// <summary>
    /// 
    /// </summary>
    private static void grid_CellFormatting_RemoveCellBorder(object sender, CellFormattingEventArgs e)
    {
      if (e.CellElement != null)
      {
        e.CellElement.DrawBorder = false;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="grid"></param>
    public static void RemoveCellFocus(this RadGridView grid)
    {
      grid.CellFormatting += new CellFormattingEventHandler(grid_CellFormatting_RemoveFocus);
    }

    /// <summary>
    /// 
    /// </summary>
    private static void grid_CellFormatting_RemoveFocus(object sender, CellFormattingEventArgs e)
    {
      if (e.CellElement != null)
      {
        if (e.CellElement.IsCurrent)
        {
          e.CellElement.IsCurrent = false;
          e.CellElement.IsCurrentColumn = false;
        }
        else
        {
          e.CellElement.ResetValue(LightVisualElement.DrawBorderProperty, ValueResetFlags.Local);
          e.CellElement.ResetValue(LightVisualElement.DrawFillProperty, ValueResetFlags.Local);
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="height"></param>
    /// <param name="includeHeader"></param>
    public static void SetRowHeight(this RadGridView grid, int height, bool includeHeader = true)
    {
      grid.TableElement.ChildRowHeight = height;
      grid.TableElement.RowHeight = height;

      if (includeHeader)
      {
        grid.TableElement.GroupHeaderHeight = height;
        grid.TableElement.FilterRowHeight = height;
        grid.TableElement.TableHeaderHeight = height;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="columns"></param>
    /// <param name="name"></param>
    /// <param name="index"></param>
    public static void Move(this GridViewColumnCollection columns, string name, int index)
    {
      columns.Move(columns[name].Index, index);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="text"></param>
    public static void SetEmptyText(this RadGridView grid, string text)
    {
      if (grid.RowCount == 0)
      {
        grid.TableElement.Text = "";
        grid.TableElement.Text = text;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="waitingBar"></param>
    /// <param name="waiting"></param>
    public static void SetIsWaiting(this RadWaitingBar waitingBar, bool waiting)
    {
      if (waiting && !waitingBar.IsWaiting) { waitingBar.StartWaiting(); }
      else if (!waiting && waitingBar.IsWaiting) { waitingBar.StopWaiting(); }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="descriptors"></param>
    /// <param name="name"></param>
    public static int Add(this GroupDescriptorCollection descriptors, string name)
    {
      int count = descriptors.Count;
      descriptors.Add(name, ListSortDirection.Ascending);
      return count;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="descriptors"></param>
    /// <param name="name"></param>
    public static int Add(this SortDescriptorCollection descriptors, string name)
    {
      int count = descriptors.Count;
      descriptors.Add(name, ListSortDirection.Ascending);
      return count;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    public static bool SetRow(this RadGridView grid, int row)
    {
      var success = true;
      try
      {
        grid.ChildRows[row].IsCurrent = true;
      }
      catch
      {
        success = false;
      }
      return success;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="columns"></param>
    /// <param name="name"></param>
    /// <param name="width"></param>
    /// <param name="centerText"></param>
    public static void LockColumnWidth(this GridViewColumnCollection columns, string name, int width, bool centerText = true)
    {
      var column = columns[name];
      column.MinWidth = width;
      column.Width = width;
      column.MaxWidth = width;
      if (centerText)
      {
        column.TextAlignment = ContentAlignment.MiddleCenter;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="columns"></param>
    /// <param name="names"></param>
    public static void Hide(this GridViewColumnCollection columns, params string[] names)
    {
      foreach (var column in columns)
      {
        column.IsVisible = !names.Contains(column.Name);
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="columns"></param>
    /// <param name="names"></param>
    public static void HideAllExcept(this GridViewColumnCollection columns, params string[] names)
    {
      foreach (var column in columns)
      {
        column.IsVisible = names.Contains(column.Name);
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="multiselect"></param>
    /// <param name="allowDelete"></param>
    /// <param name="removeBorder"></param>
    public static void SetPropertiesToList(this GridViewTemplate grid, bool multiselect, bool allowDelete, bool removeBorder = true)
    {
      grid.AllowAddNewRow = false;
      grid.AllowColumnChooser = false;
      grid.AllowDeleteRow = allowDelete;
      grid.AllowEditRow = false;
      grid.AllowRowReorder = false;
      grid.AllowRowResize = false;
      grid.AutoGenerateColumns = true;
      grid.AutoSizeColumnsMode = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.Fill;
      grid.ReadOnly = true;
      grid.ShowColumnHeaders = false;
      grid.ShowFilteringRow = false;
      grid.ShowRowHeaderColumn = false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="grid"></param>
    public static void SetPropertiesToShowFilterRow(this RadGridView grid)
    {
      grid.EnableCustomFiltering = true;
      grid.EnableFiltering = true;
      grid.ShowFilteringRow = true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="multiselect"></param>
    /// <param name="allowDelete"></param>
    /// <param name="removeBorder"></param>
    public static void SetPropertiesToList(this RadGridView grid, bool multiselect, bool allowDelete, bool removeBorder = true)
    {
      grid.AllowAddNewRow = false;
      grid.AllowColumnChooser = false;
      grid.AllowDeleteRow = allowDelete;
      grid.AllowEditRow = false;
      grid.AllowRowReorder = false;
      grid.AllowRowResize = false;
      grid.AutoGenerateColumns = true;
      grid.AutoSizeColumnsMode = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.Fill;
      grid.AutoSizeRows = false;
      grid.EnterKeyMode = Telerik.WinControls.UI.RadGridViewEnterKeyMode.EnterMovesToNextRow;
      grid.HideSelection = false;
      grid.MultiSelect = multiselect;
      grid.ReadOnly = true;
      grid.SelectionMode = Telerik.WinControls.UI.GridViewSelectionMode.FullRowSelect;
      grid.ShowColumnHeaders = false;
      grid.ShowFilteringRow = false;
      grid.ShowGroupPanel = false;
      grid.ShowRowHeaderColumn = true;
      grid.StandardTab = true;

      // remove the border
      if (removeBorder)
      {
        grid.CellFormatting += new Telerik.WinControls.UI.CellFormattingEventHandler(grid_CellFormatting_RemoveBorder);
        grid.GridViewElement.DrawBorder = false;
        grid.GridViewElement.GroupPanelElement.DrawBorder = false;
      }
    }

    private static void grid_CellFormatting_RemoveBorder(object sender, Telerik.WinControls.UI.CellFormattingEventArgs e)
    {
      if (e.CellElement.IsCurrent)
      {
        e.CellElement.DrawFill = false;
      }

      if (e.CellElement.ColumnInfo is GridViewDataColumn)
      {
        e.CellElement.DrawBorder = false;
      }
    }

    public static void SetupToGroupByYearMonthWeek(this RadGridView grid, string dateColumnName)
    {
      if (!groupByYearMonthWeek.ContainsKey(grid))
      {
        groupByYearMonthWeek.Add(grid, dateColumnName);

        grid.EnableCustomGrouping = true;
        grid.CustomGrouping += new GridViewCustomGroupingEventHandler(grid_CustomGrouping);
        grid.GroupSummaryEvaluate += new GroupSummaryEvaluateEventHandler(grid_GroupSummaryEvaluate);

        grid.GroupDescriptors.Add(dateColumnName);
        grid.GroupDescriptors.Add(dateColumnName);
        grid.GroupDescriptors.Add(dateColumnName);
      }
    }

    private static void grid_CustomGrouping(object sender, Telerik.WinControls.UI.GridViewCustomGroupingEventArgs e)
    {
      var grid = sender as RadGridView;
      var desiredColName = groupByYearMonthWeek[grid];

      string colName = grid.GroupDescriptors[e.Level].GroupNames[0].PropertyName;
      if (colName == desiredColName)
      {
        DateTime date = Convert.ToDateTime(e.Row.Cells[colName].Value);
        if (e.Level == 0)
        {
          e.GroupKey = date.Year;
        }
        else if (e.Level == 1)
        {
          e.GroupKey = string.Format("{0:MM} - {0:MMM}", date);
        }
        else
        {
          var remain = 0;
          var noOfWeek = 0;
          noOfWeek = Math.DivRem(date.Day, 7, out remain);
          if (remain > 0)
          {
            noOfWeek += 1;
          }
          e.GroupKey = noOfWeek;
        }
      }
    }

    private static void grid_GroupSummaryEvaluate(object sender, Telerik.WinControls.UI.GroupSummaryEvaluationEventArgs e)
    {
      if (e.Group.Level == 0)
      {
        e.FormatString = string.Format("{0}", e.Group.Key);
      }
      else if (e.Group.Level == 1)
      {
        e.FormatString = string.Format("{0}", e.Group.Key);
      }
      else
      {
        e.FormatString = string.Format("Week {0}", e.Group.Key);
      }
    }

    #region Nested type: ColumnFormat

    private class ColumnFormat : IComparable<ColumnFormat>
    {
      public static List<RadListView> listViews = new List<RadListView>();

      public string column;
      public string format;
      public RadListView listView;
      static ColumnFormat() {}

      #region IComparable<ColumnFormat> Members

      public int CompareTo(ColumnFormat other)
      {
        var indexA = listViews.IndexOf(listView);
        var indexB = listViews.IndexOf(other.listView);
        if (indexA == indexB)
        {
          return string.Compare(column, other.column);
        }
        return indexA.CompareTo(indexB);
      }

      #endregion
    }

    #endregion

    #region Nested type: GridViewIncrementalSearchData

    private class GridViewIncrementalSearchData
    {
      public RadGridView Grid { get; set; }
      public StringBuilder Buffer { get; set; }
      public DateTime Start { get; set; }
      public Func<GridViewRowInfo, string> GetText { get; set; }
    }

    #endregion

    #region Nested type: ListViewDeferUpdate

    private class ListViewDeferUpdate : IDisposable
    {
      private RadListView mListView;

      public ListViewDeferUpdate(RadListView listView)
      {
        mListView = listView;
        mListView.BeginUpdate();
      }

      #region IDisposable Members

      public void Dispose()
      {
        mListView.EndUpdate();
      }

      #endregion
    }

    #endregion
  }
}