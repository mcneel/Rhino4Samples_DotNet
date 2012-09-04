using System;
using RMA.Rhino;
using RMA.OpenNURBS;

namespace SampleCsCommands
{
  /// <summary>
  /// SampleCsDrawArrowheadConduit conduit
  /// </summary>
  public class SampleCsDrawArrowheadConduit : MRhinoDisplayConduit
  {
    const double m_default_arrow_size = 1.0;
    bool m_bDraw;
    OnLine m_line;
    OnPlane m_plane;
    On3dPointArray m_arrowhead;

    /// <summary>
    /// Public constructor
    /// </summary>
    public SampleCsDrawArrowheadConduit(OnPlane plane, OnLine line, double scale)
      : base(new MSupportChannels(MSupportChannels.SC_CALCBOUNDINGBOX | MSupportChannels.SC_DRAWOVERLAY), false)
    {
      m_bDraw = false;
      m_plane = plane;
      m_line = line;
      m_arrowhead = new On3dPointArray();

      double x = 0, y = 0;

      m_plane.ClosestPointTo(line.from, ref x, ref y);
      On2dPoint from = new On2dPoint(x, y);

      m_plane.ClosestPointTo(line.to, ref x, ref y);
      On2dPoint to = new On2dPoint(x, y);

      On2dVector dir = new On2dVector(from - to);
      dir.Unitize();

      m_bDraw = GetArrowHead(dir, from, scale, ref m_arrowhead);
    }

    /// <summary>
    /// Virtual MRhinoDisplayConduit.ExecConduit override
    /// </summary>
    public override bool ExecConduit(ref MRhinoDisplayPipeline dp, uint nChannel, ref bool bTerminate)
    {
      if (nChannel == MSupportChannels.SC_CALCBOUNDINGBOX)
      {
        m_pChannelAttrs.m_BoundingBox.Union(m_line.BoundingBox());
      }
      else if (nChannel == MSupportChannels.SC_DRAWOVERLAY)
      {
        dp.DrawLine(m_line.from, m_line.to, m_pDisplayAttrs.m_ObjectColor | 0xFF000000, m_pDisplayAttrs.m_nLineThickness);
        if (m_bDraw)
          dp.DrawPolygon(m_arrowhead, dp.DisplayAttrs().m_ObjectColor | 0xFF000000, true);
      }
      return true;
    }

    bool GetArrowHead(On2dVector dir, On2dPoint tip, double scale, ref On3dPointArray triangle)
    {
      double arrow_size = m_default_arrow_size * scale;

      On2dPointArray corners = new On2dPointArray();
      corners.Reserve(3);
      corners.SetCount(3);

      On2dVector up = new On2dVector(-dir.y, dir.x);
      corners[0].Set(tip.x, tip.y);
      corners[1].x = tip.x + arrow_size * (0.25 * up.x - dir.x);
      corners[1].y = tip.y + arrow_size * (0.25 * up.y - dir.y);
      corners[2].x = corners[1].x - 0.5 * arrow_size * up.x;
      corners[2].y = corners[1].y - 0.5 * arrow_size * up.y;

      triangle.Reserve(corners.Count());
      triangle.SetCount(corners.Count());

      for (int i = 0; i < corners.Count(); i++)
        triangle[i] = m_plane.PointAt(corners[i].x, corners[i].y);

      return true;
    }
  }

  ///////////////////////////////////////////////////////////////////////////
  ///////////////////////////////////////////////////////////////////////////

  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// </summary>
  public class SampleCsDrawArrowhead : RMA.Rhino.MRhinoCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{15c230f9-a250-486d-9b46-60976b78206c}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "SampleCsDrawArrowhead";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      OnLine line = new OnLine();
      OnPlane plane = new OnPlane();

      MRhinoGetPoint gp = new MRhinoGetPoint();
      gp.SetCommandPrompt("Start of line");
      gp.GetPoint();
      if (gp.CommandResult() != IRhinoCommand.result.success)
        return gp.CommandResult();

      line.from = gp.Point();
      plane = new OnPlane(RhUtil.RhinoActiveCPlane());
      plane.SetOrigin(line.from);

      gp.SetCommandPrompt("End of line");
      gp.Constrain(plane);
      gp.SetBasePoint(line.from);
      gp.DrawLineFromPoint(line.from, true);
      gp.GetPoint();
      if (gp.CommandResult() != IRhinoCommand.result.success)
        return gp.CommandResult();

      line.to = plane.ClosestPointTo(gp.Point());
      if (!line.IsValid())
        return IRhinoCommand.result.nothing;

      SampleCsDrawArrowheadConduit conduit = new SampleCsDrawArrowheadConduit(plane, line, 1.0);
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

