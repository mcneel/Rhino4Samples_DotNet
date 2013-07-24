using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using RMA.Rhino;
using RMA.OpenNURBS;

namespace SampleCsModelessForm
{
  public partial class SampleCsForm : Form
  {
    public SampleCsForm()
    {
      InitializeComponent();

      CenterToParent();
    }

    private void button1_Click(object sender, EventArgs e)
    {
      // Since our form is modeless, the user can run commands
      // while our form is visible, including the Open command, 
      // which closes the current document and opens a new one.
      // Thus, it's best to always ask for the active document
      // instead of maintaining a reference to the document passed
      // to the command that created this form.
      MRhinoDoc doc = RhUtil.RhinoApp().ActiveDoc();
      if (null != doc)
      {
        On3dPoint a = new On3dPoint(0.0, 0.0, 0.0);
        On3dPoint b = new On3dPoint(5.0, 5.0, 5.0);
        OnLine line = new OnLine(a, b);
        doc.AddCurveObject(line);
        doc.Redraw();
      }
    }
  }
}