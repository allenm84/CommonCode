using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace System.Common.References
{
  /// <summary>
  /// 
  /// </summary>
  public static class Rendering
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="r"></param>
    /// <param name="r1"></param>
    /// <param name="r2"></param>
    /// <param name="r3"></param>
    /// <param name="r4"></param>
    /// <returns></returns>
    public static GraphicsPath RoundRect(RectangleF r, float r1, float r2, float r3, float r4)
    {
      float x = r.X, y = r.Y, w = r.Width, h = r.Height;
      GraphicsPath rr = new GraphicsPath();
      rr.AddBezier(x, y + r1, x, y, x + r1, y, x + r1, y);
      rr.AddLine(x + r1, y, x + w - r2, y);
      rr.AddBezier(x + w - r2, y, x + w, y, x + w, y + r2, x + w, y + r2);
      rr.AddLine(x + w, y + r2, x + w, y + h - r3);
      rr.AddBezier(x + w, y + h - r3, x + w, y + h, x + w - r3, y + h, x + w - r3, y + h);
      rr.AddLine(x + w - r3, y + h, x + r4, y + h);
      rr.AddBezier(x + r4, y + h, x, y + h, x, y + h - r4, x, y + h - r4);
      rr.AddLine(x, y + h - r4, x, y + r1);
      return rr;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public static Image Create3DBox(int width, int height)
    {
      Bitmap bitmap = new Bitmap(width, height);
      using (Graphics graphics = Graphics.FromImage(bitmap))
      {
        graphics.Clear(Color.White);

        // get the factor and the delta
        int dx = width / 4;
        int dy = height / 10;

        // get the relevatn point
        int cx = (width - 1) / 2;
        int top = dy / 2;
        int bottom = height - dy / 2;

        // draw the polygon
        using (Pen pen = new Pen(Color.Black, 3f))
        {
          List<Point> data = new List<Point>();

          data.Add(new Point(cx, top));
          data.Add(new Point(cx + dx, top + dy));
          data.Add(new Point(cx + dx, bottom - dy));
          data.Add(new Point(cx, bottom));
          data.Add(new Point(cx - dx, bottom - dy));
          data.Add(new Point(cx - dx, top + dy));
          data.Add(data[0]);
          graphics.DrawPolygon(pen, data.ToArray());
          data.Clear();

          data.Add(new Point(cx - dx, top + dy));
          data.Add(new Point(cx, top + (dy * 2)));
          graphics.DrawLine(pen, data[0], data[1]);
          data.Clear();

          data.Add(new Point(cx, top + (dy * 2)));
          data.Add(new Point(cx + dx, top + dy));
          graphics.DrawLine(pen, data[0], data[1]);
          data.Clear();

          data.Add(new Point(cx, top + (dy * 2)));
          data.Add(new Point(cx, bottom));
          graphics.DrawLine(pen, data[0], data[1]);
          data.Clear();
        }
      }
      return bitmap;
    }
  }
}
