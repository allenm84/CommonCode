using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace System.Common.References
{
  public class CurrencyBoxNativeWindow : NativeWindow
  {
    private Control control;
    private decimal lastValue = decimal.MinValue;
    private Func<Control, int> getSelectionStart;

    public decimal Value
    {
      get
      {
        var culture = new CultureInfo("en-US");
        decimal value;
        decimal.TryParse(control.Text, NumberStyles.Currency,
          culture, out value);
        return value;
      }
    }

    private event EventHandler mValueChanged = (o, e) => { };
    public event EventHandler ValueChanged
    {
      add { mValueChanged += value; }
      remove { mValueChanged -= value; }
    }

    public CurrencyBoxNativeWindow(Control control, Func<Control, int> getSelectionStart)
    {
      this.control = control;
      this.getSelectionStart = getSelectionStart;

      if (control.IsHandleCreated)
      {
        AssignHandle(control.Handle);
      }
      else
      {
        control.HandleCreated += control_HandleCreated;
      }

      control.Validated += control_Validated;
      control.KeyPress += control_KeyPress;
      control.Leave += control_Leave;
    }

    private void FireValueChanged()
    {
      decimal value = Value;
      if (value != lastValue)
      {
        mValueChanged(this, EventArgs.Empty);
        lastValue = value;
      }
    }

    private void control_HandleCreated(object sender, EventArgs e)
    {
      AssignHandle(control.Handle);
    }

    private void control_Validated(object sender, EventArgs e)
    {
      var value = decimal.Parse(control.Text, NumberStyles.Currency);
      control.Text = value.ToString("c2");
      FireValueChanged();
    }

    private void control_KeyPress(object sender, KeyPressEventArgs e)
    {
      bool isNumber = char.IsNumber(e.KeyChar);
      bool isControl = char.IsControl(e.KeyChar);
      bool isDecimal = e.KeyChar == '.' && control.Text.Count(c => c == '.') == 0;
      bool isCurrencySign = e.KeyChar == '$' && control.Text.Count(c => c == '&') == 0 && getSelectionStart(control) == 0;
      bool valid = isNumber || isDecimal || isControl || isCurrencySign;
      if (!valid)
      {
        e.Handled = true;
      }
      else
      {
        FireValueChanged();
      }
    }

    private void control_Leave(object sender, EventArgs e)
    {
      if (string.IsNullOrWhiteSpace(control.Text))
      {
        control.Text = "$0.00";
      }
    }
  }
}
