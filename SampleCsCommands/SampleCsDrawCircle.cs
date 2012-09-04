using RMA.Rhino;
using RMA.OpenNURBS;

namespace SampleCsCommands
{
  public class SampleCsDrawCircleConduit : MRhinoDisplayConduit
  {
    OnCircle m_circle;

    public SampleCsDrawCircleConduit(OnCircle circle)
      : base(new MSupportChannels(MSupportChannels.SC_CALCBOUNDINGBOX | MSupportChannels.SC_DRAWOVERLAY), false)
    {
      m_circle = circle;
    }

    public override bool ExecConduit(ref MRhinoDisplayPipeline dp, uint nChannel, ref bool bTerminate)
    {
      if (nChannel == MSupportChannels.SC_CALCBOUNDINGBOX)
      {
        MRhinoViewport vp = dp.GetRhinoVP();
        OnCircle circle = new OnCircle();
        if (CalculateCircle(vp, ref circle))
          m_pChannelAttrs.m_BoundingBox.Union(circle.BoundingBox());
      }
      else if (nChannel == MSupportChannels.SC_DRAWOVERLAY)
      {
        MRhinoViewport vp = dp.GetRhinoVP();
        OnCircle circle = new OnCircle();
        if (CalculateCircle(vp, ref circle))
          vp.DrawCircle(circle);
      }
      return true;
    }

    bool CalculateCircle(MRhinoViewport vp, ref OnCircle circle)
    {
      if (null != vp)
      {
        double pixels_per_unit = 1.0;
        if (vp.m_v.m_vp.GetWorldToScreenScale(m_circle.Center(), ref pixels_per_unit))
        {
          double units_per_pixel = 1.0 / pixels_per_unit;
          circle.Create(m_circle.plane, m_circle.radius / pixels_per_unit);
          return circle.IsValid();
        }
      }
      return false;
    }
  }

  ///////////////////////////////////////////////////////////////////////////
  ///////////////////////////////////////////////////////////////////////////

  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// </summary>
  public class SampleCsDrawCircle : RMA.Rhino.MRhinoCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{abb1e4ec-e3ce-4af9-9763-2c1760cb3049}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "CampleCsDrawCircle";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      MArgsRhinoGetCircle args = new MArgsRhinoGetCircle();
      OnCircle circle = new OnCircle();
      IRhinoCommand.result rc = RhUtil.RhinoGetCircle(args, ref circle);
      if (rc != IRhinoCommand.result.success)
        return rc;

      SampleCsDrawCircleConduit conduit = new SampleCsDrawCircleConduit(circle);
      conduit.Enable();
      context.m_doc.Redraw();

      MRhinoGetString gs = new MRhinoGetString();
      gs.SetCommandPrompt("Press <Enter> to continue");
      gs.AcceptNothing();
      gs.GetString();

      conduit.Disable();
      context.m_doc.Redraw();

      return IRhinoCommand.result.success;
    }
  }
}

