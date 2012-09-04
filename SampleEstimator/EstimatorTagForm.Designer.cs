namespace Estimator
{
  partial class EstimatorTagForm
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
      this.m_prompt = new System.Windows.Forms.Label();
      this.m_list = new System.Windows.Forms.ListView();
      this.m_ok = new System.Windows.Forms.Button();
      this.m_cancel = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // m_prompt
      // 
      this.m_prompt.AutoSize = true;
      this.m_prompt.Location = new System.Drawing.Point(13, 13);
      this.m_prompt.Name = "m_prompt";
      this.m_prompt.Size = new System.Drawing.Size(35, 13);
      this.m_prompt.TabIndex = 0;
      this.m_prompt.Text = "label1";
      // 
      // m_list
      // 
      this.m_list.FullRowSelect = true;
      this.m_list.Location = new System.Drawing.Point(16, 30);
      this.m_list.Name = "m_list";
      this.m_list.Size = new System.Drawing.Size(256, 196);
      this.m_list.TabIndex = 1;
      this.m_list.UseCompatibleStateImageBehavior = false;
      this.m_list.View = System.Windows.Forms.View.Details;
      this.m_list.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.m_list_ColumnClick);
      // 
      // m_ok
      // 
      this.m_ok.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.m_ok.Location = new System.Drawing.Point(57, 232);
      this.m_ok.Name = "m_ok";
      this.m_ok.Size = new System.Drawing.Size(75, 23);
      this.m_ok.TabIndex = 2;
      this.m_ok.Text = "OK";
      this.m_ok.UseVisualStyleBackColor = true;
      this.m_ok.Click += new System.EventHandler(this.m_ok_Click);
      // 
      // m_cancel
      // 
      this.m_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.m_cancel.Location = new System.Drawing.Point(138, 232);
      this.m_cancel.Name = "m_cancel";
      this.m_cancel.Size = new System.Drawing.Size(75, 23);
      this.m_cancel.TabIndex = 3;
      this.m_cancel.Text = "Cancel";
      this.m_cancel.UseVisualStyleBackColor = true;
      // 
      // EstimatorTagForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(284, 262);
      this.ControlBox = false;
      this.Controls.Add(this.m_cancel);
      this.Controls.Add(this.m_ok);
      this.Controls.Add(this.m_list);
      this.Controls.Add(this.m_prompt);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "EstimatorTagForm";
      this.Text = "Apply Tag";
      this.Load += new System.EventHandler(this.EstimatorTagForm_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label m_prompt;
    private System.Windows.Forms.ListView m_list;
    private System.Windows.Forms.Button m_ok;
    private System.Windows.Forms.Button m_cancel;
  }
}