using System;
using RMA.Rhino;
using RMA.OpenNURBS;

namespace SampleCsCommands
{
  /// <summary>
  /// Abstract class used to show objects transforming dynamically
  /// </summary>
  public abstract class RhinoGetTransform : RMA.Rhino.MRhinoGetPoint
  {
    protected MRhinoXformObjectList m_list;
    protected OnXform m_xform;
    protected bool m_bHaveXform;
    protected On3dPoint m_base;

    public RhinoGetTransform() : base()
    {
      m_list = new MRhinoXformObjectList();
      m_xform = new OnXform();
      m_xform.Identity();
      m_bHaveXform = false;
      m_base = new On3dPoint();
      m_base.Set(0,0,0);
    }
    
    /////////////////////////////////////////////////////////////////////////
    // STEP 1: Use MRhinoGet member functions to specify command prompt

    /////////////////////////////////////////////////////////////////////////
    // STEP 2: Use MRhinoGet member functions to specify what types of
    //         alternate input can be accepted.

    /////////////////////////////////////////////////////////////////////////
    // STEP 3: Add any objects you want transformed and grips you want
    //         transformed.  Make sure no duplicates are in the list
    //         and that no grip owners are passed in as objects.
    public void AppendObjects(IRhinoGetObject go)
    {
      m_list.AddObjects(go, true);
    }

    public void AppendObjects(IRhinoXformObjectList list)
    {
      m_list = (MRhinoXformObjectList)list;
    }

    public void AppendObject(IRhinoObject obj)
    {
      m_list.AddObject(obj);
    }

    /////////////////////////////////////////////////////////////////////////
    // STEP 4: Override this abstract function to provide your own custom
    //         transformation method. Call this function to retrieve
    //         the final transformation.
    public abstract bool CalculateTransform(MRhinoViewport vp, IOn3dPoint point, ref OnXform xform);

    /////////////////////////////////////////////////////////////////////////
    // STEP 5: Override this virtual function to provide your own custom
    //         method get a transform object.
    public virtual IRhinoGet.result GetXform()
    {
      IRhinoGet.result res = base.GetPoint();
      if (res == IRhinoGet.result.cancel)
      {
        for (int i = 0; i < m_list.m_grip_owners.Length; i++)
        {
          MRhinoObjectGrips grips = m_list.m_grip_owners[i].m_grips;
          if (grips != null)
            grips.Reset();
        }
      }
      return res;
    }

    /////////////////////////////////////////////////////////////////////////
    // Overridden MRhinoGetPoint::SetBasePoint
    public new void SetBasePoint(IOn3dPoint base_point)
    {
      m_base = (On3dPoint)base_point;
      base.SetBasePoint(base_point);
    }
    
    /////////////////////////////////////////////////////////////////////////
    // Overridden MRhinoGetPoint::OnMouseMove
    public override void OnMouseMove(MRhinoViewport vp, uint flags, IOn3dPoint pt, System.Drawing.Point view_wnd_point)
    {
      m_bHaveXform = CalculateTransform(vp, pt, ref m_xform);
      if (!m_bHaveXform)
        m_xform.Identity();

      for (int i = 0; i < m_list.m_grips.Length; i++)
        m_list.m_grips[i].MoveGrip(m_xform);

      base.OnMouseMove(vp, flags, pt, view_wnd_point);
    }

    /////////////////////////////////////////////////////////////////////////
    // Overridden MRhinoGetPoint::DynamicDraw
    public override void DynamicDraw(IntPtr hdc, MRhinoViewport vp, IOn3dPoint pt)
    {
      MRhinoDisplayPipeline dp = vp.DisplayPipeline();
      if (dp != null)
      {
        dp.PushObjectColor(0);
        dp.DrawObjects(m_list.m_objects, m_xform);
        dp.DrawObjects(m_list.m_grip_owners);
        dp.PopObjectColor();
        base.DynamicDraw(hdc, vp, pt);
      }
    }

    /////////////////////////////////////////////////////////////////////////
    // Returns the object list
    public MRhinoXformObjectList ObjectList()
    {
      return m_list;
    }
  }

  ///////////////////////////////////////////////////////////////////////////
  ///////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Translation transformation
  /// </summary>
  class RhinoGetTranslation : RhinoGetTransform
  {
    public override bool CalculateTransform(MRhinoViewport vp, IOn3dPoint point, ref OnXform xform)
    {
      bool bResult = false;
      On3dVector v = new On3dVector();
      On3dPoint pt = new On3dPoint(point);
      v = pt - m_base;
      if (!v.IsZero())
      {
        xform.Translation(v);
        bResult = xform.IsValid();
      }
      return bResult;
    }
  }

  ///////////////////////////////////////////////////////////////////////////
  ///////////////////////////////////////////////////////////////////////////

  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// </summary>
  public class SampleCsDynamicTransform : RMA.Rhino.MRhinoCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{69dd8cf8-7ac3-4a05-a50f-de231e5ca1d4}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "SampleCsDynamicTransform";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      MRhinoGetObject go = new MRhinoGetObject();
      go.SetCommandPrompt("Select objects to move");
      go.GetObjects(1, 0);
      if (go.CommandResult() != IRhinoCommand.result.success)
        return go.CommandResult();

      MRhinoXformObjectList list = new MRhinoXformObjectList();
      if (list.AddObjects(go, true) < 1)
        return IRhinoCommand.result.failure;

      MRhinoGetPoint gp = new MRhinoGetPoint();
      gp.SetCommandPrompt("Point to move from");
      gp.GetPoint();
      if (gp.CommandResult() != IRhinoCommand.result.success)
        return gp.CommandResult();

      RhinoGetTranslation gt = new RhinoGetTranslation();
      gt.SetCommandPrompt("Point to move to");
      gt.AppendObjects(list);
      gt.SetBasePoint(gp.Point());
      gt.DrawLineFromPoint(gp.Point(), true);
      gt.GetXform();
      if (gt.CommandResult() != IRhinoCommand.result.success)
        return gt.CommandResult();

      OnXform xform = new OnXform();
      if (gt.CalculateTransform(gt.View().ActiveViewport(), gt.Point(), ref xform))
      {
        // TODO: do object transformation here.
      }

      return IRhinoCommand.result.success;
    }
  }
}

