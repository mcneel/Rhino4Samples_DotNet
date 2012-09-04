using RMA.Rhino;
using RMA.OpenNURBS;

namespace SampleCsCommands
{
  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// </summary>
  public class SampleCsBrepBox : RMA.Rhino.MRhinoCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{8555a03f-3158-4d26-9542-ffc5524839d4}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "SampleCsBrepBox";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      IRhinoCommand.result rc = IRhinoCommand.result.failure;

      OnBrep brep = MakeBox();
      if (null != brep && brep.IsValid())
      {
        context.m_doc.AddBrepObject(brep);
        context.m_doc.Redraw();
        rc = IRhinoCommand.result.success;
      }

      return rc;
    }

    /// <summary>
    /// MakeBox helper class
    /// </summary>
    class OnBrepBoxFaceInfo
    {
      public int[] e;
      public bool[] bRev;

      public OnBrepBoxFaceInfo(int e0, int e1, int e2, int e3, bool b0, bool b1, bool b2, bool b3)
      {
        e = new int[4];
        e[0] = e0;
        e[1] = e1;
        e[2] = e2;
        e[3] = e3;

        bRev = new bool[4];
        bRev[0] = b0;
        bRev[1] = b1;
        bRev[2] = b2;
        bRev[3] = b3;
      }
    }

    /// <summary>
    /// The one and only MakeBox
    /// </summary>
    static OnBrep MakeBox()
    {
      /*
      This example demonstrates how to construct a OnBrep
      with the topology shown below.

      v7_______e6_____v6
       |\             |\
       | e7           | e5
       |  \ ______e4_____\ 
      e11  v4         |   v5
       |   |        e10   |
       |   |          |   |
      v3---|---e2----v2   e9
       \   e8         \   |
        e3 |           e1 |
         \ |            \ |
          \v0_____e0_____\v1

      */

      On3dPoint[] points = new On3dPoint[8];
      points[0] = new On3dPoint(0.0, 0.0, 0.0);
      points[1] = new On3dPoint(10.0, 0.0, 0.0);
      points[2] = new On3dPoint(10.0, 10.0, 0.0);
      points[3] = new On3dPoint(0.0, 10.0, 0.0);
      points[4] = new On3dPoint(0.0, 0.0, 10.0);
      points[5] = new On3dPoint(10.0, 0.0, 10.0);
      points[6] = new On3dPoint(10.0, 10.0, 10.0);
      points[7] = new On3dPoint(0.0, 10.0, 10.0);

      OnBrep brep = new OnBrep();

      int vi = 0, ei = 0, fi = 0, si = 0, c2i = 0;

      for (vi = 0; vi < 8; vi++)
        brep.NewVertex(points[vi], 0.0);

      for (ei = 0; ei < 4; ei++)
      {
        OnBrepVertex v0 = brep.m_V[ei];
        OnBrepVertex v1 = brep.m_V[(ei + 1) % 4];
        brep.m_C3.Append(new OnLineCurve(v0.point, v1.point));
        brep.NewEdge(ref v0, ref v1, ei, null, 0.0);
      }
      for (ei = 4; ei < 8; ei++)
      {
        OnBrepVertex v0 = brep.m_V[ei];
        OnBrepVertex v1 = brep.m_V[ei == 7 ? 4 : (ei + 1)];
        brep.m_C3.Append(new OnLineCurve(v0.point, v1.point));
        brep.NewEdge(ref v0, ref v1, ei, null, 0.0);
      }
      for (ei = 8; ei < 12; ei++)
      {
        OnBrepVertex v0 = brep.m_V[ei - 8];
        OnBrepVertex v1 = brep.m_V[ei - 4];
        brep.m_C3.Append(new OnLineCurve(v0.point, v1.point));
        brep.NewEdge(ref v0, ref v1, ei, null, 0.0);
      }

      OnBrepBoxFaceInfo[] f = new OnBrepBoxFaceInfo[6];
      f[0] = new OnBrepBoxFaceInfo(0, 9, 4, 8, false, false, true, true);
      f[1] = new OnBrepBoxFaceInfo(1, 10, 5, 9, false, false, true, true);
      f[2] = new OnBrepBoxFaceInfo(2, 11, 6, 10, false, false, true, true);
      f[3] = new OnBrepBoxFaceInfo(3, 8, 7, 11, false, false, true, true);
      f[4] = new OnBrepBoxFaceInfo(3, 2, 1, 0, true, true, true, true);
      f[5] = new OnBrepBoxFaceInfo(4, 5, 6, 7, false, false, false, false);

      for (fi = 0; fi < 6; fi++ )
      {
        OnBrepEdge e0 = brep.m_E[f[fi].e[0]];
        OnBrepEdge e1 = brep.m_E[f[fi].e[1]];
        OnBrepEdge e2 = brep.m_E[f[fi].e[2]];
        OnBrepEdge e3 = brep.m_E[f[fi].e[3]];
        OnBrepVertex v0 = brep.m_V[e0.get_m_vi(f[fi].bRev[0] ? 1 : 0)];
        OnBrepVertex v1 = brep.m_V[e1.get_m_vi(f[fi].bRev[1] ? 1 : 0)];
        OnBrepVertex v2 = brep.m_V[e2.get_m_vi(f[fi].bRev[2] ? 1 : 0)];
        OnBrepVertex v3 = brep.m_V[e3.get_m_vi(f[fi].bRev[3] ? 1 : 0)];

        si = brep.AddSurface(OnUtil.ON_NurbsSurfaceQuadrilateral(v0.point, v1.point, v2.point, v3.point));
        OnInterval s = brep.m_S[si].Domain(0);
        OnInterval t = brep.m_S[si].Domain(1);
        On2dPoint p0 = new On2dPoint(s[0], t[0]);
        On2dPoint p1 = new On2dPoint(s[1], t[0]);
        On2dPoint p2 = new On2dPoint(s[1], t[1]);
        On2dPoint p3 = new On2dPoint(s[0], t[1]);

        OnBrepFace face = brep.NewFace(si);
        OnBrepLoop loop = brep.NewLoop(IOnBrepLoop.TYPE.outer, ref face);

        loop.m_pbox.m_min.x = s[0];
        loop.m_pbox.m_min.y = t[0];
        loop.m_pbox.m_min.z = 0.0;

        loop.m_pbox.m_max.x = s[1];
        loop.m_pbox.m_max.y = t[1];
        loop.m_pbox.m_max.z = 0.0;

        // south side of surface
        c2i = brep.AddTrimCurve(new OnLineCurve(p0, p1));
        OnBrepTrim trim0 = brep.NewTrim(ref e0, f[fi].bRev[0], ref loop, c2i);
        trim0.set_m_tolerance(0, 0.0);
        trim0.set_m_tolerance(1, 0.0);
        trim0.m_type = (trim0.get_m_vi(0) != trim0.get_m_vi(1)) ? IOnBrepTrim.TYPE.mated : IOnBrepTrim.TYPE.singular;
        trim0.m_iso = IOnSurface.ISO.S_iso;

        // east side of surface
        c2i = brep.AddTrimCurve(new OnLineCurve(p1, p2));
        OnBrepTrim trim1 = brep.NewTrim(ref e1, f[fi].bRev[1], ref loop, c2i);
        trim1.set_m_tolerance(0, 0.0);
        trim1.set_m_tolerance(1, 0.0);
        trim1.m_type = (trim1.get_m_vi(0) != trim1.get_m_vi(1)) ? IOnBrepTrim.TYPE.mated : IOnBrepTrim.TYPE.singular;
        trim1.m_iso = IOnSurface.ISO.E_iso;

        // north side of surface
        c2i = brep.AddTrimCurve(new OnLineCurve(p2, p3));
        OnBrepTrim trim2 = brep.NewTrim(ref e2, f[fi].bRev[2], ref loop, c2i);
        trim2.set_m_tolerance(0, 0.0);
        trim2.set_m_tolerance(1, 0.0);
        trim2.m_type = (trim2.get_m_vi(0) != trim2.get_m_vi(1)) ? IOnBrepTrim.TYPE.mated : IOnBrepTrim.TYPE.singular;
        trim2.m_iso = IOnSurface.ISO.N_iso;

        // west side of surface
        c2i = brep.AddTrimCurve(new OnLineCurve(p3, p0));
        OnBrepTrim trim3 = brep.NewTrim(ref e3, f[fi].bRev[3], ref loop, c2i);
        trim3.set_m_tolerance(0, 0.0);
        trim3.set_m_tolerance(1, 0.0);
        trim3.m_type = (trim3.get_m_vi(0) != trim3.get_m_vi(1)) ? IOnBrepTrim.TYPE.mated : IOnBrepTrim.TYPE.singular;
        trim3.m_iso = IOnSurface.ISO.W_iso;
      }

      if (!brep.IsValid())
        return null;

      return brep;
    }
  }
}

