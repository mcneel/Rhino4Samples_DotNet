using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SampleCsDragDrop
{
  /// <summary>
  /// The SampleCsDragDropForm class
  /// </summary>
  public partial class SampleCsDragDropForm : Form
  {
    public SampleCsDragDropForm()
    {
      InitializeComponent();
    }

    private void label2_MouseDown(object sender, MouseEventArgs e)
    {
      // Start drag-and-drop if left mouse goes down over this label
      if (e.Button == MouseButtons.Left)
      {
        // Crete a new data object
        DataObject data = new DataObject();

        // Get the TextBox text
        string dragString = this.textBox1.Text;

        // If the TextBox text was empty then use a default string
        if (string.IsNullOrEmpty(dragString))
          dragString = "Empty string";

        // Set the DataOject's data to a new SampleCsDragData object which
        // can be serialized. If you use an object that does not support the
        // serialize interface then you will NOT be able to drag and drop the
        // item onto a different session of Rhino.
        data.SetData(new SampleCsDragData(dragString));

        // Start drag-and-drop
        this.DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Move);
      }
    }
  }
}