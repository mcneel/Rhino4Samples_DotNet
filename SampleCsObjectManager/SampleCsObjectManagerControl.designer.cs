namespace SampleCsObjectManager
{
  partial class SampleCsObjectManagerControl
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.m_count_label = new System.Windows.Forms.Label();
      this.m_listbox = new System.Windows.Forms.ListBox();
      this.SuspendLayout();
      // 
      // m_count_label
      // 
      this.m_count_label.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.m_count_label.AutoSize = true;
      this.m_count_label.Location = new System.Drawing.Point(4, 4);
      this.m_count_label.Name = "m_count_label";
      this.m_count_label.Size = new System.Drawing.Size(93, 13);
      this.m_count_label.TabIndex = 0;
      this.m_count_label.Text = "0 objects selected";
      // 
      // m_listbox
      // 
      this.m_listbox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.m_listbox.FormattingEnabled = true;
      this.m_listbox.Location = new System.Drawing.Point(4, 21);
      this.m_listbox.Name = "m_listbox";
      this.m_listbox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
      this.m_listbox.Size = new System.Drawing.Size(143, 303);
      this.m_listbox.Sorted = true;
      this.m_listbox.TabIndex = 1;
      this.m_listbox.SelectedIndexChanged += new System.EventHandler(this.m_listbox_SelectedIndexChanged);
      // 
      // TestObjectManagerControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoSize = true;
      this.Controls.Add(this.m_listbox);
      this.Controls.Add(this.m_count_label);
      this.Name = "TestObjectManagerControl";
      this.Size = new System.Drawing.Size(150, 330);
      this.Load += new System.EventHandler(this.SampleCsObjectManagerControl_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label m_count_label;
    private System.Windows.Forms.ListBox m_listbox;
  }
}
