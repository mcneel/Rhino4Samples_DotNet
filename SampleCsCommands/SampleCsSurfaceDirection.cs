using RMA.Rhino;
using RMA.OpenNURBS;

namespace SampleCsCommands
{
  /// <summary>
  /// SampleCsSurfaceDirectionConduit
  /// </summary>
  public class SampleCsSurfaceDirectionConduit : MRhinoDisplayConduit
  {
    IOnBrep m_brep;
    bool m_bFlip;
    ArrayOn3dPoint m_points;
    ArrayOn3dVector m_normals;
    const int SURFACE_ARROW_COUNT = 5;

    public SampleCsSurfaceDirectionConduit()
      : base(new MSupportChannels(MSupportChannels.SC_DRAWOVERLAY), false)
    {
      m_brep = null;
      m_bFlip = false;
      m_points = new ArrayOn3dPoint();
      m_normals = new ArrayOn3dVector();
    }

    public void SetFlip(bool bFlip)
    {
      m_bFlip = bFlip;
    }

    public void SetBrep(IOnBrep brep)
    {
      if (null == brep || !brep.IsValid())
        return;

      m_brep = brep;

      int face_count = m_brep.m_F.Count();
      m_points.Reserve(face_count * SURFACE_ARROW_COUNT * SURFACE_ARROW_COUNT);
      m_normals.Reserve(face_count * SURFACE_ARROW_COUNT * SURFACE_ARROW_COUNT);

      m_points.SetCount(0);
      m_normals.SetCount(0);

      for (int i = 0; i < face_count; i++)
      {
        IOnBrepFace face = m_brep.m_F[i];
        IOnBrepLoop loop = face.OuterLoop();
        if (null == loop)
          continue;

        OnInterval udomain = face.Domain(0);
        OnInterval vdomain = face.Domain(1);

        if (loop.m_pbox.IsValid())
        {
          OnInterval domain = new OnInterval();
          domain.Set(loop.m_pbox.m_min.x, loop.m_pbox.m_max.x);
          domain.Intersection(udomain);
          if (domain.IsIncreasing())
            udomain.Set(domain.Min(), domain.Max());
          domain.Set(loop.m_pbox.m_min.y, loop.m_pbox.m_max.y);
          domain.Intersection(vdomain);
          if (domain.IsIncreasing())
            vdomain.Set(domain.Min(), domain.Max());
        }

        bool bUntrimmed = m_brep.FaceIsSurface(i);

        ArrayOnInterval intervals = new ArrayOnInterval();
        bool bRev = face.m_bRev;

        for (double u = 0.0; u < SURFACE_ARROW_COUNT; u += 1.0)
        {
          double d = u / (SURFACE_ARROW_COUNT - 1.0);
          double s = udomain.ParameterAt(d);

          intervals.SetCount(0);

          if (bUntrimmed || RhUtil.RhinoGetIsoIntervals(face, 1, s, intervals) > 0)
          {
            for (double v = 0.0; v < SURFACE_ARROW_COUNT; v += 1.0)
            {
              d = v / (SURFACE_ARROW_COUNT - 1.0);
              double t = vdomain.ParameterAt(d);

              bool bAdd = bUntrimmed;
              for (int k = 0; !bAdd && k < intervals.Count(); k++)
              {
                if (intervals[k].Includes(t))
                  bAdd = true;
              }

              if (bAdd)
              {
                On3dPoint pt = new On3dPoint();
                On3dVector du = new On3dVector();
                On3dVector dv = new On3dVector();
                On3dVector dir = new On3dVector();
                if (face.EvNormal(s, t, ref pt, ref du, ref dv, ref dir))
                {
                  m_points.Append(pt);
                  if (bRev)
                    dir.Reverse();
                  m_normals.Append(dir);
                }
              }
            }
          }
        }
      }
    }

    public override bool ExecConduit(ref MRhinoDisplayPipeline dp, uint nChannel, ref bool bTerminate)
    {
      if (nChannel == MSupportChannels.SC_DRAWOVERLAY)
      {
        if (null != m_brep)
        {
          MRhinoViewport vp = dp.GetRhinoVP();
          OnColor saved_color = vp.SetDrawColor(RhUtil.RhinoApp().AppSettings().TrackingColor());
          for (int i = 0; i < m_points.Count(); i++)
          {
            if (i % 100 == 0 && vp.InterruptDrawing())
              break;

            On3dPoint pt = m_points[i];
            On3dVector dir = new On3dVector(m_normals[i]);
            if (m_bFlip)
              dir.Reverse();

            vp.DrawDirectionArrow(pt, dir);
          }
          vp.SetDrawColor(saved_color);
        }
      }
      return true;
    }
  }

  ///////////////////////////////////////////////////////////////////////////
  ///////////////////////////////////////////////////////////////////////////

  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// </summary>
  public class SampleCsSurfaceDirection : RMA.Rhino.MRhinoCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{326b012f-066a-4a39-b5a7-f99562fcf345}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "SampleCsSurfaceDirection";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      MRhinoGetObject go = new MRhinoGetObject();
      go.SetCommandPrompt("Select surface or polysurface for direction display");
      go.SetGeometryFilter(IRhinoGetObject.GEOMETRY_TYPE_FILTER.surface_object | IRhinoGetObject.GEOMETRY_TYPE_FILTER.polysrf_object);
      go.GetObjects(1, 1);
      if (go.CommandResult() != IRhinoCommand.result.success)
        return go.CommandResult();

      MRhinoObjRef obj_ref = go.Object(0);
      IOnBrep brep = obj_ref.Brep();
      if (null == brep)
        return IRhinoCommand.result.failure;

      bool bIsSolid = brep.IsSolid();
      bool bFlip = false;

      SampleCsSurfaceDirectionConduit conduit = new SampleCsSurfaceDirectionConduit();
      conduit.SetBrep(brep);
      conduit.Enable();
      context.m_doc.Redraw();

      MRhinoGetOption gf = new MRhinoGetOption();
      gf.SetCommandPrompt("Press Enter when done");
      gf.AcceptNothing();
      if (!bIsSolid)
        gf.AddCommandOption(new MRhinoCommandOptionName("Flip"));

      for (; ; )
      {
        IRhinoGet.result res = gf.GetOption();

        if (res == IRhinoGet.result.option)
        {
          bFlip = !bFlip;
          conduit.SetFlip(bFlip);
          context.m_doc.Redraw();
          continue;
        }

        if (res == IRhinoGet.result.nothing)
        {
          if (!bIsSolid && bFlip)
          {
            OnBrep flipped_brep = new OnBrep(brep);
            flipped_brep.Flip();
            context.m_doc.ReplaceObject(obj_ref, flipped_brep);
          }
        }

        break;
      }

      conduit.Disable();
      context.m_doc.Redraw();

      return IRhinoCommand.result.success;
    }
  }
}

