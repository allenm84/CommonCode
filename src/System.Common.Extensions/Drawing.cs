using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Common.Extensions
{
  public static partial class DrawingExtensions
  {
    #region WinHLSColor

    private struct WinHLSColor
    {
      private const int ShadowAdj = -333;
      private const int HilightAdj = 500;
      private const int WatermarkAdj = -50;
      private const int Range = 240;
      private const int HLSMax = 240;
      private const int RGBMax = 255;
      private const int Undefined = 160;
      private int hue;
      private bool isSystemColors_Control;
      private int luminosity;
      private int saturation;

      public WinHLSColor(Color color)
      {
        this.isSystemColors_Control = (color.ToKnownColor() == SystemColors.Control.ToKnownColor());
        int r = color.R;
        int g = color.G;
        int b = color.B;
        var num = Math.Max(Math.Max(r, g), b);
        var num2 = Math.Min(Math.Min(r, g), b);
        var num3 = num + num2;
        this.luminosity = (num3 * 240 + 255) / 510;
        var num4 = num - num2;
        if (num4 == 0)
        {
          this.saturation = 0;
          this.hue = 160;
          return;
        }
        if (this.luminosity <= 120)
        {
          this.saturation = (num4 * 240 + num3 / 2) / num3;
        }
        else
        {
          this.saturation = (num4 * 240 + (510 - num3) / 2) / (510 - num3);
        }
        var num5 = ((num - r) * 40 + num4 / 2) / num4;
        var num6 = ((num - g) * 40 + num4 / 2) / num4;
        var num7 = ((num - b) * 40 + num4 / 2) / num4;
        if (r == num)
        {
          this.hue = num7 - num6;
        }
        else
        {
          if (g == num)
          {
            this.hue = 80 + num5 - num7;
          }
          else
          {
            this.hue = 160 + num6 - num5;
          }
        }
        if (this.hue < 0)
        {
          this.hue += 240;
        }
        if (this.hue > 240)
        {
          this.hue -= 240;
        }
      }

      public int Luminosity
      {
        get { return this.luminosity; }
      }

      public Color Darker(float percDarker)
      {
        if (!this.isSystemColors_Control)
        {
          var num = 0;
          var num2 = this.NewLuma(-333, true);
          return this.ColorFromHLS(this.hue, num2 - (int)((num2 - num) * percDarker), this.saturation);
        }
        if (percDarker == 0f)
        {
          return SystemColors.ControlDark;
        }
        if (percDarker == 1f)
        {
          return SystemColors.ControlDarkDark;
        }
        var controlDark = SystemColors.ControlDark;
        var controlDarkDark = SystemColors.ControlDarkDark;
        var num3 = controlDark.R - controlDarkDark.R;
        var num4 = controlDark.G - controlDarkDark.G;
        var num5 = controlDark.B - controlDarkDark.B;
        return Color.FromArgb(controlDark.R - (byte)(num3 * percDarker), controlDark.G - (byte)(num4 * percDarker), controlDark.B - (byte)(num5 * percDarker));
      }

      public override bool Equals(object o)
      {
        if (!(o is WinHLSColor))
        {
          return false;
        }
        var hLSColor = (WinHLSColor)o;
        return this.hue == hLSColor.hue && this.saturation == hLSColor.saturation && this.luminosity == hLSColor.luminosity && this.isSystemColors_Control == hLSColor.isSystemColors_Control;
      }

      public static bool operator ==(WinHLSColor a, WinHLSColor b)
      {
        return a.Equals(b);
      }

      public static bool operator !=(WinHLSColor a, WinHLSColor b)
      {
        return !a.Equals(b);
      }

      public override int GetHashCode()
      {
        return this.hue << 6 | this.saturation << 2 | this.luminosity;
      }

      public Color Lighter(float percLighter)
      {
        if (!this.isSystemColors_Control)
        {
          var num = this.luminosity;
          var num2 = this.NewLuma(500, true);
          return this.ColorFromHLS(this.hue, num + (int)((num2 - num) * percLighter), this.saturation);
        }
        if (percLighter == 0f)
        {
          return SystemColors.ControlLight;
        }
        if (percLighter == 1f)
        {
          return SystemColors.ControlLightLight;
        }
        var controlLight = SystemColors.ControlLight;
        var controlLightLight = SystemColors.ControlLightLight;
        var num3 = controlLight.R - controlLightLight.R;
        var num4 = controlLight.G - controlLightLight.G;
        var num5 = controlLight.B - controlLightLight.B;
        return Color.FromArgb(controlLight.R - (byte)(num3 * percLighter), controlLight.G - (byte)(num4 * percLighter), controlLight.B - (byte)(num5 * percLighter));
      }

      private int NewLuma(int n, bool scale)
      {
        return this.NewLuma(this.luminosity, n, scale);
      }

      private int NewLuma(int luminosity, int n, bool scale)
      {
        if (n == 0)
        {
          return luminosity;
        }
        if (!scale)
        {
          var num = luminosity;
          num += (int)(n * 240L / 1000L);
          if (num < 0)
          {
            num = 0;
          }
          if (num > 240)
          {
            num = 240;
          }
          return num;
        }
        if (n > 0)
        {
          return (int)((luminosity * (1000 - n) + 241L * n) / 1000L);
        }
        return luminosity * (n + 1000) / 1000;
      }

      private Color ColorFromHLS(int hue, int luminosity, int saturation)
      {
        byte blue;
        byte red;
        byte green;
        if (saturation == 0)
        {
          green = (red = (blue = (byte)(luminosity * 255 / 240)));
          if (hue != 160) {}
        }
        else
        {
          int num;
          if (luminosity <= 120)
          {
            num = (luminosity * (240 + saturation) + 120) / 240;
          }
          else
          {
            num = luminosity + saturation - (luminosity * saturation + 120) / 240;
          }
          var n = 2 * luminosity - num;
          red = (byte)((this.HueToRGB(n, num, hue + 80) * 255 + 120) / 240);
          green = (byte)((this.HueToRGB(n, num, hue) * 255 + 120) / 240);
          blue = (byte)((this.HueToRGB(n, num, hue - 80) * 255 + 120) / 240);
        }
        return Color.FromArgb(red, green, blue);
      }

      private int HueToRGB(int n1, int n2, int hue)
      {
        if (hue < 0)
        {
          hue += 240;
        }
        if (hue > 240)
        {
          hue -= 240;
        }
        if (hue < 40)
        {
          return n1 + ((n2 - n1) * hue + 20) / 40;
        }
        if (hue < 120)
        {
          return n2;
        }
        if (hue < 160)
        {
          return n1 + ((n2 - n1) * (160 - hue) + 20) / 40;
        }
        return n1;
      }
    }

    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static HSV GetHSV(this Color color)
    {
      double max = Math.Max(color.R, Math.Max(color.G, color.B));
      double min = Math.Min(color.R, Math.Min(color.G, color.B));

      return new HSV
      {
        Hue = color.GetHue(),
        Saturation = (max == 0) ? 0 : 1d - (1d * min / max),
        Value = max / 255d,
      };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public static Image Strip(this Image source, int x, int y, int width, int height)
    {
      var destination = new Bitmap(width, height);
      using (var graphics = Graphics.FromImage(destination))
      {
        var dest = new Rectangle(0, 0, width, height);
        var src = new Rectangle(x, y, width, height);
        graphics.DrawImage(source, dest, src, GraphicsUnit.Pixel);
      }
      return destination;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="src"></param>
    /// <returns></returns>
    public static Image Strip(this Image source, Rectangle src)
    {
      var destination = new Bitmap(src.Width, src.Height);
      using (var graphics = Graphics.FromImage(destination))
      {
        var dest = new Rectangle(0, 0, src.Width, src.Height);
        graphics.DrawImage(source, dest, src, GraphicsUnit.Pixel);
      }
      return destination;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="point"></param>
    /// <param name="dx"></param>
    /// <param name="dy"></param>
    /// <returns></returns>
    public static Point Move(this Point point, int dx, int dy)
    {
      return new Point(point.X + dx, point.Y + dy);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="graphics"></param>
    /// <param name="color"></param>
    /// <param name="bounds"></param>
    public static void DrawGlassyRectangle(this Graphics graphics, Color color, RectangleF bounds)
    {
      // deflate
      bounds.Inflate(-1, 0);

      // fill the inside
      using (var fill = color.ToBrush())
      {
        graphics.FillRectangle(fill, bounds);
      }

      // draw the gradient
      using (var topGrad = new LinearGradientBrush(bounds,
        Color.FromArgb(150, Color.White), Color.FromArgb(50, Color.White), LinearGradientMode.Vertical))
      {
        graphics.FillRectangle(topGrad, bounds);
      }

      // draw the border
      graphics.DrawRectangle(Pens.Black, bounds);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="color"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public static Color Blend(this Color value, Color color, double amount)
    {
      var r = (byte)((value.R * amount) + color.R * (1 - amount));
      var g = (byte)((value.G * amount) + color.G * (1 - amount));
      var b = (byte)((value.B * amount) + color.B * (1 - amount));
      return Color.FromArgb(r, g, b);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static SolidBrush ToBrush(this Color color)
    {
      return new SolidBrush(color);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static Pen ToPen(this Color color)
    {
      return new Pen(color);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="graphics"></param>
    /// <param name="pen"></param>
    /// <param name="rectangle"></param>
    public static void DrawRectangle(this Graphics graphics, Pen pen, RectangleF rectangle)
    {
      graphics.DrawRectangle(pen, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rectangle"></param>
    /// <returns></returns>
    public static PointF Center(this Rectangle rectangle)
    {
      return new PointF(rectangle.X + (rectangle.Width / 2f), rectangle.Y + (rectangle.Height / 2f));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rectangle"></param>
    /// <returns></returns>
    public static PointF Center(this RectangleF rectangle)
    {
      return new PointF(rectangle.X + (rectangle.Width / 2), rectangle.Y + (rectangle.Height / 2));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static Color Opposite(this Color color)
    {
      return Color.FromArgb(255 - color.R, 255 - color.G, 255 - color.B);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static Color Complement(this Color color)
    {
      double max = Math.Max(color.R, Math.Max(color.G, color.B));
      double min = Math.Min(color.R, Math.Min(color.G, color.B));

      double hue = color.GetHue();
      var saturation = (max == 0) ? 0 : 1d - (1d * min / max);
      var value = max / 255d;

      hue += 180.0;
      while (hue > 360.0) { hue -= 360.0; }
      while (hue < 0.0) { hue += 360.0; }

      var hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
      var f = hue / 60 - Math.Floor(hue / 60);

      value = value * 255;
      var v = Convert.ToInt32(value);
      var p = Convert.ToInt32(value * (1 - saturation));
      var q = Convert.ToInt32(value * (1 - f * saturation));
      var t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

      var retval = Color.Empty;
      switch (hi)
      {
        case 0:
          retval = Color.FromArgb(255, v, t, p);
          break;
        case 1:
          retval = Color.FromArgb(255, q, v, p);
          break;
        case 2:
          retval = Color.FromArgb(255, p, v, t);
          break;
        case 3:
          retval = Color.FromArgb(255, p, q, v);
          break;
        case 4:
          retval = Color.FromArgb(255, t, p, v);
          break;
        default:
          retval = Color.FromArgb(255, v, p, q);
          break;
      }
      return retval;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static Color BrighterOrDarker(this Color color)
    {
      // counting the perceptive luminance - human eye favors green color...
      var a = 1 - (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;
      if (a < 0.5) { return DarkDark(color); }
      return LightLight(color);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static Color LightLight(this Color color)
    {
      return new WinHLSColor(color).Lighter(.75f);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static Color DarkDark(this Color color)
    {
      return new WinHLSColor(color).Darker(.75f);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static Color Light(this Color color)
    {
      return new WinHLSColor(color).Lighter(0.5f);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static Color Dark(this Color color)
    {
      return new WinHLSColor(color).Darker(0.5f);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bitmap"></param>
    /// <returns></returns>
    public static byte[] ToBytes(this Bitmap bitmap)
    {
      // Lock the bitmap's bits.  
      var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
      var bmpData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);

      // Get the address of the first line.
      var ptr = bmpData.Scan0;

      // Declare an array to hold the bytes of the bitmap.
      var bytes = Math.Abs(bmpData.Stride) * bitmap.Height;
      var rgbValues = new byte[bytes];

      // Copy the RGB values into the array.
      Marshal.Copy(ptr, rgbValues, 0, bytes);

      // Unlock the bits.
      bitmap.UnlockBits(bmpData);

      // Return the bytes
      return rgbValues;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="graphics"></param>
    /// <param name="dropShadowRect"></param>
    /// <param name="color"></param>
    public static void DrawOrb(this Graphics graphics, RectangleF dropShadowRect, Color color)
    {
      var current = graphics.SmoothingMode;
      graphics.SmoothingMode = SmoothingMode.HighQuality;
      var center = new PointF
      {
        X = dropShadowRect.X + (dropShadowRect.Width / 2f),
        Y = dropShadowRect.Y + (dropShadowRect.Height / 2f),
      };

      // determine the inflate x and y
      float ix = dropShadowRect.Width > 24 ? -2 : -1;
      float iy = dropShadowRect.Height > 24 ? -3 : -2;

      // compute the drop shadow bounds
      var fillRect = RectangleF.Inflate(dropShadowRect, ix, iy);

      // compute the top gloss bounds
      var topGlossSize = new SizeF(fillRect.Width * .8f, fillRect.Height * .6f);
      var topGlossLocation = new PointF(center.X - (topGlossSize.Width / 2),
        fillRect.Y + 2);
      var topGlossRect = new RectangleF(topGlossLocation, topGlossSize);

      // compute the bottom gloss bounds
      var bottomGlossSize = new SizeF(fillRect.Width * .4f, fillRect.Height * .2f);
      var bottomGlossLocation = new PointF(center.X - (bottomGlossSize.Width / 2),
        fillRect.Bottom - (bottomGlossSize.Height + 1f));
      var bottomGlossRect = new RectangleF(bottomGlossLocation, bottomGlossSize);

      // draw the drop shadow
      graphics.FillEllipse(Brushes.Black, dropShadowRect);

      // draw the actual ellipsde
      var fillBrush = new LinearGradientBrush(fillRect,
        color, Light(color), -90f);
      graphics.FillEllipse(fillBrush, fillRect);
      fillBrush.Dispose();

      // draw the top gloss
      var topGlossBrush = new LinearGradientBrush(topGlossRect,
        Color.FromArgb(90, Color.White), Color.Transparent, LinearGradientMode.Vertical);
      graphics.FillEllipse(topGlossBrush, topGlossRect);
      topGlossBrush.Dispose();

      // draw the bottom gloss
      var bottomGlossBrush = new LinearGradientBrush(bottomGlossRect,
        Color.Transparent, Color.FromArgb(40, Color.White), LinearGradientMode.Vertical);
      graphics.FillEllipse(bottomGlossBrush, bottomGlossRect);
      bottomGlossBrush.Dispose();

      // reset the smoothing mode
      graphics.SmoothingMode = current;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="graphics"></param>
    public static void SetToHighestQuality(this Graphics graphics)
    {
      graphics.CompositingQuality = CompositingQuality.HighQuality;
      graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
      graphics.SmoothingMode = SmoothingMode.HighQuality;
      graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
    }

    #region Nested type: HSV

    /// <summary>
    /// 
    /// </summary>
    public class HSV
    {
      public double Hue;
      public double Saturation;
      public double Value;
    }

    #endregion
  }
}