namespace System.Common.References
{
  partial class JanusForm
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      Janus.Windows.Common.JanusColorScheme janusColorScheme2 = new Janus.Windows.Common.JanusColorScheme();
      this.visualStyleManager = new Janus.Windows.Common.VisualStyleManager(this.components);
      this.SuspendLayout();
      // 
      // visualStyleManager
      // 
      janusColorScheme2.HighlightTextColor = System.Drawing.SystemColors.HighlightText;
      janusColorScheme2.Name = "Office2007Silver";
      janusColorScheme2.Office2007ColorScheme = Janus.Windows.Common.Office2007ColorScheme.Silver;
      janusColorScheme2.Office2007CustomColor = System.Drawing.Color.Empty;
      janusColorScheme2.VisualStyle = Janus.Windows.Common.VisualStyle.Office2007;
      this.visualStyleManager.ColorSchemes.Add(janusColorScheme2);
      // 
      // JanusForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(284, 262);
      this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Name = "JanusForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "JanusForm";
      this.ResumeLayout(false);

    }

    #endregion

    protected Janus.Windows.Common.VisualStyleManager visualStyleManager;

  }
}