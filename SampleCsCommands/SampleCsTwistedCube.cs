using RMA.Rhino;
using RMA.OpenNURBS;

namespace SampleCsCommands
{
  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// </summary>
  public class SampleCsTwistedCube : RMA.Rhino.MRhinoCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{30a3a4aa-082d-4fcf-b829-46417555f219}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "SampleCsTwistedCube";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      IRhinoCommand.result rc = IRhinoCommand.result.failure;

      OnBrep brep = MakeTwistedCube();
      if (null != brep && brep.IsValid())
      {
        context.m_doc.AddBrepObject(brep);
        context.m_doc.Redraw();
        rc = IRhinoCommand.result.success;
      }

      return rc;
    }

    // Symbolic vertex index constants to make code more readable
    static readonly int 
      A = 0,
      B = 1,
      C = 2,
      D = 3,
      E = 4,
      F = 5,
      G = 6,
      H = 7;

    // Symbolic edge index constants to make code more readable
    static readonly int
      AB =  0,
      BC =  1,
      CD =  2,
      AD =  3,
      EF =  4,
      FG =  5,
      GH =  6,
      EH =  7,
      AE =  8,
      BF =  9,
      CG = 10,
      DH = 11;

    // Symbolic face index constants to make code more readable
    static readonly int
      ABCD =  0,
      BCGF =  1,
      CDHG =  2,
      ADHE =  3,
      ABFE =  4,
      EFGH =  5;

    /// <summary>
    /// TwistedCubeTrimmingCurve
    /// </summary>
    static OnCurve TwistedCubeTrimmingCurve(
      IOnSurface s,
      int side // 0 = SW to SE
               // 1 = SE to NE
               // 2 = NE to NW
               // 3 = NW to SW
      )
    {
      // A trimming curve is a 2d curve whose image lies in the surface's domain.
      // The "active" portion of the surface is to the left of the trimming curve.
      // An outer trimming loop consists of a simple closed curve running 
      // counter-clockwise around the region it trims.

      On2dPoint from = new On2dPoint();
      On2dPoint to = new On2dPoint();
      double u0 = 0.0, u1 = 0.0, v0 = 0.0, v1 = 0.0;

      s.GetDomain(0, ref u0, ref u1);
      s.GetDomain(1, ref v0, ref v1);

      switch (side)
      {
      case 0:  // SW to SE
        from.x = u0; from.y = v0;
        to.x   = u1; to.y   = v0;
        break;
      case 1: // SE to NE
        from.x = u1; from.y = v0;
        to.x   = u1; to.y   = v1;
        break;
      case 2: // NE to NW
        from.x = u1; from.y = v1;
        to.x   = u0; to.y   = v1;
        break;
      case 3: // NW to SW
        from.x = u0; from.y = v1;
        to.x   = u0; to.y   = v0;
        break;
      default:
        return null;
      }

      OnLineCurve c2d = new OnLineCurve(from, to);
      c2d.SetDomain(0.0, 1.0);

      return c2d;
    }

    /// <summary>
    /// TwistedCubeEdgeCurve
    /// </summary>
    static OnCurve TwistedCubeEdgeCurve(
      IOn3dPoint from, 
      IOn3dPoint to
      )
    {
      // Creates a 3d line segment to be used as a 3d curve in a OnBrep
      OnLineCurve c3d = new OnLineCurve(from, to);
      c3d.SetDomain(0.0, 1.0);
      return c3d;
    }

    /// <summary>
    /// TwistedCubeSideSurface
    /// </summary>
    static OnSurface TwistedCubeSideSurface( 
      IOn3dPoint SW, 
      IOn3dPoint SE,
      IOn3dPoint NE,
      IOn3dPoint NW
      )
    {
      OnNurbsSurface ns = new OnNurbsSurface(
        3,     // dimension
        false, // not rational
        2,     // "u" order
        2,     // "v" order
        2,     // number of control vertices in "u" dir
        2      // number of control vertices in "v" dir
        );

      // Corner CVs in counter clockwise order starting in the south west
      ns.SetCV(0, 0, SW);
      ns.SetCV(1, 0, SE);
      ns.SetCV(1, 1, NE);
      ns.SetCV(0, 1, NW);

      // "u" knots
      ns.SetKnot(0, 0, 0.0);
      ns.SetKnot(0, 1, 1.0);

      // "v" knots
      ns.SetKnot(1, 0, 0.0);
      ns.SetKnot(1, 1, 1.0);

      return ns;
    }

    /// <summary>
    /// MakeTwistedCubeEdge
    /// </summary>
    static void MakeTwistedCubeEdge(
      ref OnBrep brep,
      int vi0, // index of start vertex
      int vi1, // index of end vertex
      int c3i  // index of 3d curve
      )
    {
      OnBrepVertex v0 = brep.m_V[vi0];
      OnBrepVertex v1 = brep.m_V[vi1];
      OnBrepEdge edge = brep.NewEdge(ref v0, ref v1, c3i);
      edge.m_tolerance = 0.0;  // this simple example is exact - for models with
                               // non-exact data, set tolerance as explained in
                               // definition of OnBrepEdge.
    }

    /// <summary>
    /// MakeTwistedCubeEdges
    /// </summary>
    static void MakeTwistedCubeEdges(
      ref OnBrep brep
      )
    {

      // In this simple example, the edge indices exactly match the 3d
      // curve indices.  In general,the correspondence between edge and
      // curve indices can be arbitrary.  It is permitted for multiple
      // edges to use different portions of the same 3d curve.  The 
      // orientation of the edge always agrees with the natural 
      // parametric orientation of the curve.
      
      // Edge that runs from A to B
      MakeTwistedCubeEdge(ref brep, A, B, AB);
      
      // Edge that runs from B to C
      MakeTwistedCubeEdge(ref brep, B, C, BC);

      // Edge that runs from C to D
      MakeTwistedCubeEdge(ref brep, C, D, CD);

      // Edge that runs from A to D
      MakeTwistedCubeEdge(ref brep, A, D, AD);

      // Edge that runs from E to F
      MakeTwistedCubeEdge(ref brep, E, F, EF);

      // Edge that runs from F to G
      MakeTwistedCubeEdge(ref brep, F, G, FG);

      // Edge that runs from G to H
      MakeTwistedCubeEdge(ref brep, G, H, GH);

      // Edge that runs from E to H
      MakeTwistedCubeEdge(ref brep, E, H, EH);

      // Edge that runs from A to E
      MakeTwistedCubeEdge(ref brep, A, E, AE);

      // Edge that runs from B to F
      MakeTwistedCubeEdge(ref brep, B, F, BF);

      // Edge that runs from C to G
      MakeTwistedCubeEdge(ref brep, C, G, CG);

      // Edge that runs from D to H
      MakeTwistedCubeEdge(ref brep, D, H, DH);
    }

    /// <summary>
    /// MakeTwistedCubeTrimmingLoop
    /// </summary>
    static int MakeTwistedCubeTrimmingLoop(
      ref OnBrep brep, // returns index of loop
      ref OnBrepFace face,  // face loop is on
      int vSWi, int vSEi, int vNEi, int vNWi, // Indices of corner vertices listed in SW, SE, NW, NE order
      int eSi,     // index of edge on south side of surface
      int eS_dir,  // orientation of edge with respect to surface trim
      int eEi,     // index of edge on south side of surface
      int eE_dir,  // orientation of edge with respect to surface trim
      int eNi,     // index of edge on south side of surface
      int eN_dir,  // orientation of edge with respect to surface trim
      int eWi,     // index of edge on south side of surface
      int eW_dir   // orientation of edge with respect to surface trim
                                   )
    {
      IOnSurface srf = brep.m_S[face.m_si];

      OnBrepLoop loop = brep.NewLoop(IOnBrepLoop.TYPE.outer, ref face);

      // Create trimming curves running counter clockwise around the surface's domain.
      // Start at the south side
      int c2i = 0, ei = 0;
      bool bRev3d = false;
      IOnSurface.ISO iso = IOnSurface.ISO.not_iso;

      for (int side = 0; side < 4; side++ )
      {
        // side: 0=south, 1=east, 2=north, 3=west
        OnCurve c2 = TwistedCubeTrimmingCurve(srf, side);
        c2i = brep.m_C2.Count();
        brep.m_C2.Append(c2);

        switch (side)
        {
        case 0: // south
          ei = eSi;
          bRev3d = (eS_dir == -1);
          iso = IOnSurface.ISO.S_iso;
          break;
        case 1: // east
          ei = eEi;
          bRev3d = (eE_dir == -1);
          iso = IOnSurface.ISO.E_iso;
          break;
        case 2: // north
          ei = eNi;
          bRev3d = (eN_dir == -1);
          iso = IOnSurface.ISO.N_iso;
          break;
        case 3: // west
          ei = eWi;
          bRev3d = (eW_dir == -1);
          iso = IOnSurface.ISO.W_iso;
          break;
        }

        OnBrepEdge edge = brep.m_E[ei];
        OnBrepTrim trim = brep.NewTrim(ref edge, bRev3d, ref loop, c2i);
        trim.m_iso = iso;
        trim.m_type = IOnBrepTrim.TYPE.mated; // This b-rep is closed, so all trims have mates.
        trim.set_m_tolerance(0, 0.0); // This simple example is exact - for models with
        trim.set_m_tolerance(1, 0.0); // non-exact data, set tolerance as explained in
                                      // definition of OnBrepTrim.
      }

      return loop.m_loop_index;
    }

    /// <summary>
    /// MakeTwistedCubeFace
    /// </summary>
    static void MakeTwistedCubeFace( 
      ref OnBrep brep,
      int si,      // index of 3d surface
      int s_dir,   // orientation of surface with respect to brep
      int vSWi, int vSEi, int vNEi, int vNWi, // Indices of corner vertices listed in SW, SE, NW, NE order
      int eSi,     // index of edge on south side of surface
      int eS_dir,  // orientation of edge with respect to surface trim
      int eEi,     // index of edge on south side of surface
      int eE_dir,  // orientation of edge with respect to surface trim
      int eNi,     // index of edge on south side of surface
      int eN_dir,  // orientation of edge with respect to surface trim
      int eWi,     // index of edge on south side of surface
      int eW_dir   // orientation of edge with respect to surface trim
      )
    {
      OnBrepFace face = brep.NewFace(si);

      MakeTwistedCubeTrimmingLoop(
        ref brep,
        ref face,
        vSWi, vSEi, vNEi, vNWi, 
        eSi, eS_dir, 
        eEi, eE_dir, 
        eNi, eN_dir, 
        eWi, eW_dir 
        );

      face.m_bRev = (s_dir == -1);
    }

    /// <summary>
    /// MakeTwistedCubeFaces
    /// </summary>
    static void MakeTwistedCubeFaces(
      ref OnBrep brep 
      )
    {
      MakeTwistedCubeFace(ref brep,
        ABCD,       // Index of surface ABCD
        +1,         // orientation of surface with respect to brep
        A, B, C, D, // Indices of vertices listed in SW,SE,NW,NE order
        AB,+1,      // South side edge and its orientation with respect to
                    // to the trimming curve.  (AB)
        BC,+1,      // South side edge and its orientation with respect to
                    // to the trimming curve.  (BC)
        CD,+1,      // South side edge and its orientation with respect to
                    // to the trimming curve   (CD)
        AD,-1       // South side edge and its orientation with respect to
                    // to the trimming curve   (AD)
        );

      MakeTwistedCubeFace(ref brep,
        BCGF,       // Index of surface BCGF
        -1,         // orientation of surface with respect to brep
        B, C, G, F, // Indices of vertices listed in SW,SE,NW,NE order
        BC,+1,      // South side edge and its orientation with respect to
                    // to the trimming curve.  (BC)
        CG,+1,      // South side edge and its orientation with respect to
                    // to the trimming curve.  (CG)
        FG,-1,      // South side edge and its orientation with respect to
                    // to the trimming curve   (FG)
        BF,-1       // South side edge and its orientation with respect to
                    // to the trimming curve   (BF)
        );

      MakeTwistedCubeFace(ref brep,
        CDHG,       // Index of surface CDHG
        -1,         // orientation of surface with respect to brep
        C, D, H, G, // Indices of vertices listed in SW,SE,NW,NE order
        CD,+1,      // South side edge and its orientation with respect to
                    // to the trimming curve.  (CD)
        DH,+1,      // South side edge and its orientation with respect to
                    // to the trimming curve.  (DH)
        GH,-1,      // South side edge and its orientation with respect to
                    // to the trimming curve   (GH)
        CG,-1       // South side edge and its orientation with respect to
                    // to the trimming curve   (CG)
        );

      MakeTwistedCubeFace(ref brep,
        ADHE,       // Index of surface ADHE
        +1,         // orientation of surface with respect to brep
        A, D, H, E, // Indices of vertices listed in SW,SE,NW,NE order
        AD,+1,      // South side edge and its orientation with respect to
                    // to the trimming curve.  (AD)
        DH,+1,      // South side edge and its orientation with respect to
                    // to the trimming curve.  (DH)
        EH,-1,      // South side edge and its orientation with respect to
                    // to the trimming curve   (EH)
        AE,-1       // South side edge and its orientation with respect to
                    // to the trimming curve   (AE)
        );

      MakeTwistedCubeFace(ref brep,
        ABFE,       // Index of surface ABFE
        -1,         // orientation of surface with respect to brep
        A, B, F, E, // Indices of vertices listed in SW,SE,NW,NE order
        AB,+1,      // South side edge and its orientation with respect to
                    // to the trimming curve.  (AB)
        BF,+1,      // South side edge and its orientation with respect to
                    // to the trimming curve.  (BF)
        EF,-1,      // South side edge and its orientation with respect to
                    // to the trimming curve   (EF)
        AE,-1       // South side edge and its orientation with respect to
                    // to the trimming curve   (AE)
        );

      MakeTwistedCubeFace(ref brep,
        EFGH,       // Index of surface EFGH
        -1,         // orientation of surface with respect to brep
        E, F, G, H, // Indices of vertices listed in SW,SE,NW,NE order
        EF,+1,      // South side edge and its orientation with respect to
                    // to the trimming curve.  (EF)
        FG,+1,      // South side edge and its orientation with respect to
                    // to the trimming curve.  (FG)
        GH,+1,      // South side edge and its orientation with respect to
                    // to the trimming curve   (GH)
        EH,-1       // South side edge and its orientation with respect to
                    // to the trimming curve   (EH)
        );
    }

    /// <summary>
    /// The one and only MakeTwistedCube
    /// </summary>
    static OnBrep MakeTwistedCube()
    {
      /*
      This example demonstrates how to construct a OnBrep
      with the topology shown below.

                 H-------e6-------G
                /                /|
               / |              / |
              /  e7            /  e5
             /   |            /   |
            /                e10  |
           /     |          /     |
          e11    E- - e4- -/- - - F
         /                /      /
        /      /         /      /
       D---------e2-----C      e9
       |     /          |     /
       |    e8          |    /
       e3  /            e1  /
       |                |  /
       | /              | /
       |                |/
       A-------e0-------B

      */

      On3dPoint[] points = new On3dPoint[8];
      points[0] = new On3dPoint(0.0, 0.0, 0.0);   // point A = geometry for vertex 0
      points[1] = new On3dPoint(10.0, 0.0, 0.0);  // point B = geometry for vertex 1
      points[2] = new On3dPoint(10.0, 8.0, -1.0); // point C = geometry for vertex 2
      points[3] = new On3dPoint(0.0, 6.0, 0.0);   // point D = geometry for vertex 3
      points[4] = new On3dPoint(1.0, 2.0, 11.0);  // point E = geometry for vertex 4
      points[5] = new On3dPoint(10.0, 0.0, 12.0); // point F = geometry for vertex 5
      points[6] = new On3dPoint(10.0, 7.0, 13.0); // point G = geometry for vertex 6
      points[7] = new On3dPoint(0.0, 6.0, 12.0);  // point H = geometry for vertex 7

      OnBrep brep = new OnBrep();

      // Create eight vertices located at the eight points
      int vi;
      for (vi = 0; vi < 8; vi++)
      {
        OnBrepVertex v = brep.NewVertex(points[vi]);
        v.m_tolerance = 0.0;
      }

      // Create 3d curve geometry - the orientations are arbitrarily chosen
      // so that the end vertices are in alphabetical order.
      brep.m_C3.Append(TwistedCubeEdgeCurve(points[A], points[B])); // line AB
      brep.m_C3.Append(TwistedCubeEdgeCurve(points[B], points[C])); // line BC
      brep.m_C3.Append(TwistedCubeEdgeCurve(points[C], points[D])); // line CD
      brep.m_C3.Append(TwistedCubeEdgeCurve(points[A], points[D])); // line AD
      brep.m_C3.Append(TwistedCubeEdgeCurve(points[E], points[F])); // line EF
      brep.m_C3.Append(TwistedCubeEdgeCurve(points[F], points[G])); // line FG
      brep.m_C3.Append(TwistedCubeEdgeCurve(points[G], points[H])); // line GH
      brep.m_C3.Append(TwistedCubeEdgeCurve(points[E], points[H])); // line EH
      brep.m_C3.Append(TwistedCubeEdgeCurve(points[A], points[E])); // line AE
      brep.m_C3.Append(TwistedCubeEdgeCurve(points[B], points[F])); // line BF
      brep.m_C3.Append(TwistedCubeEdgeCurve(points[C], points[G])); // line CG
      brep.m_C3.Append(TwistedCubeEdgeCurve(points[D], points[H])); // line DH

      // Create the 12 edges that connect the corners of the cube.
      MakeTwistedCubeEdges(ref brep);

      // Create 3d surface geometry - the orientations are arbitrarily chosen so
      // that some normals point into the cube and others point out of the cube.
      brep.m_S.Append(TwistedCubeSideSurface(points[A], points[B], points[C], points[D])); // ABCD
      brep.m_S.Append(TwistedCubeSideSurface(points[B], points[C], points[G], points[F])); // BCGF
      brep.m_S.Append(TwistedCubeSideSurface(points[C], points[D], points[H], points[G])); // CDHG
      brep.m_S.Append(TwistedCubeSideSurface(points[A], points[D], points[H], points[E])); // ADHE
      brep.m_S.Append(TwistedCubeSideSurface(points[A], points[B], points[F], points[E])); // ABFE
      brep.m_S.Append(TwistedCubeSideSurface(points[E], points[F], points[G], points[H])); // EFGH

      // Create the CRhinoBrepFaces
      MakeTwistedCubeFaces(ref brep);

      if (!brep.IsValid())
        return null;

      return brep;
    }
  }
}

