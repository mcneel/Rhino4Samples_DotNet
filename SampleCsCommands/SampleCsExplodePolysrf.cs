using RMA.Rhino;
using RMA.OpenNURBS;

namespace SampleCsCommands
{
  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// </summary>
  public class SampleCsExplodePolysrf : RMA.Rhino.MRhinoCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{037a3f23-22da-426e-b2a1-37996b8250fa}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "SampleCsExplodePolysrf";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      MRhinoGetObject go = new MRhinoGetObject();
      go.SetCommandPrompt("Select polysurface to explode");
      go.SetGeometryFilter(IRhinoGetObject.GEOMETRY_TYPE_FILTER.polysrf_object);
      go.GetObjects(1, 1);
      if (go.CommandResult() != IRhinoCommand.result.success)
        return go.CommandResult();

      IOnBrep brep = go.Object(0).Brep();
      if (null == brep)
        return IRhinoCommand.result.failure;

      for (int i = 0; i < brep.m_F.Count(); i++)
      {
        OnBrep faceBrep = brep.DuplicateFace(i, true);
        if (null != faceBrep)
          context.m_doc.AddBrepObject(faceBrep);
      }

      context.m_doc.DeleteObject(go.Object(0));
      context.m_doc.Redraw();

      return IRhinoCommand.result.success;
    }
  }
}

