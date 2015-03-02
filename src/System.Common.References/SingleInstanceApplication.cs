using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;

namespace System.Common.References
{
  /// <summary>
  /// Represents a view that handles the command line arguments sent to a single instance
  /// application.
  /// </summary>
  public interface ISingleInstanceApplicationView
  {
    /// <summary>
    /// The main window that is shown to the user.
    /// </summary>
    Form MainWindow { get; }

    /// <summary>
    /// The method which is called after the first instance has started.
    /// </summary>
    /// <param name="commandLine"></param>
    void StartupNextInstance(IList<string> commandLine);

    /// <summary>
    /// The method which is called to start the first instance.
    /// </summary>
    /// <param name="commandLine"></param>
    void Startup(IList<string> commandLine);
  }

  /// <summary>
  /// The controller used to start a single instance application.
  /// </summary>
  public static class SingleInstanceApplicationController
  {
    private class SingleInstanceApplication : WindowsFormsApplicationBase
    {
      public SingleInstanceApplication(Form form) 
      { 
        IsSingleInstance = true;
        MainForm = form;
      }
    }

    private static ISingleInstanceApplicationView mView;

    /// <summary>
    /// Replaces the Application.Run method.
    /// </summary>
    /// <param name="view">The view to run.</param>
    /// <param name="args"></param>
    public static void Run(ISingleInstanceApplicationView view, params string[] args)
    {
      if (mView != null)
        throw new InvalidOperationException("Run has already been called in this application scope.");

      mView = view;

      SingleInstanceApplication app = new SingleInstanceApplication(mView.MainWindow);
      app.StartupNextInstance += new StartupNextInstanceEventHandler(app_StartupNextInstance);
      app.Startup += new StartupEventHandler(app_Startup);
      app.Run(args);
    }

    private static void app_Startup(object sender, StartupEventArgs e)
    {
      mView.Startup(e.CommandLine);
    }

    private static void app_StartupNextInstance(object sender, StartupNextInstanceEventArgs e)
    {
      mView.StartupNextInstance(e.CommandLine);
      e.BringToForeground = true;
    }
  }
}