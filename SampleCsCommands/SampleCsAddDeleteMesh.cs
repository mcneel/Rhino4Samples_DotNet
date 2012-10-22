using RMA.Rhino;
using RMA.OpenNURBS;

namespace SampleCsCommands
{
  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// </summary>
  public class SampleCsAddDeleteMesh : RMA.Rhino.MRhinoCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{4502c553-4bea-433f-abc9-d9da26526c01}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "SampleCsAddDeleteMesh";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      On3dPoint center = new On3dPoint(OnUtil.On_origin);
      OnSphere sphere = new OnSphere(center, 5.0);
      OnMesh mesh = RhUtil.RhinoMeshSphere(sphere, 10, 10);
      MRhinoMeshObject mesh_obj = context.m_doc.AddMeshObject(mesh);
      context.m_doc.Redraw();

      MRhinoGetString gs = new MRhinoGetString();
      gs.SetCommandPrompt("Press <Enter> to continue");
      gs.AcceptNothing();
      gs.GetString();

      context.m_doc.DeleteObject(new MRhinoObjRef(mesh_obj));
      context.m_doc.Redraw();

      return IRhinoCommand.result.success;
    }
  }
}

