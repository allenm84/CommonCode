using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace System.Common.References
{
  public class TextProgressBar : ProgressBar
  {
    static Dictionary<ContentAlignment, StringAlignment[]> map = new Dictionary<ContentAlignment, StringAlignment[]>();
    static TextProgressBar()
    {
      map[ContentAlignment.BottomCenter] = new[] { StringAlignment.Far, StringAlignment.Center };
      map[ContentAlignment.BottomLeft] = new[] { StringAlignment.Far, StringAlignment.Near };
      map[ContentAlignment.BottomRight] = new[] { StringAlignment.Far, StringAlignment.Far };

      map[ContentAlignment.MiddleCenter] = new[] { StringAlignment.Center, StringAlignment.Center };
      map[ContentAlignment.MiddleLeft] = new[] { StringAlignment.Center, StringAlignment.Near };
      map[ContentAlignment.MiddleRight] = new[] { StringAlignment.Center, StringAlignment.Far };

      map[ContentAlignment.TopCenter] = new[] { StringAlignment.Near, StringAlignment.Center };
      map[ContentAlignment.TopLeft] = new[] { StringAlignment.Near, StringAlignment.Near };
      map[ContentAlignment.TopRight] = new[] { StringAlignment.Near, StringAlignment.Far };
    }

    private SolidBrush textBrush = new SolidBrush(SystemColors.ControlText);
    private StringFormat textFormat = new StringFormat
    {
      Alignment = StringAlignment.Center,
      LineAlignment = StringAlignment.Center,
    };
    private ContentAlignment textAlignment = ContentAlignment.MiddleCenter;

    [Browsable(true)]
    [Bindable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public override string Text
    {
      get { return base.Text; }
      set
      {
        if (!string.Equals(value, base.Text))
        {
          Refresh();
        }
        base.Text = value;
      }
    }

    [Browsable(true)]
    [Bindable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    [DefaultValue(typeof(Color), "ControlText")]
    public override Color ForeColor
    {
      get { return base.ForeColor; }
      set { base.ForeColor = value; }
    }

    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public override Font Font
    {
      get { return base.Font; }
      set { base.Font = value; }
    }

    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    [DefaultValue(typeof(ContentAlignment), "MiddleCenter")]
    public ContentAlignment TextAlignment
    {
      get { return textAlignment; }
      set
      {
        if (value != textAlignment)
        {
          textAlignment = value;

          var key = map[textAlignment];
          textFormat = new StringFormat
          {
            Alignment = key[1],
            LineAlignment = key[0],
          };

          Invalidate();
        }
      }
    }

    public TextProgressBar()
    {
      ForeColor = SystemColors.ControlText;
      textAlignment = ContentAlignment.MiddleCenter;

      SetStyle(
        ControlStyles.UserPaint |
          ControlStyles.AllPaintingInWmPaint |
          ControlStyles.OptimizedDoubleBuffer, true);
    }

    protected override void OnForeColorChanged(EventArgs e)
    {
      textBrush = new SolidBrush(ForeColor);
      Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      Graphics g = e.Graphics;

      Rectangle rect = ClientRectangle;
      ProgressBarRenderer.DrawHorizontalBar(g, rect);
      rect.Inflate(-3, -3);
      if (Value > 0)
      {
        Rectangle clip = new Rectangle(rect.X, rect.Y, (int)Math.Round(((float)Value / Maximum) * rect.Width), rect.Height);
        ProgressBarRenderer.DrawHorizontalChunks(g, clip);
      }

      g.DrawString(Text, Font, textBrush, ClientRectangle, textFormat);
    }
  }
}
