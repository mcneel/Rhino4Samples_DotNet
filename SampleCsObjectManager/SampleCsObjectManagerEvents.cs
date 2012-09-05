using System;
using System.Collections.Generic;
using System.Text;
using RMA.Rhino;
using RMA.UI;

namespace SampleCsObjectManager
{
  class SampleCsObjectManagerEvents : MRhinoEventWatcher
  {
    private SampleCsObjectManagerControl m_control;

    public SampleCsObjectManagerEvents(SampleCsObjectManagerControl control)
    {
      m_control = control;
    }

    public override void OnCloseDocument(ref MRhinoDoc doc)
    {
      if (true == m_control.Visible)
        m_control.OnCloseDocument(ref doc);
    }

    public override void OnBeginCommand(IRhinoCommand command, IRhinoCommandContext context)
    {
      if (true == m_control.Visible)
        m_control.OnBeginCommand(command, context);
    }

    public override void OnEndCommand(IRhinoCommand command, IRhinoCommandContext context, IRhinoCommand.result rc)
    {
      if (true == m_control.Visible)
        m_control.OnEndCommand(command, context, rc);
    }

    public override void OnAddObject(ref MRhinoDoc doc, ref MRhinoObject @object)
    {
      if (true == m_control.Visible)
        m_control.OnAddObject(ref doc, ref @object);
    }

    public override void OnDeleteObject(ref MRhinoDoc doc, ref MRhinoObject @object)
    {
      if (true == m_control.Visible)
        m_control.OnDeleteObject(ref doc, ref @object);
    }
    
    public override void OnSelectObject(ref MRhinoDoc doc, IRhinoObject @object)
    {
      if (true == m_control.Visible)
        m_control.OnSelectObject(ref doc, @object);
    }

    public override void OnSelectObjects(ref MRhinoDoc doc, IRhinoObject[] objects)
    {
      if (true == m_control.Visible)
        m_control.OnSelectObjects(ref doc, objects);
    }

    public override void OnDeselectAllObjects(ref MRhinoDoc doc, int count)
    {
      if (true == m_control.Visible)
        m_control.OnDeselectAllObjects(ref doc, count);
    }

    public override void OnDeselectObject(ref MRhinoDoc doc, IRhinoObject @object)
    {
      if (true == m_control.Visible)
        m_control.OnDeselectObject(ref doc, @object);
    }

    public override void OnDeselectObjects(ref MRhinoDoc doc, IRhinoObject[] objects)
    {
      if (true == m_control.Visible)
        m_control.OnDeselectObjects(ref doc, objects);
    }
  }
}
