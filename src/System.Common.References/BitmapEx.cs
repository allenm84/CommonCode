using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace System.Common.References
{
  /// <summary>
  /// The state of the MLA.Utilities.Drawing.BitmapEx.
  /// </summary>
  public enum BitmapExState
  {
    /// <summary>
    /// The BitmapEx is unlocked. No manipulation can be done.
    /// </summary>
    Unlocked,

    /// <summary>
    /// The BitmapEx is locked.
    /// </summary>
    Locked
  };

  /// <summary>
  /// A wrapper around the System.Drawing.Bitmap class to provide a faster way to manipulate
  /// the Bitmap data.
  /// </summary>
  unsafe sealed public class BitmapEx
  {
    private struct PixelData
    {
      public byte blue;
      public byte green;
      public byte red;
      public byte alpha;

      public override string ToString()
      {
        return "(" + alpha.ToString() + ", " + red.ToString() + ", " + green.ToString() + ", " + blue.ToString() + ")";
      }
    }

    private Bitmap workingBitmap = null;
    private int width = 0;
    private BitmapData bitmapData = null;
    private Byte* pBase = null;
    private PixelData* pixelData = null;
    private BitmapExState currentState = BitmapExState.Unlocked;
    private bool mAutoManageState = false;

    /// <summary>
    /// Gets the System.Drawing.Bitmap used to initialize this instance. 
    /// If the bitmap is locked, this returns null.
    /// </summary>
    public Bitmap Bitmap
    {
      get
      {
        return (currentState == BitmapExState.Locked) ? null : workingBitmap;
      }
    }

    /// <summary>
    /// Gets the current state of this instance.
    /// </summary>
    public BitmapExState State
    {
      get { return currentState; }
    }

    /// <summary>
    /// Gets the size of the bitmap.
    /// </summary>
    public System.Drawing.Size Size
    {
      get { return workingBitmap.Size; }
    }

    /// <summary>
    /// Gets the width of the bitmap.
    /// </summary>
    public int Width
    {
      get { return workingBitmap.Width; }
    }

    /// <summary>
    /// Gets the height of the bitmap.
    /// </summary>
    public int Height
    {
      get { return workingBitmap.Height; }
    }

    /// <summary>
    /// Gets or sets a value which indicates if the state is automatically
    /// managed by this instance. This isn't recommended because it can
    /// become extremely slow. Its better to lock an image, perform all
    /// manipulation and then unlock it.
    /// </summary>
    public bool AutoManageState
    {
      get { return mAutoManageState; }
      set { mAutoManageState = value; }
    }

    /// <summary>
    /// Constructs a new wrapper around an existing System.Drawing.Bitmap.
    /// </summary>
    /// <param name="inputBitmap">The System.Drawing.Bitmap to manipulate.</param>
    public BitmapEx(Bitmap inputBitmap)
    {
      if (inputBitmap == null) throw new ArgumentNullException("inputBitmap");
      workingBitmap = inputBitmap;
    }

    /// <summary>
    /// Locks the image so manipulation can be done.
    /// </summary>
    public void LockImage()
    {
      if (currentState == BitmapExState.Unlocked)
      {
        Rectangle bounds = new Rectangle(Point.Empty, workingBitmap.Size);
        width = (int)(bounds.Width * sizeof(PixelData));
        if (width % 4 != 0) width = 4 * (width / 4 + 1);

        bitmapData = workingBitmap.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
        pBase = (Byte*)bitmapData.Scan0.ToPointer();
        currentState = BitmapExState.Locked;
      }
    }

    /// <summary>
    /// Unlocks the image.
    /// </summary>
    public void UnlockImage()
    {
      if (currentState == BitmapExState.Locked)
      {
        workingBitmap.UnlockBits(bitmapData);
        bitmapData = null;
        pBase = null;
        currentState = BitmapExState.Unlocked;
      }
    }

    /// <summary>
    /// Gets or sets the current pixels of the image
    /// </summary>
    public Color[] Pixels
    {
      get
      {
        int w = Width;
        int h = Height;
        LockImage();

        Color[] pixels = new Color[w * h];
        int yWidth = 0;

        for (int y = 0; y < h; ++y)
        {
          yWidth = y * w;
          for (int x = 0; x < w; ++x)
          {
            pixels[x + yWidth] = GetPixel(x, y);
          }
        }

        UnlockImage();
        return pixels;
      }
      set
      {
        int w = Width;
        int h = Height;

        LockImage();
        int yWidth = 0;

        for (int y = 0; y < h; ++y)
        {
          yWidth = y * w;
          for (int x = 0; x < w; ++x)
          {
            SetPixel(x, y, value[x + yWidth]);
          }
        }
      }
    }

    /// <summary>
    /// Gets the specified pixel from the bitmap.
    /// </summary>
    /// <param name="x">The x-coordinate of the pixel.</param>
    /// <param name="y">The y-coordinate of the pixel.</param>
    /// <returns>A System.Drawing.Color representing the pixel.</returns>
    public Color GetPixel(int x, int y)
    {
      if (mAutoManageState && currentState == BitmapExState.Unlocked)
        LockImage();

      if (currentState == BitmapExState.Unlocked)
        throw new Exception("Must be locked before any manipulation can be done");

      pixelData = (PixelData*)(pBase + y * width + x * sizeof(PixelData));

      if (mAutoManageState)
        UnlockImage();

      return Color.FromArgb(pixelData->alpha, pixelData->red, pixelData->green, pixelData->blue);
    }

    /// <summary>
    /// Sets the specified pixel of the bitmap.
    /// </summary>
    /// <param name="x">The x-coordinate of the pixel.</param>
    /// <param name="y">The y-coordinate of the pixel.</param>
    /// <param name="color">A System.Drawing.Color representing the pixel.</param>
    public void SetPixel(int x, int y, Color color)
    {
      if (mAutoManageState && currentState == BitmapExState.Unlocked)
        LockImage();

      if (currentState == BitmapExState.Unlocked)
        throw new Exception("Must be locked before any manipulation can be done");

      PixelData* data = (PixelData*)(pBase + y * width + x * sizeof(PixelData));
      data->alpha = color.A;
      data->red = color.R;
      data->green = color.G;
      data->blue = color.B;

      if (mAutoManageState)
        UnlockImage();
    }
  }
}