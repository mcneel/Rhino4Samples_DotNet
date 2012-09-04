using RMA.Rhino;
using RMA.OpenNURBS;

namespace SampleCsCommands
{
  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// </summary>
  public class SampleCsPickSurfaceEdge : RMA.Rhino.MRhinoCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{fb574783-a1d1-410d-bd03-e6a78108d259}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "SampleCsPickSurfaceEdge";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      MRhinoGetObject go = new MRhinoGetObject();
      go.SetCommandPrompt("Select surface edge");
      go.SetGeometryFilter(IRhinoGetObject.GEOMETRY_TYPE_FILTER.edge_object);
      go.EnableSubObjectSelect();
      go.GetObjects(1, 1);
      if (go.CommandResult() != IRhinoCommand.result.success)
        return go.CommandResult();

      MRhinoObjRef obj_ref = go.Object(0);
      IOnBrepEdge brep_edge = obj_ref.Edge();
      if (null == brep_edge)
        return IRhinoCommand.result.failure;

      IOnBrep brep = brep_edge.Brep();
      if (null == brep)
        return IRhinoCommand.result.failure;

      // TODO: add functionality here...

      return IRhinoCommand.result.success;
    }
  }
}

