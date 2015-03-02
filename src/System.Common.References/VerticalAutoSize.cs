using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace System.Common.References
{
  public interface IVerticalAutoSizeView
  {
    event EventHandler TextChanged;
    event EventHandler FontChanged;
    event EventHandler SizeChanged;
    string Text { get; }
    Font Font { get; }
    int Width { get; }
    int Height { set; }
  }

  public class VerticalAutoSizeController
  {
    private IVerticalAutoSizeView mView;
    private bool mGrowing;

    public VerticalAutoSizeController(IVerticalAutoSizeView view)
    {
      mView = view;
      mView.TextChanged += new EventHandler(mView_Changed);
      mView.FontChanged += new EventHandler(mView_Changed);
      mView.SizeChanged += new EventHandler(mView_Changed);
    }

    public void DoAutoSize()
    {
      if (mGrowing) return;
      try
      {
        mGrowing = true;
        var sz = new System.Drawing.Size(mView.Width, int.MaxValue);
        sz = TextRenderer.MeasureText(mView.Text, mView.Font, sz, 
          TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl);
        mView.Height = sz.Height;
      }
      finally
      {
        mGrowing = false;
      }
    }

    private void mView_Changed(object sender, EventArgs e)
    {
      DoAutoSize();
    }
  }
}