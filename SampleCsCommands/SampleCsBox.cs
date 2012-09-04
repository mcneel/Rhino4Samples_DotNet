using RMA.Rhino;
using RMA.OpenNURBS;

namespace SampleCsCommands
{
  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// </summary>
  public class SampleCsBox : RMA.Rhino.MRhinoCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{a3df4686-8b98-47c5-b018-17570847dccd}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "SampleCsBox";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      // Select a curve object
      MRhinoGetObject go = new MRhinoGetObject();
      go.SetCommandPrompt("Select curve");
      go.SetGeometryFilter(IRhinoGetObject.GEOMETRY_TYPE_FILTER.curve_object);
      go.GetObjects(1, 1);
      if (go.CommandResult() != IRhinoCommand.result.success)
        return go.CommandResult();

      // Validate the selection
      IRhinoObject obj = go.Object(0).Object();
      if (null == obj)
        return IRhinoCommand.result.failure;

      // Get the active view
      MRhinoView view = RhUtil.RhinoApp().ActiveView();
      if (null == view)
        return IRhinoCommand.result.failure;

      // Get the construction plane from the active view
      OnPlane plane = new OnPlane(view.ActiveViewport().ConstructionPlane().m_plane);

      // Create a construction plane aligned bounding box
      OnBoundingBox bbox = new OnBoundingBox();
      IRhinoObject[] objs = new IRhinoObject[1] { obj };
      bool rc = RhUtil.RhinoGetTightBoundingBox(objs, ref bbox, false, plane);
      if (rc == false)
        return IRhinoCommand.result.failure;

      // Validate bounding box
      if (0 != bbox.IsDegenerate())
      {
        RhUtil.RhinoApp().Print("Curve's tight bounding box is degenerate.\n");
        return IRhinoCommand.result.nothing;
      }

      // ON_BrepBox wants 8 points defining the box corners
      // arranged in this order:
      //
      //          v7______________v6
      //           |\             |\
      //           | \            | \
      //           |  \ _____________\ 
      //           |   v4         |   v5
      //           |   |          |   |
      //           |   |          |   |
      //          v3---|---------v2   |
      //           \   |          \   |
      //            \  |           \  |
      //             \ |            \ |
      //              \v0____________\v1
      //
      On3dPoint[] box_corners = new On3dPoint[8];
      box_corners[0] = bbox.Corner(0, 0, 0);
      box_corners[1] = bbox.Corner(1, 0, 0);
      box_corners[2] = bbox.Corner(1, 1, 0);
      box_corners[3] = bbox.Corner(0, 1, 0);
      box_corners[4] = bbox.Corner(0, 0, 1);
      box_corners[5] = bbox.Corner(1, 0, 1);
      box_corners[6] = bbox.Corner(1, 1, 1);
      box_corners[7] = bbox.Corner(0, 1, 1);

      // Transform points to the world-xy plane
      OnXform p2w = new OnXform();
      p2w.ChangeBasis(plane, OnUtil.On_xy_plane);
      for (int i = 0; i < 8; i++)
        box_corners[i].Transform(p2w);

      // Make a brep box
      OnBrep brep = OnUtil.ON_BrepBox(box_corners);
      if (null != brep)
      {
        context.m_doc.AddBrepObject(brep);
        context.m_doc.Redraw();
      }

      return IRhinoCommand.result.success;
    }
  }
}

