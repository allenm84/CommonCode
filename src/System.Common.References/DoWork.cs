using System;
using System.ComponentModel;

namespace System.Common.References
{
  /// <summary>
  /// 
  /// </summary>
  public class DoWorkOutput
  {
    /// <summary>
    /// 
    /// </summary>
    public object Result { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Action NextOperation { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool CloseOnError { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetResult<T>()
    {
      return (T)Result;
    }
  }

  /// <summary>
  /// 
  /// </summary>
  public class DoWorkInput
  {
    private DoWorkEventArgs args;
    private DoWorkOutput output;

    /// <summary>
    /// 
    /// </summary>
    public object Argument
    {
      get { return args.Argument; }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool Cancel
    {
      get { return args.Cancel; }
      set { args.Cancel = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public object Result
    {
      get { return output.Result; }
      set { output.Result = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool CloseOnError
    {
      get { return output.CloseOnError; }
      set { output.CloseOnError = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    public DoWorkInput(DoWorkEventArgs e)
    {
      args = e;
      output = new DoWorkOutput { CloseOnError = false };
      args.Result = output;
    }
  }

  /// <summary>
  /// 
  /// </summary>
  public interface IDoWorkView
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    void ShowError(string text);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="working"></param>
    void SetIsWorking(bool working);

    /// <summary>
    /// 
    /// </summary>
    void Close();
  }

  /// <summary>
  /// 
  /// </summary>
  public class DoWorkController
  {
    private IDoWorkView mView;
    private BackgroundWorker mWorker;
    private Action<DoWorkInput> mDoWork;
    private Action<DoWorkOutput> mRunWorkerCompleted;
    private event EventHandler mWorkerStarted;

    /// <summary>
    /// 
    /// </summary>
    public bool IsBusy { get { return mWorker.IsBusy; } }

    /// <summary>
    /// 
    /// </summary>
    public bool CancellationPending { get { return mWorker.CancellationPending; } }

    /// <summary>
    /// 
    /// </summary>
    public event EventHandler WorkerStarted
    {
      add { mWorkerStarted += value; }
      remove { mWorkerStarted -= value; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="view"></param>
    public DoWorkController(IDoWorkView view)
    {
      mWorkerStarted = (o, e) => { };
      mView = view;
      mView.SetIsWorking(false);

      mWorker = new BackgroundWorker();
      mWorker.WorkerSupportsCancellation = true;
      mWorker.DoWork += new DoWorkEventHandler(mWorker_DoWork);
      mWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(mWorker_RunWorkerCompleted);
    }

    /// <summary>
    /// 
    /// </summary>
    public void CancelWorker()
    {
      mWorker.CancelAsync();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="doWork"></param>
    /// <param name="runWorkerCompleted"></param>
    /// <param name="arg"></param>
    public void StartWorker(Action doWork, Action<DoWorkOutput> runWorkerCompleted = null, object arg = null)
    {
      StartWorker((x) => doWork(), runWorkerCompleted, arg);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="doWork"></param>
    /// <param name="runWorkerCompleted"></param>
    /// <param name="arg"></param>
    public void StartWorker(Action<DoWorkInput> doWork, Action<DoWorkOutput> runWorkerCompleted = null, object arg = null)
    {
      mDoWork = doWork;
      mRunWorkerCompleted = runWorkerCompleted;

      mView.SetIsWorking(true);
      mWorker.RunWorkerAsync(arg);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void mWorker_DoWork(object sender, DoWorkEventArgs e)
    {
      mWorkerStarted(this, EventArgs.Empty);
      mDoWork(new DoWorkInput(e));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void mWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      try
      {
        Action next = null;
        DoWorkOutput output = null;

        if (!e.Cancelled)
        {
          output = e.Result as DoWorkOutput;
        }

        if (e.Error != null)
        {
          if (e.Cancelled)
          {
            // TODO
          }
          else
          {
            mView.ShowError(e.Error.Message);
            if (output.CloseOnError)
              mView.Close();
          }
        }
        else if (e.Cancelled)
        {
          // TODO
        }
        else if (mRunWorkerCompleted != null)
        {
          mRunWorkerCompleted(output);
          next = output.NextOperation;
        }

        mRunWorkerCompleted = null;
        mView.SetIsWorking(false);

        if (next != null)
        {
          next();
        }
      }
      catch (Exception ex)
      {
        Exception error = ex.InnerException ?? ex;
        mView.ShowError(error.Message);
        mView.SetIsWorking(false);
      }
    }
  }
}