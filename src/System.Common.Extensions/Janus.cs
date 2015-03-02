using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if JanusGridEx
using Janus.Windows.GridEX;
#endif

#if JanusEditControls
using Janus.Windows.EditControls;
using Janus.Windows.UI.Tab;
#endif

namespace System.Common.Extensions
{
  public static partial class JanusExtensions
  {
#if JanusEditControls
  /// <summary>
  /// 
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="combBox"></param>
  /// <param name="values"></param>
  public static void Populate<T>(this UIComboBox combBox, IEnumerable<T> values)
  {
    Populate(combBox, values, x => x.ToString());
  }

  /// <summary>
  /// 
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="combBox"></param>
  /// <param name="values"></param>
  /// <param name="getText"></param>
  public static void Populate<T>(this UIComboBox combBox, IEnumerable<T> values, Func<T, string> getText)
  {
    var dataSource = values
      .Select(value =>
        new
        {
          Text = getText(value),
          Value = value,
        })
      .ToList();

    combBox.Items.Clear();
    combBox.DataSource = dataSource;
    combBox.DisplayMember = "Text";
    combBox.ValueMember = "Value";
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="comboBox"></param>
  /// <param name="value"></param>
  public static void SetSelectedIndex(this UIComboBox comboBox, object value)
  {
    comboBox.SelectedIndex = comboBox.Items
      .Cast<UIComboBoxItem>()
      .ToList()
      .FindIndex(i => object.Equals(i.Value, value));
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="tab"></param>
  public static void AutoExtendTabPages(this UITab tab)
  {
    tab.Margin = new Padding(0);
    tab.SizeChanged += new EventHandler(tab_SizeChanged);
    _autoExtendTabPages(tab);
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  private static void tab_SizeChanged(object sender, EventArgs e)
  {
    _autoExtendTabPages(sender as UITab);
  }

  private static void _autoExtendTabPages(UITab tab)
  {
    if (tab == null) return;
    if (tab.TabPages == null) return;
    if (tab.TabPages.Count == 0) return;

    for (int i = 0; i < tab.TabPages.Count; ++i)
    {
      var page = tab.TabPages[i];
      page.Size = new Size(tab.Width, page.Height);
      page.Location = new Point(0, page.Location.Y);
    }
  }
#endif

#if JanusGridEx
  /// <summary>
  /// 
  /// </summary>
  /// <param name="grid"></param>
  public static void Office2007Silver(this GridEX grid)
  {
    grid.VisualStyle = Janus.Windows.GridEX.VisualStyle.Office2007;
    grid.Office2007ColorScheme = Janus.Windows.GridEX.Office2007ColorScheme.Silver;
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="grid"></param>
  /// <param name="x"></param>
  /// <param name="y"></param>
  /// <returns></returns>
  public static GridEXRow GetRow(this GridEX grid, int x, int y)
  {
    GridArea area = grid.HitTest(x, y);
    if (area != GridArea.Cell) return null;
    int position = grid.RowPositionFromPoint(x, y);
    return grid.GetRow(position);
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="grid"></param>
  /// <returns></returns>
  public static bool ConfirmDelete(this GridEX grid)
  {
    string singular = "Are you sure you want to delete this item?";
    string plural = "Are you sure you want to delete these items?";

    var result = MessageBox.Show(grid.ParentForm, grid.SelectedItems.Count == 1 ?
    singular : plural, "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

    return (result == System.Windows.Forms.DialogResult.Yes);
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="grid"></param>
  /// <param name="row"></param>
  /// <returns></returns>
  public static bool SetRow(this GridEX grid, int row)
  {
    bool success = true;
    try { grid.Row = row; }
    catch { success = false; }
    return success;
  }

  /// <summary>
  /// 
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="grid"></param>
  /// <returns></returns>
  public static IEnumerable<T> GetSelectedItems<T>(this GridEX grid) where T : class
  {
    return grid.SelectedItems
      .Cast<GridEXSelectedItem>()
      .Select(i => i.GetRow())
      .Where(r => r.RowType == RowType.Record)
      .Select(r => r.DataRow as T);
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="grid"></param>
  /// <param name="columnName"></param>
  /// <param name="value"></param>
  public static void ApplyFilter(this GridEX grid, string columnName, object value)
  {
    grid.ApplyFilter(columnName, ConditionOperator.Equal, value);
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="grid"></param>
  /// <param name="columnName"></param>
  /// <param name="conditionOperator"></param>
  /// <param name="value"></param>
  public static void ApplyFilter(this GridEX grid, string columnName, ConditionOperator conditionOperator, object value)
  {
    var column = grid.RootTable.Columns[columnName];
    grid.RootTable.ApplyFilter(new GridEXFilterCondition(column, conditionOperator, value));
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="column"></param>
  /// <param name="text"></param>
  public static void SetToButtonColumn(this GridEXColumn column, string text)
  {
    SetToButtonColumn(column, text, 60);
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="column"></param>
  /// <param name="text"></param>
  /// <param name="width"></param>
  public static void SetToButtonColumn(this GridEXColumn column, string text, int width)
  {
    column.Width = width;
    column.AllowSize = false;

    column.ButtonDisplayMode = Janus.Windows.GridEX.CellButtonDisplayMode.Always;
    column.ButtonStyle = Janus.Windows.GridEX.ButtonStyle.TextButton;
    column.ButtonText = text;
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="table"></param>
  /// <returns></returns>
  public static string GetFilterText(this GridEXTable table)
  {
    object emptyString = "";
    if (table.FilterApplied != null && table.FilterApplied.Conditions.Count > 0)
    {
      return Convert.ToString(table.FilterApplied.Conditions[0].Value1 ?? emptyString);
    }
    return string.Empty;
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="grid"></param>
  public static void SetPropertiesToHideSelection(this GridEX grid)
  {
    grid.SelectedFormatStyle.BackColor = Color.Empty;
    grid.SelectedFormatStyle.ForeColor = Color.Empty;
    grid.SelectedInactiveFormatStyle.BackColor = Color.Empty;
    grid.SelectedInactiveFormatStyle.ForeColor = Color.Empty;
    grid.GridLines = Janus.Windows.GridEX.GridLines.None;
    grid.FocusStyle = Janus.Windows.GridEX.FocusStyle.None;
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="grid"></param>
  public static void SetPropertiesToShowFilterRow(this GridEX grid)
  {
    grid.DefaultFilterRowComparison = Janus.Windows.GridEX.FilterConditionOperator.Contains;
    grid.DynamicFiltering = true;
    grid.FilterMode = Janus.Windows.GridEX.FilterMode.Automatic;
    grid.FilterRowButtonStyle = Janus.Windows.GridEX.FilterRowButtonStyle.None;
    grid.FilterRowUpdateMode = Janus.Windows.GridEX.FilterRowUpdateMode.WhenValueChanges;
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="grid"></param>
  /// <param name="centerCard"></param>
  public static void SetPropertiesToSingleCardView(this GridEX grid, bool centerCard)
  {
    grid.AllowAddNew = Janus.Windows.GridEX.InheritableBoolean.False;
    grid.AllowColumnDrag = false;
    grid.AllowDelete = Janus.Windows.GridEX.InheritableBoolean.False;
    grid.AllowEdit = Janus.Windows.GridEX.InheritableBoolean.False;
    grid.AllowRemoveColumns = Janus.Windows.GridEX.InheritableBoolean.False;
    grid.AutoEdit = false;
    grid.AutomaticSort = true;
    grid.CardHeaders = false;
    grid.CellSelectionMode = Janus.Windows.GridEX.CellSelectionMode.EntireRow;
    grid.CellToolTip = Janus.Windows.GridEX.CellToolTip.TruncatedText;
    grid.CenterSingleCard = centerCard;
    grid.ColumnAutoResize = true;
    grid.ColumnAutoSizeMode = Janus.Windows.GridEX.ColumnAutoSizeMode.AllCellsAndHeader;
    grid.ColumnHeaders = Janus.Windows.GridEX.InheritableBoolean.False;
    grid.ExpandableCards = false;
    grid.FilterMode = Janus.Windows.GridEX.FilterMode.None;
    grid.FocusCellDisplayMode = Janus.Windows.GridEX.FocusCellDisplayMode.UseSelectedFormatStyle;
    grid.GridLines = Janus.Windows.GridEX.GridLines.RowOutline;
    grid.GroupByBoxVisible = false;
    grid.HideSelection = Janus.Windows.GridEX.HideSelection.HighlightInactive;
    grid.SelectionMode = Janus.Windows.GridEX.SelectionMode.SingleSelection;
    grid.TotalRow = Janus.Windows.GridEX.InheritableBoolean.False;
    grid.View = Janus.Windows.GridEX.View.SingleCard;
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="grid"></param>
  /// <param name="multiselect"></param>
  /// <param name="allowDelete"></param>
  public static void SetPropertiesToList(this GridEX grid, bool multiselect, bool allowDelete)
  {
    grid.AllowAddNew = Janus.Windows.GridEX.InheritableBoolean.False;
    grid.AllowColumnDrag = false;
    grid.AllowDelete = allowDelete ?
      Janus.Windows.GridEX.InheritableBoolean.True :
      Janus.Windows.GridEX.InheritableBoolean.False;
    grid.AllowEdit = Janus.Windows.GridEX.InheritableBoolean.False;
    grid.AllowRemoveColumns = Janus.Windows.GridEX.InheritableBoolean.False;
    grid.AutoEdit = false;
    grid.AutomaticSort = true;
    grid.CellSelectionMode = Janus.Windows.GridEX.CellSelectionMode.EntireRow;
    grid.CellToolTip = Janus.Windows.GridEX.CellToolTip.TruncatedText;
    grid.ColumnAutoResize = true;
    grid.ColumnAutoSizeMode = Janus.Windows.GridEX.ColumnAutoSizeMode.AllCellsAndHeader;
    grid.ColumnHeaders = Janus.Windows.GridEX.InheritableBoolean.False;
    grid.FilterMode = Janus.Windows.GridEX.FilterMode.None;
    grid.FocusCellDisplayMode = Janus.Windows.GridEX.FocusCellDisplayMode.UseSelectedFormatStyle;
    grid.FocusStyle = FocusStyle.None;
    grid.GridLines = Janus.Windows.GridEX.GridLines.RowOutline;
    grid.GroupByBoxVisible = false;
    grid.HideSelection = Janus.Windows.GridEX.HideSelection.HighlightInactive;
    grid.SelectionMode = multiselect ?
      Janus.Windows.GridEX.SelectionMode.MultipleSelection :
      Janus.Windows.GridEX.SelectionMode.SingleSelection;
    grid.TotalRow = Janus.Windows.GridEX.InheritableBoolean.False;

    if (allowDelete)
    {
      grid.DeletingRecords += new CancelEventHandler(grid_DeletingRecords);
    }
    else
    {
      grid.DeletingRecords -= new CancelEventHandler(grid_DeletingRecords);
    }
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="grid"></param>
  public static void SetToAllowDelete(this GridEX grid)
  {
    grid.AllowDelete = InheritableBoolean.True;
    grid.DeletingRecords += new CancelEventHandler(grid_DeletingRecords);
  }

  private static void grid_DeletingRecords(object sender, CancelEventArgs e)
  {
    var grid = sender as GridEX;
    if (grid == null) return;
    e.Cancel = !grid.ConfirmDelete();
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="columns"></param>
  /// <param name="names"></param>
  public static void Hide(this GridEXColumnCollection columns, params string[] names)
  {
    foreach (Janus.Windows.GridEX.GridEXColumn column in columns)
    {
      column.Visible = !(names.Contains(column.Key));
    }
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="columns"></param>
  /// <param name="names"></param>
  public static void HideAllExcept(this GridEXColumnCollection columns, params string[] names)
  {
    foreach (Janus.Windows.GridEX.GridEXColumn column in columns)
    {
      column.Visible = (names.Contains(column.Key));
    }
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="columns"></param>
  /// <param name="name"></param>
  /// <param name="width"></param>
  public static void LockColumnWidth(this GridEXColumnCollection columns, string name, int width)
  {
    columns[name].Width = width;
    columns[name].AllowSize = false;
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="columns"></param>
  /// <param name="text"></param>
  /// <param name="key"></param>
  /// <param name="width"></param>
  /// <param name="displayMode"></param>
  /// <param name="style"></param>
  public static GridEXColumn AddButtonColumn(this GridEXColumnCollection columns, string text, string key,
    int width = 60, CellButtonDisplayMode displayMode = CellButtonDisplayMode.Always,
    Janus.Windows.GridEX.ButtonStyle style = Janus.Windows.GridEX.ButtonStyle.ButtonCell)
  {
    GridEXColumn column = new GridEXColumn();
    column.AllowSize = false;
    column.ButtonDisplayMode = displayMode;
    column.ButtonStyle = style;
    column.ButtonText = text;
    column.Caption = text;
    column.EditType = EditType.NoEdit;
    column.Key = key;
    column.Width = width;
    column.TextAlignment = Janus.Windows.GridEX.TextAlignment.Center;
    columns.Add(column);

    return column;
  }
#endif
  }
}