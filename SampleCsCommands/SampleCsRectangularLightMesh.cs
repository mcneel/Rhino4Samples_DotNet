using RMA.Rhino;
using RMA.OpenNURBS;

namespace SampleCsCommands
{
  ///<summary>
  /// A Rhino.NET plug-in can contain as many MRhinoCommand derived classes as it wants.
  /// DO NOT create an instance of this class (this is the responsibility of Rhino.NET.)
  /// </summary>
  public class SampleCsRectangularLightMesh : RMA.Rhino.MRhinoCommand
  {
    ///<summary>
    /// Rhino tracks commands by their unique ID. Every command must have a unique id.
    /// The Guid created by the project wizard is unique. You can create more Guids using
    /// the "Create Guid" tool in the Tools menu.
    ///</summary>
    ///<returns>The id for this command</returns>
    public override System.Guid CommandUUID()
    {
      return new System.Guid("{314c5dae-67d8-41fa-93e7-451914e95525}");
    }

    ///<returns>The command name as it appears on the Rhino command line</returns>
    public override string EnglishCommandName()
    {
      return "SampleCsRectangularLightMesh";
    }

    ///<summary> This gets called when when the user runs this command.</summary>
    public override IRhinoCommand.result RunCommand(IRhinoCommandContext context)
    {
      MRhinoGetObject go = new MRhinoGetObject();
      go.SetCommandPrompt("Select rectangular light");
      go.SetGeometryFilter(IRhinoGetObject.GEOMETRY_TYPE_FILTER.light_object);
      go.GetObjects(1, 1);
      if (go.CommandResult() != IRhinoCommand.result.success)
        return go.CommandResult();

      IOnLight light = go.Object(0).Light();
      if (null == light)
        return IRhinoCommand.result.failure;

      if (!light.IsRectangularLight())
      {
        RhUtil.RhinoApp().Print("Not a rectangular light.\n");
        return IRhinoCommand.result.nothing;
      }

      On3dPoint origin = light.Location();
      On3dVector xaxis = light.Length();
      On3dVector yaxis = light.Width();

      OnPlane plane = new OnPlane(origin, xaxis, yaxis);
      OnInterval x_interval = new OnInterval(0.0, xaxis.Length());
      OnInterval y_interval = new OnInterval(0.0, yaxis.Length());

      OnMesh mesh = RhUtil.RhinoMeshPlane(plane, x_interval, y_interval, 2, 2);
      if (null != mesh)
      {
        mesh.ConvertQuadsToTriangles();
        context.m_doc.AddMeshObject(mesh);
        context.m_doc.Redraw();
      }

      return IRhinoCommand.result.cancel;
    }
  }
}

