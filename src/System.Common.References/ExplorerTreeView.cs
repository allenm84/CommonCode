using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace System.Common.References
{
  public class ExplorerTreeView : TreeView
  {
    private class NativeMethods
    {
      public const int TV_FIRST = 0x1100;
      public const int TVM_SETBKCOLOR = TV_FIRST + 29;
      public const int TVM_SETEXTENDEDSTYLE = TV_FIRST + 44;
      public const int TVM_GETEXTENDEDSTYLE = TV_FIRST + 45;
      public const int TVM_SETAUTOSCROLLINFO = TV_FIRST + 59;
      public const int TVS_NOHSCROLL = 0x8000;
      public const int TVS_EX_AUTOHSCROLL = 0x0020;
      public const int TVS_EX_FADEINOUTEXPANDOS = 0x0040;
      public const int GWL_STYLE = -16;
      public const int WM_PRINTCLIENT = 0x0318;
      public const int PRF_CLIENT = 0x00000004;
      public const int TVS_EX_DOUBLEBUFFER = 0x0004;

      [DllImport("user32.dll")]
      public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

      public static bool IsWinXP
      {
        get
        {
          OperatingSystem OS = Environment.OSVersion;
          return (OS.Platform == PlatformID.Win32NT) &&
            ((OS.Version.Major > 5) || ((OS.Version.Major == 5) && (OS.Version.Minor == 1)));
        }
      }

      public static bool IsWinVista
      {
        get
        {
          OperatingSystem OS = Environment.OSVersion;
          return (OS.Platform == PlatformID.Win32NT) && (OS.Version.Major >= 6);
        }
      }

      [DllImport("user32.dll", CharSet = CharSet.Unicode)]
      internal static extern int SendMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);

      [DllImport("user32.dll", CharSet = CharSet.Unicode)]
      internal static extern void SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

      [DllImport("user32.dll", CharSet = CharSet.Unicode)]
      internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);

      [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
      public extern static int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);
    }

    public ExplorerTreeView()
    {
      // Enable default double buffering processing
      SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

      // Disable default CommCtrl painting on non-Vista systems
      if (!NativeMethods.IsWinVista)
      {
        SetStyle(ControlStyles.UserPaint, true);
      }
    }

    protected override System.Windows.Forms.CreateParams CreateParams
    {
      get
      {
        System.Windows.Forms.CreateParams cp = base.CreateParams;
        cp.Style |= NativeMethods.TVS_NOHSCROLL; // lose the horizotnal scrollbar
        return cp;
      }
    }

    protected override void OnHandleCreated(System.EventArgs e)
    {
      base.OnHandleCreated(e);

      // get style
      int dw = NativeMethods.SendMessage(this.Handle, NativeMethods.TVM_GETEXTENDEDSTYLE, 0, 0);

      // Update style
      dw |= NativeMethods.TVS_EX_AUTOHSCROLL;       // autoscroll horizontaly
      dw |= NativeMethods.TVS_EX_FADEINOUTEXPANDOS; // auto hide the +/- signs

      if (DoubleBuffered)
      {
        dw |= NativeMethods.TVS_EX_DOUBLEBUFFER;
      }

      // set style
      NativeMethods.SendMessage(this.Handle, NativeMethods.TVM_SETEXTENDEDSTYLE, 0, dw);

      // little black/empty arrows and blue highlight on treenodes
      NativeMethods.SetWindowTheme(this.Handle, "explorer", null);

      if (!NativeMethods.IsWinXP)
      {
        NativeMethods.SendMessage(Handle, NativeMethods.TVM_SETBKCOLOR,
          IntPtr.Zero, (IntPtr)ColorTranslator.ToWin32(BackColor));
      }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      if (GetStyle(ControlStyles.UserPaint))
      {
        Message m = new Message();
        m.HWnd = Handle;
        m.Msg = NativeMethods.WM_PRINTCLIENT;
        m.WParam = e.Graphics.GetHdc();
        m.LParam = (IntPtr)NativeMethods.PRF_CLIENT;
        DefWndProc(ref m);
        e.Graphics.ReleaseHdc(m.WParam);
      }
      base.OnPaint(e);
    }

    public void DragScroll()
    {
      // Set a constant to define the autoscroll region
      const Single scrollRegion = 20;

      // See where the cursor is
      Point pt = this.PointToClient(Cursor.Position);

      // See if we need to scroll up or down
      if ((pt.Y + scrollRegion) > this.Height)
      {
        // Call the API to scroll down
        NativeMethods.SendMessage(this.Handle, (int)277, (int)1, 0);
      }
      else if (pt.Y < (this.Top + scrollRegion))
      {
        // Call thje API to scroll up
        NativeMethods.SendMessage(this.Handle, (int)277, (int)0, 0);
      }
    }
  }
}
