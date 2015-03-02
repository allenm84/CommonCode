namespace System.Common.References
{
  partial class AppThemePicker
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
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.radLabel1 = new Telerik.WinControls.UI.RadLabel();
      this.cboThemes = new Telerik.WinControls.UI.RadDropDownList();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.btnOK = new Telerik.WinControls.UI.RadButton();
      this.btnCancel = new Telerik.WinControls.UI.RadButton();
      this.radLabel2 = new Telerik.WinControls.UI.RadLabel();
      this.tableLayoutPanel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.cboThemes)).BeginInit();
      this.tableLayoutPanel2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.btnOK)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 2;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.radLabel1, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.cboThemes, 1, 0);
      this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 2);
      this.tableLayoutPanel1.Controls.Add(this.radLabel2, 0, 1);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 3;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(292, 102);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // radLabel1
      // 
      this.radLabel1.Anchor = System.Windows.Forms.AnchorStyles.Left;
      this.radLabel1.Location = new System.Drawing.Point(3, 12);
      this.radLabel1.Name = "radLabel1";
      this.radLabel1.Size = new System.Drawing.Size(47, 18);
      this.radLabel1.TabIndex = 0;
      this.radLabel1.Text = "Themes:";
      // 
      // cboThemes
      // 
      this.cboThemes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
      this.cboThemes.DropDownAnimationEnabled = true;
      this.cboThemes.DropDownSizingMode = Telerik.WinControls.UI.SizingMode.UpDownAndRightBottom;
      this.cboThemes.DropDownStyle = Telerik.WinControls.RadDropDownStyle.DropDownList;
      this.cboThemes.Location = new System.Drawing.Point(56, 11);
      this.cboThemes.MaxDropDownItems = 0;
      this.cboThemes.Name = "cboThemes";
      this.cboThemes.ShowImageInEditorArea = true;
      this.cboThemes.Size = new System.Drawing.Size(233, 20);
      this.cboThemes.TabIndex = 1;
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 3;
      this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanel2, 2);
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
      this.tableLayoutPanel2.Controls.Add(this.btnOK, 1, 0);
      this.tableLayoutPanel2.Controls.Add(this.btnCancel, 2, 0);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 72);
      this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 1;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Size = new System.Drawing.Size(292, 30);
      this.tableLayoutPanel2.TabIndex = 2;
      // 
      // btnOK
      // 
      this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.btnOK.Location = new System.Drawing.Point(135, 3);
      this.btnOK.Name = "btnOK";
      this.btnOK.Size = new System.Drawing.Size(74, 24);
      this.btnOK.TabIndex = 0;
      this.btnOK.Text = "OK";
      // 
      // btnCancel
      // 
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.Location = new System.Drawing.Point(215, 3);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(74, 24);
      this.btnCancel.TabIndex = 1;
      this.btnCancel.Text = "Cancel";
      // 
      // radLabel2
      // 
      this.radLabel2.AutoSize = false;
      this.tableLayoutPanel1.SetColumnSpan(this.radLabel2, 2);
      this.radLabel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.radLabel2.Location = new System.Drawing.Point(3, 45);
      this.radLabel2.Name = "radLabel2";
      this.radLabel2.Size = new System.Drawing.Size(286, 24);
      this.radLabel2.TabIndex = 3;
      this.radLabel2.Text = "NOTE: The theme will change once you click \'OK\'";
      this.radLabel2.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // AppThemePicker
      // 
      this.AcceptButton = this.btnOK;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnCancel;
      this.ClientSize = new System.Drawing.Size(292, 102);
      this.Controls.Add(this.tableLayoutPanel1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "AppThemePicker";
      // 
      // 
      // 
      this.RootElement.ApplyShapeToControl = true;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "";
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.radLabel1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.cboThemes)).EndInit();
      this.tableLayoutPanel2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.btnOK)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.btnCancel)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.radLabel2)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private Telerik.WinControls.UI.RadLabel radLabel1;
    private Telerik.WinControls.UI.RadDropDownList cboThemes;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private Telerik.WinControls.UI.RadButton btnOK;
    private Telerik.WinControls.UI.RadButton btnCancel;
    private Telerik.WinControls.UI.RadLabel radLabel2;
  }
}