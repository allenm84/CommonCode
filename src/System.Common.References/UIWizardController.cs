using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace System.Common.References
{
  public interface IUIWizardView
  {
    UITab TabControl { get; }
    bool BackEnabled { get; set; }
    string NextText { get; set; }
    bool Canceled { get; }

    void UpdateTitle(int index);
    bool BeforePageChange(int current, int desired);
    bool ConfirmCancel();
  }

  public class UIWizardController
  {
    private IUIWizardView mView;
    private bool cancelClose = false;

    public UIWizardController(IUIWizardView view)
    {
      mView = view;
      mView.TabControl.ShowTabs = false;
      mView.TabControl.PageBorder = PageBorder.None;
      mView.TabControl.ShowFocusRectangle = false;
      mView.TabControl.SelectedTabChanged += new TabEventHandler(TabControl_SelectedTabChanged);
    }

    private void UpdateWizardView()
    {
      var tabWizard = mView.TabControl;
      int index = tabWizard.SelectedIndex;
      mView.NextText = index < (tabWizard.TabPages.Count - 1) ? "Next >" : "Finish";
      mView.BackEnabled = index > 0;
      mView.UpdateTitle(index);
    }

    private void TabControl_SelectedTabChanged(object sender, TabEventArgs e)
    {
      UpdateWizardView();
    }

    public void OnLoad()
    {
      UpdateWizardView();
    }

    public void OnBackClick()
    {
      var tabWizard = mView.TabControl;
      if (mView.BeforePageChange(tabWizard.SelectedIndex, tabWizard.SelectedIndex - 1))
      {
        tabWizard.SelectedIndex -= 1;
      }
    }

    public void OnNextClick()
    {
      var tabWizard = mView.TabControl;

      bool isLast = tabWizard.SelectedIndex == tabWizard.TabPages.Count - 1;
      if (!isLast)
      {
        cancelClose = true;
        if (mView.BeforePageChange(tabWizard.SelectedIndex, tabWizard.SelectedIndex + 1))
        {
          tabWizard.SelectedIndex += 1;
        }
      }
    }

    public void OnCancelClick()
    {
      cancelClose = !mView.ConfirmCancel();
    }

    public void OnClosing(FormClosingEventArgs e)
    {
      if (cancelClose)
      {
        e.Cancel = true;
      }
      else if (e.CloseReason == CloseReason.UserClosing && mView.Canceled)
      {
        e.Cancel = !mView.ConfirmCancel();
      }

      cancelClose = false;
    }
  }
}