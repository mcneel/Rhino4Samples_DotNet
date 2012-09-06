using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using RMA.Rhino;
using RMA.UI;

namespace SampleCsObjectProperties
{
  public partial class SampleCsObjectPropertiesUserControl : UserControl
  {
    public SampleCsObjectPropertiesUserControl()
    {
      InitializeComponent();
    }

    private void btnHello_Click(object sender, EventArgs e)
    {
      MessageBox.Show("Hello Rhino!");
    }

    // Called by SampleCsObjectPropertiesDialogPage
    public void InitControls(IRhinoObject new_obj)
    {
      // TODO: add control initialization here
    }

    // Called by SampleCsObjectPropertiesDialogPage
    public bool AddPageToControlBar(IRhinoObject obj)
    {
      // TODO: test to see if this object qualifies this page
      // to be shown. For this example, we will always display
      // our dialog page.
      return true;
    }

    // Called by SampleCsObjectPropertiesDialogPage
    public IRhinoCommand.result RunScript(IRhinoObject[] objects)
    {
      // TODO: add command line, script handling, code here
      return IRhinoCommand.result.success;
    }
  }
}