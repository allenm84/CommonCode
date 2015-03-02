using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace System.Common.References
{
  public partial class JanusForm : Form
  {
    public JanusForm()
    {
      InitializeComponent();
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      visualStyleManager.AddControl(this, true);
    }
  }
}
