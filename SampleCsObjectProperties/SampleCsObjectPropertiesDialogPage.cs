using System;
using System.Collections;
using System.Collections.Generic;
using RMA.Rhino;
using RMA.UI;

namespace SampleCsObjectProperties
{
  /// <summary>
  /// This is our custom MRhinoObjectPropertiesDialogPage-inherited class
  /// that is used to display our custom object properties.
  /// </summary>
  public class SampleCsObjectPropertiesDialogPage : MRhinoObjectPropertiesDialogPage
  {
    SampleCsObjectPropertiesUserControl _control;

    public SampleCsObjectPropertiesDialogPage()
      : base()
    {
      _control = new SampleCsObjectPropertiesUserControl();
    }

    public override bool AddPageToControlBar(IRhinoObject obj)
    {
      return _control.AddPageToControlBar(obj);
    }

    public override void InitControls(IRhinoObject new_obj)
    {
      _control.InitControls(new_obj);
    }

    public override IRhinoCommand.result RunScript(IRhinoObject[] objects)
    {
      return _control.RunScript(objects);
    }

    public override string EnglishPageTitle()
    {
      return "SampleCs Page";
    }

    public override System.Windows.Forms.Control GetPageControl()
    {
      return _control;
    }

    public override string LocalPageTitle()
    {
      return "SampleCs Page";
    }
  }
}
