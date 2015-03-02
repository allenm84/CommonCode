using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;

namespace System.Common.References
{
  /// <summary>
  /// Prompts the user to select or type in a folder or a file. This class cannot be inherited.
  /// </summary>
  [Designer("System.Windows.Forms.Design.FolderBrowserDialogDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [DefaultEvent("HelpRequest")]
  [DefaultProperty("SelectedPath")]
  public sealed class FolderBrowseDialogEx : CommonDialog
  {
    [Flags]
    internal enum BrowseInfoFlag
    {
      /// <summary>
      /// Only return file system directories. If the user selects folders that are not part of the file system, 
      /// the OK button is grayed. The OK button remains enabled for "\\server" items, as well as "\\server\share" 
      /// and directory items. However, if the user selects a "\\server" item, passing the PIDL returned by 
      /// SHBrowseForFolder to SHGetPathFromIDList fails.
      /// </summary>
      ReturnOnlyFileSystemDirectories = 0x00000001,

      /// <summary>
      /// Do not include network folders below the domain level in the dialog box's tree view control.
      /// </summary>
      DontGoBelowDomain = 0x00000002,

      /// <summary>
      /// Include a status area in the dialog box. The callback function can set the status text by sending 
      /// messages (BFFM_SETSTATUSTEXTA) to the dialog box. This flag is not supported when BIF_NEWDIALOGSTYLE 
      /// is specified.
      /// </summary>
      IncludeStatusText = 0x00000004,

      /// <summary>
      /// Only return file system ancestors. An ancestor is a subfolder that is beneath the root folder in the 
      /// namespace hierarchy. If the user selects an ancestor of the root folder that is not part of the file 
      /// system, the OK button is grayed
      /// </summary>
      ReturnOnlyFileSystemAncestors = 0x00000008,

      /// <summary>
      /// Include an edit control in the browse dialog box that allows the user to type the name of an item.
      /// </summary>
      IncludeEditTextBox = 0x00000010,

      /// <summary>
      /// If the user types an invalid name into the edit box, the browse dialog box calls the application's BrowseCallbackProc 
      /// with the BFFM_VALIDATEFAILED message. This flag is ignored if BIF_EDITBOX is not specified
      /// </summary>
      ValidateEditTextBox = 0x00000020,

      /// <summary>
      /// Use the new user interface. Setting this flag provides the user with a larger dialog box that can be resized. The dialog box 
      /// has several new capabilities, including: drag-and-drop capability within the dialog box, reordering, shortcut menus, new folders, 
      /// delete, and other shortcut menu commands. Caller needs to call OleInitialize() before using this API.
      /// </summary>
      NewDialogStyle = 0x00000040,

      /// <summary>
      /// Use the new user interface, including an edit box. This flag is equivalent to BIF_EDITBOX | BIF_NEWDIALOGSTYLE
      /// </summary>
      NewUserInterface = (NewDialogStyle | IncludeEditTextBox),

      /// <summary>
      /// The browse dialog box can display URLs. The BIF_USENEWUI and BIF_BROWSEINCLUDEFILES flags must also be set. If any of 
      /// these three flags are not set, the browser dialog box rejects URLs. Even when these flags are set, the browse dialog box 
      /// displays URLs only if the folder that contains the selected item supports URLs. When the folder's 
      /// IShellFolder::GetAttributesOf method is called to request the selected item's attributes, the folder must set the 
      /// SFGAO_FOLDER attribute flag. Otherwise, the browse dialog box will not display the URL.
      /// </summary>
      IncludeURLS = 0x00000080,

      /// <summary>
      /// When combined with BIF_NEWDIALOGSTYLE, adds a usage hint to the dialog box, in place of the edit box. 
      /// BIF_EDITBOX overrides this flag
      /// </summary>
      ProvideUsageHint = 0x00000100,

      /// <summary>
      /// Do not include the New Folder button in the browse dialog box.
      /// </summary>
      NoNewFolderButton = 0x00000200,

      /// <summary>
      /// When the selected item is a shortcut, return the PIDL of the shortcut itself rather than its target.
      /// </summary>
      NoTranslateTargets = 0x00000400,

      /// <summary>
      /// Only return computers. If the user selects anything other than a computer, the OK button is grayed.
      /// </summary>
      BrowseComputersOnly = 0x00001000,

      /// <summary>
      /// Only allow the selection of printers. If the user selects anything other than a printer, the OK button is grayed. 
      /// In Microsoft Windows XP and later systems, the best practice is to use a Windows XP-style dialog, setting the root 
      /// of the dialog to the Printers and Faxes folder (CSIDL_PRINTERS).
      /// </summary>
      BrowsePrintersOnly = 0x00002000,

      /// <summary>
      /// The browse dialog box displays files as well as folders.
      /// </summary>
      BrowseEverything = 0x00004000,

      /// <summary>
      /// The browse dialog box can display shareable resources on remote systems. This is intended for applications 
      /// that want to expose remote shares on a local system. The BIF_NEWDIALOGSTYLE flag must also be set.
      /// </summary>
      DisplayShareableResrcs = 0x00008000,

      /// <summary>
      /// Allow folder junctions such as a library or a compressed file with a .zip file name extension to be browsed.
      /// </summary>
      BrowseJunctions = 0x00010000
    };

    private BrowseCallbackProc mCurrentCallback;
    private string mDescriptionText;
    private Environment.SpecialFolder mRootFolder;
    private string mSelectedPath;
    private bool mSelectedPathNeedsCheck;
    private bool mShowNewFolderButton;
    private bool mIncludeFiles;

    /// <summary>
    /// Initializes a new instance of the MLA.Controls.Customs.FolderBrowserDialogEx class.
    /// </summary>
    public FolderBrowseDialogEx()
    {
      this.Reset();
    }

    private int FolderBrowserDialog_BrowseCallbackProc(IntPtr hwnd, int msg, IntPtr lParam, IntPtr lpData)
    {
      switch (msg)
      {
        case Win32.BFFM_INITIALIZED:
          if (this.mSelectedPath.Length != 0)
          {
            HandleRef handle = new HandleRef(null, hwnd);
            Win32.SendMessage(
              handle,
              Win32.BFFM_SETSELECTION,
              Win32.TRUE,
              this.mSelectedPath);
          }
          break;

        case Win32.BFFM_SELCHANGED:
        {
          IntPtr pidl = lParam;
          if (pidl != IntPtr.Zero)
          {
            HandleRef handle = new HandleRef(null, hwnd);
            IntPtr pszPath = Marshal.AllocHGlobal((int)(260 * Marshal.SystemDefaultCharSize));

            int flag = Win32.SHGetPathFromIDList(pidl, pszPath) ? 1 : 0;
            Marshal.FreeHGlobal(pszPath);

            Win32.SendMessage(
              handle,
              Win32.BFFM_ENABLEOK,
              Win32.FALSE,
              flag);
          }
          break;
        }
      }
      return 0;
    }

    private static IMalloc GetSHMalloc()
    {
      IMalloc[] ppMalloc = new IMalloc[1];
      Win32.SHGetMalloc(ppMalloc);
      return ppMalloc[0];
    }

    /// <summary>
    /// Resets properties to their default values.
    /// </summary>
    public override void Reset()
    {
      this.mRootFolder = Environment.SpecialFolder.Desktop;
      this.mDescriptionText = string.Empty;
      this.mSelectedPath = string.Empty;
      this.mSelectedPathNeedsCheck = false;
      this.mShowNewFolderButton = true;
      this.mIncludeFiles = false;
    }

    /// <summary></summary>
    protected override bool RunDialog(IntPtr hWndOwner)
    {
      IntPtr zero = IntPtr.Zero;
      bool flag = false;

      Win32.SHGetSpecialFolderLocation(hWndOwner, (int)this.mRootFolder, ref zero);
      if (zero == IntPtr.Zero)
      {
        Win32.SHGetSpecialFolderLocation(hWndOwner, 0, ref zero);
        if (zero == IntPtr.Zero)
        {
          throw new InvalidOperationException("No Root Folder.");
        }
      }

      BrowseInfoFlag flags = BrowseInfoFlag.NewUserInterface;
      if (!this.mShowNewFolderButton)
      {
        flags |= BrowseInfoFlag.NoNewFolderButton;
      }
      if (this.mIncludeFiles)
      {
        flags |= BrowseInfoFlag.BrowseEverything;
      }

      if (Control.CheckForIllegalCrossThreadCalls && (Application.OleRequired() != ApartmentState.STA))
      {
        throw new ThreadStateException("Thread Must Be STA.");
      }
      IntPtr pidl = IntPtr.Zero;
      IntPtr hglobal = IntPtr.Zero;
      IntPtr pszPath = IntPtr.Zero;
      try
      {
        this.mCurrentCallback = new BrowseCallbackProc(this.FolderBrowserDialog_BrowseCallbackProc);
        hglobal = Marshal.AllocHGlobal((int)(260 * Marshal.SystemDefaultCharSize));
        pszPath = Marshal.AllocHGlobal((int)(260 * Marshal.SystemDefaultCharSize));

        BROWSEINFO lpbi = new BROWSEINFO();
        lpbi.pidlRoot = zero;
        lpbi.hwndOwner = hWndOwner;
        lpbi.pszDisplayName = hglobal;
        lpbi.lpszTitle = this.mDescriptionText;
        lpbi.ulFlags = (int)flags;
        lpbi.lpfn = this.mCurrentCallback;
        lpbi.lParam = IntPtr.Zero;
        lpbi.iImage = 0;

        pidl = Win32.SHBrowseForFolder(lpbi);
        if (pidl != IntPtr.Zero)
        {
          Win32.SHGetPathFromIDList(pidl, pszPath);
          this.mSelectedPathNeedsCheck = true;
          this.mSelectedPath = Marshal.PtrToStringAuto(pszPath);
          flag = true;
        }
      }
      finally
      {
        IMalloc sHMalloc = GetSHMalloc();
        sHMalloc.Free(zero);
        if (pidl != IntPtr.Zero)
        {
          sHMalloc.Free(pidl);
        }
        if (pszPath != IntPtr.Zero)
        {
          Marshal.FreeHGlobal(pszPath);
        }
        if (hglobal != IntPtr.Zero)
        {
          Marshal.FreeHGlobal(hglobal);
        }
        this.mCurrentCallback = null;
      }
      return flag;
    }

    /// <summary>
    /// Gets or sets the descriptive text displayed above the tree view control in the dialog box.
    /// </summary>
    /// <returns>The description to display. The default is an empty string ("").</returns>
    [DefaultValue("")]
    [Browsable(true)]
    [Localizable(true)]
    [Description("Gets or sets the descriptive text displayed above the tree view control in the dialog box.")]
    public string Description
    {
      get { return this.mDescriptionText; }
      set { this.mDescriptionText = (value == null) ? string.Empty : value; }
    }

    /// <summary>
    /// Gets or sets the root folder where the browsing starts from.
    /// </summary>
    /// <returns>One of the System.Environment.SpecialFolder values. The default is Desktop.</returns>
    /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">
    /// The value assigned is not one of the System.Environment.SpecialFolder values.
    /// </exception>
    [Localizable(false)]
    [Browsable(true)]
    [Description("Gets or sets the root folder where the browsing starts from.")]
    public Environment.SpecialFolder RootFolder
    {
      get { return this.mRootFolder; }
      set
      {
        if (!Enum.IsDefined(typeof(Environment.SpecialFolder), value))
        {
          throw new InvalidEnumArgumentException("value", (int)value, typeof(Environment.SpecialFolder));
        }
        this.mRootFolder = value;
      }
    }

    /// <summary>
    /// Gets or sets the path selected by the user.
    /// </summary>
    /// <returns>
    /// The path of the folder first selected in the dialog box or the last folder selected by the user. 
    /// The default is an empty string ("").
    /// </returns>
    [Editor("System.Windows.Forms.Design.SelectedPathEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
    [Browsable(true)]
    [DefaultValue("")]
    [Localizable(true)]
    [Description("Gets or sets the path selected by the user.")]
    public string SelectedPath
    {
      get
      {
        if (((this.mSelectedPath != null) && (this.mSelectedPath.Length != 0)) && this.mSelectedPathNeedsCheck)
        {
          new FileIOPermission(FileIOPermissionAccess.PathDiscovery, this.mSelectedPath).Demand();
        }
        return this.mSelectedPath;
      }
      set
      {
        this.mSelectedPath = (value == null) ? string.Empty : value;
        this.mSelectedPathNeedsCheck = false;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the New Folder button appears in the folder browser dialog box.
    /// </summary>
    [Localizable(false)]
    [DefaultValue(true)]
    [Browsable(true)]
    [Description("Gets or sets a value indicating whether the New Folder button appears in the folder browser dialog box.")]
    public bool ShowNewFolderButton
    {
      get { return this.mShowNewFolderButton; }
      set { this.mShowNewFolderButton = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether files are included and can be browsed.
    /// </summary>
    [Localizable(false)]
    [DefaultValue(false)]
    [Browsable(true)]
    [Description("Gets or sets a value indicating whether files are included and can be browsed.")]
    public bool IncludeFiles
    {
      get { return this.mIncludeFiles; }
      set { this.mIncludeFiles = value; }
    }
  }
}
