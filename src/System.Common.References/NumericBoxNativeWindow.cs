using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace System.Common.References
{
  public class NumericBoxNativeWindow : NativeWindow
  {
    private Control control;
    private decimal lastValue = decimal.MinValue;

    public decimal Value
    {
      get
      {
        var culture = new CultureInfo("en-US");
        decimal value;
        decimal.TryParse(control.Text, NumberStyles.Number,
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

    public NumericBoxNativeWindow(Control control)
    {
      this.control = control;

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
      var value = decimal.Parse(control.Text, NumberStyles.Number);
      control.Text = value.ToString("n2");
      FireValueChanged();
    }

    private void control_KeyPress(object sender, KeyPressEventArgs e)
    {
      bool isNumber = char.IsNumber(e.KeyChar);
      bool isControl = char.IsControl(e.KeyChar);
      bool isDecimal = e.KeyChar == '.' && control.Text.Count(c => c == '.') == 0;
      bool valid = isNumber || isDecimal || isControl;
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
        control.Text = "0.00";
      }
    }
  }
}
