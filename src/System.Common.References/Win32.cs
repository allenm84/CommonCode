using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace System.Common.References
{
  [Flags]
  public enum ProcessAccessFlags : uint
  {
    All = 0x001F0FFF,
    Terminate = 0x00000001,
    CreateThread = 0x00000002,
    VMOperation = 0x00000008,
    VMRead = 0x00000010,
    VMWrite = 0x00000020,
    DupHandle = 0x00000040,
    SetInformation = 0x00000200,
    QueryInformation = 0x00000400,
    Synchronize = 0x00100000
  }

  public static class Win32
  {
    public const byte AC_SRC_OVER = 0x00;
    public const byte AC_SRC_ALPHA = 0x01;
    public const int ULW_ALPHA = 0x00000002;

    public const int TRUE = 1;
    public const int FALSE = 0;

    public static readonly int BFFM_SETSELECTION;
    public const int BFFM_SETSELECTIONA = 0x466;
    public const int BFFM_SETSELECTIONW = 0x467;
    public const int BFFM_ENABLEOK = 0x465;
    public const int BFFM_INITIALIZED = 1;
    public const int BFFM_SELCHANGED = 2;

    public const int WM_USER = 0x0400;
    public const int EM_FORMATRANGE = WM_USER + 57;

    public const int EM_LINEINDEX = 0x00BB;
    public const int EM_LINELENGTH = 0x00C1;
    public const int EM_POSFROMCHAR = 0x00D6;
    public const int EM_CHARFROMPOS = 0x00D7;
    public const int EM_GETFIRSTVISIBLELINE = 0xCE;

    #region user32

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr FindWindow(
      [MarshalAs(UnmanagedType.LPTStr)] string lpClassName,
      [MarshalAs(UnmanagedType.LPTStr)] string lpWindowName);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int SendMessageA(IntPtr hwnd, int wMsg, int wParam, uint lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, string lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, int lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

    [DllImport("user32.dll")]
    private static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out IntPtr ProcessId);
    public static IntPtr GetProcessID(IntPtr hwnd)
    {
      IntPtr retval;
      GetWindowThreadProcessId(hwnd, out retval);
      return retval;
    }

    [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
    public static extern IntPtr GetDC(IntPtr hWnd);

    [DllImport("user32.dll", ExactSpelling = true)]
    public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

    [DllImport("user32")]
    public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wp, ref FORMATRANGE lp);

    [DllImport("user32", CharSet = CharSet.Auto, EntryPoint = "SendMessage")]
    public extern static IntPtr SendMessageInt(IntPtr handle, uint msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32", EntryPoint = "ShowCaret")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public extern static bool ShowCaretAPI(IntPtr hwnd);

    #endregion

    #region kernel32

    [DllImport("kernel32.dll")]
    public static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, Int32 bInheritHandle, UInt32 dwProcessId);

    [DllImport("kernel32.dll")]
    public static extern Int32 ReadProcessMemory(
      IntPtr hProcess,
      IntPtr lpBaseAddress,
      [In, Out] byte[] buffer,
      UInt32 size,
      out IntPtr lpNumberOfBytesRead);

    [DllImport("kernel32.dll")]
    public static extern Int32 CloseHandle(IntPtr hObject);

    #endregion

    #region shell32

    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    public static extern bool SHGetPathFromIDList(IntPtr pidl, IntPtr pszPath);

    [DllImport("shell32.dll")]
    public static extern int SHGetMalloc([Out, MarshalAs(UnmanagedType.LPArray)] IMalloc[] ppMalloc);

    [DllImport("shell32.dll")]
    public static extern int SHGetSpecialFolderLocation(IntPtr hwnd, int csidl, ref IntPtr ppidl);

    [DllImport("shell32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SHBrowseForFolder([In] BROWSEINFO lpbi);

    #endregion

    #region gdi32

    [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
    public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

    [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
    public static extern bool DeleteDC(IntPtr hdc);

    [DllImport("gdi32.dll", ExactSpelling = true)]
    public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

    [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
    public static extern bool DeleteObject(IntPtr hObject);

    #endregion

    static Win32()
    {
      if (Marshal.SystemDefaultCharSize == 1)
      {
        BFFM_SETSELECTION = BFFM_SETSELECTIONA;
      }
      else
      {
        BFFM_SETSELECTION = BFFM_SETSELECTIONW;
      }
    }
  }

  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct BLENDFUNCTION
  {
    public byte BlendOp;
    public byte BlendFlags;
    public byte SourceConstantAlpha;
    public byte AlphaFormat;
  }

  [ComImport, Guid("00000002-0000-0000-c000-000000000046"), SuppressUnmanagedCodeSecurity, InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  public interface IMalloc
  {
    [PreserveSig]
    IntPtr Alloc(int cb);
    [PreserveSig]
    IntPtr Realloc(IntPtr pv, int cb);
    [PreserveSig]
    void Free(IntPtr pv);
    [PreserveSig]
    int GetSize(IntPtr pv);
    [PreserveSig]
    int DidAlloc(IntPtr pv);
    [PreserveSig]
    void HeapMinimize();
  }

  public delegate int BrowseCallbackProc(IntPtr hwnd, int msg, IntPtr lParam, IntPtr lpData);

  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
  public class BROWSEINFO
  {
    public IntPtr hwndOwner;
    public IntPtr pidlRoot;
    public IntPtr pszDisplayName;
    public string lpszTitle;
    public int ulFlags;
    public BrowseCallbackProc lpfn;
    public IntPtr lParam;
    public int iImage;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct RECT
  {
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct CHARRANGE
  {
    public int cpMin;         //First character of range (0 for start of doc)
    public int cpMax;           //Last character of range (-1 for end of doc)
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct FORMATRANGE
  {
    public IntPtr hdc;             //Actual DC to draw on
    public IntPtr hdcTarget;       //Target DC for determining text formatting
    public RECT rc;                //Region of the DC to draw to (in twips)
    public RECT rcPage;            //Region of the whole DC (page size) (in twips)
    public CHARRANGE chrg;         //Range of text to draw (see earlier declaration)
  }
}