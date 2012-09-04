using System;
using System.Windows.Forms;
using RMA.Rhino;
using RMA.UI;

namespace SampleDockingDialog
{
  class SampleDockingDialogDockBar : MRhinoUiDockBar
  {
    /// <summary>
    /// Public constructor
    /// </summary>
    public SampleDockingDialogDockBar()
      : base()
    {
    }

    /// <summary>
    /// Public constructor
    /// </summary>
    public SampleDockingDialogDockBar(Guid id, string name, Control control)
      : base(id, name, control)
    {
    }

    /// <summary>
    /// Dockbar Id
    /// </summary>
    static public Guid ID()
    {
      return new System.Guid("{3D089561-495B-4071-A301-A4BF8C580F4C}");
    }

  }
}
