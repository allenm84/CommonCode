using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

// Disable warning CS0108: 'x' hides inherited member 'y'. Use the new keyword if hiding was intended.
#pragma warning disable 0108

namespace System.Common.References
{
  /// <summary>
  /// Prompts the user to select a folder.
  /// </summary>
  /// <remarks>
  /// This class will use the Vista style Select Folder dialog if possible, or the regular FolderBrowserDialog
  /// if it is not. Note that the Vista style dialog is very different, so using this class without testing
  /// in both Vista and older Windows versions is not recommended.
  /// </remarks>
  /// <threadsafety instance="false" static="true" />
  [DefaultEvent("HelpRequest"), Designer("System.Windows.Forms.Design.FolderBrowserDialogDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), DefaultProperty("SelectedPath"), Description("Prompts the user to select a folder.")]
  public sealed class VistaFolderBrowserDialog : CommonDialog
  {
    #region IIDGuid
    private static class IIDGuid
    {
      internal const string IModalWindow = "b4db1657-70d7-485e-8e3e-6fcb5a5c1802";
      internal const string IFileDialog = "42f85136-db7e-439c-85f1-e4075d135fc8";
      internal const string IFileOpenDialog = "d57c7288-d4ad-4768-be02-9d969532d960";
      internal const string IFileSaveDialog = "84bccd23-5fde-4cdb-aea4-af64b83d78ab";
      internal const string IFileDialogEvents = "973510DB-7D7F-452B-8975-74A85828D354";
      internal const string IFileDialogControlEvents = "36116642-D713-4b97-9B83-7484A9D00433";
      internal const string IFileDialogCustomize = "e6fdd21a-163f-4975-9c8c-a69f1ba37034";
      internal const string IShellItem = "43826D1E-E718-42EE-BC55-A1E261C37BFE";
      internal const string IShellItemArray = "B63EA76D-1F85-456F-A19C-48159EFA858B";
      internal const string IKnownFolder = "38521333-6A87-46A7-AE10-0F16706816C3";
      internal const string IKnownFolderManager = "44BEAAEC-24F4-4E90-B3F0-23D258FBB146";
      internal const string IPropertyStore = "886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99";
      internal const string IProgressDialog = "EBBC7C04-315E-11d2-B62F-006097DF5BD4";
    }
    #endregion

    #region CLSIDGuid
    private static class CLSIDGuid
    {
      internal const string FileOpenDialog = "DC1C5A9C-E88A-4dde-A5A1-60F82A20AEF7";
      internal const string FileSaveDialog = "C0B4E2F3-BA21-4773-8DBA-335EC946EB8B";
      internal const string KnownFolderManager = "4df0c730-df9d-4ae3-9153-aa6b82e9795a";
      internal const string ProgressDialog = "F8383852-FCD3-11d1-A6B9-006097DF5BD4";
    }
    #endregion

    #region KFIDGuid
    private static class KFIDGuid
    {
      internal const string ComputerFolder = "0AC0837C-BBF8-452A-850D-79D08E667CA7";
      internal const string Favorites = "1777F761-68AD-4D8A-87BD-30B759FA33DD";
      internal const string Documents = "FDD39AD0-238F-46AF-ADB4-6C85480369C7";
      internal const string Profile = "5E6C858F-0E22-4760-9AFE-EA3317B67173";
    }
    #endregion

    #region HRESULT
    private enum HRESULT : long
    {
      S_FALSE = 0x0001,
      S_OK = 0x0000,
      E_INVALIDARG = 0x80070057,
      E_OUTOFMEMORY = 0x8007000E,
      ERROR_CANCELLED = 0x800704C7
    }
    #endregion

    #region Native Methods

    private static class NativeMethods
    {
      public static bool IsWindowsVistaOrLater
      {
        get
        {
          return Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version >= new Version(6, 0, 6000);
        }
      }

      public static bool IsWindowsXPOrLater
      {
        get
        {
          return Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version >= new Version(5, 1, 2600);
        }
      }

      #region LoadLibrary

      public const int ErrorFileNotFound = 2;

      [DllImport("kernel32", SetLastError = true),
       ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool FreeLibrary(IntPtr hModule);

      [Flags]
      public enum LoadLibraryExFlags : uint
      {
        DontResolveDllReferences = 0x00000001,
        LoadLibraryAsDatafile = 0x00000002,
        LoadWithAlteredSearchPath = 0x00000008,
        LoadIgnoreCodeAuthzLevel = 0x00000010
      }

      #endregion

      #region Task Dialogs

      [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
      public static extern IntPtr GetActiveWindow();

      [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
      public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

      [DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
      public static extern int GetCurrentThreadId();

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1400:PInvokeEntryPointsShouldExist"), DllImport("comctl32.dll", PreserveSig = false)]
      public static extern void TaskDialogIndirect([In] ref TASKDIALOGCONFIG pTaskConfig, out int pnButton, out int pnRadioButton, [MarshalAs(UnmanagedType.Bool)] out bool pfVerificationFlagChecked);


      public delegate uint TaskDialogCallback(IntPtr hwnd, uint uNotification, IntPtr wParam, IntPtr lParam, IntPtr dwRefData);


      public const int WM_USER = 0x400;
      public const int WM_GETICON = 0x007F;
      public const int WM_SETICON = 0x0080;
      public const int ICON_SMALL = 0;

      public enum TaskDialogNotifications
      {
        Created = 0,
        Navigated = 1,
        ButtonClicked = 2,            // wParam = Button ID
        HyperlinkClicked = 3,            // lParam = (LPCWSTR)pszHREF
        Timer = 4,            // wParam = Milliseconds since dialog created or timer reset
        Destroyed = 5,
        RadioButtonClicked = 6,            // wParam = Radio Button ID
        DialogConstructed = 7,
        VerificationClicked = 8,             // wParam = 1 if checkbox checked, 0 if not, lParam is unused and always 0
        Help = 9,
        ExpandoButtonClicked = 10            // wParam = 0 (dialog is now collapsed), wParam != 0 (dialog is now expanded)
      }

      [Flags]
      public enum TaskDialogCommonButtonFlags
      {
        OkButton = 0x0001, // selected control return value IDOK
        YesButton = 0x0002, // selected control return value IDYES
        NoButton = 0x0004, // selected control return value IDNO
        CancelButton = 0x0008, // selected control return value IDCANCEL
        RetryButton = 0x0010, // selected control return value IDRETRY
        CloseButton = 0x0020  // selected control return value IDCLOSE
      };

      [Flags]
      public enum TaskDialogFlags
      {
        EnableHyperLinks = 0x0001,
        UseHIconMain = 0x0002,
        UseHIconFooter = 0x0004,
        AllowDialogCancellation = 0x0008,
        UseCommandLinks = 0x0010,
        UseCommandLinksNoIcon = 0x0020,
        ExpandFooterArea = 0x0040,
        ExpandedByDefault = 0x0080,
        VerificationFlagChecked = 0x0100,
        ShowProgressBar = 0x0200,
        ShowMarqueeProgressBar = 0x0400,
        CallbackTimer = 0x0800,
        PositionRelativeToWindow = 0x1000,
        RtlLayout = 0x2000,
        NoDefaultRadioButton = 0x4000,
        CanBeMinimized = 0x8000
      };

      public enum TaskDialogMessages
      {
        NavigatePage = WM_USER + 101,
        ClickButton = WM_USER + 102, // wParam = Button ID
        SetMarqueeProgressBar = WM_USER + 103, // wParam = 0 (nonMarque) wParam != 0 (Marquee)
        SetProgressBarState = WM_USER + 104, // wParam = new progress state
        SetProgressBarRange = WM_USER + 105, // lParam = MAKELPARAM(nMinRange, nMaxRange)
        SetProgressBarPos = WM_USER + 106, // wParam = new position
        SetProgressBarMarquee = WM_USER + 107, // wParam = 0 (stop marquee), wParam != 0 (start marquee), lparam = speed (milliseconds between repaints)
        SetElementText = WM_USER + 108, // wParam = element (TASKDIALOG_ELEMENTS), lParam = new element text (LPCWSTR)
        ClickRadioButton = WM_USER + 110, // wParam = Radio Button ID
        EnableButton = WM_USER + 111, // lParam = 0 (disable), lParam != 0 (enable), wParam = Button ID
        EnableRadioButton = WM_USER + 112, // lParam = 0 (disable), lParam != 0 (enable), wParam = Radio Button ID
        ClickVerification = WM_USER + 113, // wParam = 0 (unchecked), 1 (checked), lParam = 1 (set key focus)
        UpdateElementText = WM_USER + 114, // wParam = element (TASKDIALOG_ELEMENTS), lParam = new element text (LPCWSTR)
        SetButtonElevationRequiredState = WM_USER + 115, // wParam = Button ID, lParam = 0 (elevation not required), lParam != 0 (elevation required)
        UpdateIcon = WM_USER + 116  // wParam = icon element (TASKDIALOG_ICON_ELEMENTS), lParam = new icon (hIcon if TDF_USE_HICON_* was set, PCWSTR otherwise)
      }

      public enum TaskDialogElements
      {
        Content,
        ExpandedInformation,
        Footer,
        MainInstruction
      }

      [StructLayout(LayoutKind.Sequential, Pack = 4)]
      public struct TASKDIALOG_BUTTON
      {
        public int nButtonID;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszButtonText;
      }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1049:TypesThatOwnNativeResourcesShouldBeDisposable"), StructLayout(LayoutKind.Sequential, Pack = 4)]
      public struct TASKDIALOGCONFIG
      {
        public uint cbSize;
        public IntPtr hwndParent;
        public IntPtr hInstance;
        public TaskDialogFlags dwFlags;
        public TaskDialogCommonButtonFlags dwCommonButtons;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszWindowTitle;
        public IntPtr hMainIcon;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszMainInstruction;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszContent;
        public uint cButtons;
        //[MarshalAs(UnmanagedType.LPArray)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
        public IntPtr pButtons;
        public int nDefaultButton;
        public uint cRadioButtons;
        //[MarshalAs(UnmanagedType.LPArray)]
        public IntPtr pRadioButtons;
        public int nDefaultRadioButton;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszVerificationText;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszExpandedInformation;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszExpandedControlText;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszCollapsedControlText;
        public IntPtr hFooterIcon;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pszFooterText;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public TaskDialogCallback pfCallback;
        public IntPtr lpCallbackData;
        public uint cxWidth;
      }

      [DllImport("user32.dll", CharSet = CharSet.Auto)]
      public static extern IntPtr SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

      #endregion

      #region File Operations Definitions

      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
      internal struct COMDLG_FILTERSPEC
      {
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string pszName;
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string pszSpec;
      }

      internal enum FDAP
      {
        FDAP_BOTTOM = 0x00000000,
        FDAP_TOP = 0x00000001,
      }

      internal enum FDE_SHAREVIOLATION_RESPONSE
      {
        FDESVR_DEFAULT = 0x00000000,
        FDESVR_ACCEPT = 0x00000001,
        FDESVR_REFUSE = 0x00000002
      }

      internal enum FDE_OVERWRITE_RESPONSE
      {
        FDEOR_DEFAULT = 0x00000000,
        FDEOR_ACCEPT = 0x00000001,
        FDEOR_REFUSE = 0x00000002
      }

      internal enum SIATTRIBFLAGS
      {
        SIATTRIBFLAGS_AND = 0x00000001, // if multiple items and the attirbutes together.
        SIATTRIBFLAGS_OR = 0x00000002, // if multiple items or the attributes together.
        SIATTRIBFLAGS_APPCOMPAT = 0x00000003, // Call GetAttributes directly on the ShellFolder for multiple attributes
      }

      internal enum SIGDN : uint
      {
        SIGDN_NORMALDISPLAY = 0x00000000,           // SHGDN_NORMAL
        SIGDN_PARENTRELATIVEPARSING = 0x80018001,   // SHGDN_INFOLDER | SHGDN_FORPARSING
        SIGDN_DESKTOPABSOLUTEPARSING = 0x80028000,  // SHGDN_FORPARSING
        SIGDN_PARENTRELATIVEEDITING = 0x80031001,   // SHGDN_INFOLDER | SHGDN_FOREDITING
        SIGDN_DESKTOPABSOLUTEEDITING = 0x8004c000,  // SHGDN_FORPARSING | SHGDN_FORADDRESSBAR
        SIGDN_FILESYSPATH = 0x80058000,             // SHGDN_FORPARSING
        SIGDN_URL = 0x80068000,                     // SHGDN_FORPARSING
        SIGDN_PARENTRELATIVEFORADDRESSBAR = 0x8007c001,     // SHGDN_INFOLDER | SHGDN_FORPARSING | SHGDN_FORADDRESSBAR
        SIGDN_PARENTRELATIVE = 0x80080001           // SHGDN_INFOLDER
      }

      [Flags]
      internal enum FOS : uint
      {
        FOS_OVERWRITEPROMPT = 0x00000002,
        FOS_STRICTFILETYPES = 0x00000004,
        FOS_NOCHANGEDIR = 0x00000008,
        FOS_PICKFOLDERS = 0x00000020,
        FOS_FORCEFILESYSTEM = 0x00000040, // Ensure that items returned are filesystem items.
        FOS_ALLNONSTORAGEITEMS = 0x00000080, // Allow choosing items that have no storage.
        FOS_NOVALIDATE = 0x00000100,
        FOS_ALLOWMULTISELECT = 0x00000200,
        FOS_PATHMUSTEXIST = 0x00000800,
        FOS_FILEMUSTEXIST = 0x00001000,
        FOS_CREATEPROMPT = 0x00002000,
        FOS_SHAREAWARE = 0x00004000,
        FOS_NOREADONLYRETURN = 0x00008000,
        FOS_NOTESTFILECREATE = 0x00010000,
        FOS_HIDEMRUPLACES = 0x00020000,
        FOS_HIDEPINNEDPLACES = 0x00040000,
        FOS_NODEREFERENCELINKS = 0x00100000,
        FOS_DONTADDTORECENT = 0x02000000,
        FOS_FORCESHOWHIDDEN = 0x10000000,
        FOS_DEFAULTNOMINIMODE = 0x20000000
      }

      internal enum CDCONTROLSTATE
      {
        CDCS_INACTIVE = 0x00000000,
        CDCS_ENABLED = 0x00000001,
        CDCS_VISIBLE = 0x00000002
      }

      #endregion

      #region KnownFolder Definitions

      internal enum FFFP_MODE
      {
        FFFP_EXACTMATCH,
        FFFP_NEARESTPARENTMATCH
      }

      [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
      internal struct KNOWNFOLDER_DEFINITION
      {
        internal NativeMethods.KF_CATEGORY category;
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string pszName;
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string pszCreator;
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string pszDescription;
        internal Guid fidParent;
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string pszRelativePath;
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string pszParsingName;
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string pszToolTip;
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string pszLocalizedName;
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string pszIcon;
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string pszSecurity;
        internal uint dwAttributes;
        internal NativeMethods.KF_DEFINITION_FLAGS kfdFlags;
        internal Guid ftidType;
      }

      internal enum KF_CATEGORY
      {
        KF_CATEGORY_VIRTUAL = 0x00000001,
        KF_CATEGORY_FIXED = 0x00000002,
        KF_CATEGORY_COMMON = 0x00000003,
        KF_CATEGORY_PERUSER = 0x00000004
      }

      [Flags]
      internal enum KF_DEFINITION_FLAGS
      {
        KFDF_PERSONALIZE = 0x00000001,
        KFDF_LOCAL_REDIRECT_ONLY = 0x00000002,
        KFDF_ROAMABLE = 0x00000004,
      }


      // Property System structs and consts
      [StructLayout(LayoutKind.Sequential, Pack = 4)]
      internal struct PROPERTYKEY
      {
        internal Guid fmtid;
        internal uint pid;
      }

      #endregion

      #region Shell Parsing Names

      [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
      public static extern int SHCreateItemFromParsingName([MarshalAs(UnmanagedType.LPWStr)] string pszPath, IntPtr pbc, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppv);

      internal static IShellItem CreateItemFromParsingName(string path)
      {
        object item;
        Guid guid = new Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe"); // IID_IShellItem
        int hr = NativeMethods.SHCreateItemFromParsingName(path, IntPtr.Zero, ref guid, out item);
        if (hr != 0)
          throw new System.ComponentModel.Win32Exception(hr);
        return (IShellItem)item;
      }

      #endregion

      #region String resources

      [Flags()]
      public enum FormatMessageFlags
      {
        FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100,
        FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200,
        FORMAT_MESSAGE_FROM_STRING = 0x00000400,
        FORMAT_MESSAGE_FROM_HMODULE = 0x00000800,
        FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000,
        FORMAT_MESSAGE_ARGUMENT_ARRAY = 0x00002000
      }

      [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
      public static extern uint FormatMessage([MarshalAs(UnmanagedType.U4)] FormatMessageFlags dwFlags, IntPtr lpSource,
        uint dwMessageId, uint dwLanguageId, ref IntPtr lpBuffer,
        uint nSize, string[] Arguments);

      #endregion

      #region Desktop Window Manager

      public const int WM_NCHITTEST = 0x0084;
      public const int WM_DWMCOMPOSITIONCHANGED = 0x031E;

      public enum HitTestResult
      {
        Error = -2,
        Transparent = -1,
        Nowhere = 0,
        Client = 1,
        Caption = 2,
        SysMenu = 3,
        GrowBox = 4,
        Size = GrowBox,
        Menu = 5,
        HScroll = 6,
        VScroll = 7,
        MinButton = 8,
        MaxButton = 9,
        Left = 10,
        Right = 11,
        Top = 12,
        TopLeft = 13,
        TopRight = 14,
        Bottom = 15,
        BottomLeft = 16,
        BottomRight = 17,
        Border = 18,
        Reduce = MinButton,
        Zoom = MaxButton,
        SizeFirst = Left,
        SizeLast = BottomRight,
        Object = 19,
        Close = 20,
        Help = 21
      }

      public struct MARGINS
      {
        public MARGINS(Padding value)
        {
          Left = value.Left;
          Right = value.Right;
          Top = value.Top;
          Bottom = value.Bottom;
        }
        public int Left;
        public int Right;
        public int Top;
        public int Bottom;
      }

      [DllImport("dwmapi.dll", PreserveSig = false)]
      public static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, [In] ref MARGINS pMarInset);
      [DllImport("dwmapi.dll", PreserveSig = false)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool DwmIsCompositionEnabled();
      [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
      public static extern bool DeleteObject(IntPtr hObject);
      [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
      public static extern bool DeleteDC(IntPtr hdc);

      [StructLayout(LayoutKind.Sequential)]
      public struct DTTOPTS
      {
        public int dwSize;
        [MarshalAs(UnmanagedType.U4)]
        public DrawThemeTextFlags dwFlags;
        public int crText;
        public int crBorder;
        public int crShadow;
        public int iTextShadowType;
        public Point ptShadowOffset;
        public int iBorderSize;
        public int iFontPropId;
        public int iColorPropId;
        public int iStateId;
        public bool fApplyOverlay;
        public int iGlowSize;
        public int pfnDrawTextCallback;
        public IntPtr lParam;
      }

      [Flags()]
      public enum DrawThemeTextFlags
      {
        TextColor = 1 << 0,
        BorderColor = 1 << 1,
        ShadowColor = 1 << 2,
        ShadowType = 1 << 3,
        ShadowOffset = 1 << 4,
        BorderSize = 1 << 5,
        FontProp = 1 << 6,
        ColorProp = 1 << 7,
        StateId = 1 << 8,
        CalcRect = 1 << 9,
        ApplyOverlay = 1 << 10,
        GlowSize = 1 << 11,
        Callback = 1 << 12,
        Composited = 1 << 13
      }

      [StructLayout(LayoutKind.Sequential)]
      public class BITMAPINFO
      {
        public int biSize;
        public int biWidth;
        public int biHeight;
        public short biPlanes;
        public short biBitCount;
        public int biCompression;
        public int biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public int biClrUsed;
        public int biClrImportant;
        public byte bmiColors_rgbBlue;
        public byte bmiColors_rgbGreen;
        public byte bmiColors_rgbRed;
        public byte bmiColors_rgbReserved;
      }

      [StructLayout(LayoutKind.Sequential)]
      public struct RECT
      {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public RECT(int left, int top, int right, int bottom)
        {
          Left = left;
          Top = top;
          Right = right;
          Bottom = bottom;
        }

        public RECT(Rectangle rectangle)
        {
          Left = rectangle.X;
          Top = rectangle.Y;
          Right = rectangle.Right;
          Bottom = rectangle.Bottom;
        }

        public override string ToString()
        {
          return "Left: " + Left + ", " + "Top: " + Top + ", Right: " + Right + ", Bottom: " + Bottom;
        }
      }

      #endregion

      #region Credentials

      internal const int CREDUI_MAX_USERNAME_LENGTH = 256 + 1 + 256;
      internal const int CREDUI_MAX_PASSWORD_LENGTH = 256;

      [Flags]
      public enum CREDUI_FLAGS
      {
        INCORRECT_PASSWORD = 0x1,
        DO_NOT_PERSIST = 0x2,
        REQUEST_ADMINISTRATOR = 0x4,
        EXCLUDE_CERTIFICATES = 0x8,
        REQUIRE_CERTIFICATE = 0x10,
        SHOW_SAVE_CHECK_BOX = 0x40,
        ALWAYS_SHOW_UI = 0x80,
        REQUIRE_SMARTCARD = 0x100,
        PASSWORD_ONLY_OK = 0x200,
        VALIDATE_USERNAME = 0x400,
        COMPLETE_USERNAME = 0x800,
        PERSIST = 0x1000,
        SERVER_CREDENTIAL = 0x4000,
        EXPECT_CONFIRMATION = 0x20000,
        GENERIC_CREDENTIALS = 0x40000,
        USERNAME_TARGET_CREDENTIALS = 0x80000,
        KEEP_USERNAME = 0x100000
      }

      [Flags]
      public enum CredUIWinFlags
      {
        Generic = 0x1,
        Checkbox = 0x2,
        AutoPackageOnly = 0x10,
        InCredOnly = 0x20,
        EnumerateAdmins = 0x100,
        EnumerateCurrentUser = 0x200,
        SecurePrompt = 0x1000,
        Pack32Wow = 0x10000000
      }

      internal enum CredUIReturnCodes
      {
        NO_ERROR = 0,
        ERROR_CANCELLED = 1223,
        ERROR_NO_SUCH_LOGON_SESSION = 1312,
        ERROR_NOT_FOUND = 1168,
        ERROR_INVALID_ACCOUNT_NAME = 1315,
        ERROR_INSUFFICIENT_BUFFER = 122,
        ERROR_INVALID_PARAMETER = 87,
        ERROR_INVALID_FLAGS = 1004
      }

      internal enum CredTypes
      {
        CRED_TYPE_GENERIC = 1,
        CRED_TYPE_DOMAIN_PASSWORD = 2,
        CRED_TYPE_DOMAIN_CERTIFICATE = 3,
        CRED_TYPE_DOMAIN_VISIBLE_PASSWORD = 4
      }

      internal enum CredPersist
      {
        Session = 1,
        LocalMachine = 2,
        Enterprise = 3
      }

      [DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "CredReadW", SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      extern static internal bool CredRead(string TargetName, CredTypes Type, int Flags, out IntPtr Credential);

      [DllImport("advapi32.dll"), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
      extern static internal void CredFree(IntPtr Buffer);

      [DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "CredDeleteW", SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      extern static internal bool CredDelete(string TargetName, CredTypes Type, int Flags);

      [DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "CredWriteW", SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      extern static internal bool CredWrite(ref CREDENTIAL Credential, int Flags);

      [DllImport("credui.dll", CharSet = CharSet.Unicode, SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool CredPackAuthenticationBuffer(uint dwFlags, string pszUserName, string pszPassword, IntPtr pPackedCredentials, ref uint pcbPackedCredentials);

      [DllImport("credui.dll", CharSet = CharSet.Unicode, SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      public static extern bool CredUnPackAuthenticationBuffer(uint dwFlags, IntPtr pAuthBuffer, uint cbAuthBuffer, StringBuilder pszUserName, ref uint pcchMaxUserName, StringBuilder pszDomainName, ref uint pcchMaxDomainName, StringBuilder pszPassword, ref uint pcchMaxPassword);

      // Disable the "Internal field is never assigned to" warning.
#pragma warning disable 649
      // This type does not own the IntPtr native resource; when CredRead is used, CredFree must be called on the
      // IntPtr that the struct was marshalled from to release all resources including the CredentialBlob IntPtr,
      // When allocating the struct manually for CredWrite you should also manually deallocate the CredentialBlob.
      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1049:TypesThatOwnNativeResourcesShouldBeDisposable")]
      internal struct CREDENTIAL
      {
        public int Flags;
        public CredTypes Type;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string TargetName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string Comment;
        public long LastWritten;
        public uint CredentialBlobSize;
        // Since the resource pointed to must be either released manually or by CredFree, SafeHandle is not appropriate here
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
        public IntPtr CredentialBlob;
        [MarshalAs(UnmanagedType.U4)]
        public CredPersist Persist;
        public int AttributeCount;
        public IntPtr Attributes;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string TargetAlias;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string UserName;
      }
#pragma warning restore 649

      #endregion

    }

    #endregion

    #region NativeCommonFileDialog
    // Dummy base interface for CommonFileDialog coclasses
    private interface NativeCommonFileDialog
    { }
    #endregion

    #region NativeFileOpenDialog
    // ---------------------------------------------------------
    // Coclass interfaces - designed to "look like" the object 
    // in the API, so that the 'new' operator can be used in a 
    // straightforward way. Behind the scenes, the C# compiler
    // morphs all 'new CoClass()' calls to 'new CoClassWrapper()'
    [ComImport,
     Guid(IIDGuid.IFileOpenDialog),
     CoClass(typeof(FileOpenDialogRCW))]
    private interface NativeFileOpenDialog : IFileOpenDialog
    {
    }
    #endregion

    #region NativeFileSaveDialog
    [ComImport,
     Guid(IIDGuid.IFileSaveDialog),
     CoClass(typeof(FileSaveDialogRCW))]
    private interface NativeFileSaveDialog : IFileSaveDialog
    {
    }
    #endregion

    #region KnownFolderManager
    [ComImport,
     Guid(IIDGuid.IKnownFolderManager),
     CoClass(typeof(KnownFolderManagerRCW))]
    private interface KnownFolderManager : IKnownFolderManager
    {
    }
    #endregion

    #region FileOpenDialogRCW
    // ---------------------------------------------------
    // .NET classes representing runtime callable wrappers
    [ComImport,
     ClassInterface(ClassInterfaceType.None),
     TypeLibType(TypeLibTypeFlags.FCanCreate),
     Guid(CLSIDGuid.FileOpenDialog)]
    private class FileOpenDialogRCW
    {
    }
    #endregion

    #region FileSaveDialogRCW
    [ComImport,
     ClassInterface(ClassInterfaceType.None),
     TypeLibType(TypeLibTypeFlags.FCanCreate),
     Guid(CLSIDGuid.FileSaveDialog)]
    private class FileSaveDialogRCW
    {
    }
    #endregion

    #region KnownFolderManagerRCW
    [ComImport,
     ClassInterface(ClassInterfaceType.None),
     TypeLibType(TypeLibTypeFlags.FCanCreate),
     Guid(CLSIDGuid.KnownFolderManager)]
    private class KnownFolderManagerRCW
    {
    }
    #endregion

    #region IModalWindow
    [ComImport(),
     Guid(IIDGuid.IModalWindow),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IModalWindow
    {

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime),
       PreserveSig]
      int Show([In] IntPtr parent);
    }
    #endregion

    #region IFileDialog
    [ComImport(),
     Guid(IIDGuid.IFileDialog),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IFileDialog : IModalWindow
    {
      // Defined on IModalWindow - repeated here due to requirements of COM interop layer
      // --------------------------------------------------------------------------------
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime),
       PreserveSig]
      int Show([In] IntPtr parent);

      // IFileDialog-Specific interface members
      // --------------------------------------------------------------------------------
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetFileTypes([In] uint cFileTypes, [In, MarshalAs(UnmanagedType.LPArray)] NativeMethods.COMDLG_FILTERSPEC[] rgFilterSpec);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetFileTypeIndex([In] uint iFileType);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetFileTypeIndex(out uint piFileType);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void Advise([In, MarshalAs(UnmanagedType.Interface)] IFileDialogEvents pfde, out uint pdwCookie);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void Unadvise([In] uint dwCookie);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetOptions([In] NativeMethods.FOS fos);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetOptions(out NativeMethods.FOS pfos);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetDefaultFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetFolder([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetCurrentSelection([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetFileName([In, MarshalAs(UnmanagedType.LPWStr)] string pszName);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetTitle([In, MarshalAs(UnmanagedType.LPWStr)] string pszTitle);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetOkButtonLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszText);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetFileNameLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetResult([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void AddPlace([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, NativeMethods.FDAP fdap);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetDefaultExtension([In, MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void Close([MarshalAs(UnmanagedType.Error)] int hr);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetClientGuid([In] ref Guid guid);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void ClearClientData();

      // Not supported:  IShellItemFilter is not defined, converting to IntPtr
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetFilter([MarshalAs(UnmanagedType.Interface)] IntPtr pFilter);
    } 
    #endregion

    #region IFileOpenDialog
    [ComImport(),
     Guid(IIDGuid.IFileOpenDialog),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IFileOpenDialog : IFileDialog
    {
      // Defined on IModalWindow - repeated here due to requirements of COM interop layer
      // --------------------------------------------------------------------------------
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime),
       PreserveSig]
      int Show([In] IntPtr parent);

      // Defined on IFileDialog - repeated here due to requirements of COM interop layer
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetFileTypes([In] uint cFileTypes, [In] ref NativeMethods.COMDLG_FILTERSPEC rgFilterSpec);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetFileTypeIndex([In] uint iFileType);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetFileTypeIndex(out uint piFileType);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void Advise([In, MarshalAs(UnmanagedType.Interface)] IFileDialogEvents pfde, out uint pdwCookie);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void Unadvise([In] uint dwCookie);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetOptions([In] NativeMethods.FOS fos);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetOptions(out NativeMethods.FOS pfos);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetDefaultFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetFolder([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetCurrentSelection([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetFileName([In, MarshalAs(UnmanagedType.LPWStr)] string pszName);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetTitle([In, MarshalAs(UnmanagedType.LPWStr)] string pszTitle);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetOkButtonLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszText);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetFileNameLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetResult([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void AddPlace([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, NativeMethods.FDAP fdap);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetDefaultExtension([In, MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void Close([MarshalAs(UnmanagedType.Error)] int hr);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetClientGuid([In] ref Guid guid);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void ClearClientData();

      // Not supported:  IShellItemFilter is not defined, converting to IntPtr
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetFilter([MarshalAs(UnmanagedType.Interface)] IntPtr pFilter);

      // Defined by IFileOpenDialog
      // ---------------------------------------------------------------------------------
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetResults([MarshalAs(UnmanagedType.Interface)] out IShellItemArray ppenum);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetSelectedItems([MarshalAs(UnmanagedType.Interface)] out IShellItemArray ppsai);
    }
    #endregion

    #region IFileSaveDialog
    [ComImport(),
     Guid(IIDGuid.IFileSaveDialog),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IFileSaveDialog : IFileDialog
    {
      // Defined on IModalWindow - repeated here due to requirements of COM interop layer
      // --------------------------------------------------------------------------------
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime),
       PreserveSig]
      int Show([In] IntPtr parent);

      // Defined on IFileDialog - repeated here due to requirements of COM interop layer
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetFileTypes([In] uint cFileTypes, [In] ref NativeMethods.COMDLG_FILTERSPEC rgFilterSpec);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetFileTypeIndex([In] uint iFileType);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetFileTypeIndex(out uint piFileType);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void Advise([In, MarshalAs(UnmanagedType.Interface)] IFileDialogEvents pfde, out uint pdwCookie);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void Unadvise([In] uint dwCookie);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetOptions([In] NativeMethods.FOS fos);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetOptions(out NativeMethods.FOS pfos);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetDefaultFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetFolder([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetFolder([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetCurrentSelection([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetFileName([In, MarshalAs(UnmanagedType.LPWStr)] string pszName);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetTitle([In, MarshalAs(UnmanagedType.LPWStr)] string pszTitle);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetOkButtonLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszText);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetFileNameLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetResult([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void AddPlace([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, NativeMethods.FDAP fdap);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetDefaultExtension([In, MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void Close([MarshalAs(UnmanagedType.Error)] int hr);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetClientGuid([In] ref Guid guid);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void ClearClientData();

      // Not supported:  IShellItemFilter is not defined, converting to IntPtr
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetFilter([MarshalAs(UnmanagedType.Interface)] IntPtr pFilter);

      // Defined by IFileSaveDialog interface
      // -----------------------------------------------------------------------------------

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetSaveAsItem([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi);

      // Not currently supported: IPropertyStore
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetProperties([In, MarshalAs(UnmanagedType.Interface)] IntPtr pStore);

      // Not currently supported: IPropertyDescriptionList
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetCollectedProperties([In, MarshalAs(UnmanagedType.Interface)] IntPtr pList, [In] int fAppendDefault);

      // Not currently supported: IPropertyStore
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetProperties([MarshalAs(UnmanagedType.Interface)] out IntPtr ppStore);

      // Not currently supported: IPropertyStore, IFileOperationProgressSink
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void ApplyProperties([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, [In, MarshalAs(UnmanagedType.Interface)] IntPtr pStore, [In, ComAliasName("Interop.wireHWND")] ref IntPtr hwnd, [In, MarshalAs(UnmanagedType.Interface)] IntPtr pSink);
    }
    #endregion

    #region IFileDialogEvents
    [ComImport,
     Guid(IIDGuid.IFileDialogEvents),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IFileDialogEvents
    {
      // NOTE: some of these callbacks are cancelable - returning S_FALSE means that 
      // the dialog should not proceed (e.g. with closing, changing folder); to 
      // support this, we need to use the PreserveSig attribute to enable us to return
      // the proper HRESULT
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime),
       PreserveSig]
      HRESULT OnFileOk([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime),
       PreserveSig]
      HRESULT OnFolderChanging([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd, [In, MarshalAs(UnmanagedType.Interface)] IShellItem psiFolder);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void OnFolderChange([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void OnSelectionChange([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void OnShareViolation([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd, [In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, out NativeMethods.FDE_SHAREVIOLATION_RESPONSE pResponse);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void OnTypeChange([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void OnOverwrite([In, MarshalAs(UnmanagedType.Interface)] IFileDialog pfd, [In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, out NativeMethods.FDE_OVERWRITE_RESPONSE pResponse);
    }
    #endregion

    #region IShellItem
    [ComImport,
     Guid(IIDGuid.IShellItem),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IShellItem
    {
      // Not supported: IBindCtx
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void BindToHandler([In, MarshalAs(UnmanagedType.Interface)] IntPtr pbc, [In] ref Guid bhid, [In] ref Guid riid, out IntPtr ppv);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetParent([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetDisplayName([In] NativeMethods.SIGDN sigdnName, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetAttributes([In] uint sfgaoMask, out uint psfgaoAttribs);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void Compare([In, MarshalAs(UnmanagedType.Interface)] IShellItem psi, [In] uint hint, out int piOrder);
    }
    #endregion

    #region IShellItemArray
    [ComImport,
     Guid(IIDGuid.IShellItemArray),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IShellItemArray
    {
      // Not supported: IBindCtx
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void BindToHandler([In, MarshalAs(UnmanagedType.Interface)] IntPtr pbc, [In] ref Guid rbhid, [In] ref Guid riid, out IntPtr ppvOut);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetPropertyStore([In] int Flags, [In] ref Guid riid, out IntPtr ppv);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetPropertyDescriptionList([In] ref NativeMethods.PROPERTYKEY keyType, [In] ref Guid riid, out IntPtr ppv);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetAttributes([In] NativeMethods.SIATTRIBFLAGS dwAttribFlags, [In] uint sfgaoMask, out uint psfgaoAttribs);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetCount(out uint pdwNumItems);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetItemAt([In] uint dwIndex, [MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

      // Not supported: IEnumShellItems (will use GetCount and GetItemAt instead)
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void EnumItems([MarshalAs(UnmanagedType.Interface)] out IntPtr ppenumShellItems);
    }
    #endregion

    #region IKnownFolder
    [ComImport,
     Guid(IIDGuid.IKnownFolder),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IKnownFolder
    {
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetId(out Guid pkfid);

      // Not yet supported - adding to fill slot in vtable
      void spacer1();
      //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      //void GetCategory(out mbtagKF_CATEGORY pCategory);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetShellItem([In] uint dwFlags, ref Guid riid, out IShellItem ppv);
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetPath([In] uint dwFlags, [MarshalAs(UnmanagedType.LPWStr)] out string ppszPath);
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetPath([In] uint dwFlags, [In, MarshalAs(UnmanagedType.LPWStr)] string pszPath);
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetLocation([In] uint dwFlags, [Out, ComAliasName("Interop.wirePIDL")] IntPtr ppidl);
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetFolderType(out Guid pftid);
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetRedirectionCapabilities(out uint pCapabilities);

      // Not yet supported - adding to fill slot in vtable
      void spacer2();
      //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      //void GetFolderDefinition(out tagKNOWNFOLDER_DEFINITION pKFD);
    }
    #endregion

    #region IKnownFolderManager
    [ComImport,
     Guid(IIDGuid.IKnownFolderManager),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IKnownFolderManager
    {
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void FolderIdFromCsidl([In] int nCsidl, out Guid pfid);
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void FolderIdToCsidl([In] ref Guid rfid, out int pnCsidl);
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetFolderIds([Out] IntPtr ppKFId, [In, Out] ref uint pCount);
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetFolder([In] ref Guid rfid, [MarshalAs(UnmanagedType.Interface)] out IKnownFolder ppkf);
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetFolderByName([In, MarshalAs(UnmanagedType.LPWStr)] string pszCanonicalName, [MarshalAs(UnmanagedType.Interface)] out IKnownFolder ppkf);
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void RegisterFolder([In] ref Guid rfid, [In] ref NativeMethods.KNOWNFOLDER_DEFINITION pKFD);
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void UnregisterFolder([In] ref Guid rfid);
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void FindFolderFromPath([In, MarshalAs(UnmanagedType.LPWStr)] string pszPath, [In] NativeMethods.FFFP_MODE mode, [MarshalAs(UnmanagedType.Interface)] out IKnownFolder ppkf);
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void FindFolderFromIDList([In] IntPtr pidl, [MarshalAs(UnmanagedType.Interface)] out IKnownFolder ppkf);
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void Redirect([In] ref Guid rfid, [In] IntPtr hwnd, [In] uint Flags, [In, MarshalAs(UnmanagedType.LPWStr)] string pszTargetPath, [In] uint cFolders, [In] ref Guid pExclusion, [MarshalAs(UnmanagedType.LPWStr)] out string ppszError);
    }
    #endregion

    #region IFileDialogCustomize
    [ComImport,
     Guid(IIDGuid.IFileDialogCustomize),
     InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IFileDialogCustomize
    {
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void EnableOpenDropDown([In] int dwIDCtl);

      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void AddMenu([In] int dwIDCtl, [In, MarshalAs(UnmanagedType.LPWStr)] string pszLabel);
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void AddPushButton([In] int dwIDCtl, [In, MarshalAs(UnmanagedType.LPWStr)] string pszLabel);
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void AddComboBox([In] int dwIDCtl);
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void AddRadioButtonList([In] int dwIDCtl);
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void AddCheckButton([In] int dwIDCtl, [In, MarshalAs(UnmanagedType.LPWStr)] string pszLabel, [In] bool bChecked);
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void AddEditBox([In] int dwIDCtl, [In, MarshalAs(UnmanagedType.LPWStr)] string pszText);
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void AddSeparator([In] int dwIDCtl);
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void AddText([In] int dwIDCtl, [In, MarshalAs(UnmanagedType.LPWStr)] string pszText);
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetControlLabel([In] int dwIDCtl, [In, MarshalAs(UnmanagedType.LPWStr)] string pszLabel);
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetControlState([In] int dwIDCtl, [Out] out NativeMethods.CDCONTROLSTATE pdwState);
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetControlState([In] int dwIDCtl, [In] NativeMethods.CDCONTROLSTATE dwState);
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetEditBoxText([In] int dwIDCtl, [Out] IntPtr ppszText);
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetEditBoxText([In] int dwIDCtl, [In, MarshalAs(UnmanagedType.LPWStr)] string pszText);
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetCheckButtonState([In] int dwIDCtl, [Out] out bool pbChecked);
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetCheckButtonState([In] int dwIDCtl, [In] bool bChecked);
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void AddControlItem([In] int dwIDCtl, [In] int dwIDItem, [In, MarshalAs(UnmanagedType.LPWStr)] string pszLabel);
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void RemoveControlItem([In] int dwIDCtl, [In] int dwIDItem);
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void RemoveAllControlItems([In] int dwIDCtl);
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetControlItemState([In] int dwIDCtl, [In] int dwIDItem, [Out] out NativeMethods.CDCONTROLSTATE pdwState);
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetControlItemState([In] int dwIDCtl, [In] int dwIDItem, [In] NativeMethods.CDCONTROLSTATE dwState);
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void GetSelectedControlItem([In] int dwIDCtl, [Out] out int pdwIDItem);
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void SetSelectedControlItem([In] int dwIDCtl, [In] int dwIDItem); // Not valid for OpenDropDown
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void StartVisualGroup([In] int dwIDCtl, [In, MarshalAs(UnmanagedType.LPWStr)] string pszLabel);
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void EndVisualGroup();
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      void MakeProminent([In] int dwIDCtl);
    }
    #endregion

    #region WindowHandleWrapper
    class WindowHandleWrapper : IWin32Window
    {
      private IntPtr _handle;

      public WindowHandleWrapper(IntPtr handle)
      {
        _handle = handle;
      }

      #region IWin32Window Members

      public IntPtr Handle
      {
        get { return _handle; }
      }

      #endregion
    }
    #endregion

    private FolderBrowserDialog _downlevelDialog;
    private string _description;
    private bool _useDescriptionForTitle;
    private string _selectedPath;
    private System.Environment.SpecialFolder _rootFolder;

    /// <summary>
    /// Occurs when the user clicks the Help button on the dialog box.
    /// </summary>
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler HelpRequest
    {
      add
      {
        base.HelpRequest += value;
      }
      remove
      {
        base.HelpRequest -= value;
      }
    }

    /// <summary>
    /// Creates a new instance of the <see cref="VistaFolderBrowserDialog" /> class.
    /// </summary>
    public VistaFolderBrowserDialog()
    {
      if (!IsVistaFolderDialogSupported)
        _downlevelDialog = new FolderBrowserDialog();
      else
        Reset();
    }

    #region Public Properties

    /// <summary>
    /// Gets a value that indicates whether the current OS supports Vista-style common file dialogs.
    /// </summary>
    /// <value>
    /// <see langword="true" /> on Windows Vista or newer operating systems; otherwise, <see langword="false" />.
    /// </value>
    [Browsable(false)]
    public static bool IsVistaFolderDialogSupported
    {
      get
      {
        return NativeMethods.IsWindowsVistaOrLater;
      }
    }

    /// <summary>
    /// Gets or sets the descriptive text displayed above the tree view control in the dialog box, or below the list view control
    /// in the Vista style dialog.
    /// </summary>
    /// <value>
    /// The description to display. The default is an empty string ("").
    /// </value>
    [Category("Folder Browsing"), DefaultValue(""), Localizable(true), Browsable(true), Description("The descriptive text displayed above the tree view control in the dialog box, or below the list view control in the Vista style dialog.")]
    public string Description
    {
      get
      {
        if (_downlevelDialog != null)
          return _downlevelDialog.Description;
        return _description;
      }
      set
      {
        if (_downlevelDialog != null)
          _downlevelDialog.Description = value;
        else
          _description = value ?? String.Empty;
      }
    }

    /// <summary>
    /// Gets or sets the root folder where the browsing starts from. This property has no effect if the Vista style
    /// dialog is used.
    /// </summary>
    /// <value>
    /// One of the <see cref="System.Environment.SpecialFolder" /> values. The default is Desktop.
    /// </value>
    /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The value assigned is not one of the <see cref="System.Environment.SpecialFolder" /> values.</exception>
    [Localizable(false), Description("The root folder where the browsing starts from. This property has no effect if the Vista style dialog is used."), Category("Folder Browsing"), Browsable(true), DefaultValue(typeof(System.Environment.SpecialFolder), "Desktop")]
    public System.Environment.SpecialFolder RootFolder
    {
      get
      {
        if (_downlevelDialog != null)
          return _downlevelDialog.RootFolder;
        return _rootFolder;
      }
      set
      {
        if (_downlevelDialog != null)
          _downlevelDialog.RootFolder = value;
        else
          _rootFolder = value;
      }
    }

    /// <summary>
    /// Gets or sets the path selected by the user.
    /// </summary>
    /// <value>
    /// The path of the folder first selected in the dialog box or the last folder selected by the user. The default is an empty string ("").
    /// </value>
    [Browsable(true), Editor("System.Windows.Forms.Design.SelectedPathEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor)), Description("The path selected by the user."), DefaultValue(""), Localizable(true), Category("Folder Browsing")]
    public string SelectedPath
    {
      get
      {
        if (_downlevelDialog != null)
          return _downlevelDialog.SelectedPath;
        return _selectedPath;
      }
      set
      {
        if (_downlevelDialog != null)
          _downlevelDialog.SelectedPath = value;
        else
          _selectedPath = value ?? string.Empty;
      }
    }

    private bool _showNewFolderButton;

    /// <summary>
    /// Gets or sets a value indicating whether the New Folder button appears in the folder browser dialog box. This
    /// property has no effect if the Vista style dialog is used; in that case, the New Folder button is always shown.
    /// </summary>
    /// <value>
    /// <see langword="true" /> if the New Folder button is shown in the dialog box; otherwise, <see langword="false" />. The default is <see langword="true" />.
    /// </value>
    [Browsable(true), Localizable(false), Description("A value indicating whether the New Folder button appears in the folder browser dialog box. This property has no effect if the Vista style dialog is used; in that case, the New Folder button is always shown."), DefaultValue(true), Category("Folder Browsing")]
    public bool ShowNewFolderButton
    {
      get
      {
        if (_downlevelDialog != null)
          return _downlevelDialog.ShowNewFolderButton;
        return _showNewFolderButton;
      }
      set
      {
        if (_downlevelDialog != null)
          _downlevelDialog.ShowNewFolderButton = value;
        else
          _showNewFolderButton = value;
      }
    }


    /// <summary>
    /// Gets or sets a value that indicates whether to use the value of the <see cref="Description" /> property
    /// as the dialog title for Vista style dialogs. This property has no effect on old style dialogs.
    /// </summary>
    /// <value><see langword="true" /> to indicate that the value of the <see cref="Description" /> property is used as dialog title; <see langword="false" />
    /// to indicate the value is added as additional text to the dialog. The default is <see langword="false" />.</value>
    [Category("Folder Browsing"), DefaultValue(false), Description("A value that indicates whether to use the value of the Description property as the dialog title for Vista style dialogs. This property has no effect on old style dialogs.")]
    public bool UseDescriptionForTitle
    {
      get { return _useDescriptionForTitle; }
      set { _useDescriptionForTitle = value; }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Resets all properties to their default values.
    /// </summary>
    public override void Reset()
    {
      _description = string.Empty;
      _useDescriptionForTitle = false;
      _selectedPath = string.Empty;
      _rootFolder = Environment.SpecialFolder.Desktop;
      _showNewFolderButton = true;
    }

    #endregion

    #region Protected Methods

    /// <summary>
    /// Specifies a common dialog box.
    /// </summary>
    /// <param name="hwndOwner">A value that represents the window handle of the owner window for the common dialog box.</param>
    /// <returns><see langword="true" /> if the file could be opened; otherwise, <see langword="false" />.</returns>
    protected override bool RunDialog(IntPtr hwndOwner)
    {
      if (_downlevelDialog != null)
        return _downlevelDialog.ShowDialog(hwndOwner == IntPtr.Zero ? null : new WindowHandleWrapper(hwndOwner)) == DialogResult.OK;

      IFileDialog dialog = null;
      try
      {
        dialog = new NativeFileOpenDialog();
        SetDialogProperties(dialog);
        int result = dialog.Show(hwndOwner);
        if (result < 0)
        {
          if ((uint)result == (uint)HRESULT.ERROR_CANCELLED)
            return false;
          else
            throw System.Runtime.InteropServices.Marshal.GetExceptionForHR(result);
        }
        GetResult(dialog);
        return true;
      }
      finally
      {
        if (dialog != null)
          System.Runtime.InteropServices.Marshal.FinalReleaseComObject(dialog);
      }
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="VistaFolderBrowserDialog" /> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing"><see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
      try
      {
        if (disposing && _downlevelDialog != null)
          _downlevelDialog.Dispose();
      }
      finally
      {
        base.Dispose(disposing);
      }
    }

    #endregion

    #region Private Methods

    private void SetDialogProperties(IFileDialog dialog)
    {
      // Description
      if (!string.IsNullOrEmpty(_description))
      {
        if (_useDescriptionForTitle)
        {
          dialog.SetTitle(_description);
        }
        else
        {
          IFileDialogCustomize customize = (IFileDialogCustomize)dialog;
          customize.AddText(0, _description);
        }
      }

      dialog.SetOptions(NativeMethods.FOS.FOS_PICKFOLDERS | NativeMethods.FOS.FOS_FORCEFILESYSTEM | NativeMethods.FOS.FOS_FILEMUSTEXIST);

      if (!string.IsNullOrEmpty(_selectedPath))
      {
        string parent = Path.GetDirectoryName(_selectedPath);
        if (parent == null || !Directory.Exists(parent))
        {
          dialog.SetFileName(_selectedPath);
        }
        else
        {
          string folder = Path.GetFileName(_selectedPath);
          dialog.SetFolder(NativeMethods.CreateItemFromParsingName(parent));
          dialog.SetFileName(folder);
        }
      }
    }

    private void GetResult(IFileDialog dialog)
    {
      IShellItem item;
      dialog.GetResult(out item);
      item.GetDisplayName(NativeMethods.SIGDN.SIGDN_FILESYSPATH, out _selectedPath);
    }

    #endregion
  }
}
