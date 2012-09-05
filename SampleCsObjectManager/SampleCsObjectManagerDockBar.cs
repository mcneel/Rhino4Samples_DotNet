using System;
using System.Windows.Forms;
using RMA.Rhino;
using RMA.UI;

namespace SampleCsObjectManager
{
  class SampleCsObjectManagerDockBar : MRhinoUiDockBar
  {
    public SampleCsObjectManagerDockBar()
      : base()
    {
    }

    public SampleCsObjectManagerDockBar(Guid id, string name, Control control)
      : base(id, name, control)
    {
    }

    static public Guid ID()
    {
      return new System.Guid("{861A51D7-12B7-44D0-B90E-2822FB043746}");
    }
  }
}
