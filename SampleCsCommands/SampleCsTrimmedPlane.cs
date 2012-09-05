using RMA.Rhino;
using RMA.OpenNURBS;

namespace SampleCsCommands
{
  class SampleCsTrimmedPlaneHelper
  {
    // symbolic vertex index constants to make code more readable
    const int
      A = 0,
      B = 1,
      C = 2,
      D = 3,
      E = 4;

    // symbolic edge index constants to make code more readable
    const int
      AB = 0,
      BC = 1,
      AC = 2;

    // symbolic face index constants to make code more readable
    const int
      ABC_i = 0;

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
      CreateOneEdge(ref brep, A, C, AC);

    }

    private static OnCurve CreateTrimmingCurve(
                              OnSurface s,
                              int side  // 0 = SW to SE
      // 1 = SE to NE
      // 2 = NE to NW
      // 3 = NW to SW
                              )
    {
      // A trimming curve is a 2d curve whose image lies in the surface's domain.
      // The "active" portion of the surface is to the left of the trimming curve.
      // An outer trimming loop consists of a simple closed curve running 
      // counter-clockwise around the region it trims.
      // An inner trimming loop consists of a simple closed curve running 
      // clockwise around the region the hole.

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
        case 1: // diagonal
          from.x = u1; from.y = v0;
          to.x = (u0 + u1) / 2; to.y = v1;
          break;
        case 2: // diagonal
          from.x = (u0 + u1) / 2; from.y = v1;
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

    private static int MakeTrimmingLoop(ref OnBrep brep, // returns index of loop
         ref OnBrepFace face,  // face loop is on
         int v0, int v1, int v2, // Indices of corner vertices listed in A,B,C order
         int e0,     // index of first edge
         int e0_dir, // orientation of edge
         int e1,     // index second edgee
         int e1_dir, // orientation of edge
         int e2,     // index third edge
         int e2_dir  // orientation of edge
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

      for (int side = 0; side < 3; side++)
      {
        // side: 0=south, 1=east, 2=north, 3=west

        c2 = CreateTrimmingCurve(srf, side);

        //Add trimming curve to brep trmming curves array
        c2i = brep.m_C2.Count();
        brep.m_C2.Append(c2);

        switch (side)
        {
          case 0: // south
            ei = e0;
            bRev3d = (e0_dir == -1);
            iso = IOnSurface.ISO.S_iso;
            break;
          case 1: // diagonal
            ei = e1;
            bRev3d = (e1_dir == -1);
            iso = IOnSurface.ISO.not_iso;
            break;
          case 2: // diagonal
            ei = e2;
            bRev3d = (e2_dir == -1);
            iso = IOnSurface.ISO.not_iso;
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

    private static void MakeTrimmedFace(ref OnBrep brep,
                             int si,      // index of 3d surface
                             int s_dir,   // orientation of surface with respect to surfce
                             int v0, int v1, int v2, // Indices of corner vertices
                             int e0,     // index of first edge
                             int e0_dir,  // orientation of edge
                             int e1,     // index of second edge
                             int e1_dir,  // orientation of edge
                             int e2,     // index of third edge
                             int e2_dir  // orientation of edge
                               )
    {
      //Add new face to brep
      OnBrepFace face = brep.NewFace(si);

      //Create loop and trims for the face
      MakeTrimmingLoop(ref brep, ref face,
                    v0, v1, v2,
                    e0, e0_dir,
                    e1, e1_dir,
                    e2, e2_dir
                    );

      //Set face direction relative to surface direction
      face.m_bRev = (s_dir == -1);
    }

    private static void CreateFace(ref OnBrep brep, int si)
    {
      MakeTrimmedFace(ref brep,
        ABC_i,      // Index of face
        +1,         // orientation of surface with respect to surface
        A, B, C,    // Indices of vertices
        AB, +1,      // Side edge and its orientation with respect to
        // to the trimming curve.  (AB)
        BC, +1,      // Side edge and its orientation with respect to
        // to the trimming curve.  (BC)
        AC, -1       // Side edge and its orientation with respect to
        // to the trimming curve   (AC)
        );
    }

    public static OnBrep MakeTrimmedBrepFace()
    {
      // This example demonstrates how to construct a ON_Brep
      // with the topology shown below.
      //
      //
      //    E-------C--------D
      //    |       /\       | 
      //    |      /  \      |
      //    |     /    \     |
      //    |    e2      e1  |     
      //    |   /        \   |    
      //    |  /          \  |  
      //    | /            \ |  
      //    A-----e0-------->B
      //
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
      On3dPoint[] point = new On3dPoint[5];
      point[0] = new On3dPoint(0.0, 0.0, 0.0);  //point A = geometry for vertex 0 (and surface SW corner)
      point[1] = new On3dPoint(10.0, 0.0, 0.0); // point B = geometry for vertex 1 (and surface SE corner)
      point[2] = new On3dPoint(5.0, 10.0, 0.0); // point C = geometry for vertex 2
      point[3] = new On3dPoint(10.0, 10.0, 0.0);// point D (surface NE corner)
      point[4] = new On3dPoint(0.0, 10.0, 0.0); // point E (surface NW corner)

      // Build the brep        
      OnBrep brep = new OnBrep();

      // create four vertices of the outer edges
      int vi;
      for (vi = 0; vi < 3; vi++)
      {
        OnBrepVertex v = brep.NewVertex(point[vi]);
        v.m_tolerance = 0.0;// this simple example is exact - for models with
        // non-exact data, set tolerance as explained in
        // definition of ON_BrepVertex.
      }

      // Create 3d curve geometry - the orientations are arbitrarily chosen
      // so that the end vertices are in alphabetical order.
      brep.m_C3.Append(CreateLinearCurve(point[A], point[B])); // line AB
      brep.m_C3.Append(CreateLinearCurve(point[B], point[C])); // line BC
      brep.m_C3.Append(CreateLinearCurve(point[A], point[C])); // line CD

      // Create edge topology for each curve in the brep.
      CreateEdges(ref brep);

      // Create 3d surface geometry - the orientations are arbitrarily chosen so
      // that some normals point into the cube and others point out of the cube.
      brep.m_S.Append(CreateNurbsSurface(point[A], point[B], point[D], point[E])); // ABCD

      // Create face topology and 2d parameter space loops and trims.
      CreateFace(ref brep, ABC_i);

      return brep;
    }
  }

  ///////////////////////////////////////////////////////////////////////////
  ///////////////////////////////////////////////////////////////////////////

  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// </summary>
  public class SampleCsTrimmedPlane : RMA.Rhino.MRhinoCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{0a95b66d-ad2d-45d8-b308-d201f9837b21}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "SampleCsTrimmedPlane";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      IRhinoCommand.result rc = IRhinoCommand.result.nothing;
      OnBrep brep = SampleCsTrimmedPlaneHelper.MakeTrimmedBrepFace();

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

