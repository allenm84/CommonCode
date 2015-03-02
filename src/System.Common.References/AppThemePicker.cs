using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace System.Common.References
{
  public partial class AppThemePicker : RadForm
  {
    private class AppThemePickerDisposer { ~AppThemePickerDisposer() { Cleanup(); } }
    private static AppThemePickerDisposer _disposer = new AppThemePickerDisposer();

    private class Office2007BlueTheme : RadThemeComponentBase
    {
      public override string ThemeName { get { return "ControlDefault"; } }
      public override void Load() { /*do nothing*/ }
    }

    #region color-swatch

    private static byte[] bytes = 
    { 
      137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 72, 68, 82, 0, 
      0, 0, 16, 0, 0, 0, 16, 8, 6, 0, 0, 0, 31, 243, 255, 97, 0, 0, 0, 
      25, 116, 69, 88, 116, 83, 111, 102, 116, 119, 97, 114, 101, 0, 
      65, 100, 111, 98, 101, 32, 73, 109, 97, 103, 101, 82, 101, 97, 100, 
      121, 113, 201, 101, 60, 0, 0, 1, 187, 73, 68, 65, 84, 120, 218, 164, 
      147, 191, 74, 195, 80, 20, 198, 191, 94, 243, 199, 196, 4, 34, 24, 
      176, 142, 157, 157, 165, 131, 155, 93, 125, 6, 29, 11, 174, 66, 93, 
      74, 11, 237, 38, 65, 7, 7, 161, 62, 133, 29, 5, 31, 192, 71, 208, 77, 
      177, 197, 161, 32, 210, 14, 201, 77, 82, 239, 57, 73, 111, 91, 183, 
      98, 10, 189, 252, 114, 207, 249, 242, 125, 39, 55, 149, 110, 183, 123, 
      10, 160, 138, 205, 175, 113, 167, 211, 25, 26, 105, 154, 30, 244, 122, 
      189, 251, 77, 187, 219, 237, 118, 147, 86, 35, 203, 50, 204, 231, 115, 
      188, 62, 158, 193, 119, 77, 222, 220, 63, 25, 224, 237, 252, 28, 190, 
      89, 242, 128, 120, 168, 216, 101, 174, 62, 52, 64, 125, 44, 32, 165, 
      172, 228, 121, 14, 199, 18, 8, 3, 155, 111, 50, 11, 197, 246, 42, 91, 
      138, 3, 205, 212, 199, 2, 42, 2, 11, 164, 82, 66, 38, 146, 11, 182, 
      148, 58, 179, 252, 203, 9, 179, 73, 245, 170, 111, 225, 128, 21, 247, 
      27, 3, 157, 143, 236, 145, 237, 117, 110, 104, 46, 29, 20, 17, 146, 36, 
      97, 7, 79, 31, 77, 184, 126, 145, 185, 238, 223, 98, 52, 26, 193, 247, 
      125, 102, 90, 159, 47, 70, 112, 205, 130, 143, 239, 118, 65, 125, 122, 
      6, 244, 4, 203, 17, 8, 194, 34, 115, 54, 203, 224, 56, 14, 194, 48, 100, 
      158, 205, 102, 176, 132, 131, 192, 14, 75, 71, 241, 114, 6, 11, 7, 50, 
      149, 72, 74, 91, 36, 168, 50, 106, 155, 196, 82, 166, 122, 159, 234, 181, 
      131, 56, 142, 89, 160, 238, 221, 0, 211, 50, 179, 250, 121, 158, 135, 233, 
      116, 170, 115, 215, 111, 60, 44, 10, 242, 220, 2, 245, 173, 57, 184, 124, 
      191, 132, 185, 83, 204, 160, 31, 244, 209, 122, 249, 129, 233, 22, 153, 
      251, 135, 21, 180, 90, 138, 203, 25, 92, 95, 27, 235, 14, 200, 162, 216, 
      22, 176, 247, 202, 25, 196, 138, 45, 7, 118, 176, 200, 252, 5, 161, 102, 
      96, 235, 25, 124, 47, 29, 168, 1, 85, 232, 36, 202, 149, 247, 94, 188, 
      166, 84, 159, 11, 205, 229, 62, 213, 83, 31, 11, 140, 199, 99, 67, 168, 
      83, 23, 85, 163, 229, 65, 183, 128, 232, 72, 253, 225, 71, 223, 136, 162, 
      37, 11, 97, 128, 250, 88, 96, 50, 153, 124, 214, 106, 181, 171, 77, 63, 38, 
      245, 208, 79, 90, 217, 254, 127, 174, 95, 1, 6, 0, 230, 191, 27, 169, 130, 
      38, 87, 49, 0, 0, 0, 0, 73, 69, 78, 68, 174, 66, 96, 130 
    };

    #endregion
    private static Image colorSwatch;

    private static string filepath;
    private static string currentThemeName = "ControlDefault";

    private static object themeSyncRoot = new object();
    private static List<RadThemeComponentBase> themes;

    private static Thread loadingThemesThread;
    private static ManualResetEvent loadingThemesCompleted = new ManualResetEvent(false);
    private static SynchronizationContext setThemeContext = null;
    private static Thread waitForLoadThread;
    private static HashSet<RadButtonElement> clearText = new HashSet<RadButtonElement>();

    static AppThemePicker()
    {
      using (MemoryStream stream = new MemoryStream(bytes))
      {
        colorSwatch = Image.FromStream(stream);
      }

      string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
      string themePicker = Path.Combine(appData, "MAPA Inc", "AppThemePicker");

      if (!Directory.Exists(themePicker)) Directory.CreateDirectory(themePicker);
      filepath = Path.Combine(themePicker, "appThemeIndex.bnry");

      themes = new List<RadThemeComponentBase>();
      Application.DoEvents();

      var version = RuntimeEnvironment.GetSystemVersion();
      var directory = "";

      Application.DoEvents();
      if (version.StartsWith("v2.0"))
      {
        // read from the pre 4.0 GAC
        directory = @"C:\Windows\assembly";
      }
      else
      {
        // read from the 4.0 and later GAC
        directory = @"C:\Windows\Microsoft.NET\assembly";
      }

      Application.DoEvents();

      // start the thread to load the themes
      loadingThemesThread = new Thread(LoadThemes);
      loadingThemesThread.Name = "Loading Telerik Themes";
      loadingThemesThread.Start(directory);

      // load the index
      LoadIndex();
    }

    private static void LoadThemes(object state)
    {
      // retrieve the directory
      var directory = state as string;

      // retrieve all the assemblies
      var assemblies = Directory.GetFiles(directory,
        "Telerik*.dll", SearchOption.AllDirectories);

      Application.DoEvents();

      // add the control default theme
      themes.Add(new Office2007BlueTheme());

      // go through the assemblies
      foreach (var assembly in assemblies)
      {
        Application.DoEvents();

        var dll = Assembly.LoadFrom(assembly);
        var types = new Type[] { };

        try { types = dll.GetTypes(); }
        catch { types = null; }
        if (types == null) continue;

        foreach (var type in types)
        {
          Application.DoEvents();

          // if the type is public class [CLASS] : RadThemeComponentBase and has an empty constructor
          if (type.IsSubclassOf(typeof(RadThemeComponentBase)) &&
            type.IsClass &&
            type.IsPublic &&
            type.GetConstructor(Type.EmptyTypes) != null)
          {
            themes.Add(Activator.CreateInstance(type) as RadThemeComponentBase);
          }
        }
      }

      // we're all done!
      loadingThemesCompleted.Set();
    }

    private static void LoadIndex()
    {
      if (File.Exists(filepath))
      {
        using (Stream stream = File.OpenRead(filepath))
        {
          try
          {
            BinaryReader reader = new BinaryReader(stream);
            currentThemeName = reader.ReadString();
          }
          catch
          {
            currentThemeName = "ControlDefault";
          }
        }
      }
    }

    private static void SaveIndex()
    {
      using (Stream stream = File.Create(filepath))
      {
        BinaryWriter writer = new BinaryWriter(stream);
        writer.Write(currentThemeName);
      }
    }

    private static void Cleanup()
    {
      if (colorSwatch != null)
      {
        colorSwatch.Dispose();
        colorSwatch = null;
      }
    }

    private static void WaitForLoadCompleted()
    {
      loadingThemesCompleted.WaitOne();
      if (setThemeContext != null)
      {
        setThemeContext.Post((x) => SetTheme(), null);
      }
      else
      {
        SetTheme();
      }
    }

    private static void SetTheme()
    {
      if (!loadingThemesCompleted.WaitOne(1))
      {
        if (setThemeContext == null)
        {
          setThemeContext = SynchronizationContext.Current;
          waitForLoadThread = new Thread(WaitForLoadCompleted);
          waitForLoadThread.Name = "Wait For Load Completed";
          waitForLoadThread.IsBackground = true;
          waitForLoadThread.Start();
        }
        return;
      }

      if (clearText.Count > 0)
      {
        foreach (var button in clearText)
        {
          button.MinSize = new Size(15, 15);
          button.MaxSize = new Size(15, 15);
          button.Text = string.Empty;
        }
        clearText.Clear();
      }

      var theme = themes.SingleOrDefault(t => t.ThemeName == currentThemeName);
      if (theme != null)
      {
        ThemeResolutionService.ApplicationThemeName = theme.ThemeName;
      }
    }

    public static void AddToRadForm(RadForm form)
    {
      Size size = new Size(15, 15);
      var button = new RadButtonElement("", colorSwatch)
      {
        Alignment = ContentAlignment.MiddleCenter,
        Size = size,
        MaxSize = size,
        MinSize = size,
        DefaultSize = size,
        TextImageRelation = TextImageRelation.Overlay,
        ImageAlignment = ContentAlignment.MiddleCenter,
        Margin = new Padding(3, 3, 3, 3),
        Padding = new Padding(0),
        ShowBorder = false,
        Tag = form,
      };

      SetTheme();

      if (!loadingThemesCompleted.WaitOne(1))
      {
        button.TextImageRelation = TextImageRelation.ImageBeforeText;
        button.MinSize = new Size(90, 15);
        button.MaxSize = new Size(90, 15);
        button.Text = "Loading...";
        clearText.Add(button);
      }

      var titlePrimitive = form.FormElement.TitleBar.TitlePrimitive;
      var index = titlePrimitive.Parent.Children.IndexOf(titlePrimitive);
      titlePrimitive.Parent.Children.Insert(index + 1, button);
      button.Click += new EventHandler(button_Click);
    }

    private static void button_Click(object sender, EventArgs e)
    {
      IWin32Window owner = null;
      var button = sender as RadButtonElement;
      if (button != null)
        owner = button.Tag as IWin32Window;

      using (AppThemePicker dlg = new AppThemePicker())
      {
        dlg.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        dlg.SelectedTheme = currentThemeName;
        dlg.Text = "Set Application Theme";
        if (dlg.ShowDialog(owner) == DialogResult.OK)
        {
          var newThemeIndex = dlg.SelectedTheme;
          if (currentThemeName != newThemeIndex)
          {
            currentThemeName = dlg.SelectedTheme;
            SetTheme();

            try
            {
              SaveIndex();
            }
            catch
            {
              RadMessageBox.Show(owner, "There was a problem saving the theme. You may have to re-open the theme chooser to save the theme", "Can't Save",
                MessageBoxButtons.OK, RadMessageIcon.Info);
            }
          }
        }
      }
    }

    public string SelectedTheme
    {
      get { return cboThemes.SelectedValue as string; }
      set { cboThemes.SelectedValue = value; }
    }

    private AppThemePicker()
    {
      InitializeComponent();

      MaximumSize = Size;
      MinimumSize = Size;

      InitializeThemes();
    }

    private void InitializeThemes()
    {
      cboThemes.SortStyle = Telerik.WinControls.Enumerations.SortStyle.Ascending;
      cboThemes.Items.AddRange(themes.Select(ToItem));
    }

    private RadListDataItem ToItem(RadThemeComponentBase t)
    {
      var name = t.GetType().Name.Split(new string[] { "Theme" }, StringSplitOptions.RemoveEmptyEntries)[0];
      return new RadListDataItem(name, t.ThemeName) { Tag = t };
    }
  }
}
