namespace SampleCsDragDrop
{
  partial class SampleCsDragDropForm
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
      this.label1 = new System.Windows.Forms.Label();
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(14, 14);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(95, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Text string to drag:";
      // 
      // textBox1
      // 
      this.textBox1.Location = new System.Drawing.Point(115, 11);
      this.textBox1.Name = "textBox1";
      this.textBox1.Size = new System.Drawing.Size(225, 20);
      this.textBox1.TabIndex = 1;
      this.textBox1.Text = "Drag text";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(14, 44);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(324, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "Left mouse down and drag here to drag text from edit control above";
      this.label2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label2_MouseDown);
      // 
      // SampleCsDragDropForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(371, 84);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.textBox1);
      this.Controls.Add(this.label1);
      this.Name = "SampleCsDragDropForm";
      this.Text = "SampleCsDragDropForm";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.Label label2;
  }
}