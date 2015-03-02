using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace System.Common.Extensions
{
  public static partial class WindowsXNAExtensions
  {
    #region XnaHLSColor

    private struct XnaHLSColor
    {
      private const int ShadowAdj = -333;
      private const int HilightAdj = 500;
      private const int WatermarkAdj = -50;
      private const int Range = 240;
      private const int HLSMax = 240;
      private const int RGBMax = 0xff;
      private const int Undefined = 160;
      private int hue;
      private int luminosity;
      private int saturation;

      public XnaHLSColor(Color color)
      {
        int r = color.R;
        int g = color.G;
        int b = color.B;
        var num4 = Math.Max(Math.Max(r, g), b);
        var num5 = Math.Min(Math.Min(r, g), b);
        var num6 = num4 + num5;
        this.luminosity = ((num6 * 240) + 0xff) / 510;
        var num7 = num4 - num5;
        if (num7 == 0)
        {
          this.saturation = 0;
          this.hue = 160;
        }
        else
        {
          if (this.luminosity <= 120)
          {
            this.saturation = ((num7 * 240) + (num6 / 2)) / num6;
          }
          else
          {
            this.saturation = ((num7 * 240) + ((510 - num6) / 2)) / (510 - num6);
          }
          var num8 = (((num4 - r) * 40) + (num7 / 2)) / num7;
          var num9 = (((num4 - g) * 40) + (num7 / 2)) / num7;
          var num10 = (((num4 - b) * 40) + (num7 / 2)) / num7;
          if (r == num4)
          {
            this.hue = num10 - num9;
          }
          else if (g == num4)
          {
            this.hue = (80 + num8) - num10;
          }
          else
          {
            this.hue = (160 + num9) - num8;
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
      }

      public int Luminosity
      {
        get { return this.luminosity; }
      }

      public Color Darker(float percDarker)
      {
        var num4 = 0;
        var num5 = this.NewLuma(-333, true);
        return this.ColorFromHLS(this.hue, num5 - ((int)((num5 - num4) * percDarker)), this.saturation);
      }

      public override bool Equals(object o)
      {
        if (!(o is XnaHLSColor))
        {
          return false;
        }
        var color = (XnaHLSColor)o;
        return ((((this.hue == color.hue) &&
          (this.saturation == color.saturation)) &&
          (this.luminosity == color.luminosity)));
      }

      public static bool operator ==(XnaHLSColor a, XnaHLSColor b)
      {
        return a.Equals(b);
      }

      public static bool operator !=(XnaHLSColor a, XnaHLSColor b)
      {
        return !a.Equals(b);
      }

      public override int GetHashCode()
      {
        return (((this.hue << 6) | (this.saturation << 2)) | this.luminosity);
      }

      public Color Lighter(float percLighter)
      {
        var luminosity = this.luminosity;
        var num5 = this.NewLuma(500, true);
        return this.ColorFromHLS(this.hue, luminosity + ((int)((num5 - luminosity) * percLighter)), this.saturation);
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
        if (scale)
        {
          if (n > 0)
          {
            return (int)(((luminosity * (0x3e8 - n)) + (0xf1L * n)) / 0x3e8L);
          }
          return ((luminosity * (n + 0x3e8)) / 0x3e8);
        }
        var num = luminosity;
        num += (int)((n * 240L) / 0x3e8L);
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

      private Color ColorFromHLS(int hue, int luminosity, int saturation)
      {
        byte num;
        byte num2;
        byte num3;
        if (saturation == 0)
        {
          num = num2 = num3 = (byte)((luminosity * 0xff) / 240);
          if (hue == 160) {}
        }
        else
        {
          int num5;
          if (luminosity <= 120)
          {
            num5 = ((luminosity * (240 + saturation)) + 120) / 240;
          }
          else
          {
            num5 = (luminosity + saturation) - (((luminosity * saturation) + 120) / 240);
          }
          var num4 = (2 * luminosity) - num5;
          num = (byte)(((this.HueToRGB(num4, num5, hue + 80) * 0xff) + 120) / 240);
          num2 = (byte)(((this.HueToRGB(num4, num5, hue) * 0xff) + 120) / 240);
          num3 = (byte)(((this.HueToRGB(num4, num5, hue - 80) * 0xff) + 120) / 240);
        }
        return new Color(num, num2, num3);
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
          return (n1 + ((((n2 - n1) * hue) + 20) / 40));
        }
        if (hue < 120)
        {
          return n2;
        }
        if (hue < 160)
        {
          return (n1 + ((((n2 - n1) * (160 - hue)) + 20) / 40));
        }
        return n1;
      }
    }

    #endregion

    private static Texture2D pixel;
    private static object syncRoot = new object();

    private static void Initialize(GraphicsDevice device)
    {
      if (pixel == null)
      {
        lock (syncRoot)
        {
          if (pixel == null)
          {
            pixel = new Texture2D(device, 1, 1);
            pixel.SetData(new[] {Color.White});
            device.Disposing += device_Disposing;
          }
        }
      }
    }

    private static void device_Disposing(object sender, EventArgs e)
    {
      if (pixel != null)
      {
        pixel.Dispose();
        pixel = null;
      }
    }

    private static void drawLine(SpriteBatch spriteBatch, Vector2 pt0, Vector2 pt1, Color color)
    {
      // calculate the distance between the two vectors
      var distance = Vector2.Distance(pt0, pt1);

      // calculate the angle between the two vectors
      var angle = (float)Math.Atan2(pt1.Y - pt0.Y,
        pt1.X - pt0.X);

      // stretch the pixel between the two vectors
      spriteBatch.Draw(pixel,
        pt0,
        null,
        color,
        angle,
        Vector2.Zero,
        new Vector2(distance, 1),
        SpriteEffects.None,
        0f);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="spriteBatch"></param>
    /// <param name="pt0"></param>
    /// <param name="pt1"></param>
    /// <param name="color"></param>
    public static void DrawLine(this SpriteBatch spriteBatch, Vector2 pt0, Vector2 pt1, Color color)
    {
      Initialize(spriteBatch.GraphicsDevice);
      drawLine(spriteBatch, pt0, pt1, color);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="spriteBatch"></param>
    /// <param name="pt0"></param>
    /// <param name="pt1"></param>
    /// <param name="color"></param>
    public static void DrawLine(this SpriteBatch spriteBatch, Point pt0, Point pt1, Color color)
    {
      spriteBatch.DrawLine(pt0.ToVector2(), pt1.ToVector2(), color);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="spriteBatch"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="color"></param>
    public static void DrawRectangle(this SpriteBatch spriteBatch, float x, float y, float width, float height, Color color)
    {
      // initialize
      Initialize(spriteBatch.GraphicsDevice);

      // top line
      spriteBatch.Draw(pixel, new Vector2(x, y), null, color, 0f, Vector2.Zero, new Vector2(width, 1), SpriteEffects.None, 0f);

      // bottom line
      spriteBatch.Draw(pixel, new Vector2(x, y + (height - 1)), null, color, 0f, Vector2.Zero, new Vector2(width, 1), SpriteEffects.None, 0f);

      // left line
      spriteBatch.Draw(pixel, new Vector2(x, y), null, color, 0f, Vector2.Zero, new Vector2(1, height), SpriteEffects.None, 0f);

      // right line
      spriteBatch.Draw(pixel, new Vector2(x + (width - 1), y), null, color, 0f, Vector2.Zero, new Vector2(1, height), SpriteEffects.None, 0f);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="spriteBatch"></param>
    /// <param name="bounds"></param>
    /// <param name="color"></param>
    public static void DrawRectangle(this SpriteBatch spriteBatch, Rectangle bounds, Color color)
    {
      spriteBatch.DrawRectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height, color);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="spriteBatch"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="color"></param>
    public static void DrawRectangle(this SpriteBatch spriteBatch, int x, int y, int width, int height, Color color)
    {
      // initialize
      Initialize(spriteBatch.GraphicsDevice);

      // top line
      spriteBatch.Draw(pixel, new Rectangle(x, y, width, 1), color);

      // bottom line
      spriteBatch.Draw(pixel, new Rectangle(x, y + (height - 1), width, 1), color);

      // left line
      spriteBatch.Draw(pixel, new Rectangle(x, y, 1, height), color);

      // right line
      spriteBatch.Draw(pixel, new Rectangle(x + (width - 1), y, 1, height), color);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="spriteBatch"></param>
    /// <param name="points"></param>
    /// <param name="color"></param>
    public static void DrawPolygon(this SpriteBatch spriteBatch, IEnumerable<Vector2> points, Color color)
    {
      Initialize(spriteBatch.GraphicsDevice);
      var polygon = points.ToArray();
      for (var i = 1; i < polygon.Length; ++i)
      {
        var pt0 = polygon[i - 1];
        var pt1 = polygon[i];
        drawLine(spriteBatch, pt0, pt1, color);
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="spriteBatch"></param>
    /// <param name="points"></param>
    /// <param name="color"></param>
    public static void DrawPolygon(this SpriteBatch spriteBatch, IEnumerable<Point> points, Color color)
    {
      spriteBatch.DrawPolygon(points.Select(p => p.ToVector2()), color);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="spriteBatch"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="color"></param>
    public static void FillRectangle(this SpriteBatch spriteBatch, float x, float y, float width, float height, Color color)
    {
      Initialize(spriteBatch.GraphicsDevice);
      spriteBatch.Draw(pixel, new Vector2(x, y), null, color, 0f, Vector2.Zero, new Vector2(width, height), SpriteEffects.None, 0f);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="spriteBatch"></param>
    /// <param name="bounds"></param>
    /// <param name="color"></param>
    public static void FillRectangle(this SpriteBatch spriteBatch, Rectangle bounds, Color color)
    {
      Initialize(spriteBatch.GraphicsDevice);
      spriteBatch.Draw(pixel, bounds, color);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="spriteBatch"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="color"></param>
    public static void FillRectangle(this SpriteBatch spriteBatch, int x, int y, int width, int height, Color color)
    {
      spriteBatch.FillRectangle(new Rectangle(x, y, width, height), color);
    }

    /// <summary>
    /// Alpha blends two colors together using the following formula:
    /// <para>Ra = Sa + Da * (1 - Sa).</para>
    /// <para>Rc = (Sc * Sa + Dc * Da * (1 - Sa)) / Ra.</para>
    /// <para>* Where R is the result, S is the source and D is the destination.</para>
    /// </summary>
    /// <param name="current">The current color.</param>
    /// <param name="color">The color to alpha blend the current color with.</param>
    /// <returns>The alpha blended color.</returns>
    public static Color AlphaBlend(this Color current, Color color)
    {
      /*** Ra and Rc - Result alpha and color, Sa and Sc - source alpha and color, Dc and Da - Destination color and alpha.
     * Ra = Sa + Da * (1 - Sa)
     * Rc = (Sc * Sa + Dc * Da * (1 - Sa)) / Ra
    */

      var retval = current;

      var dest = current.ToVector4();
      var src = color.ToVector4();

      var Sa = src.W;
      var Da = dest.W;

      // if the passed in color has an alpha of 0, then there is no need for alpha blending
      if (Sa > 0)
      {
        var invSa = 1 - Sa;
        var daInvSa = Da * invSa;
        var Ra = Sa + daInvSa;
        var Sc = new Vector3(src.X, src.Y, src.Z);
        var Dc = new Vector3(dest.X, dest.Y, dest.Z);

        // Vector3 Rc = (Sc * Sa + Dc * Da * invSa) / Ra;
        Vector3 left, right, result, Rc;
        Vector3.Multiply(ref Sc, Sa, out left);
        Vector3.Multiply(ref Dc, daInvSa, out right);
        Vector3.Add(ref left, ref right, out result);
        Vector3.Divide(ref result, Ra, out Rc);
        retval = new Color(new Vector4(Rc, Ra));
      }

      return retval;
    }

    /// <summary>
    /// Blends two colors together using a factor value.
    /// </summary>
    /// <param name="color1">The color to start from.</param>
    /// <param name="color">The color to blend with.</param>
    /// <param name="factor">
    /// The emphasis to place on color. This value will be forced to be between 0 and 1. If color1 is
    /// Red and color is Blue, passing in a factor of 1 will return Blue. Passing in a factor of .5 will
    /// return Red/2 + Blue/2, and so on.
    /// </param>
    /// <returns>The result of blending the two colors.</returns>
    public static Color BlendWith(this Color color1, Color color, float factor)
    {
      var c1 = color1;
      var c2 = color;

      var pc = MathHelper.Clamp(factor, 0, 1);

      int c1a = c1.A, c1r = c1.R, c1g = c1.G, c1b = c1.B;
      int c2a = c2.A, c2r = c2.R, c2g = c2.G, c2b = c2.B;

      var a = (int)Math.Abs(c1a - ((c1a - c2a) * pc));
      var r = (int)Math.Abs(c1r - ((c1r - c2r) * pc));
      var g = (int)Math.Abs(c1g - ((c1g - c2g) * pc));
      var b = (int)Math.Abs(c1b - ((c1b - c2b) * pc));

      return new Color(
        (byte)MathHelper.Clamp(r, 0, 255),
        (byte)MathHelper.Clamp(g, 0, 255),
        (byte)MathHelper.Clamp(b, 0, 255),
        (byte)MathHelper.Clamp(a, 0, 255));
    }

    /// <summary>
    /// Computes the opposite color of the color passed in. The alpha doesn't change.
    /// </summary>
    /// <param name="color">The color to get the opposite of.</param>
    /// <returns>The opposite color of the color passed in.</returns>
    public static Color Clone(this Color color)
    {
      return new Color(color.ToVector4());
    }

    /// <summary>
    /// Computes the opposite color of the color passed in. The alpha doesn't change.
    /// </summary>
    /// <param name="color">The color to get the opposite of.</param>
    /// <returns>The opposite color of the color passed in.</returns>
    public static Color Opposite(this Color color)
    {
      return new Color(255 - color.R, 255 - color.G, 255 - color.B);
    }

    /// <summary>
    /// Returns a color that is 50% lighter.
    /// </summary>
    /// <returns>A color that is 50% lighter.</returns>
    public static Color Light(this Color baseColor)
    {
      var color = new XnaHLSColor(baseColor);
      return color.Lighter(0.5f);
    }

    /// <summary>
    /// Returns a color that is 50% darker.
    /// </summary>
    /// <returns>A color that is 50% darker.</returns>
    public static Color Dark(this Color baseColor)
    {
      var color = new XnaHLSColor(baseColor);
      return color.Darker(0.5f);
    }

    /// <summary>
    /// Changes the alpha component of a color.
    /// </summary>
    /// <param name="color">The color to change the alpha value for.</param>
    /// <param name="alpha">The new alpha value. This will be clamped to be between 0 and 255</param>
    /// <returns>A color with the new alpha value.</returns>
    public static Color Alpha(this Color color, int alpha)
    {
      return new Color(color.R, color.G, color.B, (byte)MathHelper.Clamp(alpha, 0, 255));
    }

    /// <summary>
    /// Changes the alpha component of a color.
    /// </summary>
    /// <param name="color">The color to change the alpha value for.</param>
    /// <param name="alpha">The new alpha value. This will be clamped to be between 0 and 255</param>
    /// <returns>A color with the new alpha value.</returns>
    public static Color Alpha(this Color color, byte alpha)
    {
      return new Color(color.R, color.G, color.B, (byte)MathHelper.Clamp(alpha, 0, 255));
    }

    /// <summary>
    /// Changes the alpha component of a color.
    /// </summary>
    /// <param name="color">The color to change the alpha value for.</param>
    /// <param name="alpha">The new alpha value. This will be clamped to be between 0 and 1</param>
    /// <returns>A color with the new alpha value.</returns>
    public static Color Alpha(this Color color, float alpha)
    {
      return new Color(color.R, color.G, color.B, (byte)MathHelper.Clamp(alpha * 255, 0, 255));
    }

    /// <summary>
    /// Multiplies two colors together.
    /// </summary>
    /// <param name="color1">The left side of the multiplication.</param>
    /// <param name="color">The right side of the multiplication.</param>
    /// <returns>The result of the multiplication.</returns>
    public static Color Mult(this Color color1, Color color)
    {
      var a = color1.ToVector4();
      var b = color.ToVector4();
      return new Color(a * b);
    }

    /// <summary>
    /// Creates a Rectangle structure with upper-left corner and lower-right corner at the specified locations.
    /// </summary>
    /// <param name="left">The x-coordinate of the upper-left corner of the box.</param>
    /// <param name="top">The y-coordinate of the upper-left corner of the box.</param>
    /// <param name="right">The x-coordinate of the lower-right corner of the box.</param>
    /// <param name="bottom">The y-coordinate of the lower-right corner of the box.</param>
    /// <returns>A new box created from the coordinates.</returns>
    public static Rectangle FromLTRB(this Rectangle rectangle, int left, int top, int right, int bottom)
    {
      return new Rectangle(left, top, right - left, bottom - top);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="point"></param>
    /// <param name="dx"></param>
    /// <param name="dy"></param>
    public static Point Offset(this Point point, int dx, int dy)
    {
      return new Point(point.X + dx, point.Y + dy);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="vector3"></param>
    /// <returns></returns>
    public static Vector2 DropZ(this Vector3 value)
    {
      return new Vector2(value.X, value.Y);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="viewport"></param>
    /// <returns></returns>
    public static Vector2 GetSize(this Viewport viewport)
    {
      return new Vector2(viewport.Width, viewport.Height);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="viewport"></param>
    /// <returns></returns>
    public static Rectangle GetRectangle(this Viewport viewport)
    {
      return new Rectangle(viewport.X, viewport.Y, viewport.Width, viewport.Height);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rect"></param>
    /// <returns></returns>
    public static Point Center(this Rectangle rect)
    {
      return new Point
      {
        X = rect.X + (rect.Width / 2),
        Y = rect.Y + (rect.Height / 2),
      };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="spriteBatch"></param>
    /// <param name="transform"></param>
    public static void Begin(this SpriteBatch spriteBatch, Matrix transform)
    {
      spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,
        null, null, null, null, transform);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="random"></param>
    /// <param name="rect"></param>
    /// <returns></returns>
    public static Point NextPoint(this Random random, Rectangle rect)
    {
      return new Point
      {
        X = random.Next(rect.Left, rect.Right),
        Y = random.Next(rect.Top, rect.Bottom),
      };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="random"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static Point NextPoint(this Random random, Point min, Point max)
    {
      return new Point
      {
        X = random.Next(min.X, max.X),
        Y = random.Next(min.Y, max.Y),
      };
    }

    /// <summary>
    /// Rounds a single-precision floating-point number.
    /// </summary>
    /// <param name="f"></param>
    /// <returns></returns>
    public static int Round(this float f)
    {
      return (int)Math.Round(f);
    }

    /// <summary>
    /// Rounds a vector2.
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static Point Round(this Vector2 v)
    {
      return new Point(Round(v.X), Round(v.Y));
    }

    /// <summary>
    /// Rounds an enumeration of vector2.
    /// </summary>
    /// <param name="vecs"></param>
    /// <returns></returns>
    public static IEnumerable<Point> Round(this IEnumerable<Vector2> vecs)
    {
      return vecs.Select(vec => Round(vec));
    }

    /// <summary>
    /// Floors a single-precision floating-point number.
    /// </summary>
    /// <param name="f"></param>
    /// <returns></returns>
    public static int Floor(this float f)
    {
      return (int)Math.Floor(f);
    }

    /// <summary>
    /// Floors a vector2.
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static Point Floor(this Vector2 v)
    {
      return new Point(Floor(v.X), Floor(v.Y));
    }

    /// <summary>
    /// Floors an enumeration of vector2.
    /// </summary>
    /// <param name="vecs"></param>
    /// <returns></returns>
    public static IEnumerable<Point> Floor(this IEnumerable<Vector2> vecs)
    {
      return vecs.Select(vec => Floor(vec));
    }

    /// <summary>
    /// Ceils a single-precision floating-point number.
    /// </summary>
    /// <param name="f"></param>
    /// <returns></returns>
    public static int Ceiling(this float f)
    {
      return (int)Math.Ceiling(f);
    }

    /// <summary>
    /// Ceils a vector2.
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static Point Ceiling(this Vector2 v)
    {
      return new Point(Ceiling(v.X), Ceiling(v.Y));
    }

    /// <summary>
    /// Ceils an enumeration of vector2.
    /// </summary>
    /// <param name="vecs"></param>
    /// <returns></returns>
    public static IEnumerable<Point> Ceiling(this IEnumerable<Vector2> vecs)
    {
      return vecs.Select(vec => Ceiling(vec));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pt"></param>
    /// <returns></returns>
    public static Vector2 ToVector2(this Point pt)
    {
      return new Vector2(pt.X, pt.Y);
    }

    /// <summary>
    /// Creates a Vector3 struct from a Vector2. Z is initialized to 0.
    /// </summary>
    /// <param name="vec">The vector2 to use for the (x,y) values.</param>
    /// <returns>A Vector3 struct containg the x,y and z values.</returns>
    public static Vector3 ToVector3(this Vector2 vec)
    {
      return new Vector3(vec, 0);
    }

    /// <summary>
    /// Creates a Vector3 struct from a Vector2.
    /// </summary>
    /// <param name="vec">The vector2 to use for the (x,y) values.</param>
    /// <param name="z">The z value.</param>
    /// <returns>A Vector3 struct containg the x,y and z values.</returns>
    public static Vector3 ToVector3(this Vector2 vec, float z)
    {
      return new Vector3(vec, z);
    }
  }
}