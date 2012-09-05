using RMA.Rhino;
using RMA.OpenNURBS;

namespace SampleCsCommands
{
  class SampleCsFaceWithHoleHelper
  {
    // symbolic vertex index constants to make code more readable
    const int
      A = 0,
      B = 1,
      C = 2,
      D = 3,

      E = 4,
      F = 5,
      G = 6,
      H = 7;

    // symbolic edge index constants to make code more readable
    const int
      AB = 0,
      BC = 1,
      CD = 2,
      AD = 3,

      EF = 4,
      FG = 5,
      GH = 6,
      EH = 7;

    // symbolic face index constants to make code more readable
    const int
      ABCD = 0;

    private static OnCurve CreateLinearCurve(On3dPoint from, On3dPoint to)
    {
      // creates a 3d line segment to be used as a 3d curve in a ON_Brep
      OnCurve c3d = new OnLineCurve(from, to);
      if (c3d != null)
        c3d.SetDomain(0.0, 10.0);

      return c3d;
    }

    private static OnSurface CreateNurbsSurface(On3dPoint SW, On3dPoint SE, On3dPoint NE, On3dPoint NW)
    {
      OnNurbsSurface pNurbsSurface = new OnNurbsSurface(
                                            3,     // dimension (>= 1)
                                            false, // not rational
                                            2,     // "u" order (>= 2)
                                            2,     // "v" order (>= 2)
                                            2,     // number of control vertices in "u" dir (>= order)
                                            2      // number of control vertices in "v" dir (>= order)
                                            );
      // corner CVs in counter clockwise order starting in the south west
      pNurbsSurface.SetCV(0, 0, SW);
      pNurbsSurface.SetCV(1, 0, SE);
      pNurbsSurface.SetCV(1, 1, NE);
      pNurbsSurface.SetCV(0, 1, NW);
      // "u" knots
      pNurbsSurface.SetKnot(0, 0, 0.0);
      pNurbsSurface.SetKnot(0, 1, 1.0);
      // "v" knots
      pNurbsSurface.SetKnot(1, 0, 0.0);
      pNurbsSurface.SetKnot(1, 1, 1.0);

      return pNurbsSurface;
    }

    private static void CreateOneEdge(ref OnBrep brep,
                             int vi0, // index of start vertex
                             int vi1, // index of end vertex
                             int c3i  // index of 3d curve
                             )
    {
      OnBrepVertex v0 = brep.m_V[vi0];
      OnBrepVertex v1 = brep.m_V[vi1];
      OnBrepEdge edge = brep.NewEdge(ref v0, ref v1, c3i);
      if (edge != null)
        edge.m_tolerance = 0.0;  // this simple example is exact - for models with
      // non-exact data, set tolerance as explained in
      // definition of ON_BrepEdge.
    }

    private static void CreateEdges(ref OnBrep brep)
    {
      // In this simple example, the edge indices exactly match the 3d
      // curve indices.  In general,the correspondence between edge and
      // curve indices can be arbitrary.  It is permitted for multiple
      // edges to use different portions of the same 3d curve.  The 
      // orientation of the edge always agrees with the natural 
      // parametric orientation of the curve.

      //outer edges
      // edge that runs from A to B
      CreateOneEdge(ref brep, A, B, AB);

      // edge that runs from B to C
      CreateOneEdge(ref brep, B, C, BC);

      // edge that runs from C to D
      CreateOneEdge(ref brep, C, D, CD);

      // edge that runs from A to D
      CreateOneEdge(ref brep, A, D, AD);

      //Inner Edges
      // edge that runs from E to F
      CreateOneEdge(ref brep, E, F, EF);

      // edge that runs from F to G
      CreateOneEdge(ref brep, F, G, FG);

      // edge that runs from G to H
      CreateOneEdge(ref brep, G, H, GH);

      // edge that runs from E to H
      CreateOneEdge(ref brep, E, H, EH);

    }

    private static OnCurve CreateOuterTrimmingCurve(
                              OnSurface s,
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

      //In cases when trim curve is not easily defined in surface domain, 
      //use ON_Surface::Pullback only be careful about curve direction to ensure
      //loop trims run anti-clockwise for outer loop and clockwise for inner loop.

      //In this case, trim curves lie on the four edges of the surface
      On2dPoint from = new On2dPoint();
      On2dPoint to = new On2dPoint();
      double u0 = double.NaN, u1 = double.NaN, v0 = double.NaN, v1 = double.NaN;

      s.GetDomain(0, ref u0, ref u1);
      s.GetDomain(1, ref v0, ref v1);

      switch (side)
      {
        case 0:  // SW to SE
          from.x = u0; from.y = v0;
          to.x = u1; to.y = v0;
          break;
        case 1: // SE to NE
          from.x = u1; from.y = v0;
          to.x = u1; to.y = v1;
          break;
        case 2: // NE to NW
          from.x = u1; from.y = v1;
          to.x = u0; to.y = v1;
          break;
        case 3: // NW to SW
          from.x = u0; from.y = v1;
          to.x = u0; to.y = v0;
          break;
        default:
          return null;
      }

      OnCurve c2d = new OnLineCurve(from, to);
      if (c2d != null)
        c2d.SetDomain(0.0, 1.0);
      return c2d;
    }

    private static int MakeOuterTrimmingLoop(ref OnBrep brep, // returns index of loop
         ref OnBrepFace face,  // face loop is on
         int vSWi, int vSEi, int vNEi, int vNWi, // Indices of corner vertices listed in SW,SE,NW,NE order
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
      OnSurface srf = brep.m_S[face.m_si];

      //Create new loop
      OnBrepLoop loop = brep.NewLoop(IOnBrepLoop.TYPE.outer, ref face);

      // Create trimming curves running counter clockwise around the surface's domain.
      // Note that trims of outer loops run counter clockwise while trims of inner loops (holes) run anti-clockwise.
      // Also note that when trims locate on surface N,S,E or W ends, then trim_iso becomes N_iso, S_iso, E_iso and W_iso respectfully.  
      // While if trim is parallel to surface N,S or E,W, then trim is becomes y_iso and x_iso respectfully.

      // Start at the south side
      OnCurve c2;
      int c2i, ei = 0;
      bool bRev3d = false;
      IOnSurface.ISO iso = IOnSurface.ISO.not_iso;

      for (int side = 0; side < 4; side++)
      {
        // side: 0=south, 1=east, 2=north, 3=west

        c2 = CreateOuterTrimmingCurve(srf, side);

        //Add trimming curve to brep trmming curves array
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

        //Create new trim topology that references edge, direction reletive to edge, loop and trim curve geometry
        OnBrepEdge edge = brep.m_E[ei];
        OnBrepTrim trim = brep.NewTrim(ref edge, bRev3d, ref loop, c2i);
        if (trim != null)
        {
          trim.m_iso = iso;
          trim.m_type = IOnBrepTrim.TYPE.boundary; // This one b-rep face, so all trims are boundary ones.
          trim.set_m_tolerance(0, 0.0); // This simple example is exact - for models with non-exact
          trim.set_m_tolerance(1, 0.0); // data, set tolerance as explained in definition of ON_BrepTrim.
        }
      }
      return loop.m_loop_index;
    }

    //Trim curves must run is clockwise direction
    private static OnCurve CreateInnerTrimmingCurve(
                  OnSurface s,
                  int side // 0 = near SE to SW
      // 1 = near SW to NW
      // 2 = near NW to NE
      // 3 = near NE to SE
                  )
    {
      // A trimming curve is a 2d curve whose image lies in the surface's domain.
      // The "active" portion of the surface is to the left of the trimming curve.
      // An inner trimming loop consists of a simple closed curve running 
      // clockwise around the region the hole.

      //In this case, trim curves lie with 0.2 domain distance from surface edge
      On2dPoint from = new On2dPoint();
      On2dPoint to = new On2dPoint();
      double u0 = double.NaN,
          u1 = double.NaN,
          v0 = double.NaN,
          v1 = double.NaN;

      s.GetDomain(0, ref u0, ref u1);
      s.GetDomain(1, ref v0, ref v1);

      double udis = 0.2 * (u1 - u0);
      double vdis = 0.2 * (v1 - v0);

      u0 += udis;
      u1 -= udis;
      v0 += vdis;
      v1 -= vdis;

      switch (side)
      {
        case 0:  // near SE to SW
          from.x = u1; from.y = v0;
          to.x = u0; to.y = v0;
          break;
        case 1: // near SW to NW
          from.x = u0; from.y = v0;
          to.x = u0; to.y = v1;
          break;
        case 2: // near NW to NE
          from.x = u0; from.y = v1;
          to.x = u1; to.y = v1;
          break;
        case 3: // near NE to SE
          from.x = u1; from.y = v1;
          to.x = u1; to.y = v0;
          break;
        default:
          return null;
      }

      OnCurve c2d = new OnLineCurve(from, to);
      if (c2d != null)
        c2d.SetDomain(0.0, 1.0);

      return c2d;
    }

    private static int MakeInnerTrimmingLoop(ref OnBrep brep, // returns index of loop
         ref OnBrepFace face,  // face loop is on
         int vSWi, int vSEi, int vNEi, int vNWi, // Indices of hole vertices
         int eSi,     // index of edge close to south side of surface
         int eS_dir,  // orientation of edge with respect to surface trim
         int eEi,     // index of edge close to east side of surface
         int eE_dir,  // orientation of edge with respect to surface trim
         int eNi,     // index of edge close to north side of surface
         int eN_dir,  // orientation of edge with respect to surface trim
         int eWi,     // index of edge close to west side of surface
         int eW_dir   // orientation of edge with respect to surface trim
                            )
    {
      OnSurface srf = brep.m_S[face.m_si];
      //Create new inner loop
      OnBrepLoop loop = brep.NewLoop(IOnBrepLoop.TYPE.inner, ref face);

      // Create trimming curves running counter clockwise around the surface's domain.
      // Note that trims of outer loops run counter clockwise while trims of inner loops (holes) run clockwise.
      // Also note that when trims locate on surface N,S,E or W ends, then trim_iso becomes N_iso, S_iso, E_iso and W_iso respectfully.  
      // While if trim is parallel to surface N,S or E,W, then trim iso becomes y_iso and x_iso respectfully. 
      // All other cases, iso is set to not_iso

      // Start near the south side
      OnCurve c2;
      int c2i, ei = 0;
      bool bRev3d = false;
      IOnSurface.ISO iso = IOnSurface.ISO.not_iso;

      for (int side = 0; side < 4; side++)
      {
        // side: 0=near south(y_iso), 1=near west(x_iso), 2=near north(y_iso), 3=near east(x_iso)

        //Create trim 2d curve
        c2 = CreateInnerTrimmingCurve(srf, side);

        //Add trimming curve to brep trmming curves array
        c2i = brep.m_C2.Count();
        brep.m_C2.Append(c2);

        switch (side)
        {
          case 0: // near south
            ei = eSi;
            bRev3d = (eS_dir == -1);
            iso = IOnSurface.ISO.y_iso;
            break;
          case 1: // near west
            ei = eEi;
            bRev3d = (eE_dir == -1);
            iso = IOnSurface.ISO.x_iso;
            break;
          case 2: // near north
            ei = eNi;
            bRev3d = (eN_dir == -1);
            iso = IOnSurface.ISO.y_iso;
            break;
          case 3: // near east
            ei = eWi;
            bRev3d = (eW_dir == -1);
            iso = IOnSurface.ISO.x_iso;
            break;
        }

        //Create new trim topology that references edge, direction reletive to edge, loop and trim curve geometry
        OnBrepEdge edge = brep.m_E[ei];
        OnBrepTrim trim = brep.NewTrim(ref edge, bRev3d, ref loop, c2i);
        if (trim != null)
        {
          trim.m_iso = iso;
          trim.m_type = IOnBrepTrim.TYPE.boundary; // This one b-rep face, so all trims are boundary ones.
          trim.set_m_tolerance(0, 0.0); // This simple example is exact - for models with non-exact
          trim.set_m_tolerance(1, 0.0); // data, set tolerance as explained in definition of ON_BrepTrim.
        }
      }

      return loop.m_loop_index;
    }

    private static void CreateFace(ref OnBrep brep, int si)
    {
      //Add new face to brep
      OnBrepFace face = brep.NewFace(si);

      //Create outer loop and trims for the face
      MakeOuterTrimmingLoop(ref brep, ref face,
                                A, B, C, D, // Indices of vertices listed in SW,SE,NW,NE order
                                AB, +1,      // South side edge and its orientation with respect to
        // to the trimming curve.  (AB)
                                BC, +1,      // East side edge and its orientation with respect to
        // to the trimming curve.  (BC)
                                CD, +1,      // North side edge and its orientation with respect to
        // to the trimming curve   (CD)
                                AD, -1       // West side edge and its orientation with respect to
        // to the trimming curve   (AD)
                        );


      //Create loop and trims for the face
      MakeInnerTrimmingLoop(ref brep, ref face,
                                E, F, G, H, // Indices of hole vertices
                                EF, +1,      // Parallel to south side edge and its orientation with respect to
        // to the trimming curve.  (EF)
                                FG, +1,      // Parallel to east side edge and its orientation with respect to
        // to the trimming curve.  (FG)
                                GH, +1,      // Parallel to north side edge and its orientation with respect to
        // to the trimming curve   (GH)
                                EH, -1       // Parallel to west side edge and its orientation with respect to
        // to the trimming curve   (EH)
                        );

      //Set face direction relative to surface direction
      face.m_bRev = false;
    }

    public static OnBrep MakeBrepFace()
    {
      // This example demonstrates how to construct a ON_Brep
      // with the topology shown below.
      //
      //
      //   D---------e2-----C      
      //   |                |     
      //   |  G----e6---H   |
      //   |  |         |   |
      //   e3 e5        e7  |
      //   |  |         |   |
      //   |  F<---e4---E   |
      //   |                |
      //   A-------e0------>B
      //
      //  Things need to be defined in a valid brep:
      //   1- Vertices
      //   2- 3D Curves (geometry)
      //   3- Edges (topology - reference curve geometry)
      //   4- Surface (geometry)
      //   5- Faces (topology - reference surface geometry)
      //   6- Loops (2D parameter space of faces)
      //   4- Trims and 2D curves (2D parameter space of edges)
      //

      //Vertex points
      // define the corners of the face with hole
      On3dPoint[] point = new On3dPoint[8];
      point[0] = new On3dPoint(0.0, 0.0, 0.0);
      point[1] = new On3dPoint(10.0, 0.0, 0.0);
      point[2] = new On3dPoint(10.0, 10.0, 0.0);
      point[3] = new On3dPoint(0.0, 10.0, 0.0);

      point[4] = new On3dPoint(8.0, 2.0, 0.0);
      point[5] = new On3dPoint(2.0, 2.0, 0.0);
      point[6] = new On3dPoint(2.0, 8.0, 0.0);
      point[7] = new On3dPoint(8.0, 8.0, 0.0);

      // Build the brep        
      OnBrep brep = new OnBrep();

      // create four vertices of the outer edges
      int vi;
      for (vi = 0; vi < 8; vi++)
      {
        OnBrepVertex v = brep.NewVertex(point[vi]);
        v.m_tolerance = 0.0; // this simple example is exact - for models with
        // non-exact data, set tolerance as explained in
        // definition of ON_BrepVertex.
      }

      // Create 3d curve geometry of the outer boundary 
      // The orientations are arbitrarily chosen
      // so that the end vertices are in alphabetical order.
      brep.m_C3.Append(CreateLinearCurve(point[A], point[B])); // line AB
      brep.m_C3.Append(CreateLinearCurve(point[B], point[C])); // line BC
      brep.m_C3.Append(CreateLinearCurve(point[C], point[D])); // line CD
      brep.m_C3.Append(CreateLinearCurve(point[A], point[D])); // line AD

      // Create 3d curve geometry of the hole 
      brep.m_C3.Append(CreateLinearCurve(point[E], point[F])); // line EF
      brep.m_C3.Append(CreateLinearCurve(point[F], point[G])); // line GH
      brep.m_C3.Append(CreateLinearCurve(point[G], point[H])); // line HI
      brep.m_C3.Append(CreateLinearCurve(point[E], point[H])); // line EI

      // Create edge topology for each curve in the brep.
      CreateEdges(ref brep);

      // Create 3d surface geometry - the orientations are arbitrarily chosen so
      // that some normals point into the cube and others point out of the cube.
      brep.m_S.Append(CreateNurbsSurface(point[A], point[B], point[C], point[D])); // ABCD

      // Create face topology and 2d parameter space loops and trims.
      CreateFace(ref brep, ABCD);

      return brep;
    }
  }

  ///////////////////////////////////////////////////////////////////////////
  ///////////////////////////////////////////////////////////////////////////

  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// </summary>
  public class SampleCsFaceWithHole : RMA.Rhino.MRhinoCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{eef30b51-4063-47b9-9a77-50c8c6502b9f}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "SampleCsFaceWithHole";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      IRhinoCommand.result rc = IRhinoCommand.result.nothing;
      OnBrep brep = SampleCsFaceWithHoleHelper.MakeBrepFace();

      if (brep != null)
      {
        if (context.m_doc.AddBrepObject(brep) != null)
        {
          context.m_doc.Redraw();
          rc = IRhinoCommand.result.success;
        }
        else
        {
          rc = IRhinoCommand.result.failure;
        }
      }
      return rc;
    }
  }
}

